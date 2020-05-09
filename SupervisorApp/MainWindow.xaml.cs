using CefSharp;
using CefSharp.Wpf;
using SupervisorApp.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SupervisorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser browser;
        private string initUrl = "https://localhost:44359/";

        public MainWindow()
        {
            InitializeComponent();

            InitBrowser();
        }

        private void InitBrowser()
        {
            CefSettings settings = new CefSettings();
            settings.CachePath = "BrowserCache";

            Cef.Initialize(settings);

            browser = new ChromiumWebBrowser(initUrl);
            browser.DownloadHandler = new DownloadHandler();
            browser.RequestHandler = new CustomRequestHandler();

            grid.Children.Add(browser);
        }
    }
}
