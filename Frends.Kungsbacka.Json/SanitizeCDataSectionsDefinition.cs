using Newtonsoft.Json.Linq;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// Result of the SanitizeCDataSections operation.
    /// </summary>
    public class SanitizeCDataSectionsResult
    {
        /// <summary>
        /// The sanitized JSON object.
        /// </summary>
        public JToken JsonData { get; set; } = JToken.Parse("{}");
    }

    public class SanitizeCDataSectionsOptions
    {
    }

    public class SanitizeCDataSectionsInput
    {
        /// <summary>
        /// The JSON to sanitize.
        /// </summary>
        public JToken JsonData { get; set; }
    }
}
