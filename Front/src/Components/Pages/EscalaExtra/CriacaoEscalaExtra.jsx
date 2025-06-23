
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
    const { nomeUsuario } = useAuth();

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
                DeleteEscalaExtra(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeleteEscalaExtra(idCriacaoEscalaExtra) {
        api
            .delete(`${API_BASE_URL}/escalaExtra/Deletar/${idCriacaoEscalaExtra}`)
            .then((response) => {
                setEscalasExtras(
                    escalasExtras.filter((usuario) => usuario.id !== idCriacaoEscalaExtra)
                );
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
    }, []);

    // Função para formatar a data
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

            <table className="table">
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>Data do Extra</th>
                        <th>Data Abertura</th>
                        <th>Data Fechamento</th>
                        <th>Setor</th>
                        <th>Vagas</th> {/* NOVO: Cabeçalho para Qtd Vagas */}
                        <th>Ativo</th>
                        <th>Ações</th> {/* Adicionado para melhor clareza das colunas */}
                    </tr>
                </thead>
                <tbody>
                    {escalasExtras.map((escala, index) => (
                        <tr key={index}>
                            <td>{escala.nmEscalaExtra}</td>
                            <td>{formatDate(escala.dtEscalaExtra, true)}</td>
                            <td>{formatDate(escala.dtAbertura, true)}</td>
                            <td>{formatDate(escala.dtFechamento, true)}</td>
                            <td>{setor.find(s => s.idSetor === escala.idSetor)?.nmNome || "Setor não encontrado"}</td>
                            <td>{escala.qtdVagas}</td> {/* NOVO: Exibindo Qtd Vagas */}
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
            <AlertPopup
                type={alertProps.type}
                title={alertProps.title}
                message={alertProps.message}
                show={alertProps.show}
                onConfirm={alertProps.onConfirm}
                onClose={alertProps.onClose}
            />
        </div>
    );
}

// Componente para o formulário de criação de escala extra
function CriacaoEscalaExtraForm(props) {
    const { nomeUsuario } = useAuth();

    CriacaoEscalaExtraForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        EscalaExtra: PropTypes.shape({ // Alterado para EscalaExtra para refletir o nome da prop
            idCriacaoEscalaExtra: PropTypes.string,
            nmEscalaExtra: PropTypes.string,
            dtEscalaExtra: PropTypes.string,
            dtAbertura: PropTypes.string,
            dtFechamento: PropTypes.string,
            horaDoServico: PropTypes.string,
            horaAbertura: PropTypes.string,
            horaFechamento: PropTypes.string,
            idSetor: PropTypes.string,
            isAtivo: PropTypes.bool,
            qtdVagas: PropTypes.number, // NOVO: Adicionado ao PropTypes
        }),
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
    const [qtdVagas, setQtdVagas] = useState(0); // NOVO: Estado para qtdVagas

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
            setAtivo(props.EscalaExtra.isAtivo || true);
            setQtdVagas(props.EscalaExtra.qtdVagas || 0); // NOVO: Popula qtdVagas
        }
    }, [props.EscalaExtra]);

    useEffect(() => {
        BuscarSetor();
    }, []);

    const API_URL_Setor = `${API_BASE_URL}/setor`;
    function BuscarSetor() {
        api.get(`${API_URL_Setor}/buscarTodos`)
            .then((response) => {
                setSetor(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    useEffect(() => {
        if (props.EscalaExtra && props.EscalaExtra.idSetor) {
            setSetorSelecionado(props.EscalaExtra.idSetor);
        }
    }, [props.EscalaExtra]);

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    function handleQtdVagasChange(e) { // NOVO: Handler para qtdVagas
        setQtdVagas(Number(e.target.value)); // Converte para número
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        const data = {
            nmEscalaExtra: nomeEscala, // Ajuste para nmEscalaExtra (camelCase)
            dtEscalaExtra: dataEscala,
            dtAbertura: dataAbertura,
            dtFechamento: dataFechamento,
            horaDoServico: horaDoServico,
            horaAbertura: horaInicio,
            horaFechamento: horaFim,
            idSetor: setorSelecionado, // Ajuste para idSetor (camelCase)
            nomeFuncionario: nomeUsuario,
            isAtivo: ativo,
            qtdVagas: qtdVagas, // NOVO: Adicionado qtdVagas
        };

        // Verifique se props.EscalaExtra está presente antes de acessar idCriacaoEscalaExtra
        if (props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra) {
            // Atualização (editar)
            api.patch(
                `${API_BASE_URL}/escalaExtra/Atualizar/` + props.EscalaExtra.idCriacaoEscalaExtra,
                data
            )
                .then((response) => {
                    // Verifique se a resposta contém a propriedade 'Valido'
                    if (response.data && response.data.valido) {
                        setAlertProps({
                            show: true,
                            type: "success",
                            title: "Sucesso",
                            message: "Escala Extra atualizada com sucesso!",
                            onClose: () => {
                                setAlertProps((prev) => ({ ...prev, show: false }));
                                props.ShowList();
                            },
                        });
                    } else {
                        console.log('Erro no backend:', response.data.Mensagem);
                        setAlertProps({
                            show: true,
                            type: "error",
                            title: "Erro",
                            message: response.data.Mensagem || "Falha ao atualizar a Escala Extra.",
                            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                        });
                    }
                })
                .catch((error) => {
                    console.error('Erro ao chamar a API:', error);
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao atualizar a Escala Extra.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        }
        else {
            // Criar nova
            api.post(`${API_BASE_URL}/escalaExtra/Incluir`, data)
                .then((response) => {
                    if (response.data && response.data.valido) {
                        setAlertProps({
                            show: true,
                            type: "success",
                            title: "Sucesso",
                            message: "Escala Extra cadastrada com sucesso!",
                            onClose: () => {
                                setAlertProps((prev) => ({ ...prev, show: false }));
                                props.ShowList();
                            },
                        });
                    } else {
                        setAlertProps({
                            show: true,
                            type: "error",
                            title: "Erro",
                            message: response.data.Mensagem || "Falha ao cadastrar Escala Extra.",
                            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                        });
                    }
                })
                .catch((error) => {
                    console.error(error);
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao cadastrar Escala Extra.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        }
    };

    useEffect(() => {
        if (props.EscalaExtra) {
            // Atualizando a hora de Abertura
            if (props.EscalaExtra.dtAbertura) {
                const dtAbertura = new Date(props.EscalaExtra.dtAbertura);
                const horaBrasiliaAbertura = new Date(dtAbertura.getTime());
                const horaFormatadaAbertura = horaBrasiliaAbertura.getHours().toString().padStart(2, "0") + ":00";
                setHoraInicio(horaFormatadaAbertura);
            }

            // Atualizando a hora de Fechamento
            if (props.EscalaExtra.dtFechamento) {
                const dtFechamento = new Date(props.EscalaExtra.dtFechamento);
                const horaBrasiliaFechamento = new Date(dtFechamento.getTime());
                const horaFormatadaFechamento = horaBrasiliaFechamento.getHours().toString().padStart(2, "0") + ":00";
                setHoraFim(horaFormatadaFechamento);
            }

            
            if (props.EscalaExtra.dtEscalaExtra) {
                const dtEscalaExtra = new Date(props.EscalaExtra.dtEscalaExtra);
                const horaFormatadaServico = dtEscalaExtra.getHours().toString().padStart(2, "0") + ":00";
                setHoraDoServico(horaFormatadaServico);
            }
        }
    }, [props.EscalaExtra]);


    return (
        <>
            <h2 className="text-center mb-3">
                {props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra
                    ? "Editar Escala Extra"
                    : "Cadastrar Nova Escala Extra"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {/* Campo ID (apenas para editar) */}
                        {props.EscalaExtra && props.EscalaExtra.idCriacaoEscalaExtra && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idCriacaoEscalaExtra"
                                        defaultValue={props.EscalaExtra.idCriacaoEscalaExtra}
                                        required
                                    // A remoção do onChange aqui é intencional para campos readOnly
                                    ></input>
                                </div>
                            </div>
                        )}

                        {/* Campo Nome da Escala Extra */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala Extra</label>
                            <div className="col-sm-8">
                                <input
                                    type="text"
                                    className="form-control"
                                    name="nmEscalaExtra"
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
                                    name="dtEscalaExtra"
                                    value={dataEscala.split('T')[0]} // Pega apenas a parte da data (yyyy-mm-dd)
                                    onChange={(e) => setDataEscala(e.target.value)}
                                    required
                                />
                            </div>
                        </div>

                        {/* Campo Hora Inicio da Escala Extra (horaDoServico) */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Início do Extra</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    value={horaDoServico}
                                    onChange={(e) => setHoraDoServico(e.target.value)}
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


                        {/* Campo Data Abertura*/}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Abertura</label>
                            <div className="col-sm-8">
                                <input
                                    type="date"
                                    className="form-control"
                                    name="dtAbertura"
                                    value={dataAbertura.split('T')[0]}
                                    onChange={(e) => setDataAbertura(e.target.value)}
                                    required
                                />
                            </div>
                        </div>

                        {/* Campo Hora Abertura */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Abertura</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    value={horaInicio}
                                    onChange={(e) => setHoraInicio(e.target.value)}
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

                        {/* Campo Data Fechamento*/}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data Fechamento</label>
                            <div className="col-sm-8">
                                <input
                                    type="date"
                                    className="form-control"
                                    name="dtFechamento"
                                    value={dataFechamento.split('T')[0]}
                                    onChange={(e) => setDataFechamento(e.target.value)}
                                    required
                                />
                            </div>
                        </div>

                        {/* Campo Hora Fechamento */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Hora Fechamento</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    value={horaFim}
                                    onChange={(e) => setHoraFim(e.target.value)}
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

                        {/* NOVO: Campo Quantidade de Vagas */}
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Vagas</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    className="form-control"
                                    name="qtdVagas"
                                    value={qtdVagas}
                                    onChange={handleQtdVagasChange}
                                    min="0" // Garante que o número de vagas não seja negativo
                                    required
                                />
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

// Componente principal para alternar entre a listagem e o formulário
export function CriacaoEscalaExtraPage() {
    const [content, setContent] = useState(<CriacaoEscalaExtraList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<CriacaoEscalaExtraList ShowForm={ShowForm} />);
    }

    function ShowForm(escala) {
        setContent(<CriacaoEscalaExtraForm EscalaExtra={escala} ShowList={ShowList} ShowForm={ShowForm} />);
    }

    return <div className="container">{content}</div>;
}