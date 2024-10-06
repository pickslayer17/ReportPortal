import './App.css';
import './RunPage.css';
import { useEffect, useState } from 'react';
import { useParams, } from 'react-router-dom';
import { fetchWithToken } from './helpers/api'; // Adjust the path as necessary

interface FolderVm {
    id: number;
    runId: number;
    name: string;
    parentId?: number;
    childFolderIds?: number[];
    testIds?: number[];
}

function RunPage() {
    const { runId } = useParams<{ runId: string }>();
    const [folders, setFolders] = useState<FolderVm[]>([]);
    const[rootFolderId, setRootFolderId] = useState<number | null>(null);

    useEffect(() => {
        const fetchFolders = async () => {
            try {
                const data: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(data);

                const rootFolder = data.find(folder => folder.name === '$$Root$$');
                if (rootFolder) {
                    setRootFolderId(rootFolder.id);
                } else {
                    console.error('Root folder not found');
                }
            } catch (error) {
                console.error('Failed to fetch folders:', error);
            }
        };

        fetchFolders();
    }, [runId]);

    const renderFolders = (parentId: number | null) => {
        return folders
            .filter(folder => folder.parentId === parentId)
            .map(folder => (
                <div key={folder.id} className="folder">
                    <div className="folder-name">{folder.name}</div>
                    {/* Render subfolders */}
                    <div className="subfolders">
                        {renderFolders(folder.id)}
                    </div>
                    {/* Render tests if available */}
                    {folder.testIds && folder.testIds.length > 0 && (
                        <div className="tests">
                            <h4>Tests:</h4>
                            <ul>
                                {folder.testIds.map(testId => (
                                    <li key={testId}>Test ID: {testId}</li>
                                ))}
                            </ul>
                        </div>
                    )}
                </div>
            ));
    };

    return (
        <div className="run-page">
            <h1>Run Details</h1>
            <p>Details for Run ID: {runId}</p>
            <div className="folders-list">
                {rootFolderId !== null && renderFolders(rootFolderId)} {/* Start rendering from the root folder */}
            </div>
        </div>
    );
};

export default RunPage;