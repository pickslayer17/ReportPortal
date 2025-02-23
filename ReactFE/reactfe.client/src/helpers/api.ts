const apiUrl = import.meta.env.VITE_API_URL;

export const fetchWithToken = async (endpoint: string, options: RequestInit = {}) => {
    const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    };

    const response = await fetch(`${apiUrl}/${endpoint}`, {
        headers,
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    return response.json();
};

export const deleteWithToken = async (endpoint: string, options: RequestInit = {}) => {
    const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    };

    const response = await fetch(`${apiUrl}/${endpoint}`, {
        method: 'DELETE',
        headers,
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    return response;
};

export const putWithToken = async (endpoint: string, body: object, options: RequestInit = {}) => {
    const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];
    
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    };

    const response = await fetch(`${apiUrl}/${endpoint}`, {
        method: 'PUT',
        headers,
        body: JSON.stringify( body )
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    return response.json();
};