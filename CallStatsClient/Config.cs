using System;
using Windows.Storage;

namespace CallStatsClient
{
    public static class Config
    {
        public static ApplicationDataContainer localSettings =
                ApplicationData.Current.LocalSettings;

        public static void AppSettings()
        {
            localSettings.Values["localID"] = "123";
            localSettings.Values["appID"] = "658825596";
            localSettings.Values["keyID"] = "b896d04b31a97812b0";

            Random rnd = new Random();
            int rndnum = rnd.Next();
            localSettings.Values["confID"] = rndnum.ToString();

            localSettings.Values["originID"] = "originID";
            localSettings.Values["deviceID"] = "deviceID";

            // privateKey
            localSettings.Values["password"] = "pass";
        }
    }
}
