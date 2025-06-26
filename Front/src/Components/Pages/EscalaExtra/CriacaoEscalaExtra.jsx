import { useAuth } from "../../Pages/AuthContext";
import { useState, useEffect } from 'react';
import axios from 'axios';
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import api from './../axiosConfig';

// Componente para listar as escalas extras
function CriacaoEscalaExtraList(props) {
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [escalasExtras, setEscalasExtras] = useState([]);
    const [setor, setSetor] = useState([]);
    const [cargos, setCargos] = useState([]);
    const { nomeUsuario } = useAuth();

    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    function BuscarCargos() {
        const fetchData = async () => {
            try {
                const response = await api.get(`${API_BASE_URL}/cargo/buscarTodos`);
                setCargos(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }

    function BuscarSetor() {
        api.get(`${API_BASE_URL}/setor/buscarTodos`)
            .then((response) => {
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
                console.log('extras',response.data);
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

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeleteEscalaExtra(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeleteEscalaExtra(idCriacaoEscalaExtra) {
        api.delete(`${API_BASE_URL}/escalaExtra/Deletar/${idCriacaoEscalaExtra}`)
            .then(() => {
                BuscarTodos();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Registro excluído com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao excluir o registro.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
                console.error(error);
            });
    }

    useEffect(() => {
        BuscarSetor();
        BuscarTodos();
        BuscarCargos();
    }, []);

    function formatDate(dateString, includeTime = false) {
        const date = new Date(dateString);
        const options = { year: 'numeric', month: '2-digit', day: '2-digit' };

        if (includeTime) {
            options.hour = '2-digit';
            options.minute = '2-digit';
        }
        return new Intl.DateTimeFormat('pt-BR', options).format(date);
    }

    return (
        <div>
            <h3 className="text-center mb-3">Escalas Extras Cadastradas</h3>
            <div className="text-center mb-3">
                <button
                    onClick={() => props.ShowForm(null)}
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

            <table className="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>Data do Extra</th>
                        <th>Data Abertura</th>
                        <th>Data Fechamento</th>
                        <th>Setor</th>
                        <th>Cargos</th>
                        <th>Vagas</th>
                        <th>Ativo</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {escalasExtras.map((escala, index) => (
                        <tr key={index}>
                            <td>{escala.nmEscalaExtra}</td>
                            <td>{formatDate(escala.dtEscalaExtra, true)}</td>
                            <td>{formatDate(escala.dtAbertura, true)}</td>
                            <td>{formatDate(escala.dtFechamento, true)}</td>
                            <td>{setor.find(s => s.idSetor === escala.idSetor)?.nmNome || "N/A"}</td>
                           <td>
                            {escala.idCargo && escala.idCargo.length > 0 ? (
                                <select className="form-select form-select-sm">
                                    <option>Ver Cargos ({escala.idCargo.length})</option>
                                    {escala.idCargo.map(idCargo => {
                                        const cargo = cargos.find(c => c.idCargo === idCargo);
                                        return cargo ? (
                                            <option key={idCargo}>{cargo.nmNome}</option>
                                        ) : null;
                                    })}
                                </select>
                            ) : (
                                "Nenhum cargo"
                            )}
                        </td>
                            <td>{escala.qtdVagas}</td>
                            <td>
                                <input type="checkbox" checked={escala.isAtivo} readOnly />
                            </td>
                            <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                <button
                                    onClick={() => props.ShowForm(escala)}
                                    type="button"
                                    className="btn btn-primary btn-sm me-2"
                                >
                                    Editar
                                </button>
                                <button
                                    onClick={() => handleDelete(escala.idCriacaoEscalaExtra)}
                                    type="button"
                                    className="btn btn-danger btn-sm"
                                >
                                    Delete
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <AlertPopup {...alertProps} />
        </div>
    );
}

// Componente para o formulário de criação de escala extra
function CriacaoEscalaExtraForm(props) {
    const { nomeUsuario } = useAuth();
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;

    CriacaoEscalaExtraForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        EscalaExtra: PropTypes.shape({
            idCriacaoEscalaExtra: PropTypes.string,
            nmEscalaExtra: PropTypes.string,
            dtEscalaExtra: PropTypes.string,
            dtAbertura: PropTypes.string,
            dtFechamento: PropTypes.string,
            horaDoServico: PropTypes.string,
            horaAbertura: PropTypes.string,
            horaFechamento: PropTypes.string,
            idSetor: PropTypes.string,
            cargos: PropTypes.arrayOf(PropTypes.shape({
                idCargo: PropTypes.string,
                nmNome: PropTypes.string,
            })),
            isAtivo: PropTypes.bool,
            qtdVagas: PropTypes.number,
        }),
    };

    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    // Estados dos campos do formulário (todos restaurados)
    const [setor, setSetor] = useState([]);
    const [nomeEscala, setNomeEscala] = useState('');
    const [dataEscala, setDataEscala] = useState('');
    const [dataAbertura, setDataAbertura] = useState('');
    const [dataFechamento, setDataFechamento] = useState('');
    const [horaDoServico, setHoraDoServico] = useState('');
    const [horaInicio, setHoraInicio] = useState('');
    const [horaFim, setHoraFim] = useState('');
    const [setorSelecionado, setSetorSelecionado] = useState('');
    const [ativo, setAtivo] = useState(true);
    const [qtdVagas, setQtdVagas] = useState(0);

    // Estados para gerenciar a seleção de múltiplos cargos
    const [cargosDisponiveis, setCargosDisponiveis] = useState([]);
    const [cargoParaAdicionar, setCargoParaAdicionar] = useState('');
    const [cargosSelecionados, setCargosSelecionados] = useState([]);

    // Popula o formulário ao editar (todos os campos restaurados)
    useEffect(() => {
    if (props.EscalaExtra) {
        setNomeEscala(props.EscalaExtra.nmEscalaExtra || '');
        setDataEscala(props.EscalaExtra.dtEscalaExtra || '');
        setDataAbertura(props.EscalaExtra.dtAbertura || '');
        setDataFechamento(props.EscalaExtra.dtFechamento || '');
        setHoraDoServico(props.EscalaExtra.horaDoServico || '');
        setHoraInicio(props.EscalaExtra.horaAbertura || '');
        setHoraFim(props.EscalaExtra.horaFechamento || '');
        setSetorSelecionado(props.EscalaExtra.idSetor || '');
        setAtivo(props.EscalaExtra.isAtivo === false ? false : true);
        setQtdVagas(props.EscalaExtra.qtdVagas || 0);

        // Carrega os cargos com base nos IDs recebidos
        if (props.EscalaExtra.idCargo && props.EscalaExtra.idCargo.length > 0) {
            const cargosCompletos = props.EscalaExtra.idCargo
                .map(id => cargosDisponiveis.find(c => c.idCargo === id))
                .filter(Boolean); // remove undefined

            setCargosSelecionados(cargosCompletos);
        } else {
            setCargosSelecionados([]);
        }
    }
}, [props.EscalaExtra, cargosDisponiveis]);

    // Busca os dados iniciais (setores e cargos)
    useEffect(() => {
        BuscarSetor();
        BuscarCargos();
    }, []);

    function BuscarCargos() {
        api.get(`${API_BASE_URL}/cargo/buscarTodos`)
            .then((response) => setCargosDisponiveis(response.data))
            .catch((error) => console.log(error));
    }

    function BuscarSetor() {
        api.get(`${API_BASE_URL}/setor/buscarTodos`)
            .then((response) => setSetor(response.data))
            .catch((error) => console.log(error));
    }

    const handleAdicionarCargo = () => {
        if (!cargoParaAdicionar) return;
        const cargoJaExiste = cargosSelecionados.some(c => c.idCargo === cargoParaAdicionar);
        if (cargoJaExiste) return;
        const cargoObj = cargosDisponiveis.find(c => c.idCargo === cargoParaAdicionar);
        if (cargoObj) {
            setCargosSelecionados([...cargosSelecionados, cargoObj]);
        }
    };

    const handleRemoverCargo = (idCargoParaRemover) => {
        setCargosSelecionados(cargosSelecionados.filter(c => c.idCargo !== idCargoParaRemover));
    };
    
    // Formatação de data/hora (restaurado)
    useEffect(() => {
        if (props.EscalaExtra) {
            if (props.EscalaExtra.dtAbertura) {
                const dtAbertura = new Date(props.EscalaExtra.dtAbertura);
                const horaFormatadaAbertura = dtAbertura.getHours().toString().padStart(2, "0") + ":00";
                setHoraInicio(horaFormatadaAbertura);
            }
            if (props.EscalaExtra.dtFechamento) {
                const dtFechamento = new Date(props.EscalaExtra.dtFechamento);
                const horaFormatadaFechamento = dtFechamento.getHours().toString().padStart(2, "0") + ":00";
                setHoraFim(horaFormatadaFechamento);
            }
            if (props.EscalaExtra.dtEscalaExtra) {
                const dtEscalaExtra = new Date(props.EscalaExtra.dtEscalaExtra);
                const horaFormatadaServico = dtEscalaExtra.getHours().toString().padStart(2, "0") + ":00";
                setHoraDoServico(horaFormatadaServico);
            }
        }
    }, [props.EscalaExtra]);

    const handleSubmit = (e) => {
    e.preventDefault();

    const data = {
        nmEscalaExtra: nomeEscala,
        dtEscalaExtra: dataEscala,
        dtAbertura: dataAbertura,
        dtFechamento: dataFechamento,
        horaDoServico: horaDoServico,
        horaAbertura: horaInicio,
        horaFechamento: horaFim,
        idSetor: setorSelecionado,
        nomeFuncionario: nomeUsuario,
        isAtivo: ativo,
        qtdVagas: qtdVagas,
        IdCargo: cargosSelecionados.map(c => c.idCargo), // Envia apenas os IDs
    };

    const isEditing = props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra;
    const url = isEditing
        ? `${API_BASE_URL}/escalaExtra/Atualizar/${props.EscalaExtra.idCriacaoEscalaExtra}`
        : `${API_BASE_URL}/escalaExtra/Incluir`;
    const method = isEditing ? api.patch : api.post;

    method(url, data)
        .then((response) => {
            if (response.data && response.data.valido) {
                setAlertProps({
                    show: true, type: "success", title: "Sucesso",
                    message: `Escala Extra ${isEditing ? 'atualizada' : 'cadastrada'} com sucesso!`,
                    onClose: () => {
                        setAlertProps((prev) => ({ ...prev, show: false }));
                        props.ShowList();
                    },
                });
            } else {
                setAlertProps({
                    show: true, type: "error", title: "Erro",
                    message: response.data.Mensagem || `Falha ao ${isEditing ? 'atualizar' : 'cadastrar'}.`,
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            }
        })
        .catch((error) => {
            console.error('Erro ao chamar a API:', error);
            setAlertProps({
                show: true, type: "error", title: "Erro",
                message: `Falha ao ${isEditing ? 'atualizar' : 'cadastrar'}.`,
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
        });
};

    return (
        <>
            <h2 className="text-center mb-3">
                {props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra
                    ? "Editar Escala Extra"
                    : "Cadastrar Nova Escala Extra"}
            </h2>
            <div className="row">
                <div className="col-lg-8 mx-auto">
                    <form onSubmit={handleSubmit}>
                        {/* Todos os campos originais foram restaurados */}
                        {props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input readOnly className="form-control-plaintext" defaultValue={props.EscalaExtra.idCriacaoEscalaExtra} />
                                </div>
                            </div>
                        )}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala Extra</label>
                            <div className="col-sm-8">
                                <input type="text" className="form-control" value={nomeEscala} onChange={(e) => setNomeEscala(e.target.value)} required />
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data do Extra</label>
                            <div className="col-sm-8">
                                <input type="date" className="form-control" value={dataEscala.split('T')[0]} onChange={(e) => setDataEscala(e.target.value)} required />
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Início do Extra</label>
                            <div className="col-sm-8">
                                <select className="form-control" value={horaDoServico} onChange={(e) => setHoraDoServico(e.target.value)} required>
                                    {Array.from({ length: 24 }, (_, i) => (<option key={i} value={`${i.toString().padStart(2, "0")}:00`}>{`${i.toString().padStart(2, "0")}:00`}</option>))}
                                </select>
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Abertura</label>
                            <div className="col-sm-8">
                                <input type="date" className="form-control" value={dataAbertura.split('T')[0]} onChange={(e) => setDataAbertura(e.target.value)} required />
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Abertura</label>
                            <div className="col-sm-8">
                                <select className="form-control" value={horaInicio} onChange={(e) => setHoraInicio(e.target.value)} required>
                                    {Array.from({ length: 24 }, (_, i) => (<option key={i} value={`${i.toString().padStart(2, "0")}:00`}>{`${i.toString().padStart(2, "0")}:00`}</option>))}
                                </select>
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Fechamento</label>
                            <div className="col-sm-8">
                                <input type="date" className="form-control" value={dataFechamento.split('T')[0]} onChange={(e) => setDataFechamento(e.target.value)} required />
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Fechamento</label>
                            <div className="col-sm-8">
                                <select className="form-control" value={horaFim} onChange={(e) => setHoraFim(e.target.value)} required>
                                    {Array.from({ length: 24 }, (_, i) => (<option key={i} value={`${i.toString().padStart(2, "0")}:00`}>{`${i.toString().padStart(2, "0")}:00`}</option>))}
                                </select>
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Vagas</label>
                            <div className="col-sm-8">
                                <input type="number" className="form-control" value={qtdVagas} onChange={(e) => setQtdVagas(Number(e.target.value))} min="0" required />
                            </div>
                        </div>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Setor</label>
                            <div className="col-sm-8">
                                <select className="form-control" value={setorSelecionado} onChange={(e) => setSetorSelecionado(e.target.value)} required>
                                    <option value="">Selecione um setor</option>
                                    {setor.map(s => (<option key={s.idSetor} value={s.idSetor}>{s.nmNome}</option>))}
                                </select>
                            </div>
                        </div>
                        
                        {/* Componente alterado para múltiplos cargos */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Cargos</label>
                            <div className="col-sm-8">
                                <div className="input-group">
                                    <select className="form-select" value={cargoParaAdicionar} onChange={(e) => setCargoParaAdicionar(e.target.value)}>
                                        <option value="">Selecione para adicionar...</option>
                                        {cargosDisponiveis.map(s => (<option key={s.idCargo} value={s.idCargo}>{s.nmNome}</option>))}
                                    </select>
                                    <button type="button" className="btn btn-outline-primary" onClick={handleAdicionarCargo}>Adicionar</button>
                                </div>
                                <div className="mt-2">
                                    {cargosSelecionados.length > 0 ? (
                                        <ul className="list-group">
                                            {cargosSelecionados.map(cargo => (
                                                <li key={cargo.idCargo} className="list-group-item d-flex justify-content-between align-items-center">
                                                    {cargo.nmNome}
                                                    <button type="button" className="btn btn-danger btn-sm" onClick={() => handleRemoverCargo(cargo.idCargo)}>&times;</button>
                                                </li>
                                            ))}
                                        </ul>
                                    ) : (
                                        <div className="text-muted small mt-1">Nenhum cargo selecionado.</div>
                                    )}
                                </div>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input type="checkbox" className="form-check-input" checked={ativo} onChange={(e) => setAtivo(e.target.checked)} />
                            </div>
                        </div>
                        <div className="row mt-4">
                            <div className="offset-sm-4 col-sm-4 d-grid">
                                <button type="submit" className="btn btn-primary">Salvar</button>
                            </div>
                            <div className="col-sm-4 d-grid">
                                <button type="button" className="btn btn-secondary" onClick={() => props.ShowList()}>Cancelar</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
            <AlertPopup {...alertProps} />
        </>
    );
}

// Componente principal
export function CriacaoEscalaExtraPage() {
    const [content, setContent] = useState(null);

    function ShowList() {
        setContent(<CriacaoEscalaExtraList ShowForm={ShowForm} />);
    }

    function ShowForm(escala) {
        setContent(<CriacaoEscalaExtraForm EscalaExtra={escala} ShowList={ShowList} />);
    }

    useEffect(() => {
        ShowList();
    }, []);

    return <div className="container my-4">{content}</div>;
}
