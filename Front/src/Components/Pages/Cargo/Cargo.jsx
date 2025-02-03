import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';

function CargoList(props) {
    CargoList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
   
    const [searchText, setSearchText] = useState("");
    const [cargo, setCargo] = useState([]);
    const API_URL = "https://localhost:7207/cargo";
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });

    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setCargo(response.data);
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
        const fetchData = async () => {
            const response = await axios.get(`${API_URL}/buscarTodos`);
            console.log(response.data);
            setCargo(response.data);
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
                DeleteCargo(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeleteCargo(idCargo) {
        axios
            .delete(`https://localhost:7207/cargo/Deletar/${idCargo}`)
            .then((response) => {
                console.log(response);
                setCargo(
                    cargo.filter((usuario) => usuario.id !== idCargo)
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
    const currentRecords = filterRecords(cargo)

    function filterRecords(records) {
        return records.filter(record => {
            const nomeDescicao = cargo.find(cargo => cargo.idCargo === record.idCargo)?.nmNome || "";
            return (
                record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||                
                nomeDescicao.toLowerCase().includes(searchText.toLowerCase())
            );
        });
    }

    return (
        <>
            <h3 className="text-center mb-3">Listagem de Cargos</h3>
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
                        <th>DESCRIÇÃO</th>
                        <th>ATIVO</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((cargo, index) => {
                            return (
                                <tr key={index}>
                                    {/*<td>{departamento.idDepartamento}</td>*/}
                                    <td style={{ textAlign: "left" }}>{cargo.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{cargo.nmDescricao}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={cargo.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(cargo)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(cargo.idCargo)}
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
function CargoForm(props) {
    CargoForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        cargo: PropTypes.shape({
            idCargo: PropTypes.number,
            nmNome: PropTypes.string,
            nmDescricao: PropTypes.string,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.cargo.nmNome || '');
    const [descricao, setDescricao] = useState(props.cargo.nmDescricao || '');
    const [ativo, setAtivo] = useState(props.cargo.isAtivo || false);
    const [alertProps, setAlertProps] = useState({
        show: false, // Define se o AlertPopup deve ser exibido
        type: "info", // Tipo da mensagem (success, error, info, confirm)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.cargo.idCargo) {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                isAtivo: ativo,
            };
            axios
                .patch(
                    "https://localhost:7207/cargo/Atualizar/" +
                    props.cargo.idCargo,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Cargo atualizado com sucesso!",
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
                        message: "Falha ao atualizar o cargo.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                isAtivo: ativo,
            };
            axios
                .post("https://localhost:7207/cargo/Incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Cargo cadastrado com sucesso!",
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
                        message: "Falha ao cadastrar o cargo.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };
    return (
        <>
            <h2 className="text-center mb-3">
                {props.cargo.idCargo
                    ? "Editar Cargo"
                    : "Cadastrar Novo Cargo"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.cargo.idCargo && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idCargo"
                                        defaultValue={props.cargo.idCargo}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome do Cargo</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.cargo.nmNome}
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
                                    defaultValue={props.cargo.nmDescricao}
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
                                    value={props.cargo.isAtivo}
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
export function Cargo() {

    const [content, setContent] = useState(
        <CargoList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<CargoList ShowForm={ShowForm} />);
    }

    function ShowForm(cargo) {
        setContent(
            <CargoForm cargo={cargo} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}

export default CargoForm;