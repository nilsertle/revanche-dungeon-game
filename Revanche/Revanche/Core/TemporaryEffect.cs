using Newtonsoft.Json;

namespace Revanche.Core
{
    public sealed class TemporaryEffect
    {
        [JsonProperty] public int Active { get; set; }
        [JsonProperty] public float Strength { get; set; }
        [JsonProperty] public int Duration { get; set; }
        [JsonProperty] public double Time { get; set; }

        internal void Update(float deltaTime)
        {
            if (Active == 0)
            {
                return;
            }
            Time += deltaTime;
            if (this.Time >= this.Duration)
            {
                this.Active = 0;
            }
        }

        internal static TemporaryEffect CreateTemporaryEffect(int duration, float strength)
        {
            var eff = new TemporaryEffect
            {
                Active = 1,
                Duration = duration,
                Strength = strength,
                Time = 0
            };
            return eff;
        }

    }


}
