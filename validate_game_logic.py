#!/usr/bin/env python3
"""
Standalone test validator for Pully game logic.
Doesn't require Unity - validates core scoring/math logic independently.
"""

import sys

class Color:
    """Represents Unity Color"""
    def __init__(self, r, g, b, a=1.0):
        self.r = r
        self.g = g
        self.b = b
        self.a = a
    
    def __repr__(self):
        return f"Color({self.r:.3f}, {self.g:.3f}, {self.b:.3f})"

class RulesetDefinition:
    """Mock of Unity ScriptableObject"""
    class Shape:
        Circle = 0
        Square = 1
        Triangle = 2
        Star = 3
    
    class Gesture:
        SingleTap = 0
        DoubleTap = 1
        LongPress = 2
        SwipeTap = 3
        TwoFingerTap = 4
    
    def __init__(self):
        self.rules = []
        self.comboStep = 1.1
        self.comboCap = 5.0
        self.lives = 3
        self.roundSeconds = 60.0
        self.targetLifetime = 1.6
        self.spawnIntervalStart = 1.2
        self.spawnIntervalEnd = 0.6
        self.maxConcurrentTargets = 4
        self.doubleTapWindow = 0.3
        self.longPressDuration = 0.5
        self.swipeMinDistance = 50.0
        self.seed = 12345

class TargetRule:
    def __init__(self, shape, color, gesture, baseReward):
        self.shape = shape
        self.color = color
        self.requiredGesture = gesture
        self.baseReward = baseReward

class ScoreManager:
    """Pure Python implementation for testing"""
    def __init__(self, ruleset):
        self._ruleset = ruleset
        self._score = 0
        self._combo = 1.0
        self._lives = ruleset.lives
        self._isGameOver = False
    
    @property
    def Score(self): return self._score
    @property
    def ComboMultiplier(self): return self._combo
    @property
    def Lives(self): return self._lives
    @property
    def IsGameOver(self): return self._isGameOver
    
    def Initialize(self):
        self._score = 0
        self._combo = 1.0
        self._lives = self._ruleset.lives
        self._isGameOver = False
    
    def OnTargetHit(self, baseReward):
        """Score formula: baseReward * comboMultiplier"""
        reward = int(baseReward * self._combo)
        self._score += reward
        self._combo = min(self._combo * self._ruleset.comboStep, self._ruleset.comboCap)
        return reward
    
    def OnTargetMiss(self):
        """Miss reduces lives and resets combo"""
        self._lives -= 1
        self._combo = 1.0
        if self._lives <= 0:
            self._isGameOver = True

