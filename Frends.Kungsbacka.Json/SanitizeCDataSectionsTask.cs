using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static partial class JsonTasks
    {
        /// <summary>
        /// Moves data contained in #cdata-section sections to the text nodes of the containing elements and removes the #cdata-section token. 
        /// </summary>
        /// <param name="input">Required parameters</param>
        /// <param name="options">Optional parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>JToken sanitized of #cdata-sections with data moved to parent JToken</returns>
        public static async Task<SanitizeCDataSectionsResult> SanitizeCDataSections([PropertyTab] SanitizeCDataSectionsInput input, [PropertyTab] SanitizeCDataSectionsOptions options, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.JsonData == null)
            {
                throw new ArgumentNullException(nameof(input.JsonData));
            }

            JToken token = input.JsonData.DeepClone();
            JToken sanitized = ProcessCData(token);
            SanitizeCDataSectionsResult result = new SanitizeCDataSectionsResult
            {
                JsonData = sanitized
            };

            return result;
        }


        /// <summary>
        /// Recursively processes a JToken to move data contained in #cdata-section 
        /// sections to the text nodes of the containing elements and removes the #cdata-section token.
        /// </summary>
        /// <param name="token">The JToken to process.</param>
        /// <returns>The processed JToken with #cdata-section sections moved to text nodes.</returns>
        static JToken ProcessCData(JToken token)
        {
            if (token is JObject obj)
            {
                if (obj.Properties().Count() == 1 && obj.Properties().First().Name == "#cdata-section")
                {
                    return obj["#cdata-section"];
                }

                foreach (var property in obj.Properties().ToList())
                {
                    property.Value = ProcessCData(property.Value);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    array[i] = ProcessCData(array[i]);
                }
            }

            return token;
        }
    }
}