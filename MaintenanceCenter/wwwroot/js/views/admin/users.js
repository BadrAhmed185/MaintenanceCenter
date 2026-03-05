document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('usersTableBody');
    const userForm = document.getElementById('userForm');
    const userModal = new bootstrap.Modal(document.getElementById('userModal'));
    const roleSelect = document.getElementById('Role');
    const workshopContainer = document.getElementById('workshopContainer');
    const workshopSelect = document.getElementById('WorkshopId');

    async function init() {
        await loadUsers();
        await loadWorkshops();
    }

    async function loadUsers() {
        try {
            const response = await ApiClient.get('/auth/users');
            if (response && response.succeeded) {
                renderTable(response.data);
            }
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">خطأ في التحميل</td></tr>`;
        }
    }

    async function loadWorkshops() {
        const response = await ApiClient.get('/workshops');
        if (response && response.succeeded) {
            workshopSelect.innerHTML += response.data.map(w => `<option value="${w.id}">${w.name}</option>`).join('');
        }
    }

    function renderTable(users) {
        if (users.length === 0) return tableBody.innerHTML = `<tr><td colspan="5" class="text-center">لا يوجد مستخدمين</td></tr>`;

        tableBody.innerHTML = users.map(u => `
            <tr>
                <td class="fw-bold">${u.displayName}</td>
                <td>${u.userName}</td>
                <td><span class="badge bg-secondary">${u.role}</span></td>
                <td>${u.workshopName}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteUser('${u.id}')">🗑️ إيقاف</button>
                </td>
            </tr>
        `).join('');
    }

    // Dynamic UI Toggle
    roleSelect.addEventListener('change', function () {
        if (this.value === 'Technician') {
            workshopContainer.classList.remove('d-none');
            workshopSelect.setAttribute('required', 'required');
        } else {
            workshopContainer.classList.add('d-none');
            workshopSelect.removeAttribute('required');
            workshopSelect.value = '';
        }
    });

    userForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const role = roleSelect.value;
        const btn = document.getElementById('saveUserBtn');
        btn.disabled = true;

        try {
            let response;
            // Architectural routing based on role specifics
            if (role === 'Technician') {
                const data = {
                    displayName: document.getElementById('DisplayName').value,
                    userName: document.getElementById('UserName').value,
                    password: document.getElementById('Password').value,
                    workshopId: parseInt(workshopSelect.value)
                };
                response = await ApiClient.post('/technicians', data); // Uses Technicians API
            } else {
                const data = {
                    displayName: document.getElementById('DisplayName').value,
                    userName: document.getElementById('UserName').value,
                    password: document.getElementById('Password').value,
                    role: role
                };
                response = await ApiClient.post('/auth/register', data); // Uses generic Auth API
            }

            if (response && response.succeeded) {
                userModal.hide();
                userForm.reset();
                Swal.fire('نجاح', 'تم إضافة المستخدم بنجاح', 'success');
                loadUsers();
            }
        } catch (error) {
            Swal.fire('خطأ', error.message || 'حدث خطأ', 'error');
        } finally {
            btn.disabled = false;
        }
    });

    init();
});