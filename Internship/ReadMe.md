# 🧩 Semantic Kernel Plugins: Text & API

This project provides **two Semantic Kernel plugins**:  
- 📋 **Text Plugin**: Utilities for analyzing and transforming text.  
- 🌐 **API Plugin**: Connects to a REST API for managing todo lists and tasks.  

Together, these plugins extend Semantic Kernel (SK) with both **local text processing** and **remote API integration**.

---

## 🚀 Features

### 📋 Text Plugin
- `analyze_text` → Analyze text and provide statistics (word count, character count, sentiment).
- `extract_keywords` → Extract keywords by removing stop words and showing frequency.
- `transform_case` → Transform text case (upper, lower, title, sentence, alternating).
- `count_occurrences` → Count how many times a word/phrase appears.
- `extract_emails` → Extract email addresses from text.
- `reverse_text` → Reverse the order of characters in text.

### 🌐 API Plugin
- `get_all_lists` → Get all todo lists.
- `get_list_by_id` → Get a specific list by ID.
- `get_all_tasks` → Get all tasks.
- `get_task_by_id` → Get a specific task by ID.
- `create_list` → Create a new list with name and description.
- `create_task` → Create a new task (title, description, dueDate, isComplete, todoListId).
- `update_list` → Update a list by ID with optional fields.
- `update_task` → Update a task by ID with optional fields.
- `delete_task` → Delete a task by ID.
- `delete_list` → Delete a list by ID.

---

## 🛠️ Requirements

- [.NET 8.0+](https://dotnet.microsoft.com/)
- [Microsoft.SemanticKernel](https://github.com/microsoft/semantic-kernel) NuGet package
- A running API backend at `http://localhost:5125`  
  (change `BaseAddress` in `TodoPlugin.cs` if different)

---

## 📂 Project Structure

