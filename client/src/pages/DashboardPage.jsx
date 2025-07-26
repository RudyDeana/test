import React from 'react'
import { Phone, Users, Clock, User } from 'lucide-react'
import { useAuthStore } from '../stores/authStore'
import { useVoipStore } from '../stores/voipStore'
import { Link } from 'react-router-dom'

const DashboardPage = () => {
  const { user } = useAuthStore()
  const { users, isConnected, makeCall } = useVoipStore()

  const onlineUsers = users.filter(u => u.extension !== user?.extension)

  const handleQuickCall = (extension) => {
    makeCall(extension)
  }

  return (
    <div className="space-y-8">
      {/* Welcome Section */}
      <div className="card p-6">
        <div className="flex items-center space-x-4">
          <div className="w-16 h-16 bg-primary-100 rounded-full flex items-center justify-center">
            <User className="w-8 h-8 text-primary-600" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              Benvenuto, {user?.username}!
            </h1>
            <p className="text-gray-600">
              Interno: {user?.extension} • {user?.role === 'admin' ? 'Amministratore' : 'Utente'}
            </p>
            <div className="flex items-center mt-2">
              <div className={`w-3 h-3 rounded-full mr-2 ${
                isConnected ? 'bg-success-500' : 'bg-danger-500'
              }`}></div>
              <span className={`text-sm ${
                isConnected ? 'text-success-600' : 'text-danger-600'
              }`}>
                {isConnected ? 'Sistema attivo' : 'Sistema offline'}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Link
          to="/phone"
          className="card p-6 hover:shadow-md transition-shadow cursor-pointer group"
        >
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 bg-success-100 rounded-lg flex items-center justify-center group-hover:bg-success-200 transition-colors">
              <Phone className="w-6 h-6 text-success-600" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Tastierino</h3>
              <p className="text-gray-600">Effettua una chiamata</p>
            </div>
          </div>
        </Link>

        <div className="card p-6">
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center">
              <Users className="w-6 h-6 text-primary-600" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-gray-900">
                {onlineUsers.length}
              </h3>
              <p className="text-gray-600">Utenti online</p>
            </div>
          </div>
        </div>

        <div className="card p-6">
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 bg-warning-100 rounded-lg flex items-center justify-center">
              <Clock className="w-6 h-6 text-warning-600" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Sistema 24/7</h3>
              <p className="text-gray-600">Sempre disponibile</p>
            </div>
          </div>
        </div>
      </div>

      {/* Online Users */}
      <div className="card p-6">
        <h2 className="text-xl font-semibold text-gray-900 mb-4">
          Utenti Online ({onlineUsers.length})
        </h2>
        
        {onlineUsers.length === 0 ? (
          <div className="text-center py-8">
            <Users className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">Nessun altro utente online al momento</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {onlineUsers.map((u) => (
              <div
                key={u.extension}
                className="flex items-center justify-between p-4 bg-gray-50 rounded-lg"
              >
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
                    <User className="w-5 h-5 text-primary-600" />
                  </div>
                  <div>
                    <div className="font-medium text-gray-900">{u.username}</div>
                    <div className="text-sm text-gray-500">Ext: {u.extension}</div>
                  </div>
                </div>
                
                <button
                  onClick={() => handleQuickCall(u.extension)}
                  className="btn btn-primary btn-sm"
                  title={`Chiama ${u.username}`}
                >
                  <Phone className="w-4 h-4" />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* System Information */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Informazioni Sistema
          </h3>
          <div className="space-y-3">
            <div className="flex justify-between">
              <span className="text-gray-600">Versione:</span>
              <span className="font-medium">WebVoIP v1.0</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Protocollo:</span>
              <span className="font-medium">WebRTC</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Stato servizio:</span>
              <span className={`font-medium ${
                isConnected ? 'text-success-600' : 'text-danger-600'
              }`}>
                {isConnected ? 'Attivo' : 'Offline'}
              </span>
            </div>
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Il Tuo Profilo
          </h3>
          <div className="space-y-3">
            <div className="flex justify-between">
              <span className="text-gray-600">Username:</span>
              <span className="font-medium">{user?.username}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Email:</span>
              <span className="font-medium">{user?.email}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Interno:</span>
              <span className="font-medium">{user?.extension}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Ruolo:</span>
              <span className="font-medium">
                {user?.role === 'admin' ? 'Amministratore' : 'Utente'}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default DashboardPage