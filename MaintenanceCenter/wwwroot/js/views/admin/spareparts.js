document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('catalogTableBody');
    const catalogForm = document.getElementById('catalogForm');
    const catalogModal = new bootstrap.Modal(document.getElementById('catalogModal'));

    const itemIdInput = document.getElementById('ItemId');
    const itemNameInput = document.getElementById('ItemName');
    const itemCostInput = document.getElementById('ItemCost');

    async function loadItems() {
        try {
            const response = await ApiClient.get('/spareparts');
            if (response && response.succeeded) {
                renderTable(response.data);
            }
        } catch (error) {
            tableBody.innerHTML = `<tr><td colspan="3" class="text-center text-danger">خطأ في التحميل</td></tr>`;
        }
    }

    function renderTable(items) {
        if (items.length === 0) return tableBody.innerHTML = `<tr><td colspan="3" class="text-center">لا توجد بيانات</td></tr>`;

        tableBody.innerHTML = items.map(item => `
            <tr>
                <td class="fw-bold">${item.name}</td>
                <td>${item.currentCost} ج.م</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-primary" onclick="editItem(${item.id}, '${item.name}', ${item.currentCost})">✏️ تعديل</button>
                    <button class="btn btn-sm btn-outline-danger ms-1" onclick="deleteItem(${item.id})">🗑️ حذف</button>
                </td>
            </tr>
        `).join('');
    }

    window.editItem = function (id, name, cost) {
        itemIdInput.value = id;
        itemNameInput.value = name;
        itemCostInput.value = cost;
        catalogModal.show();
    };

    window.deleteItem = async function (id) {
        if (confirm('هل أنت متأكد من حذف هذا العنصر؟')) {
            try {
                await ApiClient.delete(`/spareparts/${id}`);
                Swal.fire('نجاح', 'تم الحذف بنجاح', 'success');
                loadItems();
            } catch (error) {
                Swal.fire('خطأ', error.message || 'حدث خطأ أثناء الحذف', 'error');
            }
        }
    };

    // Reset form when modal is closed
    document.getElementById('catalogModal').addEventListener('hidden.bs.modal', () => catalogForm.reset());

    catalogForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const id = itemIdInput.value;
        const data = {
            name: itemNameInput.value.trim(),
            currentCost: parseFloat(itemCostInput.value)
        };

        try {
            if (id) {
                data.id = parseInt(id);
                await ApiClient.put(`/spareparts/${id}`, data);
            } else {
                await ApiClient.post('/spareparts', data);
            }

            catalogModal.hide();
            Swal.fire('نجاح', 'تم الحفظ بنجاح', 'success');
            loadItems();
        } catch (error) {
            Swal.fire('خطأ', error.message || 'حدث خطأ أثناء الحفظ', 'error');
        }
    });

    loadItems();
});