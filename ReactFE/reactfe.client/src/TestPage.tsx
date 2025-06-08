import './TestPage.css';
import React, { useEffect, useState } from 'react';
import { fetchWithToken } from './helpers/api';
import { TestResultVm } from './interfaces/TestResultVmProps';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { vscDarkPlus } from 'react-syntax-highlighter/dist/esm/styles/prism';

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

const outcomeMap: Record<string | number, string> = {
    0: 'Passed',
    1: 'Failed',
    2: 'Skipped',
    // ...добавьте остальные значения, если есть
};

function renderStackTrace(stackTrace?: string) {
    if (!stackTrace) return null;
    return (
        <SyntaxHighlighter
            language="csharp"
            style={vscDarkPlus}
            customStyle={{
                borderRadius: 8,
                fontSize: 14,
                marginTop: 8,
                marginBottom: 8,
                background: '#23272e',
                padding: 16,
                lineHeight: 1.5,
                wordBreak: 'break-all',
            }}
            showLineNumbers
        >
            {stackTrace}
        </SyntaxHighlighter>
    );
}

const TestPage: React.FC<TestPageProps> = ({ testId, folderId, onClose, isModal }) => {
    const [test, setTest] = useState<TestVm | null>(null);
    const [testResults, setTestResults] = useState<TestResultVm[]>([]);
    const [activeTab, setActiveTab] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [showFullError, setShowFullError] = useState(false);

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
                            <p><strong>Outcome:</strong> {outcomeMap[testResults[activeTab].testOutcome] ?? testResults[activeTab].testOutcome}</p>
                            <p>
                                <strong>Error Message:</strong>
                                <div
                                    style={{
                                        maxHeight: showFullError ? 'none' : 80,
                                        overflow: 'hidden',
                                        whiteSpace: 'pre-wrap',
                                        position: 'relative',
                                        background: '#222',
                                        borderRadius: 6,
                                        padding: 8,
                                        marginTop: 4,
                                        marginBottom: 4,
                                        cursor: 'pointer'
                                    }}
                                    onClick={() => setShowFullError(v => !v)}
                                    title="Click to expand/collapse"
                                >
                                    {testResults[activeTab].errorMessage}
                                    {!showFullError && (
                                        <div style={{
                                            position: 'absolute',
                                            bottom: 0,
                                            left: 0,
                                            width: '100%',
                                            height: 24,
                                            background: 'linear-gradient(transparent, #222 80%)'
                                        }} />
                                    )}
                                </div>
                                <span
                                    style={{ color: '#4af', cursor: 'pointer', fontSize: 12 }}
                                    onClick={() => setShowFullError(v => !v)}
                                >
                                    {showFullError ? 'Скрыть' : 'Показать полностью'}
                                </span>
                            </p>
                            {testResults[activeTab].screenShot && (
                                <div>
                                    <strong>Screenshot:</strong>
                                    <img src={`data:image/png;base64,${testResults[activeTab].screenShot}`} alt="Test screenshot" />
                                </div>
                            )}
                            <p><strong>Stack Trace:</strong> {renderStackTrace(testResults[activeTab].stackTrace)}</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default TestPage;