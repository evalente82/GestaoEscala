import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import { useNavigate } from 'react-router-dom';
import { useAuth } from "../AuthContext";
import api from "./../axiosConfig";
import "./Escala.css";

function EscalaList(props) {
    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);
    const [recordsPerPage] = useState(10);
    const [searchText, setSearchText] = useState("");
    const [escala, setEscala] = useState([]);
    const [departamentos, setDepartamentos] = useState([]);
    const [cargos, setCargos] = useState([]);
    const [tipoEscalas, setTipoEscalas] = useState([]);
    const { permissoes } = useAuth();
    const possuiPermissao = (permissao) => permissoes.includes(permissao);
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const API_URL = `${API_BASE_URL}/escala`;
    const currentRecords = filterRecords(escala);

    const [isLoading, setIsLoading] = useState(false); // Estado de carregamento
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    function BuscarTodos() {
        setIsLoading(true);
        api.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setEscala(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as Escalas.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .finally(() => setIsLoading(false));
    }

    function BuscarDepartamentos() {
        const fetchData = async () => {
            try {
                const response = await api.get(`${API_BASE_URL}/departamento/buscarTodos`);
                setDepartamentos(response.data);
                console.log('Departamentos', response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }

    function Buscarcargos() {
        const fetchData = async () => {
            try {
                const response = await api.get(`${API_BASE_URL}/cargo/buscarTodos`);
                setCargos(response.data);
                console.log('Cargos', response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }

    function BuscarTipoEscalas() {
        const fetchData = async () => {
            try {
                const response = await api.get(`${API_BASE_URL}/tipoEscala/buscarTodos`);
                setTipoEscalas(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }

    EscalaList.propTypes = {
        ShowForm: PropTypes.func.isRequired,
        ShowMontaEscala: PropTypes.func.isRequired,
    };

    useEffect(() => {
        BuscarDepartamentos();
        Buscarcargos();
        BuscarTipoEscalas();
        BuscarTodos();
    }, []);

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeleteEscala(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeleteEscala(idEscala) {
        setIsLoading(true);
        api
            .delete(`${API_URL}/Deletar/${idEscala}`)
            .then((response) => {
                console.log(response);
                setEscala(escala.filter((usuario) => usuario.id !== idEscala));
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
            })
            .finally(() => setIsLoading(false));
    }

    const GeraMesSeguinte = (idEscala) => {
        if (idEscala) {
            setIsLoading(true);
            api
                .post(`${API_BASE_URL}/escalaPronta/RecriarEscalaProximoMes/${idEscala}`, {}, {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Escala do Mês seguinte construída com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            BuscarTodos();
                        },
                    });
                })
                .catch((error) => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao gerar a escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                })
                .finally(() => setIsLoading(false));
        }
    };

    function filterRecords(records) {
        const search = searchText?.toLowerCase() || "";
        return records.filter((record) => {
            const nomeEscala = record.nmNomeEscala?.toLowerCase() || "";
            const departamento = departamentos.find(dept => dept.idDepartamento === record.idDepartamento)?.nmNome.toLowerCase() || "";
            const cargo = cargos.find(c => c.idCargo === record.idCargo)?.nmNome.toLowerCase() || "";
            const tipoEscala = tipoEscalas.find(te => te.idTipoEscala === record.idTipoEscala)?.nmNome.toLowerCase() || "";
            const mesReferencia = getNomeMes(record.nrMesReferencia).toLowerCase() || "";
            return (
                nomeEscala.includes(search) ||
                departamento.includes(search) ||
                cargo.includes(search) ||
                tipoEscala.includes(search) ||
                mesReferencia.includes(search)
            );
        });
    }

    function getNomeMes(numeroMes) {
        const nomesMeses = [
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        ];
        return numeroMes >= 1 && numeroMes <= 12 ? nomesMeses[numeroMes - 1] : nomesMeses[new Date().getMonth()];
    }

    return (
        <>
            {isLoading && (
                <div className="loading-overlay">
                    <div className="spinner" />
                    <p>Carregando...</p>
                </div>
            )}
            <h3 className="text-center mb-3">Listagem de Escalas</h3>
            <div className="text-center mb-3">
                {possuiPermissao("CadastrarEscala") && (
                    <button
                        onClick={() => props.ShowForm({})}
                        type="button"
                        className="btn btn-primary me-2"
                        disabled={isLoading}
                    >
                        Cadastrar
                    </button>
                )}
                {possuiPermissao("CadastrarEscala") && (
                    <button
                        onClick={() => BuscarTodos()}
                        type="button"
                        className="btn btn-outline-primary me-2"
                        disabled={isLoading}
                    >
                        Atualizar
                    </button>
                )}
            </div>
            <br />
            <br />
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
                        <th>NOME</th>
                        <th>DEPARTAMENTO</th>
                        <th>CARGO</th>
                        <th>TIPO ESCALA</th>
                        <th>MÊS REFERÊNCIA</th>
                        <th>PESSOA POR POSTO</th>
                        <th>ATIVO</th>
                        <th>GERADA</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords.map((escala, index) => (
                        <tr key={index}>
                            <td>{escala.nmNomeEscala}</td>
                            <td>{departamentos.find(departamento => departamento.idDepartamento === escala.idDepartamento)?.nmNome}</td>
                            <td>{cargos.find(cargo => cargo.idCargo === escala.idCargo)?.nmNome}</td>
                            <td>{tipoEscalas.find(tipoEscala => tipoEscala.idTipoEscala === escala.idTipoEscala)?.nmNome}</td>
                            <td>{getNomeMes(escala.nrMesReferencia)}</td>
                            <td>{escala.nrPessoaPorPosto}</td>
                            <td><input type="checkbox" checked={escala.isAtivo == 1} readOnly /></td>
                            <td><input type="checkbox" checked={escala.isGerada == 1} readOnly /></td>
                            <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                {possuiPermissao("VisualizarEscalas") && (
                                    <button
                                        onClick={() => navigate(`/Exibicao/${escala.idEscala}`)}
                                        type="button"
                                        className="btn gerar-escala-btn-visualizar-escala btn-sm me-2"
                                        disabled={escala.isGerada == false || isLoading}
                                    >
                                        Visualizar Escala
                                    </button>
                                )}
                                {possuiPermissao("GerarEscalas") && (
                                    <button
                                        onClick={() => props.ShowMontaEscala(escala)}
                                        type="button"
                                        className="btn gerar-escala-btn-gerar-escala btn-sm me-2"
                                        disabled={escala.isGerada == true || isLoading}
                                    >
                                        Gerar Escala
                                    </button>
                                )}
                                {possuiPermissao("GerarEscalas") && (
                                    <button
                                        onClick={() => GeraMesSeguinte(escala.idEscala)}
                                        type="button"
                                        className="btn gerar-escala-btn-mes-seguinte btn-sm me-2"
                                        disabled={escala.isGerada == false || escala.isAtivo == false || isLoading}
                                    >
                                        Mês Seguinte
                                    </button>
                                )}
                                {possuiPermissao("EditarEscalas") && (
                                    <button
                                        onClick={() => props.ShowForm(escala)}
                                        type="button"
                                        className="btn btn-primary btn-sm me-2"
                                        disabled={isLoading}
                                    >
                                        Editar
                                    </button>
                                )}
                                {possuiPermissao("DeletarEscalas") && (
                                    <button
                                        onClick={() => handleDelete(escala.idEscala)}
                                        type="button"
                                        className="btn btn-danger btn-sm"
                                        disabled={isLoading}
                                    >
                                        Delete
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

function EscalaForm(props) {
    EscalaForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        escala: PropTypes.shape({
            idEscala: PropTypes.number,
            nmNomeEscala: PropTypes.string,
            idDepartamento: PropTypes.number,
            idCargo: PropTypes.string,
            idTipoEscala: PropTypes.number,
            nrMesReferencia: PropTypes.number,
            nrPessoaPorPosto: PropTypes.number,
            isAtivo: PropTypes.bool,
            isGerada: PropTypes.bool,
        }).isRequired,
    };

    useEffect(() => {
        setNome(props.escala.nmNomeEscala || '');
        setMesReferencia(props.escala.nrMesReferencia || '');
        setPessoaPorPosto(props.escala.nrPessoaPorPosto || '');
        setAtivo(props.escala.isAtivo || false);
        setGerada(props.escala.isGerada || false);
        setDepartamentoSelecionado(props.escala.idDepartamento || '');
        setCargoSelecionado(props.escala.idCargo || '');
        setTipoEscalaSelecionado(props.escala.idTipoEscala || '');
    }, [props.escala]);

    const [nome, setNome] = useState(props.escala.nmNomeEscala || '');
    const [mesReferencia, setMesReferencia] = useState(props.escala.nrMesReferencia || '');
    const [pessoaPorPosto, setPessoaPorPosto] = useState(props.escala.nrPessoaPorPosto || '');
    const [departamentos, setDepartamentos] = useState([]);
    const [cargos, setCargos] = useState([]);
    const [tipoEscalas, setTipoEscalas] = useState([]);
    const [departamentoSelecionado, setDepartamentoSelecionado] = useState('');
    const [cargoSelecionado, setCargoSelecionado] = useState('');
    const [tipoEscalaSelecionado, setTipoEscalaSelecionado] = useState('');
    const [ativo, setAtivo] = useState(props.escala.isAtivo || false);
    const [gerada, setGerada] = useState(props.escala.isGerada || false);
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    useEffect(() => {
        BuscarDepartametos();
    }, []);
    const API_URL = `${API_BASE_URL}/departamento`;
    function BuscarDepartametos() {
        api.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(`DEPARTAMENTO `);
                console.log(response.data);
                setDepartamentos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarCargos();
    }, []);
    const API_URL_Carcos = `${API_BASE_URL}/cargo`;
    function BuscarCargos() {
        api.get(`${API_URL_Carcos}/buscarTodos`)
            .then((response) => {
                console.log(`CARGOS`);
                console.log(response.data);
                setCargos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarTipoEscala();
    }, []);
    const API_URL_TipoEscala = `${API_BASE_URL}/tipoEscala`;
    function BuscarTipoEscala() {
        api.get(`${API_URL_TipoEscala}/buscarTodos`)
            .then((response) => {
                console.log(`TIPOESCALA`);
                console.log(response.data);
                setTipoEscalas(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    var nomesMeses = [
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSelectChange = (e) => {
        const selectedValue = e.target.value;
        setCargoSelecionado(selectedValue);
        console.log(selectedValue);
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.escala.idEscala) {
            const data = {
                nmNomeEscala: nome,
                idDepartamento: departamentoSelecionado,
                idCargo: cargoSelecionado,
                idTipoEscala: tipoEscalaSelecionado,
                nrMesReferencia: mesReferencia,
                nrPessoaPorPosto: pessoaPorPosto,
                isAtivo: ativo,
                isGerada: gerada,
            };
            console.log("Dados enviados alterar:", data);
            api
                .patch(
                    `${API_BASE_URL}/escala/Atualizar/` + props.escala.idEscala,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Escala atualizada com sucesso!",
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
                        message: "Falha ao atualizar a Escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNomeEscala: nome,
                idDepartamento: departamentoSelecionado,
                idCargo: cargoSelecionado,
                idTipoEscala: tipoEscalaSelecionado,
                nrMesReferencia: mesReferencia,
                nrPessoaPorPosto: pessoaPorPosto,
                isAtivo: ativo,
                isGerada: gerada,
            };
            console.log("Dados enviados:", data);
            api
                .post(`${API_BASE_URL}/escala/Incluir`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Escala cadastrada com sucesso!",
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
                        message: "Falha ao cadastrar a Escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };

    return (
        <>
            <h2 className="text-center mb-3">
                {props.escala.idEscala ? "Editar Escala" : "Cadastrar Nova Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.escala.idEscala && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idEscala"
                                        defaultValue={props.escala.idEscala}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.escala.nmNomeEscala}
                                    required
                                    onChange={(e) => setNome(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Departamento</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    name="departamento"
                                    value={departamentoSelecionado}
                                    onChange={(e) => setDepartamentoSelecionado(e.target.value)}
                                    required
                                    disabled={ativo == true}
                                >
                                    <option value="">Selecione um departamento</option>
                                    {departamentos.map(departamento => (
                                        <option key={departamento.idDepartamento} value={departamento.idDepartamento}>{departamento.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Cargo</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    name="cargos"
                                    value={cargoSelecionado}
                                    onChange={handleSelectChange}
                                    required
                                    disabled={ativo == true}
                                >
                                    <option value="">Selecione um Cargo</option>
                                    {cargos.map(cargo => (
                                        <option key={cargo.idCargo} value={cargo.idCargo}>{cargo.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Tipo Escala</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    name="tipoEscala"
                                    value={tipoEscalaSelecionado}
                                    onChange={(e) => setTipoEscalaSelecionado(e.target.value)}
                                    required
                                    disabled={ativo == true}
                                >
                                    <option value="">Selecione um Tipo de Escala</option>
                                    {tipoEscalas.map(tipoEscala => (
                                        <option key={tipoEscala.idTipoEscala} value={tipoEscala.idTipoEscala}>{tipoEscala.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Mês de Referência</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-select"
                                    aria-label="Default select example"
                                    name='mesReferencia'
                                    value={mesReferencia}
                                    onChange={(e) => setMesReferencia(e.target.value)}
                                    required
                                    disabled={ativo == true}
                                >
                                    {nomesMeses.map((nome, i) => (
                                        <option key={i} value={i + 1}>{nome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Qtd Pessoa por Posto</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="pessoaPorPosto"
                                    defaultValue={props.escala.nrPessoaPorPosto}
                                    required
                                    disabled={ativo == true}
                                    onChange={(e) => setPessoaPorPosto(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-check-input"
                                    name="ativo"
                                    type="checkbox"
                                    checked={ativo}
                                    onChange={handleAtivoChange}
                                />
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

function MontaEscala(props) {
    MontaEscala.propTypes = {
        ShowList: PropTypes.func.isRequired,
        ShowMontaEscala: PropTypes.func.isRequired,
        escala: PropTypes.shape({
            idEscala: PropTypes.number,
            nmNomeEscala: PropTypes.string,
            idDepartamento: PropTypes.number,
            idCargo: PropTypes.number,
            idTipoEscala: PropTypes.number,
            nrMesReferencia: PropTypes.number,
            nrPessoaPorPosto: PropTypes.number,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.escala.nmNomeEscala || '');
    const [mesReferencia, setMesReferencia] = useState(props.escala.nrMesReferencia || '');
    const [pessoaPorPosto, setPessoaPorPosto] = useState(props.escala.nrPessoaPorPosto || '');
    const [departamentos, setDepartamentos] = useState([]);
    const [cargos, setCargos] = useState([]);
    const [tipoEscalas, setTipoEscalas] = useState([]);
    const [departamentoSelecionado, setDepartamentoSelecionado] = useState('');
    const [cargoSelecionado, setCargoSelecionado] = useState('');
    const [tipoEscalaSelecionado, setTipoEscalaSelecionado] = useState('');
    const [ativo, setAtivo] = useState(props.escala.isAtivo || false);
    const [isLoading, setIsLoading] = useState(false);
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;

    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    useEffect(() => {
        setNome(props.escala.nmNomeEscala || '');
        setMesReferencia(props.escala.nrMesReferencia || '');
        setPessoaPorPosto(props.escala.nrPessoaPorPosto || '');
        setAtivo(props.escala.isAtivo || false);
        setDepartamentoSelecionado(props.escala.idDepartamento || '');
        setCargoSelecionado(props.escala.idCargo || '');
        setTipoEscalaSelecionado(props.escala.idTipoEscala || '');
    }, [props.escala]);

    useEffect(() => {
        BuscarDepartametos();
    }, []);
    const API_URL = `${API_BASE_URL}/departamento`;
    function BuscarDepartametos() {
        api.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setDepartamentos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarCargos();
    }, []);
    const API_URL_Cargo = `${API_BASE_URL}/cargo`;
    function BuscarCargos() {
        api.get(`${API_URL_Cargo}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setCargos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarTipoEscala();
    }, []);
    const API_URL_TipoEscala = `${API_BASE_URL}/tipoEscala`;
    function BuscarTipoEscala() {
        api.get(`${API_URL_TipoEscala}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setTipoEscalas(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    var nomesMeses = [
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        setIsLoading(true);
        e.preventDefault();
        if (props.escala.idEscala) {
            var idEscala = props.escala.idEscala;
            api
                .post(
                    `${API_BASE_URL}/escala/montarEscala`,
                    idEscala,
                    {
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Escala construída com sucesso!",
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
                        message: "Falha ao gerar a Escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                })
                .finally(() => setIsLoading(false));
        }
    };

    return (
        <>
            {isLoading && (
                <div className="loading-overlay">
                    <div className="spinner" />
                    <p>Carregando...</p>
                </div>
            )}
            <h2 className="text-center mb-3">
                {props.escala.idEscala ? "Montar Escala Definitiva" : "Cadastre uma Nova Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.escala.idEscala && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        disabled
                                        className="form-control-plaintext"
                                        name="idEscala"
                                        defaultValue={props.escala.idEscala}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala</label>
                            <div className="col-sm-8">
                                <input
                                    disabled
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.escala.nmNomeEscala}
                                    required
                                    onChange={(e) => setNome(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Departamento</label>
                            <div className="col-sm-8">
                                <select
                                    disabled
                                    className="form-control"
                                    name="departamento"
                                    value={departamentoSelecionado}
                                    onChange={(e) => setDepartamentoSelecionado(e.target.value)}
                                    required
                                >
                                    <option value="">Selecione um departamento</option>
                                    {departamentos.map(departamento => (
                                        <option key={departamento.idDepartamento} value={departamento.idDepartamento}>{departamento.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Cargo</label>
                            <div className="col-sm-8">
                                <select
                                    disabled
                                    className="form-control"
                                    name="cargo"
                                    value={cargoSelecionado}
                                    onChange={(e) => setCargoSelecionado(e.target.value)}
                                    required
                                >
                                    <option value="">Selecione um Cargo</option>
                                    {cargos.map(cargo => (
                                        <option key={cargo.idCargo} value={cargo.idCargo}>{cargo.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Tipo Escala</label>
                            <div className="col-sm-8">
                                <select
                                    disabled
                                    className="form-control"
                                    name="tipoEscala"
                                    value={tipoEscalaSelecionado}
                                    onChange={(e) => setTipoEscalaSelecionado(e.target.value)}
                                    required
                                >
                                    <option value="">Selecione um Tipo de Escala</option>
                                    {tipoEscalas.map(tipoEscala => (
                                        <option key={tipoEscala.idTipoEscala} value={tipoEscala.idTipoEscala}>{tipoEscala.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Mês de Referência</label>
                            <div className="col-sm-8">
                                <select
                                    disabled
                                    className="form-select"
                                    aria-label="Default select example"
                                    name='mesReferencia'
                                    value={mesReferencia}
                                    onChange={(e) => setMesReferencia(e.target.value)}
                                    required
                                >
                                    {nomesMeses.map((nome, i) => (
                                        <option key={i} value={i + 1}>{nome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Qtd Pessoa por Posto</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    disabled
                                    className="form-control"
                                    name="pessoaPorPosto"
                                    defaultValue={props.escala.nrPessoaPorPosto}
                                    required
                                    onChange={(e) => setPessoaPorPosto(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input
                                    disabled
                                    className="form-check-input"
                                    name="ativo"
                                    type="checkbox"
                                    checked={ativo}
                                    onChange={handleAtivoChange}
                                />
                            </div>
                        </div>

                        <div className="row">
                            <div className="offset-sm-4 col-sm-4 d-grid">
                                <button type="submit" className="btn btn-primary btn-sm me-3" disabled={isLoading}>
                                    Gerar Escala
                                </button>
                            </div>
                            <div className="col-sm-4 d-grid">
                                <button
                                    onClick={() => props.ShowList()}
                                    type="button"
                                    className="btn btn-danger me-2"
                                    disabled={isLoading}
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

export function Escala() {
    const [content, setContent] = useState(
        <EscalaList ShowForm={ShowForm} ShowMontaEscala={ShowMontaEscala} />
    );

    function ShowList() {
        setContent(<EscalaList ShowForm={ShowForm} ShowMontaEscala={ShowMontaEscala} />);
    }

    function ShowForm(escala) {
        setContent(
            <EscalaForm escala={escala} ShowList={ShowList} ShowForm={ShowForm} ShowMontaEscala={ShowMontaEscala} />
        );
    }

    function ShowMontaEscala(escala) {
        setContent(
            <MontaEscala escala={escala} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }

    return <div className="container">{content}</div>;
}