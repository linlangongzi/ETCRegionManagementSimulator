using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ETCRegionManagementUnitTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestClientPage : Page
    {
        Client testClient;
        public TestClientPage()
        {
            this.InitializeComponent();
            testClient = new Client();
            UpdateStatus();
            //var customPage = new TestClientPage();

            // Act: Simulate page initialization

            // Assert
            //Assert.IsNotNull(customPage);
        }

        private async void UpdateStatus()
        {
            try
            {
                string status = await testClient.GetConnectionStatus();
                if (testClient.IsConnected)
                {
                    togglebtn_connect.Content = "Disconnect";
                }
                else
                {
                    togglebtn_connect.Content = "Connect";
                }
                textblock_status.Text = status;
            }
            catch (Exception ex)
            {
                // Handle error while updating status
                textblock_status.Text = $"Error: {ex.Message}";
            }
        }

        private async void togglebtn_connect_OnClick(object sender, RoutedEventArgs e)
        {
            if (testClient.IsConnected)
            {
                await testClient.DisconnectAsync();
            }
            else
            {
                string ipAddress = textbox_ip.Text.Trim();
                string portText = textbox_port.Text.Trim();
                int port;
                textblock_status.Text = $"Attempting to connect to {ipAddress} : {portText}";

                if (string.IsNullOrWhiteSpace(ipAddress) || !IsValidIpAddress(ipAddress))
                {
                    textblock_status.Text = "Invalid IP address.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(portText) || !int.TryParse(portText, out port) || port <= 0 || port > 65535)
                {
                    textblock_status.Text = "Invalid port number.";
                    return;
                }

                try
                {
                    await testClient.ConnectAsync(ipAddress, port);
                }
                catch (Exception ex)
                {
                    // Handle connection error
                    textblock_status.Text = $"Error: {ex.Message}";
                }
            }
            UpdateStatus(); // Update status after connecting or disconnecting

        }

        private bool IsValidIpAddress(string ipAddress)
        {
            // Regular expression pattern for IPv4 address
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

            // Create Regex object
            Regex regex = new Regex(pattern);

            // Match the input string with the regex pattern
            Match match = regex.Match(ipAddress);

            // Return true if the input string matches the pattern, otherwise false
            return match.Success;
        }

        private async void btn_send_OnClick(object sender, RoutedEventArgs e)
        {
            await testClient.SendDataAsync(Encoding.UTF8.GetBytes(textbox_messages.Text));
        }
    }

}
