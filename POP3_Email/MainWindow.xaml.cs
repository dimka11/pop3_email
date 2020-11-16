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
        private int email_count = -1;
        private int _messageSavedToFile = 0;
        public MainWindow()
        {
            InitializeComponent();

            settings = new EmailSettings(ServerName.Text, UserName.Text, UserPassword.Password);
            // UpdateEmailList(); // disable due the app is too long time start up
        }

        private void ButtonUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            var countRows = EmailGrid.Items.Count;
            UpdateEmailList();
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            settings = new EmailSettings(ServerName.Text, UserName.Text, UserPassword.Password);
        }

        private void UpdateEmailList()
        {
            var worker = new EmailWorker(settings);
            if (!worker.Run())
            {
                MessageBox.Show("Авторизация не удалась!");
                return;
            }

            if (email_count == -1)
            {
                email_count = worker.getCount();
            }
            else
            {
                var new_count = worker.getCount();
                if (email_count == new_count)
                {
                    MessageBox.Show("Новых сообщений нет");
                    return;
                }
            }

            var list = worker.GetEmailList();

            EmailGrid.ItemsSource = list;

            if ((bool)SaveToFile.IsChecked)
            {
                if (_messageSavedToFile == 0)
                {
                    worker.SaveMessageToFile();
                }
                else
                {
                    var new_count = worker.getCount();
                    if (email_count != new_count)
                    {
                        worker.SaveMessageToFile();
                    }
                }
            }

            // Заглушка для грида
            // List<Email> emailList = new List<Email>
            // {
            //     new Email { From= "iPhone 6S", Subject= "Apple" },
            //     new Email { From="Lumia 950", Subject="Microsoft" },
            //     new Email { From="Nexus 5X", Subject="Google" }
            // };
            // EmailGrid.ItemsSource = emailList;
        }
    }
}
