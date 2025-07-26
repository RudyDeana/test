#!/bin/bash
set -e

echo "[1/4] Installazione Asterisk..."
sudo apt update
sudo apt install -y asterisk nginx python3 python3-pip

echo "[2/4] Configurazione Asterisk..."
sudo cp asterisk-config/pjsip.conf /etc/asterisk/pjsip.conf
sudo cp asterisk-config/extensions.conf /etc/asterisk/extensions.conf
sudo systemctl restart asterisk

echo "[3/4] Setup Web Interface..."
sudo cp -r web/* /var/www/html/

echo "[4/4] Avvio server chat WebSocket..."
pip3 install websockets
nohup python3 chat-server.py &

echo "✅ Installazione completata. Apri il browser su http://raspberrypi.local"