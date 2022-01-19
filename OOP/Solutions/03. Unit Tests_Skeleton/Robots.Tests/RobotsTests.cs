namespace Robots.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class RobotsTests
    {
        private RobotManager robots;
        private Robot robov = new Robot("Robov", 15);

        [SetUp]
        public void InitTest()
        {
            robots = new RobotManager(2);
        }

        [Test]
        public void NegativeCapacity()
        {
            Assert.That(() => { robots = new RobotManager(-5); },
                Throws.ArgumentException);
        }

        [Test]
        public void AreadyInList()
        {
            robots.Add(robov);
            Assert.That(() => { robots.Add(robov); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void NoCapacity()
        {
            robots = new RobotManager(1);
            robots.Add(robov);
            Assert.That(() => { robots.Add(new Robot("Marius", 10)); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveInvalid()
        {
            robots.Add(robov);
            Assert.That(() => { robots.Remove("Marius"); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void CountGrows()
        {
            robots.Add(robov);
            robots.Add(new Robot("Marius", 10));
            Assert.That(robots.Count, Is.EqualTo(2));
        }

        [Test]
        public void CountDowns()
        {
            robots.Add(robov);
            robots.Add(new Robot("Marius", 10));
            robots.Remove("Robov");
            Assert.That(robots.Count, Is.EqualTo(1));
        }

        [Test]
        public void InvalidWorker()
        {
            robots.Add(robov);
            Assert.That(() => { robots.Work("Alibaba", "kopae", 12); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void NoBattery()
        {
            robots.Add(robov);
            Assert.That(() => { robots.Work("Robov", "kopae", 20); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void BatteryDowns()
        {
            robots.Add(robov);
            int battery = 12;
            robots.Work("Robov", "Pee", 3);
            Assert.That(robov.Battery, Is.EqualTo(battery));
        }

        [Test]
        public void InvalidCharge()
        {
            Assert.That(() => { robots.Charge("suleiman"); },
                Throws.InvalidOperationException);
        }

        [Test]
        public void Chargeable()
        {
            robots.Add(robov);
            int battery = robov.MaximumBattery;
            robots.Work("Robov", "Pie", 10);
            robots.Charge("Robov");
            Assert.That(robov.Battery, Is.EqualTo(battery));
        }
    }
}
