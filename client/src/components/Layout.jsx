import React from 'react'
import { Link, useLocation } from 'react-router-dom'
import { Phone, Home, Settings, LogOut, Users, Wifi, WifiOff } from 'lucide-react'
import { useAuthStore } from '../stores/authStore'
import { useVoipStore } from '../stores/voipStore'
import IncomingCallModal from './IncomingCallModal'

const Layout = ({ children }) => {
  const location = useLocation()
  const { user, logout } = useAuthStore()
  const { isConnected, incomingCall } = useVoipStore()

  const navigation = [
    { name: 'Dashboard', href: '/dashboard', icon: Home },
    { name: 'Telefono', href: '/phone', icon: Phone },
  ]

  if (user?.role === 'admin') {
    navigation.push({ name: 'Admin', href: '/admin', icon: Settings })
  }

  const isActive = (href) => location.pathname === href

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Top Navigation */}
      <nav className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <h1 className="text-xl font-bold text-gray-900">WebVoIP</h1>
              </div>
              
              {/* Desktop Navigation */}
              <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
                {navigation.map((item) => {
                  const Icon = item.icon
                  return (
                    <Link
                      key={item.name}
                      to={item.href}
                      className={`
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors
                        ${isActive(item.href)
                          ? 'border-primary-500 text-gray-900'
                          : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'
                        }
                      `}
                    >
                      <Icon className="w-4 h-4 mr-2" />
                      {item.name}
                    </Link>
                  )
                })}
              </div>
            </div>

            <div className="flex items-center space-x-4">
              {/* Connection Status */}
              <div className="flex items-center space-x-2">
                {isConnected ? (
                  <Wifi className="w-5 h-5 text-success-500" />
                ) : (
                  <WifiOff className="w-5 h-5 text-danger-500" />
                )}
                <span className={`text-sm ${isConnected ? 'text-success-600' : 'text-danger-600'}`}>
                  {isConnected ? 'Connesso' : 'Disconnesso'}
                </span>
              </div>

              {/* User Info */}
              <div className="flex items-center space-x-3">
                <div className="text-right">
                  <div className="text-sm font-medium text-gray-900">{user?.username}</div>
                  <div className="text-xs text-gray-500">Ext: {user?.extension}</div>
                </div>
                
                <button
                  onClick={logout}
                  className="inline-flex items-center p-2 border border-transparent rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 transition-colors"
                  title="Logout"
                >
                  <LogOut className="w-5 h-5" />
                </button>
              </div>
            </div>
          </div>
        </div>
        
        {/* Mobile Navigation */}
        <div className="sm:hidden">
          <div className="pt-2 pb-3 space-y-1 bg-gray-50 border-t border-gray-200">
            {navigation.map((item) => {
              const Icon = item.icon
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`
                    flex items-center pl-3 pr-4 py-2 border-l-4 text-base font-medium transition-colors
                    ${isActive(item.href)
                      ? 'bg-primary-50 border-primary-500 text-primary-700'
                      : 'border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800'
                    }
                  `}
                >
                  <Icon className="w-5 h-5 mr-3" />
                  {item.name}
                </Link>
              )
            })}
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {children}
      </main>

      {/* Incoming Call Modal */}
      {incomingCall && <IncomingCallModal />}
    </div>
  )
}

export default Layout