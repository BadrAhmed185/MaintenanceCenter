// wwwroot/js/views/reception/index.js

document.addEventListener('DOMContentLoaded', function () {
    const receiveForm = document.getElementById('receiveDeviceForm');
    const saveBtn = document.getElementById('saveBtn');

    // Regex patterns matching the server-side DTO
    const phoneRegex = /^01[0125][0-9]{8}$/;
    const nameRegex = /^[\p{L}\s]+$/u; // 'u' flag for unicode (Arabic support)

    receiveForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        // 1. Collect Data
        const requestData = {
            deviceName: document.getElementById('DeviceName').value.trim(),
            faultDescription: document.getElementById('FaultDescription').value.trim(),
            deviceCondition: document.getElementById('DeviceCondition').value.trim(),
            clientEntityName: document.getElementById('ClientEntityName').value.trim(),
            delivererName: document.getElementById('DelivererName').value.trim(),
            delivererPhone: document.getElementById('DelivererPhone').value.trim()
        };

        // 2. STRICT CLIENT-SIDE VALIDATION
        let validationErrors = [];

        if (requestData.deviceName.length < 3) validationErrors.push("اسم الجهاز يجب أن يكون 3 أحرف على الأقل.");
        if (requestData.faultDescription.length < 4) validationErrors.push("وصف العطل يجب أن يكون 4 أحرف على الأقل.");
        if (requestData.clientEntityName.length < 3) validationErrors.push("الجهة القادم منها الجهاز غير صحيحة.");

        if (!nameRegex.test(requestData.delivererName)) {
            validationErrors.push("اسم المُسلّم يجب أن يحتوي على حروف فقط بدون أرقام.");
        }

        if (!phoneRegex.test(requestData.delivererPhone)) {
            validationErrors.push("رقم الهاتف يجب أن يكون رقم موبايل مصري صحيح (11 رقم).");
        }

        // If client-side validation fails, stop and show errors
        if (validationErrors.length > 0) {
            Swal.fire({
                icon: 'warning',
                title: 'بيانات غير صالحة',
                html: `<ul style="text-align: right; direction: rtl;">
                        ${validationErrors.map(err => `<li>${err}</li>`).join('')}
                       </ul>`,
                confirmButtonColor: '#003366',
                confirmButtonText: 'حسناً، سأقوم بالتعديل'
            });
            return; // STOP EXECUTION
        }

        // 3. UI Feedback: Show loading state
        const originalBtnText = saveBtn.innerHTML;
        saveBtn.disabled = true;
        saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الحفظ...';

        try {
            // 4. Call the API
            const response = await ApiClient.post('/maintenancerequests/receive', requestData);

            if (response && response.succeeded) {
                const trackingCode = response.data.trackingCode;
                Swal.fire({
                    icon: 'success',
                    title: 'تم استلام الجهاز بنجاح!',
                    html: `
                        <p class="mb-2">يرجى تسليم هذا الكود للجهة لمتابعة حالة الجهاز:</p>
                        <div class="p-3 bg-light border rounded text-center mb-3">
                            <h2 class="text-mis-blue fw-bold mb-0" style="letter-spacing: 2px;">${trackingCode}</h2>
                        </div>
                    `,
                    confirmButtonText: '🖨️ طباعة الإيصال وإغلاق',
                    confirmButtonColor: '#003366',
                    allowOutsideClick: false
                }).then((result) => {
                    if (result.isConfirmed) {
                        receiveForm.reset();
                    }
                });
            }
        } catch (error) {
            // IF THE SERVER CATCHES SOMETHING THE CLIENT MISSED
            // The ApiClient will throw the formatted Error list from BaseApiController

            let errorHtml = `<p>${error.message || 'حدث خطأ أثناء الحفظ.'}</p>`;

            // If the server returned an array of validation errors (from ModelState)
            if (error.errors && Array.isArray(error.errors)) {
                errorHtml += `<ul style="text-align: right; direction: rtl;">
                                ${error.errors.map(err => `<li>${err}</li>`).join('')}
                               </ul>`;
            }

            Swal.fire({
                icon: 'error',
                title: 'خطأ في الحفظ',
                html: errorHtml,
                confirmButtonColor: '#003366'
            });
        } finally {
            saveBtn.disabled = false;
            saveBtn.innerHTML = originalBtnText;
        }
    });
});