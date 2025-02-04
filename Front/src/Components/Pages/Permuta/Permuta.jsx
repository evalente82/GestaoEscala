/* eslint-disable react-hooks/exhaustive-deps */
import NavBar from "../../Menu/NavBar";
import { useEffect, useState, useRef } from "react";
import axios from "axios";
import PropTypes from 'prop-types';
import AlertPopup from '../AlertPopup/AlertPopup';
import Select from 'react-select';
import { useAuth } from "../AuthContext";

function PermutaList(props) {
    const [searchText, setSearchText] = useState("");
    const [permuta, setPermuta] = useState([]);
    const [escalas, setEscalas] = useState([]);
    const { permissoes } = useAuth();
    const possuiPermissao = (permissao) => permissoes.includes(permissao);

    const [alertProps, setAlertProps] = useState({
        show: false, // Exibe ou esconde o AlertPopup
        type: "info", // Tipo de mensagem (success, error, confirm, info)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onConfirm: null, // Callback para ações de confirmação (opcional)
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const API_URL = "https://localhost:7207/permutas";

    function BuscarTodos() {
        axios.get(`${API_URL}/buscarTodos`)
            .then((response) => {
                console.log(response.data);
                setPermuta(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar os Permutas.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
                });
            });
    }
    function BuscarEscalas() {
        axios.get("https://localhost:7207/escala/buscarTodos")
            .then((response) => {
                console.log("Escalas carregadas:", response.data);
                setEscalas(response.data); // Armazena as escalas no estado
            })
            .catch((error) => {
                console.error("Erro ao carregar escalas:", error);
            });
    }

    useEffect(() => {
        // Chamando a função BuscarTodos dentro de useEffect
        BuscarTodos();
        BuscarEscalas();
    }, []);

    PermutaList.propTypes = {
        ShowForm: PropTypes.func.isRequired, // Indica que ShowForm é uma função obrigatória
    };
        

    function handleDelete(id) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeletePermuta(id); // Executa a exclusão
                setAlertProps((prev) => ({ ...prev, show: false })); // Fecha o AlertPopup após confirmar
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
        });
    }

    function DeletePermuta(idPermuta) {
        axios
        .delete(`https://localhost:7207/permutas/Deletar/${idPermuta}`)
            .then((response) => {
                console.log(response);
                setPermuta(
                    permuta.filter((p) => p.id !== idPermuta)
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
    
    const filteredRecords = filterRecords(permuta);

    // Função para filtrar os registros com base no texto de busca
function filterRecords(records) {
    return records.filter((record) => {
        const search = searchText.toLowerCase(); // Normaliza o texto de busca

        return (
            record.nmNomeSolicitante?.toLowerCase().includes(search) || // Filtro pelo nome do Solicitante
            record.nmNomeSolicitado?.toLowerCase().includes(search) || // Filtro pelo nome do Solicitado
            record.dtDataSolicitadaTroca?.toLowerCase().includes(search) || // Filtro pela Data da Troca
            record.dtSolicitacao?.toLowerCase().includes(search) // Filtro pela Data da Solicitação
        );
    });
}

function formatarData(dataISO) {
    if (!dataISO) return ""; // Retorna vazio caso a data seja inválida ou indefinida
    const data = new Date(dataISO);
    const dia = String(data.getDate()).padStart(2, "0");
    const mes = String(data.getMonth() + 1).padStart(2, "0");
    const ano = data.getFullYear();
    return `${dia}-${mes}-${ano}`;
}


    return (
        <>
            <h3 className="text-center mb-3">Lista de Permutas</h3>
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
                        <th>NOME SOLICITANTE</th>
                        <th>NOME SOLICITADO</th>
                        <th>DATA TROCA</th>
                        <th>DATA SOLICITAÇÃO</th>
                        <th>NOME APROVADOR</th>
                        <th>DATA APROVAÇÃO</th>
                        <th>NOME ESCALA</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredRecords.map((p, index) => (
                        <tr key={index}>
                            <td style={{ textAlign: "left" }}>{p.nmNomeSolicitante}</td>
                            <td style={{ textAlign: "left" }}>{p.nmNomeSolicitado}</td>
                            <td style={{ textAlign: "left" }}>{formatarData(p.dtDataSolicitadaTroca)}</td>
                            <td style={{ textAlign: "left" }}>{formatarData(p.dtSolicitacao)}</td>
                            <td style={{ textAlign: "left" }}>{p.nmNomeAprovador}</td>
                            <td style={{ textAlign: "left" }}>{formatarData(p.dtAprovacao)}</td>
                            <td style={{ textAlign: "left" }}>{escalas.find((e) => e.idEscala === p.idEscala)?.nmNomeEscala || "Escala não encontrada"}</td>
                            <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                            {/* Botão Editar - Aparece apenas para quem tem "EditarPermuta" */}
                            {possuiPermissao("EditarPermuta") && (
                                    <button
                                        onClick={() => props.ShowForm(p)}
                                        type="button"
                                        className="btn btn-primary btn-sm me-2"
                                    >
                                        Editar
                                    </button>
                                )}
                                {/* Botão Deletar - Aparece apenas para quem tem "DeletarPermuta" */}
                                {possuiPermissao("DeletarPermuta") && (
                                    <button
                                        onClick={() => handleDelete(p.idPermuta)}
                                        type="button"
                                        className="btn btn-danger btn-sm"
                                    >
                                        Deletar
                                    </button>
                                )}
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
function PermutaForm(props) {
    PermutaForm.propTypes = {
        ShowList: PropTypes.func.isRequired,
        permuta: PropTypes.shape({
            idPermuta: PropTypes.string,
            nmNomeSolicitante: PropTypes.string,
            nmNomeSolicitado: PropTypes.string,
            nmNomeAprovador: PropTypes.string,
            idFuncionarioSolicitante: PropTypes.string,
            idFuncionarioSolicitado: PropTypes.string,
            idFuncionarioAprovador: PropTypes.string,
            idEscala: PropTypes.string,
            dtSolicitacao: PropTypes.string,
            dtDataSolicitadaTroca: PropTypes.String,
            dtAprovacao: PropTypes.bool,
        }).isRequired,
    };
    
    const [idDaEscala, setIdDaEscala] = useState(props.permuta.idEscala || '');
    useEffect(() => {
        setIdDaEscala(props.permuta.idEscala || '');
        setNomeSolicitante(props.permuta.nmNomeSolicitante || '');
        setNomeSolicitado (props.permuta.nmNomeSolicitado || '');
        setNomeAprovador(props.permuta.nmNomeAprovador || '');
        setIdFuncionarioSolicitante(props.permuta.idFuncionarioSolicitante || '');
        setIdFuncionarioSolicitado(props.permuta.idFuncionarioSolicitado || '');
        setIdFuncionarioAprovador(props.permuta.idFuncionarioAprovador || '');
        setDtSolicitacao(props.permuta.dtSolicitacao || '');
        setDtDataSolicitadaTroca(props.permuta.dtDataSolicitadaTroca || '');
        setDtAprovacao(props.permuta.dtAprovacao || '');
    }, [props.permuta]);

    const [nomeSolicitante, setNomeSolicitante] = useState(props.permuta.nmNomeSolicitante || '');
    const [nomeSolicitado, setNomeSolicitado ] = useState(props.permuta.nmNomeSolicitado || '');
    const [nomeAprovador, setNomeAprovador] = useState(props.permuta.nmNomeAprovador || '');
    const [idFuncionarioSolicitante, setIdFuncionarioSolicitante] = useState(props.permuta.idFuncionarioSolicitante || '');
    const [idFuncionarioSolicitado, setIdFuncionarioSolicitado] = useState(props.permuta.idFuncionarioSolicitado || '');
    const [idFuncionarioAprovador, setIdFuncionarioAprovador] = useState(props.permuta.idFuncionarioAprovador || '');
    const [dtSolicitacao, setDtSolicitacao] = useState(props.permuta.dtSolicitacao || '');
    const [dtDataSolicitadaTroca, setDtDataSolicitadaTroca] = useState(props.permuta.dtDataSolicitadaTroca || '');
    const [dtAprovacao, setDtAprovacao] = useState(props.permuta.dtAprovacao || '');
    const [alertProps, setAlertProps] = useState({
        show: false, // Define se o AlertPopup deve ser exibido
        type: "info", // Tipo da mensagem (success, error, info, confirm)
        title: "", // Título da modal
        message: "", // Mensagem da modal
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha a modal
    });
    const [funcionarios, setFuncionarios] = useState([]); // Lista de funcionários
    const [filtro, setFiltro] = useState(''); // Filtro de busca no select
    const [escala, setEscala] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const [diasDisponiveis, setDiasDisponiveis] = useState([]);
    const [datasTrabalhoSolicitante, setDatasTrabalhoSolicitante] = useState([]);
    const [datasTrabalhoSolicitado, setDatasTrabalhoSolicitado] = useState([]);
    const [funcionariosEscala, setFuncionariosEscala] = useState([]); // Lista de funcionários filtrados pela escala




    useEffect(() => {
        BuscarFuncionarios();
        BuscaEscala()
    }, []);

    useEffect(() => {
        if (idDaEscala) {
            BuscaEscalaPronta(idDaEscala);
        }
    }, [idDaEscala, idFuncionarioSolicitante, idFuncionarioSolicitado]);
   
    function BuscarFuncionarios() {
        axios.get("https://localhost:7207/funcionario/buscarTodos")
            .then((response) => {
                console.log(response.data);
                setFuncionarios(response.data);
            })
            .catch((error) => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível carregar a lista de funcionários.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), // Fecha o AlertPopup ao cancelar
                });
            });
    }

    function BuscaEscala() {
        axios
            .get("https://localhost:7207/escala/buscarTodos")
            .then((response) => {
                setEscala(response.data);
                console.log('buscando escala !');
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaEscalaPronta(idEscala) {
        axios
            .get(`https://localhost:7207/escalaPronta/buscarPorId/${idEscala}`)
            .then((response) => {
                const escala = response.data;
                console.log("Dados recebidos de escalaPronta:", escala);
    
                if (!Array.isArray(escala)) {
                    console.error("Formato inesperado de escala:", escala);
                    return;
                }
    
                // Obter IDs únicos de funcionários da escala selecionada
                const idsFuncionariosNaEscala = [...new Set(escala.map(dia => dia.idFuncionario))];
    
                // Filtrar os funcionários que pertencem à escala selecionada
                const funcionariosNaEscala = funcionarios.filter(f =>
                    idsFuncionariosNaEscala.includes(f.idFuncionario)
                );
    
                setFuncionariosEscala(funcionariosNaEscala); // Atualiza os funcionários filtrados
    
                // Atualizar as datas disponíveis para cada funcionário
                const mapDatasTrabalho = escala.reduce((acc, dia) => {
                    if (!acc[dia.idFuncionario]) {
                        acc[dia.idFuncionario] = [];
                    }
                    acc[dia.idFuncionario].push(dia.dtDataServico);
                    return acc;
                }, {});
    
                // Atualiza os dias disponíveis por funcionário
                setDatasTrabalhoSolicitante(mapDatasTrabalho[idFuncionarioSolicitante] || []);
                setDatasTrabalhoSolicitado(mapDatasTrabalho[idFuncionarioSolicitado] || []);
    
                console.log("Funcionários da escala selecionada:", funcionariosNaEscala);
                console.log("Datas do Solicitante:", mapDatasTrabalho[idFuncionarioSolicitante] || []);
                console.log("Datas do Solicitado:", mapDatasTrabalho[idFuncionarioSolicitado] || []);
            })
            .catch((error) => {
                console.error("Erro ao buscar escala pronta:", error);
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Falha ao carregar os funcionários da escala.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }
    
    
    

    function getMesPorExtenso(mes) {
        const meses = [
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        ];
        return meses[mes - 1] || ""; // Ajusta índice (1 para Janeiro)
    }

    const handleSubmit = (e) => {
        e.preventDefault();
    
        // Validação: Solicitante e Solicitado não podem ser iguais
        if (idFuncionarioSolicitante === idFuncionarioSolicitado) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "O solicitante e o solicitado não podem ser a mesma pessoa.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
            return;
        }
    
        // Validação: Verificar se o Solicitado já trabalha na data selecionada
        if (datasTrabalhoSolicitado.includes(dtDataSolicitadaTroca)) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "O funcionário solicitado já está trabalhando nesta data.",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
            });
            return;
        }
    
        // Dados comuns para envio
        const data = {
            idEscala: idDaEscala,
            idFuncionarioSolicitante: idFuncionarioSolicitante,
            nmNomeSolicitante: nomeSolicitante,
            idFuncionarioSolicitado: idFuncionarioSolicitado,
            nmNomeSolicitado: nomeSolicitado,
            dtSolicitacao: new Date().toISOString(), // Data atual em UTC
            dtDataSolicitadaTroca: new Date(dtDataSolicitadaTroca).toISOString(), // Converte para UTC
        };
    
        console.log("Dados para enviar:", data); // Log para depuração
    
        // Verifica se é atualização ou inclusão
        if (props.permuta.idPermuta) {
            // Atualização
            axios
                .patch(
                    `https://localhost:7207/permutas/Atualizar/${props.permuta.idPermuta}`,
                    data
                )
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Permuta atualizada com sucesso!",
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
                        message: "Falha ao atualizar a Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error("Erro ao atualizar permuta:", error);
                });
        } else {
            // Inclusão
            axios
                .post("https://localhost:7207/permutas/Incluir", data)
                .then(() => {
                    setAlertProps({
                        show: true,
                        type: "success",
                        title: "Sucesso",
                        message: "Permuta cadastrada com sucesso!",
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
                        message: "Falha ao cadastrar a Permuta.",
                        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                    });
                    console.error("Erro ao salvar permuta:", error);
                });
        }
    };
    
    
    const [selectedOption, setSelectedOption] = useState(null);

    function SelectComFiltroSolicitante({ value, onChange }) {
        const opcoes = funcionariosEscala.map((f) => ({
            value: f.idFuncionario,
            label: `${f.nmNome} - ${f.nrMatricula}`,
        }));
    
        return (
            <Select
                options={opcoes}
                placeholder="Digite para buscar..."
                value={opcoes.find((o) => o.value === value) || null}
                onChange={(selectedOption) => {
                    if (selectedOption) {
                        setIdFuncionarioSolicitante(selectedOption.value);
                        setNomeSolicitante(selectedOption.label);
                        
                        // Atualiza as datas disponíveis para o solicitante
                        setDatasTrabalhoSolicitante( 
                            escala.find(e => e.idFuncionario === selectedOption.value)?.datas || []
                        );
                    } else {
                        setIdFuncionarioSolicitante(null);
                        setNomeSolicitante("");
                        setDatasTrabalhoSolicitante([]);
                    }
                    onChange(selectedOption ? selectedOption.value : null);
                }}
                isClearable
                noOptionsMessage={() => "Nenhum funcionário disponível"}
            />
        );
    }
    
    
    function SelectComFiltroSolicitado({ value, onChange }) {
        const opcoes = funcionariosEscala.map((f) => ({
            value: f.idFuncionario,
            label: `${f.nmNome} - ${f.nrMatricula}`,
        }));
    
        return (
            <Select
                options={opcoes}
                placeholder="Digite para buscar..."
                value={opcoes.find((o) => o.value === value) || null}
                onChange={(selectedOption) => {
                    onChange(selectedOption ? selectedOption.value : null);
                }}
                isClearable
                noOptionsMessage={() => "Nenhum funcionário na escala"}
            />
        );
    }
    
    


    return (
        <>
            <h2 className="text-center mb-3">
                {props.permuta.idPermuta
                    ? "Editar Permuta"
                    : "Cadastrar Nova Permuta"}
            </h2>
            <div className="row">
                <div className="col-lg-6 mx-auto">
                    {/* {errorMessage} */}
                    <form onSubmit={(e) => handleSubmit(e)}>
                        {props.permuta.idPermuta && (
                            <div className="row mb-3">
                                <label className="col-sm-4 col-form-label">ID</label>
                                <div className="col-sm-8">
                                    <input
                                        readOnly
                                        className="form-control-plaintext"
                                        name="idPermuta"
                                        defaultValue={props.permuta.idPermuta}
                                        required
                                        //onChange={(e) => setNome(e.target.value)}
                                    ></input>
                                </div>
                            </div>
                        )}

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">ID DA ESCALA</label>
                            <div className="col-sm-8">
                            <Select
                                options={(escala || []).map((e) => ({
                                    value: e.idEscala,
                                    label: `${e.nmNomeEscala} - ${getMesPorExtenso(e.nrMesReferencia)}`,
                                }))}
                                placeholder="Selecione uma escala"
                                onChange={(selectedOption) => {
                                    if (selectedOption) {
                                        setIdDaEscala(selectedOption.value);
                                        BuscaEscalaPronta(selectedOption.value);
                                    }
                                }}
                                isClearable
                                noOptionsMessage={() => "Nenhuma escala encontrada"}
                            />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitante</label>
                            <div className="col-sm-8">
                            <SelectComFiltroSolicitante
                            name="nomeSolicitante"
                            funcionarios={funcionarios}
                            value={idFuncionarioSolicitante} // Valor controlado pelo estado
                            onChange={(id) => {
                                const funcionarioSelecionado = funcionarios.find((f) => f.idFuncionario === id);

                                if (funcionarioSelecionado) {
                                    setIdFuncionarioSolicitante(funcionarioSelecionado.idFuncionario);
                                    setNomeSolicitante(funcionarioSelecionado.nmNome);
                                    console.log("Solicitante selecionado:", funcionarioSelecionado); // Log do funcionário selecionado
                                } else {
                                    setIdFuncionarioSolicitante(null);
                                    setNomeSolicitante("");
                                    console.log("Nenhum solicitante selecionado.");
                                }
                            }}
                        />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Nome Solicitado</label>
                            <div className="col-sm-8">
                            <SelectComFiltroSolicitado
                            name="NomeSolicitado"
                            funcionarios={funcionarios}
                            value={idFuncionarioSolicitado} // Valor controlado pelo estado
                            onChange={(id) => {
                                const funcionarioSelecionado = funcionarios.find((f) => f.idFuncionario === id);

                                if (funcionarioSelecionado) {
                                    setIdFuncionarioSolicitado(funcionarioSelecionado.idFuncionario);
                                    setNomeSolicitado(funcionarioSelecionado.nmNome);
                                    console.log("Solicitado selecionado:", funcionarioSelecionado); // Log do funcionário selecionado
                                } else {
                                    setIdFuncionarioSolicitado(null);
                                    setNomeSolicitado("");
                                    console.log("Nenhum solicitado selecionado.");
                                }
                            }}
                        />
                            </div>
                        </div>

                        <div className="row mb-3">
                            <label className="col-sm-4 col-form-label">Data da troca</label>
                            <div className="col-sm-8">
                                <select
                                    className="form-control"
                                    value={dtDataSolicitadaTroca}
                                    onChange={(e) => setDtDataSolicitadaTroca(e.target.value)}
                                >
                                    <option value="" disabled>
                                        Selecione uma data
                                    </option>
                                    {datasTrabalhoSolicitante.length > 0 ? (
                                        datasTrabalhoSolicitante.map((dia, index) => (
                                            <option key={`${index}-${dia}`} value={dia}>
                                                {dia}
                                            </option>
                                        ))
                                    ) : (
                                        <option disabled>Sem datas disponíveis</option>
                                    )}
                                </select>
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
export function Permuta() {
    const [content, setContent] = useState(
        <PermutaList ShowForm={ShowForm} />
    );

    function ShowList() {
        setContent(<PermutaList ShowForm={ShowForm} />);
    }

    function ShowForm(permuta) {
        setContent(
            <PermutaForm permuta={permuta} ShowList={ShowList} ShowForm={ShowForm} />
        );
    }
    return <div className="container">{content}</div>;
}