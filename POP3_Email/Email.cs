using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP3_Email
{
    public class Email
    {
        public string From { get; set; }
        public string Subject { get; set; }

        public Email(string from, string subject)
        {
            From = from;
            Subject = subject;
        }

        public Email()
        {
            
        }
    }
}
