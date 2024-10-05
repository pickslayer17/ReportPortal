import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import Cookies from 'js-cookie'; // Assuming you store the token in cookies

const apiUrl = import.meta.env.VITE_API_URL;

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const [isTokenValid, setIsTokenValid] = useState<boolean | null>(null);

    useEffect(() => {
        const validateToken = async () => {
            const token = Cookies.get('token'); // Retrieve the token from cookies
            if (!token) {
                setIsTokenValid(false);
                return;
            }

            try {
                const response = await fetch(`${apiUrl}/api/UserManagement/ValidateToken`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`, // Include the token in the request
                    },
                });

                if (response.ok) {
                    setIsTokenValid(true); // Token is valid
                } else {
                    setIsTokenValid(false); // Invalid token
                }
            } catch (error) {
                setIsTokenValid(false); // Error occurred, assume token is invalid
            }
        };

        validateToken();
    }, []);

    if (isTokenValid === null) {
        return <div>Loading...</div>; // While waiting for validation, show a loading state
    }

    if (!isTokenValid) {
        return <Navigate to="/" replace />; // Redirect to login if token is invalid
    }

    return <>{children}</>; // Render protected content if token is valid
};

export default ProtectedRoute;