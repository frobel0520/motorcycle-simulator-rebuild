using System;
using MotorcycleSimulator.Core.Control;
using NUnit.Framework;

namespace MotorcycleSimulator.Tests.Editor
{
    public sealed class PidControllerTests
    {
        [Test]
        public void Step_CombinesProportionalIntegralAndDerivativeTerms()
        {
            var controller = new PidController(2f, 3f, 4f, 0f);

            var output = controller.Step(setpoint: 5f, actual: 3f, deltaTime: 0.5f);

            Assert.That(output, Is.EqualTo(23f).Within(0.0001f));
        }

        [Test]
        public void Step_UsesDeadZone()
        {
            var controller = new PidController(10f, 0f, 0f, deadZone: 0.1f);

            var output = controller.Step(setpoint: 1.05f, actual: 1f, deltaTime: 0.02f);

            Assert.That(output, Is.EqualTo(0f));
        }

        [Test]
        public void Reset_ClearsControllerState()
        {
            var controller = new PidController(0f, 1f, 1f, deadZone: 0f);
            controller.Step(setpoint: 1f, actual: 0f, deltaTime: 1f);

            controller.Reset();
            var output = controller.Step(setpoint: 1f, actual: 0f, deltaTime: 1f);

            Assert.That(output, Is.EqualTo(2f).Within(0.0001f));
        }

        [Test]
        public void Step_RejectsNonPositiveDeltaTime()
        {
            var controller = new PidController(1f, 0f, 0f);

            Assert.That(
                () => controller.Step(setpoint: 1f, actual: 0f, deltaTime: 0f),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
