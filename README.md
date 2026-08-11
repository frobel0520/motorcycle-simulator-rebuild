# Motorcycle Simulator Rebuild

This project rebuilds the motorcycle mixed-traffic simulator on Unity 6.3 LTS.

## Project structure

- `Assets/_Project/Art`: project-owned visual assets.
- `Assets/_Project/Prefabs`: reusable scene objects.
- `Assets/_Project/Scenes`: simulation scenes.
- `Assets/_Project/Scripts`: project code.
- `Assets/_Project/Settings`: ScriptableObject-based experiment settings.

## Scene convention

Every simulation scene has a root object named `Simulation` with a child named
`Systems`. `SimulationBootstrap` discovers this child automatically. Runtime
systems must use this stable hierarchy, prefab-local component discovery, or
explicit configuration assets instead of ad-hoc Inspector wiring.

## Dependency rules

1. Keep traffic, risk, reward, and vehicle-domain logic independent of Unity
   APIs whenever practical.
2. Use MonoBehaviours only as adapters between Unity and domain logic.
3. Use ScriptableObjects for experiment parameters that need tuning.
4. Use Inspector references only for meaningful scene composition, not routine
   component wiring.
