# CONVENTIONS.md

Read before writing code. Keep edits consistent with these.

## Project
- Unity 6 LTS, 3D project, 2D-sprite gameplay (ortho/shallow-perspective camera), portrait.
- All game content under `Assets/_Game/` (`Scripts/`, `Scenes/`, `Sprites/`, `Data/`).
- One assembly definition: `Pully.Game` (namespace `Pully.Game`).
- Tests under `Assets/Tests/` — `Pully.Tests.EditMode`, `Pully.Tests.PlayMode`.

## Code
- C#, current Unity conventions; PascalCase types/methods, camelCase fields.
- Ruleset values come from the `RulesetDefinition` SO — never hardcode the mapping or thresholds.
- Seed all RNG from the ruleset `seed`; gameplay must be deterministic per seed.
- Input via the new Input System; support touch + mouse (Editor).

## Build & assets
- Build only via `Editor/Builder.cs` batchmode; APK → `Builds/Android/pully.apk`.
- Sprites go through the Game Art atlas; correct import settings (PPU, point filter for pixel art).
- Commit `.meta` files; never delete them by hand.

## Process
- Branch per issue; PR links `SAA-###`; merge only when green.
- Append `run-log.md` + post Discord on every significant change.
