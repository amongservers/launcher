using AmongServers.Launcher.Coordinator.Entities;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

namespace AmongServers.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ApiClient _client;

        public MainWindow()
        {
            _client = new ApiClient(Constants.ApiUrl);
            InitializeComponent();
        }

        /// <summary>
        /// Refreshes the banner image.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshBannerAsync(CancellationToken cancellationToken = default)
        {
            BannerEntity[] banners = null;
            BannerEntity activeBanner = new BannerEntity() {
                Link = "https://amongservers.com",
                Text = "Visit AmongServers.com!"
            };

            // load the banners from the API
            try {
                banners = await _client.ListBannersAsync(Constants.ApplicationVersion, cancellationToken);
            } catch (Exception ex) {
                Debug.Fail(ex.ToString());
            }

            // pick a banner from the service or use the default
            if (banners != null && banners.Length > 0) {
                activeBanner = banners[RandomNumberGenerator.GetInt32(banners.Length)];
            }

            // load the image
            if (!string.IsNullOrEmpty(activeBanner.ImageUrl)) {
                imgBanner.Source = new BitmapImage(new Uri(activeBanner.ImageUrl));
            }

            imgBanner.Cursor = Cursors.Hand;
            btnBanner.Cursor = Cursors.Hand;
            imgBanner.DataContext = activeBanner;
            btnBanner.DataContext = activeBanner;
        }

        /// <summary>
        /// Refresh the server list.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshServersAsync(CancellationToken cancellationToken = default)
        {
            // load the servers from the API
            ServerEntity[] servers;

            try {
                servers = await _client.ListServersAsync(cancellationToken);
            } catch (Exception ex) {
                Debug.Fail(ex.ToString());
                return;
            }

            servers = new[] {
                new ServerEntity() {
                    Name = "My Among Server",
                    IPAddress = "144.56.23.9",
                    Port = 32323
                },
                 new ServerEntity() {
                    Name = "Potato lobby, come join!",
                    IPAddress = "2.31.87.122",
                    Port = 32323
                }
            };

            listServers.ItemsSource = servers.OrderBy(s=> s.Name).Select(s => new {
                Name = s.Name,
                IPAddressPort = $"{s.IPAddress}:{s.Port}",
                IsSaved = false,
                CountPlayers = 0,
                CountLobbies = 0
            });
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Text = $"Version {Constants.ApplicationVersion}";
            _ = RefreshBannerAsync();
            btnRefresh.IsEnabled = false;
           await RefreshServersAsync();
            btnRefresh.IsEnabled = true;
        }

        private void btnBanner_Click(object sender, RoutedEventArgs e)
        {
            BannerEntity banner = (BannerEntity)btnBanner.DataContext;

            if (banner == null)
                return;
            if (string.IsNullOrEmpty(banner.Link))
                return;

            Process.Start(new ProcessStartInfo(banner.Link) {
                UseShellExecute = true
            });
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            btnRefresh.IsEnabled = false;
            await RefreshServersAsync();
            btnRefresh.IsEnabled = true;
        }

        private void btnOpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Constants.WebsiteUrl) {
                UseShellExecute = true
            });
        }

        private async void btnDirectPlay_Click(object sender, RoutedEventArgs e)
        {
            btnDirectPlay.IsEnabled = false;

            try {
                await Bootstrapper.LaunchGameAsync();
            } catch(Exception ex) {
                MessageBox.Show($"An error occured launching the game{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            btnDirectPlay.IsEnabled = true;
        }

        private void txtDirectPlay_TextChanged(object sender, TextChangedEventArgs e)
        {
            // check if the endpoint is valid
            bool isValidIp = IPEndPoint.TryParse(txtDirectPlay.Text, out IPEndPoint _);

            // if it's empty or valid we leave the default border brush
            if (string.IsNullOrEmpty(txtDirectPlay.Text) || isValidIp) {

            } else {
                txtDirectPlay.BorderBrush = Brushes.Red;
            }

            // enable the play button
            btnDirectPlay.IsEnabled = isValidIp;
        }
    }
}
