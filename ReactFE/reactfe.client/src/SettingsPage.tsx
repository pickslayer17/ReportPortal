import React, { useState, useEffect, useCallback } from 'react';
import './SettingsPage.css';
import { postWithToken, fetchWithToken } from './helpers/api';
import { getUserRoleFromToken } from './helpers/auth';

type DeleteTarget = { id: number; name: string };

function SettingsPage() {
    const [activeTab, setActiveTab] = useState<'Projects' | 'Users'>('Projects');
    const [projects, setProjects] = useState<any[]>([]);
    const [users, setUsers] = useState<any[]>([]);
    const [isAdmin, setIsAdmin] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [modalType, setModalType] = useState<'Project' | 'User' | ''>('');
    const [inputValue, setInputValue] = useState('');
    const [userPassword, setUserPassword] = useState('test');
    const [showPassword, setShowPassword] = useState(false);
    const [userRole, setUserRole] = useState<'User' | 'Admin'>('User');
    const [deleteTarget, setDeleteTarget] = useState<DeleteTarget | null>(null);
    const [deleteSuccess, setDeleteSuccess] = useState(false);

    // Универсальный рефреш данных
    const fetchData = useCallback(async () => {
        if (activeTab === 'Projects') {
            const data = await fetchWithToken('api/ProjectManagement/GetAllProject');
            setProjects(data);
        } else {
            const data = await fetchWithToken('api/UserManagement/GetUsers');
            setUsers(data);
        }
    }, [activeTab]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    useEffect(() => {
        const role = getUserRoleFromToken();
        setIsAdmin(role && role.toLowerCase() === 'admin');
    }, []);

    const openModal = (type: 'Project' | 'User') => {
        setModalType(type);
        setShowModal(true);
        setInputValue('');
        setUserPassword('test');
        setUserRole('User');
    };

    const closeModal = () => {
        setShowModal(false);
        setInputValue('');
        setUserPassword('test');
        setUserRole('User');
        setShowPassword(false);
        setDeleteTarget(null);
        setDeleteSuccess(false);
    };

    const handleAdd = async () => {
        const endpoint = modalType === 'Project'
            ? 'api/ProjectManagement/AddProject'
            : 'api/UserManagement/CreateUser';

        const body = modalType === 'Project'
            ? { name: inputValue }
            : {
                email: inputValue,
                password: userPassword,
                userRole: userRole === 'Admin' ? 1 : 0 // enum!
            };

        try {
            await postWithToken(endpoint, body);
            closeModal();
            await fetchData();
        } catch (error) {
            alert('Ошибка при добавлении: ' + error);
        }
    };

    // Открыть модалку подтверждения удаления
    const confirmDelete = (item: any) => {
        setDeleteTarget({
            id: item.id,
            name: activeTab === 'Projects' ? item.name : item.email
        });
        setShowModal(true);
        setModalType('');
    };

    // Удалить после подтверждения
    const handleDelete = async () => {
        if (!deleteTarget) return;
        const endpoint = activeTab === 'Projects'
            ? `api/ProjectManagement/DeleteProject/${deleteTarget.id}`
            : `api/UserManagement/DeleteUser/${deleteTarget.id}`;

        try {
            await postWithToken(endpoint, {});
            await fetchData();
            setDeleteSuccess(true);
        } catch (error) {
            alert('Ошибка при удалении: ' + error);
            closeModal();
        }
    };

    return (
        <div className="settings-container">
            <aside className="sidebar">
                <button
                    className={activeTab === 'Projects' ? 'active' : ''}
                    onClick={() => setActiveTab('Projects')}
                >
                    Projects
                </button>
                <button
                    className={activeTab === 'Users' ? 'active' : ''}
                    onClick={() => setActiveTab('Users')}
                >
                    Users
                </button>
            </aside>

            <main className="content">
                <h1>{activeTab}</h1>
                {isAdmin && (
                    <button className="add-button" onClick={() => openModal(activeTab.slice(0, -1) as 'Project' | 'User')}>
                        Add New {activeTab.slice(0, -1)}
                    </button>
                )}

                <ul className="list">
                    {(activeTab === 'Projects' ? projects : users).map((item: any) => (
                        <li key={item.id} className="list-item">
                            <span>{activeTab === 'Projects' ? item.name : item.email}</span>
                            {isAdmin && (
                                <>
                                    <button>Edit</button>
                                    <button onClick={() => confirmDelete(item)}>Delete</button>
                                </>
                            )}
                        </li>
                    ))}
                </ul>
            </main>

            {showModal && (
                <div className="modal">
                    <div className="modal-content">
                        {/* Модалка добавления */}
                        {modalType && !deleteTarget && (
                            <>
                                <h2>Add New {modalType}</h2>
                                {modalType === 'Project' && (
                                    <input
                                        type="text"
                                        placeholder="Project Name"
                                        value={inputValue}
                                        onChange={(e) => setInputValue(e.target.value)}
                                    />
                                )}
                                {modalType === 'User' && (
                                    <>
                                        <input
                                            type="text"
                                            placeholder="Email"
                                            value={inputValue}
                                            onChange={(e) => setInputValue(e.target.value)}
                                        />
                                        <div style={{ position: 'relative', marginBottom: '20px' }}>
                                            <input
                                                type={showPassword ? 'text' : 'password'}
                                                placeholder="Password"
                                                value={userPassword}
                                                onChange={(e) => setUserPassword(e.target.value)}
                                                style={{ width: '100%', paddingRight: '40px' }}
                                            />
                                            <span
                                                onClick={() => setShowPassword((v) => !v)}
                                                style={{
                                                    position: 'absolute',
                                                    right: '10px',
                                                    top: '50%',
                                                    transform: 'translateY(-50%)',
                                                    cursor: 'pointer',
                                                    color: '#ffd000',
                                                    fontSize: '1.2em'
                                                }}
                                                title={showPassword ? 'Скрыть пароль' : 'Показать пароль'}
                                            >
                                                {showPassword ? '🙈' : '👁️'}
                                            </span>
                                        </div>
                                        <select
                                            value={userRole}
                                            onChange={e => setUserRole(e.target.value as 'User' | 'Admin')}
                                            style={{ width: '100%', padding: '10px', borderRadius: '8px', marginBottom: '20px' }}
                                        >
                                            <option value="User">User</option>
                                            <option value="Admin">Admin</option>
                                        </select>
                                    </>
                                )}
                                <div className="modal-actions">
                                    <button onClick={handleAdd}>Add</button>
                                    <button onClick={closeModal}>Cancel</button>
                                </div>
                            </>
                        )}

                        {/* Модалка подтверждения удаления */}
                        {deleteTarget && !deleteSuccess && (
                            <>
                                <h2>Подтвердите удаление</h2>
                                <p>
                                    {activeTab === 'Projects'
                                        ? <>Удалить проект <b>{deleteTarget.name}</b>?</>
                                        : <>Удалить пользователя <b>{deleteTarget.name}</b>?</>
                                    }
                                </p>
                                <div className="modal-actions">
                                    <button onClick={handleDelete} style={{ background: '#ffd000', color: '#181818' }}>Удалить</button>
                                    <button onClick={closeModal}>Отмена</button>
                                </div>
                            </>
                        )}

                        {/* Модалка успешного удаления */}
                        {deleteTarget && deleteSuccess && (
                            <>
                                <h2>Успешно!</h2>
                                <p>
                                    {activeTab === 'Projects'
                                        ? <>Проект <b>{deleteTarget.name}</b> успешно удалён.</>
                                        : <>Пользователь <b>{deleteTarget.name}</b> успешно удалён.</>
                                    }
                                </p>
                                <div className="modal-actions">
                                    <button onClick={closeModal}>OK</button>
                                </div>
                            </>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}

export default SettingsPage;