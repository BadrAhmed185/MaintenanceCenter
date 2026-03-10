// wwwroot/js/api-client.js

const ApiClient = (function () {
    // We use relative paths since the API and MVC are on the same server,
    // but you can define a BaseUrl here if you ever separate them.
    const baseUrl = '/engmnt/api';

    async function request(endpoint, method = 'GET', data = null) {
        const url = `${baseUrl}${endpoint}`;
        
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
                // Notice: NO 'Authorization' header here. The browser sends the HttpOnly cookie automatically!
            }
        };

        if (data && (method === 'POST' || method === 'PUT')) {
            options.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(url, options);

            // Global 401 Unauthorized Interceptor
            if (response.status === 401) {
                window.location.href = '~/Auth/Login?sessionExpired=true';
                return null;
            }

            const result = await response.json();

            // If the API returns a standard BadRequest with our ServiceResult errors
            if (!response.ok) {
                throw new Error(result.message || 'حدث خطأ غير متوقع');
            }

            return result;
        } catch (error) {
            console.error(`[ApiClient Error] ${method} ${endpoint}:`, error);
            throw error; // Re-throw so the specific view can show a SweetAlert
        }
    }

    // Expose public methods
    return {
        get: (endpoint) => request(endpoint, 'GET'),
        post: (endpoint, data) => request(endpoint, 'POST', data),
        put: (endpoint, data) => request(endpoint, 'PUT', data),
        delete: (endpoint) => request(endpoint, 'DELETE')
    };
})();