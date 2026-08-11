using System;
using MotorcycleSimulator.Core.Risk;
using NUnit.Framework;

namespace MotorcycleSimulator.Tests.Editor
{
    public sealed class RayRiskMapTests
    {
        private static readonly RayRiskSettings Settings = new(maximumDistance: 5f, relativeSpeedWeight: 0.5f);

        [Test]
        public void Evaluate_AssignsMoreRiskToNearerObstacle()
        {
            var map = RayRiskMap.Evaluate(
                Settings,
                new RayObservation(false, 0f, 0f),
                new RayObservation(false, 0f, 0f),
                new RayObservation(true, 1f, 0f),
                new RayObservation(true, 4f, 0f),
                new RayObservation(false, 0f, 0f));

            Assert.That(map[RiskDirection.Forward], Is.GreaterThan(map[RiskDirection.Right30]));
        }

        [Test]
        public void Evaluate_AssignsMoreRiskToClosingObstacle()
        {
            var map = RayRiskMap.Evaluate(
                Settings,
                new RayObservation(true, 2f, -5f),
                new RayObservation(true, 2f, 5f),
                new RayObservation(false, 0f, 0f),
                new RayObservation(false, 0f, 0f),
                new RayObservation(false, 0f, 0f));

            Assert.That(map[RiskDirection.Left60], Is.GreaterThan(map[RiskDirection.Left30]));
        }

        [Test]
        public void Evaluate_NormalizesTheDistribution()
        {
            var map = RayRiskMap.Evaluate(
                Settings,
                new RayObservation(true, 1f, -1f),
                new RayObservation(true, 2f, 0f),
                new RayObservation(true, 3f, 1f),
                new RayObservation(false, 0f, 0f),
                new RayObservation(false, 0f, 0f));

            var sum = map[RiskDirection.Left60] + map[RiskDirection.Left30] + map[RiskDirection.Forward]
                + map[RiskDirection.Right30] + map[RiskDirection.Right60];

            Assert.That(sum, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void Evaluate_RequiresFiveObservations()
        {
            Assert.That(
                () => RayRiskMap.Evaluate(Settings, new RayObservation(false, 0f, 0f)),
                Throws.TypeOf<ArgumentException>());
        }
    }
}
