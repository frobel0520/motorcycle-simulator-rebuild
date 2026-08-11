using System;

namespace MotorcycleSimulator.Core.Traffic
{
    /// <summary>
    /// Immutable parameters for the Intelligent Driver Model (IDM).
    /// All values use SI units: metres and seconds.
    /// </summary>
    public readonly struct IdmParameters
    {
        public IdmParameters(
            float desiredSpeed,
            float minimumGap,
            float desiredTimeHeadway,
            float maximumAcceleration,
            float comfortableDeceleration,
            int accelerationExponent = 4)
        {
            if (desiredSpeed <= 0f) throw new ArgumentOutOfRangeException(nameof(desiredSpeed));
            if (minimumGap < 0f) throw new ArgumentOutOfRangeException(nameof(minimumGap));
            if (desiredTimeHeadway < 0f) throw new ArgumentOutOfRangeException(nameof(desiredTimeHeadway));
            if (maximumAcceleration <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumAcceleration));
            if (comfortableDeceleration <= 0f) throw new ArgumentOutOfRangeException(nameof(comfortableDeceleration));
            if (accelerationExponent <= 0) throw new ArgumentOutOfRangeException(nameof(accelerationExponent));

            DesiredSpeed = desiredSpeed;
            MinimumGap = minimumGap;
            DesiredTimeHeadway = desiredTimeHeadway;
            MaximumAcceleration = maximumAcceleration;
            ComfortableDeceleration = comfortableDeceleration;
            AccelerationExponent = accelerationExponent;
        }

        public float DesiredSpeed { get; }
        public float MinimumGap { get; }
        public float DesiredTimeHeadway { get; }
        public float MaximumAcceleration { get; }
        public float ComfortableDeceleration { get; }
        public int AccelerationExponent { get; }
    }

    /// <summary>Leader state expressed in the follower's longitudinal frame.</summary>
    public readonly struct IdmLeader
    {
        public IdmLeader(float netDistance, float speed)
        {
            NetDistance = netDistance;
            Speed = speed;
        }

        public float NetDistance { get; }
        public float Speed { get; }
    }

    /// <summary>
    /// Pure IDM acceleration model. Unity perception and Rigidbody access belong
    /// in a runtime adapter, not in this class.
    /// </summary>
    public static class IdmModel
    {
        private const float MinimumComputableGap = 0.01f;

        public static float ComputeAcceleration(
            IdmParameters parameters,
            float speed,
            IdmLeader? leader = null)
        {
            if (speed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(speed));
            }

            var freeRoadTerm = MathF.Pow(speed / parameters.DesiredSpeed, parameters.AccelerationExponent);
            var interactionTerm = 0f;

            if (leader.HasValue)
            {
                var leaderState = leader.Value;
                var closingSpeed = speed - leaderState.Speed;
                var desiredGap = parameters.MinimumGap
                    + speed * parameters.DesiredTimeHeadway
                    + speed * closingSpeed
                        / (2f * MathF.Sqrt(parameters.MaximumAcceleration * parameters.ComfortableDeceleration));

                var gap = MathF.Max(leaderState.NetDistance, MinimumComputableGap);
                interactionTerm = MathF.Pow(desiredGap / gap, 2f);
            }

            return parameters.MaximumAcceleration * (1f - freeRoadTerm - interactionTerm);
        }
    }
}
