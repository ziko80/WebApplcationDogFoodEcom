const API = {
    products:  '/api/products',
    medicines: '/api/products/medicines',
    vaccines:  '/api/products/vaccines',
    cart:      '/api/cart',
    orders:    '/api/orders'
};

// ── State ──────────────────────────────────────────
let currentFilter = 'all';

// ── DOM refs ───────────────────────────────────────
const $ = (s) => document.querySelector(s);
const productsGrid   = $('#productsGrid');
const productCount   = $('#productCount');
const sectionTitle   = $('#sectionTitle');
const cartBadge      = $('#cartBadge');
const cartItems      = $('#cartItems');
const cartTotal      = $('#cartTotal');
const cartSidebar    = $('#cartSidebar');
const cartOverlay    = $('#cartOverlay');
const checkoutBtn    = $('#checkoutBtn');
const checkoutModal  = $('#checkoutModal');
const checkoutForm   = $('#checkoutForm');
const checkoutSummary = $('#checkoutSummary');
const productsSection = $('#productsSection');
const ordersSection   = $('#ordersSection');
const ordersList      = $('#ordersList');
const heroBanner      = $('#heroBanner');
const searchInput     = $('#searchInput');

// ── Helpers ────────────────────────────────────────
const fmt = (n) => `$${Number(n).toFixed(2)}`;

async function api(url, opts) {
    const res = await fetch(url, opts);
    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || res.statusText);
    }
    return res.status === 204 ? null : res.json();
}

function showToast(msg, type = '') {
    const t = $('#toast');
    t.textContent = msg;
    t.className = `toast ${type}`;
    t.classList.remove('hidden');
    setTimeout(() => t.classList.add('hidden'), 2500);
}

// ── Products ───────────────────────────────────────
async function loadProducts(filter, search) {
    let url = API.products;
    if (filter === 'medicines') url = API.medicines;
    else if (filter === 'vaccines') url = API.vaccines;

    if (search && filter !== 'medicines' && filter !== 'vaccines') {
        url += `?search=${encodeURIComponent(search)}`;
    }

    const products = await api(url);
    const filtered = search && (filter === 'medicines' || filter === 'vaccines')
        ? products.filter(p =>
            p.name.toLowerCase().includes(search.toLowerCase()) ||
            p.description.toLowerCase().includes(search.toLowerCase()) ||
            p.targetCondition.toLowerCase().includes(search.toLowerCase()))
        : products;

    renderProducts(filtered);
}

function renderProducts(products) {
    productCount.textContent = `${products.length} items`;

    if (!products.length) {
        productsGrid.innerHTML = `<div class="orders-empty"><p>No products found.</p></div>`;
        return;
    }

    productsGrid.innerHTML = products.map(p => {
        const catClass = p.category === 0 ? 'medicine' : 'vaccine';
        const catLabel = p.category === 0 ? 'Medicine' : 'Vaccine';
        const stockClass = p.stockQuantity < 20 ? 'low' : '';
        return `
        <div class="product-card">
            <div class="card-badge-row">
                <span class="badge badge-${catClass}">${catLabel}</span>
            </div>
            <div class="card-body">
                <h3>${p.name}</h3>
                <div class="brand">${p.brand}</div>
                <div class="description">${p.description}</div>
                <div class="card-meta">
                    <span class="meta-tag">💊 ${p.dosageInfo}</span>
                    <span class="meta-tag">🎯 ${p.targetCondition}</span>
                </div>
                <div class="card-footer">
                    <div>
                        <div class="price">${fmt(p.price)}</div>
                        <div class="stock ${stockClass}">${p.stockQuantity > 0 ? p.stockQuantity + ' in stock' : 'Out of stock'}</div>
                    </div>
                    <button class="btn-add" onclick="addToCart(${p.id})" ${p.stockQuantity === 0 ? 'disabled' : ''}>Add to Cart</button>
                </div>
            </div>
        </div>`;
    }).join('');
}

