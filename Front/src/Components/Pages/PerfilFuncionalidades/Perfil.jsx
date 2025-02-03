/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
import AlertPopup from "../AlertPopup/AlertPopup";

function PerfilList(props) {
    const [searchText, setSearchText] = useState("");
    const [perfis, setPerfis] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });
    const API_URL = "https://localhost:7207/perfil";

    // Função para buscar todos os perfis
    function BuscarPerfis() {
        axios
            .get(`${API_URL}/buscarTodos`)
            .then((response) => {
                setPerfis(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os Perfis.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarPerfis();
    }, []);

    PerfilList.propTypes = {
        ShowForm: PropTypes.func.isRequired,
    };

    function handleDelete(idPerfil) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este perfil?",
            onConfirm: () => {
                DeletePerfil(idPerfil);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeletePerfil(idPerfil) {
        axios
            .delete(`${API_URL}/deletar/${idPerfil}`)
            .then(() => {
                setPerfis(perfis.filter((perfil) => perfil.idPerfil !== idPerfil));
                BuscarPerfis();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Perfil excluído com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao excluir o perfil.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    const filteredProfiles = perfis.filter((perfil) =>
        perfil.nome.toLowerCase().includes(searchText.toLowerCase())
    );

    return (
        <>
            <h3 className="text-center mb-3">Listagem de Perfis</h3>
            <button
                onClick={() => props.ShowForm({})}
                type="button"
                className="btn btn-primary me-2"
            >
                Cadastrar Novo Perfil
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
                        <th>DESCRIÇÃO</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredProfiles.map((perfil, index) => (
                        <tr key={index}>
                            <td>{perfil.nome}</td>
                            <td>{perfil.descricao}</td>
                            <td>
                                <button
                                    onClick={() => props.ShowForm(perfil)}
                                    type="button"
                                    className="btn btn-primary btn-sm me-2"
                                >
                                    Editar
                                </button>
                                <button
                                    onClick={() => handleDelete(perfil.idPerfil)}
                                    type="button"
                                    className="btn btn-danger btn-sm"
                                >
                                    Deletar
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
        </>
    );
}

function PerfilForm(props) {
    PerfilForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        perfil: PropTypes.shape({
            idPerfil: PropTypes.string,
            nome: PropTypes.string,
            descricao: PropTypes.string,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.perfil.nome || "");
    const [descricao, setDescricao] = useState(props.perfil.descricao || "");
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    const handleSubmit = (e) => {
        e.preventDefault();

        const data = { nome, descricao };

        if (props.perfil.idPerfil) {
            axios
            .put(
                    `https://localhost:7207/perfil/atualizar/${props.perfil.idPerfil}`,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Perfil atualizado com sucesso!",
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
                        message: "Falha ao atualizar o perfil.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        } else {
            axios
                .post("https://localhost:7207/perfil/incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Perfil cadastrado com sucesso!",
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
                        message: "Falha ao cadastrar o perfil.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        }
    };

    return (
        <>
            <h2 className="text-center mb-3">
                {props.perfil.idPerfil ? "Editar Perfil" : "Cadastrar Novo Perfil"}
            </h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Nome</label>
                    <input
                        className="form-control"
                        name="nome"
                        value={nome}
                        required
                        onChange={(e) => setNome(e.target.value)}
                    />
                </div>
                <div className="mb-3">
                    <label className="form-label">Descrição</label>
                    <textarea
                        className="form-control"
                        name="descricao"
                        value={descricao}
                        onChange={(e) => setDescricao(e.target.value)}
                    />
                </div>
                <button type="submit" className="btn btn-primary btn-sm me-3">
                    Salvar
                </button>
                <button
                    onClick={() => props.ShowList()}
                    type="button"
                    className="btn btn-danger btn-sm"
                >
                    Cancelar
                </button>
            </form>
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

export function Perfil() {
    const [content, setContent] = useState(<PerfilList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<PerfilList ShowForm={ShowForm} />);
    }

    function ShowForm(perfil) {
        setContent(<PerfilForm perfil={perfil} ShowList={ShowList} />);
    }

    return <div className="container">{content}</div>;
}
