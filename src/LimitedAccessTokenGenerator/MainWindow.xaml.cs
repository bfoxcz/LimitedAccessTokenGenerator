using Microsoft.Win32;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace LimitedAccessTokenGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string registryToRead = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModel\LimitedAccessFeatures";

        public MainWindow()
        {
            InitializeComponent();

            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryToRead);
            if (key != null)
            {
                FeatureId.Items.Clear();
                foreach (string k in key.GetSubKeyNames())
                {
                    FeatureId.Items.Add(k);
                }
            }
        }

        static string ComputeSha256Hash(string rawData)
        {
            string returnStr = "";

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData)).Take(16).ToArray();
                returnStr = System.Convert.ToBase64String(bytes);
            }

            return returnStr;
        }

        public void FeatureId_Selected(object sender, RoutedEventArgs e)
        {
            GenerateToken();
        }

        private void PackageFamilyName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            GenerateToken();
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputKey.Text);
        }

        private void GenerateToken()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryToRead + "\\" + FeatureId.SelectedItem.ToString());
            if (key != null)
            {
                var dv = key?.GetValue("");
                if (dv != null)
                {
                    FeatureKey.Text = dv.ToString();
                    TextKeyToHash.Text = FeatureId.SelectedItem.ToString() + "!" + dv.ToString() + "!" + PackageFamilyName.Text;

                    OutputKey.Text = ComputeSha256Hash(TextKeyToHash.Text);
                }
            }
        }
    }
}
