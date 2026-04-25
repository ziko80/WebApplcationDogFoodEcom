import { useEffect, useState, useRef } from 'react'
import './pawmeds.css'

interface Product {
  id: number
  name: string
  description: string
  price: number
  category: number // 0=Medicine, 1=Vaccine, 2=Accessory, 3=Toys, 4=Feeding, 5=Grooming, 6=Hygiene, 7=Travel
  brand: string
  imageUrl: string
  stockQuantity: number
  dosageInfo: string
  targetCondition: string
}

type Filter = 'all' | 'medicine' | 'vaccine' | 'accessory' | 'toys' | 'feeding' | 'grooming' | 'hygiene' | 'travel'

const accessoryKeys: Filter[] = ['toys', 'feeding', 'grooming', 'hygiene', 'travel']

export default function Products() {
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [filter, setFilter] = useState<Filter>('all')
  const [search, setSearch] = useState('')
  const [accOpen, setAccOpen] = useState(false)
  const [page, setPage] = useState<'products' | 'orders'>('products')
  const ddRef = useRef<HTMLDivElement>(null)

  const fetchProducts = async () => {
    setLoading(true)
    setError(null)
    try {
      const res = await fetch('/api/products')
      if (!res.ok) throw new Error(`HTTP ${res.status}`)
      const data: Product[] = await res.json()
      setProducts(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load products')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchProducts() }, [])

  // close dropdown on outside click
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ddRef.current && !ddRef.current.contains(e.target as Node)) setAccOpen(false)
    }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  const isAccFilter = accessoryKeys.includes(filter)

  const filtered = products.filter((p) => {
    const categoryMap: Record<Exclude<Filter, 'all'>, number> = {
      medicine: 0, vaccine: 1, accessory: 2, toys: 3, feeding: 4, grooming: 5, hygiene: 6, travel: 7
    }
    if (filter !== 'all' && p.category !== categoryMap[filter]) return false
    if (search.trim()) {
      const q = search.trim().toLowerCase()
      return (
        p.name.toLowerCase().includes(q) ||
        p.description.toLowerCase().includes(q) ||
        p.targetCondition.toLowerCase().includes(q)
      )
    }
    return true
  })

  const categoryLabels: Record<number, string> = {
    0: 'Medicine', 1: 'Vaccine', 2: 'Accessory', 3: 'Toys', 4: 'Feeding', 5: 'Grooming', 6: 'Hygiene', 7: 'Travel'
  }
  const categoryLabel = (c: number) => categoryLabels[c] ?? 'Other'

  const navClick = (f: Filter) => { setFilter(f); setAccOpen(false); setPage('products') }

  return (
    <div className="app-container">
      {/* ── Top navbar ── */}
      <nav className="pm-navbar">
        <span className="pm-navbar-brand">🐾 PawMeds</span>
        <div className="pm-navbar-links">
          <button type="button" className={`pm-nav-link ${filter === 'all' && page === 'products' ? 'pm-nav-active' : ''}`} onClick={() => navClick('all')}>All Products</button>
          <button type="button" className={`pm-nav-link ${filter === 'medicine' ? 'pm-nav-active' : ''}`} onClick={() => navClick('medicine')}>Medicines</button>
          <button type="button" className={`pm-nav-link ${filter === 'vaccine' ? 'pm-nav-active' : ''}`} onClick={() => navClick('vaccine')}>Vaccines</button>

          <div className="pm-nav-dropdown" ref={ddRef}>
            <button type="button" className={`pm-nav-link ${isAccFilter ? 'pm-nav-active' : ''}`} onClick={() => setAccOpen(!accOpen)}>
              Accessories ▾
            </button>
            {accOpen && (
              <div className="pm-dropdown-menu">
                {accessoryKeys.map((k) => (
                  <button key={k} type="button" className={`pm-dropdown-item ${filter === k ? 'pm-dropdown-active' : ''}`} onClick={() => navClick(k)}>
                    {k.charAt(0).toUpperCase() + k.slice(1)}
                  </button>
                ))}
              </div>
            )}
          </div>

          <button type="button" className={`pm-nav-link ${page === 'orders' ? 'pm-nav-active' : ''}`} onClick={() => { setPage('orders'); setAccOpen(false) }}>My Orders</button>
        </div>

        <div className="pm-navbar-right">
          <input
            type="search"
            className="pm-navbar-search"
            placeholder="Search products..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            aria-label="Search products"
          />
          <span className="pm-cart-icon" title="Cart">🛒<sup className="pm-cart-badge">0</sup></span>
        </div>
      </nav>

      {/* ── Hero banner ── */}
      <section className="pm-hero">
        <h2>Premium Medicine, Vaccines &amp; Accessories for Your Pets</h2>
        <p>Trusted brands. Vet-recommended. Fast delivery to your door.</p>
      </section>

      {/* ── Orders page ── */}
      {page === 'orders' && (
        <main className="main-content">
          <section className="weather-section">
            <div className="card">
              <h2 className="section-title">My Orders</h2>
              <p className="empty-state">You have no orders yet.</p>
            </div>
          </section>
        </main>
      )}

      {/* ── Products page ── */}
      {page === 'products' && (
        <main className="main-content">
          <section className="weather-section">
            <div className="card">
              {error && (
                <div className="error-message" role="alert">
                  <span>⚠️ {error}</span>
                </div>
              )}

              {loading && products.length === 0 && (
                <div className="loading-skeleton" role="status">
                  {[...Array(6)].map((_, i) => (
                    <div key={i} className="skeleton-row" />
                  ))}
                </div>
              )}

              {!loading && filtered.length === 0 && !error && (
                <p className="empty-state">No products match your filters.</p>
              )}

              {filtered.length > 0 && (
                <div className="product-grid">
                  {filtered.map((p) => (
                    <article key={p.id} className="product-card">
                      <div className="product-card-head">
                        <span className={`badge badge-${({0:'med',1:'vac',2:'acc',3:'toy',4:'feed',5:'groom',6:'hyg',7:'travel'} as Record<number,string>)[p.category] ?? 'acc'}`}>
                          {categoryLabel(p.category)}
                        </span>
                        <span className="product-brand">{p.brand}</span>
                      </div>
                      <h3 className="product-name">{p.name}</h3>
                      <p className="product-desc">{p.description}</p>
                      <dl className="product-meta">
                        <div>
                          <dt>Target</dt>
                          <dd>{p.targetCondition}</dd>
                        </div>
                        <div>
                          <dt>Dosage</dt>
                          <dd>{p.dosageInfo}</dd>
                        </div>
                        <div>
                          <dt>In stock</dt>
                          <dd>{p.stockQuantity}</dd>
                        </div>
                      </dl>
                      <div className="product-footer">
                        <span className="product-price">${p.price.toFixed(2)}</span>
                        <button type="button" className="add-btn" disabled={p.stockQuantity === 0}>
                          {p.stockQuantity === 0 ? 'Out of stock' : 'Add to cart'}
                        </button>
                      </div>
                    </article>
                  ))}
                </div>
              )}
            </div>
          </section>
        </main>
      )}
    </div>
  )
}
