using Newtonsoft.Json;
using System.ComponentModel;

namespace MarketWatcher
{
    public class Quote
    {


        [JsonProperty("l")]
        public float Minimun { get; set; }
        [JsonProperty("h")]
        public float Maximum { get; set; }

        [JsonProperty("c")]
        public float Cours { get; set; }

        [JsonProperty("o")] 
        public float Ouverture { get; set; }
    }
}
