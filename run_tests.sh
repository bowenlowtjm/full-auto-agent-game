#!/bin/bash
# Unity Test Runner - Runs EditMode/PlayMode tests from CLI

UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.4f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="/Users/bowenlow/Documents/agent-game-explore/full-auto-agent-game"

if [ "$1" == "editmode" ]; then
    echo "Running EditMode tests..."
    PLATFORM="EditMode"
elif [ "$1" == "playmode" ]; then
    echo "Running PlayMode tests..."
    PLATFORM="PlayMode"
else
    echo "Usage: $0 [editmode|playmode]"
    exit 1
fi

echo "Starting Unity test runner..."
"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -testResults /tmp/unity_test_results.xml \
    -runTests \
    -testPlatform "$PLATFORM" \
    -testCategory !Disabled 2>&1 | tee /tmp/unity_test_log.txt

EXIT_CODE=$?

echo ""
echo "=== TEST RESULTS ==="
if [ -f /tmp/unity_test_results.xml ]; then
    # Parse test results
    grep -o 'result="[^"]*"' /tmp/unity_test_results.xml | head -5
    grep -o 'total="[^"]*"' /tmp/unity_test_results.xml | head -1
    grep -o 'passed="[^"]*"' /tmp/unity_test_results.xml | head -1
    grep -o 'failed="[^"]*"' /tmp/unity_test_results.xml | head -1
else
    echo "No test results file generated"
fi

echo ""
echo "Exit code: $EXIT_CODE"
exit $EXIT_CODE
