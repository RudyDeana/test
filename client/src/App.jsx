import React, { useEffect } from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './stores/authStore'
import { useVoipStore } from './stores/voipStore'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import PhonePage from './pages/PhonePage'
import AdminPage from './pages/AdminPage'
import Layout from './components/Layout'

function App() {
  const { user, checkAuth } = useAuthStore()
  const { initialize } = useVoipStore()

  useEffect(() => {
    checkAuth()
  }, [checkAuth])

  useEffect(() => {
    if (user) {
      initialize(user)
    }
  }, [user, initialize])

  if (!user) {
    return <LoginPage />
  }

  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/phone" element={<PhonePage />} />
        {user.role === 'admin' && (
          <Route path="/admin" element={<AdminPage />} />
        )}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </Layout>
  )
}

export default App