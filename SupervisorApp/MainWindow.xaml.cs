using CefSharp;
using CefSharp.Wpf;
using SupervisorApp.Handler;
using SupervisorApp.Utils;
using System.Windows;

namespace SupervisorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser browser;
        private Config config;

        public MainWindow()
        {
            InitializeComponent();
            config = Utils.Utils.GetConfig();

            InitBrowser();
        }

        private void InitBrowser()
        {
            CefSettings settings = new CefSettings();
            settings.CachePath = "BrowserCache";

            Cef.Initialize(settings);

            browser = new ChromiumWebBrowser(config.Url);
            browser.DownloadHandler = new DownloadHandler();
            browser.RequestHandler = new CustomRequestHandler();

            grid.Children.Add(browser);
        }

    }
}
