import './App.css';
import './ProjectPage.css'; // Assume we have styles for ProjectPage
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchWithToken } from './helpers/api';

interface Run {
    id: number;
    name: string;
    projectId: number;
}

function ProjectPage() {
    const { projectId } = useParams<{ projectId: string }>(); // Retrieve projectId from URL params
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
            <div className="runs-list">
                {runs.map((run) => (
                    <div key={run.id} className="run-item" onClick={() => handleRunClick(run.id)}>
                        {run.name}
                    </div>
                ))}
            </div>
        </div>
    );
}

export default ProjectPage;