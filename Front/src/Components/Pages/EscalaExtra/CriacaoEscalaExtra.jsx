import { useState, useEffect } from 'react';
import axios from 'axios';
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import api from './../axiosConfig';


// Componente para listar as escalas extras
function CriacaoEscalaExtraList({ ShowForm }) {
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [escalasExtras, setEscalasExtras] = useState([]);

    useEffect(() => {
        // Carregar escalas extras
        axios.get(`${API_BASE_URL}/escalaExtra`)
            .then(response => {
                setEscalasExtras(response.data);
            })
            .catch(error => {
                console.error('Erro ao carregar escalas extras', error);
            });
    }, []);

    return (
        <div>
            <h3 className="text-center mb-3">Escalas Extras Cadastradas</h3>
            <div className="text-center mb-3">
                    <button 
                        onClick={() => ShowForm({})}
                        type="button"
                        className="btn btn-primary me-2"
                        >
                        Cadastrar
                    </button>
                    <button
                        //onClick={() => BuscarTodos()}
                        type="button"
                        className="btn btn-outline-primary me-2"
                        >
                        Atualizar
                    </button>
                </div>
            
            <table className="table">
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>Data</th>
                        <th>Data Abertura</th>
                        <th>Data Fechamento</th>
                        <th>Setor</th>
                        <th>Ativo</th>
                    </tr>
                </thead>
                <tbody>
                    {escalasExtras.map((escala, index) => (
                        <tr key={index}>
                            <td>{escala.nmEscalaExtra}</td>
                            <td>{escala.dtEscalaExtra}</td>
                            <td>{escala.dtAbertura}</td>
                            <td>{escala.dtFechamento}</td>
                            <td>{escala.nomeSetor}</td>
                            <td>{escala.ativo}</td>
                            <td>
                                <input type="checkbox" checked={escala.isAtivo} readOnly />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>            
        </div>
    );
}

// Componente para o formulário de criação de escala extra
function CriacaoEscalaExtraForm(props) {
    CriacaoEscalaExtraForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        ShowForm: PropTypes.func.isRequired,
        escalasExtras: PropTypes.array.isRequired,  // Agora o componente espera a lista de escalas extras
    };

    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    // Campos do formulário
    const [dataEscala, setDataEscala] = useState('');
    const [nomeEscala, setNomeEscala] = useState('');
    const [setor, setSetor] = useState('');
    const [horaInicio, sethoraInicio] = useState('');
    const [horaFim, sethoraFim] = useState('');
    const [ativo, setAtivo] = useState(true);
    const [setores, setSetores] = useState([]);

    useEffect(() => {
        // Carregar setores       

        axios.get(`${API_BASE_URL}/setores`)
            .then(response => {
                setSetores(response.data);
            })
            .catch(error => {
                console.error('Erro ao carregar setores', error);
            });
    }, []);

    const handleAtivoChange = (e) => {
        setAtivo(e.target.checked);
    };

    const handleSave = () => {
        const data = {
            NmEscalaExtra: nomeEscala,
            DtEscalaExtra: dataEscala,

            IdSetor: setor,
            IsAtivo: ativo,
        };

        // Salvando a nova criação
        api.post(`${API_BASE_URL}/criacaoEscalaExtra`, data)
            .then(response => {
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Escala extra criada com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
                props.ShowList();
            })
            .catch(error => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao criar a escala extra.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    };

    return (
        <>
            <h3 className="text-center mb-3">Criar Nova Escala Extra</h3>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => e.preventDefault()}>
                        {/* Campo Nome da Escala Extra */}
                        <div className="mb-3">
                            <label className="form-label">Nome da Escala Extra</label>
                            <input
                                type="text"
                                className="form-control"
                                value={nomeEscala}
                                onChange={(e) => setNomeEscala(e.target.value)}
                            />
                        </div>
                        
                        {/* Campo Data */}
                        <div className="mb-3">
                            <label className="form-label">Data</label>
                            <input
                                type="date"
                                className="form-control"
                                value={dataEscala}
                                onChange={(e) => setDataEscala(e.target.value)}
                            />
                        </div>

                        {/* Campo Hora Inicio da Escala Extra */}
                        <div className="mb-3">
                            <label className="form-label">Hora Início</label>
                            <input
                                type="text"
                                className="form-control"
                                value={horaInicio}
                                onChange={(e) => sethoraInicio(e.target.value)}
                            />
                        </div>

                        {/* Campo Hora Fim da Escala Extra */}
                        <div className="mb-3">
                            <label className="form-label">Hora Fim</label>
                            <input
                                type="text"
                                className="form-control"
                                value={horaFim}
                                onChange={(e) => sethoraFim(e.target.value)}
                            />
                        </div>
                        
                        {/* Campo Setor */}
                        <div className="mb-3">
                            <label className="form-label">Setor</label>
                            <select
                                className="form-select"
                                value={setor}
                                onChange={(e) => setSetor(e.target.value)}
                            >
                                <option value="">Selecione um setor</option>
                                {setores.map((setor) => (
                                    <option key={setor.idSetor} value={setor.idSetor}>
                                        {setor.nomeSetor}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Campo Ativo */}
                        <div className="mb-3">
                            <label className="form-check-label">Ativo</label>
                            <input
                                type="checkbox"
                                className="form-check-input"
                                checked={ativo}
                                onChange={handleAtivoChange}
                            />
                        </div>

                        {/* Botão Salvar */}
                        <div className="row">
                            <div className="offset-sm-4 col-sm-4 d-grid">
                                <button type="submit" className="btn btn-primary btn-sm me-3">
                                    Salvar
                                </button>
                            </div>
                            <div className="col-sm-4 d-grid">
                                <button
                                    onClick={() => props.ShowList()}
                                    type="button"
                                    className="btn btn-danger me-2"
                                >
                                    Cancelar
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>

            <AlertPopup
                type={alertProps.type}
                title={alertProps.title}
                message={alertProps.message}
                show={alertProps.show}
                onClose={alertProps.onClose}
            />
        </>
    );
}


// Componente de navegação entre listagem e formulário
export function CriacaoEscalaExtraPage() {
    const [content, setContent] = useState(<CriacaoEscalaExtraList ShowForm={ShowForm} />); 
    const [escalasExtras, setEscalasExtras] = useState([]);

    useEffect(() => {
        // Carregar escalas extras
        axios.get(`${import.meta.env.VITE_BACKEND_API}/escalaExtra`)
            .then(response => {
                setEscalasExtras(response.data);
            })
            .catch(error => {
                console.error('Erro ao carregar escalas extras', error);
            });
    }, []);

    function ShowList() {
        setContent(<CriacaoEscalaExtraList ShowForm={ShowForm} />);
    }

    function ShowForm(escalaExtra) {
        setContent(
            <CriacaoEscalaExtraForm
                escalaExtra={escalaExtra}
                ShowList={ShowList}
                escalasExtras={escalasExtras} // Passando escalasExtras para o formulário
            />
        );
    }

    return <div className="container">{content}</div>;
}

export default CriacaoEscalaExtraPage;
