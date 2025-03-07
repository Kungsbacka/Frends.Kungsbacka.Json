using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace Frends.Kungsbacka.Json.Tests
{
	[TestFixture]
	class SanitizeCDataSectionsTests
	{
		[Test]
		public void AllDataMigratedToParentAndSectionsRemoved()
		{
			JToken jsonWithoutCdata = JToken.Parse(_jsonWithoutCdata);
			var result = JsonTasks.SanitizeCDataSections(
				new SanitizeCDataSectionsInput() { JsonData = JToken.Parse(_jsonWithCdata) },
				new SanitizeCDataSectionsOptions(),
				CancellationToken.None)
				.GetAwaiter()
				.GetResult();

			Assert.True(JToken.DeepEquals(result.JsonData, jsonWithoutCdata));
		}

		[Test]
		public void NoChangesMade()
		{
			JToken jsonData = JToken.Parse(_jsonWithoutCdata);
			var result = JsonTasks.SanitizeCDataSections(
				new SanitizeCDataSectionsInput() { JsonData = jsonData },
				new SanitizeCDataSectionsOptions(),
				CancellationToken.None)
				.GetAwaiter()
				.GetResult();

			Assert.True(JToken.DeepEquals(result.JsonData, jsonData));
		}


		readonly string _jsonWithCdata = @"
        {
	        ""FlowInstance"": {
		        ""@xmlns"": ""http://www.oeplatform.org/version/2.0/schemas/flowinstance"",
		        ""@xmlns:xsi"": ""http://www.w3.org/2001/XMLSchema-instance"",
		        ""@xsi:schemaLocation"": ""http://www.oeplatform.org/version/2.0/schemas/flowinstance schema-3793.xsd"",
		        ""Header"": {
			        ""Flow"": {
				        ""FamilyID"": ""766"",
				        ""Version"": ""6"",
				        ""FlowID"": ""3793"",
				        ""Name"": {
					        ""#cdata-section"": ""TEST Ansöka om skolskjuts till grundskola läsåret 2024/2025""
				        }
			        },
			        ""FlowInstanceID"": ""190456"",
			        ""Status"": {
				        ""ID"": ""21762"",
				        ""Name"": {
					        ""#cdata-section"": ""Inkommen""
				        }
			        }			       
		        },
		        ""Values"": {
			        ""elev"": {
				        ""QueryID"": ""144324"",
				        ""Name"": {
					        ""#cdata-section"": ""Vilken elev ansöker du om skolskjuts för?""
				        },
				        ""CitizenIdentifier"": ""20200307TEST"",
				        ""Firstname"": ""Testbarn"",
				        ""Lastname"": ""5år"",
				        ""Address"": ""Testgatan 1"",
				        ""Zipcode"": ""123 45"",
				        ""PostalAddress"": ""Testköping""
			        }
		        }
	        }
        }";

		readonly string _jsonWithoutCdata = @"
        {
		  ""FlowInstance"": {
			""@xmlns"": ""http://www.oeplatform.org/version/2.0/schemas/flowinstance"",
			""@xmlns:xsi"": ""http://www.w3.org/2001/XMLSchema-instance"",
			""@xsi:schemaLocation"": ""http://www.oeplatform.org/version/2.0/schemas/flowinstance schema-3793.xsd"",
			""Header"": {
			  ""Flow"": {
				""FamilyID"": ""766"",
				""Version"": ""6"",
				""FlowID"": ""3793"",
				""Name"": ""TEST Ansöka om skolskjuts till grundskola läsåret 2024/2025""
			  },
			  ""FlowInstanceID"": ""190456"",
			  ""Status"": {
				""ID"": ""21762"",
				""Name"": ""Inkommen""
			  }
			},
			""Values"": {
			  ""elev"": {
				""QueryID"": ""144324"",
				""Name"": ""Vilken elev ansöker du om skolskjuts för?"",
				""CitizenIdentifier"": ""20200307TEST"",
				""Firstname"": ""Testbarn"",
				""Lastname"": ""5år"",
				""Address"": ""Testgatan 1"",
				""Zipcode"": ""123 45"",
				""PostalAddress"": ""Testköping""
			  }
			}
		  }
		}";
	}
}
