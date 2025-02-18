/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
import AlertPopup from "../AlertPopup/AlertPopup";
import api from "./axiosConfig";

function FuncionalidadeList(props) {
    const [searchText, setSearchText] = useState("");
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [funcionalidades, setFuncionalidades] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });
    const API_URL = `${API_BASE_URL}/funcionalidade`;

    // Função para buscar todas as funcionalidades
    function BuscarFuncionalidades() {
        api.get(`${API_URL}/buscarTodas`)
            .then((response) => {
                setFuncionalidades(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as Funcionalidades.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarFuncionalidades();
    }, []);

    FuncionalidadeList.propTypes = {
        ShowForm: PropTypes.func.isRequired,
    };

    function handleDelete(idFuncionalidade) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir esta funcionalidade?",
            onConfirm: () => {
                DeleteFuncionalidade(idFuncionalidade);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeleteFuncionalidade(idFuncionalidade) {
        api.delete(`${API_URL}/deletar/${idFuncionalidade}`)
            .then(() => {
                setFuncionalidades(
                    funcionalidades.filter((f) => f.idFuncionalidade !== idFuncionalidade)
                );
                BuscarFuncionalidades();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Funcionalidade excluída com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao excluir a funcionalidade.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    const filteredFuncionalidades = funcionalidades.filter((funcionalidade) =>
        funcionalidade.nome.toLowerCase().includes(searchText.toLowerCase())
    );

    return (
        <>
            <h3 className="text-center mb-3">Listagem de Funcionalidades</h3>
                <div className="text-center mb-3">
                    <button 
                        onClick={() => props.ShowForm({})}
                        type="button"
                        className="btn btn-primary me-2"
                        >
                        Cadastrar
                    </button>
                    <button
                        onClick={() => BuscarFuncionalidades()}
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
                        <th>DESCRIÇÃO</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredFuncionalidades.map((funcionalidade, index) => (
                        <tr key={index}>
                            <td>{funcionalidade.nome}</td>
                            <td>{funcionalidade.descricao}</td>
                            <td>
                                <button
                                    onClick={() => props.ShowForm(funcionalidade)}
                                    type="button"
                                    className="btn btn-primary btn-sm me-2"
                                >
                                    Editar
                                </button>
                                <button
                                    onClick={() => handleDelete(funcionalidade.idFuncionalidade)}
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

function FuncionalidadeForm(props) {
    FuncionalidadeForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        funcionalidade: PropTypes.shape({
            idFuncionalidade: PropTypes.string,
            nome: PropTypes.string,
            descricao: PropTypes.string,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.funcionalidade.nome || "");
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [descricao, setDescricao] = useState(props.funcionalidade.descricao || "");
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

        if (props.funcionalidade.idFuncionalidade) {
            api
                .put(
                    `${API_BASE_URL}/funcionalidade/atualizar/${props.funcionalidade.idFuncionalidade}`,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Funcionalidade atualizada com sucesso!",
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
                        message: "Falha ao atualizar a funcionalidade.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        } else {
            api
                .post(`${API_BASE_URL}/funcionalidade/incluir`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Funcionalidade cadastrada com sucesso!",
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
                        message: "Falha ao cadastrar a funcionalidade.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                });
        }
    };

    return (
        <>
            <h2 className="text-center mb-3">
                {props.funcionalidade.idFuncionalidade
                    ? "Editar Funcionalidade"
                    : "Cadastrar Nova Funcionalidade"}
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

export function Funcionalidade() {
    const [content, setContent] = useState(<FuncionalidadeList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<FuncionalidadeList ShowForm={ShowForm} />);
    }

    function ShowForm(funcionalidade) {
        setContent(<FuncionalidadeForm funcionalidade={funcionalidade} ShowList={ShowList} />);
    }

    return <div className="container">{content}</div>;
}
