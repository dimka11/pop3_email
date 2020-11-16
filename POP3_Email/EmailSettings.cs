using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP3_Email
{
    class EmailSettings
    {
        public string EmailServer;
        public string UserName;
        public string UserPassword;

        public EmailSettings(string server, string user, string password)
        {
            EmailServer = server;
            UserName = user;
            UserPassword = password;
        }
    }
}