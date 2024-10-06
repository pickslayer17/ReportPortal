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
    const [initialParentId, setInitialParentId] = useState<number | null>(null); // To hold the initial parent ID

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);

                // Find the initial parent ID based on the $$Root$$ folder name
                const rootFolder = folderData.find(folder => folder.name === '$$Root$$');
                if (rootFolder) {
                    setInitialParentId(rootFolder.id);
                }
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchFoldersAndTests();
    }, [runId]);

    const renderFoldersAndTests = (parentId: number | null) => {
        const childFolders = folders.filter(folder => folder.parentId === parentId && folder.name !== '$$Root$$'); // Exclude $$Root$$
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
                        <div key={test.id} className="test-name" style={{ color: 'blue' }}>
                            {test.name}
                        </div>
                    ))}
                </div>
            </div>
        );
    };

    const openFolder = (folderId: number) => {
        setParentFolderId(currentFolderId);
        setCurrentFolderId(folderId);
    };

    const goBack = () => {
        setCurrentFolderId(parentFolderId);
        setParentFolderId(folders.find(folder => folder.id === parentFolderId)?.parentId || null);
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    // Use initialParentId to start rendering child folders of $$Root$$
    return (
        <div className="run-page">
            <h1>Run Details</h1>
            {currentFolderId !== null ? renderFoldersAndTests(currentFolderId) : renderFoldersAndTests(initialParentId)}
        </div>
    );
};

export default RunPage;