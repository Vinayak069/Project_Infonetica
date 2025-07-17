using System.Collections.Concurrent;
using System.Text.Json;
using WorkflowEngine.Models;

namespace WorkflowEngine.Services;

/// <summary>
/// In-memory implementation of workflow storage with optional JSON file persistence
/// </summary>
public class InMemoryWorkflowStorage : IWorkflowStorage
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();
    private readonly string _dataDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public InMemoryWorkflowStorage(IConfiguration configuration)
    {
        _dataDirectory = configuration.GetValue<string>("Storage:DataDirectory") ?? "data";
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Ensure data directory exists
        Directory.CreateDirectory(_dataDirectory);
        
        // Load existing data
        _ = Task.Run(LoadDataAsync);
    }

    // Workflow Definition Storage
    public async Task SaveWorkflowDefinitionAsync(WorkflowDefinition definition)
    {
        _definitions[definition.Id] = definition;
        await SaveDefinitionsToFileAsync();
    }

    public Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId)
    {
        _definitions.TryGetValue(definitionId, out var definition);
        return Task.FromResult(definition);
    }

    public Task<IEnumerable<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync()
    {
        return Task.FromResult(_definitions.Values.AsEnumerable());
    }

    // Workflow Instance Storage
    public async Task SaveWorkflowInstanceAsync(WorkflowInstance instance)
    {
        _instances[instance.Id] = instance;
        await SaveInstancesToFileAsync();
    }

    public Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId)
    {
        _instances.TryGetValue(instanceId, out var instance);
        return Task.FromResult(instance);
    }

    public Task<IEnumerable<WorkflowInstance>> GetAllWorkflowInstancesAsync()
    {
        return Task.FromResult(_instances.Values.AsEnumerable());
    }

    public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesByDefinitionAsync(string definitionId)
    {
        var instances = _instances.Values.Where(i => i.DefinitionId == definitionId);
        return Task.FromResult(instances);
    }

    // File persistence methods
    private async Task LoadDataAsync()
    {
        await LoadDefinitionsFromFileAsync();
        await LoadInstancesFromFileAsync();
    }

    private async Task LoadDefinitionsFromFileAsync()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, "workflow-definitions.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var definitions = JsonSerializer.Deserialize<List<WorkflowDefinition>>(json, _jsonOptions);
                if (definitions != null)
                {
                    foreach (var definition in definitions)
                    {
                        _definitions[definition.Id] = definition;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the application
            Console.WriteLine($"Failed to load workflow definitions: {ex.Message}");
        }
    }

    private async Task LoadInstancesFromFileAsync()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, "workflow-instances.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var instances = JsonSerializer.Deserialize<List<WorkflowInstance>>(json, _jsonOptions);
                if (instances != null)
                {
                    foreach (var instance in instances)
                    {
                        _instances[instance.Id] = instance;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the application
            Console.WriteLine($"Failed to load workflow instances: {ex.Message}");
        }
    }

    private async Task SaveDefinitionsToFileAsync()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, "workflow-definitions.json");
            var json = JsonSerializer.Serialize(_definitions.Values.ToList(), _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            // Log error but continue
            Console.WriteLine($"Failed to save workflow definitions: {ex.Message}");
        }
    }

    private async Task SaveInstancesToFileAsync()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, "workflow-instances.json");
            var json = JsonSerializer.Serialize(_instances.Values.ToList(), _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            // Log error but continue
            Console.WriteLine($"Failed to save workflow instances: {ex.Message}");
        }
    }
}
