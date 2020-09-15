using System;
using Newtonsoft.Json;

namespace Lib.Models
{
    public class LatestRun
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("start_time")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTimeOffset EndTime { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("output_url")]
        public string OutputUrl { get; set; }

        [JsonProperty("error_url")]
        public string ErrorUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("job_name")]
        public string JobName { get; set; }

        [JsonProperty("trigger")]
        public string Trigger { get; set; }
    }
}