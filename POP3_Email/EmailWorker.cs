using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP3_Email
{
    class EmailWorker
    {
        private EmailSettings settings;
        private List<Dictionary<string, int>> msgList;

        public EmailWorker(EmailSettings s)
        {
            settings = s;
        }

        public List<Email> GetEmailList()
        {
            return  new List<Email>();
        }

        private bool ValidateServerAnswer()
        {
            return true;
        }

        private void GetListOfMessages()
        {

        }

        private string SendData()
        {
            return "";
        }

        public int CountOfMessages()
        {
            return 0;
        }

        private void SaveMessageToFile(string msg)
        {
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = appPath + "email_messages.txt";

            if (!File.Exists(path))
            {
                File.WriteAllText(path, msg);
            }
            else
            {
                File.AppendAllText(path, msg);
            }
        }

        private bool AuthOnServer()
        {
            return true;
        }

        private void errorHappend(string error)
        {
            MessageBox.Show($"Случилась ошибка: {error}");
        }
    }
}
