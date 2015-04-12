using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using RestSharp;
using Color = System.Windows.Media.Color;

namespace LocationNotifier
{
    enum ToolTipKind
    {
        Info, Warning, SystemError, Success
    }

    public partial class App
    {
        public App()
        {
            TaskbarIcon taskbarIcon = null;

            try
            {
                var icon = LocationNotifier.Properties.Resources.icon;

                var geoLocation = CreateGeoLocation();

                string text;
                if (String.IsNullOrEmpty(geoLocation.City))
                {
                    text = String.Format("IP: {0} - Location: {1}({2})", geoLocation.Ip, geoLocation.CountryName,
                    geoLocation.CountryCode);
                }
                else
                {
                    text = String.Format("IP: {0} - Location: {1}({3})/{2}", geoLocation.Ip, geoLocation.CountryName,
                    geoLocation.City, geoLocation.CountryCode);
                }

                ToolTipKind kind;
                if (geoLocation.CountryCode == "SE")
                {
                    kind = ToolTipKind.Success;
                }
                else
                {
                    kind = ToolTipKind.Warning;
                }

                var border = CreateToolTipContent(kind, text);

                taskbarIcon = new TaskbarIcon
                {
                    Icon = icon,
                    ToolTipText = text,
                    TrayToolTip = border
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not create/find icon", MessageBoxButton.OK);
                
                if (taskbarIcon != null)
                    taskbarIcon.Dispose();
            }
        }

        private GeoLocation CreateGeoLocation()
        {
            var client = new RestClient(new Uri("http://freegeoip.net/json/"));
            var request = new RestRequest(Method.GET);
            var response = client.Execute<GeoLocation>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }

        private static Border CreateToolTipContent(ToolTipKind kind, string text)
        {
            Color background;
            Color border;

            if (kind == ToolTipKind.SystemError)
            {
                background = Colors.LightCoral;
                border = Colors.DarkRed;
            }
            else if (kind == ToolTipKind.Warning)
            {
                background = Colors.LightGoldenrodYellow;
                border = Colors.DarkOrange;
            }
            else if (kind == ToolTipKind.Success)
            {
                background = Colors.DarkSeaGreen;
                border = Colors.DarkGreen;
            }
            else
            {
                background = Colors.LightGray;
                border = Colors.DimGray;
            }

            return new Border
            {
                Padding = new Thickness(10),
                Background = new SolidColorBrush(background),
                BorderBrush = new SolidColorBrush(border),
                BorderThickness = new Thickness(4),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 16
                }
            };
        }
    }

    internal class GeoLocation
    {
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public string MetroCode { get; set; }
        public string AreaCode { get; set; }
    }
}
