// App.tsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './Login';
import MainPage from './MainPage'; // Ensure this is the correct path to MainPage
import ProtectedRoute from './ProtectedRoute';
import Navbar from './Navbar'; // Import your new Layout component
import SettingsPage from './SettingsPage'; // Import SettingsPage if it exists
import ProjectPage from './ProjectPage'; // Import ProjectPage if it exists

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                {/* Protect each route individually */}
                <Route path="/" element={<ProtectedRoute><Navbar /></ProtectedRoute>}>
                    <Route index element={<ProtectedRoute><MainPage /></ProtectedRoute>} /> {/* Default route */}
                    <Route path="settings" element={<ProtectedRoute><SettingsPage /></ProtectedRoute>} /> {/* Settings route */}
                    <Route path="ProjectPage/:projectId" element={<ProtectedRoute><ProjectPage /></ProtectedRoute>} /> {/* ProjectPage route */}
                </Route>
            </Routes>
        </Router>
    );
}

export default App;