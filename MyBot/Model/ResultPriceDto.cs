using Newtonsoft.Json;

namespace MyBot.Model
{
    public class PriceResultDto
    {
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("buy")]
        public int buy { get; set; }
        [JsonProperty("sell")]
        public int sell { get; set; }
        [JsonProperty("tax")]
        public string tax { get; set; }
        [JsonProperty("Link")]
        public string Link { get; set; }
    }
}
