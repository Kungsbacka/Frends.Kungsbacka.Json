﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static partial class JsonTasks
    {
        /// <summary>
        /// Maps properties from one JObject to another. A default value can be specified if the property
        /// is not found in the source object. Optionally simple transforms can be applied to the value.
        /// If destination object is null, a new empty JObject is created.
        /// </summary>
        /// <param name="input">Requred parameters (see MapInput class)</param>
        /// <param name="options">Optional parameters (see MapOptions class)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static object Map([PropertyTab] MapInput input, [PropertyTab] MapOptions options)
        {
            if (input.SourceObject == null)
            {
                throw new ArgumentNullException(nameof(input.SourceObject), "Source object cannot be null.");
            }
            if (input.DestinationObject == null)
            {
                input.DestinationObject = new JObject();
            }
            if (string.IsNullOrEmpty(input.Map))
            {
                throw new ArgumentException("Map cannot be null or an empty string.", nameof(input.Map));
            }
            MapTransformations.RegisterBuiltInTransformations();
            if (options?.Tranformations != null)
            {
                foreach (MapTransformation transformation in options.Tranformations)
                {
                    MapTransformations.RegisterTransformation(transformation);
                }
            }
            var mappings = JsonConvert.DeserializeObject<Mapping[]>(input.Map);
            foreach (var mapping in mappings)
            {
                if (mapping.From == null)
                {
                    throw new ArgumentNullException(nameof(mapping.From));
                }
                if (mapping.From is JArray && string.IsNullOrEmpty(mapping.To))
                {
                    throw new ArgumentException("If 'From' is an array, 'To' must have a value.", nameof(mapping.To));
                }
                if (string.IsNullOrEmpty(mapping.To))
                {
                    mapping.To = (string)mapping.From;
                }
                string to = mapping.To;
                bool keepExistingValue = TaskHelper.EndsWithChar(ref to, '!');
                if (keepExistingValue && input.DestinationObject.Properties().Any(p => p.Name.IEquals(to)))
                {
                    continue;
                }
                JToken token = GetFirstAvailableToken(input.SourceObject, mapping.From);
                if (token == null)
                {
                    if (mapping.DefaultPresent)
                    {
                        input.DestinationObject.Add(new JProperty(to, mapping.Default));
                    }
                    continue;
                }
                if (options != null && options.UnpackCdataSection)
                {
                    if (token is JObject)
                    {
                        var cdata = token["#cdata-section"];
                        if (cdata != null)
                        {
                            token = cdata;
                        }
                    }
                }
                foreach (string transformation in mapping.Transformations)
                {
                    token = MapTransformations.Transform(transformation, token);
                }
                input.DestinationObject[to] = token;
            }
            return input.DestinationObject;
        }

        private static dynamic GetFirstAvailableToken(JObject sourceObject, object mapFrom)
        {
            if (!(mapFrom is JArray))
            {
                mapFrom = new JArray(mapFrom);
            }
            foreach (string item in (JArray)mapFrom)
            {
                var from = item;
                bool useSelectToken = TaskHelper.StartsWithChar(ref from, '?');
                JToken token;
                if (useSelectToken)
                {
                    token = sourceObject.SelectToken(from);
                }
                else
                {
                    token = sourceObject[from];
                }
                if (token != null)
                {
                    return token;
                }
            }
            return null;
        }
    }
}
