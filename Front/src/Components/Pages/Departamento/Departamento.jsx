import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
function DepartamentoList(props) {
    DepartamentoList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    const [searchText, setSearchText] = useState("");
    const [departamento, setDepartamento] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });

    const API_URL = "https://localhost:7207/departamento";
    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setDepartamento(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os departamentos.",
                });
            });
    }

    useEffect(() => {
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setDepartamento(response.data);
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
                DeleteDepartamento(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeleteDepartamento(idDepartamento) {
        axios
            .delete(`${API_URL}/Deletar/${idDepartamento}`)
            .then(() => {
                setDepartamento(
                    departamento.filter((dep) => dep.idDepartamento !== idDepartamento)
                );
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Departamento excluído com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch(() => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível excluir o departamento.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }
    const currentRecords = filterRecords(departamento)

    // Função para filtrar os registros com base no texto de busca
    function filterRecords(records) {
        return records.filter(record => {
            const filtro = departamento.find(dep => dep.idDepartamento === record.idDepartamento)?.nmNome || "";
            return (
                record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||
                filtro.toLowerCase().includes(searchText.toLowerCase())
            );
        });
    }

    return (
        <>
            <h3 className="text-center mb-3">Listagem de Departamentos</h3>
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
                        {/*<th>ID</th>*/}
                        <th>NOME</th>
                        <th>DESCRIÇÃO</th>
                        <th>ATIVO</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((departamento, index) => {
                            return (
                                <tr key={index}>
                                    <td style={{ textAlign: "left" }}>{departamento.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{departamento.nmDescricao}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={departamento.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(departamento)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(departamento.idDepartamento)}
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
                onClose={alertProps.onClose}
                onConfirm={alertProps.onConfirm}
            />
        </>

    );
} 
function DepartamentoForm(props) {
    DepartamentoForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        departamento: PropTypes.shape({
            idDepartamento: PropTypes.number,
            nmNome: PropTypes.string,
            nmDescricao: PropTypes.string,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };
    // const [errorMessage, setErrorMessage] = useState('');
    const [nome, setNome] = useState(props.departamento.nmNome || '');
    const [descricao, setDescricao] = useState(props.departamento.nmDescricao || '');    
    const [ativo, setAtivo] = useState(props.departamento.isAtivo || false);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.departamento.idDepartamento) {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                isAtivo: ativo,
            };
            axios
                .patch(
                    "https://localhost:7207/departamento/Atualizar/" +
                    props.departamento.idDepartamento,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Departamento atualizado com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList();
                        },
                    });
                })
                .catch(() => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao atualizar o departamento.",
                    });
                });
        } else {
            const data = {
                nmNome: nome,
                NmDescricao: descricao,
                isAtivo: ativo,
            };
            axios
                .post("https://localhost:7207/departamento/Incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Departamento cadastrado com sucesso!",
                        onClose: () => {
                            setAlertProps((prev) => ({ ...prev, show: false }));
                            props.ShowList();
                        },
                    });
                })
                .catch(() => {
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Falha ao cadastrar o departamento.",
                    });
                });
        }
    };
    return (
        <>
            <h2 className="text-center mb-3">
                {props.departamento.idDepartamento
                    ? "Editar Departamento"
                    : "Cadastrar Novo Departamento"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.departamento.idDepartamento && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idDepartamento"
                                        defaultValue={props.departamento.idDepartamento}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome do Departamento</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.departamento.nmNome}
                                    required
                                    onChange={(e) => setNome(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Descrição</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="descricao"
                                    defaultValue={props.departamento.nmDescricao}
                                    required
                                    onChange={(e) => setDescricao(e.target.value)}
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
                                    value={props.departamento.isAtivo}
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
                onConfirm={alertProps.onConfirm}
            />
        </>
    );
}
export function Departamento() {

    const [content, setContent] = useState(
        <DepartamentoList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<DepartamentoList ShowForm={ShowForm} />);
    }

    function ShowForm(departamento) {
        setContent(
            <DepartamentoForm departamento={departamento} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}