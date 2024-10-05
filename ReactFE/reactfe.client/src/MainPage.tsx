import { useNavigate } from 'react-router-dom';
import './App.css'; // Use the merged CSS file

function MainPage() {
    const navigate = useNavigate();

    return (
        <div className="main-container">
            <nav className="navbar">
                <ul className="nav-list">
                    <li onClick={() => navigate('/projects')}>Projects</li>
                    <li onClick={() => navigate('/runs')}>Runs</li>
                    <li onClick={() => navigate('/settings')}>Settings</li>
                </ul>
            </nav>

            <div className="content">
                <h1>Welcome to the Main Page</h1>
            </div>
        </div>
    );
}

export default MainPage;