class TestRunner:
    def __init__(self):
        self.passed = 0
        self.failed = 0
    
    def assert_equals(self, actual, expected, msg=""):
        if actual == expected:
            print(f"  [OK] PASS: {msg}")
            self.passed += 1
            return True
        else:
            print(f"  [X] FAIL: {msg}")
            print(f"       Expected: {expected}, Got: {actual}")
            self.failed += 1
            return False
    
    def assert_true(self, condition, msg=""):
        if condition:
            print(f"  [OK] PASS: {msg}")
            self.passed += 1
        else:
            print(f"  [X] FAIL: {msg}")
            self.failed += 1
    
    def assert_greater(self, actual, expected, msg=""):
        if actual > expected:
            print(f"  [OK] PASS: {msg}")
            self.passed += 1
            return True
        else:
            print(f"  [X] FAIL: {msg}")
            print(f"       Expected > {expected}, Got: {actual}")
            self.failed += 1
            return False
    
    def run_all_tests(self):
        print("=" * 60)
        print("PULLY GAME LOGIC VALIDATOR")
        print("=" * 60)
        print()
        
        self.test_score_manager_init()
        self.test_score_on_hit()
        self.test_combo_building()
        self.test_combo_cap()
        self.test_miss_penalty()
        self.test_game_over()
        self.test_ruleset_generation()
        self.test_seeded_rng()
        
        print()
        print("=" * 60)
        total = self.passed + self.failed
        print(f"RESULTS: {self.passed}/{total} passed, {self.failed}/{total} failed")
        print("=" * 60)
        
        return self.failed == 0
    
    def test_score_manager_init(self):
        print("\n[1] ScoreManager Initialization")
        ruleset = RulesetDefinition()
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        self.assert_equals(sm.Score, 0, "Initial score is 0")
        self.assert_equals(sm.ComboMultiplier, 1.0, "Initial combo is 1.0")
        self.assert_equals(sm.Lives, 3, "Initial lives = ruleset")
        self.assert_equals(sm.IsGameOver, False, "Game not over initially")
    
    def test_score_on_hit(self):
        print("\n[2] Score Calculation on Hit")
        ruleset = RulesetDefinition()
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        reward = sm.OnTargetHit(1)
        self.assert_equals(reward, 1, "First hit: reward = 1 * combo(1.0)")
        self.assert_equals(sm.Score, 1, "Score updated to 1")
    
    def test_combo_building(self):
        print("\n[3] Combo Building")
        ruleset = RulesetDefinition()
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        sm.OnTargetHit(1)
        self.assert_greater(sm.ComboMultiplier, 1.0, "Combo increased after hit")
        
        combo1 = sm.ComboMultiplier
        sm.OnTargetHit(1)
        self.assert_greater(sm.ComboMultiplier, combo1, "Combo continues increasing")
    
    def test_combo_cap(self):
        print("\n[4] Combo Cap")
        ruleset = RulesetDefinition()
        ruleset.comboCap = 3.0
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        # Hit 20 times
        for _ in range(20):
            sm.OnTargetHit(1)
        
        self.assert_equals(sm.ComboMultiplier, 3.0, "Combo capped at max value")
    
    def test_miss_penalty(self):
        print("\n[5] Miss Penalty")
        ruleset = RulesetDefinition()
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        # Build combo first
        sm.OnTargetHit(1)
        self.assert_greater(sm.ComboMultiplier, 1.0, "Combo built before miss")
        
        # Miss
        sm.OnTargetMiss()
        self.assert_equals(sm.Lives, 2, "Lives decreased")
        self.assert_equals(sm.ComboMultiplier, 1.0, "Combo reset on miss")
    
    def test_game_over(self):
        print("\n[6] Game Over")
        ruleset = RulesetDefinition()
        sm = ScoreManager(ruleset)
        sm.Initialize()
        
        # Lose all lives
        for _ in range(3):
            sm.OnTargetMiss()
        
        self.assert_equals(sm.IsGameOver, True, "Game over after all lives lost")
        self.assert_equals(sm.Lives, 0, "Lives at 0")
    
    def test_ruleset_generation(self):
        print("\n[7] Ruleset Generation")
        ruleset = RulesetDefinition()
        
        # Add sample rules
        ruleset.rules.append(TargetRule(
            RulesetDefinition.Shape.Circle,
            Color(0.298, 0.788, 0.941),
            RulesetDefinition.Gesture.SingleTap,
            1
        ))
        ruleset.rules.append(TargetRule(
            RulesetDefinition.Shape.Square,
            Color(0.263, 0.38, 0.933),
            RulesetDefinition.Gesture.DoubleTap,
            3
        ))
        
        self.assert_equals(len(ruleset.rules), 2, "Rules added correctly")
        self.assert_equals(ruleset.rules[0].baseReward, 1, "Circle reward = 1")
        self.assert_equals(ruleset.rules[1].baseReward, 3, "Square reward = 3")
    
    def test_seeded_rng(self):
        print("\n[8] Seeded RNG")
        import random
        
        # Test deterministic RNG
        rng1 = random.Random(12345)
        rng2 = random.Random(12345)
        
        values1 = [rng1.random() for _ in range(10)]
        values2 = [rng2.random() for _ in range(10)]
        
        self.assert_equals(values1, values2, "Same seed = same sequence")

if __name__ == "__main__":
    runner = TestRunner()
    success = runner.run_all_tests()
    sys.exit(0 if success else 1)
