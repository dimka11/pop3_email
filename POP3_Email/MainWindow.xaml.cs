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

namespace POP3_Email
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmailSettings settings;
        public MainWindow()
        {
            InitializeComponent();

            settings = new EmailSettings(ServerName.Text, UserName.Text, UserPassword.Password);
            UpdateEmailList();
        }

        private void ButtonUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            // check griditemscount
            UpdateEmailList();
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            settings = new EmailSettings(ServerName.Text, UserName.Text, UserPassword.Password);
        }

        private void UpdateEmailList()
        {
            List<Email> emailList = new List<Email>
            {
                new Email { From= "iPhone 6S", Subject= "Apple" },
                new Email { From="Lumia 950", Subject="Microsoft" },
                new Email { From="Nexus 5X", Subject="Google" }
            };
            EmailGrid.ItemsSource = emailList;

            var worker = new EmailWorker(settings);
        }
    }
}
