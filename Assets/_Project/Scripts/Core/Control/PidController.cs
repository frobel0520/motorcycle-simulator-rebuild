using System;

namespace MotorcycleSimulator.Core.Control
{
    /// <summary>
    /// Stateful PID controller with no Unity dependency.
    /// Create one controller per controlled signal and call Reset at episode start.
    /// </summary>
    public sealed class PidController
    {
        private readonly float proportionalGain;
        private readonly float integralGain;
        private readonly float derivativeGain;
        private readonly float deadZone;

        private float integral;
        private float previousError;

        public PidController(
            float proportionalGain,
            float integralGain,
            float derivativeGain,
            float deadZone = 0.01f)
        {
            if (deadZone < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deadZone));
            }

            this.proportionalGain = proportionalGain;
            this.integralGain = integralGain;
            this.derivativeGain = derivativeGain;
            this.deadZone = deadZone;
        }

        public float Step(float setpoint, float actual, float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time must be positive.");
            }

            var error = setpoint - actual;
            if (MathF.Abs(error) < deadZone)
            {
                error = 0f;
            }

            integral += error * deltaTime;
            var derivative = (error - previousError) / deltaTime;
            previousError = error;

            return proportionalGain * error + integralGain * integral + derivativeGain * derivative;
        }

        public void Reset()
        {
            integral = 0f;
            previousError = 0f;
        }
    }
}