// ── Cart ───────────────────────────────────────────
async function refreshCart() {
    const data = await api(API.cart);
    renderCart(data);
    return data;
}

function renderCart(data) {
    cartBadge.textContent = data.itemCount;
    cartTotal.textContent = fmt(data.totalAmount);
    checkoutBtn.disabled = data.items.length === 0;

    if (!data.items.length) {
        cartItems.innerHTML = `<div class="cart-empty"><div class="cart-empty-icon">🛒</div><p>Your cart is empty</p></div>`;
        return;
    }

    cartItems.innerHTML = data.items.map(i => `
        <div class="cart-item">
            <div class="cart-item-info">
                <h4>${i.productName}</h4>
                <div class="cart-item-price">${fmt(i.price)} each</div>
            </div>
            <div class="cart-item-qty">
                <button class="qty-btn" onclick="updateQty(${i.productId}, -1)">−</button>
                <span>${i.quantity}</span>
                <button class="qty-btn" onclick="updateQty(${i.productId}, 1)">+</button>
            </div>
            <div class="cart-item-total">${fmt(i.total)}</div>
            <button class="cart-item-remove" onclick="removeFromCart(${i.productId})">🗑</button>
        </div>
    `).join('');
}

async function addToCart(productId) {
    try {
        await api(API.cart, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: 1 })
        });
        await refreshCart();
        showToast('Added to cart!', 'success');
    } catch (e) {
        showToast(e.message, 'error');
    }
}

async function updateQty(productId, delta) {
    const data = await api(API.cart);
    const item = data.items.find(i => i.productId === productId);
    if (!item) return;

    const newQty = item.quantity + delta;
    if (newQty <= 0) {
        await removeFromCart(productId);
        return;
    }

    // Remove then re-add with correct quantity
    await api(`${API.cart}/${productId}`, { method: 'DELETE' });
    await api(API.cart, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId, quantity: newQty })
    });
    await refreshCart();
}

async function removeFromCart(productId) {
    await api(`${API.cart}/${productId}`, { method: 'DELETE' });
    await refreshCart();
    showToast('Removed from cart');
}

async function clearCart() {
    await api(API.cart, { method: 'DELETE' });
    await refreshCart();
}

// ── Orders ─────────────────────────────────────────
async function loadOrders() {
    const orders = await api(API.orders);

    if (!orders.length) {
        ordersList.innerHTML = `<div class="orders-empty"><p>No orders yet. Start shopping! 🐾</p></div>`;
        return;
    }

    ordersList.innerHTML = orders.map(o => {
        const statusClass = o.status === 0 ? 'pending' : 'confirmed';
        const statusLabel = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'][o.status];
        return `
        <div class="order-card">
            <div class="order-card-header">
                <h3>Order #${o.id} — ${new Date(o.orderDate).toLocaleDateString()}</h3>
                <span class="order-status status-${statusClass}">${statusLabel}</span>
            </div>
            <table class="order-items-table">
                <thead><tr><th>Product</th><th>Qty</th><th>Price</th><th>Total</th></tr></thead>
                <tbody>
                    ${o.items.map(i => `<tr><td>${i.productName}</td><td>${i.quantity}</td><td>${fmt(i.price)}</td><td>${fmt(i.total)}</td></tr>`).join('')}
                    <tr class="order-total-row"><td colspan="3">Order Total</td><td>${fmt(o.totalAmount)}</td></tr>
                </tbody>
            </table>
            <div class="order-actions">
                <button class="btn-invoice" onclick="downloadInvoice(${o.id})">📄 Download Invoice</button>
                <button class="btn-email" id="emailBtn-${o.id}" onclick="sendInvoiceEmail(${o.id})">📧 Email Invoice</button>
            </div>
        </div>`;
    }).join('');
}

