import './Cool.css';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const apiUrl = import.meta.env.VITE_API_URL;

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [showError, setShowError] = useState(false);
    const [opacity, setOpacity] = useState(1); // State for controlling the opacity of the error message
    const navigate = useNavigate();

    const handleLogin = async (event: React.FormEvent) => {
        event.preventDefault();
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
                const data = await response.json();
                setErrorMessage(data.message || 'Login failed');
                setShowError(true);
                setOpacity(1); // Set opacity back to fully visible when the error appears

                // Hide the error message after 5 seconds
                setTimeout(() => {
                    let fadeEffect = setInterval(() => {
                        setOpacity((prev) => {
                            if (prev > 0) {
                                return prev - 0.1; // Reduce opacity gradually
                            } else {
                                clearInterval(fadeEffect); // Stop the interval when opacity reaches 0
                                setShowError(false); // Hide the error after fading
                                return 0;
                            }
                        });
                    }, 100); // Speed of the fading effect
                }, 5000); // Wait 5 seconds before fading out
            } else {
                const data = await response.json();
                console.log('Login successful', data);
                localStorage.setItem('token', data.token);
                navigate('/main');
            }
        } catch (error) {
            setErrorMessage('An error occurred. Please try again.');
            setShowError(true);
            setOpacity(1); // Set opacity back to fully visible when the error appears

            setTimeout(() => {
                let fadeEffect = setInterval(() => {
                    setOpacity((prev) => {
                        if (prev > 0) {
                            return prev - 0.1; // Reduce opacity gradually
                        } else {
                            clearInterval(fadeEffect);
                            setShowError(false);
                            return 0;
                        }
                    });
                }, 100);
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
                {showError && (
                    <p className="error-message" style={{ opacity: opacity }}>
                        {errorMessage}
                    </p>
                )}
            </form>
        </div>
    );
}

export default Login;