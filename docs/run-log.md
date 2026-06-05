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
- 2025-06-05 12:55 UTC — Commit `29ec33b`: README with setup guide
- 2025-06-05 12:58 UTC — Commit `c43042e`: Runtime primitive fallback for zero-config play
- 2025-06-05 13:00 UTC — Commit `3b6cd81`: **GAME WORKS** - GameBootstrap auto-wires everything
- **Status:** M1 COMPLETE — Game plays in Unity Editor with zero config!
