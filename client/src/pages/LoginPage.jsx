import React, { useState } from 'react'
import { Phone, Eye, EyeOff, Loader2 } from 'lucide-react'
import { useAuthStore } from '../stores/authStore'

const LoginPage = () => {
  const [isLogin, setIsLogin] = useState(true)
  const [showPassword, setShowPassword] = useState(false)
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    extension: ''
  })

  const { login, register, isLoading } = useAuthStore()

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      if (isLogin) {
        await login({
          username: formData.username,
          password: formData.password
        })
      } else {
        await register(formData)
        setIsLogin(true)
        setFormData({ username: '', email: '', password: '', extension: '' })
      }
    } catch (error) {
      // Error is handled in the store
    }
  }

  const handleChange = (e) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }))
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 to-primary-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <div className="mx-auto h-16 w-16 bg-primary-600 rounded-full flex items-center justify-center">
            <Phone className="h-8 w-8 text-white" />
          </div>
          <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
            WebVoIP System
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            {isLogin ? 'Accedi al tuo account' : 'Crea un nuovo account'}
          </p>
        </div>

        <div className="card p-8">
          <form className="space-y-6" onSubmit={handleSubmit}>
            {/* Username */}
            <div>
              <label htmlFor="username" className="block text-sm font-medium text-gray-700">
                Username
              </label>
              <input
                id="username"
                name="username"
                type="text"
                required
                value={formData.username}
                onChange={handleChange}
                className="input mt-1"
                placeholder="Inserisci username"
              />
            </div>

            {/* Email (only for registration) */}
            {!isLogin && (
              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                  Email
                </label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  required
                  value={formData.email}
                  onChange={handleChange}
                  className="input mt-1"
                  placeholder="Inserisci email"
                />
              </div>
            )}

            {/* Extension (only for registration) */}
            {!isLogin && (
              <div>
                <label htmlFor="extension" className="block text-sm font-medium text-gray-700">
                  Interno
                </label>
                <input
                  id="extension"
                  name="extension"
                  type="text"
                  required
                  value={formData.extension}
                  onChange={handleChange}
                  className="input mt-1"
                  placeholder="es. 1003"
                />
              </div>
            )}

            {/* Password */}
            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                Password
              </label>
              <div className="mt-1 relative">
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={formData.password}
                  onChange={handleChange}
                  className="input pr-10"
                  placeholder="Inserisci password"
                />
                <button
                  type="button"
                  className="absolute inset-y-0 right-0 pr-3 flex items-center"
                  onClick={() => setShowPassword(!showPassword)}
                >
                  {showPassword ? (
                    <EyeOff className="h-5 w-5 text-gray-400" />
                  ) : (
                    <Eye className="h-5 w-5 text-gray-400" />
                  )}
                </button>
              </div>
            </div>

            {/* Submit Button */}
            <div>
              <button
                type="submit"
                disabled={isLoading}
                className="btn btn-primary w-full flex justify-center items-center"
              >
                {isLoading ? (
                  <Loader2 className="animate-spin h-5 w-5" />
                ) : (
                  isLogin ? 'Accedi' : 'Registrati'
                )}
              </button>
            </div>

            {/* Toggle Form */}
            <div className="text-center">
              <button
                type="button"
                onClick={() => setIsLogin(!isLogin)}
                className="text-sm text-primary-600 hover:text-primary-500"
              >
                {isLogin 
                  ? "Non hai un account? Registrati" 
                  : "Hai già un account? Accedi"
                }
              </button>
            </div>
          </form>

          {/* Demo Credentials */}
          {isLogin && (
            <div className="mt-6 p-4 bg-gray-50 rounded-lg">
              <h4 className="text-sm font-medium text-gray-700 mb-2">Credenziali Demo:</h4>
              <div className="text-xs text-gray-600 space-y-1">
                <div><strong>Admin:</strong> admin / admin123</div>
                <div><strong>User1:</strong> user1 / user123 (ext: 1001)</div>
                <div><strong>User2:</strong> user2 / user123 (ext: 1002)</div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default LoginPage