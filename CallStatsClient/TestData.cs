﻿using CallStatsLib;
using CallStatsLib.Request;
using System;
using System.Collections.Generic;

namespace CallStatsClient
{
    public static class TestData
    {
        private static string _localID = Config.localSettings.Values["localID"].ToString();
        private static string _originID = "originID";
        private static string _deviceID = "deviceID";
        private static string _connectionID = "connectionXYZ";
        private static string _remoteID = "2";

        private enum EndpointInfoType { browser, native, plugin, middlebox }

        public static CreateConferenceData CreateConference()
        {
            CreateConferenceData createConferenceData = new CreateConferenceData();
            createConferenceData.localID = _localID;
            createConferenceData.originID = _originID; 
            createConferenceData.deviceID = _deviceID;
            createConferenceData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();

            EndpointInfo endpointInfo = new EndpointInfo();
            endpointInfo.type = EndpointInfoType.native.ToString();
            endpointInfo.os = "Windows";
            endpointInfo.osVersion = "10";
            endpointInfo.buildName = "";
            endpointInfo.buildVersion = "";
            endpointInfo.appVersion = "1.0";

            createConferenceData.endpointInfo = endpointInfo;

            return createConferenceData;
        }

        public static UserAliveData UserAlive()
        {
            UserAliveData userAliveData = new UserAliveData();
            userAliveData.localID = _localID;
            userAliveData.originID = _originID;
            userAliveData.deviceID = _deviceID;
            userAliveData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();

            return userAliveData;
        }

        private enum FabricTransmissionDirection { sendrecv, sendonly, receiveonly }

        private enum RemoteEndpointType { peer, server }

        private enum IceCandidateType
        {
            host, srflx, prflx, relay, stun, serverreflexive, peerreflexive, turn, relayed, local
        }

        private enum IceCandidateTransport { tcp, udp }

        private enum IceCandidateState { frozen, waiting, inprogress, failed, succeeded, cancelled }

        public static FabricSetupData FabricSetup()
        {
            List<IceCandidate> localIceCandidatesList = new List<IceCandidate>();
            IceCandidate localIceCandidateObj = new IceCandidate();
            localIceCandidateObj.id = "1";
            localIceCandidateObj.type = LocalIceCandidateType.localCandidate.ToString();
            localIceCandidateObj.ip = "127.0.0.1";
            localIceCandidateObj.port = 8888;
            localIceCandidateObj.candidateType = IceCandidateType.host.ToString();
            localIceCandidateObj.transport = IceCandidateTransport.tcp.ToString();
            localIceCandidatesList.Add(localIceCandidateObj);

            List<IceCandidate> remoteIceCandidatesList = new List<IceCandidate>();
            IceCandidate remoteIceCandidateObj = new IceCandidate();
            remoteIceCandidateObj.id = "2";
            remoteIceCandidateObj.type = RemoteIceCandidateType.remoteCandidate.ToString();
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
            fabricSetupData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricSetupData.connectionID = _connectionID;
            fabricSetupData.remoteID = _remoteID;
            fabricSetupData.delay = 2;
            fabricSetupData.iceGatheringDelay = 2;
            fabricSetupData.iceConnectivityDelay = 2;
            fabricSetupData.fabricTransmissionDirection = FabricTransmissionDirection.sendrecv.ToString();
            fabricSetupData.remoteEndpointType = RemoteEndpointType.peer.ToString();
            fabricSetupData.localIceCandidates = localIceCandidatesList;
            fabricSetupData.remoteIceCandidates = remoteIceCandidatesList;
            fabricSetupData.iceCandidatePairs = iceCandidatePairsList;

            return fabricSetupData;
        }

        private enum FabricSetupFailedReason
        {
            MediaConfigError, MediaPermissionError, MediaDeviceError, NegotiationFailure,
            SDPGenerationError, TransportFailure, SignalingError, IceConnectionFailure
        }

        public static FabricSetupFailedData FabricSetupFailed()
        {
            FabricSetupFailedData fabricSetupFailedData = new FabricSetupFailedData();
            fabricSetupFailedData.localID = _localID;
            fabricSetupFailedData.originID = _originID;
            fabricSetupFailedData.deviceID = _deviceID;
            fabricSetupFailedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricSetupFailedData.fabricTransmissionDirection = FabricTransmissionDirection.sendrecv.ToString();
            fabricSetupFailedData.remoteEndpointType = RemoteEndpointType.peer.ToString();
            fabricSetupFailedData.reason = FabricSetupFailedReason.MediaConfigError.ToString();
            fabricSetupFailedData.name = "name";
            fabricSetupFailedData.message = "message";
            fabricSetupFailedData.stack = "stack";

            return fabricSetupFailedData;
        }

