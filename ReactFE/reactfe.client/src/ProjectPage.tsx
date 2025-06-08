import './App.css';
import './ProjectPage.css';
import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchWithToken, uploadWithToken } from './helpers/api';

interface Run {
    id: number;
    name: string;
    projectId: number;
}

function ProjectPage() {
    const { projectId } = useParams<{ projectId: string }>();
    const [runs, setRuns] = useState<Run[]>([]);
    const navigate = useNavigate();
    const fileInputRef = useRef<HTMLInputElement>(null);

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

    const handleUploadClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file) {
            // Сбросить значение input
            event.target.value = '';
            return;
        }

        if (!file.name.endsWith('.trx')) {
            alert('Please select a .trx file.');
            event.target.value = '';
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        try {
            await uploadWithToken(
                `api/RunManagement/Project/${projectId}/upload-trx`,
                formData
            );
            alert('Файл успешно загружен!');
        } catch (error) {
            alert('Ошибка загрузки файла');
            console.error(error);
        } finally {
            // Сбросить значение input после загрузки
            event.target.value = '';
        }
    };

    return (
        <div className="project-page">
            <div className="project-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <h1>Project Runs</h1>
                <div>
                    <button className="upload-btn" onClick={handleUploadClick}>
                        Upload trx file
                    </button>
                    <input
                        type="file"
                        accept=".trx"
                        style={{ display: 'none' }}
                        ref={fileInputRef}
                        onChange={handleFileChange}
                    />
                </div>
            </div>
            <table className="run-table">
                <thead>
                    <tr>
                        <th className="run-header">Run Name</th>
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