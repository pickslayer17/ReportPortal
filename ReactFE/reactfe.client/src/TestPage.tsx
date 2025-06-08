import './TestPage.css';
import React, { useEffect, useState } from 'react';
import { fetchWithToken } from './helpers/api';
import { TestResultVm } from './interfaces/TestResultVmProps';

interface TestVm {
    id: number;
    folderId: number;
    path: string;
    runId: number;
    name: string;
}

interface TestPageProps {
    testId: number;
    folderId?: number;
    onClose?: () => void;
    isModal?: boolean;
}

const TestPage: React.FC<TestPageProps> = ({ testId, folderId, onClose, isModal }) => {
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

    if (loading) {
        return <p>Loading...</p>;
    }

    if (!test) {
        return <p>No test data available.</p>;
    }

    return (
        <div className="test-page">
            <h1>
                {test.name}
                {isModal && onClose && (
                    <button onClick={onClose} className="testpage-close-btn">Закрыть</button>
                )}
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