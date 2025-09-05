using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace SemanticKernelPlugins
{
    public class TodoPlugin
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5125") // Replace with your API base
        };

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        // 1. Get All Lists
        [KernelFunction("get_all_lists")]
        [Description("Get all todo lists from the API.")]
        public static async Task<string> GetAllListsAsync()
        {
            var response = await _httpClient.GetAsync("/lists");
            var content = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{content}";
        }

        // 2. Get List by ID
        [KernelFunction("get_list_by_id")]
        [Description("Get a specific list by its ID.")]
        public static async Task<string> GetListByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/lists/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{content}";
        }

        // 3. Get All Tasks
        [KernelFunction("get_all_tasks")]
        [Description("Get all tasks from the API.")]
        public static async Task<string> GetAllTasksAsync()
        {
            var response = await _httpClient.GetAsync("/tasks");
            var content = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{content}";
        }

        // 4. Get Task by ID
        [KernelFunction("get_task_by_id")]
        [Description("Get a specific task by its ID.")]
        public static async Task<string> GetTaskByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/tasks/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{content}";
        }

        // 5. Create a New List
        [KernelFunction("create_list")]
        [Description("Create a new list with name and description.")]
        public static async Task<string> CreateListAsync(string name, string description)
        {
            var list = new { name, description };
            var json = JsonSerializer.Serialize(list, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/lists", content);
            var result = await response.Content.ReadAsStringAsync();

            return $"Status: {response.StatusCode}\n\n{result}";
        }

        // 6. Create a New Task
        [KernelFunction("create_task")]
        [Description("Create a new task with title, description, dueDate, isComplete, and todoListId.")]
        public static async Task<string> CreateTaskAsync(string title, string description, string dueDate, string isComplete, string todoListId)
        {
            var task = new
            {
                title,
                description,
                dueDate,
                isComplete = bool.Parse(isComplete),
                todoListId = int.Parse(todoListId)
            };

            var json = JsonSerializer.Serialize(task, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/tasks", content);
            var result = await response.Content.ReadAsStringAsync();

            return $"Status: {response.StatusCode}\n\n{result}";
        }

        // 7. Update a List
        [KernelFunction("update_list")]
        [Description("Update a list by ID with optional fields.")]
        public static async Task<string> UpdateListAsync(string id, string? name = null, string? description = null)
        {
            var getResponse = await _httpClient.GetAsync($"/lists/{id}");
            if (!getResponse.IsSuccessStatusCode)
                return $"Failed to get list {id}: {getResponse.StatusCode}";

            var existingJson = await getResponse.Content.ReadAsStringAsync();
            var existingList = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson);

            if (existingList == null) return "Failed to deserialize list data.";

            if (name != null) existingList["name"] = name;
            if (description != null) existingList["description"] = description;

            var updatedJson = JsonSerializer.Serialize(existingList, _jsonOptions);
            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"/lists/{id}", content);
            var result = await putResponse.Content.ReadAsStringAsync();

            return $"Status: {putResponse.StatusCode}\n\n{result}";
        }

        // 8. Update a Task
        [KernelFunction("update_task")]
        [Description("Update a task by ID with optional fields.")]
        public static async Task<string> UpdateTaskAsync(string id, string? title = null, string? description = null, string? dueDate = null, string? isComplete = null, string? todoListId = null)
        {
            var getResponse = await _httpClient.GetAsync($"/tasks/{id}");
            if (!getResponse.IsSuccessStatusCode)
                return $"Failed to get task {id}: {getResponse.StatusCode}";

            var existingJson = await getResponse.Content.ReadAsStringAsync();
            var existingTask = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson);

            if (existingTask == null) return "Failed to deserialize task data.";

            if (title != null) existingTask["title"] = title;
            if (description != null) existingTask["description"] = description;
            if (dueDate != null) existingTask["dueDate"] = dueDate;
            if (isComplete != null) existingTask["isComplete"] = bool.Parse(isComplete);
            if (todoListId != null) existingTask["todoListId"] = int.Parse(todoListId);

            var updatedJson = JsonSerializer.Serialize(existingTask, _jsonOptions);
            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"/tasks/{id}", content);
            var result = await putResponse.Content.ReadAsStringAsync();

            return $"Status: {putResponse.StatusCode}\n\n{result}";
        }

        // 9. Delete a Task
        [KernelFunction("delete_task")]
        [Description("Delete a task by ID.")]
        public static async Task<string> DeleteTaskAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/tasks/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{result}";
        }

        // 10. Delete a List
        [KernelFunction("delete_list")]
        [Description("Delete a list by ID.")]
        public static async Task<string> DeleteListAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/lists/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode}\n\n{result}";
        }
    }
}