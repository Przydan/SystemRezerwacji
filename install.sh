#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${GREEN}"
echo "╔═══════════════════════════════════════════════════════════╗"
echo "║          System Rezerwacji - Instalator                   ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo -e "${NC}"

# Configuration (can be overridden via environment variables)
REPO_URL="https://github.com/przydan/SystemRezerwacji.git"
INSTALL_DIR="${INSTALL_DIR:-$HOME/SystemRezerwacji}"

# Default values
SEED_TEST_DATA="${SEED_TEST_DATA:-true}"
SA_PASSWORD="${SA_PASSWORD:-}"
ADMIN_EMAIL="${ADMIN_EMAIL:-}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-}"

# Password validation function
validate_password() {
    local password="$1"
    local min_length=12
    
    if [ ${#password} -lt $min_length ]; then
        echo -e "${RED}✗ Hasło musi mieć minimum $min_length znaków${NC}"
        return 1
    fi
    
    # Check for uppercase
    if ! echo "$password" | grep -q '[A-Z]'; then
        echo -e "${RED}✗ Hasło musi zawierać wielką literę${NC}"
        return 1
    fi
    
    # Check for lowercase
    if ! echo "$password" | grep -q '[a-z]'; then
        echo -e "${RED}✗ Hasło musi zawierać małą literę${NC}"
        return 1
    fi
    
    # Check for digit
    if ! echo "$password" | grep -q '[0-9]'; then
        echo -e "${RED}✗ Hasło musi zawierać cyfrę${NC}"
        return 1
    fi
    
    # Check for special character
    if ! echo "$password" | grep -q '[!@#$%^&*(),.?":{}|<>]'; then
        echo -e "${RED}✗ Hasło musi zawierać znak specjalny (!@#\$%^&* itp.)${NC}"
        return 1
    fi
    
    return 0
}

# Email validation function
validate_email() {
    local email="$1"
    if echo "$email" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'; then
        return 0
    else
        echo -e "${RED}✗ Nieprawidłowy format adresu email${NC}"
        return 1
    fi
}

# Check for Docker
check_docker() {
    echo -e "${YELLOW}[1/5] Sprawdzanie wymagań...${NC}"
    
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
    echo -e "${YELLOW}[2/5] Pobieranie aplikacji...${NC}"
    
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
    echo -e "${YELLOW}[3/5] Konfiguracja...${NC}"
    
    cd "$INSTALL_DIR"
    
    # Check if running interactively
    if [ -t 0 ]; then
        # ========== INTERACTIVE MODE ==========
        
        echo ""
        echo -e "${CYAN}--- Konfiguracja Bazy Danych ---${NC}"
        
        # SA Password
        if [ -z "$SA_PASSWORD" ]; then
            while true; do
                echo -e "  Podaj hasło do bazy danych SQL Server (SA):"
                echo -e "  ${YELLOW}(min. 12 znaków, wielka litera, mała litera, cyfra, znak specjalny)${NC}"
                read -s -p "  Hasło: " SA_PASSWORD
                echo
                if validate_password "$SA_PASSWORD"; then
                    read -s -p "  Potwierdź hasło: " SA_PASSWORD_CONFIRM
                    echo
                    if [ "$SA_PASSWORD" = "$SA_PASSWORD_CONFIRM" ]; then
                        echo -e "${GREEN}  ✓ Hasło bazy danych ustawione${NC}"
                        break
                    else
                        echo -e "${RED}  ✗ Hasła nie są identyczne${NC}"
                        SA_PASSWORD=""
                    fi
                else
                    SA_PASSWORD=""
                fi
            done
        fi
        
        echo ""
        echo -e "${CYAN}--- Konfiguracja Konta Administratora ---${NC}"
        
        # Admin Email
        if [ -z "$ADMIN_EMAIL" ]; then
            while true; do
                read -p "  Email administratora: " ADMIN_EMAIL
                if validate_email "$ADMIN_EMAIL"; then
                    echo -e "${GREEN}  ✓ Email: $ADMIN_EMAIL${NC}"
                    break
                else
                    ADMIN_EMAIL=""
                fi
            done
        fi
        
        # Admin Password
        if [ -z "$ADMIN_PASSWORD" ]; then
            while true; do
                echo -e "  Hasło administratora:"
                echo -e "  ${YELLOW}(min. 12 znaków, wielka litera, mała litera, cyfra, znak specjalny)${NC}"
                read -s -p "  Hasło: " ADMIN_PASSWORD
                echo
                if validate_password "$ADMIN_PASSWORD"; then
                    read -s -p "  Potwierdź hasło: " ADMIN_PASSWORD_CONFIRM
                    echo
                    if [ "$ADMIN_PASSWORD" = "$ADMIN_PASSWORD_CONFIRM" ]; then
                        echo -e "${GREEN}  ✓ Hasło administratora ustawione${NC}"
                        break
                    else
                        echo -e "${RED}  ✗ Hasła nie są identyczne${NC}"
                        ADMIN_PASSWORD=""
                    fi
                else
                    ADMIN_PASSWORD=""
                fi
            done
        fi
        
        echo ""
        echo -e "${CYAN}--- Dane Testowe ---${NC}"
        
        # Test Data
        read -p "  Czy załadować dane testowe? (t/n) [t]: " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Nn]$ ]]; then
            SEED_TEST_DATA="false"
        else
            SEED_TEST_DATA="true"
        fi
        
    else
        # ========== NON-INTERACTIVE MODE ==========
        echo "  Tryb nieinteraktywny - używam zmiennych środowiskowych."
        
        # Use defaults if not provided
        SA_PASSWORD="${SA_PASSWORD:-YourStrong@Password}"
        ADMIN_EMAIL="${ADMIN_EMAIL:-admin@x.pl}"
        ADMIN_PASSWORD="${ADMIN_PASSWORD:-Pass1234!@#\$}"
        SEED_TEST_DATA="${SEED_TEST_DATA:-true}"
    fi
    
    echo ""
    echo -e "${GREEN}✓ Konfiguracja zakończona${NC}"
    echo "  • Dane testowe: $SEED_TEST_DATA"
}

