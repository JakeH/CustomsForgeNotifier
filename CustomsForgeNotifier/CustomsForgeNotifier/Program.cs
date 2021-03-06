﻿using DuoVia.FuzzyStrings;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CustomsForgeNotifier
{
    internal class Program
    {
        private static readonly ILog Logger = LogHelper.GetLog();
        private static readonly INotifier Notifier;

        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Logger.Error("Unhandled exception", (e.ExceptionObject as Exception));
            };

            if (!string.IsNullOrWhiteSpace(Settings.Notifier))
            {
                switch (Settings.Notifier.ToLower().Trim())
                {
                    case "pushbullet":
                        Notifier = new PushbulletNotifier(
                            Settings.Pushbullet.APIToken,
                            Settings.Pushbullet.APIUri,
                            Settings.Pushbullet.DeviceIden);
                        break;
                }
            }
        }

        private static void Main(string[] args)
        {
            PerformTasks();
        }

        private static void PerformTasks()
        {
            DateTime newLastUpdated = DateTime.MinValue;
            DateTime lastUpdated = Settings.LastEntryUpdated;
            int processedEntryCount = 0;

            Logger.InfoFormat("Starting with last updated of '{0}'", lastUpdated);

            FileInfo artistsFile = new FileInfo("ArtistsToMatch.txt");
            if (!artistsFile.Exists)
            {
                Logger.ErrorFormat("Cannot find an 'artists to match' file at: '{0}'", artistsFile.FullName);
                return;
            }

            List<string> artistsToMatch = new List<string>(File.ReadAllLines(artistsFile.FullName, Encoding.UTF8));

            Queue<ForgeEntry> matchesToNotify = new Queue<ForgeEntry>();

            foreach (var entry in DataRequester.GetForgeEntries(lastUpdated, Settings.AbsoluteRetrievalLimit))
            {
                processedEntryCount++;

                // update the lastest time, which we will save in the settings file
                if (entry.UpdatedAt > newLastUpdated)
                    newLastUpdated = entry.UpdatedAt;

                foreach (string artist in artistsToMatch)
                {
                    // seems a coefficient of 0.95 or greater does a good job at catching simple errors
                    if (artist.DiceCoefficient(entry.SortArtistName) >= 0.95)
                    {
                        matchesToNotify.Enqueue(entry);

                        Logger.InfoFormat("Matched entry '{0}' with watched artist '{1}'",
                            entry.SortArtistName, artist);

                        break;
                    }
                }
            }

            // save the new last entry updated date
            if (newLastUpdated > Settings.LastEntryUpdated)
                Settings.LastEntryUpdated = newLastUpdated;

            Logger.InfoFormat("Processed {0} entries, matching {1}", processedEntryCount, matchesToNotify.Count);

            if (Notifier != null)
            {
                // process notifications
                while (matchesToNotify.Count > 0)
                {
                    ForgeEntry match = matchesToNotify.Dequeue();

                    StringBuilder message = new StringBuilder();

                    message.AppendFormat("{0}{1} - {2}\n\n",
                        match.IsOfficial ? "[" + Resources.NotifyStrings.Official + "] " : string.Empty,
                        match.ArtistName, match.SongName);

                    message.AppendFormat("{0}: {1}\n\n", Resources.NotifyStrings.InfoUri, match.InformationUri);

                    // only include the download link if the release is not official
                    if (!match.IsOfficial)
                        message.AppendFormat("{0}: {1}", Resources.NotifyStrings.DownloadUri, match.DownloadUri);

                    Notifier.Notify(Resources.NotifyStrings.Title, message.ToString());

                    // rate limit this
                    Thread.Sleep(1000);
                }
            }

            Logger.Info("Finished");
        }
    }
}