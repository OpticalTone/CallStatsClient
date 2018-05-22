using CallStatsLib;
using CallStatsLib.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
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
            InitializeComponent();

            Config.AppSettings();

            try
            {
                Task task = InitializeCallStats();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] InitializeCallStats, message: '{ex.Message}'");
            }
        }

        private async Task InitializeCallStats()
        {
            string localID = Config.localSettings.Values["localID"].ToString();
            string appID = Config.localSettings.Values["appID"].ToString();
            string keyID = Config.localSettings.Values["keyID"].ToString();
            string confID = Config.localSettings.Values["confID"].ToString();

            // Use this when you have private key and ecc-key.p12 file:
            //ECDsa privateKey = new X509Certificate2("ecc-key.p12", 
            //    Config.localSettings.Values["password"].ToString()).GetECDsaPrivateKey();

            ECDsa privateKey = null;

            CallStats callstats = new CallStats(localID, appID, keyID, confID, privateKey);

            await callstats.StepsToIntegrate(
                TestData.CreateConference(), 
                TestData.UserAlive(), 
                TestData.FabricSetup(),
                TestData.FabricSetupFailed(), 
                TestData.SSRCMap(),
                TestData.ConferenceStatsSubmission(),
                TestData.FabricTerminated(),
                TestData.UserLeft());

            Debug.WriteLine("UserDetails: ");
            await callstats.UserDetails(TestData.UserDetails());

            Debug.WriteLine("FabricStateChange: ");
            await callstats.FabricStateChange(TestData.FabricStateChange());

            Debug.WriteLine("FabricTransportChange: ");
            await callstats.FabricTransportChange(TestData.FabricTransportChange());

            Debug.WriteLine("FabricDropped: ");
            await callstats.FabricDropped(TestData.FabricDropped());

            Debug.WriteLine("FabricAction: ");
            await callstats.FabricAction(TestData.FabricAction());

            Debug.WriteLine("SystemStatusStatsSubmission: ");
            await callstats.SystemStatusStatsSubmission(TestData.SystemStatusStatsSubmission());

            Debug.WriteLine("IceDisruptionStart: ");
            await callstats.IceDisruptionStart(TestData.IceDisruptionStart());

            Debug.WriteLine("IceDisruptionEnd: ");
            await callstats.IceDisruptionEnd(TestData.IceDisruptionEnd());

            Debug.WriteLine("IceRestart: ");
            await callstats.IceRestart(TestData.IceRestart());

            Debug.WriteLine("IceFailed: ");
            await callstats.IceFailed(TestData.IceFailed());

            Debug.WriteLine("IceAborted: ");
            await callstats.IceAborted(TestData.IceAborted());

            Debug.WriteLine("IceTerminated: ");
            await callstats.IceTerminated(TestData.IceTerminated());

            Debug.WriteLine("IceConnectionDisruptionStart: ");
            await callstats.IceConnectionDisruptionStart(TestData.IceConnectionDisruptionStart());

            Debug.WriteLine("IceConnectionDisruptionEnd: ");
            await callstats.IceConnectionDisruptionEnd(TestData.IceConnectionDisruptionEnd());

            Debug.WriteLine("MediaAction: ");
            await callstats.MediaAction(TestData.MediaAction());

            Debug.WriteLine("MediaPlayback: ");
            await callstats.MediaPlayback(TestData.MediaPlayback());

            Debug.WriteLine("ConnectedOrActiveDevices: ");
            await callstats.ConnectedOrActiveDevices(TestData.ConnectedOrActiveDevices());

            Debug.WriteLine("ApplicationErrorLogs: ");
            await callstats.ApplicationErrorLogs(TestData.ApplicationErrorLogs());

            Debug.WriteLine("ConferenceUserFeedback: ");
            await callstats.ConferenceUserFeedback(TestData.ConferenceUserFeedback());

            Debug.WriteLine("DominantSpeaker: ");
            await callstats.DominantSpeaker(TestData.DominantSpeaker());

            Debug.WriteLine("SDPEvent: ");
            await callstats.SDPEvent(TestData.SDPEvent());

            Debug.WriteLine("BridgeStatistics: ");
            await callstats.BridgeStatistics(TestData.BridgeStatistics());

            Debug.WriteLine("BridgeAlive: ");
            await callstats.BridgeAlive(TestData.BridgeAlive());

            Timer timer = new Timer(30000);
            timer.Elapsed += async (sender, e) =>
            {
                Debug.WriteLine("BridgeAlive: ");
                await callstats.BridgeAlive(TestData.BridgeAlive());
            };
            timer.Start();
        }
    }
}
