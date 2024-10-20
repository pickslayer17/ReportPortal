import './RunPage.css';
import './App.css';
import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';

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
    const { runId } = useParams<{ runId: string }>();
    const location = useLocation();
    const state = location.state as TestPageState; // Cast to TestPageState
    const navigate = useNavigate();
    const [folders, setFolders] = useState<FolderVm[]>([]);
    const [tests, setTests] = useState<TestVm[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
    const [parentFolderId, setParentFolderId] = useState<number | null>(null);
    const [runName, setRunName] = useState<string | null>(null);

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);

                const run: Run = await fetchWithToken(`api/RunManagement/Runs/${runId}`);
                setRunName(run.name);

                // Set the initial folder ID from location.state or default to root
                const initialFolderId = state?.folderId || folderData.find(folder => folder.name === '$$Root$$')?.id || 0;
                setCurrentFolderId(initialFolderId);
                setParentFolderId(folderData.find(folder => folder.id === initialFolderId)?.parentId || null); // Set parent ID based on initial folder
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchFoldersAndTests();
    }, [runId, state?.folderId]);

    const handleTestClick = (testId: number, folderId: number) => {
        navigate(`/TestPage/${testId}`, { state: { folderId } });
    };

    // Render folders and tests using a table
    const getTotalTestCountForFolder = (folderId: number): number => {
        // Get direct tests in this folder
        const directTestCount = tests.filter(test => test.folderId === folderId).length;

        // Find all child folders of the current folder
        const childFolders = folders.filter(folder => folder.parentId === folderId);

        // Recursively sum up the test counts for child folders
        const childTestCount = childFolders.reduce((total, folder) => {
            return total + getTotalTestCountForFolder(folder.id);
        }, 0);

        // Return the total test count (direct + child folders)
        return directTestCount + childTestCount;
    };

    const renderFoldersAndTests = (parentId: number | null) => {
        if (folders.length === 0)
            return (
                <div className="run-page">
                    <div className="run-header">
                        <h1>No available tests in the run.</h1>
                    </div>
                </div>
            );

        // Filter child folders and tests for the current folder
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
                                <td>
                                    ({getTotalTestCountForFolder(folder.id)} tests)
                                </td>
                            </tr>
                        ))}
                        {folderTests.map(test => (
                            <tr key={test.id} className="test-row">
                                <td className="test-container" >
                                    <span className="test-name"  onClick={() => handleTestClick(test.id, test.folderId)}>
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

        return path; // If no path is found, default to "Root"
    };

        const openFolder = (folderId: number) => {
            // Set the current folder and parent folder ID
            const currentFolder = folders.find(folder => folder.id === folderId);
            if (currentFolder) {
                setParentFolderId(currentFolder.parentId); // Set parent folder ID based on the opened folder
                setCurrentFolderId(folderId); // Open the new folder
            }
        };

        const goBack = () => {
            if (parentFolderId !== null) {
                setCurrentFolderId(parentFolderId); // Navigate to the parent folder
                const parentFolder = folders.find(folder => folder.id === parentFolderId);
                setParentFolderId(parentFolder?.parentId || null); // Update parent folder ID
            }
        };

    if (loading) {
        return <p>Loading...</p>;
    }

    // Find initial parent id by folder name $$Root$$
    const initialParentId = folders.find(folder => folder.name === '$$Root$$')?.id || null;

        return (
            <div className="run-page">
                <h1>{runName}</h1>
                {renderFoldersAndTests(currentFolderId !== null ? currentFolderId : initialParentId)}
            </div>
        );
};

export default RunPage;