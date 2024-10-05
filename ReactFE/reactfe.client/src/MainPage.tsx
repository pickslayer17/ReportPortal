import './App.css';
import './Navbar.css';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const apiUrl = import.meta.env.VITE_API_URL;

interface Project {
    id: number;
    name: string;
}

function MainPage() {
    const [projects, setProjects] = useState<Project[]>([]);
    const [isDropdownOpen, setDropdownOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1]; // Get the token from cookies
                const response = await fetch(`${apiUrl}/api/ProjectManagement/GetAllProject`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`, // Include token in the Authorization header
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch projects'); // Handle error response
                }

                const data: Project[] = await response.json(); // Explicitly declare type for the response data
                setProjects(data); // Save the project list
            } catch (error) {
                console.error('Failed to fetch projects:', error);
                // Optionally handle token expiration or redirect to login
            }
        };

        fetchProjects();
    }, []);

    const handleProjectClick = (projectId: number) => {
        navigate(`/ProjectPage/${projectId}`);
    };

    const handleLogout = () => {
        // Clear the token cookie
        document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;"; // Set cookie to expire
        navigate('/login'); // Redirect to login page
    };

    return (
        <div className="main-page">
            <nav className="navbar">
                <div className="navbar-container">
                    <div className="navbar-item" onClick={() => setDropdownOpen(!isDropdownOpen)}>
                        Projects
                        {isDropdownOpen && (
                            <ul className="dropdown">
                                {projects.map((project) => (
                                    <li key={project.id} onClick={() => handleProjectClick(project.id)}>
                                        {project.name}
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>
                    <div className="navbar-item" onClick={() => navigate('/settings')}>
                        Settings
                    </div>
                    <div className="navbar-item" onClick={handleLogout}>
                        Logout
                    </div>
                </div>
            </nav>
            {/* Other main page content goes here */}
        </div>
    );
}

export default MainPage;