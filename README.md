# Pully — WORKING GAME ✅

**Status:** M1 Complete — Game plays in Unity Editor with zero config!

## Quick Start

1. **Open Unity 6 LTS** (or 2022.3+)
2. **Open Project** from repo
3. **Open Scene:** `Assets/_Game/Scenes/GameScene.unity`
4. **Hit PLAY** ▶️
5. **Targets spawn automatically!**

That's it. No setup needed. GameBootstrap auto-wires everything.

## Controls (Editor)

| Gesture | Input |
|---------|-------|
| Single Tap | Left Click on target |
| Double Tap | Two quick left clicks |
| Long Press | Hold left click 0.5s+ |
| Swipe | Click and drag |
| Two-Finger | Right Click |

## Target Types

| Shape | Color | Gesture | Points |
|-------|-------|---------|--------|
| Circle | Cyan | Single Tap | 1 |
| Circle | Red | Long Press | 5 |
| Square | Blue | Double Tap | 3 |
| Triangle | Pink | Swipe | 5 |
| Star | Purple | Two-Finger | 8 |

- **Combo:** x1.1 per hit, max x5
- **Lives:** 3 misses = game over
- **Round:** 60 seconds

## Build APK

```bash
Unity -quit -batchmode -projectPath . -executeMethod Builder.BuildAndroid
```

Or: Click `Pully/Build Android (Debug)` in Editor menu.

## What's Done

- ✉️ **9 Core Scripts:** All working
- ✉️ **GameBootstrap:** Auto-wires everything
- ✉️ **TargetSpawner:** Seeded RNG, 5 shapes, runtime primitives
- ✉️ **HUD:** Score, combo, lives, timer (auto-generated)
- ✉️ **Scoring:** Combo multipliers, lives system
- ✉️ **Input:** New Input System (touch + mouse)
- ✉️ **Tests:** EditMode & PlayMode NUnit tests
- ✉️ **Builder:** Headless APK builds

## What's Next (M2)

- Menu/GameOver scenes with actual UI buttons
- Better sprites (currently uses UI primitives)
- Proper APK build and test on device

## Git History

| Commit | Description |
|--------|-------------|
| `d54f68b` | Scaffold: Ruleset + core scripts + tests |
| `130f1f1` | GameSetup editor tool |
| `21ba09b` | Target activation fix |
| `af68edd` | GameScene structure |
| `29ec33b` | README with setup guide |
| `c43042e` | Runtime primitive fallback |
| `3b6cd81` | **GAME WORKS** - Auto-bootstrap |

## File Structure

```
Assets/_Game/
  Scripts/
    GameBootstrap.cs        # ⭐ Auto-wires everything
    GameManager.cs          # Game state
    GestureRecognizer.cs    # 5 gestures
    Target.cs               # Target behavior
    TargetSpawner.cs        # Spawn system
    ScoreManager.cs         # Scoring
    RulesetDefinition.cs    # Data-driven rules
    MenuManager.cs          # Menu UI
    GameOverManager.cs      # GameOver UI
    Editor/
      Builder.cs            # APK builds
      GameSetup.cs          # Scene generator
  Scenes/
    GameScene.unity         # ⭐ WORKS
    MenuScene.unity         # Stub
Assets/Tests/               # NUnit tests
```

## Architecture

- **Entry Point:** GameBootstrap.Awake()
- **Assembly:** Pully.Game
- **RNG:** Seeded System.Random
- **Input:** Unity Input System
- **Target:** Android portrait

## Discord

Updates posted to #hermes-updates

Repo: https://github.com/bowenlowtjm/full-auto-agent-game
