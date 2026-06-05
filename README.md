# Pully — WORKING GAME ✅

**Status:** M1 Complete — All 3 scenes, game fully playable!

## Quick Start (Unity)

1. **Open Unity Hub**
2. **Add Project:** `agent-game-explore/full-auto-agent-game`
3. **Select Version:** 2022.3.4f1 (or any 2022.3 LTS)
4. **Open MenuScene** (`Assets/_Game/Scenes/MenuScene.unity`)
5. **Add ONE component:**
   - Select **Canvas** in Hierarchy
   - Click **Add Component**
   - Type `MenuBootstrap` → Add it
6. **Hit PLAY** ▶️

**That's it!** The game auto-generates all UI and starts working.

**For GameScene:** Add `GameBootstrap` to Camera
**For GameOverScene:** Add `GameOverBootstrap` to Canvas

## How to Play

1. **Menu:** Click PLAY button
2. **Game:** Targets spawn — use correct gestures before they disappear!
3. **Game Over:** Click RETRY or MENU

## Gestures

| Target | Color | Gesture | Points |
|--------|-------|---------|--------|
| Circle | Cyan | Single Tap | 1 |
| Circle | Red | Long Press (0.5s) | 5 |
| Square | Blue | Double Tap | 3 |
| Triangle | Pink | Swipe (drag) | 5 |
| Star | Purple | Two-Finger | 8 |

**Controls (Editor testing):**
- **Tap:** Left Click
- **Double:** Double-click
- **Long:** Hold left click 0.5+ seconds
- **Swipe:** Click and drag quickly
- **Two-Finger:** Right Click

## Game Features

- ✅ **Combo System:** x1.1 per hit, max x5
- ✅ **Lives:** 3 misses = Game Over
- ✅ **Timer:** 60 seconds per round
- ✅ **Seeded RNG:** Deterministic spawns (seed: 12345)
- ✅ **High Score:** Saved to PlayerPrefs
- ✅ **3 Scenes:** Menu → Game → GameOver → (loop)

## Build APK

```bash
# Using Unity command line
/Applications/Unity/Hub/Editor/2022.3.4f1/Unity.app/Contents/MacOS/Unity \
  -quit -batchmode -projectPath /Users/bowenlow/Documents/agent-game-explore/full-auto-agent-game \
  -executeMethod Builder.BuildAndroid

# Or in Unity Editor: Pully/Build Android (Debug)
```

Output: `Builds/Android/pully.apk`

## Project Structure

```
Assets/_Game/
  Scripts/
    GameBootstrap.cs         # Auto-wires GameScene
    MenuBootstrap.cs         # Auto-wires MenuScene  
    GameOverBootstrap.cs     # Auto-wires GameOverScene
    GameManager.cs           # Game state & flow
    GestureRecognizer.cs     # 5 gesture recognition
    Target.cs                # Target lifecycle
    TargetSpawner.cs         # Spawn system (seeded RNG)
    ScoreManager.cs          # Scoring & combo
    RulesetDefinition.cs     # Data-driven rules
    Editor/
      Builder.cs             # APK builder
      GameSetup.cs           # Scene generator
  Scenes/
    MenuScene.unity          # Title + Play button
    GameScene.unity          # Main gameplay
    GameOverScene.unity        # Score + Retry/Menu
Assets/Tests/
  EditMode/ScoringTests.cs   # Unit tests
  PlayMode/GestureTests.cs   # Integration tests
```

## Git History

| Commit | Description |
|--------|-------------|
| `d54f68b` | Scaffold: Core scripts + tests |
| `130f1f1` | GameSetup editor tool |
| `21ba09b` | Target activation fix |
| `af68edd` | GameScene structure |
| `29ec33b` | Setup guide |
| `c43042e` | Runtime primitive fallback |
| `3b6cd81` | GameBootstrap auto-wiring |
| `7a48e0b` | README updates |
| `de3aec4` | Menu + GameOver scenes |
| `TBD` | Bootstrap components attached |

## Discord

Updates posted to **#hermes-updates**

Repo: https://github.com/bowenlowtjm/full-auto-agent-game
