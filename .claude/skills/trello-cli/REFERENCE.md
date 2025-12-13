# Trello CLI - Complete Reference

## Output Format

All commands return JSON:

```json
// Success
{"ok": true, "data": [...]}

// Error
{"ok": false, "error": "Error message", "code": "ERROR_CODE"}
```

### Error Codes

| Code | Meaning |
|------|---------|
| `AUTH_ERROR` | Not authenticated or invalid credentials |
| `NOT_FOUND` | Board/List/Card not found |
| `MISSING_PARAM` | Required parameter missing |
| `HTTP_ERROR` | Network or API error |
| `ERROR` | General error |

---

## Commands

### Authentication

```bash
# Check if authenticated
trello-cli --check-auth
# Returns: {"ok":true,"data":{"id":"...","username":"...","fullName":"..."}}

# Set credentials (one-time)
trello-cli --set-auth <api-key> <token>

# Clear saved credentials
trello-cli --clear-auth
```

### Board Operations

```bash
# List all boards
trello-cli --get-boards
# Returns: {"ok":true,"data":[{"id":"...","name":"Board Name","url":"..."}]}

# Get specific board
trello-cli --get-board <board-id>
# Returns: {"ok":true,"data":{"id":"...","name":"...","desc":"...","url":"..."}}
```

### List Operations

```bash
# Get all lists in a board
trello-cli --get-lists <board-id>
# Returns: {"ok":true,"data":[{"id":"...","name":"To Do"},{"id":"...","name":"Done"}]}

# Create new list
trello-cli --create-list <board-id> "<list-name>"
# Returns: {"ok":true,"data":{"id":"...","name":"..."}}
```

### Card Operations

#### Reading Cards

```bash
# Get cards in a specific list
trello-cli --get-cards <list-id>

# Get ALL cards in a board (recommended)
trello-cli --get-all-cards <board-id>

# Get single card with full details
trello-cli --get-card <card-id>
# Returns: {"ok":true,"data":{"id":"...","name":"...","desc":"...","due":"...","idList":"..."}}
```

#### Creating Cards

```bash
# Simple card
trello-cli --create-card <list-id> "<card-name>"

# Card with description
trello-cli --create-card <list-id> "<card-name>" --desc "<description>"

# Card with due date (ISO format)
trello-cli --create-card <list-id> "<card-name>" --due "2025-01-15"

# Full card
trello-cli --create-card <list-id> "<card-name>" --desc "<description>" --due "2025-01-15"
```

#### Updating Cards

```bash
# Update name only
trello-cli --update-card <card-id> --name "<new-name>"

# Update description only
trello-cli --update-card <card-id> --desc "<new-description>"

# Update due date only
trello-cli --update-card <card-id> --due "2025-01-20"

# Update multiple fields at once
trello-cli --update-card <card-id> --name "<name>" --desc "<desc>" --due "<date>"

# Clear due date
trello-cli --update-card <card-id> --due ""
```

#### Moving Cards

```bash
# Move card to another list
trello-cli --move-card <card-id> <target-list-id>
```

#### Deleting Cards

```bash
# Delete card permanently
trello-cli --delete-card <card-id>
# Returns: {"ok":true,"data":true}
```

---

## Common Workflows

### 1. First Time Setup

```bash
# Get API key from: https://trello.com/app-key
# Get Token from the same page (click "Token" link)
trello-cli --set-auth <your-api-key> <your-token>
trello-cli --check-auth  # Verify it works
```

### 2. Explore Board Structure

```bash
trello-cli --get-boards                    # Find your board
trello-cli --get-lists <board-id>          # See all lists
trello-cli --get-all-cards <board-id>      # See all cards
```

### 3. Create Task with Full Details

```bash
# Step 1: Find the target list
trello-cli --get-lists <board-id>

# Step 2: Create the card
trello-cli --create-card <list-id> "Implement login feature" \
  --desc "Add OAuth2 authentication with Google and GitHub providers" \
  --due "2025-01-20"
```

### 4. Move Card Through Workflow

```bash
# Get list IDs
trello-cli --get-lists <board-id>
# Example output shows: To Do (id1), In Progress (id2), Done (id3)

# Move from To Do to In Progress
trello-cli --move-card <card-id> <in-progress-list-id>

# Later, move to Done
trello-cli --move-card <card-id> <done-list-id>
```

### 5. Update Existing Card

```bash
# Get card details first
trello-cli --get-card <card-id>

# Update what you need
trello-cli --update-card <card-id> --desc "Updated requirements: ..."
```

---

## Tips

1. **Use `--get-all-cards`** instead of multiple `--get-cards` calls
2. **Cache board and list IDs** - they don't change often
3. **ISO date format** for due dates: `YYYY-MM-DD`
4. **Quote strings with spaces** in card names and descriptions
5. **Check `ok` field** in response before processing data
