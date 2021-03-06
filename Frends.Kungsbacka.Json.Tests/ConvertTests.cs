using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    class ConvertTests
    {
        const string xml = @"<?xml version='1.0' standalone='no'?>
             <root>
               <person id='1'>
                 <name>Alan</name>
                 <url>http://www.google.com</url>
               </person>
               <person id='2'>
                <name>Louis</name>
                 <url>http://www.yahoo.com</url>
              </person>
            </root>";

        [Test]
        public void ShouldConvertXmlStringToJToken()
        {
            var result = JsonTasks.ConvertXmlStringToJToken(xml);
            Assert.IsInstanceOf<JObject>(result);
        }

        [Test]
        public void ShouldConvertStringToJToken()
        {
            string json = @"{
                ""FirstName"": ""John"",
                ""LastName"": ""Doe""
            }";
            JToken token = JToken.Parse(json);
            var result = JsonTasks.ConvertJsonStringToJToken(json);
            Assert.AreEqual(token, result);
        }

        [Test]
        public void ShouldConvertXmlBytesToJToken()
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
            var result = JsonTasks.ConvertXmlBytesToJToken(bytes);
            Assert.IsInstanceOf<JObject>(result);
        }
    }
}
