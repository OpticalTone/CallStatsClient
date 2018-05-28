using CallStatsLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Jose;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CallStatsClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _localID = (string)Config.localSettings.Values["localID"];
        private string _appID = (string)Config.localSettings.Values["appID"];
        private string _keyID = (string)Config.localSettings.Values["keyID"];

        private static readonly string _jti = new Func<string>(() =>
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const int length = 10;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        })();

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

        private string GenerateJWT()
        {
            var header = new Dictionary<string, object>()
            {
                { "typ", "JWT" },
                { "alg", "ES256" }
            };

            var payload = new Dictionary<string, object>()
            {
                { "userID", _localID },
                { "appID", _appID },
                { "keyID", _keyID },
                { "iat", DateTime.UtcNow.ToUnixTimeStamp() },
                { "nbf", DateTime.UtcNow.AddMinutes(-5).ToUnixTimeStamp() },
                { "exp", DateTime.UtcNow.AddHours(1).ToUnixTimeStamp() },
                { "jti", _jti }
            };

            try
            {
                string eccKey = @"ecc-key.p12";
                if (File.Exists(eccKey))
                {
                    if (new FileInfo(eccKey).Length != 0)
                    {
                        return JWT.Encode(payload, new X509Certificate2(eccKey,
                            (string)Config.localSettings.Values["password"]).GetECDsaPrivateKey(),
                            JwsAlgorithm.ES256, extraHeaders: header);
                    }
                    else
                    {
                        Debug.WriteLine("[Error] File is empty.");
                        return string.Empty;
                    }
                }
                else
                {
                    Debug.WriteLine("[Error] File does not exist.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] GenerateJWT: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task InitializeCallStats()
        {
            string confID = Config.localSettings.Values["confID"].ToString();

            CallStats callstats = new CallStats(_localID, _appID, _keyID, confID, GenerateJWT());

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

    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeStamp(this DateTime dateTimeUtc)
        {
            return (long)Math.Round((dateTimeUtc.ToUniversalTime() - UnixEpoch).TotalSeconds);
        }
    }
}
