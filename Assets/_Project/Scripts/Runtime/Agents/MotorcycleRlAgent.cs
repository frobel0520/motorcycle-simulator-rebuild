using MotorcycleSimulator.Core.Risk;
using MotorcycleSimulator.Runtime.Perception;
using MotorcycleSimulator.Runtime.Scenarios;
using MotorcycleSimulator.Runtime.Vehicles;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MotorcycleSimulator.Runtime.Agents
{
    /// <summary>
    /// Minimal ML-Agents adapter for the motorcycle. It owns no scene-object
    /// references: required components are discovered on this GameObject.
    /// Reward and episode-reset ownership will be added with the scenario system.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KinematicMotorcycleBody))]
    [RequireComponent(typeof(FiveRayRiskSensor))]
    [RequireComponent(typeof(BehaviorParameters))]
    [RequireComponent(typeof(DecisionRequester))]
    public sealed class MotorcycleRlAgent : Agent
    {
        private const int ObservationCount = 8;
        private const int ActionCount = 2;

        private KinematicMotorcycleBody motorcycleBody;
        private FiveRayRiskSensor riskSensor;
        private MotorcycleEpisodeController episodeController;

        protected override void OnEnable()
        {
            ConfigureMlAgentsComponents();
            base.OnEnable();
        }

        public override void Initialize()
        {
            motorcycleBody = GetComponent<KinematicMotorcycleBody>();
            riskSensor = GetComponent<FiveRayRiskSensor>();
            episodeController = FindFirstObjectByType<MotorcycleEpisodeController>();
            MaxStep = 1000;
        }

        public override void OnEpisodeBegin()
        {
            if (episodeController == null)
            {
                episodeController = FindFirstObjectByType<MotorcycleEpisodeController>();
            }

            if (episodeController != null)
            {
                MaxStep = episodeController.MaximumEpisodeSteps;
                episodeController.ResetEpisode();
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(motorcycleBody.NormalizedSpeed);

            var riskMap = riskSensor.CurrentRiskMap;
            sensor.AddObservation(riskMap[RiskDirection.Left60]);
            sensor.AddObservation(riskMap[RiskDirection.Left30]);
            sensor.AddObservation(riskMap[RiskDirection.Forward]);
            sensor.AddObservation(riskMap[RiskDirection.Right30]);
            sensor.AddObservation(riskMap[RiskDirection.Right60]);

            var localGoalOffset = episodeController == null
                ? Vector2.zero
                : episodeController.GetNormalizedLocalGoalOffset(transform);
            sensor.AddObservation(localGoalOffset.x);
            sensor.AddObservation(localGoalOffset.y);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var continuous = actions.ContinuousActions;
            motorcycleBody.ApplyControl(continuous[0], continuous[1]);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuous = actionsOut.ContinuousActions;
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                continuous[0] = 0f;
                continuous[1] = 0f;
                return;
            }

            continuous[0] = (keyboard.wKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed ? 1f : 0f);
            continuous[1] = (keyboard.dKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed ? 1f : 0f);
        }

        private void ConfigureMlAgentsComponents()
        {
            var behavior = GetComponent<BehaviorParameters>();
            behavior.BehaviorName = "MotorcycleRiskAware";
            behavior.BehaviorType = BehaviorType.Default;
            behavior.BrainParameters.VectorObservationSize = ObservationCount;
            behavior.BrainParameters.NumStackedVectorObservations = 1;
            behavior.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(ActionCount);
            behavior.BrainParameters.VectorActionDescriptions = new[]
            {
                "normalized longitudinal acceleration",
                "normalized steering",
            };

            var decisionRequester = GetComponent<DecisionRequester>();
            decisionRequester.DecisionPeriod = 1;
            decisionRequester.DecisionStep = 0;
            decisionRequester.TakeActionsBetweenDecisions = true;
        }
    }
}
