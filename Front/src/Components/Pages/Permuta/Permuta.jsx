/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState, useRef } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import Select from 'react-select';

function PermutaList(props) {
    const [searchText, setSearchText] = useState("");
    const [permuta, setPermuta] = useState([]);
    
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const API_URL = "https://localhost:7207/permutas";

    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
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
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
                });
            });
    }

    useEffect(() => {
        // Chamando a função BuscarTodos dentro de useEffect
        BuscarTodos();
    }, []);

    PermutaList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
        

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeletePermuta(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeletePermuta(idPermuta) {
        axios
        .delete(`https://localhost:7207/permutas/Deletar/${idPermuta}`)
            .then((response) => {
                console.log(response);
                setPermuta(
                    permuta.filter((p) => p.id !== idPermuta)
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
    
    //const currentRecords = filterRecords(permuta);

    // Função para filtrar os registros com base no texto de busca
    // function filterRecords(records) {
    //     return records.filter(record => {
    //         const filtro = permuta.find(p => p.idPermuta === record.idPermuta)?.nmNome || "";
    //         return (
    //             record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||
    //             record.nrMatricula.toString().includes(searchText) ||
    //             record.nmEndereco.toLowerCase().includes(searchText.toLowerCase()) ||
    //             filtro.toLowerCase().includes(searchText.toLowerCase())
    //         );
    //     });
    // }

    return (
        <>
            <NavBar />
            <h3 className="text-center mb-3">Lista de Permutas</h3>
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
            <table className="table">
                <thead>
                    <tr>
                        <th>NOME SOLICITANTE</th>
                        <th>NOME SOLICITADO</th>
                        <th>DATA TROCA</th>
                        <th>DATA SOLICITAÇÃO</th>
                        <th>NOME APROVADOR</th>
                        <th>DATA APROVAÇÃO</th>
                        <th>NOME ESCALA</th>
                    </tr>
                </thead>
                <tbody>
                    {permuta
                        .map((p, index) => {
                            return (
                                <tr key={index}>
                                    <td style={{ textAlign: "left" }}>{p.nmNomeSolicitante}</td>
                                    <td style={{ textAlign: "left" }}>{p.nmNomeSolicitado}</td>
                                    <td style={{ textAlign: "left" }}>{p.dtDataSolicitadaTroca}</td>
                                    <td style={{ textAlign: "left" }}>{p.dtSolicitacao}</td>
                                    <td style={{ textAlign: "left" }}>{p.nmNomeAprovador}</td>
                                    <td style={{ textAlign: "left" }}>{p.dtAprovacao}</td>
                                    <td style={{ textAlign: "left" }}>{p.idEscala}</td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(p)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(p.idFuncionario)}
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
function PermutaForm(props) {
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
            dtDataSolicitadaTroca: PropTypes.String,
            dtAprovacao: PropTypes.bool,
        }).isRequired,
    };
    
    const [idDaEscala, setIdDaEscala] = useState(props.permuta.idEscala || '');
    useEffect(() => {
        setIdDaEscala(props.permuta.idEscala || '');
        setNomeSolicitante(props.permuta.nmNomeSolicitante || '');
        setSolicitado(props.permuta.nmNomeSolicitado || '');
        setNomeAprovador(props.permuta.nmNomeAprovador || '');
        setIdFuncionarioSolicitante(props.permuta.idFuncionarioSolicitante || '');
        setIdFuncionarioSolicitado(props.permuta.idFuncionarioSolicitado || '');
        setIdFuncionarioAprovador(props.permuta.idFuncionarioAprovador || '');
        setDtSolicitacao(props.permuta.dtSolicitacao || '');
        setDtDataSolicitadaTroca(props.permuta.dtDataSolicitadaTroca || '');
        setDtAprovacao(props.permuta.dtAprovacao || '');
    }, [props.permuta]);

    const [nomeSolicitante, setNomeSolicitante] = useState(props.permuta.nmNomeSolicitante || '');
    const [nomeSolicitado, setSolicitado] = useState(props.permuta.nmNomeSolicitado || '');
    const [nomeAprovador, setNomeAprovador] = useState(props.permuta.nmNomeAprovador || '');
    const [idFuncionarioSolicitante, setIdFuncionarioSolicitante] = useState(props.permuta.idFuncionarioSolicitante || '');
    const [idFuncionarioSolicitado, setIdFuncionarioSolicitado] = useState(props.permuta.idFuncionarioSolicitado || '');
    const [idFuncionarioAprovador, setIdFuncionarioAprovador] = useState(props.permuta.idFuncionarioAprovador || '');
    const [dtSolicitacao, setDtSolicitacao] = useState(props.permuta.dtSolicitacao || '');
    const [dtDataSolicitadaTroca, setDtDataSolicitadaTroca] = useState(props.permuta.dtDataSolicitadaTroca || '');
    const [dtAprovacao, setDtAprovacao] = useState(props.permuta.dtAprovacao || '');
    const [alertProps, setAlertProps] = useState({
        show: false, // Define se o AlertPopup deve ser exibido
        type: "info", // Tipo da mensagem (success, error, info, confirm)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const [funcionarios, setFuncionarios] = useState([]); // Lista de funcionários
    const [filtro, setFiltro] = useState(''); // Filtro de busca no select

    useEffect(() => {
        BuscarFuncionarios();
    }, []);

    function BuscarFuncionarios() {
        axios.get("https://localhost:7207/funcionario/buscarTodos")
            .then((response) => {
                console.log(response.data);
                setFuncionarios(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar a lista de funcionários.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
                });
            });
    }

    const handleFuncionarioChange = (e) => {
        const selectedId = e.target.value;
        const funcionarioSelecionado = funcionarios.find((f) => f.idFuncionario === selectedId);

        if (funcionarioSelecionado) {
            setIdFuncionarioSolicitante(funcionarioSelecionado.idFuncionario);
            setNomeSolicitante(funcionarioSelecionado.nmNome);
        }
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.permuta.idPermuta) {
            const data = {
                idEscala: idDaEscala,
                nmNomeSolicitante: nomeSolicitante,
                nmNomeSolicitado: nomeSolicitado,
                nmNomeAprovador: nomeAprovador,
                idFuncionarioSolicitante: idFuncionarioSolicitante,
                idFuncionarioSolicitado: idFuncionarioSolicitado,
                idFuncionarioAprovador: idFuncionarioAprovador,
                dtSolicitacao: dtSolicitacao,
                dtDataSolicitadaTroca: dtDataSolicitadaTroca,
                dtAprovacao: dtAprovacao,
            };
            axios
                .patch(
                    "https://localhost:7207/permutas/Atualizar/" +
                    props.permuta.idPermuta,
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
                        message: "Falha ao atualizar o Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNomeSolicitante: nomeSolicitante,
                nmNomeSolicitado: nomeSolicitado,
                nmNomeAprovador: nomeAprovador,
                idFuncionarioSolicitante: idFuncionarioSolicitante,
                idFuncionarioSolicitado: idFuncionarioSolicitado,
                idFuncionarioAprovador: idFuncionarioAprovador,
                dtSolicitacao: dtSolicitacao,
                dtDataSolicitadaTroca: dtDataSolicitadaTroca,
                dtAprovacao: dtAprovacao,
            };
            axios
                .post("https://localhost:7207/permutas/Incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Permuta cadastrada com sucesso!",
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
                        message: "Falha ao cadastrar a Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };
    const [selectedOption, setSelectedOption] = useState(null);

    function SelectComFiltro({ funcionarios, value, onChange }) {
    const opcoes = funcionarios
        .filter((f) => f.idFuncionario && f.nmNome) // Remove funcionários com dados inválidos
        .map((f) => ({
            value: f.idFuncionario,
            label: `${f.nmNome} - ${f.nrMatricula}`,
        }));

    console.log("Opções disponíveis para o select:", opcoes); // Log das opções disponíveis
    console.log("Valor recebido no value:", value); // Log do valor recebido no select

    return (
        <Select
            options={opcoes}
            placeholder="Digite para buscar..."
            value={opcoes.find((o) => o.value === value) || null} // Sincroniza o valor com o pai
            onChange={(selectedOption) => {
                console.log("Opção selecionada:", selectedOption); // Log do clique no dropdown
                onChange(selectedOption ? selectedOption.value : null); // Passa o ID selecionado para o pai
            }}
            isClearable
            noOptionsMessage={() => "Nenhuma opção encontrada"}
        />
    );
}

    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.permuta.idPermuta
                    ? "Editar Permuta"
                    : "Cadastrar Nova Permuta"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
                    <form onSubmit={(e) => handleSubmit(e)}>
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
                                        //onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">ID DA ESCALA</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.permuta.idEscala}
                                    required
                                    onChange={(e) => setIdDaEscala(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitante</label>
                            <div className="col-sm-8">
                            <SelectComFiltro
                                funcionarios={funcionarios}
                                value={idFuncionarioSolicitante} // Valor controlado pelo estado do pai
                                onChange={(id) => {
                                    const funcionarioSelecionado = funcionarios.find((f) => f.idFuncionario === id);

                                    if (funcionarioSelecionado) {
                                        setIdFuncionarioSolicitante(funcionarioSelecionado.idFuncionario);
                                        setNomeSolicitante(funcionarioSelecionado.nmNome);

                                        console.log("Funcionário selecionado:", funcionarioSelecionado); // Log do funcionário selecionado
                                    } else {
                                        setIdFuncionarioSolicitante(null);
                                        setNomeSolicitante("");

                                        console.log("Nenhum funcionário selecionado.");
                                    }
                                }}
                            />
                            </div>
                        </div>

                        {/* <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitado</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="matricula"
                                    defaultValue={props.permuta.nmNomeSolicitado}
                                    required
                                    onChange={(e) => setSolicitado(e.target.value)}
                                ></input>
                            </div>
                        </div> */}

                        {/* <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data da troca</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="telefone"
                                    defaultValue={props.permuta.dtDataSolicitadaTroca}
                                    required
                                    onChange={(e) => setDtDataSolicitadaTroca(e.target.value)}
                                ></input>
                            </div>
                        </div> */}


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
export function Permuta() {
    const [content, setContent] = useState(
        <PermutaList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<PermutaList ShowForm={ShowForm} />);
    }

    function ShowForm(permuta) {
        setContent(
            <PermutaForm permuta={permuta} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}