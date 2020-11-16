using System;
using System.Collections.Generic;
using System.Linq;
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
        List<Email> list = null;
        public MainWindow()
        {
            InitializeComponent();

            settings = new EmailSettings(ServerName.Text, UserName.Text, UserPassword.Password);
            UpdateEmailList();
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
            if (list != null)
            {
                EmailGrid.ItemsSource = list;
            }

            var worker = new EmailWorker(settings);
            if (!worker.Run())
            {
                MessageBox.Show("Авторизация не удалась!");
                return;
            }

            SaveMsgToFile(worker);

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

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                list = worker.GetEmailList();
            }).Start();

            EmailGrid.ItemsSource = list;
        }

        void SaveMsgToFile(EmailWorker worker)
        {
            if ((bool)SaveToFile.IsChecked)
            {
                if (_messageSavedToFile == 0)
                {
                    worker.SaveMessageToFile();
                    _messageSavedToFile = 1;
                }
                else
                {
                    var new_count = worker.getCount();
                    if (email_count != new_count)
                    {
                        worker.SaveMessageToFile();
                        _messageSavedToFile = 1;
                    }
                }
            }
        }
    }
}
