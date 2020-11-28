using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {
        private WebBrowser webBrowser;

        public HomeUserControl()
        {
            InitializeComponent();
            LoadWrapPanel();
        }

        public void LoadWrapPanel()
        {
            try
            {
                using BlueFoxContext db = new BlueFoxContext();
                foreach (var webPage in db.HomeWebPages)
                {
                    Border border = new Border()
                    {
                        BorderBrush = Brushes.DeepSkyBlue,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(10),
                        Width = 1100,
                        Margin = new Thickness(25, 20, 0, 0)
                    };
                    Grid grid = new Grid()
                    {
                        Height = 350,
                        Margin = new Thickness(10, 30, 10, 30),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    webBrowser = new WebBrowser()
                    {
                        Source = new Uri(webPage.WebPage_URL),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    webBrowser.Navigated += wbMain_Navigated;
                    border.Child = grid;
                    grid.Children.Add(webBrowser);
                    WrapPanel.Children.Add(border);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        void wbMain_Navigated(object sender, NavigationEventArgs e)
        {
            SetSilent(webBrowser, true); // make it silent
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate(new Uri("... some url..."));
        }

        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            if (browser.Document is IOleServiceProvider sp)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object wb;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out wb);
                if (wb != null)
                {
                    wb.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, wb, new object[] { silent });
                }
            }
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }
    }
}
