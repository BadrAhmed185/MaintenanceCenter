document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const form = document.getElementById('filterForm');
    const tableBody = document.getElementById('monitoringTableBody');
    const workshopSelect = document.getElementById('WorkshopId');
    const technicianSelect = document.getElementById('TechnicianId');
    const searchInput = document.getElementById('SearchTerm');

    // Pagination Elements
    const prevBtn = document.getElementById('prevPageBtn');
    const nextBtn = document.getElementById('nextPageBtn');
    const pageInfo = document.getElementById('pageInfo');

    // State Variables
    let allTechnicians = [];
    let currentPage = 1;
    const pageSize = 20;

    // --- Utility: Debounce Function to prevent API spamming ---
    function debounce(func, delay) {
        let timeoutId;
        return function (...args) {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(() => {
                func.apply(this, args);
            }, delay);
        };
    }

    // 1. Initialization
    async function init() {
        await loadCatalogs();
        await fetchFilteredData();
        setupAutoFilterListeners(); // Initialize the auto-listeners
    }

    // 2. Load Lookups
    async function loadCatalogs() {
        try {
            const [wsResponse, techResponse] = await Promise.all([
                ApiClient.get('/workshops'),
                ApiClient.get('/technicians')
            ]);

            if (wsResponse.succeeded) {
                workshopSelect.innerHTML += wsResponse.data.map(w => `<option value="${w.id}">${w.name}</option>`).join('');
            }
            if (techResponse.succeeded) {
                allTechnicians = techResponse.data;
            }
        } catch (error) {
            console.error("Failed to load catalogs", error);
        }
    }

    // 3. Cascading Dropdown Logic
    workshopSelect.addEventListener('change', function () {
        const selectedWorkshopId = parseInt(this.value);

        if (!selectedWorkshopId) {
            technicianSelect.innerHTML = '<option value="">-- كل الفنيين --</option>';
            technicianSelect.disabled = true;
        } else {
            const filteredTechs = allTechnicians.filter(t => t.workshopId === selectedWorkshopId);
            technicianSelect.innerHTML = '<option value="">-- كل الفنيين في هذه الورشة --</option>' +
                filteredTechs.map(t => `<option value="${t.id}">${t.displayName}</option>`).join('');
            technicianSelect.disabled = false;
        }

        // Reset tech value so it doesn't accidentally filter by a tech from a previous workshop
        technicianSelect.value = '';
    });

    // --- 4. THE NEW UX: Auto-Filter Listeners ---
    function setupAutoFilterListeners() {
        // Debounce text input (wait 500ms after user stops typing)
        searchInput.addEventListener('input', debounce(() => {
            currentPage = 1;
            fetchFilteredData();
        }, 500));

        // Instant fetch for Dropdowns and Dates
        const instantInputs = ['Status', 'WorkshopId', 'TechnicianId', 'StartDate', 'EndDate'];
        instantInputs.forEach(id => {
            document.getElementById(id).addEventListener('change', () => {
                currentPage = 1;
                fetchFilteredData();
            });
        });
    }

    // 5. Manual Form Submission (Maintains the "Apply Filters" button functionality)
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        currentPage = 1;
        fetchFilteredData();
    });

    // 6. Form Reset
    document.getElementById('resetBtn').addEventListener('click', function () {
        setTimeout(() => {
            technicianSelect.innerHTML = '<option value="">-- اختر الورشة أولاً --</option>';
            technicianSelect.disabled = true;
            currentPage = 1;
            fetchFilteredData(); // Auto-fetch immediately after reset
        }, 10);
    });

    // 7. Pagination Controls
    prevBtn.addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            fetchFilteredData();
        }
    });

    nextBtn.addEventListener('click', () => {
        currentPage++;
        fetchFilteredData();
    });

    // 8. Core Fetch Logic
    async function fetchFilteredData() {
        tableBody.innerHTML = `<tr><td colspan="6" class="text-center py-5"><span class="spinner-border spinner-border-sm text-mis-blue"></span> جاري التحميل...</td></tr>`;

        const params = new URLSearchParams();
        params.append('PageNumber', currentPage);
        params.append('PageSize', pageSize);

        const searchTerm = searchInput.value.trim();
        const status = document.getElementById('Status').value;
        const workshopId = workshopSelect.value;
        const techId = technicianSelect.value;
        const startDate = document.getElementById('StartDate').value;
        const endDate = document.getElementById('EndDate').value;

        if (searchTerm) params.append('SearchTerm', searchTerm);
        if (status !== "") params.append('Status', status);
        if (workshopId) params.append('WorkshopId', workshopId);
        if (techId) params.append('TechnicianId', techId);
        if (startDate) params.append('StartDate', startDate);
        if (endDate) params.append('EndDate', endDate);

        try {
            const response = await ApiClient.get(`/maintenancerequests/filter?${params.toString()}`);

            if (response && response.succeeded) {
                renderTable(response.data);
                updatePaginationState(response.data.length);
            }
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">حدث خطأ أثناء تحميل البيانات</td></tr>`;
        }
    }

    // 9. Render Table
    function renderTable(requests) {
        if (!requests || requests.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="6" class="text-center text-muted py-5">لا توجد بيانات تطابق محددات البحث</td></tr>`;
            return;
        }

        tableBody.innerHTML = requests.map(req => {
            const dateStr = new Date(req.createdAt).toLocaleDateString('ar-EG');
            const statusBadge = getStatusBadge(req.status);
            const costStr = req.totalCost > 0 ? `<span class="fw-bold text-success">${req.totalCost.toLocaleString('ar-EG')}</span>` : `<span class="text-muted">--</span>`;

            return `
                <tr>
                    <td class="fw-bold text-mis-blue">${req.trackingCode}</td>
                    <td>
                        <div class="fw-bold">${req.deviceName}</div>
                        <div class="small text-muted">${req.clientEntityName}</div>
                    </td>
                    <td>
                        <div>${req.workshopName || '<span class="text-warning small">غير موجه</span>'}</div>
                        <div class="small text-muted">${req.technicianName || '---'}</div>
                    </td>
                    <td>${dateStr}</td>
                    <td>${statusBadge}</td>
                    <td class="text-center">${costStr}</td>
                </tr>
            `;
        }).join('');
    }

    // 10. Status Badges
    function getStatusBadge(statusInt) {
        switch (statusInt) {
            case 0: return `<span class="badge bg-primary px-3 py-2">تم الاستلام</span>`;
            case 1: return `<span class="badge bg-warning text-dark px-3 py-2">تحت الفحص</span>`;
            case 2: return `<span class="badge" style="background-color: #6f42c1; color: white;">المقايسة جاهزة</span>`;
            case 3: return `<span class="badge bg-info text-dark px-3 py-2">جاري التصليح</span>`;
            case 4: return `<span class="badge bg-success px-3 py-2">جاهز للاستلام</span>`;
            case 5: return `<span class="badge bg-secondary px-3 py-2">تم التسليم</span>`;
            default: return `<span class="badge bg-dark px-3 py-2">مجهول</span>`;
        }
    }

    // 11. Pagination State
    function updatePaginationState(currentDataLength) {
        pageInfo.textContent = `الصفحة ${currentPage}`;
        prevBtn.disabled = (currentPage === 1);
        nextBtn.disabled = (currentDataLength < pageSize);
    }

    // Kickoff
    init();
});