import React, { useState, useEffect } from 'react';
import './SettingsPage.css';

function SettingsPage() {
    const [activeTab, setActiveTab] = useState('Projects');
    const [projects, setProjects] = useState([]);
    const [users, setUsers] = useState([]);
    const [isAdmin, setIsAdmin] = useState(true); // TODO: заменить на реальную проверку прав
    const [showModal, setShowModal] = useState(false);
    const [modalType, setModalType] = useState('');
    const [inputValue, setInputValue] = useState('');

    useEffect(() => {
        if (activeTab === 'Projects') {
            fetch('/api/ProjectManagement/GetAllProject')
                .then(res => res.json())
                .then(data => setProjects(data));
        } else {
            fetch('/api/UserManagement/GetUsers')
                .then(res => res.json())
                .then(data => setUsers(data));
        }
    }, [activeTab]);

    const openModal = (type) => {
        setModalType(type);
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setInputValue('');
    };

    const handleAdd = () => {
        const endpoint = modalType === 'Project'
            ? '/api/ProjectManagement/AddProject'
            : '/api/UserManagement/CreateUser';

        const body = modalType === 'Project'
            ? { name: inputValue }
            : { email: inputValue, password: 'defaultPass123', userRole: 'User' };

        fetch(endpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        }).then(() => {
            closeModal();
            setActiveTab(activeTab); // Триггер обновления
        });
    };

    const handleDelete = (id) => {
        const endpoint = activeTab === 'Projects'
            ? `/api/ProjectManagement/DeleteProject/${id}`
            : `/api/UserManagement/DeleteUser/${id}`;

        fetch(endpoint, { method: 'POST' })
            .then(() => setActiveTab(activeTab)); // Триггер обновления
    };

    return (
        <div className="settings-container">
            <aside className="sidebar">
                <button onClick={() => setActiveTab('Projects')}>Projects</button>
                <button onClick={() => setActiveTab('Users')}>Users</button>
            </aside>

            <main className="content">
                <h1>{activeTab}</h1>
                {isAdmin && (
                    <button className="add-button" onClick={() => openModal(activeTab.slice(0, -1))}>
                        Add New {activeTab.slice(0, -1)}
                    </button>
                )}

                <ul className="list">
                    {(activeTab === 'Projects' ? projects : users).map(item => (
                        <li key={item.id} className="list-item">
                            <span>{activeTab === 'Projects' ? item.name : item.email}</span>
                            {isAdmin && (
                                <>
                                    <button>Edit</button>
                                    <button onClick={() => handleDelete(item.id)}>Delete</button>
                                </>
                            )}
                        </li>
                    ))}
                </ul>
            </main>

            {showModal && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Add New {modalType}</h2>
                        <input
                            type="text"
                            placeholder={`${modalType} Name`}
                            value={inputValue}
                            onChange={(e) => setInputValue(e.target.value)}
                        />
                        <div className="modal-actions">
                            <button onClick={handleAdd}>Add</button>
                            <button onClick={closeModal}>Cancel</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default SettingsPage;