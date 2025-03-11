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
            var options = new SanitizeCDataSectionsOptions();

            // Act
            TestDelegate act = () => JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Test]
        public void SanitizeCDataSections_ShouldThrowArgumentNullException_WhenJsonDataIsNull()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput { JsonData = null };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            TestDelegate act = () => JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Test]
        public void SanitizeCDataSections_ShouldReturnEmptyObject_WhenInputIsEmptyJsonObject()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject()
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "{}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessSingleCDataSection()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject { ["#cdata-section"] = "Some value" }
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "\"Some value\"";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessNestedCDataSections()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["parent"] = new JObject
                    {
                        ["#cdata-section"] = "Nested value"
                    }
                }
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "{\"parent\":\"Nested value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessArraysWithCDataSections()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JArray
                {
                    new JObject { ["#cdata-section"] = "Value1" },
                    new JObject { ["#cdata-section"] = "Value2" }
                }
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "[\"Value1\",\"Value2\"]";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldLeaveRegularPropertiesUnchanged()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JObject
                {
                    ["name"] = "John",
                    ["#cdata-section"] = "Some value"
                }
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "{\"name\":\"John\",\"#cdata-section\":\"Some value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleComplexNestedStructure()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
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
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "{\"root\":{\"child\":[\"Value1\",\"Value2\"]}}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleEmptyArray()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = new JArray()
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "[]";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleMixedContent()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
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
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            var expected = "{\"name\":\"John\",\"data\":\"Nested value\"}";
            Assert.AreEqual(expected, result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}

