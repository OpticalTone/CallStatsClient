using CallStatsLib;
using CallStatsLib.Request;
using System.Collections.Generic;

namespace CallStatsClient
{
    public static class TestData
    {
        private static string _localID = Config.localSettings.Values["localID"].ToString();
        private static string _originID = Config.localSettings.Values["originID"].ToString();
        private static string _deviceID = Config.localSettings.Values["deviceID"].ToString();

        private enum EndpointInfoType
        {
            browser, native, plugin, middlebox
        }

        public static CreateConferenceData CreateConference()
        {
            CreateConferenceData createConferenceData = new CreateConferenceData();
            createConferenceData.localID = _localID;
            createConferenceData.originID = _originID; 
            createConferenceData.deviceID = _deviceID;
            createConferenceData.timestamp = TimeStamp.Now();

            EndpointInfo endpointInfo = new EndpointInfo();
            endpointInfo.type = EndpointInfoType.browser.ToString();
            endpointInfo.os = "";
            endpointInfo.osVersion = "";
            endpointInfo.buildName = "";
            endpointInfo.buildVersion = "";
            endpointInfo.appVersion = "";

            createConferenceData.endpointInfo = endpointInfo;

            return createConferenceData;
        }

        public static UserAliveData UserAlive()
        {
            UserAliveData userAliveData = new UserAliveData();
            userAliveData.localID = _localID;
            userAliveData.originID = _originID;
            userAliveData.deviceID = _deviceID;
            userAliveData.timestamp = TimeStamp.Now();

            return userAliveData;
        }

        private enum FabricTransmissionDirection
        {
            sendrecv, sendonly, receiveonly
        }

        private enum RemoteEndpointType
        {
            peer, server
        }

        private enum IceCandidateType
        {
            host, srflx, prflx, relay, stun, serverreflexive,
            peerreflexive, turn, relayed, local
        }

        private enum IceCandidateTransport
        {
            tcp, udp
        }

        private enum IceCandidateState
        {
            frozen, waiting, inprogress, failed, succeeded, cancelled
        }

        private enum LocalIceCandidateType
        {
            localcandidate,
            // "local-candidate"
        }

        private enum RemoteIceCandidateType
        {
            remotecandidate,
            // "remote-candidate"
        }

        public static FabricSetupData FabricSetup()
        {
            List<IceCandidate> localIceCandidatesList = new List<IceCandidate>();
            IceCandidate localIceCandidateObj = new IceCandidate();
            localIceCandidateObj.id = "1";
            localIceCandidateObj.type = LocalIceCandidateType.localcandidate.ToString();
            localIceCandidateObj.ip = "127.0.0.1";
            localIceCandidateObj.port = 8888;
            localIceCandidateObj.candidateType = IceCandidateType.host.ToString();
            localIceCandidateObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidateObj);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidateObj = new IceCandidate();
            remoteIceCandidateObj.id = "2";
            remoteIceCandidateObj.type = RemoteIceCandidateType.remotecandidate.ToString();
            remoteIceCandidateObj.ip = "127.0.0.2";
            remoteIceCandidateObj.port = 8888;
            remoteIceCandidateObj.candidateType = IceCandidateType.host.ToString();
            remoteIceCandidateObj.transport = IceCandidateTransport.tcp.ToString();
            remoteIceCandidatesList.Add(remoteIceCandidateObj);

            List<IceCandidatePair> iceCandidatePairsList = new List<IceCandidatePair>();
            IceCandidatePair iceCandidatePairObj = new IceCandidatePair();
            iceCandidatePairObj.id = "3";
            iceCandidatePairObj.localCandidateId = "1";
            iceCandidatePairObj.remoteCandidateId = "2";
            iceCandidatePairObj.state = IceCandidateState.succeeded.ToString();
            iceCandidatePairObj.priority = 1;
            iceCandidatePairObj.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePairObj);

