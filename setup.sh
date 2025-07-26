#!/bin/bash

# WebVoIP System Setup Script for Raspberry Pi
# Questo script installa e configura il sistema VoIP completo

set -e

echo "🚀 Installazione WebVoIP System su Raspberry Pi"
echo "================================================"

# Colori per output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Funzione per stampare messaggi colorati
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verifica se siamo su Raspberry Pi
if [[ ! -f /proc/device-tree/model ]] || ! grep -q "Raspberry Pi" /proc/device-tree/model 2>/dev/null; then
    print_warning "Questo script è ottimizzato per Raspberry Pi, ma continuerò comunque..."
fi

# Aggiorna il sistema
print_status "Aggiornamento del sistema..."
sudo apt update && sudo apt upgrade -y

# Installa Node.js 18.x (versione LTS)
print_status "Installazione Node.js..."
if ! command -v node &> /dev/null; then
    curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
    sudo apt-get install -y nodejs
fi

print_success "Node.js versione: $(node --version)"
print_success "NPM versione: $(npm --version)"

# Installa dipendenze di sistema
print_status "Installazione dipendenze di sistema..."
sudo apt install -y \
    git \
    curl \
    sqlite3 \
    nginx \
    ufw \
    htop \
    build-essential \
    python3 \
    python3-pip

# Installa PM2 per gestione processi
print_status "Installazione PM2..."
sudo npm install -g pm2

# Crea utente dedicato (se non esiste)
if ! id "voip" &>/dev/null; then
    print_status "Creazione utente dedicato 'voip'..."
    sudo useradd -m -s /bin/bash voip
    sudo usermod -aG sudo voip
fi

# Installa dipendenze NPM
print_status "Installazione dipendenze backend..."
npm install

print_status "Installazione dipendenze frontend..."
cd client && npm install && cd ..

# Build del frontend
print_status "Build del frontend..."
cd client && npm run build && cd ..

