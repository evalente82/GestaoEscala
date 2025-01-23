import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';

function PostoTrabalhoList(props) {
    function BuscarTodos() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/departamento/buscarTodos");
                setDepartamentos(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }
    function BuscarPostos(){
            const API_URL = "https://localhost:7207/postoTrabalho";
            const fetchData = async () => {
                const response = await axios.get(`${API_URL}/buscarTodos`);
                console.log(response.data);
                setPosto(response.data);
            };
            fetchData();
        
    }
     

    PostoTrabalhoList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    const [currentPage, setCurrentPage] = useState(1);
    const [recordsPerPage] = useState(10); //, setRecordsPerPage

    const [searchText, setSearchText] = useState("");

    const [posto, setPosto] = useState([]);
    const [departamentos, setDepartamentos] = useState([]);

    useEffect(() => {
        BuscarTodos(setDepartamentos);
    }, []); // Passando um array vazio, o efeito será executado apenas uma vez no carregamento do componente


    const API_URL = "https://localhost:7207/postoTrabalho";
    useEffect(() => {
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setPosto(response.data);
        };
        fetchData();
    }, []);

    

    function handleDelete(id) {
        // Mostrar a popup de confirmação
        if (window.confirm("Tem certeza que deseja excluir este registro?")) {
            DeletePostoTrabalho(id);
        }
    }

    function DeletePostoTrabalho(idPostoTrabalho) {
        axios
            .delete(`https://localhost:7207/postoTrabalho/Deletar/${idPostoTrabalho}`)
            .then((response) => {
                console.log(response);
                setPosto(
                    posto.filter((usuario) => usuario.id !== idPostoTrabalho)
                );
                BuscarPostos();
            })
            .catch((error) => {
                console.error(error);
            });
    }

    const currentRecords = posto
    return (
        <>
            <NavBar />
            <h3 className="text-center mb-3">Listagem de Postos</h3>
            <button
                onClick={() => props.ShowForm({})}
                type="button"
                className="btn btn-primary me-2"
            >
                Cadastrar
            </button>
            <button
                onClick={() => BuscarPostos()}
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
            {/* <div className="d-flex justify-content-center">
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
            </div> */}
            <table className="table">
                <thead>
                    <tr>
                        {/*<th>ID</th>*/}
                        <th>NOME</th>
                        <th>ENDEREÇO</th>
                        <th>DEPARTAMENTO</th>
                        <th>ATIVO</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((posto, index) => {
                            return (
                                <tr key={index}>
                                    {/*<td>{funcionario.idFuncionario}</td>*/}
                                    <td style={{ textAlign: "left" }}>{posto.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{posto.nmEnderco}</td>
                                    <td>{departamentos.find(departamento => departamento.idDepartamento === posto.idDepartamento)?.nmNome}</td>
                                    {/*<td>{posto.idDepartamento}</td>*/}
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={posto.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(posto)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(posto.idPostoTrabalho)}
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
function PostoTrabalhoForm(props) {
    PostoTrabalhoForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        posto: PropTypes.shape({
            idPostoTrabalho: PropTypes.number,
            nmNome: PropTypes.string,
            nmEnderco: PropTypes.number,
            idDepartamento: PropTypes.number,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };

    // const [errorMessage, setErrorMessage] = useState('');
    const [nome, setNome] = useState(props.posto.nmNome || '');
    const [ativo, setAtivo] = useState(props.posto.isAtivo || false);
    const [endereco, setEndereco] = useState(props.posto.nmEnderco || '');
    const [departamentos, setDepartamentos] = useState([]);
    const [departamentoSelecionado, setDepartamentoSelecionado] = useState('');

    useEffect(() => {
        BuscarTodos();
    }, []);
    const API_URL = "https://localhost:7207/departamento";
    function BuscarTodos() {
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
        if (props.posto.idPostoTrabalho) {
            setDepartamentoSelecionado(props.posto.idDepartamento.toString());
        }
    }, [props.posto.idPostoTrabalho]);


    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.posto.idPostoTrabalho) {
            const data = {
                nmNome: nome,
                nmEnderco: endereco,
                idDepartamento: departamentoSelecionado,
                isAtivo: ativo,
            };
            axios
                .patch(
                    "https://localhost:7207/postoTrabalho/Atualizar/" +
                    props.posto.idPostoTrabalho,
                    data
                )
                .then(() => {
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        alert(errors.Email);
                        // outros tratamentos de erro
                    } else {
                        console.log(error);
                        // outros tratamentos de erro
                    }
                });
        } else {
            const data = {
                nmNome: nome,
                nmEnderco: endereco,
                idDepartamento: departamentoSelecionado,
                isAtivo: ativo,
            };
            axios
                .post("https://localhost:7207/postoTrabalho/Incluir", data)
                .then(() => {
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        // outros tratamentos de erro
                    } else {
                        console.log(error);
                        // outros tratamentos de erro
                    }
                });
        }
    };
    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.posto.idPostoTrabalho
                    ? "Editar Postos"
                    : "Cadastrar Novo Posto Trabalho"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.posto.idPostoTrabalho && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idPostoTrabalho"
                                        defaultValue={props.posto.idPostoTrabalho}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome do Posto</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.posto.nmNome}
                                    required
                                    onChange={(e) => setNome(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Endereço</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="endereco"
                                    defaultValue={props.posto.nmEnderco}
                                    required
                                    onChange={(e) => setEndereco(e.target.value)}
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
                                >
                                    <option value="">Selecione um departamento</option>
                                    {departamentos.map(departamento => (
                                        <option key={departamento.idDepartamento} value={departamento.idDepartamento}>{departamento.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-check-input"
                                    name="ativo"
                                    type="checkbox"
                                    value={props.posto.isAtivo}
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
export function PostoTrabalho() {
    const [content, setContent] = useState(
        <PostoTrabalhoList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<PostoTrabalhoList ShowForm={ShowForm} />);
    }

    function ShowForm(posto) {
        setContent(
            <PostoTrabalhoForm posto={posto} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}
