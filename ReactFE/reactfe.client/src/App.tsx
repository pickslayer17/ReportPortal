// App.tsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './Login';
import MainPage from './MainPage'; // Ensure this is the correct path to MainPage

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/main" element={<MainPage />} />
                {/* You can also add other routes as needed */}
            </Routes>
        </Router>
    );
}

export default App;