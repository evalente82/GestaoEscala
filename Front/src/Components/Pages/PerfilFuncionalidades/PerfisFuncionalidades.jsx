/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
import AlertPopup from "../AlertPopup/AlertPopup";

function PerfisFuncionalidadesList(props) {
    const [searchText, setSearchText] = useState("");
    const [perfisFuncionalidades, setPerfisFuncionalidades] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    const API_URL = "https://localhost:7207/perfisFuncionalidades";

    function BuscarPerfisFuncionalidades() {
        axios
            .get(`${API_URL}/buscarTodas`)
            .then((response) => {
                // console.log("Perfis e Funcionalidades:", response.data);
                setPerfisFuncionalidades(response.data);
            })
            .catch(() => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as vinculações de perfis e funcionalidades.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarPerfisFuncionalidades();
    }, []);

    const filteredPerfisFuncionalidades = perfisFuncionalidades.filter((pf) =>
        pf.nomePerfil?.toLowerCase().includes(searchText.toLowerCase())
    );

    function handleDelete(idPerfil, idFuncionalidade) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar Desvinculação",
            message: "Tem certeza que deseja remover esta funcionalidade do perfil?",
            onConfirm: () => {
                DesvincularFuncionalidade(idPerfil, idFuncionalidade);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DesvincularFuncionalidade(idPerfil, idFuncionalidade) {
        console.log("perfil2", idPerfil);
        console.log("funcionalidade2", idFuncionalidade);
    
        axios
            .delete(`${API_URL}/deletar`, {
                data: {
                    idPerfil: idPerfil,
                    idFuncionalidade: idFuncionalidade,
                    nomePerfil: "string", // Esses valores podem ser fixos, já que não são usados no backend
                    nomeFuncionalidade: "string",
                },
            })
            .then(() => {
                BuscarPerfisFuncionalidades(); // Atualiza a lista após a deleção
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Funcionalidade desvinculada com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                console.error("Erro ao desvincular funcionalidade:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao desvincular a funcionalidade.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    return (
        <>
            <h3 className="text-center mb-3">Perfis e Funcionalidades</h3>
                <div className="text-center mb-3">
                    <button 
                        onClick={() => props.ShowForm({})}
                        type="button"
                        className="btn btn-primary me-2"
                        >
                        Cadastrar
                    </button>
                    <button
                        onClick={() => BuscarPerfisFuncionalidades()}
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
                placeholder="Pesquisar Perfis..."
                className="form-control mb-3"
            />
            <table className="table">
                <thead>
                    <tr>
                        <th>PERFIL</th>
                        <th>FUNCIONALIDADE</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredPerfisFuncionalidades.map((pf, index) => (
                        <tr key={index}>
                            <td>{pf.nomePerfil || "N/A"}</td>
                            <td>{pf.nomeFuncionalidade || "N/A"}</td>
                            <td>
                                <button
                                    key={pf.idFuncionalidade}
                                    onClick={() =>
                                        handleDelete(pf.idPerfil, pf.idFuncionalidade)
                                    }
                                    type="button"
                                    className="btn btn-danger btn-sm"
                                >
                                    Desvincular
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


function PerfisFuncionalidadesForm(props) {
    PerfisFuncionalidadesForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
    };

    const [perfis, setPerfis] = useState([]);
    const [funcionalidades, setFuncionalidades] = useState([]);
    const [perfilSelecionado, setPerfilSelecionado] = useState("");
    const [funcionalidadeSelecionada, setFuncionalidadeSelecionada] = useState("");
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    useEffect(() => {
        axios.get("https://localhost:7207/perfil/buscarTodos").then((response) => {
            setPerfis(response.data);
        });
        axios.get("https://localhost:7207/funcionalidade/buscarTodas").then((response) => {
            setFuncionalidades(response.data);
        });
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();

        const perfilSelecionadoNome = perfis.find(
            (perfil) => perfil.idPerfil === perfilSelecionado
        )?.nome;

        const funcionalidadeSelecionadaNome = funcionalidades.find(
            (funcionalidade) => funcionalidade.idFuncionalidade === funcionalidadeSelecionada
        )?.nome;

        console.log("Perfil Selecionado (ID):", perfilSelecionado);
        console.log("Perfil Selecionado (Nome):", perfilSelecionadoNome);
        console.log("Funcionalidade Selecionada (ID):", funcionalidadeSelecionada);
        console.log("Funcionalidade Selecionada (Nome):", funcionalidadeSelecionadaNome);

        try {
            await axios.post("https://localhost:7207/perfisFuncionalidades/incluir", {
                idPerfil: perfilSelecionado,
                idFuncionalidade: funcionalidadeSelecionada,
                nomePerfil: perfilSelecionadoNome,
                nomeFuncionalidade: funcionalidadeSelecionadaNome,
            });
            setAlertProps({
                show: true,
                type: "success",
                title: "Sucesso",
                message: "Funcionalidade vinculada ao perfil com sucesso!",
                onClose: () => {
                    setAlertProps((prev) => ({ ...prev, show: false }));
                    props.ShowList();
                },
            });
        } catch (error) {
            console.error("Erro ao vincular funcionalidade ao perfil:", error);
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Falha ao vincular funcionalidade ao perfil.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
        }
    };

    return (
        <>
            <h2 className="text-center mb-3">Vincular Funcionalidade ao Perfil</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Perfil</label>
                    <select
                        className="form-control"
                        value={perfilSelecionado}
                        onChange={(e) => setPerfilSelecionado(e.target.value)}
                        required
                    >
                        <option value="">Selecione um perfil</option>
                        {perfis.map((perfil) => (
                            <option key={perfil.idPerfil} value={perfil.idPerfil}>
                                {perfil.nome}
                            </option>
                        ))}
                    </select>
                </div>
                <div className="mb-3">
                    <label className="form-label">Funcionalidade</label>
                    <select
                        className="form-control"
                        value={funcionalidadeSelecionada}
                        onChange={(e) => setFuncionalidadeSelecionada(e.target.value)}
                        required
                    >
                        <option value="">Selecione uma funcionalidade</option>
                        {funcionalidades.map((funcionalidade) => (
                            <option
                                key={funcionalidade.idFuncionalidade}
                                value={funcionalidade.idFuncionalidade}
                            >
                                {funcionalidade.nome}
                            </option>
                        ))}
                    </select>
                </div>
                <button type="submit" className="btn btn-primary btn-sm me-3">
                    Vincular
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



export function PerfisFuncionalidades() {
    const [content, setContent] = useState(<PerfisFuncionalidadesList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<PerfisFuncionalidadesList ShowForm={ShowForm} />);
    }

    function ShowForm() {
        setContent(<PerfisFuncionalidadesForm ShowList={ShowList} />);
    }

    return <div className="container">{content}</div>;
}
