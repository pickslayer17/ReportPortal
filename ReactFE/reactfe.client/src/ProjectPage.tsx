import { useParams } from 'react-router-dom';
import './App.css';

function ProjectPage() {
    const { id } = useParams(); // Get the project ID from the URL

    return (
        <div className="project-page">
            <h1>Project {id}</h1>
            {/* Display project details here */}
        </div>
    );
}

export default ProjectPage;