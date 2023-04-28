using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CodeBeautify
{
    public partial class WordPressversionsDto
    {
        [JsonProperty("Ok")]
        public bool Status { get; set; }

        [JsonProperty("Result")]
        public List<Result> Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("update")]
        public string Update { get; set; }

        [JsonProperty("zip-link")]
        public Uri ZipLink { get; set; }

        [JsonProperty("zip-md5")]
        public Uri ZipMd5 { get; set; }

        [JsonProperty("zip-sha1")]
        public Uri ZipSha1 { get; set; }

        [JsonProperty("gz-link")]
        public Uri GzLink { get; set; }

        [JsonProperty("gz-md5")]
        public Uri GzMd5 { get; set; }

        [JsonProperty("gz-sha1")]
        public Uri GzSha1 { get; set; }
    }

    public partial class WordPressversionsDto
    {
        public static WordPressversionsDto FromJson(string json) => JsonConvert.DeserializeObject<WordPressversionsDto>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WordPressversionsDto self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}