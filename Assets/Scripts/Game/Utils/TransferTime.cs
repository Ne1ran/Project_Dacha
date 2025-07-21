using System;
using Newtonsoft.Json;

namespace Game.Utils
{
    public class TransferTime
    {
        [JsonIgnore]
        public DateTime DateTime { get; private set; }

        public TransferTime()
        {
        }

        public TransferTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        [JsonProperty]
        public long Delta
        {
            get => (long) -DateTime.Now.Subtract(DateTime).TotalSeconds;
            set => DateTime = DateTime.Now.AddSeconds(value);
        }
    }
}