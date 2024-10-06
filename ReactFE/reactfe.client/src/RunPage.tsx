import './RunPage.css';
import './App.css';
import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { fetchWithToken } from './helpers/api'; // Import your helper function

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
    const [loading, setLoading] = useState(true); // Set loading state

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                // Fetch folders using fetchWithToken
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                // Fetch tests using fetchWithToken
                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);

            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false); // End loading after fetch
            }
        };

        fetchFoldersAndTests();
    }, [runId]);

    // Helper function to display folders and tests recursively
    const renderFolder = (folder: FolderVm) => {
        const childFolders = folders.filter(f => f.parentId === folder.id);
        const folderTests = tests.filter(t => t.folderId === folder.id);

        return (
            <div key={folder.id} className="folder">
                <div className="folder-name">{folder.name}</div>
                <div className="folder-content">
                    {childFolders.map(childFolder => renderFolder(childFolder))}
                    {folderTests.map(test => (
                        <div key={test.id} className="test-name">
                            {test.name}
                        </div>
                    ))}
                </div>
            </div>
        );
    };

    const rootFolder = folders.find(f => f.name === '$$Root$$');

    if (loading) {
        return <p>Loading...</p>;
    }

    if (!rootFolder) {
        return <p>No folders found for this run.</p>;
    }

    return (
        <div className="run-page">
            <h1>Run Details</h1>
            {renderFolder(rootFolder)}
        </div>
    );
};

export default RunPage;