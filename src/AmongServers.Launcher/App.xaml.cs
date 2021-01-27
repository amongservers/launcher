using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace AmongServers.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AmongServers");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // parse a custom protocol
            if (e.Args.Length > 0) {
                string arg = e.Args.First();

                if (Uri.TryCreate(arg, UriKind.Absolute, out Uri protocolUri)) {
                    if (protocolUri.Scheme.Equals("amongservers", StringComparison.OrdinalIgnoreCase)) {
                        if (protocolUri.AbsolutePath.StartsWith("join/", StringComparison.OrdinalIgnoreCase)) {
                            // parse the endpoint or show an error
                            if (!IPEndPoint.TryParse(protocolUri.AbsolutePath.Substring(5), out IPEndPoint serverEndpoint)) {
                                MessageBox.Show("The join address contains an invalid endpoint", "AS Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
                                Environment.Exit(0);
                                return;
                            }

                            try {
                                Bootstrapper.ReplaceRegionInfoAsync("AmongServers", serverEndpoint).Wait();
                                Bootstrapper.LaunchGameAsync().Wait();
                            } catch(Exception ex) {
                                MessageBox.Show($"An error occured trying to launch the game{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", "AS Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        Environment.Exit(0);
                    }
                }
            }

            // create data folder if possible
            try {
                if (!Directory.Exists(DataPath)) {
                    Directory.CreateDirectory(DataPath);
                }
            } catch(Exception ex) {
                Debug.Fail($"An error occured creating data path folder{Environment.NewLine}{Environment.NewLine}{ex.ToString()}");
            }
        }
    }
}
