import './RunPage.css';
import './App.css';
import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';
import * as signalR from '@microsoft/signalr'; // Import SignalR

interface FolderVm {
    id: number;
    runId: number;
    name: string;
    parentId: number | null;
    childFolderIds: number[] | null;
    testIds: number[] | null;
}

interface TestVm {
    id: number;
    folderId: number;
    path: string;
    runId: number;
    name: string;
}

interface Run {
    id: number;
    name: string;
    projectId: number;
}

interface TestPageState {
    folderId: number;
}

const RunPage: React.FC = () => {
    const apiUrl = import.meta.env.VITE_API_URL;
    const { runId } = useParams<{ runId: string }>();
    const location = useLocation();
    const state = location.state as TestPageState;
    const navigate = useNavigate();
    const [folders, setFolders] = useState<FolderVm[]>([]);
    const [tests, setTests] = useState<TestVm[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
    const [parentFolderId, setParentFolderId] = useState<number | null>(null);
    const [runName, setRunName] = useState<string | null>(null);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null); // SignalR connection

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);

                const run: Run = await fetchWithToken(`api/RunManagement/Runs/${runId}`);
                setRunName(run.name);

                const initialFolderId = state?.folderId || folderData.find(folder => folder.name === '$$Root$$')?.id || 0;
                setCurrentFolderId(initialFolderId);
                setParentFolderId(folderData.find(folder => folder.id === initialFolderId)?.parentId || null);
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        // Connect to SignalR hub
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/runUpdates`, {
                withCredentials: true
            }) // Point to your SignalR hub endpoint
            .withAutomaticReconnect()
            .build();

        setConnection(connection);

        fetchFoldersAndTests();
    }, [runId, state?.folderId]);

    // Listen for real-time updates
    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected to SignalR hub');

                    connection.on("UpdateFolders", (updatedFolders: FolderVm[]) => {
                        setFolders(updatedFolders);
                        console.log("Received updated data:", updatedFolders);
                    });

                    connection.on("UpdateTests", (updatedTests: TestVm[]) => {
                        setTests(updatedTests);
                        console.log("Received updated data:", updatedTests);
                    });

                    connection.on("ReceiveUpdate", (updatedData) => {
                        // Handle the updated data here, e.g., refresh the folders/tests
                        console.log("Received updated data:", updatedData);
                    });
                })
                .catch(error => console.error("Error establishing SignalR connection: ", error));

            return () => {
                connection.stop();
            };
        }
    }, [connection]);

    const handleTestClick = (testId: number, folderId: number) => {
        navigate(`/TestPage/${testId}`, { state: { folderId } });
    };

    const getTotalTestCountForFolder = (folderId: number): number => {
        const directTestCount = tests.filter(test => test.folderId === folderId).length;
        const childFolders = folders.filter(folder => folder.parentId === folderId);
        const childTestCount = childFolders.reduce((total, folder) => total + getTotalTestCountForFolder(folder.id), 0);
        return directTestCount + childTestCount;
    };

    const renderFoldersAndTests = (parentId: number | null) => {
        if (folders.length === 0) {
            return (
                <div className="run-page">
                    <div className="run-header">
                        <h1>No available tests in the run.</h1>
                    </div>
                </div>
            );
        }

        const childFolders = folders.filter(folder => folder.parentId === parentId);
        const folderTests = tests.filter(test => test.folderId === parentId);

        return (
            <div className="folder-container">
                <table className="folder-table">
                    <thead>
                        <tr>
                            <th onClick={goBack}>{getBackButtonName()}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {childFolders.map(folder => (
                            <tr key={folder.id} className="folder-row">
                                <td className="folder" onClick={() => openFolder(folder.id)}>
                                    {folder.name}
                                </td>
                                <td>({getTotalTestCountForFolder(folder.id)} tests)</td>
                            </tr>
                        ))}
                        {folderTests.map(test => (
                            <tr key={test.id} className="test-row">
                                <td className="test-container">
                                    <span className="test-name" onClick={() => handleTestClick(test.id, test.folderId)}>
                                        {test.name}
                                    </span>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    };

    const getBackButtonName = (): string => {
        let currentFolder = folders.find(folder => folder.id === currentFolderId);
        let path = "/";
        while (currentFolder?.name !== '$$Root$$') {
            if (currentFolder) {
                if (currentFolder?.name !== '$$Root$$')
                    path = ' / ' + currentFolder.name + path;
            }
            currentFolder = folders.find(folder => folder.id === currentFolder?.parentId);
        }
        return path;
    };

    const openFolder = (folderId: number) => {
        const currentFolder = folders.find(folder => folder.id === folderId);
        if (currentFolder) {
            setParentFolderId(currentFolder.parentId);
            setCurrentFolderId(folderId);
        }
    };

    const goBack = () => {
        if (parentFolderId !== null) {
            setCurrentFolderId(parentFolderId);
            const parentFolder = folders.find(folder => folder.id === parentFolderId);
            setParentFolderId(parentFolder?.parentId || null);
        }
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    const initialParentId = folders.find(folder => folder.name === '$$Root$$')?.id || null;

    return (
        <div className="run-page">
            <h1>{runName}</h1>
            {renderFoldersAndTests(currentFolderId !== null ? currentFolderId : initialParentId)}
        </div>
    );
};

export default RunPage;