        private enum StreamType { inbound, outbound }

        private enum ReportType { local, remote }

        private enum MediaType { audio, video, screen }

        public static SSRCMapData SSRCMap()
        {
            List<SSRCData> ssrcDataList = new List<SSRCData>();
            SSRCData ssrcData = new SSRCData();
            ssrcData.ssrc = "1";
            ssrcData.cname = "cname";
            ssrcData.streamType = StreamType.inbound.ToString();
            ssrcData.reportType = ReportType.local.ToString();
            ssrcData.mediaType = MediaType.audio.ToString();
            ssrcData.userID = _localID;
            ssrcData.msid = "msid";
            ssrcData.mslabel = "mslabel";
            ssrcData.label = "label";
            ssrcData.localStartTime = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            ssrcDataList.Add(ssrcData);

            SSRCMapData ssrcMapData = new SSRCMapData();
            ssrcMapData.localID = _localID;
            ssrcMapData.originID = _originID;
            ssrcMapData.deviceID = _deviceID;
            ssrcMapData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            ssrcMapData.connectionID = _connectionID;
            ssrcMapData.remoteID = _remoteID;
            ssrcMapData.ssrcData = ssrcDataList;

            return ssrcMapData;
        }

        public static ConferenceStatsSubmissionData ConferenceStatsSubmission()
        {
            List<object> statsList = new List<object>();

            ConferenceStatsSubmissionData conferenceStatsSubmissionData = new ConferenceStatsSubmissionData();
            conferenceStatsSubmissionData.localID = _localID;
            conferenceStatsSubmissionData.originID = _originID;
            conferenceStatsSubmissionData.deviceID = _deviceID;
            conferenceStatsSubmissionData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            conferenceStatsSubmissionData.connectionID = _connectionID;
            conferenceStatsSubmissionData.remoteID = _remoteID;
            conferenceStatsSubmissionData.stats = statsList;

            return conferenceStatsSubmissionData;
        }

        public static FabricTerminatedData FabricTerminated()
        {
            FabricTerminatedData fabricTerminatedData = new FabricTerminatedData();
            fabricTerminatedData.localID = _localID;
            fabricTerminatedData.originID = _originID;
            fabricTerminatedData.deviceID = _deviceID;
            fabricTerminatedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricTerminatedData.connectionID = _connectionID;
            fabricTerminatedData.remoteID = _remoteID;

            return fabricTerminatedData;
        }

        public static UserLeftData UserLeft()
        {
            UserLeftData userLeftData = new UserLeftData();
            userLeftData.localID = _localID;
            userLeftData.originID = _originID;
            userLeftData.deviceID = _deviceID;
            userLeftData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();

            return userLeftData;
        }

        public static UserDetailsData UserDetails()
        {
            UserDetailsData userDetailsData = new UserDetailsData();
            userDetailsData.localID = _localID;
            userDetailsData.originID = _originID;
            userDetailsData.deviceID = _deviceID;
            userDetailsData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            userDetailsData.userName = "userName";

            return userDetailsData;
        }

        private enum ChangedState { signalingState, connectionState, iceConnectionState, iceGatheringState }

        public static FabricStateChangeData FabricStateChange()
        {
            FabricStateChangeData fabricStateChangeData = new FabricStateChangeData();
            fabricStateChangeData.localID = _localID;
            fabricStateChangeData.originID = _originID;
            fabricStateChangeData.deviceID = _deviceID;
            fabricStateChangeData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricStateChangeData.connectionID = _connectionID;
            fabricStateChangeData.remoteID = _remoteID;
            fabricStateChangeData.prevState = StateChange.stable.ToString();
            fabricStateChangeData.newState = StateChange.haveLocalOffer.ToString();
            fabricStateChangeData.changedState = ChangedState.signalingState.ToString();

            return fabricStateChangeData;
        }

