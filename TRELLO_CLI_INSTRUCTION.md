# Trello CLI - AI Instruction

You have access to a CLI tool called `trello-cli` that allows you to interact with Trello boards, lists, and cards. Use this tool to help users manage their Trello tasks.

## Output Format

All responses are compact JSON:
- Success: `{"ok":true,"data":...}`
- Error: `{"ok":false,"error":"...","code":"..."}`

## Available Commands

### Authentication

```bash
# Check if authenticated
trello-cli --check-auth
```

### Board Operations

```bash
# List all boards
trello-cli --get-boards

# Get specific board
trello-cli --get-board <board-id>
```

### List Operations

```bash
# Get all lists in a board
trello-cli --get-lists <board-id>

# Create a new list
trello-cli --create-list <board-id> "<list-name>"
```

### Card Operations

```bash
# Get cards in a specific list
trello-cli --get-cards <list-id>

# Get ALL cards in a board
trello-cli --get-all-cards <board-id>

# Get specific card details
trello-cli --get-card <card-id>

# Create a new card
trello-cli --create-card <list-id> "<card-name>"
trello-cli --create-card <list-id> "<card-name>" --desc "<description>"
trello-cli --create-card <list-id> "<card-name>" --desc "<description>" --due "2025-01-15"

# Update a card
trello-cli --update-card <card-id> --name "<new-name>"
trello-cli --update-card <card-id> --desc "<new-description>"
trello-cli --update-card <card-id> --due "2025-01-15"
trello-cli --update-card <card-id> --name "<name>" --desc "<desc>" --due "<date>"

# Move card to another list
trello-cli --move-card <card-id> <target-list-id>

# Delete a card
trello-cli --delete-card <card-id>
```

## Response Examples

### Get Boards
```json
{"ok":true,"data":[{"id":"abc123","name":"My Project","url":"https://trello.com/b/abc123"}]}
```

### Get Lists
```json
{"ok":true,"data":[{"id":"list1","name":"To Do","boardId":"abc123"},{"id":"list2","name":"In Progress","boardId":"abc123"},{"id":"list3","name":"Done","boardId":"abc123"}]}
```

### Get Cards
```json
{"ok":true,"data":[{"id":"card1","name":"Fix bug","desc":"Fix login issue","listId":"list1","due":"2025-01-15"}]}
```

### Create Card
```json
{"ok":true,"data":{"id":"newcard123","name":"New Task","listId":"list1"}}
```

### Error Response
```json
{"ok":false,"error":"Card not found","code":"NOT_FOUND"}
```

## Error Codes

| Code | Meaning |
|------|---------|
| `AUTH_ERROR` | Not authenticated or invalid credentials |
| `NOT_FOUND` | Board/List/Card not found |
| `MISSING_PARAM` | Required parameter missing |
| `HTTP_ERROR` | Network or API error |
| `ERROR` | General error |

## Workflow Examples

### Example 1: List all tasks in a board

```bash
# Step 1: Get boards to find the board ID
trello-cli --get-boards

# Step 2: Get all cards in that board
trello-cli --get-all-cards <board-id>
```

### Example 2: Create a task in "To Do" list

```bash
# Step 1: Get lists to find "To Do" list ID
trello-cli --get-lists <board-id>

# Step 2: Create card in that list
trello-cli --create-card <todo-list-id> "Implement feature X" --desc "Details here"
```

### Example 3: Move task from "To Do" to "Done"

```bash
# Step 1: Get lists to find target list ID
trello-cli --get-lists <board-id>

# Step 2: Move the card
trello-cli --move-card <card-id> <done-list-id>
```

### Example 4: Update task with due date

```bash
trello-cli --update-card <card-id> --due "2025-01-20" --desc "Updated description"
```

## Best Practices

1. **Always check `ok` field** in response before processing data
2. **Cache board and list IDs** when possible to reduce API calls
3. **Use `--get-all-cards`** instead of multiple `--get-cards` calls when you need all cards
4. **Quote strings with spaces** in card names and descriptions
5. **Use ISO date format** (YYYY-MM-DD) for due dates

## Common Patterns

### Find a card by name
```bash
# Get all cards and filter by name in the response
trello-cli --get-all-cards <board-id>
# Then search for the card name in the JSON response
```

### Get board overview
```bash
# Get board info
trello-cli --get-board <board-id>

# Get all lists
trello-cli --get-lists <board-id>

# Get all cards
trello-cli --get-all-cards <board-id>
```

### Create complete task
```bash
trello-cli --create-card <list-id> "Task Name" --desc "Task description with details" --due "2025-02-01"
```
