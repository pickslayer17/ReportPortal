import './App.css';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

const apiUrl = import.meta.env.VITE_API_URL;

interface RunVm {
    id: number;
    name: string;
    projectId: number;
}

function ProjectPage() {
    const { projectId } = useParams<{ projectId: string }>(); // Retrieve projectId from route params
    const [runs, setRuns] = useState<RunVm[]>([]);

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

                const data: RunVm[] = await response.json();
                setRuns(data); // Set the runs list
            } catch (error) {
                console.error('Failed to fetch runs:', error);
            }
        };

        if (projectId) {
            fetchRuns(); // Fetch runs when projectId is available
        }
    }, [projectId]);

    return (
        <div>
            <h2>Project {projectId} - Runs</h2>
            <ul>
                {runs.map(run => (
                    <li key={run.id}>{run.name}</li>
                ))}
            </ul>
        </div>
    );
}

export default ProjectPage;