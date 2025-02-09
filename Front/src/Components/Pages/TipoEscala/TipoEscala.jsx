import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import './TipoEscala.css'
import AlertPopup from '../AlertPopup/AlertPopup';

function TipoEscalaList(props) {
    TipoEscalaList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    const [searchText, setSearchText] = useState("");
    const [tipoEscala, setTipoEscala] = useState([]);
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const API_URL = `${API_BASE_URL}/tipoEscala`;

    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setTipoEscala(response.data);
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
            setTipoEscala(response.data);
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
                DeleteTipoEscala(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }
    
    function DeleteTipoEscala(idTipoEscala) {
        axios
            .delete(`${API_URL}/Deletar/${idTipoEscala}`)
            .then((response) => {
                console.log(response);
                setTipoEscala(
                    tipoEscala.filter((usuario) => usuario.id !== idTipoEscala)
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
    const currentRecords = filterRecords(tipoEscala)

    // Função para filtrar os registros com base no texto de busca
    function filterRecords(records) {
        return records.filter(record => {
            const filtro = tipoEscala.find(x => x.idTipoEscala === record.idTipoEscala)?.nmNome || "";
            return (
                record.nmNome.toLowerCase().includes(searchText.toLowerCase()) ||
                filtro.toLowerCase().includes(searchText.toLowerCase())
            );
        });
    }

    return (
        <>
            
            <h3 className="text-center mb-3">Listagem de Tipos de Escala</h3>
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
                        <th style={{ textAlign: "left" }}>NOME</th>
                        <th style={{ textAlign: "left" }}>DESCRIÇÃO</th>
                        <th >HORAS TRABALHADA</th>
                        <th>HORAS DE FOLGA</th>
                        <th>EXPEDIENTE</th>
                        <th>ATIVO</th>
                    </tr>
                </thead>
                <tbody>
                    {currentRecords
                        .map((tipoEscala, index) => {
                            return (
                                <tr key={index}>
                                    <td style={{ textAlign: "left" }}>{tipoEscala.nmNome}</td>
                                    <td style={{ textAlign: "left" }}>{tipoEscala.nmDescricao}</td>
                                    <td>{tipoEscala.nrHorasTrabalhada}</td>
                                    <td>{tipoEscala.nrHorasFolga}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={tipoEscala.isExpediente == 1}
                                            readOnly
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={tipoEscala.isAtivo == 1}
                                            readOnly
                                        />
                                    </td>                                   
                                    <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                                        <button
                                            onClick={() => props.ShowForm(tipoEscala)}
                                            type="button"
                                            className="btn btn-primary btn-sm me-2"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleDelete(tipoEscala.idTipoEscala)}
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
function TipoEscalaForm(props) {
    TipoEscalaForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        tipoEscala: PropTypes.shape({
            idTipoEscala: PropTypes.number,
            nmNome: PropTypes.string,
            nmDescricao: PropTypes.number,
            nrHorasTrabalhada: PropTypes.number,
            nrHorasFolga: PropTypes.number,
            isExpediente: PropTypes.bool,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.tipoEscala.nmNome || '');
    const [ativo, setAtivo] = useState(props.tipoEscala.isAtivo || false);
    const [expediente, setExpediente] = useState(props.tipoEscala.isExpediente || false);
    const [descricao, setDescricao] = useState(props.tipoEscala.nmDescricao || '');
    const [horasTrabalhada, setHorasTrabalhada] = useState(props.tipoEscala.nrHorasTrabalhada || '');
    const [horasFolga, setHorasFolga] = useState(props.tipoEscala.nrHorasFolga || '');
    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;
    
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
    function handleExpedienteChange(e) {
        setExpediente(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.tipoEscala.idTipoEscala) {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                nrHorasTrabalhada: horasTrabalhada,
                nrHorasFolga: horasFolga,
                isExpediente: expediente,
                isAtivo: ativo,
            };
            axios
                .patch(
                    `${API_BASE_URL}/tipoEscala/Atualizar/` +
                    props.tipoEscala.idTipoEscala,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Tipo de Escala atualizado com sucesso!",
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
                        message: "Falha ao atualizar o Tipo de Escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        } else {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                nrHorasTrabalhada: horasTrabalhada,
                nrHorasFolga: horasFolga,
                isExpediente: expediente,
                isAtivo: ativo,
            };
            axios
                .post(`${API_BASE_URL}/tipoEscala/Incluir`, data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Tipo de Escala cadastrado com sucesso!",
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
                        message: "Falha ao cadastrar o Tipo de Escala.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error(error);
                });
        }
    };
    return (
        <>
            <h2 className="text-center mb-3">
                {props.tipoEscala.idTipoEscala
                    ? "Editar Tipo de Escala"
                    : "Cadastrar Tipo de Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.tipoEscala.idTipoEscala && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idTipoEscala"
                                        defaultValue={props.tipoEscala.idTipoEscala}
                                        required
                                        onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome da Escala</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-control"
                                    name="nome"
                                    defaultValue={props.tipoEscala.nmNome}
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
                                    defaultValue={props.tipoEscala.nmDescricao}
                                    required
                                    onChange={(e) => setDescricao(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Horas Trabalhada</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    className="form-control"
                                    name="horasTrabalhada"
                                    defaultValue={props.tipoEscala.nrHorasTrabalhada}
                                    required
                                    onChange={(e) => setHorasTrabalhada(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Horas de Folga</label>
                            <div className="col-sm-8">
                                <input
                                    type="number"
                                    className="form-control"
                                    name="horasFolga"
                                    defaultValue={props.tipoEscala.nrHorasFolga}
                                    required
                                    onChange={(e) => setHorasFolga(e.target.value)}
                                ></input>
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Expediente</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-check-input"
                                    name="expediente"
                                    type="checkbox"
                                    value={props.tipoEscala.isExpediente}
                                    checked={expediente}
                                    onChange={handleExpedienteChange}
                                />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Ativo</label>
                            <div className="col-sm-8">
                                <input
                                    className="form-check-input"
                                    name="ativo"
                                    type="checkbox"
                                    value={props.tipoEscala.isAtivo}
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
export function TipoEscala() {
    const [content, setContent] = useState(
        <TipoEscalaList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<TipoEscalaList ShowForm={ShowForm} />);
    }

    function ShowForm(tipoEscala) {
        setContent(
            <TipoEscalaForm tipoEscala={tipoEscala} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}
