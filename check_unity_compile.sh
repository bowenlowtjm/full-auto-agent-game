#!/bin/bash
# Unity Compilation Checker Loop
# Usage: ./check_unity_compile.sh

PROJECT_PATH="/Users/bowenlow/Documents/agent-game-explore/full-auto-agent-game"
UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.4f1/Unity.app/Contents/MacOS/Unity"
LOG_FILE="/tmp/unity_compile_check.log"

echo "=== UNITY COMPILATION CHECK ==="
echo "Time: $(date)"
echo ""

# Kill any running Unity instances to ensure clean compile
pkill -9 -f "Unity.*full-auto-agent-game" 2>/dev/null
sleep 2

# Run Unity in batch mode to compile
echo "Starting Unity compilation..."
"$UNITY_PATH" -projectPath "$PROJECT_PATH" -batchmode -quit -logFile "$LOG_FILE" 2>&1
EXIT_CODE=$?

# Check for compilation errors
echo ""
echo "=== CHECKING FOR ERRORS ==="

# Count different error types
CS_ERRORS=$(grep -c "error CS" "$LOG_FILE" 2>/dev/null || echo "0")
NULL_REF=$(grep -c "NullReferenceException" "$LOG_FILE" 2>/dev/null || echo "0")
FATAL_ERRORS=$(grep -c "Fatal\|Aborting batchmode" "$LOG_FILE" 2>/dev/null || echo "0")
BUILD_SUCCESS=$(grep -c "Tundra build success\|Mono: successfully reloaded" "$LOG_FILE" 2>/dev/null || echo "0")

echo "C# Compilation Errors: $CS_ERRORS"
echo "Null Reference Exceptions: $NULL_REF"
echo "Fatal Errors: $FATAL_ERRORS"
echo "Build Success Indicators: $BUILD_SUCCESS"
echo ""

# Show actual errors if any
if [ "$CS_ERRORS" -gt 0 ] || [ "$NULL_REF" -gt 0 ] || [ "$FATAL_ERRORS" -gt 0 ]; then
    echo "=== COMPILATION ERRORS FOUND ==="
    echo ""
    grep -E "error CS|NullReferenceException|ArgumentException|Fatal" "$LOG_FILE" | grep -v "Licensing\|IPC\|signature\|Socket\|HubIPC" | head -20
    echo ""
    echo "=== CHECKPOINT STATUS: FAILED ==="
    echo "Fix the above errors before continuing."
    exit 1
else
    echo "=== COMPILATION SUCCESS ==="
    echo "No errors detected."
    echo ""
    echo "=== CHECKPOINT STATUS: PASSED ==="
    exit 0
fi
