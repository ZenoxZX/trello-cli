# Trello CLI Tool

You can manage Trello via `trello-cli`. All outputs are JSON: `{"ok":true,"data":...}` or `{"ok":false,"error":"..."}`.

## Commands

| Command | Usage |
|---------|-------|
| `--get-boards` | List all boards |
| `--get-board <id>` | Get board details |
| `--get-lists <board-id>` | Get lists in board |
| `--create-list <board-id> "<name>"` | Create list |
| `--get-cards <list-id>` | Get cards in list |
| `--get-all-cards <board-id>` | Get all cards in board |
| `--get-card <card-id>` | Get card details |
| `--create-card <list-id> "<name>" [--desc "..."] [--due "YYYY-MM-DD"]` | Create card |
| `--update-card <card-id> [--name "..."] [--desc "..."] [--due "..."]` | Update card |
| `--move-card <card-id> <list-id>` | Move card to list |
| `--delete-card <card-id>` | Delete card |
| `--check-auth` | Verify auth |

## Workflow

1. `--get-boards` → find board ID
2. `--get-lists <board-id>` → find list IDs (To Do, In Progress, Done, etc.)
3. Use card commands with the IDs

## Examples

```bash
trello-cli --get-boards
trello-cli --get-all-cards 6abc123
trello-cli --create-card 6list789 "New task" --desc "Details" --due "2025-01-20"
trello-cli --move-card 6card456 6donelist123
```
