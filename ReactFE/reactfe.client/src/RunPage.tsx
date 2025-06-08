import './RunPage.css';
import './App.css';
import './dropdown.css';

import { FaTrash } from 'react-icons/fa';
import EditTestReviewModal from './EditTestReviewModal';
import { EditTestReviewMode } from './enums/EditTestReviewMode'
import { TestVm, TestReviewOutcome, TestReviewVm } from './interfaces/TestVmProps';
import { UserVm } from './interfaces/UserVmProps';
import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { deleteWithToken } from './helpers/api';
import { fetchWithToken } from './helpers/api';
import { putWithToken } from './helpers/api'; // обязательно импортировать
import * as signalR from '@microsoft/signalr'; // Import SignalR
import { testOutcome } from './enums/testOutcome';
import { capitalizeFirstLetter } from './helpers/render';

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

const outcomeOptions = [
    { value: TestReviewOutcome.ToInvestigate, label: 'To Investigate' },
    { value: TestReviewOutcome.NotRepro, label: 'Not Repro' },
    { value: TestReviewOutcome.ProductBug, label: 'Product Bug' }
];

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
    const [editingOutcomeId, setEditingOutcomeId] = useState<number | null>(null);
    const [editingReviewerId, setEditingReviewerId] = useState<number | null>(null);
    const [bugNumber, setBugNumber] = useState<string>(''); // для Product Bug

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
                    connection.off("RemoveFolder");
                    connection.off("RemoveTest");
                    connection.on("AddTest", async (test: TestVm) => {
                        const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                        setFolders(folderData);
                        setTests(prevTests => [...prevTests, test]);
                        console.log("Received updated data:", test);
                    });
                    connection.on("RemoveTest", async (testId: number) => {
                        const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                        setTests(testData);
                        console.log("Test was removed:", testId);
                    });

                    connection.on("RemoveFolder", async (folder: FolderVm) => {
                        const folderData: FolderVm[] = await fetchWithToken(`api/FolderManagement/Runs/${runId}/folders`);
                        setFolders(folderData);
                        const testData: TestVm[] = await fetchWithToken(`api/TestManagement/Runs/${runId}/tests`);
                        setTests(testData);
                        console.log("Folder was removed:", folder);
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

                    connection.on("UpdateTestResult", async (test: TestVm) => {
                        try {
                            setTests((prevTests) => {
                                const updatedTests = prevTests.map((t) =>
                                    t.id === test.id
                                        ? { ...t, testResults: [...test.testResults] } // Only update testResults with a new array reference
                                        : t
                                );
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

    const getAllTestsForFolder = (folderId: number): TestVm[] => {
        const directTests = tests.filter(test => test.folderId === folderId);
        const childFolders = folders.filter(folder => folder.parentId === folderId);
        const childTests = childFolders.flatMap(childFolder => getAllTestsForFolder(childFolder.id));
        return [...directTests, ...childTests];
    };

    const handleDeleteTest = (testId: number) => {
        if (window.confirm('Are you sure you want to delete this test id: ' + testId + '?')) {
            deleteWithToken(`api/TestManagement/tests/${testId}`)
        }
    };

    const handleDeleteFolder = (folderId: number) => {
        if (window.confirm('Are you sure you want to delete this folder id: ' + folderId + '?')) {
            deleteWithToken(`api/FolderManagement/folder/${folderId}/delete`)
        }
    };

    const renderFolderTable = (childFolders: FolderVm[], currentFolderId: number | null) => {
        return (
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
                        <th>Total</th>
                        <th>Passed</th>
                        <th>Failed</th>
                        <th>Not Run</th>
                        <th className="toinvestigate-col"><span className="to-investigate">?</span></th>
                        <th className="notrepro-col"><span className="not-repro">NR</span></th>
                        <th className="productbug-col"><div className="product-bug">BUG</div></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {childFolders.map(folder => {
                        // Get all tests for the folder and its child folders
                        const folderTests = getAllTestsForFolder(folder.id);

                        // Calculate totals for TestOutcome
                        const outcomeCounts = folderTests.reduce(
                            (acc, test) => {
                                const latestTestResult = test.testResults?.[test.testResults.length - 1]?.testOutcome;
                                if (latestTestResult === testOutcome.Passed) acc.passed += 1;
                                else if (latestTestResult === testOutcome.Failed) acc.failed += 1;
                                else if (latestTestResult === testOutcome.NotRun) acc.notRun += 1;
                                return acc;
                            },
                            { passed: 0, failed: 0, notRun: 0 }
                        );

                        // Calculate totals for TestReviewOutcome
                        const reviewCounts = folderTests.reduce(
                            (acc, test) => {
                                const reviewOutcome = test.testReview?.testReviewOutcome;
                                if (reviewOutcome === TestReviewOutcome.ToInvestigate) acc.toInvestigate += 1;
                                else if (reviewOutcome === TestReviewOutcome.NotRepro) acc.notRepro += 1;
                                else if (reviewOutcome === TestReviewOutcome.ProductBug) acc.productBug += 1;
                                return acc;
                            },
                            { toInvestigate: 0, notRepro: 0, productBug: 0 }
                        );

                        return (
                            <tr key={folder.id} className="folder-row">
                                <td></td> {/* Empty cell for checkbox column */}
                                <td className="folder" onClick={() => openFolder(folder.id)}>
                                    {capitalizeFirstLetter(folder.name)}
                                </td>
                                <td>({folderTests.length} tests)</td>
                                <td>{outcomeCounts.passed}</td>
                                <td>{outcomeCounts.failed}</td>
                                <td>{outcomeCounts.notRun}</td>
                                <td className="toinvestigate-col">{reviewCounts.toInvestigate}</td>
                                <td className="notrepro-col">{reviewCounts.notRepro}</td>
                                <td className="productbug-col">{reviewCounts.productBug}</td>
                                <td>
                                    <button
                                        className="delete-button"
                                        onClick={() => handleDeleteFolder(folder.id)}>
                                        <FaTrash /> { }
                                    </button>
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        );
    };

    const renderTestTable = (folderTests: TestVm[]) => {
        return (
            <table className="folder-table test-table">
                <thead>
                    <tr>
                        <th className="col-checkbox">
                            <input
                                type="checkbox"
                                checked={allSelected && selectedTests.length === tests.filter(test => test.folderId === currentFolderId).length}
                                onChange={handleSelectAll}
                                disabled={tests.filter(test => test.folderId === currentFolderId).length === 0}
                            />
                        </th>
                        <th className="col-name">Name</th>
                        <th className="col-outcome">Outcome</th>
                        <th className="col-reviewer">Reviewer</th>
                        <th className="col-comments">Comments</th>
                        <th className="col-delete"></th>
                    </tr>
                </thead>
                <tbody>
                    {folderTests.map(test => (
                        <tr key={test.id} className="test-row">
                            <td className="col-checkbox"
                                onClick={(e) => {
                                    if (e.target instanceof HTMLInputElement && e.target.type === "checkbox") {
                                        return;
                                    }
                                    handleCheckboxChange(test.id);
                                }}
                                style={{ cursor: "pointer" }}
                            >
                                <input
                                    type="checkbox"
                                    checked={selectedTests.includes(test.id)}
                                    onChange={() => handleCheckboxChange(test.id)}
                                />
                            </td>
                            <td className="test-container col-name">
                                <span
                                    className="test-name"
                                    title={test.name}
                                    onClick={() => handleTestClick(test.id, test.folderId)}
                                >
                                    {capitalizeFirstLetter(test.name)}
                                </span>
                            </td>
                            <td className="col-outcome">
                                {editingOutcomeId === test.id ? (
                                    <select
                                        className="outcome-span styled-dropdown"
                                        value={test.testReview.testReviewOutcome}
                                        onChange={e => {
                                            const val = Number(e.target.value);
                                            handleOutcomeChange(test, val);
                                        }}
                                        autoFocus
                                        onBlur={() => setEditingOutcomeId(null)}
                                    >
                                        {outcomeOptions.map(opt => (
                                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                                        ))}
                                    </select>
                                ) : (
                                    <span
                                        className="outcome-span"
                                        onClick={() => setEditingOutcomeId(test.id)}
                                        title="Изменить Outcome"
                                    >
                                        {test.testReview.testReviewOutcome === TestReviewOutcome.ToInvestigate && <span className="to-investigate">To Investigate</span>}
                                        {test.testReview.testReviewOutcome === TestReviewOutcome.NotRepro && <span className="not-repro">Not Repro</span>}
                                        {test.testReview.testReviewOutcome === TestReviewOutcome.ProductBug && (
                                            <div className="product-bug">
                                                Product Bug
                                                <span className="bug-id">{test.testReview.productBug ?? ''}</span>
                                            </div>
                                        )}
                                    </span>
                                )}
                            </td>
                            <td className="col-reviewer">
                                {editingReviewerId === test.id ? (
                                    <select
                                        className="reviewer-span styled-dropdown"
                                        value={test.testReview.reviewerId ?? ''}
                                        onChange={e => handleReviewerChange(test, Number(e.target.value))}
                                        onBlur={() => setEditingReviewerId(null)}
                                        autoFocus
                                    >
                                        <option value="">-</option>
                                        {users.map(u => (
                                            <option key={u.id} value={u.id}>{u.email}</option>
                                        ))}
                                    </select>
                                ) : (
                                    <span
                                        className="reviewer-span"
                                        onClick={() => setEditingReviewerId(test.id)}
                                        title="Изменить Reviewer"
                                    >
                                        {reviewerEmails[test.testReview.reviewerId] || '-'}
                                    </span>
                                )}
                            </td>
                            <td className="col-comments">
                                <span
                                    className="comments-span"
                                    onClick={() => openModalForCell(EditTestReviewMode.comments, test)}
                                    title="Изменить Comments"
                                >
                                    {test.testReview.comments}
                                </span>
                            </td>
                            <td className="col-delete">
                                <button
                                    className="delete-button"
                                    onClick={() => handleDeleteTest(test.id)}>
                                    <FaTrash /> { }
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        );
    };

    const renderFoldersAndTests = (parentId: number | null) => {
        const childFolders = folders.filter(folder => folder.parentId === parentId);
        const folderTests = tests.filter(test => test.folderId === parentId);

        return (
            <div className="folder-container">
                {folderTests.length > 0
                    ? renderTestTable(folderTests)
                    : renderFolderTable(childFolders, parentId)}
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
            path.unshift({ name: capitalizeFirstLetter(currentFolder.name), id: currentFolder.id });

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

    const handleOutcomeChange = async (test: TestVm, newOutcome: TestReviewOutcome) => {
        setEditingOutcomeId(null);

        // Если выбран Product Bug — открываем попап с выбранным Product Bug
        if (newOutcome === TestReviewOutcome.ProductBug) {
            // Передаём в testReview нужный outcome
            setTestReviews([
                {
                    ...test.testReview,
                    testReviewOutcome: TestReviewOutcome.ProductBug
                }
            ]);
            setEditMode(EditTestReviewMode.outcome);
            setIsModalOpen(true);
            setSelectedTests([]);
            return;
        }

        const updatedReview = {
            ...test.testReview,
            testReviewOutcome: newOutcome,
            productBug: null
        };

        try {
            await putWithToken(
                `api/TestReviewManagement/TestReview/${test.testReview.id}/UpdateOutcome`,
                updatedReview
            );
            setTests(prev =>
                prev.map(t =>
                    t.id === test.id
                        ? { ...t, testReview: updatedReview }
                        : t
                )
            );
        } catch (error) {
            console.error('Failed to update outcome', error);
        }
    };

    const handleReviewerChange = async (test: TestVm, newReviewerId: number) => {
        setEditingReviewerId(null);
        const updatedReview = {
            ...test.testReview,
            reviewerId: newReviewerId
        };
        try {
            await putWithToken(
                `api/TestReviewManagement/TestReview/${test.testReview.id}/UpdateReviewer`,
                updatedReview
            );
            setTests(prev =>
                prev.map(t =>
                    t.id === test.id
                        ? { ...t, testReview: updatedReview }
                        : t
                )
            );
        } catch (error) {
            // обработка ошибки (можно добавить toast)
            console.error('Failed to update reviewer', error);
        }
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