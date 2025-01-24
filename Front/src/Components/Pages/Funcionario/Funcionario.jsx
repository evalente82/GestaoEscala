/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';

function FuncionarioList(props) {
    const [searchText, setSearchText] = useState("");
    const [funcionario, setFuncionario] = useState([]);
    const [cargos, setCargos] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const API_URL = "https://localhost:7207/funcionario";

    // Definindo a função BuscarTodos dentro do componente FuncionarioList
    function BuscarTodos() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/cargo/buscarTodos");
                setCargos(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }

    function BuscarFuncionarios() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setFuncionario(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os Cargos.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
                });
            });
    }


    useEffect(() => {
        // Chamando a função BuscarTodos dentro de useEffect
        BuscarTodos();
    }, []);

    FuncionarioList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    
    // Chame BuscarTodos() no início do componente para carregar os cargos
    useEffect(() => {
        BuscarTodos(setCargos);
    }, []); // Passando um array vazio, o efeito será executado apenas uma vez no carregamento do componente    

    useEffect(() => {
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setFuncionario(response.data);
        };
        fetchData();
    }, []);

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeleteFuncionario(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeleteFuncionario(idFuncionario) {
        axios
        .delete(`https://localhost:7207/funcionario/Deletar/${idFuncionario}`)
            .then((response) => {
                console.log(response);
                setFuncionario(
                    funcionario.filter((usuario) => usuario.id !== idFuncionario)
                );
                BuscarFuncionarios();
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
    
    const currentRecords = filterRecords(funcionario);

    // Função para filtrar os registros com base no texto de busca
    function filterRecords(records) {
        return records.filter(record => {
            const cargoNome = cargos.find(cargo => cargo.idCargo === record.idCargo)?.nmNome || "";
            return (
                record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||
                record.nrMatricula.toString().includes(searchText) ||
                record.nmEndereco.toLowerCase().includes(searchText.toLowerCase()) ||
                cargoNome.toLowerCase().includes(searchText.toLowerCase())
            );
        });
    }

    return (
        <>
            <NavBar />
            <h3 className="text-center mb-3">Listagem de Funcionários</h3>
            <button
                onClick={() => props.ShowForm({})}
                type="button"
                className="btn btn-primary me-2"
            >
                Cadastrar
            </button>
            <button
                onClick={() => BuscarFuncionarios()}
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
            <table className="table">
                <thead>
                    <tr>
                        <th>NOME</th>
                        <th>MATRÍCULA</th>
                        <th>TELEFONE</th>
                        <th>ENDEREÇO</th>
                        <th>CARGO</th>
                        <th>ATIVO</th>
                        <th>E-MAIL</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((funcionario, index) => {
                            return (
                                <tr key={index}>
                                    <td style={{ textAlign: "left" }}>{funcionario.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{funcionario.nrMatricula}</td>
                                    <td style={{ textAlign: "left" }}>{funcionario.nrTelefone}</td>
                                    <td style={{ textAlign: "left" }}>{funcionario.nmEndereco}</td>
                                    <td>{cargos.find(cargo => cargo.idCargo === funcionario.idCargo)?.nmNome}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={funcionario.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td style={{ textAlign: "left" }}>{funcionario.nmEmail}</td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(funcionario)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(funcionario.idFuncionario)}
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
function FuncionarioForm(props) {
    FuncionarioForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        funcionario: PropTypes.shape({
            idFuncionario: PropTypes.number,
            nmNome: PropTypes.string,
            nrMatricula: PropTypes.number,
            nrTelefone: PropTypes.number,
            nmEmail: PropTypes.string,
            nmEndereco: PropTypes.string,
            idCargo: PropTypes.String,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };
    
    const [nome, setNome] = useState(props.funcionario.nmNome || '');
    const [ativo, setAtivo] = useState(props.funcionario.isAtivo || false);
    const [matricula, setMatricula] = useState(props.funcionario.nrMatricula || '');
    const [telefone, setTelefone] = useState(props.funcionario.nrTelefone || '');
    const [email, setEmail] = useState(props.funcionario.nmEmail || '');
    const [endereco, setEndereco] = useState(props.funcionario.nmEndereco || '');
    const [cargos, setCargos] = useState([]);
    const [cargoSelecionado, setCargoSelecionado] = useState('');
    const [alertProps, setAlertProps] = useState({
        show: false, // Define se o AlertPopup deve ser exibido
        type: "info", // Tipo da mensagem (success, error, info, confirm)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });

    useEffect(() => {
        BuscarTodos();
    }, []);
    const API_URL = "https://localhost:7207/cargo";
    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setCargos(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    useEffect(() => {
        if (props.funcionario.idFuncionario) {
            setCargoSelecionado(props.funcionario.idCargo.toString());
        }
    }, [props.funcionario.idFuncionario]);


    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.funcionario.idFuncionario) {
            const data = {
                nmNome: nome,
                nrMatricula: matricula,
                nrTelefone: telefone,
                nmEmail: email,
                nmEndereco: endereco,
                idCargo: cargoSelecionado,
                isAtivo: ativo,
            };
            axios
                .patch(
                    "https://localhost:7207/funcionario/Atualizar/" +
                    props.funcionario.idFuncionario,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Funcionário atualizado com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList(); // Voltar para a lista após fechar a modal
                        },
                    });
                })
                .catch((error) => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao atualizar o Funcionário.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNome: nome,
                nrMatricula: matricula,
                nrTelefone: telefone,
                nmEmail: email,
                nmEndereco: endereco,
                idCargo: cargoSelecionado,
                isAtivo: ativo,
            };
            axios
                .post("https://localhost:7207/funcionario/Incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Funcionário cadastrado com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList(); // Voltar para a lista após fechar a modal
                        },
                    });
                })
                .catch((error) => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao cadastrar o Funcionário.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };

    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.funcionario.idFuncionario
                    ? "Editar Funcionario"
                    : "Cadastrar Novo Funcionario"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.funcionario.idFuncionario && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idFuncionario"
                                        defaultValue={props.funcionario.idFuncionario}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome do Funcionário</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.funcionario.nmNome}
                                    required
                                    onChange={(e) => setNome(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Matrícula</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    className="form-control"
                                    name="matricula"
                                    defaultValue={props.funcionario.nrMatricula}
                                    required
                                    onChange={(e) => setMatricula(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Telefone</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    className="form-control"
                                    name="telefone"
                                    defaultValue={props.funcionario.nrTelefone}
                                    required
                                    onChange={(e) => setTelefone(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Endereço</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="endereco"
                                    defaultValue={props.funcionario.nmEndereco}
                                    required
                                    onChange={(e) => setEndereco(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Cargo</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    name="cargo"
                                    value={cargoSelecionado}
                                    onChange={(e) => setCargoSelecionado(e.target.value)}
                                    required
                                >
                                    <option value="">Selecione um cargo</option>
                                    {cargos.map(cargo => (
                                        <option key={cargo.idCargo} value={cargo.idCargo}>{cargo.nmNome}</option>
                                    ))}
                                </select>
                            </div>
                        </div>                        

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">E-mail</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="Email"
                                    defaultValue={props.funcionario.nmEmail}
                                    required
                                    onChange={(e) => setEmail(e.target.value)}
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
                                    value={props.funcionario.isAtivo}
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
export function Funcionario() {
    const [content, setContent] = useState(
        <FuncionarioList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<FuncionarioList ShowForm={ShowForm} />);
    }

    function ShowForm(funcionario) {
        setContent(
            <FuncionarioForm funcionario={funcionario} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}