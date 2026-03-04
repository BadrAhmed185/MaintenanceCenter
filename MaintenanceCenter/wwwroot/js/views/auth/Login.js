// wwwroot/js/views/auth/login.js

document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const loginBtn = document.getElementById('loginBtn');

    // Check if the user was redirected here because their session expired
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('sessionExpired')) {
        Swal.fire({
            icon: 'info',
            title: 'انتهت الجلسة',
            text: 'يرجى تسجيل الدخول مرة أخرى.',
            confirmButtonColor: '#003366'
        });
        // Clean up the URL
        window.history.replaceState({}, document.title, "/Auth/Login");
    }

    loginForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const userName = document.getElementById('UserName').value.trim();
        const password = document.getElementById('Password').value;

        // UI feedback: Disable button and show loading state
        const originalBtnText = loginBtn.innerHTML;
        loginBtn.disabled = true;
        loginBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري التحقق...';

        try {
            // Using our clean ApiClient wrapper!
            const response = await ApiClient.post('/auth/login', {
                userName: userName,
                password: password
            });

            if (response && response.succeeded) {
                // Dynamic Role-Based Routing
                const role = response.role;

                switch (role) {
                    case 'Admin':
                        window.location.href = '/Admin/Dashboard'; // Will build this next
                        break;
                    case 'Technician':
                        window.location.href = '/Technician/Workspace'; // Will build this next
                        break;
                    case 'Receptionist':
                    default:
                        window.location.href = '/Reception/index';
                        break;
                }
            }

        } catch (error) {
            // ApiClient throws the error message we defined in the backend
            Swal.fire({
                icon: 'error',
                title: 'خطأ في تسجيل الدخول',
                text: error.message || 'بيانات الاعتماد غير صحيحة.',
                confirmButtonColor: '#003366'
            });
        } finally {
            // Restore button state
            loginBtn.disabled = false;
            loginBtn.innerHTML = originalBtnText;
        }
    });
});