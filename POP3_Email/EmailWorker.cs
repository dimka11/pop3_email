using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP3_Email
{
    class EmailWorker
    {
        private EmailSettings settings;
        private List<Email> msgList;
        private SslStream sslStream;

        public EmailWorker(EmailSettings s)
        {
            settings = s;
        }

        public bool Run()
        {
            sslStream = getSslStream();
            if (!AuthMailUser())
            {
                return false;
            }
            return true;
        }

        public void SaveMessageToFile()
        {
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = appPath + "email_messages.txt";

            for (int i = 1; i <= getCount(); i++)
            {
                var msg = getMessage(i);

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, msg);
                }
                else
                {
                    File.AppendAllText(path, msg);
                }
            }
        }

        private void errorHappend(string error)
        {
            MessageBox.Show($"Случилась ошибка: {error}");
        }

        SslStream getSslStream()
        {
            TcpClient mail = new TcpClient();
            SslStream sslStream;
            int bytes = -1;

            mail.Connect(settings.EmailServer, 995);
            sslStream = new SslStream(mail.GetStream());
            sslStream.AuthenticateAsClient(settings.EmailServer);
            return sslStream;
        }

        bool AuthMailUser()
        {
            int bytes = -1;
            byte[] buffer = new byte[2048];

            bytes = sslStream.Read(buffer, 0, buffer.Length);
            LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            sslStream.Write(Encoding.ASCII.GetBytes($"USER {settings.UserName}\r\n"));
            bytes = sslStream.Read(buffer, 0, buffer.Length);
            LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            sslStream.Write(Encoding.ASCII.GetBytes($"PASS {settings.UserPassword}\r\n"));
            bytes = sslStream.Read(buffer, 0, buffer.Length);
            LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            string[] tokens = Encoding.ASCII.GetString(buffer, 0, bytes).Split(' ');
            if (tokens[0].Equals("-ERR"))
            {
                errorHappend(Encoding.ASCII.GetString(buffer, 0, bytes));
                return false;
            }
            return true;
        }

        string getMessage(int msg)
        {
            var messageString = new StringBuilder();

            var bytes = -1;
            byte[] buffer = new byte[2048];

            sslStream.Write(Encoding.ASCII.GetBytes($"RETR {msg}\r\n"));
            bytes = sslStream.Read(buffer, 0, buffer.Length);
            LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            while (true)
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                // LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

                var str = Encoding.ASCII.GetString(buffer, 0, bytes);

                messageString.Append(str);

                if (Encoding.ASCII.GetString(buffer, 0, bytes).Equals(".\r\n") || Encoding.ASCII.GetString(buffer, 0, bytes).Equals("\r\n.\r\n"))
                {
                    break;
                }
            }
            return messageString.ToString();
        }

        public int getCount()
        {
            var bytes = -1;
            byte[] buffer = new byte[2048];
            sslStream.Write(Encoding.ASCII.GetBytes("STAT\r\n"));
            bytes = sslStream.Read(buffer, 0, buffer.Length);
            LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            string[] tokens = Encoding.ASCII.GetString(buffer, 0, bytes).Split(' ');

            return int.Parse(tokens[1]);
        }

        void getSummary()
        {
            var bytes = -1;
            byte[] buffer = new byte[2048];

            sslStream.Write(Encoding.ASCII.GetBytes("LIST 17\r\n"));

            while (true)
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

            }
        }

        void getTopOfMessages()
        {
            var bytes = -1;
            byte[] buffer = new byte[2048];

            sslStream.Write(Encoding.ASCII.GetBytes("TOP 15 0\r\n"));
            while (true)
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));
            }
        }

        string[] getSubject(int msg)
        {
            var bytes = -1;
            byte[] buffer = new byte[2048];

            var from = "";
            var subject = "";

            sslStream.Write(Encoding.ASCII.GetBytes($"TOP {msg} 0\r\n"));

            while (true)
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                LogWrite(Encoding.ASCII.GetString(buffer, 0, bytes));

                var str = Encoding.ASCII.GetString(buffer, 0, bytes);

                int index = str.IndexOf("From");
                if (index == 0)
                {
                    from = ParseFrom(str);
                    continue;
                }

                if ((Encoding.ASCII.GetString(buffer, 0, bytes).IndexOf(" ") == 0 || Encoding.ASCII.GetString(buffer, 0, bytes).IndexOf("\t") == 0) && from.Length != 0 && from.IndexOf('@') == -1)
                {
                    from = from + ParseFrom(str);
                    continue;
                }

                index = str.IndexOf("Subject");
                if (index == 0)
                {
                    subject = ParseSubject(str);
                    continue;
                }

                if (Encoding.ASCII.GetString(buffer, 0, bytes).IndexOf(" ") == 0 && subject.Length != 0 && Encoding.ASCII.GetString(buffer, 0, bytes).IndexOf("@") == -1)
                {
                    subject = subject + ParseSubject(str);
                    continue;
                }

                if (Encoding.ASCII.GetString(buffer, 0, bytes).Equals(".\r\n") ||
                    Encoding.ASCII.GetString(buffer, 0, bytes).Equals("\r\n.\r\n"))
                {
                    break;
                }
            }

            return new[] { from, subject };
        }

        string ParseFrom(string str)
        {
            var s1 = "";
            if (str.IndexOf("?utf-8?b?") != -1)
            {
                string[] tokens = str.Split('?');
                s1 = Encoding.UTF8.GetString(System.Convert.FromBase64String(tokens[3]));
            }

            var s2 = "";
            if (str.IndexOf('<') != -1)
            {
                s2 = str.Substring(str.IndexOf('<') + 1,
                    str.IndexOf('>') - 1 - str.IndexOf('<'));
            }

            return (s1 + ' ' + s2).TrimStart(' ');
        }

        string ParseSubject(string str)
        {
            if (str.IndexOf("?utf-8?b?") != -1)
            {
                string[] tokens = str.Split('?');
                return Encoding.UTF8.GetString(System.Convert.FromBase64String(tokens[3]));
            }
            string[] tokens1 = str.Split(':');
            return tokens1[1].TrimStart(' ');

        }

        public List<Email> GetEmailList()
        {
            var list = new List<Email>();
            for (int i = 1; i <= getCount(); i++)
            {
                var msg = getSubject(i);
                list.Add(new Email(msg[0], msg[1]));
            }
            return list;
        }

        void LogWrite(string msg)
        {
            bool log_write = false;
            if (log_write) MessageBox.Show(msg);
            Trace.WriteLine(msg);
        }
    }
}
