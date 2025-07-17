#!/bin/bash

# Demo script for Workflow Engine API
# This script demonstrates creating a workflow definition, starting an instance, and executing actions

BASE_URL="http://localhost:5172/api"
echo "=== Workflow Engine API Demo ==="
echo "Base URL: $BASE_URL"
echo ""

# Test health endpoint
echo "1. Testing health endpoint..."
curl -s http://localhost:5172/health | jq '.'
echo ""

# Create a simple workflow definition
echo "2. Creating a workflow definition..."
DEFINITION_RESPONSE=$(curl -s -X POST "$BASE_URL/workflowdefinitions" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Document Approval",
    "description": "Simple document approval workflow",
    "states": [
      {
        "id": "draft",
        "name": "Draft",
        "isInitial": true,
        "description": "Document is being drafted"
      },
      {
        "id": "review",
        "name": "Under Review",
        "description": "Document is under review"
      },
      {
        "id": "approved",
        "name": "Approved",
        "isFinal": true,
        "description": "Document has been approved"
      },
      {
        "id": "rejected",
        "name": "Rejected",
        "isFinal": true,
        "description": "Document has been rejected"
      }
    ],
    "actions": [
      {
        "id": "submit",
        "name": "Submit for Review",
        "fromStates": ["draft"],
        "toState": "review"
      },
      {
        "id": "approve",
        "name": "Approve",
        "fromStates": ["review"],
        "toState": "approved"
      },
      {
        "id": "reject",
        "name": "Reject",
        "fromStates": ["review"],
        "toState": "rejected"
      },
      {
        "id": "revise",
        "name": "Send Back for Revision",
        "fromStates": ["review"],
        "toState": "draft"
      }
    ]
  }')

echo "$DEFINITION_RESPONSE" | jq '.'
DEFINITION_ID=$(echo "$DEFINITION_RESPONSE" | jq -r '.id')
echo "Created workflow definition with ID: $DEFINITION_ID"
echo ""

# Start a workflow instance
echo "3. Starting a workflow instance..."
INSTANCE_RESPONSE=$(curl -s -X POST "$BASE_URL/workflowinstances" \
  -H "Content-Type: application/json" \
  -d "{
    \"definitionId\": \"$DEFINITION_ID\",
    \"metadata\": {
      \"documentTitle\": \"Project Proposal\",
      \"author\": \"John Doe\"
    }
  }")

echo "$INSTANCE_RESPONSE" | jq '.'
INSTANCE_ID=$(echo "$INSTANCE_RESPONSE" | jq -r '.id')
echo "Created workflow instance with ID: $INSTANCE_ID"
echo ""

# Get available actions
echo "4. Getting available actions for the instance..."
curl -s "$BASE_URL/workflowinstances/$INSTANCE_ID/available-actions" | jq '.'
echo ""

# Execute an action
echo "5. Executing 'submit' action..."
EXECUTE_RESPONSE=$(curl -s -X POST "$BASE_URL/workflowinstances/$INSTANCE_ID/execute" \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "submit",
    "notes": "Ready for review"
  }')

echo "$EXECUTE_RESPONSE" | jq '.'
echo ""

# Get available actions after transition
echo "6. Getting available actions after transition..."
curl -s "$BASE_URL/workflowinstances/$INSTANCE_ID/available-actions" | jq '.'
echo ""

# Execute approve action
echo "7. Executing 'approve' action..."
EXECUTE_RESPONSE=$(curl -s -X POST "$BASE_URL/workflowinstances/$INSTANCE_ID/execute" \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "approve",
    "notes": "Document approved after review"
  }')

echo "$EXECUTE_RESPONSE" | jq '.'
echo ""

# Try to execute action on final state (should fail)
echo "8. Trying to execute action on final state (should fail)..."
ERROR_RESPONSE=$(curl -s -X POST "$BASE_URL/workflowinstances/$INSTANCE_ID/execute" \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "submit",
    "notes": "This should fail"
  }')

echo "$ERROR_RESPONSE" | jq '.'
echo ""

# Get final instance state
echo "9. Getting final instance state..."
curl -s "$BASE_URL/workflowinstances/$INSTANCE_ID" | jq '.'
echo ""

echo "=== Demo Complete ==="
echo "The workflow engine successfully:"
echo "- Created a workflow definition with states and actions"
echo "- Started an instance in the initial state"
echo "- Executed actions to transition between states"
echo "- Validated state transitions and prevented invalid actions"
echo "- Maintained complete history of all transitions"
