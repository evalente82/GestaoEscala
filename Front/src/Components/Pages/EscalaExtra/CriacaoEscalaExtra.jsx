import { useState, useEffect } from 'react';
import axios from 'axios';
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import api from './../axiosConfig';



// Componente para listar as escalas extras
function CriacaoEscalaExtraList({ ShowForm }) {
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [escalasExtras, setEscalasExtras] = useState([]);
    const [setor, setSetor] = useState([]);

    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });

    function BuscarSetor() {
        api.get(`${API_BASE_URL}/setor/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setSetor(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os Setores.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    function BuscarTodos() {
        api.get(`${API_BASE_URL}/escalaExtra/buscarExtras`)
            .then((response) => {
                console.log(response.data);
                setEscalasExtras(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as Escalas Extras.",
                });
            });
    }

    useEffect(() => {
            BuscarSetor();
        }, []); 
    useEffect(() => {
        // Carregar escalas extras
        axios.get(`${API_BASE_URL}/escalaExtra/buscarExtras`)
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
                        onClick={() => BuscarTodos()}
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
                            <td>{setor.find(s => s.idSetor === escala.idSetor)?.nmNome || "Setor não encontrado"}</td>
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
    const [setor, setSetor] = useState([]);
    const [setorSelecionado, setSetorSelecionado] = useState('');
    const [dataAbertura, setDataAbertura] = useState('');
    const [dataFechamento, setDataFechamento] = useState('');
    const [horaInicio, sethoraInicio] = useState('');
    const [horaFim, sethoraFim] = useState('');
    const [ativo, setAtivo] = useState(true);

     useEffect(() => {
        BuscarSetor();
    }, []);
    const API_URL_Setor = `${API_BASE_URL}/setor`;
    function BuscarSetor() {
        api.get(`${API_URL_Setor}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setSetor(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    useEffect(() => {
        if (props.escalasExtras.idSetor) {
            setSetorSelecionado(props.escalasExtras.idSetor.toString());
        }
    }, [props.escalasExtras.idSetor]);

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
    e.preventDefault();

    const data = [{
        NmEscalaExtra: nomeEscala,
        DtEscalaExtra: dataEscala,
        dtAbertura: dataAbertura,
        dtFechamento: dataFechamento,
        horaAbertura: horaInicio,
        horaFechamento: horaFim,
        IdSetor: setorSelecionado,  // Usando o idSetor selecionado
        IsAtivo: ativo,
    }];
console.log("Dados a serem enviados:", data);
    // Salvando a nova criação
    api.post(`${API_BASE_URL}/escalaExtra/Incluir`, data)
        .then((response) => {
            console.log(response); // Verifique a resposta para garantir que está correta
            setAlertProps({
                show: true,
                type: "success",
                title: "Sucesso",
                message: "Escala Extra cadastrada com sucesso!",
                onClose: () => {
                    setAlertProps((prev) => ({ ...prev, show: false }));
                    props.ShowList(); // Voltar para a lista após fechar a modal
                },
            });
        })
        .catch((error) => {
    if (error.response) {
        // A resposta do servidor está no erro
        console.error("Erro na requisição:", error.response.data);
    } else if (error.request) {
        // A requisição foi feita, mas não houve resposta
        console.error("Sem resposta do servidor:", error.request);
    } else {
        // Algum erro na configuração da requisição
        console.error("Erro ao configurar a requisição:", error.message);
    }
    setAlertProps({
        show: true,
        type: "error",
        title: "Erro",
        message: "Falha ao cadastrar Escala Extra.",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });
});
};

    return (
        <>
            <h3 className="text-center mb-3">Criar Nova Escala Extra</h3>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {/* Campo Nome da Escala Extra */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala Extra</label>
                             <div className="col-sm-8">
                                <input
                                type="text"
                                className="form-control"
                                value={nomeEscala}
                                onChange={(e) => setNomeEscala(e.target.value)}
                                required
                            />
                             </div>                            
                        </div>
                        
                        {/* Campo Data */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data do Extra</label>
                            <div className="col-sm-8">
                                <input
                                type="date"
                                className="form-control"
                                value={dataEscala}
                                onChange={(e) => setDataEscala(e.target.value)}
                                required
                            />
                            </div>                            
                        </div>

                        {/* Campo Data Abertura*/}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Abertura</label>
                            <div className="col-sm-8">
                                <input
                                type="date"
                                className="form-control"
                                value={dataAbertura}
                                onChange={(e) => setDataAbertura(e.target.value)}
                                required
                            />
                            </div>                            
                        </div>                        

                        {/* Campo Hora Inicio da Escala Extra */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Abertura</label>
                                <div className="col-sm-8">
                                    <select
                                    className="form-control"
                                    value={horaInicio}
                                    onChange={(e) => sethoraInicio(e.target.value)}
                                    required
                                    >
                                    {Array.from({ length: 24 }, (_, i) => {
                                        const hour = i.toString().padStart(2, "0") + ":00";
                                        return (
                                        <option key={hour} value={hour}>
                                            {hour}
                                        </option>
                                        );
                                    })}
                                    </select>
                                </div>
                            </div>


                        {/* Campo Data Fechameto*/}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Fechamento</label>
                            <div className="col-sm-8">
                                <input
                                type="date"
                                className="form-control"
                                value={dataFechamento}
                                onChange={(e) => setDataFechamento(e.target.value)}
                                required
                            />
                            </div>                            
                        </div>

                        {/* Campo Hora Fim da Escala Extra */}
                         <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Fechamento</label>
                                <div className="col-sm-8">
                                    <select
                                    className="form-control"
                                    value={horaFim}
                                    onChange={(e) => sethoraFim(e.target.value)}
                                    required
                                    >
                                    {Array.from({ length: 24 }, (_, i) => {
                                        const hour = i.toString().padStart(2, "0") + ":00";
                                        return (
                                        <option key={hour} value={hour}>
                                            {hour}
                                        </option>
                                        );
                                    })}
                                    </select>
                                </div>
                            </div>
                        
                        {/* Campo Setor */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Setor</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    name="setor"
                                    value={setorSelecionado}
                                    onChange={(e) => setSetorSelecionado(e.target.value)}
                                    required
                                >
                                    <option value="">Selecione um setor</option>
                                    {setor.map(s => (
                                        <option key={s.idSetor} value={s.idSetor}>{s.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        {/* Campo Ativo */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input
                                type="checkbox"
                                className="form-check-input"
                                checked={ativo}
                                onChange={handleAtivoChange}
                            />
                            </div>                            
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
