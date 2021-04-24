using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
    public class OAuthSettings : IOAuthSettings
    {
        public OAuthKeySet Google { get; set; }
        public OAuthKeySet Facebook { get; set; }
    }

    public interface IOAuthSettings
    {
        public OAuthKeySet Google { get; set; }
        public OAuthKeySet Facebook { get; set; }
    }

    public class OAuthKeySet
    {
        public string Secret { get; set; }
        public string ClientId { get; set; }
    }

    public class HERESettings : IHERESettings
    {
        public string AppId { get; set; }
        public string ApiKey { get; set; }
    }

    public interface IHERESettings
    {
        public string AppId { get; set; }
        public string ApiKey { get; set; }
    }

    public class BestTimeSettings : IBestTimeSettings
    {
        public string ApiKey { get; set; }
        public string PrivateKey { get; set; }
        public int ForecastLifetime { get; set; }
    }

    public interface IBestTimeSettings
    {
        public string ApiKey { get; set; }
        public string PrivateKey { get; set; }
        public int ForecastLifetime { get; set; }
    }

    public class BigDataCloudSettings : IBigDataCloudSettings
    {
        public string ApiKey { get; set; }
    }

    public interface IBigDataCloudSettings
    {
        public string ApiKey { get; set; }
    }

    public class SegmentSettings
    {
        public string Key { get; set; }
    }

    public class Office365Settings : IOffice365Settings
    {
        public string SmtpServerAddress { get; set; }
        public int SmtpServerPort { get; set; }
        public string Sender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public interface IOffice365Settings
    {
        public string SmtpServerAddress { get; set; }
        public int SmtpServerPort { get; set; }
        public string Sender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
