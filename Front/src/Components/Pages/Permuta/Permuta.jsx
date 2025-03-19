/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState, useRef } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import Select from 'react-select';
import { useAuth } from "../AuthContext";
import api from "./../axiosConfig";
import './Permuta.css';

function PermutaList(props) {
    const [searchText, setSearchText] = useState("");
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [permuta, setPermuta] = useState([]);
    const [escalas, setEscalas] = useState([]);
    const { permissoes } = useAuth();
    const possuiPermissao = (permissao) => permissoes.includes(permissao);

    // Estado para os filtros de status
    const [statusFilters, setStatusFilters] = useState({
        aguardandoSolicitado: false, // null
        aguardandoChefia: false,     // AprovadaSolicitado
        aprovadas: false,            // Aprovada
        recusadas: false,            // Recusada e RecusadaSolicitado
    });

    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });
    const API_URL = `${API_BASE_URL}/permutas`;

    function BuscarTodos() {
        api.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setPermuta(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os Permutas.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    function BuscarEscalas() {
        api.get(`${API_BASE_URL}/escala/buscarTodos`)
            .then((response) => {
                console.log("Escalas carregadas:", response.data);
                setEscalas(response.data);
            })
            .catch((error) => {
                console.error("Erro ao carregar escalas:", error);
            });
    }

    useEffect(() => {
        BuscarTodos();
        BuscarEscalas();
    }, []);

    // Função para manipular a mudança nos checkboxes
    const handleStatusFilterChange = (filter) => {
        setStatusFilters((prev) => ({
            ...prev,
            [filter]: !prev[filter],
        }));
    };

    // Função para filtrar os registros com base no texto de busca e nos filtros de status
    function filterRecords(records) {
        let filtered = records;

        // Aplicar filtro de status
        const activeFilters = Object.values(statusFilters).some((v) => v);
        if (activeFilters) {
            filtered = filtered.filter((record) => {
                if (statusFilters.aguardandoSolicitado && record.nmStatus === null) return true;
                if (statusFilters.aguardandoChefia && record.nmStatus === "AprovadaSolicitado") return true;
                if (statusFilters.aprovadas && record.nmStatus === "Aprovada") return true;
                if (statusFilters.recusadas && (record.nmStatus === "Recusada" || record.nmStatus === "RecusadaSolicitado")) return true;
                return false;
            });
        }

        // Aplicar filtro de texto
        if (searchText) {
            const search = searchText.toLowerCase();
            filtered = filtered.filter((record) => (
                record.nmNomeSolicitante?.toLowerCase().includes(search) ||
                record.nmNomeSolicitado?.toLowerCase().includes(search) ||
                record.dtDataSolicitadaTroca?.toLowerCase().includes(search) ||
                record.dtSolicitacao?.toLowerCase().includes(search)
            ));
        }

        return filtered;
    }

    function formatarData(dataISO) {
        if (!dataISO) return "";
        const data = new Date(dataISO);
        const dia = String(data.getUTCDate()).padStart(2, "0");
        const mes = String(data.getUTCMonth() + 1).padStart(2, "0");
        const ano = data.getUTCFullYear();
        return `${dia}-${mes}-${ano}`;
    }

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeletePermuta(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function handleAprovar(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar Aprovação",
            message: "Tem certeza que deseja Aprovar esta Permuta?",
            onConfirm: () => {
                AprovarPermuta(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function handleReprovar(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar Reprovação",
            message: "Tem certeza que deseja Reprovar esta Permuta?",
            onConfirm: () => {
                ReprovarPermuta(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeletePermuta(idPermuta) {
        api.delete(`${API_BASE_URL}/permutas/Deletar/${idPermuta}`)
            .then((response) => {
                console.log(response);
                setPermuta(permuta.filter((p) => p.id !== idPermuta));
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

    function AprovarPermuta(idPermuta) {
        api.put(`${API_BASE_URL}/permutas/Aprovar/${idPermuta}`)
            .then((response) => {
                console.log(response);
                setPermuta(permuta.filter((p) => p.id !== idPermuta));
                BuscarTodos();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Permuta Autorizada com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao Autorizar a Permuta.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
                console.error(error);
            });
    }

    function ReprovarPermuta(idPermuta) {
        api.put(`${API_BASE_URL}/permutas/Recusar/${idPermuta}`)
            .then((response) => {
                console.log(response);
                setPermuta(permuta.filter((p) => p.id !== idPermuta));
                BuscarTodos();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Permuta Recusada com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao Recusar a Permuta.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
                console.error(error);
            });
    }

    const filteredRecords = filterRecords(permuta);

    return (
        <>
            <h3 className="text-center mb-3">Lista de Permutas</h3>
            <div className="text-center mb-3">
                <button
                    onClick={() => props.ShowForm({})}
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
            <div className="mb-3 d-flex justify-content-center flex-wrap gap-3">
                <div className="form-check">
                    <input
                        type="checkbox"
                        className="form-check-input"
                        id="aguardandoSolicitado"
                        checked={statusFilters.aguardandoSolicitado}
                        onChange={() => handleStatusFilterChange("aguardandoSolicitado")}
                    />
                    <label className="form-check-label" htmlFor="aguardandoSolicitado">
                        Aguardando aprovação do solicitado
                    </label>
                </div>
                <div className="form-check">
                    <input
                        type="checkbox"
                        className="form-check-input"
                        id="aguardandoChefia"
                        checked={statusFilters.aguardandoChefia}
                        onChange={() => handleStatusFilterChange("aguardandoChefia")}
                    />
                    <label className="form-check-label" htmlFor="aguardandoChefia">
                        Aguardando Aprovação Chefia
                    </label>
                </div>
                <div className="form-check">
                    <input
                        type="checkbox"
                        className="form-check-input"
                        id="aprovadas"
                        checked={statusFilters.aprovadas}
                        onChange={() => handleStatusFilterChange("aprovadas")}
                    />
                    <label className="form-check-label" htmlFor="aprovadas">
                        Aprovadas
                    </label>
                </div>
                <div className="form-check">
                    <input
                        type="checkbox"
                        className="form-check-input"
                        id="recusadas"
                        checked={statusFilters.recusadas}
                        onChange={() => handleStatusFilterChange("recusadas")}
                    />
                    <label className="form-check-label" htmlFor="recusadas">
                        Recusadas
                    </label>
                </div>
            </div>
            <input
                type="text"
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
                placeholder="Pesquisar..."
                className="form-control mb-3"
            />
            <table className="table">
                <thead>
                    <tr>
                        <th style={{ width: '15%' }}>NOME SOLICITANTE</th>
                        <th style={{ width: '15%' }}>NOME SOLICITADO</th>
                        <th style={{ width: '10%' }}>DATA TROCA</th>
                        <th style={{ width: '8%' }}>DATA SOLICITAÇÃO</th>
                        <th style={{ width: '12%' }}>APROVADOR / REPROVADOR</th>
                        <th style={{ width: '8%' }}>DATA APROVAÇÃO</th>
                        <th style={{ width: '8%' }}>DATA REPROVAÇÃO</th>
                        <th style={{ width: '15%' }}>NOME ESCALA</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredRecords.map((p, index) => (
                        <tr key={index}>
                            <td style={{ textAlign: "left", width: '15%' }}>{p.nmNomeSolicitante}</td>
                            <td style={{ textAlign: "left", width: '15%' }}>{p.nmNomeSolicitado}</td>
                            <td style={{ textAlign: "left", width: '10%' }}>{formatarData(p.dtDataSolicitadaTroca)}</td>
                            <td style={{ textAlign: "left", width: '8%' }}>{formatarData(p.dtSolicitacao)}</td>
                            <td style={{ textAlign: "left", width: '12%' }}>{p.nmNomeAprovador}</td>
                            <td style={{ textAlign: "left", width: '8%' }}>{formatarData(p.dtAprovacao)}</td>
                            <td style={{ textAlign: "left", width: '8%' }}>{formatarData(p.dtReprovacao)}</td>
                            <td style={{ textAlign: "left", width: '15%' }}>{escalas.find((e) => e.idEscala === p.idEscala)?.nmNomeEscala || "Escala não encontrada"}</td>
                            <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                {possuiPermissao("EditarPermuta") && (
                                    <button
                                        onClick={() => handleAprovar(p.idPermuta)}
                                        type="button"
                                        className="btn btn-success btn-sm me-2 botaoPequeno"
                                    >
                                        Aprovar
                                    </button>
                                )}
                                {possuiPermissao("EditarPermuta") && (
                                    <button
                                        onClick={() => handleReprovar(p.idPermuta)}
                                        type="button"
                                        className="btn btn-warning btn-sm me-2 botaoPequeno"
                                    >
                                        Reprovar
                                    </button>
                                )}
                                {possuiPermissao("EditarPermuta") && (
                                    <button
                                        onClick={() => props.ShowForm(p)}
                                        type="button"
                                        className="btn btn-primary btn-sm me-2 botaoPequeno"
                                    >
                                        Editar
                                    </button>
                                )}
                                {possuiPermissao("DeletarPermuta") && (
                                    <button
                                        onClick={() => handleDelete(p.idPermuta)}
                                        type="button"
                                        className="btn btn-danger btn-sm botaoPequeno"
                                    >
                                        Deletar
                                    </button>
                                )}
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
        </>
    );
}


function PermutaForm(props) {
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;

    // Estados para os valores dos campos
    const [idDaEscala, setIdDaEscala] = useState(props.permuta.idEscala || '');
    const [nomeSolicitante, setNomeSolicitante] = useState(props.permuta.nmNomeSolicitante || '');
    const [nomeSolicitado, setNomeSolicitado] = useState(props.permuta.nmNomeSolicitado || '');
    const [nomeAprovador, setNomeAprovador] = useState(props.permuta.nmNomeAprovador || '');
    const [idFuncionarioSolicitante, setIdFuncionarioSolicitante] = useState(props.permuta.idFuncionarioSolicitante || '');
    const [idFuncionarioSolicitado, setIdFuncionarioSolicitado] = useState(props.permuta.idFuncionarioSolicitado || '');
    const [idFuncionarioAprovador, setIdFuncionarioAprovador] = useState(props.permuta.idFuncionarioAprovador || '');
    const [dtSolicitacao, setDtSolicitacao] = useState(props.permuta.dtSolicitacao || '');
    const [dtDataSolicitadaTroca, setDtDataSolicitadaTroca] = useState(props.permuta.dtDataSolicitadaTroca || '');
    const [dtAprovacao, setDtAprovacao] = useState(props.permuta.dtAprovacao || '');

    // Estados para as listas de opções
    const [funcionarios, setFuncionarios] = useState([]);
    const [escala, setEscala] = useState([]);
    const [datasTrabalhoSolicitante, setDatasTrabalhoSolicitante] = useState([]);
    const [datasTrabalhoSolicitado, setDatasTrabalhoSolicitado] = useState([]);
    const [funcionariosEscala, setFuncionariosEscala] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    // Estado para o AlertPopup
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    // Carregar dados iniciais
    useEffect(() => {
        const loadInitialData = async () => {
            await Promise.all([BuscarFuncionarios(), BuscaEscala()]);
            if (props.permuta.idEscala) {
                await BuscaEscalaPronta(props.permuta.idEscala);
            }
            setIsLoading(false);
        };
        loadInitialData();
    }, []);

    // Atualizar os estados com base nos props ao editar
    useEffect(() => {
        setIdDaEscala(props.permuta.idEscala || '');
        setNomeSolicitante(props.permuta.nmNomeSolicitante || '');
        setNomeSolicitado(props.permuta.nmNomeSolicitado || '');
        setNomeAprovador(props.permuta.nmNomeAprovador || '');
        setIdFuncionarioSolicitante(props.permuta.idFuncionarioSolicitante || '');
        setIdFuncionarioSolicitado(props.permuta.idFuncionarioSolicitado || '');
        setIdFuncionarioAprovador(props.permuta.idFuncionarioAprovador || '');
        setDtSolicitacao(props.permuta.dtSolicitacao || '');
        setDtDataSolicitadaTroca(props.permuta.dtDataSolicitadaTroca || '');
        setDtAprovacao(props.permuta.dtAprovacao || '');
    }, [props.permuta]);

    // Buscar funcionários da escala selecionada quando idDaEscala mudar
    useEffect(() => {
        if (idDaEscala && !isLoading) {
            BuscaEscalaPronta(idDaEscala);
        }
    }, [idDaEscala, isLoading]);

    // Funções de busca
    async function BuscarFuncionarios() {
        try {
            const response = await api.get(`${API_BASE_URL}/funcionario/buscarTodos`);
            setFuncionarios(response.data);
        } catch (error) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Não foi possível carregar a lista de funcionários.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
        }
    }

    async function BuscaEscala() {
        try {
            const response = await api.get(`${API_BASE_URL}/escala/buscarTodos`);
            const escalasAtivas = response.data.filter(e => e.isAtivo === true && e.isGerada === true);
            setEscala(escalasAtivas);
        } catch (error) {
            console.error(error);
        }
    }

    async function BuscaEscalaPronta(idEscala) {
        try {
            const response = await api.get(`${API_BASE_URL}/escalaPronta/buscarPorId/${idEscala}`);
            const escalaData = response.data;
            if (!Array.isArray(escalaData)) {
                console.error("Formato inesperado de escala:", escalaData);
                return;
            }

            const idsFuncionariosNaEscala = [...new Set(escalaData.map(dia => dia.idFuncionario))];
            const funcionariosNaEscala = funcionarios.filter(f => idsFuncionariosNaEscala.includes(f.idFuncionario));
            setFuncionariosEscala(funcionariosNaEscala);

            const mapDatasTrabalho = escalaData.reduce((acc, dia) => {
                if (!acc[dia.idFuncionario]) acc[dia.idFuncionario] = [];
                acc[dia.idFuncionario].push(dia.dtDataServico);
                return acc;
            }, {});

            // Garante que dtDataSolicitadaTroca esteja incluído nas opções ao editar
            const datasSolicitante = mapDatasTrabalho[idFuncionarioSolicitante] || [];
            if (props.permuta.dtDataSolicitadaTroca && !datasSolicitante.includes(props.permuta.dtDataSolicitadaTroca)) {
                datasSolicitante.push(props.permuta.dtDataSolicitadaTroca);
            }
            setDatasTrabalhoSolicitante(datasSolicitante);
            setDatasTrabalhoSolicitado(mapDatasTrabalho[idFuncionarioSolicitado] || []);
        } catch (error) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Falha ao carregar os funcionários da escala.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
        }
    }

    function getMesPorExtenso(mes) {
        const meses = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                       "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
        return meses[mes - 1] || "";
    }

    const escalaOptions = (escala || []).map(e => ({
        value: e.idEscala,
        label: `${e.nmNomeEscala} - ${getMesPorExtenso(e.nrMesReferencia)}`
    }));

    const funcionariosOptions = funcionariosEscala.map(f => ({
        value: f.idFuncionario,
        label: `${f.nmNome} - ${f.nrMatricula}`
    }));

    function SelectComFiltroSolicitante({ value, onChange }) {
        const opcoes = funcionariosOptions;
        return (
            <Select
                options={opcoes}
                placeholder="Digite para buscar..."
                value={opcoes.find(o => o.value === value) || null}
                onChange={(selectedOption) => {
                    if (selectedOption) {
                        setIdFuncionarioSolicitante(selectedOption.value);
                        setNomeSolicitante(selectedOption.label.split(' - ')[0]);
                        // Atualiza as datas apenas se o solicitante mudar
                        const novasDatas = datasTrabalhoSolicitante.filter(d => d !== dtDataSolicitadaTroca);
                        if (props.permuta.dtDataSolicitadaTroca) novasDatas.push(props.permuta.dtDataSolicitadaTroca);
                        setDatasTrabalhoSolicitante(novasDatas);
                    } else {
                        setIdFuncionarioSolicitante('');
                        setNomeSolicitante('');
                        setDatasTrabalhoSolicitante([]);
                    }
                    onChange(selectedOption ? selectedOption.value : null);
                }}
                isClearable
                noOptionsMessage={() => "Nenhum funcionário disponível"}
            />
        );
    }

    function SelectComFiltroSolicitado({ value, onChange }) {
        const opcoes = funcionariosOptions;
        return (
            <Select
                options={opcoes}
                placeholder="Digite para buscar..."
                value={opcoes.find(o => o.value === value) || null}
                onChange={(selectedOption) => {
                    if (selectedOption) {
                        setIdFuncionarioSolicitado(selectedOption.value);
                        setNomeSolicitado(selectedOption.label.split(' - ')[0]);
                    } else {
                        setIdFuncionarioSolicitado('');
                        setNomeSolicitado('');
                    }
                    onChange(selectedOption ? selectedOption.value : null);
                }}
                isClearable
                noOptionsMessage={() => "Nenhum funcionário na escala"}
            />
        );
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (idFuncionarioSolicitante === idFuncionarioSolicitado) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "O solicitante e o solicitado não podem ser a mesma pessoa.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
            return;
        }

        if (datasTrabalhoSolicitado.includes(dtDataSolicitadaTroca)) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "O funcionário solicitado já está trabalhando nesta data.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
            return;
        }

        const data = {
            idEscala: idDaEscala,
            idFuncionarioSolicitante: idFuncionarioSolicitante,
            nmNomeSolicitante: nomeSolicitante,
            idFuncionarioSolicitado: idFuncionarioSolicitado,
            nmNomeSolicitado: nomeSolicitado,
            dtSolicitacao: new Date().toISOString(),
            dtDataSolicitadaTroca: new Date(dtDataSolicitadaTroca).toISOString(),
        };

        if (props.permuta.idPermuta) {
            api.patch(`${API_BASE_URL}/permutas/Atualizar/${props.permuta.idPermuta}`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Permuta atualizada com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList();
                        },
                    });
                })
                .catch((error) => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao atualizar a Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error("Erro ao atualizar permuta:", error);
                });
        } else {
            api.post(`${API_BASE_URL}/permutas/Incluir`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Permuta cadastrada com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList();
                        },
                    });
                })
                .catch((error) => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao cadastrar a Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error("Erro ao salvar permuta:", error);
                });
        }
    };

    if (isLoading) {
        return <div>Carregando...</div>;
    }

    return (
        <>
            <h2 className="text-center mb-3">
                {props.permuta.idPermuta ? "Editar Permuta" : "Cadastrar Nova Permuta"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={handleSubmit}>
                        {props.permuta.idPermuta && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idPermuta"
                                        defaultValue={props.permuta.idPermuta}
                                        required
                                    />
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Escalas</label>
                            <div className="col-sm-8">
                                <Select
                                    options={escalaOptions}
                                    placeholder="Selecione uma escala"
                                    value={escalaOptions.find(option => option.value === idDaEscala) || null}
                                    onChange={(selectedOption) => {
                                        if (selectedOption) {
                                            setIdDaEscala(selectedOption.value);
                                            BuscaEscalaPronta(selectedOption.value);
                                        } else {
                                            setIdDaEscala('');
                                        }
                                    }}
                                    isClearable
                                    noOptionsMessage={() => "Nenhuma escala encontrada"}
                                />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitante</label>
                            <div className="col-sm-8">
                                <SelectComFiltroSolicitante
                                    value={idFuncionarioSolicitante}
                                    onChange={(id) => {
                                        const funcionarioSelecionado = funcionarios.find(f => f.idFuncionario === id);
                                        if (funcionarioSelecionado) {
                                            setIdFuncionarioSolicitante(funcionarioSelecionado.idFuncionario);
                                            setNomeSolicitante(funcionarioSelecionado.nmNome);
                                        } else {
                                            setIdFuncionarioSolicitante('');
                                            setNomeSolicitante('');
                                        }
                                    }}
                                />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitado</label>
                            <div className="col-sm-8">
                                <SelectComFiltroSolicitado
                                    value={idFuncionarioSolicitado}
                                    onChange={(id) => {
                                        const funcionarioSelecionado = funcionarios.find(f => f.idFuncionario === id);
                                        if (funcionarioSelecionado) {
                                            setIdFuncionarioSolicitado(funcionarioSelecionado.idFuncionario);
                                            setNomeSolicitado(funcionarioSelecionado.nmNome);
                                        } else {
                                            setIdFuncionarioSolicitado('');
                                            setNomeSolicitado('');
                                        }
                                    }}
                                />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data da Troca</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    value={dtDataSolicitadaTroca}
                                    onChange={(e) => setDtDataSolicitadaTroca(e.target.value)}
                                >
                                    <option value="" disabled>Selecione uma data</option>
                                    {datasTrabalhoSolicitante.length > 0 ? (
                                        datasTrabalhoSolicitante.map((dia, index) => {
                                            const dataFormatada = new Intl.DateTimeFormat("pt-BR", {
                                                day: "2-digit",
                                                month: "2-digit",
                                                year: "numeric",
                                            }).format(new Date(dia));
                                            return (
                                                <option key={`${index}-${dia}`} value={dia}>
                                                    {dataFormatada}
                                                </option>
                                            );
                                        })
                                    ) : (
                                        <option disabled>Sem datas disponíveis</option>
                                    )}
                                </select>
                            </div>
                        </div>

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

PermutaForm.propTypes = {
    ShowList: PropTypes.func.isRequired,
    permuta: PropTypes.shape({
        idPermuta: PropTypes.string,
        nmNomeSolicitante: PropTypes.string,
        nmNomeSolicitado: PropTypes.string,
        nmNomeAprovador: PropTypes.string,
        idFuncionarioSolicitante: PropTypes.string,
        idFuncionarioSolicitado: PropTypes.string,
        idFuncionarioAprovador: PropTypes.string,
        idEscala: PropTypes.string,
        dtSolicitacao: PropTypes.string,
        dtDataSolicitadaTroca: PropTypes.string,
        dtAprovacao: PropTypes.string,
    }).isRequired,
};

// Exportação do Permuta permanece igual
export function Permuta() {
    const [content, setContent] = useState(<PermutaList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<PermutaList ShowForm={ShowForm} />);
    }

    function ShowForm(permuta) {
        setContent(<PermutaForm permuta={permuta} ShowList={ShowList} ShowForm={ShowForm} />);
    }

    return <div className="container">{content}</div>;
}