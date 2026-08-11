using System;

namespace MotorcycleSimulator.Core.Control
{
    /// <summary>Parameters for a planar kinematic bicycle model in SI units.</summary>
    public readonly struct KinematicBicycleParameters
    {
        public KinematicBicycleParameters(
            float wheelBase,
            float maximumAcceleration,
            float maximumDeceleration,
            float maximumSpeed,
            float maximumSteeringAngleRadians)
        {
            if (wheelBase <= 0f) throw new ArgumentOutOfRangeException(nameof(wheelBase));
            if (maximumAcceleration <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumAcceleration));
            if (maximumDeceleration <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumDeceleration));
            if (maximumSpeed <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumSpeed));
            if (maximumSteeringAngleRadians <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumSteeringAngleRadians));

            WheelBase = wheelBase;
            MaximumAcceleration = maximumAcceleration;
            MaximumDeceleration = maximumDeceleration;
            MaximumSpeed = maximumSpeed;
            MaximumSteeringAngleRadians = maximumSteeringAngleRadians;
        }

        public float WheelBase { get; }
        public float MaximumAcceleration { get; }
        public float MaximumDeceleration { get; }
        public float MaximumSpeed { get; }
        public float MaximumSteeringAngleRadians { get; }
    }

    /// <summary>Pose and longitudinal speed in Unity's XZ ground plane.</summary>
    public readonly struct KinematicBicycleState
    {
        public KinematicBicycleState(float x, float z, float headingRadians, float speed)
        {
            X = x;
            Z = z;
            HeadingRadians = headingRadians;
            Speed = speed;
        }

        public float X { get; }
        public float Z { get; }
        public float HeadingRadians { get; }
        public float Speed { get; }
    }

    /// <summary>Normalized longitudinal and steering commands, each in [-1, 1].</summary>
    public readonly struct KinematicBicycleControl
    {
        public KinematicBicycleControl(float acceleration, float steering)
        {
            Acceleration = acceleration;
            Steering = steering;
        }

        public float Acceleration { get; }
        public float Steering { get; }
    }

    /// <summary>
    /// Pure rear-axle kinematic bicycle integration. It deliberately models
    /// planar navigation only; lean, tyre slip, and balance remain a future
    /// motorcycle-specific dynamics layer.
    /// </summary>
    public static class KinematicBicycleModel
    {
        public static KinematicBicycleState Step(
            KinematicBicycleParameters parameters,
            KinematicBicycleState state,
            KinematicBicycleControl control,
            float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            var accelerationInput = Clamp(control.Acceleration, -1f, 1f);
            var steeringInput = Clamp(control.Steering, -1f, 1f);
            var acceleration = accelerationInput >= 0f
                ? accelerationInput * parameters.MaximumAcceleration
                : accelerationInput * parameters.MaximumDeceleration;
            var speed = Clamp(state.Speed + acceleration * deltaTime, 0f, parameters.MaximumSpeed);
            var steeringAngle = steeringInput * parameters.MaximumSteeringAngleRadians;
            var yawRate = speed / parameters.WheelBase * MathF.Tan(steeringAngle);
            var heading = state.HeadingRadians + yawRate * deltaTime;

            // Heading 0 points along Unity's positive Z axis.
            var x = state.X + speed * MathF.Sin(heading) * deltaTime;
            var z = state.Z + speed * MathF.Cos(heading) * deltaTime;

            return new KinematicBicycleState(x, z, heading, speed);
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            return MathF.Min(MathF.Max(value, minimum), maximum);
        }
    }
}