        private enum ConnectionState { connected, completed }

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
            fabricTransportChangeData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricTransportChangeData.connectionID = _connectionID;
            fabricTransportChangeData.remoteID = _remoteID;
            fabricTransportChangeData.currIceCandidatePair = currIceCandidatePairObj;
            fabricTransportChangeData.prevIceCandidatePair = prevIceCandidatePairObj;
            fabricTransportChangeData.currIceConnectionState = ConnectionState.completed.ToString();
            fabricTransportChangeData.prevIceConnectionState = ConnectionState.connected.ToString();
            fabricTransportChangeData.delay = 2;
            fabricTransportChangeData.relayType = RelayType.turnTls;

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
            fabricDroppedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricDroppedData.connectionID = _connectionID;
            fabricDroppedData.remoteID = _remoteID;
            fabricDroppedData.currIceCandidatePair = currIceCandidatePairObj;
            fabricDroppedData.currIceConnectionState = "failed";
            fabricDroppedData.prevIceConnectionState = "disconnected";
            fabricDroppedData.delay = 2;

            return fabricDroppedData;
        }

        private enum EventType { fabricHold, fabricResume }

        public static FabricActionData FabricAction()
        {
            FabricActionData fabricActionData = new FabricActionData();
            fabricActionData.eventType = EventType.fabricHold.ToString();
            fabricActionData.localID = _localID;
            fabricActionData.originID = _originID;
            fabricActionData.deviceID = _deviceID;
            fabricActionData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            fabricActionData.connectionID = _connectionID;
            fabricActionData.remoteID = _remoteID;

            return fabricActionData;
        }

        public static SystemStatusStatsSubmissionData SystemStatusStatsSubmission() 
        {
            SystemStatusStatsSubmissionData systemStatusStatsSubmissionData = new SystemStatusStatsSubmissionData();
            systemStatusStatsSubmissionData.localID = _localID;
            systemStatusStatsSubmissionData.originID = _originID;
            systemStatusStatsSubmissionData.deviceID = _deviceID;
            systemStatusStatsSubmissionData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            systemStatusStatsSubmissionData.cpuLevel = 1;
            systemStatusStatsSubmissionData.batteryLevel = 1;
            systemStatusStatsSubmissionData.memoryUsage = 1;
            systemStatusStatsSubmissionData.memoryAvailable = 1;
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
            iceDisruptionStartData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceDisruptionStartData.connectionID = _connectionID;
            iceDisruptionStartData.remoteID = _remoteID;
            iceDisruptionStartData.currIceCandidatePair = currIceCandidatePairObj;
            iceDisruptionStartData.currIceConnectionState = "disconnected";
            iceDisruptionStartData.prevIceConnectionState = ConnectionState.completed.ToString();

            return iceDisruptionStartData;
        }

        private enum IceDisruptionEndConnectionState { connected, completed, checking }

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
            iceDisruptionEndData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceDisruptionEndData.connectionID = _connectionID;
            iceDisruptionEndData.remoteID = _remoteID;
            iceDisruptionEndData.currIceCandidatePair = currIceCandidatePairObj;
            iceDisruptionEndData.prevIceCandidatePair = prevIceCandidatePairObj;
            iceDisruptionEndData.currIceConnectionState = IceDisruptionEndConnectionState.checking.ToString();
            iceDisruptionEndData.prevIceConnectionState = "disconnected";
            iceDisruptionEndData.delay = 2;

