import './App.css';
import { useParams } from 'react-router-dom';

function RunPage() {
    const { runId } = useParams<{ runId: string }>();

    return (
        <div>
            <h1>Run Details</h1>
            <p>Details for Run ID: {runId}</p>
            {/* Add more details or API fetch for this specific run here */}
        </div>
    );
}

export default RunPage;