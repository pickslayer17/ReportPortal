// Navbar.tsx
import './Navbar.css'; // Import your Navbar CSS
import React, { useState, useEffect } from 'react';
import { Outlet, useNavigate } from 'react-router-dom'; // Ensure useNavigate is imported
import { fetchWithToken } from './helpers/api';

interface Project {
    id: number;
    name: string;
}

const Navbar: React.FC = () => {
    const [projects, setProjects] = useState<Project[]>([]);
    const [isDropdownOpen, setDropdownOpen] = useState(false);
    const navigate = useNavigate(); // Get navigate from useNavigate

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await fetchWithToken(`api/ProjectManagement/GetAllProject`);
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
        <div>
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
                    <div className="navbar-item logout-button" onClick={() => handleLogout()}>
                        Logout
                    </div>
                </div>
            </nav>
            <Outlet /> {/* This will render the child routes */}
        </div>
    );
};

export default Navbar;