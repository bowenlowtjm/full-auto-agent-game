#!/bin/bash
# Unity Post-Commit Compilation Checker
# Run this after making code changes to verify they compile in Unity

PROJECT_PATH="/Users/bowenlow/Documents/agent-game-explore/full-auto-agent-game"
UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.4f1/Unity.app/Contents/MacOS/Unity"
LOG_FILE="/tmp/unity_checkpoint.log"

echo ""
echo "╔════════════════════════════════════════════════════════════╗"
echo "║       UNITY CHECKPOINT COMPILATION CHECK                 ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo "Time: $(date)"
echo ""

# Kill existing Unity
pkill -9 -f "Unity.*full-auto-agent-game" 2>/dev/null
sleep 2

# Compile
echo ">>> Compiling project in Unity..."
"$UNITY_PATH" -projectPath "$PROJECT_PATH" -batchmode -quit -logFile "$LOG_FILE" 2>&1

# Extract errors
CS_ERRORS=$(grep "error CS" "$LOG_FILE" 2>/dev/null | grep -v "Licensing\|IPC\|signature\|Socket\|LogAssemblyErrors" | wc -l | tr -d ' ')
NULL_ERRORS=$(grep "NullReferenceException" "$LOG_FILE" 2>/dev/null | wc -l | tr -d ' ')

echo ""
echo "=== COMPILATION RESULTS ==="
echo "C# Errors: $CS_ERRORS"
echo "Runtime Exceptions: $NULL_ERRORS"
echo ""

if [ "$CS_ERRORS" -gt 0 ] || [ "$NULL_ERRORS" -gt 0 ]; then
    echo "╔════════════════════════════════════════════════════════════╗"
    echo "║  ❌ CHECKPOINT FAILED - COMPILATION ERRORS DETECTED      ║"
    echo "╚════════════════════════════════════════════════════════════╝"
    echo ""
    echo "Errors:"
    grep -E "error CS|NullReferenceException" "$LOG_FILE" | grep -v "Licensing\|IPC\|signature\|Socket\|LogAssemblyErrors" | head -10
    echo ""
    echo "Full log: $LOG_FILE"
    exit 1
else
    echo "╔════════════════════════════════════════════════════════════╗"
    echo "║  ✅ CHECKPOINT PASSED - NO COMPILATION ERRORS            ║"
    echo "╚════════════════════════════════════════════════════════════╝"
    exit 0
fi
