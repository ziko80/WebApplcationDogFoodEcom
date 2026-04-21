import { useState, type FormEvent } from 'react'
import './pawmeds.css'

// Demo credentials — replace with real auth later.
const DEMO_USER = 'admin'
const DEMO_PASS = 'admin123'

interface LoginProps {
  onLogin: (username: string) => void
}

export default function Login({ onLogin }: LoginProps) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    setError(null)
    setSubmitting(true)

    // Fake network delay so the UI feels real.
    setTimeout(() => {
      if (username.trim() === DEMO_USER && password === DEMO_PASS) {
        localStorage.setItem('pawmeds.auth.user', username.trim())
        onLogin(username.trim())
      } else {
        setError('Invalid username or password.')
      }
      setSubmitting(false)
    }, 300)
  }

  return (
    <div className="login-shell">
      <form className="login-card" onSubmit={handleSubmit} aria-labelledby="login-heading">
        <div className="login-brand">
          <span className="login-logo" aria-hidden="true">🐾</span>
          <h1 id="login-heading">PawMeds</h1>
          <p>Sign in to browse our medicine &amp; vaccine catalog.</p>
        </div>

        <label className="login-field">
          <span>Username</span>
          <input
            type="text"
            autoComplete="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            autoFocus
          />
        </label>

        <label className="login-field">
          <span>Password</span>
          <input
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>

        {error && (
          <div className="login-error" role="alert">
            {error}
          </div>
        )}

        <button type="submit" className="login-submit" disabled={submitting}>
          {submitting ? 'Signing in…' : 'Sign in'}
        </button>

        <p className="login-hint">
          Demo credentials: <code>{DEMO_USER}</code> / <code>{DEMO_PASS}</code>
        </p>
      </form>
    </div>
  )
}
