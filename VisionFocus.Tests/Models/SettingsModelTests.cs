using NUnit.Framework;
using VisionFocus.Core.Models;

namespace VisionFocus.Tests
{
    [TestFixture]
    public class SettingsModelTests
    {
        [Test]
        public void GetDefault_ShouldReturnValidSettings()
        {
            var settings = SettingsModel.GetDefault();

            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.SessionDurationMinutes, Is.EqualTo(25));
            Assert.That(settings.AlertThresholdSeconds, Is.EqualTo(5.0));
            Assert.That(settings.WarningThresholdSeconds, Is.EqualTo(3.0));
            Assert.That(settings.AlertVolume, Is.EqualTo(0.8));
        }

        [Test]
        public void GetDefault_ShouldHaveDefaultSubjects()
        {
            var settings = SettingsModel.GetDefault();

            Assert.That(settings.Subjects.Count, Is.EqualTo(3));
            Assert.That(settings.Subjects, Contains.Item("Math"));
            Assert.That(settings.Subjects, Contains.Item("Science"));
            Assert.That(settings.Subjects, Contains.Item("English"));
        }

        [Test]
        public void SettingsModel_ShouldAllowModification()
        {
            var settings = new SettingsModel();
            settings.SessionDurationMinutes = 60;
            settings.Subjects.Add("History");

            Assert.That(settings.SessionDurationMinutes, Is.EqualTo(60));
            Assert.That(settings.Subjects.Count, Is.EqualTo(4));
        }
    }
}