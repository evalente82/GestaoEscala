import NavBar from "../../Menu/NavBar";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from 'prop-types';

function CargoList(props) {
    CargoList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
    const [currentPage, setCurrentPage] = useState(1);
    const [recordsPerPage] = useState(10); //, setRecordsPerPage

    const [searchText, setSearchText] = useState("");

    const [cargo, setCargo] = useState([]);

    const API_URL = "https://localhost:7207/cargo";
    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setCargo(response.data);
            })
            .catch((error) => {
                console.log(error);
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
        // Mostrar a popup de confirmação
        if (window.confirm("Tem certeza que deseja excluir este registro?")) {
            DeleteCargo(id);
        }
    }

    function DeleteCargo(idCargos) {
        axios
            .delete(`https://localhost:7207/cargo/Deletar/${idCargos}`)
            .then((response) => {
                console.log(response);
                setCargo(
                    cargo.filter((usuario) => usuario.id !== idCargos)
                );
                BuscarTodos();
            })
            .catch((error) => {
                console.error(error);
            });
    }

    //const indexOfLastRecord = currentPage * recordsPerPage;
    //const indexOfFirstRecord = indexOfLastRecord - recordsPerPage;
    const currentRecords = cargo
    //    .filter(
    //        (departamento) =>
    //            departamento.nome.toLowerCase().includes(searchText.toLowerCase()) ||
    //            departamento.descricao.toLowerCase().includes(searchText.toLowerCase())
    //    )
    //    .slice(indexOfFirstRecord, indexOfLastRecord);
    //useEffect(() => BuscarTodos(), []);


    return (
        <>
            <NavBar />
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
            <div className="d-flex justify-content-center">
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
            </div>
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
                                    <td>{cargo.nmNome}</td>
                                    <td>{cargo.nmDescricao}</td>
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
                                            onClick={() => handleDelete(cargo.idCargos)}
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
function CargoForm(props) {
    CargoForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        cargo: PropTypes.shape({
            idCargos: PropTypes.number,
            nmNome: PropTypes.string,
            nmDescricao: PropTypes.string,
            isAtivo: PropTypes.bool,
        }).isRequired,
    };

    const [nome, setNome] = useState(props.cargo.nmNome || '');
    const [descricao, setDescricao] = useState(props.cargo.nmDescricao || '');
    const [ativo, setAtivo] = useState(props.cargo.isAtivo || false);

    function handleAtivoChange(e) {
        setAtivo(e.target.checked);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        if (props.cargo.idCargos) {
            const data = {
                nmNome: nome,
                nmDescricao: descricao,
                isAtivo: ativo,
            };
            axios
                .patch(
                    "https://localhost:7207/cargo/Atualizar/" +
                    props.cargo.idCargos,
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
                    } else {
                        console.log(error);
                    }
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
                    props.ShowList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        const errors = error.response.data;
                        console.log(errors);
                        alert(errors.Email);
                    } else {
                        console.log(error);
                    }
                });
        }
    };

    return (
        <>
            <NavBar />
            <h2 className="text-center mb-3">
                {props.cargo.idCargos
                    ? "Editar Cargo"
                    : "Cadastrar Novo Cargo"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.cargo.idCargos && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idCargos"
                                        defaultValue={props.cargo.idCargos}
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