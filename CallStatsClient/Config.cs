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
            localSettings.Values["appID"] = "123";
            localSettings.Values["keyID"] = "123";

            Random rnd = new Random();
            int rndnum = rnd.Next();
            localSettings.Values["confID"] = rndnum.ToString();

            // secret string
            localSettings.Values["secret"] = "123";
        }
    }
}
