using Frends.Kungsbacka.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Xml.Linq;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    public class SanitizeCDataSectionsTests
    {
        [Test]
        public void SanitizeCDataSections_ShouldThrowArgumentNullException_WhenInputIsNull()
        {
            // Arrange
            SanitizeCDataSectionsInput input = null;
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            TestDelegate act = () => JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Test]
        public void SanitizeCDataSections_ShouldThrowArgumentNullException_WhenJsonDataIsNull()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput { JsonData = null };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            TestDelegate act = () => JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Test]
        public void SanitizeCDataSections_ShouldReturnEmptyObject_WhenInputIsEmptyJsonObject()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject()
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "{}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessSingleCDataSection()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject { ["#cdata-section"] = "Some value" }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "\"Some value\"";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessNestedCDataSections()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["parent"] = new JObject
                    {
                        ["#cdata-section"] = "Nested value"
                    }
                }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "{\"parent\":\"Nested value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessArraysWithCDataSections()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JArray
                {
                    new JObject { ["#cdata-section"] = "Value1" },
                    new JObject { ["#cdata-section"] = "Value2" }
                }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "[\"Value1\",\"Value2\"]";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldLeaveRegularPropertiesUnchanged()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["name"] = "John",
                    ["#cdata-section"] = "Some value"
                }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "{\"name\":\"John\",\"#cdata-section\":\"Some value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleComplexNestedStructure()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["root"] = new JObject
                    {
                        ["child"] = new JArray
                        {
                            new JObject { ["#cdata-section"] = "Value1" },
                            new JObject { ["#cdata-section"] = "Value2" }
                        }
                    }
                }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "{\"root\":{\"child\":[\"Value1\",\"Value2\"]}}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleEmptyArray()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JArray()
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "[]";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleMixedContent()
        {
            // Arrange
            SanitizeCDataSectionsInput input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["name"] = "John",
                    ["data"] = new JObject
                    {
                        ["#cdata-section"] = "Nested value"
                    }
                }
            };
            SanitizeCDataSectionsOptions options = new SanitizeCDataSectionsOptions();

            // Act
            SanitizeCDataSectionsResult result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            string expected = "{\"name\":\"John\",\"data\":\"Nested value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}

