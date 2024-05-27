import { useEffect, useState } from 'react';
import './App.css';
import axios from 'axios'

interface Team {
    id: number;
    title: string;
    imageUrl: string;
    url: string;
    origin: string;
}

function App() {
    const [teams, setTeams] = useState<Team[]>([]);

    useEffect(() => {
        getTeams();
    }, []);

    const contents = teams === undefined
        ? <p>Loading... </p>
        :
        <div>
            {teams.map(team =>
                <p key={team.id}>
                    <td>{team.title}</td>
                    <td>{team.imageUrl}</td>
                    <td>{team.url}</td>
                    <td>{team.origin}</td>
                </p>
            )}
        </div>;

    return (
        <div>
            <h1 id="tabelLabel">Team list</h1>
            {contents}
        </div>
    );

    async function getTeams() {
        const response = await axios.get('teams');
        setTeams(response.data);
    }
}

export default App;