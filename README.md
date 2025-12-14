# Trello CLI

A CLI tool that provides Trello integration with Claude Code. With this tool, you can manage your Trello boards, lists, and cards using natural language through Claude Code.

## What is it?

`trello-cli` is a command-line tool that communicates with the Trello API. Claude Code uses this tool to:

- List your boards
- View, create, and update your cards
- Move cards between lists
- Track your tasks

## Installation

### Build from Source

```bash
# Clone the repository
git clone https://github.com/ZenoxZX/trello-cli.git
cd trello-cli

# Install as global tool
dotnet pack src/TrelloCli.csproj -c Release
dotnet tool install --global --add-source src/nupkg TrelloCli

# Verify installation
trello-cli --help
```

### Uninstalling

```bash
dotnet tool uninstall --global TrelloCli
```

### Setting Up Trello API Credentials

1. Get your API key and token from https://trello.com/app-key
2. Click the "Token" link on that page to generate a token
3. Configure the CLI:

```bash
# Set your credentials
trello-cli --set-auth <api-key> <token>

# Verify authentication
trello-cli --check-auth
```

## Usage

Simply mention "Trello" when talking to Claude Code:

```
"Show my Trello tasks"
"Add a new card to Trello: Login page design"
"Move this card to Done on Trello"
"List my Trello boards"
```

## Documentation

| File | Description |
|------|-------------|
| [docs/instruction.md](docs/instruction.md) | Detailed command reference and usage examples for AI |
| [docs/system-prompt.md](docs/system-prompt.md) | System prompt for AI integration |
| [.claude/skills/trello-cli/SKILL.md](.claude/skills/trello-cli/SKILL.md) | Claude Code skill definition and quick reference |
| [.claude/skills/trello-cli/REFERENCE.md](.claude/skills/trello-cli/REFERENCE.md) | Complete documentation of all commands |

## Claude Code Skill System

This repo uses Claude Code's **skill** system. Skills are configuration files that give Claude Code specialized capabilities.

### What is a Skill?

A skill is a markdown file that defines how Claude Code should use specific tools or APIs. They are located in the `.claude/skills/` directory.

### Adding the Skill to Your Personal Directory

To use this skill everywhere on your system, copy it to your personal `.claude` directory:

```bash
# Copy the skill folder to your personal directory
cp -r .claude/skills/trello-cli ~/.claude/skills/
```

After this, Claude Code will automatically activate this skill whenever you mention "Trello" in any directory.

### Skill Structure

```
~/.claude/
└── skills/
    └── trello-cli/
        ├── SKILL.md       # Main skill definition (trigger rules, quick reference)
        └── REFERENCE.md   # Detailed command documentation
```

### SKILL.md Anatomy

```markdown
---
name: trello-cli
description: Trello board, list and card management via CLI...
---

# Skill Content
...
```

- **name**: Unique name of the skill
- **description**: Description that determines when it activates (contains trigger words)

## Command Summary

```bash
# Board operations
trello-cli --get-boards
trello-cli --get-board <board-id>

# List operations
trello-cli --get-lists <board-id>
trello-cli --create-list <board-id> "<name>"

# Card operations
trello-cli --get-all-cards <board-id>
trello-cli --create-card <list-id> "<name>" [--desc "<desc>"] [--due "YYYY-MM-DD"]
trello-cli --update-card <card-id> [--name "<name>"] [--desc "<desc>"] [--due "<date>"]
trello-cli --move-card <card-id> <target-list-id>
trello-cli --delete-card <card-id>
```

## Requirements

- .NET 6.0 or later
- Trello account with API access

## License

This project is licensed under the [MIT License](LICENSE).
