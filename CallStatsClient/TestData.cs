using CallStatsLib;
using CallStatsLib.Request;
using System.Collections.Generic;

namespace CallStatsClient
{
    public static class TestData
    {
        private enum EndpointInfoType
        {
            browser, native, plugin, middlebox
        }

        public static CreateConferenceData CreateConference()
        {
            CreateConferenceData createConferenceData = new CreateConferenceData();
            createConferenceData.localID = Config.localSettings.Values["localID"].ToString();
            createConferenceData.originID = "originID";
            createConferenceData.deviceID = "deviceID";
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

        public static FabricSetupData FabricSetup()
        {
            List<IceCandidate> localIceCandidatesList = new List<IceCandidate>();
            IceCandidate localIceCandidate = new IceCandidate();
            localIceCandidate.id = "1";
            localIceCandidate.type = "localcandidate";
            localIceCandidate.ip = "127.0.0.1";
            localIceCandidate.port = 8888;
            localIceCandidate.candidateType = IceCandidateType.host.ToString();
            localIceCandidate.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidate);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidate = new IceCandidate();
            remoteIceCandidate.id = "2";
            remoteIceCandidate.type = "remotecandidate";
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
            conferenceStatsSubmissionData.timestamp = TimeStamp.Now();
            conferenceStatsSubmissionData.remoteID = "remoteID";
            conferenceStatsSubmissionData.stats = statsList;

            return conferenceStatsSubmissionData;
        }

        public static FabricTerminatedData FabricTerminated()
        {
            FabricTerminatedData fabricTerminatedData = new FabricTerminatedData();
            fabricTerminatedData.timestamp = TimeStamp.Now();
            fabricTerminatedData.remoteID = "remoteID";

            return fabricTerminatedData;
        }

        public static UserLeftData UserLeft()
        {
            UserLeftData userLeftData = new UserLeftData();
            userLeftData.timestamp = TimeStamp.Now();

            return userLeftData;
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
            fabricSetupFailedData.timestamp = TimeStamp.Now();
            fabricSetupFailedData.fabricTransmissionDirection = FabricTransmissionDirection.sendrecv.ToString();
            fabricSetupFailedData.remoteEndpointType = RemoteEndpointType.peer.ToString();
            fabricSetupFailedData.reason = FabricSetupFailedReason.MediaConfigError.ToString();
            fabricSetupFailedData.name = "name";
            fabricSetupFailedData.message = "message";
            fabricSetupFailedData.stack = "stack";

            return fabricSetupFailedData;
        }
    }
}
