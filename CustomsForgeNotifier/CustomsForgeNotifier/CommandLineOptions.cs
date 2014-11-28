using CommandLine;

namespace CustomsForgeNotifier
{
    internal class CommandLineOptions
    {
        [Option("reset", HelpText = "Resets the saved last updated date, causing the app to restart as if it were ran for the first time.")]
        public bool ResetLastUpdate { get; set; }
    }
}