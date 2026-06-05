# GOTCHAS.md — accumulated Unity/agent traps

Read before each phase. Append any new trap + its fix so the next run (and the next phase) avoids it. Seeded with common Unity-agent pitfalls; confirm/replace with what you actually hit.

## Seeded (verify in your setup)
- **.meta files:** every asset has a `.meta` with a GUID. Don't delete/regenerate casually — it breaks references. Commit them.
- **Scene wiring via MCP:** assigning serialized references through the Editor bridge often needs the object to exist + scene saved first; expect retries.
- **Input System:** project must be set to the new Input System (Player Settings) or touch reads return nothing.
- **Batchmode build:** no enabled scenes in Build Settings → silent empty build. `Builder.cs` exits 2 to catch this.
- **Android module:** missing SDK/NDK/JDK → build fails late. Verify in M0.
- **Sprite import:** pixel art blurs unless filter mode = Point and compression = None.
- **Determinism:** any `Time.deltaTime`-driven spawn without a fixed/seeded step breaks replay. Drive spawns from the seeded sequence.

## Discovered this experiment
- <date> — <trap> → <fix>
