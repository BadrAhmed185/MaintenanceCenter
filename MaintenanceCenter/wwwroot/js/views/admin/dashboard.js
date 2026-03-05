document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('inboxTableBody');
    const assignModal = new bootstrap.Modal(document.getElementById('assignModal'));
    const assignForm = document.getElementById('assignForm');

    // Form Elements
    const requestIdInput = document.getElementById('assignRequestId');
    const trackingCodeLabel = document.getElementById('assignTrackingCode');
    const workshopSelect = document.getElementById('assignWorkshop');
    const technicianSelect = document.getElementById('assignTechnician');
    const assignSubmitBtn = document.getElementById('assignSubmitBtn');

    // State to hold technicians so we can filter them locally without extra API calls
    let allTechnicians = [];

    // 1. Initialize Page
    async function init() {
        await loadInbox();
        await loadCatalogs();
    }

    // 2. Load "Received" Devices (Status = 0)
    async function loadInbox() {
        try {
            // Using our query engine endpoint from Phase A
            const response = await ApiClient.get('/maintenancerequests/filter?status=1');

            if (response && response.succeeded) {
                renderTable(response.data);
            }
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">خطأ في تحميل البيانات</td></tr>`;
            console.error(error);
        }
    }

    // 3. Render the HTML Table
    function renderTable(requests) {
        if (!requests || requests.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="6" class="text-center text-muted py-5">🎉 لا توجد أجهزة جديدة بانتظار التوجيه</td></tr>`;
            return;
        }

        tableBody.innerHTML = requests.map(req => {
            // Format date nicely
            const dateStr = new Date(req.createdAt).toLocaleDateString('ar-EG');

            return `
                <tr>
                    <td class="fw-bold text-mis-blue">${req.trackingCode}</td>
                    <td>${req.deviceName}</td>
                    <td>${req.clientEntityName}</td>
                    <td>${dateStr}</td>
                    <td><span class="badge bg-primary px-3 py-2">🔵 تم الاستلام</span></td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-mis assign-btn" 
                                data-id="${req.id}" 
                                data-code="${req.trackingCode}">
                            ⚙️ توجيه للفحص
                        </button>
                    </td>
                </tr>
            `;
        }).join('');

        // Attach event listeners to the new buttons
        document.querySelectorAll('.assign-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                openAssignModal(this.getAttribute('data-id'), this.getAttribute('data-code'));
            });
        });
    }

    // 4. Load Workshops & Technicians for the Modal Dropdowns
    async function loadCatalogs() {
        try {
            const [wsResponse, techResponse] = await Promise.all([
                ApiClient.get('/workshops'),
                ApiClient.get('/technicians')
            ]);

            if (wsResponse.succeeded) {
                workshopSelect.innerHTML = '<option value="">-- اختر الورشة --</option>' +
                    wsResponse.data.map(w => `<option value="${w.id}">${w.name}</option>`).join('');
            }

            if (techResponse.succeeded) {
                allTechnicians = techResponse.data;
            }
        } catch (error) {
            console.error("Failed to load catalogs", error);
        }
    }

    // 5. Open Modal & Setup State
    function openAssignModal(id, trackingCode) {
        requestIdInput.value = id;
        trackingCodeLabel.textContent = trackingCode;

        // Reset dropdowns
        workshopSelect.value = "";
        technicianSelect.innerHTML = '<option value="">-- يرجى اختيار الورشة أولاً --</option>';
        technicianSelect.disabled = true;

        assignModal.show();
    }

    // 6. Handle Cascading Dropdown (Workshop -> Technician)
    workshopSelect.addEventListener('change', function () {
        const selectedWorkshopId = parseInt(this.value);

        if (!selectedWorkshopId) {
            technicianSelect.innerHTML = '<option value="">-- يرجى اختيار الورشة أولاً --</option>';
            technicianSelect.disabled = true;
            return;
        }

        // Filter the cached technicians by workshop
        const filteredTechs = allTechnicians.filter(t => t.workshopId === selectedWorkshopId);

        if (filteredTechs.length === 0) {
            technicianSelect.innerHTML = '<option value="">-- لا يوجد فنيين مسجلين بهذه الورشة --</option>';
            technicianSelect.disabled = true;
            return;
        }

        technicianSelect.innerHTML = '<option value="">-- اختر الفني --</option>' +
            filteredTechs.map(t => `<option value="${t.id}">${t.displayName}</option>`).join('');

        technicianSelect.disabled = false;
    });

    // 7. Handle Form Submission
    assignForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const requestData = {
            requestId: parseInt(requestIdInput.value),
            workshopId: parseInt(workshopSelect.value),
            technicianId: technicianSelect.value
        };

        const originalBtnText = assignSubmitBtn.innerHTML;
        assignSubmitBtn.disabled = true;
        assignSubmitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الحفظ...';

        try {
            const response = await ApiClient.post('/maintenancerequests/assign', requestData);

            if (response && response.succeeded) {
                assignModal.hide();
                Swal.fire({
                    icon: 'success',
                    title: 'تم التوجيه!',
                    text: 'تم تعيين الجهاز للفني بنجاح وحالته الآن "تحت الفحص".',
                    confirmButtonColor: '#003366'
                });
                // Reload the grid (the assigned device will vanish from the Inbox)
                loadInbox();
            }
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: error.message || 'حدث خطأ أثناء التوجيه.',
                confirmButtonColor: '#003366'
            });
        } finally {
            assignSubmitBtn.disabled = false;
            assignSubmitBtn.innerHTML = originalBtnText;
        }
    });

    // Kickoff
    init();
});