# run-log.md

Append one entry per significant change during the run, and a final summary entry at the end. Mirror the same line into the experiment results table (spec `07-Metrics`).

## Final summary template
```
Run: <RUN_ID>            e.g. pully-B-L3-20260604
Config / Rung / Memory: <A|B> / <L1|L3|L4> / <flat-docs|OpenViking>
Models: orch=<…> workers=<…>
Linear: SAA epic #__, <x>/<y> issues Done
Outcome: APK <built/blocked> · gestures <ok/partial> · art atlas <ok/—>
Gates passed: <n>/9 (see spec/ACCEPTANCE.md)
Code quality: __/15    Gameplay quality: __/10  (latency __ms, __ softlocks)
Human interventions: <count + type>
Time: __h__m    Tokens: ~__ (per-role if team)
Bottleneck: <what cost the most retries>
New gotchas: <promoted to GOTCHAS.md>
Self-report (honest): <what's done / stubbed / known issues>
```

## Running log
- 2025-06-05 12:35 UTC — Scaffold Unity project from templates
- 2025-06-05 12:45 UTC — Commit `d54f68b`: RulesetDefinition + core scripts + tests + docs
- 2025-06-05 12:48 UTC — Commit `130f1f1`: GameSetup editor tool for scene/prefab generation
- 2025-06-05 12:50 UTC — Commit `21ba09b`: Target scale/activation fix
- 2025-06-05 12:52 UTC — Commit `af68edd`: GameScene with camera, canvas, basic UI
- **Status:** M1 (core loop) CODE COMPLETE. Unity import → run Pully/Setup Game → wire prefabs → build APK.
- **Blockers:** Unity project requires manual import to generate meta files and wire scene references.
