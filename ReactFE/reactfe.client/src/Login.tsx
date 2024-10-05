import './Cool.css';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie'; // Import the js-cookie library

const apiUrl = import.meta.env.VITE_API_URL;

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [showError, setShowError] = useState(false); // State for controlling error visibility
    const navigate = useNavigate();

    // Check for token in cookies on component mount
    useEffect(() => {
        const token = Cookies.get('token'); // Check if token exists in cookies
        if (token) {
            // Optionally, you can add logic to check if the token is valid
            // For now, we'll assume the token is valid and redirect the user
            navigate('/main'); // Redirect to the main page
        }
    }, [navigate]);

    const handleLogin = async (event: React.FormEvent) => {
        event.preventDefault(); // Prevent form submission
        console.log("API URL:", apiUrl);
        try {
            const response = await fetch(`${apiUrl}/api/UserManagement/Login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            if (!response.ok) {
                // Handle error response
                const data = await response.json();
                setErrorMessage(data.message || 'Login failed');
                setShowError(true); // Show the error message

                // Hide the error message after 5 seconds
                setTimeout(() => {
                    setShowError(false);
                }, 5000);
            } else {
                const data = await response.json();
                console.log('Login successful', data);

                // Store the token in cookies instead of localStorage
                Cookies.set('token', data.token, { secure: true, sameSite: 'Strict', expires: 7 }); // Expires in 7 days

                // Redirect to the main page using React Router
                navigate('/main'); // Change this to your main page route
            }
        } catch (error) {
            setErrorMessage('An error occurred. Please try again.');
            setShowError(true); // Show the error message

            // Hide the error message after 5 seconds
            setTimeout(() => {
                setShowError(false);
            }, 5000);
        }
    };

    return (
        <div className="login-container">
            <h1>Login</h1>
            <form onSubmit={handleLogin}>
                <div>
                    <label htmlFor="username">Username:</label>
                    <input
                        type="text"
                        id="username"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Login</button>
                {/* Error message displayed conditionally */}
                {showError && <p className={`error-message ${showError ? '' : 'hide'}`}>{errorMessage}</p>}
            </form>
        </div>
    );
}

export default Login;