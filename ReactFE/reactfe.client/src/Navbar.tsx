import './Navbar.css';
import './dropdown.css';
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';

interface Project {
    id: number;
    name: string;
}

const Navbar: React.FC = () => {
    const [projects, setProjects] = useState<Project[]>([]);
    const [isDropdownOpen, setDropdownOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await fetchWithToken(`api/ProjectManagement/GetAllProject`);
                setProjects(data);
            } catch (error) {
                console.error('Failed to fetch projects:', error);
            }
        };

        fetchProjects();
    }, []);

    const handleProjectClick = (projectId: number) => {
        navigate(`/ProjectPage/${projectId}`);
    };

    const handleLogout = () => {
        document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/login');
    };

    return (
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
                <div className="navbar-item logout-button" onClick={handleLogout}>
                    Logout
                </div>
            </div>
        </nav>
    );
};

export default Navbar;