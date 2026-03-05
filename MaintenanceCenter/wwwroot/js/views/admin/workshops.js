document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('workshopsTableBody');
    const workshopForm = document.getElementById('workshopForm');
    const workshopModal = new bootstrap.Modal(document.getElementById('workshopModal'));

    const wsIdInput = document.getElementById('WorkshopId');
    const wsNameInput = document.getElementById('WorkshopName');
    const wsDescInput = document.getElementById('WorkshopDesc');

    async function loadWorkshops() {
        try {
            const response = await ApiClient.get('/workshops');
            if (response && response.succeeded) renderTable(response.data);
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="3" class="text-center text-danger">خطأ في التحميل</td></tr>`;
        }
    }

    function renderTable(items) {
        if (items.length === 0) return tableBody.innerHTML = `<tr><td colspan="3" class="text-center">لا توجد ورش مسجلة</td></tr>`;
        tableBody.innerHTML = items.map(item => `
            <tr>
                <td class="fw-bold">${item.name}</td>
                <td>${item.description || '---'}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-primary" onclick="editWs(${item.id}, '${item.name}', '${item.description || ''}')">✏️ تعديل</button>
                    <button class="btn btn-sm btn-outline-danger ms-1" onclick="deleteWs(${item.id})">🗑️ حذف</button>
                </td>
            </tr>
        `).join('');
    }

    window.editWs = function (id, name, desc) {
        wsIdInput.value = id;
        wsNameInput.value = name;
        wsDescInput.value = desc;
        workshopModal.show();
    };

    window.deleteWs = async function (id) {
        if (confirm('هل أنت متأكد من حذف هذه الورشة؟')) {
            try {
                await ApiClient.delete(`/workshops/${id}`);
                Swal.fire('نجاح', 'تم الحذف بنجاح', 'success');
                loadWorkshops();
            } catch (error) { Swal.fire('خطأ', error.message, 'error'); }
        }
    };

    document.getElementById('workshopModal').addEventListener('hidden.bs.modal', () => workshopForm.reset());

    workshopForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const id = wsIdInput.value;
        const data = { name: wsNameInput.value.trim(), description: wsDescInput.value.trim() };

        try {
            if (id) {
                data.id = parseInt(id);
                await ApiClient.put(`/workshops/${id}`, data);
            } else await ApiClient.post('/workshops', data);

            workshopModal.hide();
            Swal.fire('نجاح', 'تم الحفظ بنجاح', 'success');
            loadWorkshops();
        } catch (error) { Swal.fire('خطأ', error.message, 'error'); }
    });

    loadWorkshops();
});