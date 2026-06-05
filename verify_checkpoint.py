#!/usr/bin/env python3
"""
Unity Checkpoint Verification System
Run this after making code changes to verify compilation
"""

import subprocess
import sys
import os

def run_checkpoint():
    """Run Unity compilation check and report results"""
    
    script_path = "/Users/bowenlow/Documents/agent-game-explore/full-auto-agent-game/checkpoint_verify.sh"
    
    print("\n" + "="*60)
    print("UNITY CHECKPOINT VERIFICATION")
    print("="*60)
    
    try:
        result = subprocess.run(
            [script_path],
            capture_output=True,
            text=True,
            timeout=120
        )
        
        print(result.stdout)
        
        if result.returncode != 0:
            print("\n" + "!"*60)
            print("COMPILATION ERRORS DETECTED!")
            print("!"*60)
            print("\nPlease fix the errors above before continuing.")
            return False
        else:
            print("\n" + "="*60)
            print("✓ Ready to proceed - no compilation errors")
            print("="*60)
            return True
            
    except subprocess.TimeoutExpired:
        print("\n❌ ERROR: Unity compilation timed out (120s)")
        return False
    except Exception as e:
        print(f"\n❌ ERROR: {e}")
        return False

if __name__ == "__main__":
    success = run_checkpoint()
    sys.exit(0 if success else 1)
