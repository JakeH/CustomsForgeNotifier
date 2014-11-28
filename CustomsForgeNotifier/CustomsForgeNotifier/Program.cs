using CustomsForgeNotifier.Properties;
using DuoVia.FuzzyStrings;
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

            switch (Settings.Default.Notifier.ToLower().Trim())
            {
                case "pushbullet":
                    Notifier = new PushbulletNotifier(Settings.Default.PushbulletAPIToken, Settings.Default.PushbulletAPIUri);
                    break;
            }
        }

        private static void Main(string[] args)
        {
            PerformTasks();
        }

        private static void PerformTasks()
        {
            DateTime newLastUpdated = DateTime.MinValue;
            DateTime lastUpdated = Settings.Default.LastEntryUpdated;

            Logger.InfoFormat("Starting with last updated of '{0}'", lastUpdated);

            //TODO: upgrade this poor man's database.
            List<string> ArtistsToMatch = new List<string>(File.ReadAllLines("ArtistsToMatch.txt"));

            Queue<ForgeEntry> MatchesToNotify = new Queue<ForgeEntry>();

            foreach (var entry in DataRequester.GetForgeEntries(lastUpdated, Settings.Default.AbsoluteRetrievalLimit))
            {
                // update the lastest time, which we will save in the settings file
                newLastUpdated = (entry.UpdatedAt > newLastUpdated) ? entry.UpdatedAt : newLastUpdated;

                foreach (string artist in ArtistsToMatch)
                {
                    // seems a coefficient of 0.95 or greater does a good job at catching simple errors
                    if (artist.DiceCoefficient(entry.SortArtistName) >= 0.95)
                    {
                        MatchesToNotify.Enqueue(entry);

                        Logger.InfoFormat("Matched entry '{0}' with watched artist '{1}'",
                            entry.SortArtistName, artist);

                        break;
                    }
                }
            }

            // save the new last entry updated date
            Settings.Default.LastEntryUpdated = newLastUpdated;
            Settings.Default.Save();

            if (Notifier != null)
            {
                // process notifications

                while (MatchesToNotify.Count > 0)
                {
                    ForgeEntry match = MatchesToNotify.Dequeue();

                    StringBuilder message = new StringBuilder();

                    message.AppendFormat("{0} - {1}\n\n", match.ArtistName, match.SongName);
                    message.AppendFormat("{0}: {1}\n\n", Resources.NotifyStrings.NotifyInfoUri, match.InformationUri);
                    message.AppendFormat("{0}: {1}", Resources.NotifyStrings.NotifyDownloadUri, match.DownloadUri);

                    Notifier.Notify(Resources.NotifyStrings.NotifyTitle, message.ToString());

                    // rate limit this
                    Thread.Sleep(1000);
                }
            }

            Logger.Info("Finished");
        }
    }
}