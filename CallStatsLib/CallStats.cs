﻿using CallStatsLib.Response;
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
    public class CallStats
    {
        private static readonly HttpClient _client = new HttpClient();

        private const string _domain = "callstats.io";
        private enum Host { auth, events, stats }

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

        public CallStats(string localID, string appID, string keyID, string confID, ECDsa privateKey)
        {
            _localID = localID;
            _appID = appID;
            _keyID = keyID;
            _confID = confID;
            _privateKey = privateKey;
        }

        private string UrlBuilder(string host, string endpoint)
        {
            return $"https://{host}.{_domain}{endpoint}";
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
                { "iat", DateTime.UtcNow.ToUnixTimeStamp() },
                { "nbf", DateTime.UtcNow.AddMinutes(-5).ToUnixTimeStamp() },
                { "exp", DateTime.UtcNow.AddHours(1).ToUnixTimeStamp() },
                { "jti", _jti }
            };

            // Use this when you have private key and ecc-key.p12 file:
            // return JWT.Encode(payload, _privateKey, JwsAlgorithm.ES256, extraHeaders: header);

            return string.Empty;
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

        private async Task UserLeft(UserLeftData userLeftData)
        {
            await SendRequest(userLeftData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/user/left"));
        }

        #endregion

        #region Fabric Events 

        private async Task<Tuple<HttpStatusCode, string>> FabricSetup(FabricSetupData fabricSetupData)
        {
            return await SendRequest(fabricSetupData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/setup")); 
        }

        private async Task<Tuple<HttpStatusCode, string>> FabricSetupFailed(FabricSetupFailedData fabricSetupFailedData)
        {
            return await SendRequest(fabricSetupFailedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/setupfailed"));
        }

        private async Task FabricTerminated(FabricTerminatedData fabricTerminatedData)
        {
            await SendRequest(fabricTerminatedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/terminated"));
        }

        public async Task FabricStateChange(FabricStateChangeData fabricStateChangeData)
        {
            await SendRequest(fabricStateChangeData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/statechange"));
        }

        public async Task FabricTransportChange(FabricTransportChangeData fabricTransportChangeData)
        {
            await SendRequest(fabricTransportChangeData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/transportchange"));
        }

        public async Task FabricDropped(FabricDroppedData fabricDroppedData)
        {
            await SendRequest(fabricDroppedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/status"));
        }

        public async Task FabricAction(FabricActionData fabricActionData)
        {
            await SendRequest(fabricActionData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/fabric/actions"));
        }

        #endregion

        #region Stats Submission

        private async Task ConferenceStatsSubmission(ConferenceStatsSubmissionData conferenceStatsSubmissionData)
        {
            await SendRequest(conferenceStatsSubmissionData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/stats"));
        }

        public async Task SystemStatusStatsSubmission(SystemStatusStatsSubmissionData systemStatusStatsSubmissionData)
        {
            await SendRequest(systemStatusStatsSubmissionData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/stats/system"));
        }

        #endregion

        #region Ice Events 

        public async Task IceDisruptionStart(IceDisruptionStartData iceDisruptionStartData)
        {
            await SendRequest(iceDisruptionStartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceDisruptionEnd(IceDisruptionEndData iceDisruptionEndData)
        {
            await SendRequest(iceDisruptionEndData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceRestart(IceRestartData iceRestartData)
        {
            await SendRequest(iceRestartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceFailed(IceFailedData iceFailedData)
        {
            await SendRequest(iceFailedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceAborted(IceAbortedData iceAbortedData)
        {
            await SendRequest(iceAbortedData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceTerminated(IceTerminatedData iceTerminatedData)
        {
            await SendRequest(iceTerminatedData, UrlBuilder(Host.events.ToString(),
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceConnectionDisruptionStart(IceConnectionDisruptionStartData iceConnectionDisruptionStartData)
        {
            await SendRequest(iceConnectionDisruptionStartData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        public async Task IceConnectionDisruptionEnd(IceConnectionDisruptionEndData iceConnectionDisruptionEndData)
        {
            await SendRequest(iceConnectionDisruptionEndData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ice/status"));
        }

        #endregion

        #region Media Events

        public async Task MediaAction(MediaActionData mediaActionData)
        {
            await SendRequest(mediaActionData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/media/actions"));
        }

        public async Task MediaPlayback(MediaPlaybackData mediaPlaybackData)
        {
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
            await SendRequest(applicationErrorLogsData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/app/logs"));
        }

        public async Task ConferenceUserFeedback(ConferenceUserFeedbackData conferenceUserFeedbackData)
        {
            await SendRequest(conferenceUserFeedbackData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/feedback"));
        }

        public async Task DominantSpeaker(DominantSpeakerData dominantSpeakerData)
        {
            await SendRequest(dominantSpeakerData, UrlBuilder(Host.events.ToString(),
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/dominantspeaker"));
        }

        private async Task SSRCMap(SSRCMapData ssrcMapData)
        {
            await SendRequest(ssrcMapData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/ssrcmap"));
        }

        public async Task SDPEvent(SDPEventData sdpEventData)
        {
            await SendRequest(sdpEventData, UrlBuilder(Host.events.ToString(), 
                $"/v1/apps/{_appID}/conferences/{_confID}/{_ucID}/events/sdp"));
        }

        #endregion

        #region Bridge Events

        public async Task BridgeStatistics(BridgeStatisticsData bridgeStatisticsData)
        {
            await SendRequest(bridgeStatisticsData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/stats/bridge/status"));
        }

        public async Task BridgeAlive(BridgeAliveData bridgeAliveData)
        {
            await SendRequest(bridgeAliveData, UrlBuilder(Host.stats.ToString(), 
                $"/v1/apps/{_appID}/stats/bridge/alive"));
        }

        #endregion

        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<Tuple<HttpStatusCode, string>> SendRequest(object data, string url)
        {
            ByteArrayContent byteContent = 
                new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Version = new Version(2, 0);
            req.Content = byteContent;

            HttpResponseMessage res = await _client.SendAsync(req);

            HttpStatusCode statusCode = res.StatusCode;
            string content = await res.Content.ReadAsStringAsync();

            if (statusCode != HttpStatusCode.OK)
                Debug.WriteLine($"[Error] Http response status code: {statusCode}");

            Debug.WriteLine(content); Debug.WriteLine(string.Empty);

            return Tuple.Create(statusCode, content);
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
