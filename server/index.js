const express = require('express');
const http = require('http');
const socketIo = require('socket.io');
const cors = require('cors');
const path = require('path');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcryptjs');
const sqlite3 = require('sqlite3').verbose();
const { v4: uuidv4 } = require('uuid');

const app = express();
const server = http.createServer(app);
const io = socketIo(server, {
  cors: {
    origin: "*",
    methods: ["GET", "POST"]
  }
});

const PORT = process.env.PORT || 3000;
const JWT_SECRET = process.env.JWT_SECRET || 'voip-secret-key-2024';

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, '../client/dist')));

// Database setup
const db = new sqlite3.Database('./voip.db');

// Initialize database tables
db.serialize(() => {
  // Users table
  db.run(`CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE NOT NULL,
    email TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,
    extension TEXT UNIQUE NOT NULL,
    role TEXT DEFAULT 'user',
    status TEXT DEFAULT 'offline',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
  )`);

  // Calls table
  db.run(`CREATE TABLE IF NOT EXISTS calls (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    caller_id TEXT NOT NULL,
    callee_id TEXT NOT NULL,
    start_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    end_time DATETIME,
    duration INTEGER,
    status TEXT DEFAULT 'initiated'
  )`);

  // Create admin user if not exists
  const adminPassword = bcrypt.hashSync('admin123', 10);
  db.run(`INSERT OR IGNORE INTO users (username, email, password, extension, role) 
          VALUES ('admin', 'admin@voip.local', ?, '1000', 'admin')`, [adminPassword]);
  
  // Create demo users
  const userPassword = bcrypt.hashSync('user123', 10);
  db.run(`INSERT OR IGNORE INTO users (username, email, password, extension, role) 
          VALUES ('user1', 'user1@voip.local', ?, '1001', 'user')`, [userPassword]);
  db.run(`INSERT OR IGNORE INTO users (username, email, password, extension, role) 
          VALUES ('user2', 'user2@voip.local', ?, '1002', 'user')`, [userPassword]);
});

// Active connections
const activeUsers = new Map();
const activeCalls = new Map();

// Authentication middleware
const authenticateToken = (req, res, next) => {
  const authHeader = req.headers['authorization'];
  const token = authHeader && authHeader.split(' ')[1];

  if (!token) {
    return res.sendStatus(401);
  }

  jwt.verify(token, JWT_SECRET, (err, user) => {
    if (err) return res.sendStatus(403);
    req.user = user;
    next();
  });
};

// API Routes
app.post('/api/login', (req, res) => {
  const { username, password } = req.body;
  
  db.get('SELECT * FROM users WHERE username = ?', [username], (err, user) => {
    if (err || !user) {
      return res.status(401).json({ error: 'Credenziali non valide' });
    }

    if (bcrypt.compareSync(password, user.password)) {
      const token = jwt.sign(
        { id: user.id, username: user.username, role: user.role, extension: user.extension },
        JWT_SECRET,
        { expiresIn: '24h' }
      );
      
      // Update user status
      db.run('UPDATE users SET status = ? WHERE id = ?', ['online', user.id]);
      
      res.json({
        token,
        user: {
          id: user.id,
          username: user.username,
          email: user.email,
          extension: user.extension,
          role: user.role
        }
      });
    } else {
      res.status(401).json({ error: 'Credenziali non valide' });
    }
  });
});

app.post('/api/register', (req, res) => {
  const { username, email, password, extension } = req.body;
  const hashedPassword = bcrypt.hashSync(password, 10);
  
  db.run('INSERT INTO users (username, email, password, extension) VALUES (?, ?, ?, ?)',
    [username, email, hashedPassword, extension], function(err) {
      if (err) {
        return res.status(400).json({ error: 'Utente già esistente' });
      }
      res.json({ message: 'Utente creato con successo', id: this.lastID });
    });
});

app.get('/api/users', authenticateToken, (req, res) => {
  if (req.user.role !== 'admin') {
    return res.status(403).json({ error: 'Accesso negato' });
  }
  
  db.all('SELECT id, username, email, extension, role, status, created_at FROM users', (err, users) => {
    if (err) {
      return res.status(500).json({ error: 'Errore del database' });
    }
    res.json(users);
  });
});

app.get('/api/calls', authenticateToken, (req, res) => {
  if (req.user.role !== 'admin') {
    return res.status(403).json({ error: 'Accesso negato' });
  }
  
  db.all('SELECT * FROM calls ORDER BY start_time DESC LIMIT 100', (err, calls) => {
    if (err) {
      return res.status(500).json({ error: 'Errore del database' });
    }
    res.json(calls);
  });
});