            return iceDisruptionEndData;
        }

        private enum IceRestartConnectionState { checking, connected, completed, failed, disconnected, closed }

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
            iceRestartData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceRestartData.connectionID = _connectionID;
            iceRestartData.remoteID = _remoteID;
            iceRestartData.prevIceCandidatePair = prevIceCandidatePairObj;
            iceRestartData.currIceConnectionState = "new";
            iceRestartData.prevIceConnectionState = IceRestartConnectionState.checking.ToString();

            return iceRestartData;
        }

        private enum IceFailedConnectionState { checking, disconnected }

        public static IceFailedData IceFailed()
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
            iceCandidatePairObj.state = IceCandidateState.failed.ToString();
            iceCandidatePairObj.priority = 2;
            iceCandidatePairObj.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePairObj);

            IceFailedData iceFailedData = new IceFailedData();
            iceFailedData.eventType = "iceFailed";
            iceFailedData.localID = _localID;
            iceFailedData.originID = _originID;
            iceFailedData.deviceID = _deviceID;
            iceFailedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceFailedData.connectionID = _connectionID;
            iceFailedData.remoteID = _remoteID;
            iceFailedData.localIceCandidates = localIceCandidatesList;
            iceFailedData.remoteIceCandidates = remoteIceCandidatesList;
            iceFailedData.iceCandidatePairs = iceCandidatePairsList;
            iceFailedData.currIceConnectionState = "failed";
            iceFailedData.prevIceConnectionState = IceFailedConnectionState.checking.ToString();
            iceFailedData.delay = 2;

            return iceFailedData;
        }

        public static IceAbortedData IceAborted()
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
            iceCandidatePairObj.state = IceCandidateState.failed.ToString();
            iceCandidatePairObj.priority = 2;
            iceCandidatePairObj.nominated = true;
            iceCandidatePairsList.Add(iceCandidatePairObj);

            IceAbortedData iceAbortedData = new IceAbortedData();
            iceAbortedData.eventType = "iceAborted";
            iceAbortedData.localID = _localID;
            iceAbortedData.originID = _originID;
            iceAbortedData.deviceID = _deviceID;
            iceAbortedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceAbortedData.connectionID = _connectionID;
            iceAbortedData.remoteID = _remoteID;
            iceAbortedData.localIceCandidates = localIceCandidatesList;
            iceAbortedData.remoteIceCandidates = remoteIceCandidatesList;
            iceAbortedData.iceCandidatePairs = iceCandidatePairsList;
            iceAbortedData.currIceConnectionState = "closed";
            iceAbortedData.prevIceConnectionState = "checking";
            iceAbortedData.delay = 2;

            return iceAbortedData;
        }

        private enum IceTerminatedConnectionState { connected, completed, failed, disconnected }

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
            iceTerminatedData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceTerminatedData.connectionID = _connectionID;
            iceTerminatedData.remoteID = _remoteID;
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
            iceConnectionDisruptionStartData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceConnectionDisruptionStartData.connectionID = _connectionID;
            iceConnectionDisruptionStartData.remoteID = _remoteID;
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
            iceConnectionDisruptionEndData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            iceConnectionDisruptionEndData.connectionID = _connectionID;
            iceConnectionDisruptionEndData.remoteID = _remoteID;
            iceConnectionDisruptionEndData.currIceConnectionState = "checking";
            iceConnectionDisruptionEndData.prevIceConnectionState = "disconnected";
            iceConnectionDisruptionEndData.delay = 2;

            return iceConnectionDisruptionEndData;
        }

        private enum MediaActionEventType
        {
            audioMute, audioUnmute, screenShareStart, screenShareStop, videoPause, videoResume
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
            mediaActionData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            mediaActionData.connectionID = _connectionID;
            mediaActionData.remoteID = _remoteID;
            mediaActionData.ssrc = "11";
            mediaActionData.mediaDeviceID = "mediaDeviceID";
            mediaActionData.remoteIDList = remoteIDList;

            return mediaActionData;
        }

        private enum MediaPlaybackEventType
        {
            mediaPlaybackStart, mediaPlaybackSuspended, mediaPlaybackStalled, oneWayMedia
        }

        private enum MediaPlaybackMediaType { audio, video, screen }

        public static MediaPlaybackData MediaPlayback()
        {
            MediaPlaybackData mediaPlaybackData = new MediaPlaybackData();
            mediaPlaybackData.eventType = MediaPlaybackEventType.mediaPlaybackStart.ToString();
            mediaPlaybackData.localID = _localID;
            mediaPlaybackData.originID = _originID;
            mediaPlaybackData.deviceID = _deviceID;
            mediaPlaybackData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            mediaPlaybackData.connectionID = _connectionID;
            mediaPlaybackData.remoteID = _remoteID;    
            mediaPlaybackData.mediaType = MediaPlaybackMediaType.audio.ToString();
            mediaPlaybackData.ssrc = "11";

            return mediaPlaybackData;
        }

        private enum MediaDeviceKind { audioinput, audiooutput, videoinput }

        private enum ConnectedOrActiveDevicesEventType{ connectedDeviceList, activeDeviceList }

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
            connectedOrActiveDevicesData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            connectedOrActiveDevicesData.mediaDeviceList = mediaDeviceList;
            connectedOrActiveDevicesData.eventType = ConnectedOrActiveDevicesEventType.activeDeviceList.ToString();

            return connectedOrActiveDevicesData;
        }

        private enum ErrorLogsLevel { debug, info, warn, error, fatal }

        private enum ErrorLogsMessageType { text, json, domError }

        public static ApplicationErrorLogsData ApplicationErrorLogs()
        {
            ApplicationErrorLogsData applicationErrorLogsData = new ApplicationErrorLogsData();
            applicationErrorLogsData.localID = _localID;
            applicationErrorLogsData.originID = _originID;
            applicationErrorLogsData.deviceID = _deviceID;
            applicationErrorLogsData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            applicationErrorLogsData.connectionID = _connectionID;
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
            conferenceUserFeedbackData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            conferenceUserFeedbackData.remoteID = _remoteID;
            conferenceUserFeedbackData.feedback = feedbackObj;

            return conferenceUserFeedbackData;
        }

        public static DominantSpeakerData DominantSpeaker()
        {
            DominantSpeakerData dominantSpeakerData = new DominantSpeakerData();
            dominantSpeakerData.localID = _localID;
            dominantSpeakerData.originID = _originID;
            dominantSpeakerData.deviceID = _deviceID;
            dominantSpeakerData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();

            return dominantSpeakerData;
        }

        public static SDPEventData SDPEvent()
        {
            SDPEventData sdpEventData = new SDPEventData();
            sdpEventData.localID = _localID;
            sdpEventData.originID = _originID;
            sdpEventData.deviceID = _deviceID;
            sdpEventData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            sdpEventData.connectionID = _connectionID;
            sdpEventData.remoteID = _remoteID;
            sdpEventData.localSDP = "Stringified SDP of the local user";
            sdpEventData.remoteID = "Stringified SDP of the remote user";

            return sdpEventData;
        }

        public static BridgeStatisticsData BridgeStatistics() 
        {
            BridgeStatisticsData bridgeStatisticsData = new BridgeStatisticsData();
            bridgeStatisticsData.localID = _localID;
            bridgeStatisticsData.originID = _originID;
            bridgeStatisticsData.deviceID = _deviceID;
            bridgeStatisticsData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();
            bridgeStatisticsData.cpuUsage = 0;
            bridgeStatisticsData.batteryLevel = 0;
            bridgeStatisticsData.memoryUsage = 0;
            bridgeStatisticsData.totalMemory = 0;
            bridgeStatisticsData.totalMemory = 1;
            bridgeStatisticsData.intervalSentBytes = 0;
            bridgeStatisticsData.intervalReceivedBytes = 0;
            bridgeStatisticsData.intervalRtpFractionLoss = 0;
            bridgeStatisticsData.totalRtpLostPackets = 0;
            bridgeStatisticsData.intervalAverageRtt = 0;
            bridgeStatisticsData.intervalAverageJitter = 0;
            bridgeStatisticsData.intervalDownloadBitrate = 0;
            bridgeStatisticsData.intervalUploadBitrate = 0;
            bridgeStatisticsData.audioFabricCount = 0;
            bridgeStatisticsData.videoFabricCount = 0;
            bridgeStatisticsData.conferenceCount = 0;
            bridgeStatisticsData.participantCount = 0;

            return bridgeStatisticsData;
        }

        public static BridgeAliveData BridgeAlive()
        {
            BridgeAliveData bridgeAliveData = new BridgeAliveData();
            bridgeAliveData.localID = _localID;
            bridgeAliveData.originID = _originID;
            bridgeAliveData.deviceID = _deviceID;
            bridgeAliveData.timestamp = DateTime.UtcNow.ToUnixTimeStampMiliseconds();

            return bridgeAliveData;
        }
    }

    public static class LocalIceCandidateType
    {
        public const string localcandidate = "localcandidate";
        public const string localCandidate = "local-candidate";
    }

    public static class RemoteIceCandidateType
    {
        public const string remotecandidate = "remotecandidate";
        public const string remoteCandidate = "remote-candidate";
    }

    public static class RelayType
    {
        public const string turnTcp = "turn/tcp";
        public const string turnUdp = "turn/udp";
        public const string turnTls = "turn/tls";
    }

    public static class StateChange
    {
        public const string stable = "stable";
        public const string haveLocalOffer = "have-local-offer";
        public const string haveRemoteOffer = "have-remote-offer";
        public const string haveLocalPranswer = "have-local-pranswer";
        public const string haveRemotePranswer = "have-remote-pranswer";
        public const string closed = "closed";

        // Documentation: Invalid signalingState 442
        // "new", "connecting", "connected", "failed", "checking", "completed", "gathering", "complete" 
    }
}
