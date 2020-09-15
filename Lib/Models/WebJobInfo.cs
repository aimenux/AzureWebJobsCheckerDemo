using System;
using System.Text;
using Newtonsoft.Json;

namespace Lib.Models
{
    public class WebJobInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("history_url")]
        public string HistoryUrl { get; set; }

        [JsonProperty("scheduler_logs_url")]
        public string SchedulerLogsUrl { get; set; }

        [JsonProperty("run_command")]
        public string RunCommand { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("extra_info_url")]
        public string ExtraInfoUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("using_sdk")]
        public bool UsingSdk { get; set; }

        [JsonProperty("detailed_status")]
        public string DetailedStatus { get; set; }

        [JsonProperty("log_url")]
        public string LogUrl { get; set; }

        [JsonProperty("latest_run")]
        public LatestRun LatestRun { get; set; }

        public override string ToString()
        {
            var summary = new StringBuilder();
            summary.AppendLine($"Name: {Name}");
            var type = GetWebJobType();
            summary.AppendLine($"Type: {type}");
            summary.AppendLine($"Error: {Error}");
            switch (type)
            {
                case WebJobType.Triggered:
                    summary.AppendLine($"Status: {LatestRun.Status}");
                    summary.AppendLine($"StartTime: {LatestRun.StartTime}");
                    summary.AppendLine($"EndTime: {LatestRun.EndTime}");
                    summary.AppendLine($"Duration: {LatestRun.Duration}");
                    summary.AppendLine($"WebJobLogUrl: {LatestRun.OutputUrl}");
                    break;

                case WebJobType.Continuous:
                    summary.AppendLine($"Status: {Status}");
                    summary.AppendLine($"WebJobLogUrl: {LogUrl}");
                    break;

                default:
                    summary.AppendLine($"UnexpectedType: {Type}");
                    break;
            }
            summary.AppendLine($"WebJobUrl: {Url}");
            return summary.ToString();
        }

        private WebJobType GetWebJobType()
        {
            if (Enum.TryParse(Type, out WebJobType webJobType))
            {
                return webJobType;
            }

            throw new ArgumentOutOfRangeException(Type, "Unexpected webJobType");
        }
    }
}
