import { useState } from 'react'
import './App.css'
import Login from './components/Login'
import Products from './components/Products'

function App() {
  const [authUser, setAuthUser] = useState<string | null>(
    () => localStorage.getItem('pawmeds.auth.user')
  )

  const handleLogin = (username: string) => setAuthUser(username)
  const handleLogout = () => {
    localStorage.removeItem('pawmeds.auth.user')
    setAuthUser(null)
  }

  if (!authUser) {
    return <Login onLogin={handleLogin} />
  }

  return <Products username={authUser} onLogout={handleLogout} />
}

export default App
