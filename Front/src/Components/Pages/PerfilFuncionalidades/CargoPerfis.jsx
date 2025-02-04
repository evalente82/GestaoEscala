/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
import AlertPopup from "../AlertPopup/AlertPopup";
import Select from 'react-select';


function CargoPerfisList(props) {
    const [searchText, setSearchText] = useState("");
    const [cargoPerfis, setCargoPerfis] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    const API_URL = "https://localhost:7207/cargoPerfis";

    function BuscarCargoPerfis() {
        console.log("Buscando Cargo e perfis...");
        axios
            .get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log("Dados recebidos da API:", response.data);
                setCargoPerfis(response.data);
            })
            .catch((error) => {
                console.error("Erro ao buscar Cargo e perfis:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar as vinculações de cargo e perfis.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarCargoPerfis();
    }, []);

    const filteredCargoPerfis = cargoPerfis.filter((fp) =>
        fp.nomeCargo?.toLowerCase().includes(searchText.toLowerCase())
    );

    console.log("Cargo e Perfis filtrados:", filteredCargoPerfis);

    function handleDelete(idCargo, idPerfil) {
        console.log("Iniciando exclusão do perfil...");
        console.log("ID Cargo:", idCargo);
        console.log("ID Perfil:", idPerfil);

        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar Desvinculação",
            message: "Tem certeza que deseja remover este perfil do Cargo?",
            onConfirm: () => {
                DesvincularPerfil(idCargo, idPerfil);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DesvincularPerfil(idCargo, idPerfil) {
        console.log("Chamando API para desvincular perfil...");
        console.log("ID Cargo:", idCargo);
        console.log("ID Perfil:", idPerfil);

        axios
            .delete(`${API_URL}/deletar`, {
                data: {
                    idFuncionario: idCargo,
                    idPerfil: idPerfil,
                },
            })
            .then(() => {
                console.log("Perfil desvinculado com sucesso!");
                BuscarCargoPerfis();
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Perfil desvinculado do Cargo com sucesso!",
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
            <h3 className="text-center mb-3">Cargo e Perfis</h3>
                <div className="text-center mb-3">
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
                </div>
            <br />
            <br />
            <input
                type="text"
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
                placeholder="Pesquisar Cargos..."
                className="form-control mb-3"
            />
            <table className="table">
                <thead>
                    <tr>
                        <th>CARGOS</th>
                        <th>PERFIL</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredCargoPerfis.length > 0 ? (
                        filteredCargoPerfis.map((fp, index) => (
                            <tr key={index}>
                                <td>{fp.nomeCargo || "N/A"}</td>
                                <td>{fp.nomePerfil || "N/A"}</td>
                                <td>
                                    <button
                                        onClick={() => handleDelete(fp.idCargo, fp.idPerfil)}
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
                                Nenhum Cargo encontrado.
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

function CargoPerfisForm(props) {
    CargoPerfisForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
    };

    const [cargos, setCargos] = useState([]);
    const [perfis, setPerfis] = useState([]);
    const [cargoSelecionado, setCargoSelecionado] = useState("");
    const [perfilSelecionado, setPerfilSelecionado] = useState("");
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    useEffect(() => {
        axios.get("https://localhost:7207/cargo/buscarTodos")
            .then((response) => {
                console.log("Dados de Cargos recebidossssss:", response.data);
                setCargos(response.data);
            })
            .catch((error) => console.error("Erro ao buscar Cargos:", error));

        axios.get("https://localhost:7207/perfil/buscarTodos")
            .then((response) => {
                console.log("Dados de perfis recebidos:", response.data);
                setPerfis(response.data);
            })
            .catch((error) => console.error("Erro ao buscar perfis:", error));
    }, []);

    const opcoesCargos = cargos
        .filter((c) => c.idCargo && c.nmNome)
        .map((c) => ({
            value: c.idCargo,
            label: `${c.nmNome}`,
        }));

    const opcoesPerfis = perfis.map((perfil) => ({
        value: perfil.idPerfil,
        label: perfil.nome,
    }));

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

    // Buscar corretamente os nomes associados ao ID selecionado
    const perfilSelecionadoNome = perfis.find(
        (perfil) => perfil.idPerfil === perfilSelecionado
    )?.nome || "Perfil não encontrado"; // Ajustado para evitar undefined

    const cargoSelecionadoNome = cargos.find(
        (cargo) => cargo.idCargo === cargoSelecionado
    )?.nmNome || "Cargo não encontrado"; // Ajustado para evitar undefined

    console.log("Cargo Selecionado (ID):", cargoSelecionado);
    console.log("Cargo Selecionado (Nome):", cargoSelecionadoNome);
    console.log("Perfil Selecionado (ID):", perfilSelecionado);
    console.log("Perfil Selecionado (Nome):", perfilSelecionadoNome);

    // Validar se os valores são válidos antes da requisição
    if (!cargoSelecionado || !perfilSelecionado) {
        setAlertProps({
            show: true,
            type: "error",
            title: "Erro",
            message: "Selecione um cargo e um perfil antes de continuar.",
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
        return;
    }

    try {
        const response = await axios.post("https://localhost:7207/cargoPerfis/incluir", {
            idCargo: cargoSelecionado,
            nomeCargo: cargoSelecionadoNome,
            idPerfil: perfilSelecionado,
            nomePerfil: perfilSelecionadoNome,
        });

        console.log("Resposta da API:", response.data);

        setAlertProps({
            show: true,
            type: "success",
            title: "Sucesso",
            message: "Perfil vinculado ao Cargo com sucesso!",
            onClose: () => {
                setAlertProps((prev) => ({ ...prev, show: false }));
                props.ShowList();
            },
        });
    } catch (error) {
        console.error("Erro ao vincular perfil ao Cargo:", error.response?.data || error.message);

        setAlertProps({
            show: true,
            type: "error",
            title: "Erro",
            message: error.response?.data?.mensagem || "Falha ao vincular perfil ao Cargo.",
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }
};


    return (
        <>
            <h2 className="text-center mb-3">Vincular Perfil ao Cargo</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Cargo</label>
                    <SelectComFiltro
                        options={opcoesCargos}
                        value={cargoSelecionado}
                        onChange={setCargoSelecionado}
                        placeholder="Selecione um cargo..."
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

export function CargoPerfis() {
    const [content, setContent] = useState(<CargoPerfisList ShowForm={ShowForm} />);

    function ShowList() {
        setContent(<CargoPerfisList ShowForm={ShowForm} />);
    }

    function ShowForm() {
        setContent(<CargoPerfisForm ShowList={ShowList} />);
    }

    return <div className="container">{content}</div>;
}
