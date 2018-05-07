using Jose;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CallStatsClient.Responses;

namespace CallStatsClient
{
    class RestClient
    {
        private static readonly HttpClient client = new HttpClient();

        private string localID;
        private string appID;
        private string confID;
        private string keyID;
        private string token;
        private string ucID;

        public RestClient()
        {
            localID = Config.localSettings.Values["localID"].ToString();
            appID = Config.localSettings.Values["appID"].ToString();
            keyID = Config.localSettings.Values["keyID"].ToString();
        }

        public async Task StepsToIntegrate()
        {
            token = GenerateJWT();

            string authContent = await Authentication();
            string accessToken = DeserializeJson<AuthenticationResponse>(authContent).access_token;

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            confID = Config.localSettings.Values["confID"].ToString();

            string confContent = await CreateConference(confID);
            ucID = DeserializeJson<ConferenceResponse>(confContent).ucID;

            Timer timer = new Timer(10000);
            timer.Elapsed += async (sender, e) => 
            {
                Debug.WriteLine("UserAlive: ");
                await UserAlive();
            };
            timer.Start();

            Debug.WriteLine("FabricSetup: ");
            string fabricStatus = await FabricSetup();

            if (fabricStatus != "success")
            {
                Debug.WriteLine("FabricSetupFailed: ");
                fabricStatus = await FabricSetupFailed();
            }

            Debug.WriteLine("SSRCMap: ");
            await SSRCMap();

            Debug.WriteLine("ConferenceStatsSubmission: ");
            await ConferenceStatsSubmission();

            Debug.WriteLine("FabricTerminated: ");
            await FabricTerminated();

            Debug.WriteLine("UserLeft: ");
            await UserLeft();
        }

        #region Authentication

        private async Task<string> Authentication()
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", token },
                { "client_id", localID + "@" + appID }
            };

            string url = "https://auth.callstats.io/authenticate";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new FormUrlEncodedContent(values);

            HttpResponseMessage res = await client.SendAsync(req);
            string content = await res.Content.ReadAsStringAsync();

            return content;
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
                { "userID", localID },
                { "appID", appID },
                { "keyID", keyID },
                { "iat", TimeStamp.Now() },
                { "nbf", TimeStamp.Now() },
                { "exp", TimeStamp.ExpireInHours(1) },
                { "jti", "aSdFgHjKlZxCvBnM" }
            };
            
            ECDsa privateKey = new X509Certificate2("ecc-key.p12", Config.localSettings.Values["password"].ToString()).GetECDsaPrivateKey();

            return JWT.Encode(payload, privateKey, JwsAlgorithm.ES256, extraHeaders: header);
        }

        #endregion

        #region User Action Events

        private async Task<string> CreateConference(string confID)
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                endpointInfo = new
                {
                    type = "browser",
                    os = "os",
                    osVersion = "osVersion",
                    buildName = "buildName",
                    buildVersion = "buildVersion",
                    appVersion = "appVersion"
                }
            };

            string content = await SendRequest(data, url);

            return content;
        }

        private async Task UserAlive()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/user/alive";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now()
            };

            await SendRequest(data, url);
        }

        public async Task UserLeft()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/user/left";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now()
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Fabric Events 

        public async Task<string> FabricSetup()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/setup";

            List<object> localIceCandidatesList = new List<object>();
            object localIceCandidate = new
            {
                id = "1",
                type = "localcandidate",
                ip = "127.0.0.1",
                port = 8888,
                candidateType = "host",
                transport = "tcp"
            };
            localIceCandidatesList.Add(localIceCandidate);

            List<object> remoteIceCandidatesList = new List<object>();
            object remoteIceCandidate = new
            {
                id = "2",
                type = "remotecandidate",
                ip = "127.0.0.2",
                port = 8888,
                candidateType = "host",
                transport = "tcp"
            };
            remoteIceCandidatesList.Add(remoteIceCandidate);

            List<object> iceCandidatePairsList = new List<object>();
            object iceCandidatePair = new
            {
                id = "3",
                localCandidateId = "1",
                remoteCandidateId = "2",
                state = "succeeded",
                priority = 1,
                nominated = true
            };
            iceCandidatePairsList.Add(iceCandidatePair);

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                delay = 0,
                connectionID = "connectionID",
                iceGatheringDelay = 0,
                iceConnectivityDelay = 0,
                fabricTransmissionDirection = "sendrecv",
                remoteEndpointType = "peer",
                localIceCandidates = localIceCandidatesList,
                remoteIceCandidates = remoteIceCandidatesList,
                iceCandidatePairs = iceCandidatePairsList
            };

            string fabricContent = await SendRequest(data, url);

            return DeserializeJson<FabricResponse>(fabricContent).status;
        }

        public async Task<string> FabricSetupFailed()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/setupfailed";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                fabricTransmissionDirection = "sendrecv",
                remoteEndpointType = "peer",
                reason = "MediaConfigError",
                name = "name",
                stack = "stack"
            };

            string fabricContent = await SendRequest(data, url);

            return DeserializeJson<FabricResponse>(fabricContent).status;
        }

        public async Task FabricTerminated()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/terminated";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Stats Submission

        public async Task ConferenceStatsSubmission()
        {
            var url = $"https://stats.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/stats";

            List<object> statsList = new List<object>();
            object objStats = new
            {
                tracks = "track",
                candidatePairs = "3",
                timestamp = TimeStamp.Now()
            };
            statsList.Add(objStats);

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                connectionID = ucID,
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                stats = statsList
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Special Events

        private async Task SSRCMap()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ssrcmap";

            List<object> ssrcDataList = new List<object>();
            object ssrcData = new
            {
                ssrc = "1",
                cname = "cname",
                streamType = "inbound",
                reportType = "local",
                userID = "userID",
                msid = "msid",
                mslabel = "mslabel",
                label = "label",
                localStartTime = TimeStamp.Now()
            };
            ssrcDataList.Add(ssrcData);

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                connectionID = "connectionID",
                remoteID = "remoteID",
                ssrcData = ssrcDataList
            };

            await SendRequest(data, url);
        }

        #endregion

        private async Task<string> SendRequest(object data, string url)
        {
            string dataContent = JsonConvert.SerializeObject(data);

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Version = new Version(2, 0);

            byte[] buffer = Encoding.UTF8.GetBytes(dataContent);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            req.Content = byteContent;

            HttpResponseMessage res = await client.SendAsync(req);

            string content = await res.Content.ReadAsStringAsync();

            Debug.WriteLine($"SendRequest content: {content}");

            return content;
        }

        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
