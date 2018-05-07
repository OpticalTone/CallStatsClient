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

            string confContent = await CreateConference();
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

        private async Task<string> CreateConference()
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

        private async Task UserDetails()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/userdetails";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                userName = "userName"
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

        public async Task FabricStateChange()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/statechange";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                prevState = "stable",
                newState = "have-local-offer",
                changedState = "signalingState"
            };

            await SendRequest(data, url);
        }

        public async Task FabricTransportChange()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/transportchange";

            List<object> currIceCandidatePairList = new List<object>();
            object currIceCandidateObj = new
            {
                id = "4",
                localCandidateId = "1",
                remoteCandidateId = "2",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            currIceCandidatePairList.Add(currIceCandidateObj);

            List<object> prevIceCandidatePairList = new List<object>();
            object prevIceCandidatePairObj = new
            {
                id = "0",
                localCandidateId = "0",
                remoteCandidateId = "0",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            prevIceCandidatePairList.Add(prevIceCandidatePairObj);

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceCandidatePair = currIceCandidatePairList,
                prevIceCandidatePair = prevIceCandidatePairObj,
                currIceConnectionState = "completed",
                prevIceConnectionState = "connected",
                delay = 0,
                relayType = "turn/tcp"
            };

            await SendRequest(data, url);
        }

        public async Task FabricDropped()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/status";

            List<object> currIceCandidatePairList = new List<object>();
            object currIceCandidateObj = new
            {
                id = "4",
                localCandidateId = "1",
                remoteCandidateId = "2",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            currIceCandidatePairList.Add(currIceCandidateObj);

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceCandidatePair = currIceCandidatePairList,
                currIceConnectionState = "failed",
                prevIceConnectionState = "disconnected",
                delay = 0
            };

            await SendRequest(data, url);
        }

        public async Task FabricAction()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/fabric/actions";

            object data = new
            {
                eventType = "fabricHold",
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

        public async Task SystemStatusStatsSubmission()
        {
            string url = $"https://stats.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/stats/system";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                cpuUsage = 5,
                batteryLevel = 50,
                memoryUsage = 2,
                totalMemory = 100,
                threadCount = 1
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Ice Events 

        public async Task IceDisruptionStart()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            List<object> currIceCandidatePairList = new List<object>();
            object currIceCandidateObj = new
            {
                id = "4",
                localCandidateId = "1",
                remoteCandidateId = "2",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            currIceCandidatePairList.Add(currIceCandidateObj);

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceCandidatePair = currIceCandidatePairList,
                currIceConnectionState = "disconnected",
                prevIceConnectionState = "completed"
            };
            
            await SendRequest(data, url);
        }

        public async Task IceDisruptionEnd()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            List<object> currIceCandidatePairList = new List<object>();
            object currIceCandidateObj = new
            {
                id = "4",
                localCandidateId = "1",
                remoteCandidateId = "2",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            currIceCandidatePairList.Add(currIceCandidateObj);

            List<object> prevIceCandidatePairList = new List<object>();
            object prevIceCandidatePairObj = new
            {
                id = "0",
                localCandidateId = "0",
                remoteCandidateId = "0",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            prevIceCandidatePairList.Add(prevIceCandidatePairObj);

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceCandidatePair = currIceCandidatePairList,
                prevIceCandidatePair = prevIceCandidatePairList,
                currIceConnectionState = "connected",
                prevIceConnectionState = "disconnected",
                delay = 0
            };

            await SendRequest(data, url);
        }

        public async Task IceRestart()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            List<object> prevIceCandidatePairList = new List<object>();
            object prevIceCandidatePairObj = new
            {
                id = "0",
                localCandidateId = "0",
                remoteCandidateId = "0",
                state = "frozen",
                priority = 1,
                nominated = true
            };
            prevIceCandidatePairList.Add(prevIceCandidatePairObj);

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                prevIceCandidatePair = prevIceCandidatePairList,
                currIceConnectionState = "new",
                prevIceConnectionState = "completed"
            };

            await SendRequest(data, url);
        }

        public async Task IceFailed()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

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
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                localIceCandidates = localIceCandidatesList,
                remoteIceCandidates = remoteIceCandidatesList,
                iceCandidatePairs = iceCandidatePairsList,
                currIceConnectionState = "failed",
                prevIceConnectionState = "disconnected",
                delay = 0
            };

            await SendRequest(data, url);
        }

        public async Task IceAborted()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

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
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                localIceCandidates = localIceCandidatesList,
                remoteIceCandidates = remoteIceCandidatesList,
                iceCandidatePairs = iceCandidatePairsList,
                currIceConnectionState = "closed",
                prevIceConnectionState = "new",
                delay = 0
            };

            await SendRequest(data, url);
        }

        public async Task IceTerminated()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            object prevIceCandidatePairObj = new
            {
                id = "0",
                localCandidateId = "0",
                remoteCandidateId = "0",
                state = "frozen",
                priority = 1,
                nominated = true
            };

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                prevIceCandidatePair = prevIceCandidatePairObj,
                currIceConnectionState = "closed",
                prevIceConnectionState = "connected"
            };

            await SendRequest(data, url);
        }

        public async Task IceConnectionDisruptionStart()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceConnectionState = "disconnected",
                prevIceConnectionState = "checking"
            };

            await SendRequest(data, url);
        }

        public async Task IceConnectionDisruptionEnd()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/ice/status";

            object data = new
            {
                eventType = "iceDisruptionStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                currIceConnectionState = "checking",
                prevIceConnectionState = "disconnected",
                delay = 0
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Media Events

        public async Task MediaAction()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/media/actions";

            List<string> remoteIDList = new List<string>();
            remoteIDList.Add("remoteID1");
            remoteIDList.Add("remoteID2");

            object data = new
            {
                eventType = "screenShareStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                remoteID = "remoteID",
                connectionID = ucID,
                ssrc = "11",
                mediaDeviceID = "mediaDeviceID",
                remoteIDList = remoteIDList
            };

            await SendRequest(data, url);
        }

        public async Task MediaPlayback()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/media/pipeline";

            object data = new
            {
                eventType = "mediaPlaybackStart",
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                mediaType = "mediaType",
                ssrc = "11"
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Device Events

        public async Task ConnectedOrActiveDevices()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/devices/list";

            List<object> mediaDeviceList = new List<object>();
            object mediaDevice = new
            {
                mediaDeviceID = "mediaDeviceID",
                kind = "videoinput",
                label = "external USB Webcam",
                groupID = "groupID"
            };
            mediaDeviceList.Add(mediaDevice); 

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                mediaDeviceList = mediaDeviceList,
                eventType = "connectedDeviceList"
            };

            await SendRequest(data, url);
        }

        #endregion

        #region Special Events

        public async Task ApplicationErrorLogs()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/app/logs";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                connectionID = ucID,
                level = "debug",
                message = "Application error message",
                messageType = "json"
            };

            await SendRequest(data, url);
        }

        public async Task ConferenceUserFeedback()
        {
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/feedback";

            object feedbackObj = new
            {
                overallRating = 5,
                videoQualityRating = 5,
                audioQualityRating = 5,
                comments = "comments"
            };

            object data = new
            {
                localID = localID,
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
            string url = $"https://events.callstats.io/v1/apps/{appID}/conferences/{confID}/{ucID}/events/dominantspeaker";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now()
            };

            await SendRequest(data, url);
        }

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
                connectionID = ucID,
                remoteID = "remoteID",
                ssrcData = ssrcDataList
            };

            await SendRequest(data, url);
        }

        public async Task SDPEvent()
        {
            string url = $"https://HOSTNAME/v1/apps/{appID}/conferences/{confID}/{ucID}/events/sdp";

            object data = new
            {
                localID = localID,
                originID = "originID",
                deviceID = "deviceID",
                timestamp = TimeStamp.Now(),
                connectionID = ucID,
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
            string url = $"https://stats.callstats.io/v1/apps/{appID}/stats/bridge/status";

            object data = new
            {
                localID = localID,
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
            string url = $"https://stats.callstats.io/v1/apps/{appID}/stats/bridge/alive";

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
