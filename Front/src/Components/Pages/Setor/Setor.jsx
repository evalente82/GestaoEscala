import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';

function SetorList(props) {
    SetorList.propTypes = {
        ShowForm: PropTypes.func.isRequired,
    };

    const [searchText, setSearchText] = useState("");
    const [setores, setSetores] = useState([]);
    const API_URL = "https://localhost:7207/setor";

    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    function BuscarTodos() {
        console.log("[LOG] Buscando todos os setores...");
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log("[LOG] Setores recebidos do backend:", response.data);
                setSetores(response.data);
            })
            .catch((error) => {
                console.error("[ERRO] Erro ao buscar setores:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os setores.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

    useEffect(() => {
        BuscarTodos();
    }, []);

    function handleDelete(id) {
        console.log("[LOG] Tentando excluir setor:", id);
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este setor?",
            onConfirm: () => {
                DeleteSetor(id);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeleteSetor(idSetor) {
        console.log("[LOG] Enviando requisição para deletar setor:", idSetor);
        axios
            .delete(`${API_URL}/Deletar/${idSetor}`)
            .then(() => {
                console.log("[LOG] Setor deletado com sucesso.");
                setSetores(setores.filter((s) => s.idSetor !== idSetor));
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
                console.error("[ERRO] Falha ao excluir setor:", error);
            });
    }

    const currentRecords = setores.filter(record =>
        record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||
        (record.nmDescricao || "").toLowerCase().includes(searchText.toLowerCase())
    );

    return (
        <>
            <h3 className="text-center mb-3">Listagem de Setores</h3>
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
            <br /><br />
            <input
                type="text"
                value={searchText}
                onChange={(e) => {
                    console.log("[LOG] Pesquisando setor:", e.target.value);
                    setSearchText(e.target.value);
                }}
                placeholder="Pesquisar..."
                className="form-control mb-3"
            />

            <table className="table">
                <thead>
                    <tr>
                        <th>NOME</th>
                        <th>DESCRIÇÃO</th>
                        <th>ATIVO</th>
                        <th>AÇÕES</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords.map((setor, index) => (
                        <tr key={index}>
                            <td>{setor.nmNome}</td>
                            <td>{setor.nmDescricao}</td>
                            <td>
                                <input type="checkbox" checked={setor.isAtivo} readOnly />
                            </td>
                            <td>
                                <button onClick={() => {
                                    console.log("[LOG] Editando setor:", setor);
                                    props.ShowForm(setor);
                                }} className="btn btn-primary btn-sm me-2">
                                    Editar
                                </button>
                                <button onClick={() => handleDelete(setor.idSetor)} className="btn btn-danger btn-sm">
                                    Excluir
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <AlertPopup {...alertProps} />
        </>
    );
}

function SetorForm(props) {
    SetorForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        setor: PropTypes.shape({
            idSetor: PropTypes.string,
            nmNome: PropTypes.string,
            nmDescricao: PropTypes.string,
            isAtivo: PropTypes.bool,
        }),
    };

    const setorInicial = props.setor || { idSetor: "", nmNome: "", nmDescricao: "", isAtivo: false };

    const [nome, setNome] = useState(setorInicial.nmNome);
    const [descricao, setDescricao] = useState(setorInicial.nmDescricao);
    const [ativo, setAtivo] = useState(setorInicial.isAtivo);
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

    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = { nmNome: nome, nmDescricao: descricao, isAtivo: ativo };
    
        console.log("[LOG] Enviando dados para salvar setor:", data);
    
        try {
            if (setorInicial.idSetor) {
                await axios.patch(`https://localhost:7207/setor/Atualizar/${setorInicial.idSetor}`, data);
                console.log("[LOG] Setor atualizado com sucesso.");
    
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Setor atualizado com sucesso!",
                    onClose: () => {
                        setAlertProps(prev => ({ ...prev, show: false }));
                        props.ShowList(); // 🔹 Só exibe a lista depois do alerta ser fechado
                    },
                });
    
            } else {
                await axios.post("https://localhost:7207/setor/Incluir", data);
                console.log("[LOG] Novo setor cadastrado com sucesso.");
    
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Setor cadastrado com sucesso!",
                    onClose: () => {
                        setAlertProps(prev => ({ ...prev, show: false }));
                        props.ShowList(); // 🔹 Só exibe a lista depois do alerta ser fechado
                    },
                });
            }
        } catch (error) {
            console.error("[ERRO] Falha ao salvar setor:", error);
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Falha ao cadastrar ou atualizar o Setor.",
                onClose: () => setAlertProps(prev => ({ ...prev, show: false })),
            });
        }
    };
    

    return (
        <>
            <h2 className="text-center mb-3">{setorInicial.idSetor ? "Editar Setor" : "Cadastrar Novo Setor"}</h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={handleSubmit}>
                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome do Setor</label>
                            <div className="col-sm-8">
                                <input className="form-control" value={nome} onChange={(e) => setNome(e.target.value)} required />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Descrição</label>
                            <div className="col-sm-8">
                                <input className="form-control" value={descricao} onChange={(e) => setDescricao(e.target.value)} required />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input className="form-check-input" type="checkbox" checked={ativo} onChange={handleAtivoChange} />
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

export function Setor() {
    const [content, setContent] = useState(<SetorList ShowForm={ShowForm} />);

    function ShowList() { setContent(<SetorList ShowForm={ShowForm} />); }
    function ShowForm(setor) { setContent(<SetorForm setor={setor} ShowList={ShowList} />); }

    return <div className="container">{content}</div>;
}

export default SetorForm;
