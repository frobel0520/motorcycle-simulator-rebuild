using System;
using MotorcycleSimulator.Core.Traffic;
using NUnit.Framework;

namespace MotorcycleSimulator.Tests.Editor
{
    public sealed class IdmModelTests
    {
        private static readonly IdmParameters Parameters = new(
            desiredSpeed: 13.88f,
            minimumGap: 7f,
            desiredTimeHeadway: 1.5f,
            maximumAcceleration: 3f,
            comfortableDeceleration: 1.5f);

        [Test]
        public void ComputeAcceleration_AcceleratesFromRestOnFreeRoad()
        {
            var acceleration = IdmModel.ComputeAcceleration(Parameters, speed: 0f);

            Assert.That(acceleration, Is.EqualTo(3f).Within(0.0001f));
        }

        [Test]
        public void ComputeAcceleration_BrakesForCloseSlowerLeader()
        {
            var acceleration = IdmModel.ComputeAcceleration(
                Parameters,
                speed: 10f,
                leader: new IdmLeader(netDistance: 5f, speed: 2f));

            Assert.That(acceleration, Is.LessThan(0f));
        }

        [Test]
        public void ComputeAcceleration_RemainsFiniteForOverlappingLeader()
        {
            var acceleration = IdmModel.ComputeAcceleration(
                Parameters,
                speed: 5f,
                leader: new IdmLeader(netDistance: 0f, speed: 0f));

            Assert.That(float.IsNaN(acceleration) || float.IsInfinity(acceleration), Is.False);
        }

        [Test]
        public void ComputeAcceleration_RejectsNegativeSpeed()
        {
            Assert.That(
                () => IdmModel.ComputeAcceleration(Parameters, speed: -0.1f),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
