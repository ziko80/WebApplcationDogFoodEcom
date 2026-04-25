import { useEffect, useState } from 'react'
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

export default function Products() {
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [filter, setFilter] = useState<Filter>('all')
  const [search, setSearch] = useState('')

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

  useEffect(() => {
    fetchProducts()
  }, [])

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

  return (
    <div className="app-container">
      <header className="app-header products-header">
        <div>
          <h1 className="app-title">🐾 PawMeds</h1>
          <p className="app-subtitle">Pet medicines, vaccines &amp; accessories</p>
        </div>

      </header>

      <main className="main-content">
        <section className="weather-section">
          <div className="card">
            <div className="section-header">
              <h2 className="section-title">Products</h2>
              <div className="header-actions">
                <input
                  type="search"
                  className="product-search"
                  placeholder="Search products…"
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  aria-label="Search products"
                />
                <fieldset className="toggle-switch" aria-label="Category filter">
                  <legend className="visually-hidden">Category filter</legend>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'all' ? 'active' : ''}`}
                    onClick={() => setFilter('all')}
                  >
                    All
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'medicine' ? 'active' : ''}`}
                    onClick={() => setFilter('medicine')}
                  >
                    Medicine
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'vaccine' ? 'active' : ''}`}
                    onClick={() => setFilter('vaccine')}
                  >
                    Vaccine
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'accessory' ? 'active' : ''}`}
                    onClick={() => setFilter('accessory')}
                  >
                    Accessory
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'toys' ? 'active' : ''}`}
                    onClick={() => setFilter('toys')}
                  >
                    Toys
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'feeding' ? 'active' : ''}`}
                    onClick={() => setFilter('feeding')}
                  >
                    Feeding
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'grooming' ? 'active' : ''}`}
                    onClick={() => setFilter('grooming')}
                  >
                    Grooming
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'hygiene' ? 'active' : ''}`}
                    onClick={() => setFilter('hygiene')}
                  >
                    Hygiene
                  </button>
                  <button
                    type="button"
                    className={`toggle-option ${filter === 'travel' ? 'active' : ''}`}
                    onClick={() => setFilter('travel')}
                  >
                    Travel
                  </button>
                </fieldset>
                <button
                  type="button"
                  className="refresh-button"
                  onClick={fetchProducts}
                  disabled={loading}
                >
                  {loading ? 'Loading…' : 'Refresh'}
                </button>
              </div>
            </div>

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
    </div>
  )
}
