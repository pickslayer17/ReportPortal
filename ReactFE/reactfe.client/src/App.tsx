// App.tsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './Login';
import MainPage from './MainPage'; // Ensure this is the correct path to MainPage
import ProtectedRoute from './ProtectedRoute';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route
                    path="/"
                    element={
                        <ProtectedRoute>
                            <MainPage />
                        </ProtectedRoute>
                    }
                />
                {/* Add more protected routes here */}
            </Routes>
        </Router>
    );
}

export default App;