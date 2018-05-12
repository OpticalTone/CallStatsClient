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
            IceCandidate localIceCandidate = new IceCandidate();
            localIceCandidate.id = "1";
            localIceCandidate.type = LocalIceCandidateType.localcandidate.ToString();
            localIceCandidate.ip = "127.0.0.1";
            localIceCandidate.port = 8888;
            localIceCandidate.candidateType = IceCandidateType.host.ToString();
            localIceCandidate.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidate);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidate = new IceCandidate();
            remoteIceCandidate.id = "2";
            remoteIceCandidate.type = RemoteIceCandidateType.remotecandidate.ToString();
            remoteIceCandidate.ip = "127.0.0.2";
            remoteIceCandidate.port = 8888;
            remoteIceCandidate.candidateType = IceCandidateType.host.ToString();
            remoteIceCandidate.transport = IceCandidateTransport.tcp.ToString();
            remoteIceCandidatesList.Add(remoteIceCandidate);

            List<IceCandidatePair> iceCandidatePairsList = new List<IceCandidatePair>();
            IceCandidatePair iceCandidatePair = new IceCandidatePair();
            iceCandidatePair.id = "3";
            iceCandidatePair.localCandidateId = "1";
            iceCandidatePair.remoteCandidateId = "2";
            iceCandidatePair.state = IceCandidateState.succeeded.ToString();
            iceCandidatePair.priority = 1;
            iceCandidatePair.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePair);

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

        public static SystemStatusStatsSubmissionData SystemStatusStatsSubmission()
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
    }
}
