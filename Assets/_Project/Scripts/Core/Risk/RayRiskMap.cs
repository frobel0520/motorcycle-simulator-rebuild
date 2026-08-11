using System;

namespace MotorcycleSimulator.Core.Risk
{
    public enum RiskDirection
    {
        Left60 = 0,
        Left30 = 1,
        Forward = 2,
        Right30 = 3,
        Right60 = 4,
    }

    /// <summary>
    /// Per-ray observation. RelativeSpeedAlongHeading is the detected object's
    /// velocity projected onto the ego heading minus ego speed. A negative value
    /// therefore represents a closing object.
    /// </summary>
    public readonly struct RayObservation
    {
        public RayObservation(bool hasHit, float distance, float relativeSpeedAlongHeading)
        {
            HasHit = hasHit;
            Distance = distance;
            RelativeSpeedAlongHeading = relativeSpeedAlongHeading;
        }

        public bool HasHit { get; }
        public float Distance { get; }
        public float RelativeSpeedAlongHeading { get; }
    }

    public readonly struct RayRiskSettings
    {
        public RayRiskSettings(float maximumDistance, float relativeSpeedWeight = 0.5f)
        {
            if (maximumDistance <= 0f) throw new ArgumentOutOfRangeException(nameof(maximumDistance));
            if (relativeSpeedWeight < 0f || relativeSpeedWeight >= 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(relativeSpeedWeight));
            }

            MaximumDistance = maximumDistance;
            RelativeSpeedWeight = relativeSpeedWeight;
        }

        public float MaximumDistance { get; }
        public float RelativeSpeedWeight { get; }
    }

    /// <summary>Normalized risk distribution across the five forward directions.</summary>
    public sealed class RayRiskMap
    {
        private readonly float[] probabilities;

        private RayRiskMap(float[] probabilities)
        {
            this.probabilities = probabilities;
        }

        public float this[RiskDirection direction] => probabilities[(int)direction];

        public static RayRiskMap Evaluate(RayRiskSettings settings, params RayObservation[] observations)
        {
            if (observations == null || observations.Length != 5)
            {
                throw new ArgumentException("Exactly five ray observations are required.", nameof(observations));
            }

            var logits = new float[observations.Length];
            var maximumLogit = float.NegativeInfinity;

            for (var index = 0; index < observations.Length; index++)
            {
                var observation = observations[index];
                var distance = observation.HasHit
                    ? Clamp(observation.Distance, 0f, settings.MaximumDistance)
                    : settings.MaximumDistance;
                var relativeSpeed = observation.HasHit ? observation.RelativeSpeedAlongHeading : 0f;
                var speedFactor = 1f + settings.RelativeSpeedWeight * MathF.Tanh(relativeSpeed);
                var safety = distance * speedFactor;

                // Lower safety corresponds to higher risk.
                logits[index] = -safety;
                maximumLogit = MathF.Max(maximumLogit, logits[index]);
            }

            var probabilities = new float[observations.Length];
            var sum = 0f;
            for (var index = 0; index < logits.Length; index++)
            {
                probabilities[index] = MathF.Exp(logits[index] - maximumLogit);
                sum += probabilities[index];
            }

            for (var index = 0; index < probabilities.Length; index++)
            {
                probabilities[index] /= sum;
            }

            return new RayRiskMap(probabilities);
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            return MathF.Min(MathF.Max(value, minimum), maximum);
        }
    }
}
