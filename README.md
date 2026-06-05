# templates/ — scaffold payload (copy into each new run repo)

These files are copied **verbatim into the new run repo** at Step 0 of [../RUN-PROTOCOL.md](../RUN-PROTOCOL.md), then filled in. They are not the spec — the spec is in `../spec/`.

| Path (→ repo root) | What it is | Fill-in |
|--------------------|-----------|---------|
| `AGENTS.md` | run-repo routing + conventions + spec pointer | none — ready |
| `DESIGN.md` | art taste memory | palette + style (Game Art / run param) |
| `Editor/Builder.cs` | batchmode Android build (agent-callable) | none — ready |
| `Assets/_Game/Scripts/RulesetDefinition.cs` | data-driven ruleset SO | create an asset, populate from `spec/RULESET.md` |
| `Assets/_Game/Scripts/Pully.Game.asmdef` | game assembly | none — ready |
| `docs/decisions.md` · `CONVENTIONS.md` · `GOTCHAS.md` · `run-log.md` | project memory | append during the run |
| `.gitignore` | Unity ignores | none — ready |

After copying: create the Unity 6 **3D** project around these (Android module, Input System, 2D Sprite + 2D Atlas), then commit `chore: scaffold <RUN_ID> from spec kit`.

> Note: Unity also generates `ProjectSettings/`, `Packages/`, and `.meta` files when you create/open the project — commit those too. They aren't templated here because Unity produces them.
