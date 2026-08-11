using MotorcycleSimulator.Core.Risk;
using MotorcycleSimulator.Runtime.Perception;
using MotorcycleSimulator.Runtime.Scenarios;
using MotorcycleSimulator.Runtime.Vehicles;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Agents
{
    /// <summary>
    /// Editor-facing, runtime-only display of the numerical observations sent
    /// to ML-Agents. It owns no scene references and can be removed for builds.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KinematicMotorcycleBody))]
    [RequireComponent(typeof(FiveRayRiskSensor))]
    public sealed class MotorcycleObservationHud : MonoBehaviour
    {
        private KinematicMotorcycleBody motorcycleBody;
        private FiveRayRiskSensor riskSensor;
        private MotorcycleEpisodeController episodeController;

        private void Awake()
        {
            motorcycleBody = GetComponent<KinematicMotorcycleBody>();
            riskSensor = GetComponent<FiveRayRiskSensor>();
            episodeController = FindFirstObjectByType<MotorcycleEpisodeController>();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying || riskSensor.CurrentRiskMap == null)
            {
                return;
            }

            var risk = riskSensor.CurrentRiskMap;
            var goal = episodeController == null
                ? Vector2.zero
                : episodeController.GetNormalizedLocalGoalOffset(transform);

            GUILayout.BeginArea(new Rect(12f, 12f, 310f, 230f), GUI.skin.box);
            GUILayout.Label("ML-Agents observation debug (8 values)");
            GUILayout.Label($"Speed normalized: {motorcycleBody.NormalizedSpeed:F3}");
            GUILayout.Label($"Risk Left 60:  {risk[RiskDirection.Left60]:F3}");
            GUILayout.Label($"Risk Left 30:  {risk[RiskDirection.Left30]:F3}");
            GUILayout.Label($"Risk Forward:  {risk[RiskDirection.Forward]:F3}");
            GUILayout.Label($"Risk Right 30: {risk[RiskDirection.Right30]:F3}");
            GUILayout.Label($"Risk Right 60: {risk[RiskDirection.Right60]:F3}");
            GUILayout.Label($"Goal local X: {goal.x:F3}");
            GUILayout.Label($"Goal local Z: {goal.y:F3}");
            GUILayout.EndArea();
        }
    }
}