// ── Checkout ───────────────────────────────────────
async function openCheckout() {
    const data = await refreshCart();
    if (!data.items.length) return;

    checkoutSummary.innerHTML = `
        ${data.items.map(i => `<div class="line"><span>${i.productName} × ${i.quantity}</span><span>${fmt(i.total)}</span></div>`).join('')}
        <div class="line total"><span>Total</span><span>${fmt(data.totalAmount)}</span></div>`;

    toggleCart(false);
    checkoutModal.classList.remove('hidden');
}

checkoutForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const cartData = await api(API.cart);

    const order = {
        customerName: $('#customerName').value,
        customerEmail: $('#customerEmail').value,
        shippingAddress: $('#shippingAddress').value,
        items: cartData.items.map(i => ({ productId: i.productId, quantity: i.quantity }))
    };

    try {
        await api(API.orders, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(order)
        });
        await clearCart();
        checkoutModal.classList.add('hidden');
        checkoutForm.reset();
        showToast('Order placed successfully! 🎉', 'success');
        await loadProducts(currentFilter);
    } catch (e) {
        showToast(e.message, 'error');
    }
});

// ── Navigation ─────────────────────────────────────
function toggleCart(show) {
    cartSidebar.classList.toggle('hidden', !show);
    cartOverlay.classList.toggle('hidden', !show);
}

document.querySelectorAll('.nav-link').forEach(link => {
    link.addEventListener('click', async (e) => {
        e.preventDefault();
        document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
        link.classList.add('active');

        const filter = link.dataset.filter;
        currentFilter = filter;

        if (filter === 'orders') {
            productsSection.classList.add('hidden');
            heroBanner.classList.add('hidden');
            ordersSection.classList.remove('hidden');
            await loadOrders();
        } else {
            ordersSection.classList.add('hidden');
            heroBanner.classList.remove('hidden');
            productsSection.classList.remove('hidden');
            sectionTitle.textContent = filter === 'all' ? 'All Products' : filter === 'medicines' ? 'Medicines' : 'Vaccines';
            await loadProducts(filter, searchInput.value);
        }
    });
});

$('#cartToggle').addEventListener('click', () => { toggleCart(true); refreshCart(); });
$('#cartClose').addEventListener('click', () => toggleCart(false));
$('#cartOverlay').addEventListener('click', () => toggleCart(false));
$('#clearCartBtn').addEventListener('click', clearCart);
$('#checkoutBtn').addEventListener('click', openCheckout);
$('#modalClose').addEventListener('click', () => checkoutModal.classList.add('hidden'));

let searchTimer;
searchInput.addEventListener('input', () => {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(() => {
        if (currentFilter !== 'orders') loadProducts(currentFilter, searchInput.value);
    }, 300);
});

// ── Billing ────────────────────────────────────────
async function downloadInvoice(orderId) {
    try {
        const res = await fetch(`/api/billing/invoice/${orderId}`);
        if (!res.ok) throw new Error('Failed to generate invoice');
        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `PawMeds_Invoice_${String(orderId).padStart(5, '0')}.pdf`;
        document.body.appendChild(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(url);
        showToast('Invoice downloaded!', 'success');
    } catch (e) {
        showToast(e.message, 'error');
    }
}

async function sendInvoiceEmail(orderId) {
    const btn = $(`#emailBtn-${orderId}`);
    if (btn) { btn.disabled = true; btn.textContent = '⏳ Sending...'; }
    try {
        const data = await api(`/api/billing/send-invoice/${orderId}`, { method: 'POST' });
        showToast(data.message, 'success');
        if (btn) { btn.textContent = '✅ Sent!'; }
    } catch (e) {
        showToast(e.message || 'Failed to send email', 'error');
        if (btn) { btn.disabled = false; btn.textContent = '📧 Email Invoice'; }
    }
}

// ── Init ───────────────────────────────────────────
(async () => {
    await loadProducts('all');
    await refreshCart();
})();
