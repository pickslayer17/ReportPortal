const apiUrl = import.meta.env.VITE_API_URL;

export const fetchWithToken = async (endpoint: string, options: RequestInit = {}) => {
    const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    };

    const response = await fetch(`${apiUrl}/${endpoint}`, {
        ...options,
        headers,
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    return response.json();
};