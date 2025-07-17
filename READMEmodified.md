# Workflow Engine - Configurable State Machine API

A minimal backend service that implements a configurable workflow engine (state-machine API) built with **ASP.NET Core 8.0**. This service allows clients to define workflows as configurable state machines, start workflow instances, execute state transitions, and inspect running instances.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- Any modern web browser for API testing via Swagger UI

### Running the Application

1. **Clone and navigate to the project**:
   ```bash
   git clone <repository-url>
   cd WorkflowEngine
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access the API**:
   - **Swagger UI**: [http://localhost:5172](http://localhost:5172) (Development)
   - **API Base URL**: `http://localhost:5172/api`
   - **Health Check**: `http://localhost:5172/health`

The application will automatically create a `data/` directory for JSON file persistence.

## 📋 Features

### Core Functionality
- ✅ **Define Workflows**: Create configurable state machines with states and actions
- ✅ **Start Instances**: Launch new workflow instances from definitions
- ✅ **Execute Actions**: Move instances between states with full validation
- ✅ **Inspect States**: View current state, history, and available actions
- ✅ **Validation Rules**: Comprehensive validation for definitions and actions
- ✅ **Persistence**: In-memory storage with JSON file backup

### Validation Rules
- **State Rules**: Exactly one initial state required, unique state IDs
- **Action Rules**: Valid state references, no transitions from final states
- **Instance Rules**: Actions validated against current state and definition
- **Final State Protection**: No actions can be executed on final states

## 🔧 API Endpoints

### Workflow Definitions
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/workflowdefinitions` | Create a new workflow definition |
| `GET` | `/api/workflowdefinitions` | Get all workflow definitions |
| `GET` | `/api/workflowdefinitions/{id}` | Get a specific workflow definition |

### Workflow Instances
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/workflowinstances` | Start a new workflow instance |
| `GET` | `/api/workflowinstances` | Get all workflow instances |
| `GET` | `/api/workflowinstances/{id}` | Get a specific workflow instance |
| `GET` | `/api/workflowinstances/by-definition/{definitionId}` | Get instances by definition |
| `POST` | `/api/workflowinstances/{id}/execute` | Execute an action on an instance |
| `GET` | `/api/workflowinstances/{id}/available-actions` | Get available actions for an instance |

## 📖 Example Usage

### 1. Create a Simple Approval Workflow

```json
POST /api/workflowdefinitions
{
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
}
```

### 2. Start a Workflow Instance

```json
POST /api/workflowinstances
{
  "definitionId": "{workflow-definition-id}",
  "metadata": {
    "documentTitle": "Project Proposal",
    "author": "John Doe"
  }
}
```

### 3. Execute an Action

```json
POST /api/workflowinstances/{instance-id}/execute
{
  "actionId": "submit",
  "notes": "Ready for review"
}
```

## 🏗️ Architecture

### Core Components

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Controllers   │───▶│  Workflow Engine │───▶│     Storage     │
│                 │    │     Service      │    │                 │
│ - Definitions   │    │                  │    │ - InMemory +    │
│ - Instances     │    │ - Validation     │    │   JSON Files    │
│ - Actions       │    │ - State Logic    │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Key Classes

- **Models**: `State`, `Action`, `WorkflowDefinition`, `WorkflowInstance`
- **Services**: `IWorkflowEngine`, `WorkflowEngineService`, `IWorkflowStorage`
- **DTOs**: Request/response objects for API operations
- **Exceptions**: Custom exceptions for workflow-specific errors

### Data Structure

**WorkflowDefinition**:
- Collection of States (exactly one `IsInitial = true`)
- Collection of Actions (transitions between states)
- Metadata and timestamps

**WorkflowInstance**:
- Reference to definition
- Current state
- Execution history with timestamps
- Optional metadata

## 🗂️ Project Structure

```
WorkflowEngine/
├── Controllers/
│   ├── WorkflowDefinitionsController.cs
│   └── WorkflowInstancesController.cs
├── Models/
│   ├── State.cs
│   ├── Action.cs
│   ├── WorkflowDefinition.cs
│   ├── WorkflowInstance.cs
│   └── DTOs/
│       ├── CreateWorkflowDefinitionDto.cs
│       └── ExecuteActionDto.cs
├── Services/
│   ├── IWorkflowEngine.cs
│   ├── WorkflowEngineService.cs
│   ├── IWorkflowStorage.cs
│   └── InMemoryWorkflowStorage.cs
├── Exceptions/
│   └── WorkflowException.cs
├── Program.cs
├── appsettings.json
└── README.md
```

## 🧪 Testing the API

The easiest way to test the API is through the **Swagger UI** available at the root URL when running in development mode.

### Sample Test Flow:
1. **Create a workflow definition** using the example above
2. **Start an instance** with the returned definition ID
3. **Check available actions** for the new instance
4. **Execute actions** to move through the workflow
5. **Inspect the instance** to see current state and history

## ⚙️ Configuration

### Storage Configuration
Modify `appsettings.json` to change the data directory:

```json
{
  "Storage": {
    "DataDirectory": "custom-data-path"
  }
}
```

### Environment Variables
- `DOTNET_ENVIRONMENT`: Set to `Development` for Swagger UI
- `ASPNETCORE_URLS`: Override default URLs (e.g., `http://localhost:8080`)

## 🔒 Error Handling

The API provides helpful error messages for common scenarios:

- **400 Bad Request**: Invalid workflow definitions, invalid actions
- **404 Not Found**: Workflow definition or instance not found
- **500 Internal Server Error**: Unexpected errors with details

Example error response:
```json
{
  "error": "Action 'invalid-action' cannot be executed from state 'draft'."
}
```

## 🎯 Design Decisions

### **Simplicity Over Complexity**
- In-memory storage with JSON persistence for easy setup
- Minimal dependencies (only built-in ASP.NET Core packages)
- Clear, readable code structure

### **Extensibility**
- Interface-based design allows easy replacement of storage layer
- Separation of concerns between controllers, services, and models
- Comprehensive validation framework

### **Production Readiness**
- Proper error handling and validation
- Swagger documentation for easy API exploration
- Health check endpoint for monitoring

## 🚧 Future Enhancements

**Potential improvements that could be added with more time:**

- **Database Integration**: Replace in-memory storage with Entity Framework
- **Authentication**: Add JWT/OAuth support for multi-user scenarios
- **Workflow Versioning**: Support multiple versions of workflow definitions
- **Conditional Actions**: Add support for conditional transitions
- **Parallel States**: Support for parallel execution paths
- **Time-based Triggers**: Actions that execute automatically after delays
- **Audit Logging**: Enhanced logging and audit trails
- **Performance Optimization**: Caching and bulk operations

---

**Assignment completed for**: Infonetica Software Engineer Intern Position  
**Expected effort**: ≤ 2 hours  
**Technology stack**: .NET 8.0, ASP.NET Core, C#
