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

            Debug.WriteLine("FabricAction: ");
            await rClient.FabricAction(TestData.FabricAction());

            Debug.WriteLine("SystemStatusStatsSubmission: ");
            await rClient.SystemStatusStatsSubmission(TestData.SystemStatusStatsSubmission());

            Debug.WriteLine("IceDisruptionStart: ");
            await rClient.IceDisruptionStart(TestData.IceDisruptionStart());

            Debug.WriteLine("IceDisruptionEnd: ");
            await rClient.IceDisruptionEnd(TestData.IceDisruptionEnd());

            Debug.WriteLine("IceRestart: ");
            await rClient.IceRestart(TestData.IceRestart());

            Debug.WriteLine("IceFailed: ");
            await rClient.IceFailed(TestData.IceFailed());

            Debug.WriteLine("IceAborted: ");
            await rClient.IceAborted(TestData.IceAborted());

            Debug.WriteLine("IceTerminated: ");
            await rClient.IceTerminated(TestData.IceTerminated());

            Debug.WriteLine("IceConnectionDisruptionStart: ");
            await rClient.IceConnectionDisruptionStart(TestData.IceConnectionDisruptionStart());

            Debug.WriteLine("IceConnectionDisruptionEnd: ");
            await rClient.IceConnectionDisruptionEnd(TestData.IceConnectionDisruptionEnd());

            Debug.WriteLine("MediaAction: ");
            await rClient.MediaAction(TestData.MediaAction());

            Debug.WriteLine("MediaPlayback: ");
            await rClient.MediaPlayback(TestData.MediaPlayback());

            Debug.WriteLine("ConnectedOrActiveDevices: ");
            await rClient.ConnectedOrActiveDevices(TestData.ConnectedOrActiveDevices());

            Debug.WriteLine("ApplicationErrorLogs: ");
            await rClient.ApplicationErrorLogs(TestData.ApplicationErrorLogs());

            Debug.WriteLine("ConferenceUserFeedback: ");
            await rClient.ConferenceUserFeedback(TestData.ConferenceUserFeedback());

            Debug.WriteLine("DominantSpeaker: ");
            await rClient.DominantSpeaker(TestData.DominantSpeaker());
        }
    }
}
