﻿namespace CallStatsLib.Request
{
    public class MediaPlaybackData
    {
        public string eventType { get; set; }
        public string localID { get; set; }
        public string originID { get; set; }
        public string deviceID { get; set; }
        public long timestamp { get; set; }
        public string remoteID { get; set; }
        public string connectionID { get; set; }
        public string mediaType { get; set; }
        public string ssrc { get; set; }
    }
}
