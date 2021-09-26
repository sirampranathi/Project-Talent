using Newtonsoft.Json;

namespace MVPStudio.Framework.Config
{
    public class Settings
    {
        [JsonProperty(PropertyName = "TestType")]
        public static string TestType { get; set; }

        [JsonProperty(PropertyName = "AUT")]
        public static string AUT { get; set; }

        [JsonProperty(PropertyName = "Build")]
        public static string Build { get; set; }

        [JsonProperty(PropertyName = "IsLog")]
        public static string IsLog { get; set; }

        [JsonProperty(PropertyName = "LogPath")]
        public static string LogPath { get; set; }

        [JsonProperty(PropertyName = "IsReporting")]
        public static string IsReporting { get; set; }

        [JsonProperty(PropertyName = "ExtendReportPath")]
        public static string ExtendReportPath { get; set; }

        [JsonProperty(PropertyName = "ScreenshotPath")]
        public static string ScreenshotPath { get; set; }
    }
}