import './RunPage.css';
import './App.css';
import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
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
    testResultIds: number[] | null;
}

const RunPage: React.FC = () => {
    const { runId } = useParams<{ runId: string }>();
    const [folders, setFolders] = useState<FolderVm[]>([]);
    const [tests, setTests] = useState<TestVm[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
    const [parentFolderId, setParentFolderId] = useState<number | null>(null);

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchFoldersAndTests();
    }, [runId]);

    // Render folders and tests
    const renderFoldersAndTests = (parentId: number | null) => {
        const childFolders = folders.filter(folder => folder.parentId === parentId);
        const folderTests = tests.filter(test => test.folderId === parentId);

        return (
            <div className="folder-container">
                {parentFolderId !== null && (
                    <button onClick={goBack} className="back-button">
                        {folders.find(folder => folder.id === parentFolderId)?.name}
                    </button>
                )}
                <div className="folder-list">
                    {childFolders.map(folder => (
                        <div key={folder.id} className="folder" onClick={() => openFolder(folder.id)}>
                            {folder.name}
                        </div>
                    ))}
                    {folderTests.map(test => (
                        <div key={test.id} className="test-container" style={{ border: '1px solid black', backgroundColor: 'white' }}>
                            <div className="test-name" style={{ color: 'blue' }}>
                                {test.name}
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        );
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
        // Navigate back to the parent folder
        setCurrentFolderId(parentFolderId);
        const parentFolder = folders.find(folder => folder.id === parentFolderId);
        setParentFolderId(parentFolder?.parentId || null); // Update the parent folder ID for the back button
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    // Find initial parent id by folder name $$Root$$
    const initialParentId = folders.find(folder => folder.name === '$$Root$$')?.id || null;

    return (
        <div className="run-page">
            <h1>Run Details</h1>
            {renderFoldersAndTests(currentFolderId !== null ? currentFolderId : initialParentId)}
        </div>
    );
};

export default RunPage;