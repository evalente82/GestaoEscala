/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
import AlertPopup from "../AlertPopup/AlertPopup";
import Select from 'react-select';


function FuncionariosPerfisList(props) {
    const [searchText, setSearchText] = useState("");
    const [funcionariosPerfis, setFuncionariosPerfis] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    const API_URL = "https://localhost:7207/funcionariosPerfis";

    function BuscarFuncionariosPerfis() {
        console.log("Buscando funcionários e perfis...");
        axios
            .get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log("Dados recebidos da API:", response.data);
                setFuncionariosPerfis(response.data);
            })
            .catch((error) => {
                console.error("Erro ao buscar funcionários e perfis:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as vinculações de funcionários e perfis.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarFuncionariosPerfis();
    }, []);

    const filteredFuncionariosPerfis = funcionariosPerfis.filter((fp) =>
        fp.nomeFuncionario?.toLowerCase().includes(searchText.toLowerCase())
    );

    console.log("Funcionários e Perfis filtrados:", filteredFuncionariosPerfis);

    function handleDelete(idFuncionario, idPerfil) {
        console.log("Iniciando exclusão do perfil...");
        console.log("ID Funcionário:", idFuncionario);
        console.log("ID Perfil:", idPerfil);

        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar Desvinculação",
            message: "Tem certeza que deseja remover este perfil do funcionário?",
            onConfirm: () => {
                DesvincularPerfil(idFuncionario, idPerfil);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DesvincularPerfil(idFuncionario, idPerfil) {
        console.log("Chamando API para desvincular perfil...");
        console.log("ID Funcionário:", idFuncionario);
        console.log("ID Perfil:", idPerfil);

        axios
            .delete(`${API_URL}/desvincular`, {
                data: {
                    idFuncionario: idFuncionario,
                    idPerfil: idPerfil,
                },
            })
            .then(() => {
                console.log("Perfil desvinculado com sucesso!");
                BuscarFuncionariosPerfis();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Perfil desvinculado do funcionário com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch((error) => {
                console.error("Erro ao desvincular perfil:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao desvincular o perfil.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    return (
        <>
            <NavBar />
            <h3 className="text-center mb-3">Funcionários e Perfis</h3>
            <button
                onClick={() => props.ShowForm()}
                type="button"
                className="btn btn-primary me-2"
            >
                Vincular Novo Perfil
            </button>
            <br />
            <br />
            <input
                type="text"
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
                placeholder="Pesquisar Funcionários..."
                className="form-control mb-3"
            />
            <table className="table">
                <thead>
                    <tr>
                        <th>FUNCIONÁRIO</th>
                        <th>PERFIL</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredFuncionariosPerfis.length > 0 ? (
                        filteredFuncionariosPerfis.map((fp, index) => (
                            <tr key={index}>
                                <td>{fp.nomeFuncionario || "N/A"}</td>
                                <td>{fp.nomePerfil || "N/A"}</td>
                                <td>
                                    <button
                                        onClick={() => handleDelete(fp.idFuncionario, fp.idPerfil)}
                                        type="button"
                                        className="btn btn-danger btn-sm"
                                    >
                                        Desvincular
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="3" className="text-center">
                                Nenhum funcionário encontrado.
                            </td>
                        </tr>
                    )}
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

function FuncionariosPerfisForm(props) {
    FuncionariosPerfisForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
    };

    const [funcionarios, setFuncionarios] = useState([]);
    const [perfis, setPerfis] = useState([]);
    const [funcionarioSelecionado, setFuncionarioSelecionado] = useState("");
    const [perfilSelecionado, setPerfilSelecionado] = useState("");
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    useEffect(() => {
        axios.get("https://localhost:7207/funcionario/buscarTodos")
            .then((response) => {
                console.log("Dados de funcionários recebidos:", response.data);
                setFuncionarios(response.data);
            })
            .catch((error) => console.error("Erro ao buscar funcionários:", error));

        axios.get("https://localhost:7207/perfil/buscarTodos")
            .then((response) => {
                console.log("Dados de perfis recebidos:", response.data);
                setPerfis(response.data);
            })
            .catch((error) => console.error("Erro ao buscar perfis:", error));
    }, []);

    // Formata os funcionários para o SelectComFiltro
    const opcoesFuncionarios = funcionarios
        .filter((f) => f.idFuncionario && f.nmNome)
        .map((f) => ({
            value: f.idFuncionario,
            label: `${f.nmNome} - ${f.nrMatricula}`,
        }));

    // Formata os perfis para Select padrão
    const opcoesPerfis = perfis.map((perfil) => ({
        value: perfil.idPerfil,
        label: perfil.nome,
    }));

    // Componente de seleção com busca
function SelectComFiltro({ options, value, onChange, placeholder }) {
    console.log("Opções disponíveis para o select:", options);
    console.log("Valor recebido no value:", value);

    return (
        <Select
            options={options}
            placeholder={placeholder}
            value={options.find((o) => o.value === value) || null}
            onChange={(selectedOption) => {
                console.log("Opção selecionada:", selectedOption);
                onChange(selectedOption ? selectedOption.value : null);
            }}
            isClearable
            noOptionsMessage={() => "Nenhuma opção encontrada"}
        />
    );
}

    const handleSubmit = async (e) => {
        e.preventDefault();

        console.log("Funcionário Selecionado (ID):", funcionarioSelecionado);
        console.log("Perfil Selecionado (ID):", perfilSelecionado);

        try {
            await axios.post("https://localhost:7207/funcionariosPerfis/incluir", {
                idFuncionario: funcionarioSelecionado,
                idPerfil: perfilSelecionado,
            });
            setAlertProps({
                show: true,
                type: "success",
                title: "Sucesso",
                message: "Perfil vinculado ao funcionário com sucesso!",
                onClose: () => {
                    setAlertProps((prev) => ({ ...prev, show: false }));
                    props.ShowList();
                },
            });
        } catch (error) {
            console.error("Erro ao vincular perfil ao funcionário:", error);
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Falha ao vincular perfil ao funcionário.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
        }
    };

    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">Vincular Perfil ao Funcionário</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Funcionário</label>
                    <SelectComFiltro
                        options={opcoesFuncionarios}
                        value={funcionarioSelecionado}
                        onChange={setFuncionarioSelecionado}
                        placeholder="Selecione um funcionário..."
                    />
                </div>
                <div className="mb-3">
                    <label className="form-label">Perfil</label>
                    <Select
                        options={opcoesPerfis}
                        placeholder="Selecione um perfil..."
                        value={opcoesPerfis.find((p) => p.value === perfilSelecionado) || null}
                        onChange={(selectedOption) => setPerfilSelecionado(selectedOption ? selectedOption.value : null)}
                        isClearable
                    />
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

export function FuncionariosPerfis() {
    const [content, setContent] = useState(<FuncionariosPerfisList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<FuncionariosPerfisList ShowForm={ShowForm} />);
    }

    function ShowForm() {
        setContent(<FuncionariosPerfisForm ShowList={ShowList} />);
    }

    return <div className="container">{content}</div>;
}
