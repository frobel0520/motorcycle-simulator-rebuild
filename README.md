# Risk-Aware Motorcycle Interaction in Mixed Traffic Flow

Unity implementation and ongoing Unity 6 rebuild for the research project **Risk-Aware Motorcycle Interaction in Mixed Traffic Flow via Deep Reinforcement Learning**.

**Paper:** [DOI: 10.1002/cav.70161](https://doi.org/10.1002/cav.70161) | [Google Scholar](https://scholar.google.com/scholar?q=Risk-Aware+Motorcycle+Interaction+in+Mixed+Traffic+Flow+via+Deep+Reinforcement+Learning)

> **Project status:** active rebuild. The original Unity prototype is being restructured into a testable Unity 6.3 LTS codebase. The current repository provides the simulation foundation and a minimal ML-Agents environment; the full training pipeline and mixed-traffic reproduction are still in progress.

## Research overview

This work studies how an ego motorcycle can navigate mixed traffic safely and naturally using deep reinforcement learning. It combines:

- **Ray-based local perception:** five forward rays at -60, -30, 0, +30, and +60 degrees detect nearby traffic and obstacles.
- **Risk-probability mapping:** distance and relative speed are converted into directional safety scores and normalized risk probabilities.
- **Risk-aware control:** the agent selects longitudinal acceleration and steering actions using local risk and goal information.
- **Curriculum learning:** traffic complexity is progressively increased to learn robust interaction behaviours in scenes with cars, buses, and motorcycles.

The published results show collision-free trajectories, responsive deceleration, and varied yet structured motorcycle behaviour for animation-oriented mixed-traffic simulation.

## Current Unity 6 rebuild

| Area | Current state |
| --- | --- |
| Engine | Unity 6.3 LTS (6000.3.20f1), Universal 3D |
| ML framework | Unity ML-Agents 4.0.0 |
| Motorcycle motion | Kinematic bicycle model with normalized acceleration and steering |
| Risk perception | Five-ray sensor and numerically stable risk-probability map |
| Episode management | Goal, collision, boundary, and time-limit termination paths |
| Validation | Unit tests for PID, IDM, kinematic motion, and risk mapping |
| Training | Python/PPO configuration and full mixed-traffic curriculum are pending |

## Repository layout

```text
Assets/_Project/
|-- Prefabs/       Reusable simulation objects
|-- Resources/     Versioned ScriptableObject experiment settings
|-- Scenes/        Simulation.unity
|-- Scripts/
|   |-- Core/      Unity-independent PID, IDM, kinematic, and risk logic
|   `-- Runtime/   Unity adapters, agents, perception, and scenarios
`-- Tests/Editor/  Core-domain unit tests
Docs/              Legacy-project migration audit
```

## Getting started

1. Install **Unity 6.3 LTS (6000.3.20f1)** through Unity Hub.
2. Clone this repository and open the repository root as a Unity project.
3. Open `Assets/_Project/Scenes/Simulation.unity`.
4. Press **Play**. Until a trained model is supplied, use `W` / `S` for acceleration and braking and `A` / `D` for steering through the Agent heuristic.
5. Run **Window > General > Test Runner > EditMode** to validate the Unity-independent core logic.

The `Motorcycle Observation Hud` component can be enabled on the motorcycle to inspect the eight current ML observations: normalized speed, five directional risks, and the local goal offset.

## Design principles

- Keep traffic, risk, reward, and vehicle-domain logic independent of Unity APIs where practical.
- Use MonoBehaviours only as adapters between Unity and the domain model.
- Store tunable experiment settings in ScriptableObjects rather than hand-wired scene references.
- Keep episode and scenario ownership outside the RL Agent to prevent monolithic controller scripts.

## Citation

```bibtex
@article{wei2026riskaware,
  title   = {Risk-Aware Motorcycle Interaction in Mixed Traffic Flow via Deep Reinforcement Learning},
  author  = {Wei, Yu-Tsen and Ma, Kuo-Wei and Chen, Guan-Hao and Wong, Sai-Keung},
  journal = {Computer Animation and Virtual Worlds},
  volume  = {37},
  pages   = {e70161},
  year    = {2026},
  doi     = {10.1002/cav.70161}
}
```

## Authors

Yu-Tsen Wei, Kuo-Wei Ma, Guan-Hao Chen, and Sai-Keung Wong

Department of Computer Science, National Yang Ming Chiao Tung University, Taiwan

## License

No license has been selected yet. All rights reserved unless stated otherwise.
