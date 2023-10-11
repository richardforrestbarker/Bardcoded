using System.Text.Json.Serialization;

namespace Bardcoded.API.Data
{
    public record Health(bool IsUp)
    {
        public static readonly Health Down = new Health(false);
        public static readonly Health Up = new Health(true);
    }
}
