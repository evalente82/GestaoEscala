import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import LoadingPopup from '../LoadingPopUp/LoadingPopUp'
import AlertPopup from '../AlertPopup/AlertPopup'
import { useNavigate } from 'react-router-dom';


function EscalaList(props) {
    // Definindo a função BuscarTodos dentro do componente FuncionarioList
    function BuscarTodos() {
        const API_URL = "https://localhost:7207/escala";
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log('Escalas');
            console.log(response.data);
            setEscala(response.data);
        };
        fetchData();
    }
    function BuscarDepartamentos() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/departamento/buscarTodos");
                setDepartamentos(response.data);
                console.log('Departamentos');
                console.log(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }
    function Buscarcargos() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/cargo/buscarTodos");
                setCargos(response.data);
                console.log('Cargos');
                console.log(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }
    function BuscarTipoEscalas() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/tipoEscala/buscarTodos");
                setTipoEscalas(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }
    EscalaList.propTypes = {
        ShowForm: PropTypes.func.isRequired,
        ShowMontaEscala: PropTypes.func.isRequired,// Indica que ShowForm é uma função obrigatória
    };

    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);
    const [recordsPerPage] = useState(10); //, setRecordsPerPage
    const [searchText, setSearchText] = useState("");
    const [escala, setEscala] = useState([]);
    const [departamentos, setDepartamentos] = useState([]);
    const [cargos, setCargos] = useState([]);
    const [tipoEscalas, setTipoEscalas] = useState([]);

    // Chame BuscarTodos() no início do componente para carregar os cargos
    useEffect(() => {
        BuscarDepartamentos(setDepartamentos);
    }, []); // Passando um array vazio, o efeito será executado apenas uma vez no carregamento do componente

    useEffect(() => {
        Buscarcargos(setCargos);
    }, []);

    useEffect(() => {
        BuscarTipoEscalas(setTipoEscalas);
    }, []); // Passando um array vazio, o efeito será executado apenas uma vez no carregamento do componente

    const API_URL = "https://localhost:7207/escala";
    useEffect(() => {
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setEscala(response.data);
        };
        fetchData();
    }, []);

    function handleDelete(id) {
        // Mostrar a popup de confirmação
        if (window.confirm("Tem certeza que deseja excluir Uma escala que já foi Gerada ?")) {
            DeleteEscala(id);
        }
    }

    function DeleteEscala(idEscala) {
        axios
            .delete(`https://localhost:7207/escala/Deletar/${idEscala}`)
            .then((response) => {
                console.log(response);
                setEscala(
                    escala.filter((usuario) => usuario.id !== idEscala)
                );
                BuscarTodos();
            })
            .catch((error) => {
                console.error(error);
            });
    }

    //const indexOfLastRecord = currentPage * recordsPerPage;
    //const indexOfFirstRecord = indexOfLastRecord - recordsPerPage;
    const currentRecords = escala
    //    .filter(
    //        (departamento) =>
    //            departamento.nome.toLowerCase().includes(searchText.toLowerCase()) ||
    //            departamento.descricao.toLowerCase().includes(searchText.toLowerCase())
    //    )
    //    .slice(indexOfFirstRecord, indexOfLastRecord);
    //useEffect(() => BuscarTodos(), []);
    function getNomeMes(numeroMes) {
        var dataAtual = new Date();
        var numeroMesAtual = dataAtual.getMonth();
        var nomesMeses = [
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        ];
        if (numeroMes >= 1 && numeroMes <= 12) {
            return nomesMeses[numeroMes - 1];
        } else {
            return nomesMeses[numeroMesAtual];
        }
    }

    return (
        <>
            <NavBar />
            <h3 className="text-center mb-3">Listagem de Escalas</h3>
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
            <br />
            <br />
            <input
                type="text"
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
                placeholder="Pesquisar..."
                className="form-control mb-3"
            />
            <div className="d-flex justify-content-center">
                <button
                    type="button"
                    className="btn btn-outline-primary me-2"
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage(currentPage - 1)}
                >
                    Anterior
                </button>
                <button
                    type="button"
                    className="btn btn-outline-primary"
                    disabled={currentRecords.length < recordsPerPage}
                    onClick={() => setCurrentPage(currentPage + 1)}
                >
                    Próximo
                </button>
            </div>
            <table className="table">
                <thead>
                    <tr>
                        {/*<th>ID</th>*/}
                        <th>NOME</th>
                        <th>DEPARTAMENTO</th>
                        <th>CARGO</th>
                        <th>TIPO ESCALA</th>
                        <th>MÊS REFEREÊNCIA</th>
                        <th>PESSOA POR POSTO</th>
                        <th>ATIVO</th>
                        <th>GERADA</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((escala, index) => {
                            return (
                                <tr key={index}>
                                    <td>{escala.nmNomeEscala}</td>
                                    <td>{departamentos.find(departamento => departamento.idDepartamento === escala.idDepartamento)?.nmNome}</td>
                                    <td>{cargos.find(cargo => cargo.idCargo === escala.idCargo)?.nmNome}</td>
                                    <td>{tipoEscalas.find(tipoEscala => tipoEscala.idTipoEscala === escala.idTipoEscala)?.nmNome}</td>
                                    <td>{getNomeMes(escala.nrMesReferencia)}</td>
                                    <td>{escala.nrPessoaPorPosto}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={escala.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={escala.isGerada == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                    <button
                                            onClick={() => navigate(`/Exibicao/${escala.idEscala}`)}
                                            type="button"
                                            className="btn btn-success btn-sm me-2"
                                            disabled={escala.isGerada == false}
                                        >
                                            Visualizar Escala
                                        </button>
                                        <button
                                            onClick={() => props.ShowMontaEscala(escala)}
                                            type="button"
                                            className="btn btn-warning btn-sm me-2"
                                        >
                                            Gerar Escala
                                        </button>
                                        <button
                                            onClick={() => props.ShowForm(escala)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(escala.idEscala)}
                                            type="button"
                                            className="btn btn-danger btn-sm"
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            );
                        })}
                </tbody>
            </table>
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

    // const [errorMessage, setErrorMessage] = useState('');
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
    

    useEffect(() => {
        BuscarDepartametos();
    }, []);
    const API_URL = "https://localhost:7207/departamento";
    function BuscarDepartametos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(`DEPARTAMENTO `);
                console.log(response.data)
                setDepartamentos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarCargos();
    }, []);
    const API_URL_Carcos = "https://localhost:7207/cargo";
    function BuscarCargos() {
        axios.get(`${API_URL_Carcos}/buscarTodos`)
            .then((response) => {
                console.log(`CARGOS`);
                console.log(response.data)
                setCargos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }
    useEffect(() => {
        BuscarTipoEscala();
    }, []);
    const API_URL_TipoEscala = "https://localhost:7207/tipoEscala";
    function BuscarTipoEscala() {
        axios.get(`${API_URL_TipoEscala}/buscarTodos`)
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
    var selectMesses = document.createElement("select"); // criar um elemento <select>
    for (var j = 0; j < nomesMeses.length; j++) {
        var option2 = document.createElement("option");
        option2.value = j + 1; // Valor do mês é o índice + 1
        option2.text = nomesMeses[j];
        selectMesses.appendChild(option2); // Corrigido para adicionar opções ao elemento selectMesses
    }

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
            axios
                .patch(
                    "https://localhost:7207/escala/Atualizar/" +
                    props.escala.idEscala,
                    data
                )
                .then(() => {
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        // outros tratamentos de erro
                    } else {
                        console.log('teste1 ' + data)
                        console.log(error);
                        // outros tratamentos de erro
                    }
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
            axios
                .post("https://localhost:7207/escala/Incluir", data)
                .then(() => {
                    props.ShowList();
                    console.log('Ação Incluir')
                    console.log(data)
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        console.log(data)
                    } else {
                        console.log(error);
                        console.log('debug: ' + data)
                    }
                });
        }
    };
    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.escala.idEscala
                    ? "Editar Escala"
                    : "Cadastrar Nova Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
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
                                    value={props.escala.idDepartamento}
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
                            className="form-control"
                            name="cargos"
                            value={cargoSelecionado}
                            onChange={handleSelectChange}
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
                                    className="form-control"
                                    name="tipoEscala"
                                    value={props.escala.idTipoEscala} // Troque isso para props.escala.idTipoEscala
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
                                    className="form-check-input"
                                    name="ativo"
                                    type="checkbox"
                                    value={props.escala.isAtivo}
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

    useEffect(() => {
        setNome(props.escala.nmNomeEscala || '');
        setMesReferencia(props.escala.nrMesReferencia || '');
        setPessoaPorPosto(props.escala.nrPessoaPorPosto || '');
        setAtivo(props.escala.isAtivo || false);
        setDepartamentoSelecionado(props.escala.idDepartamento || '');
        setCargoSelecionado(props.escala.idCargo || '');
        setTipoEscalaSelecionado(props.escala.idTipoEscala || '');
    }, [props.escala]);

    // const [errorMessage, setErrorMessage] = useState('');
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
    const [isAlertPopup, setIsAlertPopup] = useState(false);
    const [erro, setError] = useState(false);


    useEffect(() => {
        BuscarDepartametos();
    }, []);
    const API_URL = "https://localhost:7207/departamento";
    function BuscarDepartametos() {
        axios.get(`${API_URL}/buscarTodos`)
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
    const API_URL_Cargo = "https://localhost:7207/cargo";
    function BuscarCargos() {
        axios.get(`${API_URL_Cargo}/buscarTodos`)
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
    const API_URL_TipoEscala = "https://localhost:7207/tipoEscala";
    function BuscarTipoEscala() {
        axios.get(`${API_URL_TipoEscala}/buscarTodos`)
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
    var selectMesses = document.createElement("select"); // criar um elemento <select>
    for (var j = 0; j < nomesMeses.length; j++) {
        var option2 = document.createElement("option");
        option2.value = j + 1; // Valor do mês é o índice + 1
        option2.text = nomesMeses[j];
        selectMesses.appendChild(option2); // Corrigido para adicionar opções ao elemento selectMesses
    }

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        setIsLoading(true);
        e.preventDefault();
        if (props.escala.idEscala) {
            var idEscala = props.escala.idEscala
            axios
                .post(
                    "https://localhost:7207/escala/montarEscala",
                    idEscala,
                    {
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }
                )
                .then(() => {
                    
                    setIsAlertPopup(true);
                    alert("Escala: " + props.escala.nmNomeEscala + " gerada com sucesso!");
                    props.ShowList();
                    setIsLoading(false);
                })
                .catch((error) => {
                    setIsLoading(false);
                    setError(error); // Defina o estado do erro quando ocorrer um erro
                    console.log(error);
                });
        }
    };
    return (
        <>
            <NavBar />
            {erro && <AlertPopup error={erro} />}
            {isLoading && <LoadingPopup />}
            <h2 className="text-center mb-3">
                {props.escala.idEscala
                    ? "Montar Escala Definitiva"
                    : "Cadastre uma Nova Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
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
                                    value={props.escala.idDepartamento}
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
                                    value={props.escala.idCargo}
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
                                    value={props.escala.idTipoEscala} // Troque isso para props.escala.idTipoEscala
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
                                    value={props.escala.isAtivo}
                                    checked={ativo}
                                    onChange={handleAtivoChange}
                                />
                            </div>
                        </div>

                        <div className="row">
                            <div className="offset-sm-4 col-sm-4 d-grid">
                                <button type="submit" className="btn btn-primary btn-sm me-3">
                                    Gerar Escala
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
