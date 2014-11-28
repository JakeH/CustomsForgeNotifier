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
            var options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.ResetLastUpdate)
                {
                    Settings.Default.LastEntryUpdated = DateTime.MinValue;
                    Settings.Default.Save();
                }

            }

            PerformTasks();
        }

        private static void PerformTasks()
        {
            DateTime newLastUpdated = DateTime.MinValue;
            DateTime lastUpdated = Settings.Default.LastEntryUpdated;
            int processedEntryCount = 0;

            Logger.InfoFormat("Starting with last updated of '{0}'", lastUpdated);

            FileInfo artistsFile = new FileInfo("ArtistsToMatch.txt");
            if (!artistsFile.Exists)
            {
                Logger.ErrorFormat("Cannot find an 'artists to match' file at: '{0}'", artistsFile.FullName);
                return;
            }

            //TODO: upgrade this poor man's database.
            List<string> ArtistsToMatch = new List<string>(File.ReadAllLines(artistsFile.FullName, Encoding.UTF8));

            Queue<ForgeEntry> MatchesToNotify = new Queue<ForgeEntry>();

            foreach (var entry in DataRequester.GetForgeEntries(lastUpdated, Settings.Default.AbsoluteRetrievalLimit))
            {
                processedEntryCount++;

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

            Logger.InfoFormat("Processed {0} entries, matching {1}", processedEntryCount, MatchesToNotify.Count);

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