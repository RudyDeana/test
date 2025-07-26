# 📞 WebVoIP System

Un sistema VoIP web completo simile a 3CX, progettato per funzionare su Raspberry Pi e accessibile da qualsiasi dispositivo (telefoni, tablet, computer).

## ✨ Caratteristiche

### 🎯 Funzionalità Principali
- **📱 Multi-dispositivo**: Funziona su telefoni, tablet e computer
- **🌐 Completamente web**: Nessuna app da installare
- **🎨 Interfaccia moderna**: Design responsivo e intuitivo
- **☁️ Self-hosted**: Controllo completo dei tuoi dati
- **⚡ Real-time**: Comunicazione istantanea con WebRTC
- **👨‍💼 Dashboard Admin**: Gestione completa utenti e sistema

### 🔧 Tecnologie
- **Backend**: Node.js, Express, Socket.io, SQLite
- **Frontend**: React, Tailwind CSS, Zustand
- **WebRTC**: Chiamate audio peer-to-peer
- **Database**: SQLite (leggero e affidabile)
- **Server**: Nginx reverse proxy

### 🎮 Interfacce
1. **Dashboard Principale**: Panoramica sistema e utenti online
2. **Tastierino Telefonico**: Interfaccia per effettuare chiamate
3. **Pannello Admin**: Gestione utenti e registro chiamate

## 🚀 Installazione Rapida

### Metodo 1: Script Automatico (Consigliato)

```bash
# Clona il repository
git clone https://github.com/RudyDeana/test
cd webvoip-system

# Rendi eseguibile lo script
chmod +x setup.sh

# Esegui l'installazione
./setup.sh
```

### Metodo 2: Installazione Manuale

```bash
# Installa dipendenze
npm run install:all

# Build frontend
npm run build

# Avvia il server
npm start
```

## 💻 Sviluppo

```bash
# Installa dipendenze
npm run install:all

# Avvia in modalità sviluppo
npm run dev
```

Questo avvierà:
- Backend su `http://localhost:3000`
- Frontend su `http://localhost:5173`

## 🔐 Credenziali di Default

| Utente | Username | Password | Interno | Ruolo |
|--------|----------|----------|---------|-------|
| Admin  | admin    | admin123 | 1000    | Admin |
| User 1 | user1    | user123  | 1001    | User  |
| User 2 | user2    | user123  | 1002    | User  |

## 📋 Requisiti di Sistema

### 🍓 Raspberry Pi (Consigliato)
- **Modello**: Raspberry Pi 4 (2GB RAM minimo, 4GB consigliato)
- **OS**: Raspberry Pi OS Lite 64-bit
- **Storage**: MicroSD 16GB+ (Classe 10)
- **Rete**: Ethernet o WiFi

### 💻 Altri Sistemi
- **CPU**: Dual-core 1GHz+
- **RAM**: 1GB+
- **OS**: Linux, macOS, Windows
- **Node.js**: v18.x o superiore

## 🌐 Accesso al Sistema

Dopo l'installazione, il sistema sarà accessibile su:
- `http://IP_RASPBERRY_PI`
- `http://raspberrypi.local` (se supportato)

### 📱 Dispositivi Supportati
- **Desktop**: Chrome, Firefox, Safari, Edge
- **Mobile**: iOS Safari, Android Chrome
- **Tablet**: Tutti i browser moderni

## 🛠️ Configurazione Avanzata

### 🔧 Variabili d'Ambiente

Crea un file `.env` nella root del progetto:

```env
PORT=3000
JWT_SECRET=your-secret-key
NODE_ENV=production
```

### 🌍 Configurazione Nginx

Il sistema include una configurazione Nginx ottimizzata con:
- Proxy reverso per l'applicazione
- Supporto WebSocket
- Header di sicurezza
- Caching per file statici

### 📊 Monitoraggio

Il sistema include:
- **Script di monitoraggio**: Controllo automatico ogni 5 minuti
- **Backup automatico**: Backup giornaliero del database
- **Log centralizzati**: `/var/log/webvoip-monitor.log`

## 🔍 Struttura del Progetto

```
webvoip-system/
├── server/                 # Backend Node.js
│   └── index.js           # Server principale
├── client/                # Frontend React
│   ├── src/
│   │   ├── components/    # Componenti React
│   │   ├── pages/         # Pagine principali
│   │   ├── stores/        # State management (Zustand)
│   │   └── ...
│   ├── dist/              # Build produzione
│   └── package.json
├── setup.sh               # Script installazione
├── package.json           # Dipendenze backend
└── README.md
```

## 🔧 Comandi Utili

### 📊 Gestione Servizi
```bash
# Stato del server
sudo -u voip pm2 status

# Log in tempo reale
sudo -u voip pm2 logs webvoip-server

# Riavvia il server
sudo -u voip pm2 restart webvoip-server

# Riavvia Nginx
sudo systemctl restart nginx
```

### 💾 Backup e Ripristino
```bash
# Backup manuale
sudo -u voip /home/voip/webvoip/backup.sh

# Lista backup
ls -la /home/voip/backups/

# Ripristino database
sudo -u voip cp backup_file.db /home/voip/webvoip/voip.db
```

## 🚨 Risoluzione Problemi

### 🔍 Debug Comune

**Il sito non si carica:**
```bash
# Controlla stato servizi
sudo systemctl status nginx
sudo -u voip pm2 status

# Controlla log
sudo -u voip pm2 logs webvoip-server
sudo tail -f /var/log/nginx/error.log
```

**Problemi audio:**
- Assicurati che il browser abbia i permessi per il microfono
- Usa HTTPS per la produzione (WebRTC richiede connessione sicura)
- Controlla firewall e NAT

**Database corrotto:**
```bash
# Ripristina da backup
sudo -u voip cp /home/voip/backups/latest_backup.tar.gz /tmp/
cd /tmp && tar -xzf latest_backup.tar.gz
sudo -u voip cp voip.db /home/voip/webvoip/
```

## 🔐 Sicurezza

### 🛡️ Misure di Sicurezza Implementate
- JWT per autenticazione
- Hash delle password con bcrypt
- Header di sicurezza HTTP
- Firewall configurato
- Utente dedicato senza privilegi elevati

### 🔒 Raccomandazioni Aggiuntive
- Cambia le password di default
- Usa HTTPS in produzione
- Aggiorna regolarmente il sistema
- Monitora i log per attività sospette

## 🤝 Supporto e Contributi

### 📧 Supporto
Per problemi o domande, controlla:
1. Questa documentazione
2. Log di sistema (`/var/log/webvoip-monitor.log`)
3. Log applicazione (`sudo -u voip pm2 logs`)

### 🎯 Roadmap Futura
- [ ] Supporto video chiamate
- [ ] Chat di testo integrata
- [ ] Integrazione LDAP/Active Directory
- [ ] API REST completa
- [ ] App mobile native
- [ ] Supporto SIP tradizionale

## 📄 Licenza

Questo progetto è rilasciato sotto licenza MIT. Vedi il file LICENSE per i dettagli.

## 🎉 Ringraziamenti

Costruito con amore per la community open source, utilizzando:
- React & Tailwind CSS per una UI moderna
- WebRTC per comunicazioni real-time
- Node.js & Express per un backend robusto
- SQLite per semplicità e affidabilità

---

**📞 WebVoIP System - Telefonia moderna per tutti! 🚀**
