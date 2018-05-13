using CallStatsLib.Responses;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using Jose;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Linq;
using CallStatsLib.Request;
using System.Net;

namespace CallStatsLib
{
    public class RestClient
    {
        private static readonly HttpClient _client = new HttpClient();
        private static string _domain = "callstats.io";
        private enum Host
        {
            auth,
            events,
            stats
        }

        private string _localID;
        private string _appID;
        private string _keyID;
        private string _confID;
        private ECDsa _privateKey; 
        private string _ucID;

        private static readonly string _jti = new Func<string>(() => 
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const int length = 10;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        })();

        public RestClient(string localID, string appID, string keyID, string confID, ECDsa privateKey)
        {
            _localID = localID;
            _appID = appID;
            _keyID = keyID;
            _confID = confID;
            _privateKey = privateKey;
        }

        public async Task StepsToIntegrate(CreateConferenceData createConferenceData, UserAliveData userAliveData, 
            FabricSetupData fabricSetupData, FabricSetupFailedData fabricSetupFailedData, 
            SSRCMapData ssrcMapData, ConferenceStatsSubmissionData conferenceStatsSubmissionData,
            FabricTerminatedData fabricTerminatedData, UserLeftData userLeftData)
        {
            string authContent = await Authentication();
            string accessToken = DeserializeJson<AuthenticationResponse>(authContent).access_token;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            Debug.WriteLine("CreateConference: ");
            var confContent = await CreateConference(createConferenceData);
            _ucID = DeserializeJson<ConferenceResponse>(confContent.Item2).ucID;

            Timer timer = new Timer(10000);
            timer.Elapsed += async (sender, e) =>
            {
                Debug.WriteLine("UserAlive: ");
                await UserAlive(userAliveData);
            };
            timer.Start();

            Debug.WriteLine("FabricSetup: ");
            var fabricStatus = await FabricSetup(fabricSetupData);

            if (fabricStatus.Item1 != HttpStatusCode.OK)
            {
                Debug.WriteLine("FabricSetupFailed: ");
                await FabricSetupFailed(fabricSetupFailedData);
            }

            Debug.WriteLine("SSRCMap: ");
            await SSRCMap(ssrcMapData);

            Debug.WriteLine("ConferenceStatsSubmission: ");
            await ConferenceStatsSubmission(conferenceStatsSubmissionData);

            Debug.WriteLine("FabricTerminated: ");
            await FabricTerminated(fabricTerminatedData);

            Debug.WriteLine("UserLeft: ");
            await UserLeft(userLeftData);
        }

        #region Authentication

