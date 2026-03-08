document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('techTableBody');
    const searchInput = document.getElementById('SearchTerm');
    const statusFilter = document.getElementById('StatusFilter');

    // Modal Elements
    const inspectionModal = new bootstrap.Modal(document.getElementById('inspectionModal'));
    const form = document.getElementById('inspectionForm');
    const pricingSection = document.getElementById('pricingSection');

    // Catalogs & Calculation
    let catalogs = { parts: [], services: [] };
    let selectedParts = []; // Array of { id, name, qty, price }
    let selectedServices = []; // Array of { id, name, price }

    // Utility: Debounce
    function debounce(func, delay) {
        let timeoutId;
        return function (...args) {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(() => { func.apply(this, args); }, delay);
        };
    }

    // 1. Init & Load Data
    async function init() {
        await fetchMyRequests();
        await loadCatalogs();

        // Auto-filter listeners
        searchInput.addEventListener('input', debounce(fetchMyRequests, 500));
        statusFilter.addEventListener('change', fetchMyRequests);
    }

    async function loadCatalogs() {
        const [partsRes, servicesRes] = await Promise.all([
            ApiClient.get('/spareparts'),
            ApiClient.get('/maintenanceservices')
        ]);

        if (partsRes.succeeded) {
            catalogs.parts = partsRes.data;
            document.getElementById('PartSelect').innerHTML += partsRes.data.map(p => `<option value="${p.id}" data-price="${p.currentCost}">${p.name} (${p.currentCost} ج.م)</option>`).join('');
        }
        if (servicesRes.succeeded) {
            catalogs.services = servicesRes.data;
            document.getElementById('ServiceSelect').innerHTML += servicesRes.data.map(s => `<option value="${s.id}" data-price="${s.currentCost}">${s.name} (${s.currentCost} ج.م)</option>`).join('');
        }
    }

    async function fetchMyRequests() {
        tableBody.innerHTML = `<tr><td colspan="5" class="text-center py-4"><span class="spinner-border spinner-border-sm"></span></td></tr>`;
        const params = new URLSearchParams({ PageNumber: 1, PageSize: 50 });

        if (searchInput.value) params.append('SearchTerm', searchInput.value);
        if (statusFilter.value) params.append('Status', statusFilter.value);

        try {
            const response = await ApiClient.get(`/maintenancerequests/filter?${params.toString()}`);
            if (response && response.succeeded) renderTable(response.data);
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">خطأ في التحميل</td></tr>`;
        }
    }

    function renderTable(requests) {
        if (!requests || requests.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="5" class="text-center text-muted py-4">لا توجد أجهزة مسندة إليك حالياً.</td></tr>`;
            return;
        }

        tableBody.innerHTML = requests.map(req => {
            const dateStr = new Date(req.createdAt).toLocaleDateString('ar-EG');
            let badge = getStatusBadge(req.status); 

            // Stringify req so we can pass it to the modal
            const reqJson = encodeURIComponent(JSON.stringify(req));


            return `
                <tr>
                    <td class="fw-bold">${req.trackingCode}</td>
                    <td>${req.deviceName}</td>
                    <td>${dateStr}</td>
                    <td>${badge}</td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-outline-primary" onclick="openInspection('${reqJson}')">📝 فحص / تعديل</button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    // 2. Open Modal & Bind Existing Data (If Any)
    window.openInspection = function (reqJsonEncoded) {
        const req = JSON.parse(decodeURIComponent(reqJsonEncoded));

        // Reset state
        selectedParts = [];
        selectedServices = [];
        form.reset();

        // Populate basic info
        document.getElementById('RequestId').value = req.id;
        document.getElementById('displayCode').textContent = req.trackingCode;
        document.getElementById('displayFault').textContent = req.faultDescription;

        // Populate existing data if it's an edit
        document.getElementById('TechnicalReport').value = req.technicalReport || '';
        document.getElementById('NextStatus').value = req.status > 0 ? req.status : 1;
        document.getElementById('TotalCostOverride').value = req.totalCost || 0;

        // If you passed the existing Parts/Services in the DTO, you would populate `selectedParts` here.
        // For this prototype, we'll assume they just add new ones or overwrite.

        updateListsUI();
        inspectionModal.show();
    };

    // 3. UI Interactions (Adding Parts/Services to local arrays)
    document.getElementById('NextStatus').addEventListener('change', function () {
        // If unrepairable (Status 6), hide pricing section
        pricingSection.style.display = this.value == "6" ? "none" : "block";
        if (this.value == "6") document.getElementById('TotalCostOverride').value = 0;
    });

    document.getElementById('addPartBtn').addEventListener('click', () => {
        const select = document.getElementById('PartSelect');
        const qty = parseInt(document.getElementById('PartQty').value);
        if (!select.value || qty < 1) return;

        const price = parseFloat(select.options[select.selectedIndex].getAttribute('data-price'));
        selectedParts.push({ id: parseInt(select.value), name: select.options[select.selectedIndex].text, qty: qty, price: price });
        updateListsUI();
    });

    document.getElementById('addServiceBtn').addEventListener('click', () => {
        const select = document.getElementById('ServiceSelect');
        if (!select.value) return;

        const price = parseFloat(select.options[select.selectedIndex].getAttribute('data-price'));
        selectedServices.push({ id: parseInt(select.value), name: select.options[select.selectedIndex].text, price: price });
        updateListsUI();
    });

    window.removePart = (index) => { selectedParts.splice(index, 1); updateListsUI(); };
    window.removeService = (index) => { selectedServices.splice(index, 1); updateListsUI(); };

    // 4. Real-time Calculation
    function updateListsUI() {
        document.getElementById('selectedPartsList').innerHTML = selectedParts.map((p, i) => `
            <li class="list-group-item d-flex justify-content-between align-items-center p-2">
                ${p.name} (العدد: ${p.qty}) <button type="button" class="btn btn-sm text-danger p-0" onclick="removePart(${i})">✖</button>
            </li>
        `).join('');

        document.getElementById('selectedServicesList').innerHTML = selectedServices.map((s, i) => `
            <li class="list-group-item d-flex justify-content-between align-items-center p-2">
                ${s.name} <button type="button" class="btn btn-sm text-danger p-0" onclick="removeService(${i})">✖</button>
            </li>
        `).join('');

        // Calculate sum
        const partSum = selectedParts.reduce((sum, p) => sum + (p.price * p.qty), 0);
        const serviceSum = selectedServices.reduce((sum, s) => sum + s.price, 0);
        document.getElementById('calculatedSum').textContent = partSum + serviceSum;
    }

    document.getElementById('copySumBtn').addEventListener('click', () => {
        document.getElementById('TotalCostOverride').value = document.getElementById('calculatedSum').textContent;
    });

    // 5. Submit Form
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const statusVal = document.getElementById('NextStatus').value;
        const data = {
            requestId: parseInt(document.getElementById('RequestId').value),
            technicalReport: document.getElementById('TechnicalReport').value,
            status: parseInt(statusVal),
            isRepairable: statusVal != "6",
            totalCost: parseFloat(document.getElementById('TotalCostOverride').value),
            selectedParts: selectedParts.map(p => ({ sparePartId: p.id, quantity: p.qty })),
            selectedServices: selectedServices.map(s => s.id)
        };

        const btn = document.getElementById('saveBtn');
        btn.disabled = true;

        try {
            const res = await ApiClient.post('/maintenancerequests/inspect', data);
            if (res && res.succeeded) {
                inspectionModal.hide();
                Swal.fire('نجاح', 'تم حفظ التقرير والمقايسة بنجاح', 'success');
                fetchMyRequests(); // Refresh grid
            }
        } catch (error) {
            Swal.fire('خطأ', error.message, 'error');
        } finally {
            btn.disabled = false;
        }
    });

    function getStatusBadge(statusInt) {
        switch (statusInt) {
            case 1: // Received
                return `<span class="badge bg-primary px-3 py-2">تم الاستلام</span>`;
            case 2: // UnderInspection
                return `<span class="badge bg-warning text-dark px-3 py-2">تحت الفحص</span>`;
            case 3: // QuotationReady
                return `<span class="badge" style="background-color: #6f42c1; color: white;">المقايسة جاهزة</span>`;
            case 4: // QuotationRejected
                return `<span class="badge bg-danger px-3 py-2">رفض المقايسة</span>`;
            case 5: // Unrepairable
                return `<span class="badge bg-dark px-3 py-2">غير قابل للإصلاح</span>`;
            case 6: // UnderRepair
                return `<span class="badge bg-info text-dark px-3 py-2">جاري التصليح</span>`;
            case 7: // ReadyForDelivery
                return `<span class="badge bg-success px-3 py-2">جاهز للاستلام</span>`;
            case 8: // Delivered
                return `<span class="badge bg-secondary px-3 py-2">تم التسليم</span>`;
            default:
                return `<span class="badge bg-secondary px-3 py-2">مجهول</span>`;
        }
    }


    init();
});