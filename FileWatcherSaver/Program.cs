namespace FileWatcherSaver
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArg = args[0].ToLower().Trim().Substring(0, 2);
                switch (firstArg)
                {
                    case "/s": // Show
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            new ScreensaverForm(screen.Bounds).Show();
                        }
                        Application.Run();
                        break;
                    case "/c": // Configure
                        Application.Run(new SettingsForm());
                        break;
                    case "/p": // Preview (Skip for now)
                        Application.Exit();
                        break;
                    default:
                        // Default to show
                         foreach (Screen screen in Screen.AllScreens)
                        {
                            new ScreensaverForm(screen.Bounds).Show();
                        }
                        Application.Run();
                        break;
                }
            }
            else
            {
                // Double-click behavior
                foreach (Screen screen in Screen.AllScreens)
                {
                    new ScreensaverForm(screen.Bounds).Show();
                }
                Application.Run();
            }
        }
    }
}
