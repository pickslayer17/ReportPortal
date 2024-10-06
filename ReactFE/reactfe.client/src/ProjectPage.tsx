import './App.css';
import './ProjectPage.css'; // Assume we have styles for ProjectPage
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const apiUrl = import.meta.env.VITE_API_URL;

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
                const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];
                const response = await fetch(`${apiUrl}/api/RunManagement/Project/${projectId}/Runs`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch runs');
                }

                const data: Run[] = await response.json();
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