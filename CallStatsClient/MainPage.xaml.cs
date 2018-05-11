using CallStatsLib;
using CallStatsLib.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CallStatsClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Config.AppSettings();

            Task task = InitializeCallStats();
        }

        public async Task InitializeCallStats()
        {
            string localID = Config.localSettings.Values["localID"].ToString();
            string appID = Config.localSettings.Values["appID"].ToString();
            string keyID = Config.localSettings.Values["keyID"].ToString();
            string confID = Config.localSettings.Values["confID"].ToString();

            ECDsa privateKey = new X509Certificate2("ecc-key.p12", Config.localSettings.Values["password"].ToString()).GetECDsaPrivateKey();

            RestClient rClient = new RestClient(localID, appID, keyID, confID, privateKey);

            await rClient.StepsToIntegrate(TestData.CreateConference(), 
                                           TestData.UserAlive(), 
                                           TestData.FabricSetup(),
                                           TestData.FabricSetupFailed(), 
                                           TestData.SSRCMap(),
                                           TestData.ConferenceStatsSubmission(),
                                           TestData.FabricTerminated(),
                                           TestData.UserLeft());

            Debug.WriteLine("UserDetails: ");
            await rClient.UserDetails(TestData.UserDetails());

            Debug.WriteLine("FabricStateChange: ");
            await rClient.FabricStateChange(TestData.FabricStateChange());

            Debug.WriteLine("FabricTransportChange: ");
            await rClient.FabricTransportChange(TestData.FabricTransportChange());

            Debug.WriteLine("FabricDropped: ");
            await rClient.FabricDropped(TestData.FabricDropped());
        }
    }
}
