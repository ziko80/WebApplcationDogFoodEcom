import { useState, useRef, useEffect } from 'react'
import './AiChat.css'

interface ChatTurn {
  role: 'user' | 'assistant'
  content: string
}

export default function AiChat() {
  const [open, setOpen] = useState(false)
  const [message, setMessage] = useState('')
  const [history, setHistory] = useState<ChatTurn[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const bottomRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [history, loading])

  const send = async () => {
    const text = message.trim()
    if (!text || loading) return

    const userTurn: ChatTurn = { role: 'user', content: text }
    const updatedHistory = [...history, userTurn]
    setHistory(updatedHistory)
    setMessage('')
    setLoading(true)
    setError(null)

    try {
      const res = await fetch('/api/ai/chat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          message: text,
          history: history,
        }),
      })

      if (!res.ok) {
        const errText = await res.text()
        throw new Error(errText || `HTTP ${res.status}`)
      }

      const data = await res.json()
      setHistory([...updatedHistory, { role: 'assistant', content: data.reply }])
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to get a response')
    } finally {
      setLoading(false)
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      send()
    }
  }

  return (
    <>
      <button
        type="button"
        className="chat-fab"
        onClick={() => setOpen(!open)}
        aria-label="Toggle AI assistant"
      >
        {open ? '✕' : '🤖'}
      </button>

      {open && (
        <aside className="chat-panel" role="complementary" aria-label="AI assistant">
          <header className="chat-panel-header">
            <span>🐾 PawMeds Assistant</span>
          </header>

          <div className="chat-messages">
            {history.length === 0 && !loading && (
              <p className="chat-empty">Ask me anything about pet health or our products!</p>
            )}
            {history.map((turn, i) => (
              <div key={i} className={`chat-bubble chat-${turn.role}`}>
                {turn.content}
              </div>
            ))}
            {loading && (
              <div className="chat-bubble chat-assistant chat-typing">Thinking…</div>
            )}
            {error && (
              <div className="chat-error">⚠️ {error}</div>
            )}
            <div ref={bottomRef} />
          </div>

          <form className="chat-input-bar" onSubmit={(e) => { e.preventDefault(); send() }}>
            <input
              type="text"
              className="chat-input"
              placeholder="Type a message…"
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onKeyDown={handleKeyDown}
              disabled={loading}
              aria-label="Chat message"
            />
            <button type="submit" className="chat-send" disabled={loading || !message.trim()}>
              Send
            </button>
          </form>
        </aside>
      )}
    </>
  )
}
