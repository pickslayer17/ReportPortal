import './TestPage.css';
import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';

interface TestVm {
    id: number;
    folderId: number;
    path: string;
    runId: number;
    name: string;
}

interface TestResultVm {
    id: number;
    testId: number;
    errorMessage: string;
    stackTrace: string;
    screenShot: string; // This will be a base64 string for the image
    testOutcome: 'Passed' | 'Failed' | 'NotRun';
}

const TestPage: React.FC = () => {
    const { testId } = useParams<{ testId: string }>();
    const location = useLocation();
    const navigate = useNavigate();
    const folderId = location.state?.folderId; // Get folderId from navigation state

    const [test, setTest] = useState<TestVm | null>(null);
    const [testResults, setTestResults] = useState<TestResultVm[]>([]);
    const [activeTab, setActiveTab] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchTest = async () => {
            try {
                const testData: TestVm = await fetchWithToken(`api/TestManagement/tests/${testId}`);
                setTest(testData);

                const testResultData: TestResultVm[] = await fetchWithToken(`api/TestResultManagement/test/${testId}/testResults`);
                setTestResults(testResultData);

                setActiveTab(0);
            } catch (error) {
                console.error('Error fetching test and test results:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchTest();
    }, [testId]);

    const goBack = () => {
        // Navigate back to the RunPage with the correct folderId
        navigate(`/RunPage/${test?.runId}`, { state: { folderId } });
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    if (!test) {
        return <p>No test data available.</p>;
    }

    return (
        <div className="test-page">
            
            <h1>{test.name}
                <button onClick={goBack} className="back-button">Back to Folder</button> {/* Add Back button */}
            </h1>
            <div className="test-results-tabs">
                <ul className="tabs-list">
                    {testResults.map((result, index) => (
                        <li
                            key={result.id}
                            className={`tab ${activeTab === index ? 'active' : ''}`}
                            onClick={() => setActiveTab(index)}
                        >
                            {`Result ${index + 1} - ${result.testOutcome}`}
                        </li>
                    ))}
                </ul>
                <div className="tab-content">
                    {activeTab !== null && testResults[activeTab] && (
                        <div>
                            <p><strong>Outcome:</strong> {testResults[activeTab].testOutcome}</p>
                            <p><strong>Error Message:</strong> {testResults[activeTab].errorMessage}</p>
                            {testResults[activeTab].screenShot && (
                                <div>
                                    <strong>Screenshot:</strong>
                                    <img src={`data:image/png;base64,${testResults[activeTab].screenShot}`} alt="Test screenshot" />
                                </div>
                            )}
                            <p><strong>Stack Trace:</strong> {testResults[activeTab].stackTrace}</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default TestPage;