# Configurazione Nginx
print_status "Configurazione Nginx..."
sudo tee /etc/nginx/sites-available/webvoip > /dev/null <<EOF
server {
    listen 80;
    listen [::]:80;
    server_name _;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;
    
    # Main application
    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        proxy_read_timeout 86400;
    }
    
    # WebSocket support
    location /socket.io/ {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
    
    # Static files
    location /static/ {
        alias /home/voip/webvoip/client/dist/;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
EOF

# Abilita il sito
sudo ln -sf /etc/nginx/sites-available/webvoip /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default

# Test configurazione Nginx
sudo nginx -t

# Configurazione firewall
print_status "Configurazione firewall..."
sudo ufw --force enable
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 3000/tcp
sudo ufw reload

# Crea directory di lavoro
sudo mkdir -p /home/voip/webvoip
sudo cp -r . /home/voip/webvoip/
sudo chown -R voip:voip /home/voip/webvoip

# Configurazione PM2
print_status "Configurazione PM2..."
sudo -u voip bash -c "cd /home/voip/webvoip && pm2 start server/index.js --name webvoip-server"
sudo -u voip pm2 save
sudo -u voip pm2 startup

# Abilita servizi
print_status "Abilitazione servizi..."
sudo systemctl enable nginx
sudo systemctl restart nginx

# Configurazioni ottimizzate per Raspberry Pi
print_status "Ottimizzazioni per Raspberry Pi..."

# Aumenta memoria condivisa GPU (per prestazioni migliori)
if ! grep -q "gpu_mem=128" /boot/config.txt; then
    echo "gpu_mem=128" | sudo tee -a /boot/config.txt
fi

# Ottimizza parametri di rete
sudo tee -a /etc/sysctl.conf > /dev/null <<EOF

# Ottimizzazioni WebVoIP
net.core.rmem_max = 16777216
net.core.wmem_max = 16777216
net.ipv4.tcp_rmem = 4096 87380 16777216
net.ipv4.tcp_wmem = 4096 65536 16777216
net.core.netdev_max_backlog = 5000
EOF

# Script di monitoraggio
print_status "Creazione script di monitoraggio..."
sudo tee /home/voip/webvoip/monitor.sh > /dev/null <<'EOF'
#!/bin/bash

# Script di monitoraggio WebVoIP
LOG_FILE="/var/log/webvoip-monitor.log"

echo "$(date): Controllo stato servizi WebVoIP" >> $LOG_FILE

# Controlla PM2
if ! pgrep -f "pm2" > /dev/null; then
    echo "$(date): PM2 non attivo, riavvio..." >> $LOG_FILE
    sudo -u voip pm2 resurrect
fi

# Controlla Nginx
if ! systemctl is-active --quiet nginx; then
    echo "$(date): Nginx non attivo, riavvio..." >> $LOG_FILE
    sudo systemctl restart nginx
fi

# Controlla spazio disco
DISK_USAGE=$(df / | awk 'NR==2 {print $5}' | sed 's/%//')
if [ $DISK_USAGE -gt 85 ]; then
    echo "$(date): ATTENZIONE: Spazio disco al ${DISK_USAGE}%" >> $LOG_FILE
fi

# Controlla memoria
MEM_USAGE=$(free | awk 'NR==2{printf "%.2f", $3*100/$2}')
if (( $(echo "$MEM_USAGE > 85" | bc -l) )); then
    echo "$(date): ATTENZIONE: Memoria al ${MEM_USAGE}%" >> $LOG_FILE
fi

echo "$(date): Controllo completato" >> $LOG_FILE
EOF

sudo chmod +x /home/voip/webvoip/monitor.sh
sudo chown voip:voip /home/voip/webvoip/monitor.sh

# Aggiungi al crontab per monitoraggio automatico
(sudo -u voip crontab -l 2>/dev/null; echo "*/5 * * * * /home/voip/webvoip/monitor.sh") | sudo -u voip crontab -

# Crea script di backup
print_status "Creazione script di backup..."
sudo tee /home/voip/webvoip/backup.sh > /dev/null <<'EOF'
#!/bin/bash

BACKUP_DIR="/home/voip/backups"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="webvoip_backup_$DATE.tar.gz"

mkdir -p $BACKUP_DIR

# Backup database e configurazioni
tar -czf "$BACKUP_DIR/$BACKUP_FILE" \
    /home/voip/webvoip/voip.db \
    /home/voip/webvoip/server \
    /etc/nginx/sites-available/webvoip

# Mantieni solo gli ultimi 7 backup
find $BACKUP_DIR -name "webvoip_backup_*.tar.gz" -type f -mtime +7 -delete

echo "$(date): Backup creato: $BACKUP_FILE"
EOF

sudo chmod +x /home/voip/webvoip/backup.sh
sudo chown voip:voip /home/voip/webvoip/backup.sh

# Backup giornaliero
(sudo -u voip crontab -l 2>/dev/null; echo "0 2 * * * /home/voip/webvoip/backup.sh") | sudo -u voip crontab -

# Ottieni indirizzo IP
IP_ADDRESS=$(hostname -I | awk '{print $1}')

print_success "✅ Installazione completata!"
echo ""
echo "================================================"
echo "📱 WebVoIP System è ora attivo!"
echo "================================================"
echo ""
echo "🌐 URL di accesso:"
echo "   http://$IP_ADDRESS"
echo "   http://$(hostname).local (se supportato)"
echo ""
echo "👤 Credenziali di accesso:"
echo "   Admin:  admin / admin123"
echo "   User1:  user1 / user123 (interno: 1001)"
echo "   User2:  user2 / user123 (interno: 1002)"
echo ""
echo "🔧 Comandi utili:"
echo "   Stato servizi: sudo -u voip pm2 status"
echo "   Log applicazione: sudo -u voip pm2 logs webvoip-server"
echo "   Riavvio: sudo -u voip pm2 restart webvoip-server"
echo "   Backup manuale: sudo -u voip /home/voip/webvoip/backup.sh"
echo ""
echo "📁 Directory principali:"
echo "   Applicazione: /home/voip/webvoip"
echo "   Log sistema: /var/log/webvoip-monitor.log"
echo "   Backup: /home/voip/backups"
echo ""
echo "🎉 Il sistema è pronto per l'uso!"
echo "   Apri il browser e vai all'URL indicato sopra"
echo "   Il sistema funziona su telefoni, tablet e computer"
echo ""

# Test finale
print_status "Test finale della configurazione..."
sleep 3

if curl -s -o /dev/null -w "%{http_code}" http://localhost:3000 | grep -q "200\|302"; then
    print_success "✅ Server risponde correttamente"
else
    print_error "❌ Problema con il server, controlla i log"
fi

if systemctl is-active --quiet nginx; then
    print_success "✅ Nginx attivo"
else
    print_error "❌ Problema con Nginx"
fi

print_success "Installazione WebVoIP completata! 🎉"
print_warning "Riavvia il Raspberry Pi per applicare tutte le ottimizzazioni: sudo reboot"