        private async Task<string> Authentication()
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", GenerateJWT() },
                { "client_id", _localID + "@" + _appID }
            };

            HttpRequestMessage req = 
                new HttpRequestMessage(HttpMethod.Post, UrlBuilder(Host.auth.ToString(), "/authenticate"));

            req.Content = new FormUrlEncodedContent(values);

            HttpResponseMessage res = await _client.SendAsync(req);

            return await res.Content.ReadAsStringAsync();
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
                { "iat", TimeStamp.Now() },
                { "nbf", TimeStamp.Now() },
                { "exp", TimeStamp.ExpireInHours(1) },
                { "jti", _jti }
            };

            return JWT.Encode(payload, _privateKey, JwsAlgorithm.ES256, extraHeaders: header);
        }

        #endregion

        #region User Action Events

        private async Task<Tuple<HttpStatusCode, string>> CreateConference(CreateConferenceData createConferenceData)
        {
            return await SendRequest(createConferenceData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}"));
        }

        private async Task UserAlive(UserAliveData userAliveData)
        {
            await SendRequest(userAliveData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/user/alive"));
        }

        public async Task UserDetails(UserDetailsData userDetailsData)
        {
            await SendRequest(userDetailsData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/userdetails"));
        }

        public async Task UserLeft(UserLeftData userLeftData)
        {
            await SendRequest(userLeftData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/user/left"));
        }

        #endregion

        #region Fabric Events 

        public async Task<Tuple<HttpStatusCode, string>> FabricSetup(FabricSetupData fabricSetupData)
        {
            fabricSetupData.connectionID = _ucID;

            return await SendRequest(fabricSetupData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/setup")); 
        }

        public async Task<Tuple<HttpStatusCode, string>> FabricSetupFailed(FabricSetupFailedData fabricSetupFailedData)
        {
            return await SendRequest(fabricSetupFailedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/setupfailed"));
        }

        public async Task FabricTerminated(FabricTerminatedData fabricTerminatedData)
        {
            fabricTerminatedData.connectionID = _ucID;

            await SendRequest(fabricTerminatedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/terminated"));
        }

        public async Task FabricStateChange(FabricStateChangeData fabricStateChangeData)
        {
            fabricStateChangeData.connectionID = _ucID;

            await SendRequest(fabricStateChangeData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/statechange"));
        }

        public async Task FabricTransportChange(FabricTransportChangeData fabricTransportChangeData)
        {
            fabricTransportChangeData.connectionID = _ucID;

            await SendRequest(fabricTransportChangeData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/transportchange"));
        }

        public async Task FabricDropped(FabricDroppedData fabricDroppedData)
        {
            fabricDroppedData.connectionID = _ucID;

            await SendRequest(fabricDroppedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/status"));
        }

        public async Task FabricAction(FabricActionData fabricActionData)
        {
            fabricActionData.connectionID = _ucID;

            await SendRequest(fabricActionData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/actions"));
        }

        #endregion

        #region Stats Submission

        public async Task ConferenceStatsSubmission(ConferenceStatsSubmissionData conferenceStatsSubmissionData)
        {
            conferenceStatsSubmissionData.connectionID = _ucID;

            await SendRequest(conferenceStatsSubmissionData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/stats"));
        }

        public async Task SystemStatusStatsSubmission(SystemStatusStatsSubmissionData systemStatusStatsSubmissionData)
        {
            await SendRequest(systemStatusStatsSubmissionData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/stats/system"));
        }

        #endregion

        #region Ice Events 

        public async Task IceDisruptionStart(IceDisruptionStartData iceDisruptionStartData)
        {
            iceDisruptionStartData.connectionID = _ucID;

            await SendRequest(iceDisruptionStartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceDisruptionEnd(IceDisruptionEndData iceDisruptionEndData)
        {
            iceDisruptionEndData.connectionID = _ucID;

            await SendRequest(iceDisruptionEndData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceRestart(IceRestartData iceRestartData)
        {
            iceRestartData.connectionID = _ucID;

            await SendRequest(iceRestartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceFailed(IceFailedData iceFailedData)
        {
            iceFailedData.connectionID = _ucID;

            await SendRequest(iceFailedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceAborted(IceAbortedData iceAbortedData)
        {
            iceAbortedData.connectionID = _ucID;

            await SendRequest(iceAbortedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceTerminated(IceTerminatedData iceTerminatedData)
        {
            iceTerminatedData.connectionID = _ucID;

            await SendRequest(iceTerminatedData, UrlBuilder(Host.events.ToString(),
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceConnectionDisruptionStart(IceConnectionDisruptionStartData iceConnectionDisruptionStartData)
        {
            iceConnectionDisruptionStartData.connectionID = _ucID;

            await SendRequest(iceConnectionDisruptionStartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceConnectionDisruptionEnd(IceConnectionDisruptionEndData iceConnectionDisruptionEndData)
        {
            iceConnectionDisruptionEndData.connectionID = _ucID;

            await SendRequest(iceConnectionDisruptionEndData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        #endregion

        #region Media Events

        public async Task MediaAction(MediaActionData mediaActionData)
        {
            mediaActionData.connectionID = _ucID;

            await SendRequest(mediaActionData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/media/actions"));
        }

        public async Task MediaPlayback(MediaPlaybackData mediaPlaybackData)
        {
            mediaPlaybackData.connectionID = _ucID;

            await SendRequest(mediaPlaybackData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/media/pipeline"));
        }

        #endregion

        #region Device Events

        public async Task ConnectedOrActiveDevices(ConnectedOrActiveDevicesData connectedOrActiveDevicesData)
        {
            await SendRequest(connectedOrActiveDevicesData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/devices/list"));
        }

        #endregion

        #region Special Events

        public async Task ApplicationErrorLogs(ApplicationErrorLogsData applicationErrorLogsData)
        {
            applicationErrorLogsData.connectionID = _ucID;

            await SendRequest(applicationErrorLogsData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/app/logs"));
        }

        public async Task ConferenceUserFeedback()
        {
            string url = $"https://events.callstats.io/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/feedback";

            object feedbackObj = new
            {
                overallRating = 5,
                videoQualityRating = 5,
                audioQualityRating = 5,
                comments = "comments"
            };

            object data = new
            {
                localID = _localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                feedback = feedbackObj
            };

            await SendRequest(data, url);
        }

        private async Task DominantSpeaker()
        {
            string url = $"https://events.callstats.io/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/dominantspeaker";

            object data = new
            {
                localID = _localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now()
            };

            await SendRequest(data, url);
        }

        private async Task SSRCMap(SSRCMapData ssrcMapData)
        {
            ssrcMapData.connectionID = _ucID;

            await SendRequest(ssrcMapData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ssrcmap"));
        }

        public async Task SDPEvent()
        {
            string url = $"https://HOSTNAME/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/sdp";

            object data = new
            {
                localID = _localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                connectionID = _ucID,
                remoteID = "remoteID",
                localSDP = "",
                remoteSDP = ""
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Bridge Events

        private async Task BridgeStatistics()
        {
            string url = $"https://stats.callstats.io/v1/apps/{_appID}/stats/bridge/status";

            object data = new
            {
                localID = _localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                cpuUsage = 5,
                batteryLevel = 50,
                memoryUsage = 2,
                totalMemory = 100,
                threadCount = 1,
                intervalSentBytes = 0,
                intervalReceivedBytes = 0,
                intervalRtpFractionLoss = 0,
                totalRtpLostPackets = 0,
                intervalAverageRtt = 0,
                intervalAverageJitter = 0,
                intervalDownloadBitrate = 0,
                intervalUploadBitrate = 0,
                audioFabricCount = 0,
                videoFabricCount = 0,
                conferenceCount = 0,
                participantCount = 0
            };

            await SendRequest(data, url);
        }

        public async Task BridgeAlive()
        {
            string url = $"https://stats.callstats.io/v1/apps/{_appID}/stats/bridge/alive";

            object data = new
            {
                localID = _localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now()
            };

            await SendRequest(data, url);
        }

        #endregion

        private async Task<Tuple<HttpStatusCode, string>> SendRequest(object data, string url)
        {
            string dataContent = JsonConvert.SerializeObject(data);

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Version = new Version(2, 0);

            byte[] buffer = Encoding.UTF8.GetBytes(dataContent);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            req.Content = byteContent;

            HttpResponseMessage res = await _client.SendAsync(req);

            HttpStatusCode statusCode = res.StatusCode;
            string content = await res.Content.ReadAsStringAsync();

            Debug.WriteLine($"SendRequest statusCode: {statusCode}");
            Debug.WriteLine($"SendRequest content: {content}");

            return Tuple.Create(statusCode, content);
        }

        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string UrlBuilder(string host, string endpoint)
        {
            return $"https://{host}.{_domain}{endpoint}";
        }
    }
}
