using NUnit.Framework;
using System;
using VisionFocus.Core.Models;

namespace VisionFocus.Tests.Models
{
    [TestFixture]
    public class SessionSummaryTests
    {
        [Test]
        public void ToCsvString_ShouldContainAllRequiredFields()
        {
            // Arrange
            var summary = new SessionSummary
            {
                Date = new DateTime(2025, 10, 16),
                StartTime = new TimeSpan(10, 0, 0),
                Subject = "Science",
                SessionDurationMinutes = 60,
                TotalAlertCount = 3
            };

            // Act
            string csv = summary.ToCsvString();
            string[] parts = csv.Split(',');

            // Assert
            Assert.That(parts.Length, Is.EqualTo(5)); 
            Assert.That(DateTime.TryParse(parts[0], out _), Is.True); 
            Assert.That(TimeSpan.TryParse(parts[1], out _), Is.True); 
            Assert.That(parts[2], Is.EqualTo("Science")); 
            Assert.That(parts[3], Is.EqualTo("60")); 
            Assert.That(parts[4], Is.EqualTo("3")); 
        }

        [Test]
        public void ToCsvString_ShouldReturnExpectedFormat()
        {
            // Arrange
            var summary = new SessionSummary
            {
                Date = new DateTime(2025, 10, 16),
                StartTime = new TimeSpan(9, 15, 0),
                Subject = "Math",
                SessionDurationMinutes = 45,
                TotalAlertCount = 2
            };

            // Act
            string csv = summary.ToCsvString();

            // Assert
            Assert.That(csv, Is.EqualTo("2025-10-16,09:15:00,Math,45,2"));
        }

        [Test]
        public void FromCsvString_ShouldReturnValidObject_WhenInputIsCorrect()
        {
            // Arrange
            string csvLine = "2025-10-16,09:15:00,Math,45,2";

            // Act
            var result = SessionSummary.FromCsvString(csvLine);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Date, Is.EqualTo(new DateTime(2025, 10, 16)));
            Assert.That(result.StartTime, Is.EqualTo(new TimeSpan(9, 15, 0)));
            Assert.That(result.Subject, Is.EqualTo("Math"));
            Assert.That(result.SessionDurationMinutes, Is.EqualTo(45));
            Assert.That(result.TotalAlertCount, Is.EqualTo(2));
        }

        [Test]
        public void FromCsvString_ShouldReturnNull_WhenInputIsInvalid()
        {
            // Arrange
            string invalidCsv = "invalid,data";

            // Act
            var result = SessionSummary.FromCsvString(invalidCsv);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void FromCsvString_ShouldReturnNull_WhenFieldCountIsWrong()
        {
            // Arrange
            string csvLine = "2025-10-16,09:15:00,Math"; 

            // Act
            var result = SessionSummary.FromCsvString(csvLine);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCsvHeader_ShouldReturnCorrectHeader()
        {
            // Act
            string header = SessionSummary.GetCsvHeader();

            // Assert
            Assert.That(header, Is.EqualTo("Date,StartTime,Subject,SessionDurationMinutes,TotalAlertCount"));
        }
    }
}