            FabricSetupData fabricSetupData = new FabricSetupData();
            fabricSetupData.localID = _localID; 
            fabricSetupData.originID = _originID; 
            fabricSetupData.deviceID = _deviceID; 
            fabricSetupData.timestamp = TimeStamp.Now();
            fabricSetupData.remoteID = "remoteID";
            fabricSetupData.delay = 0;
            fabricSetupData.iceGatheringDelay = 0;
            fabricSetupData.iceConnectivityDelay = 0;
            fabricSetupData.fabricTransmissionDirection = FabricTransmissionDirection.sendrecv.ToString();
            fabricSetupData.remoteEndpointType = RemoteEndpointType.peer.ToString();
            fabricSetupData.localIceCandidates = localIceCandidatesList;
            fabricSetupData.remoteIceCandidates = remoteIceCandidatesList;
            fabricSetupData.iceCandidatePairs = iceCandidatePairsList;

            return fabricSetupData;
        }

        private enum FabricSetupFailedReason
        {
            MediaConfigError, MediaPermissionError, MediaDeviceError,
            NegotiationFailure, SDPGenerationError, TransportFailure,
            SignalingError, IceConnectionFailure
        }

        public static FabricSetupFailedData FabricSetupFailed()
        {
            FabricSetupFailedData fabricSetupFailedData = new FabricSetupFailedData();
            fabricSetupFailedData.localID = _localID;
            fabricSetupFailedData.originID = _originID;
            fabricSetupFailedData.deviceID = _deviceID;
            fabricSetupFailedData.timestamp = TimeStamp.Now();
            fabricSetupFailedData.fabricTransmissionDirection = FabricTransmissionDirection.sendrecv.ToString();
            fabricSetupFailedData.remoteEndpointType = RemoteEndpointType.peer.ToString();
            fabricSetupFailedData.reason = FabricSetupFailedReason.MediaConfigError.ToString();
            fabricSetupFailedData.name = "name";
            fabricSetupFailedData.message = "message";
            fabricSetupFailedData.stack = "stack";

            return fabricSetupFailedData;
        }

        private enum StreamType
        {
            inbound, outbound
        }

        private enum ReportType
        {
            local, remote
        }

        private enum MediaType
        {
            audio, video, screen
        }

        public static SSRCMapData SSRCMap()
        {
            List<SSRCData> ssrcDataList = new List<SSRCData>();
            SSRCData ssrcData = new SSRCData();
            ssrcData.ssrc = "1";
            ssrcData.cname = "cname";
            ssrcData.streamType = StreamType.inbound.ToString();
            ssrcData.reportType = ReportType.local.ToString();
            ssrcData.mediaType = MediaType.audio.ToString();
            ssrcData.userID = "userID";
            ssrcData.msid = "msid";
            ssrcData.mslabel = "mslabel";
            ssrcData.label = "label";
            ssrcData.localStartTime = TimeStamp.Now();
            ssrcDataList.Add(ssrcData);

            SSRCMapData ssrcMapData = new SSRCMapData();
            ssrcMapData.localID = _localID;
            ssrcMapData.originID = _originID;
            ssrcMapData.deviceID = _deviceID;
            ssrcMapData.timestamp = TimeStamp.Now();
            ssrcMapData.remoteID = "remoteID";
            ssrcMapData.ssrcData = ssrcDataList;

            return ssrcMapData;
        }

        public static ConferenceStatsSubmissionData ConferenceStatsSubmission()
        {
            List<Stats> statsList = new List<Stats>();
            Stats stats = new Stats();
            stats.tracks = "track";
            stats.candidatePairs = "3";
            stats.timestamp = TimeStamp.Now();
            statsList.Add(stats);

            ConferenceStatsSubmissionData conferenceStatsSubmissionData = new ConferenceStatsSubmissionData();
            conferenceStatsSubmissionData.localID = _localID;
            conferenceStatsSubmissionData.originID = _originID;
            conferenceStatsSubmissionData.deviceID = _deviceID;
            conferenceStatsSubmissionData.timestamp = TimeStamp.Now();
            conferenceStatsSubmissionData.remoteID = "remoteID";
            conferenceStatsSubmissionData.stats = statsList;

            return conferenceStatsSubmissionData;
        }

        public static FabricTerminatedData FabricTerminated()
        {
            FabricTerminatedData fabricTerminatedData = new FabricTerminatedData();
            fabricTerminatedData.localID = _localID;
            fabricTerminatedData.originID = _originID;
            fabricTerminatedData.deviceID = _deviceID;
            fabricTerminatedData.timestamp = TimeStamp.Now();
            fabricTerminatedData.remoteID = "remoteID";

            return fabricTerminatedData;
        }

        public static UserLeftData UserLeft()
        {
            UserLeftData userLeftData = new UserLeftData();
            userLeftData.localID = _localID;
            userLeftData.originID = _originID;
            userLeftData.deviceID = _deviceID;
            userLeftData.timestamp = TimeStamp.Now();

            return userLeftData;
        }

        public static UserDetailsData UserDetails()
        {
            UserDetailsData userDetailsData = new UserDetailsData();
            userDetailsData.localID = _localID;
            userDetailsData.originID = _originID;
            userDetailsData.deviceID = _deviceID;
            userDetailsData.timestamp = TimeStamp.Now();
            userDetailsData.userName = "userName";

            return userDetailsData;
        }

        private enum StateChange
        {
            stable,
            //"have-local-offer",
            //"have-remote-offer",
            //"have-local-pranswer",
            //"have-remote-pranswer",
            closed,
            //"new",
            connecting,
            connected,
            disconnected,
            failed,
            checking,
            completed,
            gathering,
            complete
        }

        private enum ChangedState
        {
            signalingState, connectionState,
            iceConnectionState, iceGatheringState
        }

        public static FabricStateChangeData FabricStateChange()
        {
            FabricStateChangeData fabricStateChangeData = new FabricStateChangeData();
            fabricStateChangeData.localID = _localID;
            fabricStateChangeData.originID = _originID;
            fabricStateChangeData.deviceID = _deviceID;
            fabricStateChangeData.timestamp = TimeStamp.Now();
            fabricStateChangeData.remoteID = "remoteID";
            fabricStateChangeData.prevState = StateChange.stable.ToString();
            fabricStateChangeData.newState = StateChange.closed.ToString();
            fabricStateChangeData.changedState = ChangedState.signalingState.ToString();

            return fabricStateChangeData;
        }

        private enum ConnectionState
        {
            connected, completed
        }

        public static FabricTransportChangeData FabricTransportChange()
        {
            IceCandidatePair currIceCandidatePairObj = new IceCandidatePair();
            currIceCandidatePairObj.id = "4";
            currIceCandidatePairObj.localCandidateId = "1";
            currIceCandidatePairObj.remoteCandidateId = "2";
            currIceCandidatePairObj.state = IceCandidateState.succeeded.ToString();
            currIceCandidatePairObj.priority = 1;
            currIceCandidatePairObj.nominated = true;

            IceCandidatePair prevIceCandidatePairObj = new IceCandidatePair();
            prevIceCandidatePairObj.id = "6";
            prevIceCandidatePairObj.localCandidateId = "7";
            prevIceCandidatePairObj.remoteCandidateId = "8";
            prevIceCandidatePairObj.state = IceCandidateState.failed.ToString();
            prevIceCandidatePairObj.priority = 1;
            prevIceCandidatePairObj.nominated = true;

            FabricTransportChangeData fabricTransportChangeData = new FabricTransportChangeData();
            fabricTransportChangeData.localID = _localID;
            fabricTransportChangeData.originID = _originID;
            fabricTransportChangeData.deviceID = _deviceID;
            fabricTransportChangeData.timestamp = TimeStamp.Now();
            fabricTransportChangeData.remoteID = "remoteID";
            fabricTransportChangeData.currIceCandidatePair = currIceCandidatePairObj;
            fabricTransportChangeData.prevIceCandidatePair = prevIceCandidatePairObj;
            fabricTransportChangeData.currIceConnectionState = ConnectionState.completed.ToString();
            fabricTransportChangeData.prevIceConnectionState = ConnectionState.connected.ToString();
            fabricTransportChangeData.delay = 0;
            fabricTransportChangeData.relayType = "turn/tcp"; // "turn/udp" "turn/tcp" "turn/tls"

            return fabricTransportChangeData;
        }

        public static FabricDroppedData FabricDropped()
        {
            IceCandidatePair currIceCandidatePairObj = new IceCandidatePair();
            currIceCandidatePairObj.id = "4";
            currIceCandidatePairObj.localCandidateId = "1";
            currIceCandidatePairObj.remoteCandidateId = "2";
            currIceCandidatePairObj.state = IceCandidateState.succeeded.ToString();
            currIceCandidatePairObj.priority = 4;
            currIceCandidatePairObj.nominated = true;

            FabricDroppedData fabricDroppedData = new FabricDroppedData();
            fabricDroppedData.localID = _localID;
            fabricDroppedData.originID = _originID;
            fabricDroppedData.deviceID = _deviceID;
            fabricDroppedData.timestamp = TimeStamp.Now();
            fabricDroppedData.remoteID = "remoteID";
            fabricDroppedData.currIceCandidatePair = currIceCandidatePairObj;
            fabricDroppedData.currIceConnectionState = "failed";
            fabricDroppedData.prevIceConnectionState = "disconnected";
            fabricDroppedData.delay = 0;

            return fabricDroppedData;
        }

        private enum EventType
        {
            fabricHold, fabricResume
        }

        public static FabricActionData FabricAction()
        {
            FabricActionData fabricActionData = new FabricActionData();
            fabricActionData.eventType = EventType.fabricHold.ToString();
            fabricActionData.localID = _localID;
            fabricActionData.originID = _originID;
            fabricActionData.deviceID = _deviceID;
            fabricActionData.timestamp = TimeStamp.Now();
            fabricActionData.remoteID = "remoteID";

            return fabricActionData;
        }

        public static SystemStatusStatsSubmissionData SystemStatusStatsSubmission() // statusCode: MethodNotAllowed
        {
            SystemStatusStatsSubmissionData systemStatusStatsSubmissionData = new SystemStatusStatsSubmissionData();
            systemStatusStatsSubmissionData.localID = _localID;
            systemStatusStatsSubmissionData.originID = _originID;
            systemStatusStatsSubmissionData.deviceID = _deviceID;
            systemStatusStatsSubmissionData.timestamp = TimeStamp.Now();
            systemStatusStatsSubmissionData.cpuUsage = 5;
            systemStatusStatsSubmissionData.batteryLevel = 50;
            systemStatusStatsSubmissionData.memoryUsage = 2;
            systemStatusStatsSubmissionData.totalMemory = 100;
            systemStatusStatsSubmissionData.threadCount = 1;

            return systemStatusStatsSubmissionData;
        }

        public static IceDisruptionStartData IceDisruptionStart()
        {
            IceCandidatePair currIceCandidatePairObj = new IceCandidatePair();
            currIceCandidatePairObj.id = "4";
            currIceCandidatePairObj.localCandidateId = "1";
            currIceCandidatePairObj.remoteCandidateId = "2";
            currIceCandidatePairObj.state = IceCandidateState.frozen.ToString();
            currIceCandidatePairObj.priority = 2;
            currIceCandidatePairObj.nominated = true;

            IceDisruptionStartData iceDisruptionStartData = new IceDisruptionStartData();
            iceDisruptionStartData.eventType = "iceDisruptionStart";
            iceDisruptionStartData.localID = _localID;
            iceDisruptionStartData.originID = _originID;
            iceDisruptionStartData.deviceID = _deviceID;
            iceDisruptionStartData.timestamp = TimeStamp.Now();
            iceDisruptionStartData.remoteID = "remoteID";
            iceDisruptionStartData.currIceCandidatePair = currIceCandidatePairObj;
            iceDisruptionStartData.currIceConnectionState = "disconnected";
            iceDisruptionStartData.prevIceConnectionState = ConnectionState.completed.ToString();

            return iceDisruptionStartData;
        }

        private enum IceDisruptionEndConnectionState
        {
            connected, completed, checking
        }

        public static IceDisruptionEndData IceDisruptionEnd()
        {
            IceCandidatePair currIceCandidatePairObj = new IceCandidatePair();
            currIceCandidatePairObj.id = "4";
            currIceCandidatePairObj.localCandidateId = "1";
            currIceCandidatePairObj.remoteCandidateId = "2";
            currIceCandidatePairObj.state = IceCandidateState.inprogress.ToString();
            currIceCandidatePairObj.priority = 1;
            currIceCandidatePairObj.nominated = true;

            IceCandidatePair prevIceCandidatePairObj = new IceCandidatePair();
            prevIceCandidatePairObj.id = "0";
            prevIceCandidatePairObj.localCandidateId = "0";
            prevIceCandidatePairObj.remoteCandidateId = "0";
            prevIceCandidatePairObj.state = IceCandidateState.frozen.ToString();
            prevIceCandidatePairObj.priority = 2;
            prevIceCandidatePairObj.nominated = false;

            IceDisruptionEndData iceDisruptionEndData = new IceDisruptionEndData();
            iceDisruptionEndData.eventType = "iceDisruptionEnd";
            iceDisruptionEndData.localID = _localID;
            iceDisruptionEndData.originID = _originID;
            iceDisruptionEndData.deviceID = _deviceID;
            iceDisruptionEndData.timestamp = TimeStamp.Now();
            iceDisruptionEndData.remoteID = "remoteID";
            iceDisruptionEndData.currIceCandidatePair = currIceCandidatePairObj;
            iceDisruptionEndData.prevIceCandidatePair = prevIceCandidatePairObj;
            iceDisruptionEndData.currIceConnectionState = IceDisruptionEndConnectionState.checking.ToString();
            iceDisruptionEndData.prevIceConnectionState = "disconnected";
            iceDisruptionEndData.delay = 2;

            return iceDisruptionEndData;
        }

        private enum IceRestartConnectionState
        {
            checking, connected, completed,
            failed, disconnected, closed
        }

        public static IceRestartData IceRestart()
        {
            IceCandidatePair prevIceCandidatePairObj = new IceCandidatePair();
            prevIceCandidatePairObj.id = "0";
            prevIceCandidatePairObj.localCandidateId = "0";
            prevIceCandidatePairObj.remoteCandidateId = "0";
            prevIceCandidatePairObj.state = IceCandidateState.frozen.ToString();
            prevIceCandidatePairObj.priority = 1;
            prevIceCandidatePairObj.nominated = true;

            IceRestartData iceRestartData = new IceRestartData();
            iceRestartData.eventType = "iceRestarted";
            iceRestartData.localID = _localID;
            iceRestartData.originID = _originID;
            iceRestartData.deviceID = _deviceID;
            iceRestartData.timestamp = TimeStamp.Now();
            iceRestartData.remoteID = "remoteID";
            iceRestartData.prevIceCandidatePair = prevIceCandidatePairObj;
            iceRestartData.currIceConnectionState = "new";
            iceRestartData.prevIceConnectionState = IceRestartConnectionState.checking.ToString();

            return iceRestartData;
        }

        private enum IceFailedConnectionState
        {
            checking, disconnected
        }

        public static IceFailedData IceFailed()
        {
            List<IceCandidate> localIceCandidatesList = new List<IceCandidate>();
            IceCandidate localIceCandidatesObj = new IceCandidate();
            localIceCandidatesObj.id = "1";
            localIceCandidatesObj.type = LocalIceCandidateType.localcandidate.ToString();
            localIceCandidatesObj.ip = "127.0.0.1";
            localIceCandidatesObj.port = 8888;
            localIceCandidatesObj.candidateType = IceCandidateType.host.ToString();
            localIceCandidatesObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidatesObj);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidatesObj = new IceCandidate();
            remoteIceCandidatesObj.id = "2";
            remoteIceCandidatesObj.type = LocalIceCandidateType.localcandidate.ToString(); // Documentation: RemoteIceCandidateType.remotecandidate.ToString(); 
            remoteIceCandidatesObj.ip = "127.0.0.2";
            remoteIceCandidatesObj.port = 8888;
            remoteIceCandidatesObj.candidateType = IceCandidateType.host.ToString();
            remoteIceCandidatesObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(remoteIceCandidatesObj);

            List<IceCandidatePair> iceCandidatePairsList = new List<IceCandidatePair>();
            IceCandidatePair iceCandidatePairObj = new IceCandidatePair();
            iceCandidatePairObj.id = "3";
            iceCandidatePairObj.localCandidateId = "1";
            iceCandidatePairObj.remoteCandidateId = "2";
            iceCandidatePairObj.state = IceCandidateState.failed.ToString();
            iceCandidatePairObj.priority = 2;
            iceCandidatePairObj.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePairObj);

            IceFailedData iceFailedData = new IceFailedData();
            iceFailedData.eventType = "iceFailed";
            iceFailedData.localID = _localID;
            iceFailedData.originID = _originID;
            iceFailedData.deviceID = _deviceID;
            iceFailedData.timestamp = TimeStamp.Now();
            iceFailedData.remoteID = "remoteID";
            iceFailedData.localIceCandidates = localIceCandidatesList;
            iceFailedData.remoteIceCandidates = remoteIceCandidatesList;
            iceFailedData.iceCandidatePairs = iceCandidatePairsList;
            iceFailedData.currIceConnectionState = "failed";
            iceFailedData.prevIceConnectionState = IceFailedConnectionState.disconnected.ToString();
            iceFailedData.delay = 2;

            return iceFailedData;
        }

        private enum IceAbortedConnectionState
        {
            checking, //"new"
        }

        public static IceAbortedData IceAborted()
        {
            List<IceCandidate> localIceCandidatesList = new List<IceCandidate>();
            IceCandidate localIceCandidatesObj = new IceCandidate();
            localIceCandidatesObj.id = "1";
            localIceCandidatesObj.type = LocalIceCandidateType.localcandidate.ToString();
            localIceCandidatesObj.ip = "127.0.0.1";
            localIceCandidatesObj.port = 8888;
            localIceCandidatesObj.candidateType = IceCandidateType.host.ToString();
            localIceCandidatesObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidatesObj);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidatesObj = new IceCandidate();
            remoteIceCandidatesObj.id = "2";
            remoteIceCandidatesObj.type = LocalIceCandidateType.localcandidate.ToString(); // Documentation: RemoteIceCandidateType.remotecandidate.ToString();  
            remoteIceCandidatesObj.ip = "127.0.0.2";
            remoteIceCandidatesObj.port = 8888;
            remoteIceCandidatesObj.candidateType = IceCandidateType.host.ToString();
            remoteIceCandidatesObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(remoteIceCandidatesObj);

            List<IceCandidatePair> iceCandidatePairsList = new List<IceCandidatePair>();
            IceCandidatePair iceCandidatePairObj = new IceCandidatePair();
            iceCandidatePairObj.id = "3";
            iceCandidatePairObj.localCandidateId = "1";
            iceCandidatePairObj.remoteCandidateId = "2";
            iceCandidatePairObj.state = IceCandidateState.failed.ToString();
            iceCandidatePairObj.priority = 2;
            iceCandidatePairObj.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePairObj);

            IceAbortedData iceAbortedData = new IceAbortedData();
            iceAbortedData.eventType = "iceFailed";
            iceAbortedData.localID = _localID;
            iceAbortedData.originID = _originID;
            iceAbortedData.deviceID = _deviceID;
            iceAbortedData.timestamp = TimeStamp.Now();
            iceAbortedData.remoteID = "remoteID";
            iceAbortedData.localIceCandidates = localIceCandidatesList;
            iceAbortedData.remoteIceCandidates = remoteIceCandidatesList;
            iceAbortedData.iceCandidatePairs = iceCandidatePairsList;
            iceAbortedData.currIceConnectionState = "failed"; // Documentation: "closed";
            iceAbortedData.prevIceConnectionState = IceAbortedConnectionState.checking.ToString();
            iceAbortedData.delay = 2;

            return iceAbortedData;
        }

        private enum IceTerminatedConnectionState
        {
            connected, completed, failed, disconnected
        }

        public static IceTerminatedData IceTerminated()
        {
            IceCandidatePair iceCandidatePairObj = new IceCandidatePair();
            iceCandidatePairObj.id = "3";
            iceCandidatePairObj.localCandidateId = "1";
            iceCandidatePairObj.remoteCandidateId = "2";
            iceCandidatePairObj.state = IceCandidateState.failed.ToString();
            iceCandidatePairObj.priority = 2;
            iceCandidatePairObj.nominated = true;

            IceTerminatedData iceTerminatedData = new IceTerminatedData();
            iceTerminatedData.eventType = "iceTerminated";
            iceTerminatedData.localID = _localID;
            iceTerminatedData.originID = _originID;
            iceTerminatedData.deviceID = _deviceID;
            iceTerminatedData.timestamp = TimeStamp.Now();
            iceTerminatedData.remoteID = "remoteID";
            iceTerminatedData.prevIceCandidatePair = iceCandidatePairObj;
            iceTerminatedData.currIceConnectionState = "closed";
            iceTerminatedData.prevIceConnectionState = IceTerminatedConnectionState.failed.ToString();

            return iceTerminatedData;
        }

        public static IceConnectionDisruptionStartData IceConnectionDisruptionStart()
        {
            IceConnectionDisruptionStartData iceConnectionDisruptionStartData = new IceConnectionDisruptionStartData();
            iceConnectionDisruptionStartData.eventType = "iceConnectionDisruptionStart";
            iceConnectionDisruptionStartData.localID = _localID;
            iceConnectionDisruptionStartData.originID = _originID;
            iceConnectionDisruptionStartData.deviceID = _deviceID;
            iceConnectionDisruptionStartData.timestamp = TimeStamp.Now();
            iceConnectionDisruptionStartData.remoteID = "remoteID";
            iceConnectionDisruptionStartData.currIceConnectionState = "disconnected";
            iceConnectionDisruptionStartData.prevIceConnectionState = "checking";

            return iceConnectionDisruptionStartData;
        }

        public static IceConnectionDisruptionEndData IceConnectionDisruptionEnd()
        {
            IceConnectionDisruptionEndData iceConnectionDisruptionEndData = new IceConnectionDisruptionEndData();
            iceConnectionDisruptionEndData.eventType = "iceConnectionDisruptionEnd";
            iceConnectionDisruptionEndData.localID = _localID;
            iceConnectionDisruptionEndData.originID = _originID;
            iceConnectionDisruptionEndData.deviceID = _deviceID;
            iceConnectionDisruptionEndData.timestamp = TimeStamp.Now();
            iceConnectionDisruptionEndData.remoteID = "remoteID";
            iceConnectionDisruptionEndData.currIceConnectionState = "checking";
            iceConnectionDisruptionEndData.prevIceConnectionState = "disconnected";
            iceConnectionDisruptionEndData.delay = 2;

            return iceConnectionDisruptionEndData;
        }

        private enum MediaActionEventType
        {
            audioMute, audioUnmute, screenShareStart,
            screenShareStop, videoPause, videoResume
        }

        public static MediaActionData MediaAction()
        {
            List<string> remoteIDList = new List<string>();
            remoteIDList.Add("remoteID1");
            remoteIDList.Add("remoteID2");

            MediaActionData mediaActionData = new MediaActionData();
            mediaActionData.eventType = MediaActionEventType.audioMute.ToString();
            mediaActionData.localID = _localID;
            mediaActionData.originID = _originID;
            mediaActionData.deviceID = _deviceID;
            mediaActionData.timestamp = TimeStamp.Now();
            mediaActionData.remoteID = "remoteID";
            mediaActionData.ssrc = "11";
            mediaActionData.mediaDeviceID = "mediaDeviceID";
            mediaActionData.remoteIDList = remoteIDList;

            return mediaActionData;
        }

        private enum MediaPlaybackEventType
        {
            mediaPlaybackStart, mediaPlaybackSuspended,
            mediaPlaybackStalled, oneWayMedia
        }

        private enum MediaPlaybackMediaType
        {
            audio, video, screen
        }

        public static MediaPlaybackData MediaPlayback()
        {
            MediaPlaybackData mediaPlaybackData = new MediaPlaybackData();
            mediaPlaybackData.eventType = MediaPlaybackEventType.mediaPlaybackStart.ToString();
            mediaPlaybackData.localID = _localID;
            mediaPlaybackData.originID = _originID;
            mediaPlaybackData.deviceID = _deviceID;
            mediaPlaybackData.timestamp = TimeStamp.Now();
            mediaPlaybackData.remoteID = "remoteID";    // Documentation: This field is not in docs, also connectionID
            mediaPlaybackData.mediaType = MediaPlaybackMediaType.audio.ToString();
            mediaPlaybackData.ssrc = "11";

            return mediaPlaybackData;
        }

        private enum MediaDeviceKind
        {
            audioinput, audiooutput, videoinput
        }

        private enum ConnectedOrActiveDevicesEventType
        {
            connectedDeviceList, activeDeviceList
        }

        public static ConnectedOrActiveDevicesData ConnectedOrActiveDevices()
        {
            List<MediaDevice> mediaDeviceList = new List<MediaDevice>();
            MediaDevice mediaDeviceObj = new MediaDevice();
            mediaDeviceObj.mediaDeviceID = "mediaDeviceID";
            mediaDeviceObj.kind = MediaDeviceKind.videoinput.ToString();
            mediaDeviceObj.label = "external USB Webcam";
            mediaDeviceObj.groupID = "groupID";
            mediaDeviceList.Add(mediaDeviceObj);

            ConnectedOrActiveDevicesData connectedOrActiveDevicesData = new ConnectedOrActiveDevicesData();
            connectedOrActiveDevicesData.localID = _localID;
            connectedOrActiveDevicesData.originID = _originID;
            connectedOrActiveDevicesData.deviceID = _deviceID;
            connectedOrActiveDevicesData.timestamp = TimeStamp.Now();
            connectedOrActiveDevicesData.mediaDeviceList = mediaDeviceList;
            connectedOrActiveDevicesData.eventType = ConnectedOrActiveDevicesEventType.activeDeviceList.ToString();

            return connectedOrActiveDevicesData;
        }

        private enum ErrorLogsLevel
        {
            debug, info, warn, error, fatal
        }

        private enum ErrorLogsMessageType
        {
            text, json, domError
        }

        public static ApplicationErrorLogsData ApplicationErrorLogs()
        {
            ApplicationErrorLogsData applicationErrorLogsData = new ApplicationErrorLogsData();
            applicationErrorLogsData.localID = _localID;
            applicationErrorLogsData.originID = _originID;
            applicationErrorLogsData.deviceID = _deviceID;
            applicationErrorLogsData.timestamp = TimeStamp.Now();
            applicationErrorLogsData.level = ErrorLogsLevel.debug.ToString();
            applicationErrorLogsData.message = "Application error message";
            applicationErrorLogsData.messageType = ErrorLogsMessageType.json.ToString();

            return applicationErrorLogsData;
        }

        public static ConferenceUserFeedbackData ConferenceUserFeedback()
        {
            Feedback feedbackObj = new Feedback();
            feedbackObj.overallRating = 4;
            feedbackObj.videoQualityRating = 3;
            feedbackObj.audioQualityRating = 5;
            feedbackObj.comments = "Comments from the participant";

            ConferenceUserFeedbackData conferenceUserFeedbackData = new ConferenceUserFeedbackData();
            conferenceUserFeedbackData.localID = _localID;
            conferenceUserFeedbackData.originID = _originID;
            conferenceUserFeedbackData.deviceID = _deviceID;
            conferenceUserFeedbackData.timestamp = TimeStamp.Now();
            conferenceUserFeedbackData.remoteID = "remoteID";
            conferenceUserFeedbackData.feedback = feedbackObj;

            return conferenceUserFeedbackData;
        }

        public static DominantSpeakerData DominantSpeaker()
        {
            DominantSpeakerData dominantSpeakerData = new DominantSpeakerData();
            dominantSpeakerData.localID = _localID;
            dominantSpeakerData.originID = _originID;
            dominantSpeakerData.deviceID = _deviceID;
            dominantSpeakerData.timestamp = TimeStamp.Now();

            return dominantSpeakerData;
        }
    }
}
