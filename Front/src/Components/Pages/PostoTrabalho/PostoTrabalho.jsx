import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import api from "./axiosConfig";

function PostoTrabalhoList(props) {
    const [searchText, setSearchText] = useState("");
    const [posto, setPosto] = useState([]);
    const [setor, setSetor] = useState([]);
    const [departamentos, setDepartamentos] = useState([]);
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const API_URL = `${API_BASE_URL}/postoTrabalho`;

    function BuscarTodos() {
        const fetchData = async () => {
            try {
                const response = await api.get(`${API_BASE_URL}/departamento/buscarTodos`);
                setDepartamentos(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
    }
    function BuscarPostos() {
        api.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setPosto(response.data);
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

    PostoTrabalhoList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };   
    
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
    
    // 🔹 Adiciona o useEffect para buscar os setores no carregamento
    useEffect(() => {
        BuscarTodos();
        BuscarSetor();
    }, []); 
    


    useEffect(() => {
        BuscarTodos(setDepartamentos);
    }, []); // Passando um array vazio, o efeito será executado apenas uma vez no carregamento do componente
    
    useEffect(() => {
        const fetchData = async () => {
            const response = await api.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setPosto(response.data);
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
                DeletePostoTrabalho(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeletePostoTrabalho(idPostoTrabalho) {
        api
            .delete(`${API_URL}/Deletar/${idPostoTrabalho}`)
            .then((response) => {
                console.log(response);
                setPosto(
                    posto.filter((usuario) => usuario.id !== idPostoTrabalho)
                );
                BuscarPostos();
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

    const currentRecords = filterRecords(posto)

     // Função para filtrar os registros com base no texto de busca
     function filterRecords(records) {
        return records.filter(record => {
            const filtro = posto.find(p => p.idPostoTrabalho === record.idPostoTrabalho)?.nmNome || "";
            return (
                record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||                
                filtro.toLowerCase().includes(searchText.toLowerCase())
            );
        });
    }
    return (
        <>
            <h3 className="text-center mb-3">Listagem de Postos</h3>
                <div className="text-center mb-3">
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
                        <th>ENDEREÇO</th>
                        <th>DEPARTAMENTO</th>
                        <th>SETOR</th>
                        <th>ATIVO</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((posto, index) => {
                            return (
                                <tr key={index}>
                                    <td style={{ textAlign: "left" }}>{posto.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{posto.nmEnderco}</td>
                                    <td>{departamentos.find(departamento => departamento.idDepartamento === posto.idDepartamento)?.nmNome}</td>
                                    <td>{setor.find(s => s.idSetor === posto.idSetor)?.nmNome || "Setor não encontrado"}</td>

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
function PostoTrabalhoForm(props) {
    PostoTrabalhoForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        posto: PropTypes.shape({
            idPostoTrabalho: PropTypes.string,
            nmNome: PropTypes.string,
            nmEnderco: PropTypes.string,
            idDepartamento: PropTypes.string,
            idSetor: PropTypes.string,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };
    const [nome, setNome] = useState(props.posto.nmNome || '');
    const [ativo, setAtivo] = useState(props.posto.isAtivo || false);
    const [endereco, setEndereco] = useState(props.posto.nmEnderco || '');
    const [departamentos, setDepartamentos] = useState([]);
    const [departamentoSelecionado, setDepartamentoSelecionado] = useState('');
    const [setor, setSetor] = useState([]);
    const [setorSelecionado, setSetorSelecionado] = useState('');
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
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
    const API_URL = `${API_BASE_URL}/departamento`;
    function BuscarTodos() {
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
        if (props.posto.idPostoTrabalho) {
            setDepartamentoSelecionado(props.posto.idDepartamento.toString());
        }
    }, [props.posto.idPostoTrabalho]);

    useEffect(() => {
        if (props.posto.idSetor) {
            setSetorSelecionado(props.posto.idSetor.toString());
        }
    }, [props.posto.idSetor]);


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
                idSetor: setorSelecionado,
                isAtivo: ativo,
            };
            api
                .patch(
                    `${API_BASE_URL}/postoTrabalho/Atualizar/` +
                    props.posto.idPostoTrabalho,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Posto de Trabalho atualizado com sucesso!",
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
                        message: "Falha ao atualizar o Posto de Trabalho.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNome: nome,
                nmEnderco: endereco,
                idDepartamento: departamentoSelecionado,
                idSetor: setorSelecionado,
                isAtivo: ativo,
            };
            api
                .post(`${API_BASE_URL}/postoTrabalho/Incluir`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Posto de Trabalho cadastrado com sucesso!",
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
                        message: "Falha ao cadastrar o Posto de trabalho.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };
    return (
        <>
            <h2 className="text-center mb-3">
                {props.posto.idPostoTrabalho
                    ? "Editar Postos"
                    : "Cadastrar Novo Posto Trabalho"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
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
