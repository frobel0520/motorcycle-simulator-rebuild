using MotorcycleSimulator.Core.Control;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Settings
{
    /// <summary>
    /// Shared, versioned kinematic parameters for motorcycle prefabs.
    /// Store the asset at Resources/MotorcycleKinematicSettings so runtime
    /// bodies can load it without a manual object reference.
    /// </summary>
    [CreateAssetMenu(
        fileName = "MotorcycleKinematicSettings",
        menuName = "Motorcycle Simulator/Motorcycle Kinematic Settings")]
    public sealed class MotorcycleKinematicSettings : ScriptableObject
    {
        [Header("Geometry")]
        [SerializeField, Min(0.01f)] private float wheelBase = 1.4f;

        [Header("Longitudinal limits (m/s, m/s²)")]
        [SerializeField, Min(0.01f)] private float maximumAcceleration = 3f;
        [SerializeField, Min(0.01f)] private float maximumDeceleration = 5f;
        [SerializeField, Min(0.01f)] private float maximumSpeed = 13.88f;

        [Header("Steering")]
        [SerializeField, Range(1f, 89f)] private float maximumSteeringAngleDegrees = 30f;

        public float MaximumSpeed => maximumSpeed;

        public KinematicBicycleParameters CreateParameters()
        {
            return new KinematicBicycleParameters(
                wheelBase,
                maximumAcceleration,
                maximumDeceleration,
                maximumSpeed,
                maximumSteeringAngleDegrees * Mathf.Deg2Rad);
        }
    }
}
