#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}"
echo "╔═══════════════════════════════════════════════════════════╗"
echo "║          System Rezerwacji - Instalator                   ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo -e "${NC}"

# Configuration
REPO_URL="https://github.com/przydan/SystemRezerwacji.git"
INSTALL_DIR="${INSTALL_DIR:-$HOME/SystemRezerwacji}"
SEED_TEST_DATA="${SEED_TEST_DATA:-true}"

# Check for Docker
check_docker() {
    echo -e "${YELLOW}[1/4] Sprawdzanie wymagań...${NC}"
    
    if ! command -v docker &> /dev/null; then
        echo -e "${RED}✗ Docker nie jest zainstalowany!${NC}"
        echo "  Zainstaluj Docker: https://docs.docker.com/get-docker/"
        exit 1
    fi
    echo -e "${GREEN}✓ Docker znaleziony${NC}"

    if ! docker compose version &> /dev/null && ! docker-compose version &> /dev/null; then
        echo -e "${RED}✗ Docker Compose nie jest zainstalowany!${NC}"
        exit 1
    fi
    echo -e "${GREEN}✓ Docker Compose znaleziony${NC}"
    
    # Check if docker daemon is running
    if ! docker info &> /dev/null; then
        echo -e "${RED}✗ Docker daemon nie działa!${NC}"
        echo "  Uruchom Docker i spróbuj ponownie."
        exit 1
    fi
    echo -e "${GREEN}✓ Docker daemon działa${NC}"
}

# Clone or update repository
clone_repo() {
    echo -e "${YELLOW}[2/4] Pobieranie aplikacji...${NC}"
    
    if [ -d "$INSTALL_DIR" ]; then
        echo "  Katalog $INSTALL_DIR już istnieje."
        if [ -t 0 ]; then
            # Interactive mode
            read -p "  Czy chcesz go nadpisać? (t/n): " -n 1 -r
            echo
            if [[ $REPLY =~ ^[Tt]$ ]]; then
                rm -rf "$INSTALL_DIR"
            else
                echo "  Używam istniejącej instalacji..."
                return
            fi
        else
            # Non-interactive mode - use existing
            echo "  Tryb nieinteraktywny - używam istniejącej instalacji."
            return
        fi
    fi
    
    git clone --depth 1 "$REPO_URL" "$INSTALL_DIR"
    echo -e "${GREEN}✓ Pobrano do $INSTALL_DIR${NC}"
}

# Configure environment
configure() {
    echo -e "${YELLOW}[3/4] Konfiguracja...${NC}"
    
    cd "$INSTALL_DIR"
    
    # Check if running interactively
    if [ -t 0 ]; then
        # Interactive mode - ask user
        read -p "  Czy załadować dane testowe? (t/n) [t]: " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Nn]$ ]]; then
            SEED_TEST_DATA="false"
        else
            SEED_TEST_DATA="true"
        fi
    else
        # Non-interactive mode (curl | bash) - use default or env var
        echo "  Tryb nieinteraktywny - używam domyślnych ustawień."
        SEED_TEST_DATA="${SEED_TEST_DATA:-true}"
    fi
    
    echo -e "${GREEN}✓ Dane testowe: $SEED_TEST_DATA${NC}"
}

# Start application
start_app() {
    echo -e "${YELLOW}[4/4] Uruchamianie aplikacji...${NC}"
    
    cd "$INSTALL_DIR"
    
    export SEED_TEST_DATA
    
    # Determine docker compose command
    if docker compose version &> /dev/null; then
        COMPOSE_CMD="docker compose"
    else
        COMPOSE_CMD="docker-compose"
    fi
    
    echo "  Budowanie obrazów (może potrwać kilka minut)..."
    $COMPOSE_CMD build --quiet
    
    echo "  Uruchamianie kontenerów..."
    $COMPOSE_CMD up -d
    
    echo ""
    echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
    echo -e "${GREEN}║              Instalacja zakończona!                       ║${NC}"
    echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  Aplikacja dostępna pod adresem: ${GREEN}http://localhost:8080${NC}"
    echo ""
    echo "  Dane logowania administratora:"
    echo -e "    Email:    ${YELLOW}admin@x.pl${NC}"
    echo -e "    Hasło:    ${YELLOW}Pass1234!\@#\$${NC}"
    echo ""
    echo "  Przydatne komendy:"
    echo "    Logi:      cd $INSTALL_DIR && docker compose logs -f"
    echo "    Stop:      cd $INSTALL_DIR && docker compose down"
    echo "    Restart:   cd $INSTALL_DIR && docker compose restart"
    echo "    Usuń dane: cd $INSTALL_DIR && docker compose down -v"
    echo ""
}

# Main
main() {
    check_docker
    clone_repo
    configure
    start_app
}

main "$@"
