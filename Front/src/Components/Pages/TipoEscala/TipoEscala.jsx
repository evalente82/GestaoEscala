import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import './TipoEscala.css'

function TipoEscalaList(props) {
    TipoEscalaList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    const [currentPage, setCurrentPage] = useState(1);
    const [recordsPerPage] = useState(10); //, setRecordsPerPage

    const [searchText, setSearchText] = useState("");

    const [tipoEscala, setTipoEscala] = useState([]);

    const API_URL = "https://localhost:7207/tipoEscala";
    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setTipoEscala(response.data);
            })
            .catch((error) => {
                console.log(error);
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
        // Mostrar a popup de confirmação
        if (window.confirm("Tem certeza que deseja excluir este registro?")) {
            DeleteFuncionario(id);
        }
    }

    function DeleteFuncionario(idTipoEscala) {
        axios
            .delete(`https://localhost:7207/tipoEscala/Deletar/${idTipoEscala}`)
            .then((response) => {
                console.log(response);
                setTipoEscala(
                    tipoEscala.filter((tEscala) => tEscala.id !== idTipoEscala)
                );
                BuscarTodos();
            })
            .catch((error) => {
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
            <NavBar />
            <h3 className="text-center mb-3">Listagem de Tipos de Escala</h3>
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
                                    {/*<td>{funcionario.idFuncionario}</td>*/}
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

    // const [errorMessage, setErrorMessage] = useState('');
    const [nome, setNome] = useState(props.tipoEscala.nmNome || '');
    const [ativo, setAtivo] = useState(props.tipoEscala.isAtivo || false);
    const [expediente, setExpediente] = useState(props.tipoEscala.isExpediente || false);
    const [descricao, setDescricao] = useState(props.tipoEscala.nmDescricao || '');
    const [horasTrabalhada, setHorasTrabalhada] = useState(props.tipoEscala.nrHorasTrabalhada || '');
    const [horasFolga, setHorasFolga] = useState(props.tipoEscala.nrHorasFolga || '');
        
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
                    "https://localhost:7207/tipoEscala/Atualizar/" +
                    props.tipoEscala.idTipoEscala,
                    data
                )
                .then(() => {
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        alert(errors.Email);
                        // outros tratamentos de erro
                    } else {
                        console.log(error);
                        // outros tratamentos de erro
                    }
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
                .post("https://localhost:7207/tipoEscala/Incluir", data)
                .then(() => {
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        alert(errors.Email);
                        // outros tratamentos de erro
                    } else {
                        console.log(error);
                        // outros tratamentos de erro
                    }
                });
        }
    };
    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.tipoEscala.idTipoEscala
                    ? "Editar Tipo de Escala"
                    : "Cadastrar Tipo de Escala"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
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
