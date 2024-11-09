import './RunPage.css';
import './App.css';
import './dropdown.css';

import EditTestReviewModal from './EditTestReviewModal';
import { EditTestReviewMode } from './enums/EditTestReviewMode'
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
    const [runName, setRunName] = useState<string | null>(null);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [reviewerEmails, setReviewerEmails] = useState<{ [key: number]: string }>({});
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedTests, setSelectedTests] = useState<number[]>([]); // Track selected test IDs
    const [allSelected, setAllSelected] = useState(false);
    const [selectedAction, setSelectedAction] = useState<string | null>(null);
    const [editMode, setEditMode] = useState<EditTestReviewMode>(EditTestReviewMode.all);

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

                const dict = Object.fromEntries(userData.map(userVm => [userVm.id, userVm.email]));
                setReviewerEmails(dict);

                const initialFolderId = state?.folderId || folderData.find(folder => folder.name === '$$Root$$')?.id || 0;
                setCurrentFolderId(initialFolderId);
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
                .then(async () => {
                    console.log('Connected to SignalR hub');

                    await connection.invoke("JoinRunGroup", runId);

                    connection.off("AddTest");
                    connection.off("UpdateTest");
                    connection.on("AddTest", async (test: TestVm) => {
                        const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                        setFolders(folderData);
                        setTests(prevTests => [...prevTests, test]);
                        console.log("Received updated data:", test);
                    });

                    connection.on("UpdateTest", async (test: TestVm) => {
                        try {
                            setTests((prevTests) => {
                                const updatedTests = prevTests.map((t) => (t.id === test.id ? test : t));
                                console.log("Tests after update:", updatedTests); // Optional logging
                                return updatedTests;
                            });

                            console.log("Received updated data:", test);
                            // Optionally notify the user of the update (e.g., using a toast)
                        } catch (error) {
                            console.error("Error updating tests:", error);
                        }
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

    const openModalForCell = (mode: EditTestReviewMode, test: TestVm) => {
        setTestReviews([test.testReview]); // Only open for the selected test
        setEditMode(mode); // Set the mode based on the clicked cell
        setIsModalOpen(true); // Open the modal
        setSelectedTests([]);
    };

    const openModal = (mode: EditTestReviewMode) => {
        if (selectedTests.length === 0) {
            alert('Choose at least 1 test for edit');
            return;
        }
        const selectedReviews = tests.filter(test => selectedTests.includes(test.id)).map(test => test.testReview);
        setTestReviews(selectedReviews);
        setIsModalOpen(true);
        setEditMode(mode);
        setSelectedTests([]);
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);  // Close the modal
        setTestReviews([]);     // Clear the test reviews data
    };

    const renderFoldersAndTests = (parentId: number | null) => {
        const childFolders = folders.filter(folder => folder.parentId === parentId);
        const folderTests = tests.filter(test => test.folderId === parentId);

        return (
            <div className="folder-container">
                <table className="folder-table">
                    <thead>
                        <tr>
                            <th>
                                <input
                                    type="checkbox"
                                    checked={allSelected && selectedTests.length === tests.filter(test => test.folderId === currentFolderId).length}
                                    onChange={handleSelectAll}
                                    disabled={tests.filter(test => test.folderId === currentFolderId).length === 0}
                                />
                            </th>
                            <th>Name</th>
                            {childFolders.length === 0 && (
                                <>
                                    <th>Outcome</th>
                                    <th>Reviewer</th>
                                    <th>Comments</th>
                                </>
                            )}
                        </tr>
                    </thead>
                    <tbody>
                        {childFolders.map(folder => (
                            <tr key={folder.id} className="folder-row">
                                <td></td> {/* Empty cell for checkbox column */}
                                <td className="folder" onClick={() => openFolder(folder.id)}>
                                    {folder.name}
                                </td>
                                <td>({getTotalTestCountForFolder(folder.id)} tests)</td>
                            </tr>
                        ))}
                        {folderTests.map(test => (
                            <tr key={test.id} className="test-row">
                                <td
                                    onClick={(e) => {
                                        // Only toggle if the click is outside the checkbox itself
                                        if (e.target instanceof HTMLInputElement && e.target.type === "checkbox") {
                                            return; // If clicked on the checkbox, do nothing here
                                        }
                                        handleCheckboxChange(test.id); // Otherwise, toggle as usual
                                    }}
                                    style={{ cursor: "pointer" }}
                                >
                                    <input
                                        type="checkbox"
                                        checked={selectedTests.includes(test.id)}
                                        onChange={() => handleCheckboxChange(test.id)} // Handles direct checkbox clicks
                                    />
                                </td>
                                <td className="test-container">
                                    <span className="test-name" onClick={() => handleTestClick(test.id, test.folderId)}>
                                        {test.name}
                                    </span>
                                </td>
                                {renderTestOutcome(test.testReview.testReviewOutcome, test)}
                                {renderTestReviewer(test)}
                                {renderTestComments(test)}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    };

    const renderTestComments = (test: TestVm) => (
        <td
            className="test-comment-container"
            onClick={() => openModalForCell(EditTestReviewMode.comments, test)}
            style={{ cursor: 'pointer' }} // Add pointer cursor for clickable effect
        >
            {test.testReview.comments}
        </td>
    );

    const renderTestReviewer = (test: TestVm) => (
        <td
            className="test-reviewer-container"
            onClick={() => openModalForCell(EditTestReviewMode.reviewer, test)}
            style={{ cursor: 'pointer' }} // Add pointer cursor for clickable effect
        >
            {reviewerEmails[test.testReview.reviewerId] || '-'}
        </td>
    );

    const renderTestOutcome = (outcome: TestReviewOutcome, test: TestVm) => {
        return (
            <td
                className="test-review-outcome-container"
                onClick={() => openModalForCell(EditTestReviewMode.outcome, test)}
                style={{ cursor: 'pointer' }} // Add pointer cursor for clickable effect
            >
                {outcome === TestReviewOutcome.ToInvestigate && <span className="to-investigate">?</span>}
                {outcome === TestReviewOutcome.NotRepro && <span className="not-repro">NR</span>}
                {outcome === TestReviewOutcome.ProductBug && (
                    <div className="product-bug">
                        BUG
                        <span className="bug-id">{test.testReview.productBug?? ''}</span>
                    </div>
                )}
            </td>
        );
    };

    const getBreadcrumbPath = (): { name: string; id: number | null }[] => {
        const path: { name: string; id: number | null }[] = [];
        let currentFolder = folders.find(folder => folder.id === currentFolderId);

        // Traverse back up to the root folder
        while (currentFolder && currentFolder.name !== '$$Root$$') {
            path.unshift({ name: currentFolder.name, id: currentFolder.id });

            // Move to the parent folder if the parentId exists
            if (currentFolder.parentId !== undefined && currentFolder.parentId !== null) {
                currentFolder = folders.find(folder => folder.id === currentFolder?.parentId);
            } else {
                // Break out if there is no valid parentId
                break;
            }
        }

        // Add "Home" at the beginning of the path
        path.unshift({ name: 'Home', id: initialParentId });

        return path;
    };

    const openFolder = (folderId: number | null) => {
        if (folderId === null) return;
        const folder = folders.find(folder => folder.id === folderId);
        if (folder) {
            setCurrentFolderId(folderId);
            setSelectedTests([]); // Clear selected tests when switching folders
            setAllSelected(false); // Uncheck "Select All" as well
        }
    };

    const handleSelectAll = () => {
        const folderTests = tests.filter(test => test.folderId === currentFolderId);
        const allTestsSelected = selectedTests.length === folderTests.length;

        setAllSelected(!allTestsSelected);
        setSelectedTests(allTestsSelected ? [] : folderTests.map(test => test.id));
    };

    const handleCheckboxChange = (testId: number) => {
        setSelectedTests(prev =>
            prev.includes(testId)
                ? prev.filter(id => id !== testId) // Deselect if already selected
                : [...prev, testId] // Select if not already selected
        );
    };


    const handleActionChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const action = event.target.value;

        // Determine the EditTestReviewMode based on the selected action
        let mode: EditTestReviewMode | null = null;
        switch (action) {
            case "Set Outcome":
                mode = EditTestReviewMode.outcome;
                break;
            case "Select Reviewer":
                mode = EditTestReviewMode.reviewer;
                break;
            case "Add Comments":
                mode = EditTestReviewMode.comments;
                break;
            case "Update Selected Tests":
                mode = EditTestReviewMode.all;
                break;
            default:
                mode = null;
        }

        if (mode!=null) {
            openModal(mode);
        }

        // Reset the selected action to the placeholder option
        setSelectedAction(null);
    };

    const renderActions = () => (
        <div className="header-actions">
            <select onChange={handleActionChange} value={selectedAction || ""} className="action-select">
                <option value="" disabled>Select Action</option>
                <option value="Set Outcome">Set Outcome</option>
                <option value="Select Reviewer">Select Reviewer</option>
                <option value="Add Comments">Add Comments</option>
                <option value="Update Selected Tests"></option>
            </select>
        </div>
    );

    const renderBreadcrumb = () => {
        const breadcrumbPath = getBreadcrumbPath();
        return (
            <div className="back-navigation-container">
                {breadcrumbPath.map((folder, index) => (
                    <React.Fragment key={folder.id ?? 'home'}>
                        <span onClick={() => openFolder(folder.id)} className="breadcrumb-link">
                            {folder.name}
                        </span>
                        {index < breadcrumbPath.length - 1 && <span> / </span>}
                    </React.Fragment>
                ))}
            </div>
        );
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    const initialParentId = folders.find(folder => folder.name === '$$Root$$')?.id || null;

    return (
        <div className="run-page">
            <div className="run-header">
                <h1>{runName}</h1>
            </div>
            <div className="header-controls">
                {renderBreadcrumb()}
                {renderActions()}
            </div>
            {renderFoldersAndTests(currentFolderId !== null ? currentFolderId : initialParentId)}
            <EditTestReviewModal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                users={users}
                testReviews={testReviews}
                editMode={editMode}
            />
        </div>
    );
};

export default RunPage;