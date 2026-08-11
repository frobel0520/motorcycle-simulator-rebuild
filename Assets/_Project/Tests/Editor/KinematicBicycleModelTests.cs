using System;
using MotorcycleSimulator.Core.Control;
using NUnit.Framework;

namespace MotorcycleSimulator.Tests.Editor
{
    public sealed class KinematicBicycleModelTests
    {
        private static readonly KinematicBicycleParameters Parameters = new(
            wheelBase: 1.4f,
            maximumAcceleration: 3f,
            maximumDeceleration: 5f,
            maximumSpeed: 15f,
            maximumSteeringAngleRadians: MathF.PI / 6f);

        [Test]
        public void Step_AcceleratesAndMovesForward()
        {
            var state = KinematicBicycleModel.Step(
                Parameters,
                new KinematicBicycleState(0f, 0f, 0f, 0f),
                new KinematicBicycleControl(1f, 0f),
                deltaTime: 1f);

            Assert.That(state.Speed, Is.EqualTo(3f).Within(0.0001f));
            Assert.That(state.X, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(state.Z, Is.EqualTo(3f).Within(0.0001f));
        }

        [Test]
        public void Step_PositiveSteeringTurnsTowardPositiveX()
        {
            var state = KinematicBicycleModel.Step(
                Parameters,
                new KinematicBicycleState(0f, 0f, 0f, 5f),
                new KinematicBicycleControl(0f, 1f),
                deltaTime: 0.1f);

            Assert.That(state.HeadingRadians, Is.GreaterThan(0f));
            Assert.That(state.X, Is.GreaterThan(0f));
        }

        [Test]
        public void Step_ClampsSpeedToConfiguredLimit()
        {
            var state = KinematicBicycleModel.Step(
                Parameters,
                new KinematicBicycleState(0f, 0f, 0f, 14f),
                new KinematicBicycleControl(1f, 0f),
                deltaTime: 1f);

            Assert.That(state.Speed, Is.EqualTo(15f));
        }

        [Test]
        public void Step_RejectsNonPositiveDeltaTime()
        {
            Assert.That(
                () => KinematicBicycleModel.Step(
                    Parameters,
                    new KinematicBicycleState(0f, 0f, 0f, 0f),
                    new KinematicBicycleControl(0f, 0f),
                    deltaTime: 0f),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
