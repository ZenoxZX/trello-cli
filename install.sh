#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

echo_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

echo_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Get the directory where the script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo_info "Installing trello-cli from $SCRIPT_DIR"

# Check for homebrew
if ! command -v brew &> /dev/null; then
    echo_error "Homebrew is required but not installed."
    echo_error "Install it from https://brew.sh"
    exit 1
fi

# Check for dotnet, install if needed
if ! command -v dotnet &> /dev/null; then
    echo_info "Installing .NET SDK via Homebrew..."
    brew install dotnet
else
    echo_info ".NET SDK already installed: $(dotnet --version)"
fi

# Verify dotnet is now available
if ! command -v dotnet &> /dev/null; then
    echo_error "Failed to install .NET SDK"
    exit 1
fi

# Check .NET version (need 10.0+)
DOTNET_VERSION=$(dotnet --version | cut -d'.' -f1)
if [ "$DOTNET_VERSION" -lt 10 ] 2>/dev/null; then
    echo_warn "This tool targets .NET 10.0. You have .NET $(dotnet --version)."
    echo_warn "Attempting to build anyway - it may work with newer SDK versions."
fi

# Uninstall existing version if present
if dotnet tool list --global | grep -q "trelloCli\|TrelloCli\|trello-cli"; then
    echo_info "Removing existing trello-cli installation..."
    dotnet tool uninstall --global TrelloCli 2>/dev/null || true
fi

# Build and pack
echo_info "Building trello-cli..."
cd "$SCRIPT_DIR"
dotnet pack src/TrelloCli.csproj -c Release

# Install as global tool
echo_info "Installing trello-cli as global .NET tool..."
dotnet tool install --global --add-source src/nupkg TrelloCli

# Verify installation
if ! command -v trello-cli &> /dev/null; then
    echo_warn "trello-cli installed but not in PATH."
    echo_warn "You may need to add ~/.dotnet/tools to your PATH:"
    echo_warn "  export PATH=\"\$PATH:\$HOME/.dotnet/tools\""
fi

# Install Claude Code skills
CLAUDE_SKILLS_DIR="$HOME/.claude/skills"
if [ -d "$SCRIPT_DIR/plugins/trello-cli/skills/trello-cli" ]; then
    echo_info "Installing Claude Code skills..."
    mkdir -p "$CLAUDE_SKILLS_DIR"
    cp -r "$SCRIPT_DIR/plugins/trello-cli/skills/trello-cli" "$CLAUDE_SKILLS_DIR/"
    echo_info "Skills installed to $CLAUDE_SKILLS_DIR/trello-cli"
fi

echo ""
echo_info "Installation complete!"
echo ""
echo "Next steps:"
echo "  1. Get your Trello API credentials from https://trello.com/app-key"
echo "  2. Configure authentication:"
echo "     trello-cli --set-auth <api-key> <token>"
echo "  3. Verify setup:"
echo "     trello-cli --check-auth"
echo ""
echo "If 'trello-cli' is not found, add this to your shell profile:"
echo "  export PATH=\"\$PATH:\$HOME/.dotnet/tools\""