app.get('/api/profile', authenticateToken, (req, res) => {
  db.get('SELECT id, username, email, extension, role, status FROM users WHERE id = ?', 
    [req.user.id], (err, user) => {
      if (err || !user) {
        return res.status(404).json({ error: 'Utente non trovato' });
      }
      res.json(user);
    });
});

// WebRTC Signaling
io.on('connection', (socket) => {
  console.log('Client connesso:', socket.id);

  socket.on('register', (data) => {
    const { extension, username } = data;
    activeUsers.set(extension, {
      socketId: socket.id,
      username,
      extension,
      status: 'available'
    });
    
    socket.extension = extension;
    socket.join(`user_${extension}`);
    
    // Notify all clients about updated user list
    io.emit('users_updated', Array.from(activeUsers.values()));
    
    console.log(`Utente ${username} (${extension}) registrato`);
  });

  socket.on('call_request', (data) => {
    const { targetExtension, offer, callId } = data;
    const caller = activeUsers.get(socket.extension);
    const callee = activeUsers.get(targetExtension);

    if (callee) {
      const callData = {
        id: callId,
        caller: caller,
        callee: callee,
        status: 'ringing',
        startTime: new Date().toISOString()
      };

      activeCalls.set(callId, callData);

      // Send call request to callee
      io.to(callee.socketId).emit('incoming_call', {
        callId,
        caller: caller,
        offer
      });

      // Log call to database
      db.run('INSERT INTO calls (caller_id, callee_id, status) VALUES (?, ?, ?)',
        [caller.extension, callee.extension, 'initiated']);

      console.log(`Chiamata da ${caller.extension} a ${targetExtension}`);
    } else {
      socket.emit('call_failed', { error: 'Utente non disponibile' });
    }
  });

  socket.on('call_answer', (data) => {
    const { callId, answer } = data;
    const call = activeCalls.get(callId);

    if (call) {
      call.status = 'active';
      activeCalls.set(callId, call);

      // Send answer to caller
      io.to(call.caller.socketId).emit('call_answered', {
        callId,
        answer
      });

      console.log(`Chiamata ${callId} accettata`);
    }
  });

  socket.on('call_reject', (data) => {
    const { callId } = data;
    const call = activeCalls.get(callId);

    if (call) {
      // Notify caller
      io.to(call.caller.socketId).emit('call_rejected', { callId });
      activeCalls.delete(callId);

      console.log(`Chiamata ${callId} rifiutata`);
    }
  });

  socket.on('call_end', (data) => {
    const { callId } = data;
    const call = activeCalls.get(callId);

    if (call) {
      // Notify both parties
      io.to(call.caller.socketId).emit('call_ended', { callId });
      io.to(call.callee.socketId).emit('call_ended', { callId });
      
      // Update call in database
      const duration = Math.floor((new Date() - new Date(call.startTime)) / 1000);
      db.run('UPDATE calls SET end_time = CURRENT_TIMESTAMP, duration = ?, status = ? WHERE caller_id = ? AND callee_id = ? AND status != ?',
        [duration, 'completed', call.caller.extension, call.callee.extension, 'completed']);

      activeCalls.delete(callId);
      console.log(`Chiamata ${callId} terminata`);
    }
  });

  socket.on('ice_candidate', (data) => {
    const { callId, candidate, targetExtension } = data;
    const target = activeUsers.get(targetExtension);

    if (target) {
      io.to(target.socketId).emit('ice_candidate', {
        callId,
        candidate
      });
    }
  });

  socket.on('disconnect', () => {
    if (socket.extension) {
      activeUsers.delete(socket.extension);
      
      // End any active calls
      for (const [callId, call] of activeCalls.entries()) {
        if (call.caller.extension === socket.extension || call.callee.extension === socket.extension) {
          const otherParty = call.caller.extension === socket.extension ? call.callee : call.caller;
          io.to(otherParty.socketId).emit('call_ended', { callId, reason: 'disconnect' });
          activeCalls.delete(callId);
        }
      }

      // Update user status in database
      db.run('UPDATE users SET status = ? WHERE extension = ?', ['offline', socket.extension]);
      
      // Notify all clients
      io.emit('users_updated', Array.from(activeUsers.values()));
      
      console.log(`Utente ${socket.extension} disconnesso`);
    }
  });
});

// Serve React app for all other routes
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, '../client/dist/index.html'));
});

server.listen(PORT, '0.0.0.0', () => {
  console.log(`🚀 Server VoIP in esecuzione su porta ${PORT}`);
  console.log(`📱 Dashboard: http://localhost:${PORT}`);
  console.log(`👤 Admin login: admin / admin123`);
  console.log(`👤 User login: user1 / user123 (ext: 1001)`);
  console.log(`👤 User login: user2 / user123 (ext: 1002)`);
});