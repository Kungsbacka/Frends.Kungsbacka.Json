using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

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
            var input = new SanitizeCDataSectionsInput { JsonData = JToken.Parse("{}") };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("{}", result.JsonData.ToString());
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessSingleCDataSection()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("{\"#cdata-section\":\"Some value\"}")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("\"Some value\"", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessNestedCDataSections()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("{\"parent\":{\"#cdata-section\":\"Nested value\"}}")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("{\"parent\":\"Nested value\"}", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldProcessArraysWithCDataSections()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("[ {\"#cdata-section\":\"Value1\"}, {\"#cdata-section\":\"Value2\"} ]")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("[\"Value1\",\"Value2\"]", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldLeaveRegularPropertiesUnchanged()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("{\"name\":\"John\",\"#cdata-section\":\"Some value\"}")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("{\"name\":\"John\",\"#cdata-section\":\"Some value\"}", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleComplexNestedStructure()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("{\"root\":{\"child\":[{\"#cdata-section\":\"Value1\"},{\"#cdata-section\":\"Value2\"}]}}")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("{\"root\":{\"child\":[\"Value1\",\"Value2\"]}}", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleEmptyArray()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("[]")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("[]", result.JsonData.ToString());
        }

        [Test]
        public void SanitizeCDataSections_ShouldHandleMixedContent()
        {
            // Arrange
            var input = new SanitizeCDataSectionsInput
            {
                JsonData = JToken.Parse("{\"name\":\"John\",\"data\":{\"#cdata-section\":\"Nested value\"}}")
            };
            var options = new SanitizeCDataSectionsOptions();

            // Act
            var result = JsonTasks.SanitizeCDataSections(input, options);

            // Assert
            Assert.AreEqual("{\"name\":\"John\",\"data\":\"Nested value\"}", result.JsonData.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}
