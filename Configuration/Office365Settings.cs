using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
    public class Office365Settings : IOffice365Settings
    {
        public string SmtpServerAddress { get; set; }
        public string Sender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public interface IOffice365Settings
    {
        public string SmtpServerAddress { get; set; }
        public string Sender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
