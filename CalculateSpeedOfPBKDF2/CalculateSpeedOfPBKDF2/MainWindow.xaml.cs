using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace CalculateSpeedOfPBKDF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rfc2898DeriveBytes rfc2898DeriveBytes;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> dataContext = new List<int> { 500, 1000, 5000, 10000 };
            List<HashAlgorithmName> hashAlgorithm = new List<HashAlgorithmName> {
                    HashAlgorithmName.MD5,
                    HashAlgorithmName.SHA1,
                    HashAlgorithmName.SHA256,
                    HashAlgorithmName.SHA384,
                    HashAlgorithmName.SHA512
            };
            List<int> itemForSaltSize = new List<int> {64, 80, 128, 256, 512};

            foreach (var item in dataContext)
                cmbNumberOfIteration.Items.Add(item);
            foreach (var item in hashAlgorithm)
                cmbHashAlgorithm.Items.Add(item);
            foreach (var item in itemForSaltSize)
                cmbSaltSize.Items.Add(item);
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrEmpty(txtPassword.Text))
                MessageBox.Show("Fill password field!");
            else
            {
                var selectedHashAlgorithm = cmbHashAlgorithm.SelectedItem;
                var selectedIterationCount = cmbNumberOfIteration.SelectedItem;
                var selectedSaltSize = cmbSaltSize.SelectedItem;
                string password = txtPassword.Text;
                DateTime startedTime = DateTime.Now;
                string hashedPassword = hashPassword(password, Convert.ToInt16(selectedSaltSize), Convert.ToInt16(selectedIterationCount), (HashAlgorithmName)selectedHashAlgorithm);
                txtHashedPassword.Text = hashedPassword;
                TimeSpan substractedTimeSpan = DateTime.Now.Subtract(startedTime);
                txtSpeed.Text = substractedTimeSpan.TotalMilliseconds.ToString();
            }
        }

        public string hashPassword(string password, int saltSize, int itaration, HashAlgorithmName hashAlgorithmName)
        {
            byte[] saltByte;
            byte[] passwordByte;
            byte[] saltedHashPassword = new byte[saltSize/8+outputSizeHashAlgorithm(hashAlgorithmName)/8+1];
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltSize/8, itaration, hashAlgorithmName);
            saltByte = rfc2898DeriveBytes.Salt;
            passwordByte = rfc2898DeriveBytes.GetBytes(saltSize/8 + outputSizeHashAlgorithm(hashAlgorithmName)/8+1);
            Buffer.BlockCopy(saltByte, 0, saltedHashPassword, 0, saltSize / 8);
            Buffer.BlockCopy(passwordByte, 0, saltedHashPassword, saltSize/8, outputSizeHashAlgorithm(hashAlgorithmName)/8);
            return Convert.ToBase64String(passwordByte);
        }

        public int outputSizeHashAlgorithm(HashAlgorithmName hashAlgorithmName)
        {
            switch (hashAlgorithmName.Name)
            {
                case "MD5":
                    return 128;
                case "SHA1":
                    return 160;
                case "SHA256":
                    return 256;
                case "SHA384":
                    return 384;
                case "SHA512":
                    return 512;
            }
            return 0;
        }

    }
}
