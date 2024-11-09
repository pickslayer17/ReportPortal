import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './Layout';
import Login from './Login';
import MainPage from './MainPage';
import SettingsPage from './SettingsPage';
import ProjectPage from './ProjectPage';
import RunPage from './RunPage';
import TestPage from './TestPage';
import ProtectedRoute from './ProtectedRoute';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
                    <Route index element={<MainPage />} />
                    <Route path="settings" element={<SettingsPage />} />
                    <Route path="ProjectPage/:projectId" element={<ProjectPage />} />
                    <Route path="RunPage/:runId" element={<RunPage />} />
                    <Route path="TestPage/:testId" element={<TestPage />} />
                </Route>
            </Routes>
        </Router>
    );
}

export default App;