# Secure file permissions
secure_permissions() {
    echo -e "${YELLOW}[4/6] Zabezpieczanie uprawnień...${NC}"
    
    cd "$INSTALL_DIR"
    
    # Remove macOS artifacts
    find . -name ".DS_Store" -type f -delete 2>/dev/null || true
    echo -e "${GREEN}  ✓ Usunięto pliki .DS_Store${NC}"
    
    # Secure .env file (contains sensitive data)
    if [ -f ".env" ]; then
        chmod 600 .env
        echo -e "${GREEN}  ✓ Zabezpieczono plik .env (chmod 600)${NC}"
    fi
    
    # Ensure scripts are executable
    if [ -f "install.sh" ]; then
        chmod +x install.sh
    fi
    
    echo -e "${GREEN}✓ Uprawnienia zabezpieczone${NC}"
}

# Start application
start_app() {
    echo -e "${YELLOW}[5/6] Uruchamianie aplikacji...${NC}"
    
    cd "$INSTALL_DIR"
    
    # Export all configuration variables
    export SEED_TEST_DATA
    export SA_PASSWORD
    export ADMIN_EMAIL
    export ADMIN_PASSWORD
    
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
}

# Show final info
show_info() {
    echo ""
    echo -e "${YELLOW}[6/6] Gotowe!${NC}"
    echo ""
    echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
    echo -e "${GREEN}║              Instalacja zakończona!                       ║${NC}"
    echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  Aplikacja dostępna pod adresem: ${GREEN}http://localhost:8080${NC}"
    echo ""
    echo "  Dane logowania administratora:"
    echo -e "    Email:    ${YELLOW}$ADMIN_EMAIL${NC}"
    echo -e "    Hasło:    ${YELLOW}(ustawione podczas instalacji)${NC}"
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
    secure_permissions
    start_app
    show_info
}

main "$@"
