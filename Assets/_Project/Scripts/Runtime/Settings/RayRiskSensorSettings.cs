using MotorcycleSimulator.Core.Risk;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Settings
{
    /// <summary>
    /// Shared geometry and risk parameters for the five-ray perception adapter.
    /// Store the asset at Resources/RayRiskSensorSettings.
    /// </summary>
    [CreateAssetMenu(
        fileName = "RayRiskSensorSettings",
        menuName = "Motorcycle Simulator/Ray Risk Sensor Settings")]
    public sealed class RayRiskSensorSettings : ScriptableObject
    {
        [Header("Ray Geometry")]
        [SerializeField, Min(0.01f)] private float maximumDistance = 5f;
        [SerializeField, Min(0f)] private float sphereRadius = 0.5f;
        [SerializeField] private LayerMask obstacleLayers = ~0;

        [Header("Risk Model")]
        [SerializeField, Range(0f, 0.99f)] private float relativeSpeedWeight = 0.5f;

        public float MaximumDistance => maximumDistance;
        public float SphereRadius => sphereRadius;
        public LayerMask ObstacleLayers => obstacleLayers;

        public RayRiskSettings CreateRiskSettings()
        {
            return new RayRiskSettings(maximumDistance, relativeSpeedWeight);
        }
    }
}
