import './App.css';
import './ProjectPage.css';
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';

interface Run {
    id: number;
    name: string;
    projectId: number;
}

function ProjectPage() {
    const { projectId } = useParams<{ projectId: string }>();
    const [runs, setRuns] = useState<Run[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchRuns = async () => {
            try {
                const data = await fetchWithToken(`api/RunManagement/Project/${projectId}/Runs`);
                setRuns(data);
            } catch (error) {
                console.error('Failed to fetch runs:', error);
            }
        };

        fetchRuns();
    }, [projectId]);

    const handleRunClick = (runId: number) => {
        navigate(`/RunPage/${runId}`);
    };

    return (
        <div className="project-page">
            <div className="project-header">
                <h1>Project Runs</h1>
            </div>
            <table className="run-table"> {/* Update class for consistent styling */}
                <thead>
                    <tr>
                        <th className="run-header">Run Name</th> {/* Add headers for alignment */}
                    </tr>
                </thead>
                <tbody>
                    {runs.map((run) => (
                        <tr key={run.id} className="run-row">
                            <td className="run-item" onClick={() => handleRunClick(run.id)}>
                                {run.name}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default ProjectPage;