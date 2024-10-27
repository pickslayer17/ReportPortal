import './RunPage.css';
import './App.css';

import EditTestReviewModal from './EditTestReviewModal';
import { TestVm, TestReviewOutcome, TestReviewVm } from './interfaces/TestVmProps';
import { UserVm } from './interfaces/UserVmProps';
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
    const [users, setUsers] = useState<UserVm[]>([]);
    const [testReviews, setTestReviews] = useState<TestReviewVm[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
    const [parentFolderId, setParentFolderId] = useState<number | null>(null);
    const [runName, setRunName] = useState<string | null>(null);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [reviewerEmails, setReviewerEmails] = useState<{ [key: number]: string }>({});
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedTests, setSelectedTests] = useState<number[]>([]); // Track selected test IDs

    useEffect(() => {
        const fetchFoldersAndTests = async () => {
            try {
                const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                setFolders(folderData);

                const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                setTests(testData);

                const run: Run = await fetchWithToken(`api/RunManagement/Runs/${runId}`);
                setRunName(run.name);

                const userData: UserVm[] = await fetchWithToken(`api/UserManagement/GetUsers`);
                setUsers(userData);

                const initialFolderId = state?.folderId || folderData.find(folder => folder.name === '$$Root$$')?.id || 0;
                setCurrentFolderId(initialFolderId);
                setParentFolderId(folderData.find(folder => folder.id === initialFolderId)?.parentId || null);
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/runUpdates`, { withCredentials: true })
            .withAutomaticReconnect()
            .build();

        setConnection(connection);

        fetchFoldersAndTests();
    }, [runId, state?.folderId]);

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

    const openModal = () => {
        if (selectedTests.length === 0) {
            alert('Choose at least 1 test for edit');
            return;
        }

        const selectedReviews = tests.filter(test => selectedTests.includes(test.id)).map(test => test.testReview);
        setTestReviews(selectedReviews);
        setIsModalOpen(true);
        setSelectedTests([]); // Clear selections after opening modal
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
                            <th>
                                <button onClick={openModal}>
                                    Update selected tests
                                </button>
                            </th>
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
                                    <input
                                        type="checkbox"
                                        checked={selectedTests.includes(test.id)}
                                        onChange={() => {
                                            setSelectedTests(prev =>
                                                prev.includes(test.id)
                                                    ? prev.filter(id => id !== test.id)
                                                    : [...prev, test.id]
                                            );
                                        }}
                                    />
                                    <span className="test-name" onClick={() => handleTestClick(test.id, test.folderId)}>
                                        {test.name}
                                    </span>
                                </td>
                                <td className="test-review-outcome-container">
                                    {test.testReview.testReviewOutcome === TestReviewOutcome.ToInvestigate && (
                                        <span className="to-investigate">?</span>
                                    )}
                                    {test.testReview.testReviewOutcome === TestReviewOutcome.NotRepro && (
                                        <span className="not-repro">NR</span>
                                    )}
                                    {test.testReview.testReviewOutcome === TestReviewOutcome.ProductBug && (
                                        <div className="product-bug">
                                            BUG
                                            <span className="bug-id">0</span>
                                        </div>
                                    )}
                                </td>
                                <td className="test-reviewer-container">
                                    {reviewerEmails[test.testReview.reviewerId] || 'No reviewer'}
                                </td>
                                <td className="test-comment-container">
                                    {test.testReview.comments}
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
           
            <EditTestReviewModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                users={users}
                testReviews={testReviews}
            />
        </div>
    );
};

export default RunPage;