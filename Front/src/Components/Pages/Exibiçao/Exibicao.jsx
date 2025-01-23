import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";
import { useParams } from 'react-router-dom';
import './Exibicao.css';


export function Exibicao() {
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const { idEscala } = useParams();
    const [showEditContent, setShowEditContent] = useState(false);
    const [funcionarioOrigem, setFuncionarioOrigem] = useState('');
    const [funcionarioDestino, setFuncionarioDestino] = useState('');
    const [escalaAlterada, setEscalaAlterada] = useState([]);
    const [highlightedIds, setHighlightedIds] = useState([]); // IDs a serem destacados

    useEffect(() => {
        BuscaEscala(idEscala);
        BuscaFuncionarios();
        BuscaEscalaPronta(idEscala);
    }, []);

    useEffect(() => {
        if (escala) {
            BuscaPostos(escala.idDepartamento);
        }
    }, [escala]);

    useEffect(() => {
        if (buscaEscalaPronta) {
            setEscalaAlterada(buscaEscalaPronta); // Preenche escalaAlterada com os dados iniciais
        }
    }, [buscaEscalaPronta]);

    useEffect(() => {
        if (escala && buscaEscalaPronta) {
            BuscaPostos(escala.idDepartamento);
        }
    }, [escala, buscaEscalaPronta]);

    function obterQuantidadeDiasNoMes(ano, mes) {
        const ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
        return ultimoDiaDoMes;
    }

    function BuscaEscala(id) {
        axios
            .get(`https://localhost:7207/escala/buscarPorId/${id}`)
            .then((response) => {
                setEscala(response.data);
                // console.log('buscando escala !');
                // console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaPostos(idDepartamento) {
        axios
            .get(`https://localhost:7207/PostoTrabalho/buscarTodos`)
            .then((response) => {
                // Verifique se buscaEscalaPronta está disponível, caso contrário use um Set vazio
                const postosNaEscala = buscaEscalaPronta
                    ? new Set(buscaEscalaPronta.map(item => item.idPostoTrabalho))
                    : new Set();
    
                // Filtre apenas os postos que pertencem ao departamento e estão na escala
                const postosFiltrados = response.data.filter(
                    posto => posto.idDepartamento === idDepartamento && postosNaEscala.has(posto.idPostoTrabalho)
                );
    
                setPostos(postosFiltrados);
                console.log("Postos filtrados:", postosFiltrados);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaFuncionarios() {
        axios
            .get(`https://localhost:7207/funcionario/buscarTodos`)
            .then((response) => {
                setFuncionarios(response.data);
                console.log('Funcionarios');
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaEscalaPronta(id) {
        axios
            .get(`https://localhost:7207/escalaPronta/buscarPorId/${id}`)
            .then((response) => {
                setBuscaEscalaPronta(response.data);
                console.log('buscaEscalaPronta');
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    const obterNomeFuncionario = (idFuncionario) => {
        const funcionario = funcionarios?.find(func => func.idFuncionario === idFuncionario);
        return funcionario ? funcionario.nmNome : 'Desconhecido';
    };

    const obterIdFuncionarioPorNome = (nome) => {
        if (!funcionarios) {
            console.log('Lista de funcionários não carregada.');
            return null; // Retorna null explicitamente
        }
    
        console.log('Nome do funcionário:', nome);
        const funcionario = funcionarios.find(func => func.nmNome === nome);
        if (!funcionario) {
            console.log(`Funcionário com o nome "${nome}" não encontrado.`);
        }
        return funcionario ? funcionario.idFuncionario : null;
    };


    const handleTrocarFuncionario = () => {
        const idOrigem = obterIdFuncionarioPorNome(funcionarioOrigem);
        const idDestino = obterIdFuncionarioPorNome(funcionarioDestino);

        if (idOrigem && idDestino) {
            console.log('IDs encontrados:', { idOrigem, idDestino });

            // Atualiza escalaAlterada trocando os IDs de origem e destino
            const novaEscala = escalaAlterada.map(item => {
                if (item.idFuncionario === idOrigem) {
                    return { ...item, idFuncionario: idDestino };
                }
                if (item.idFuncionario === idDestino) {
                    return { ...item, idFuncionario: idOrigem };
                }
                return item;
            });

            setEscalaAlterada(novaEscala); // Atualiza o estado
            setHighlightedIds([idOrigem, idDestino]); // Define os IDs para destaque

            // Remove o destaque após 3 segundos
            setTimeout(() => {
                setHighlightedIds([]);
            }, 3000);

            console.log('Escala atualizada com troca completa:', novaEscala);
        } else {
            console.log('IDs de origem ou destino não encontrados.');
        }
    };

    const handleSalvarEscalaAlterada = async () => {
        if (!escalaAlterada || escalaAlterada.length === 0) {
            alert("Não há dados para salvar.");
            return;
        }
    
        try {
            const response = await axios.put("https://localhost:7207/escala/SalvarEscalaAlterada", escalaAlterada);
            if (response.status === 200) {
                alert("Escala salva com sucesso!");
            } else {
                alert("Erro ao salvar a escala.");
            }
        } catch (error) {
            console.error("Erro ao salvar a escala:", error);
            alert("Erro ao salvar a escala.");
        }
    };
    

    const numDias = escala ? obterQuantidadeDiasNoMes(2025, escala.nrMesReferencia) : 0;

    return (
        <>
            <NavBar />
            <div className="container mt-3">
                <h1>Exibição da Escala {escala ? escala.nmNomeEscala : 'Carregando...'}</h1>
                <button
                    type="button"
                    className="btn btn-outline-primary me-2"
                    onClick={() => setShowEditContent(!showEditContent)}
                >
                    EDITAR
                </button>
                {showEditContent && (
                    <>
                        <div className="container highlight-box mt-3">
                            <div className="row">
                                <div className="col-4">
                                    <input
                                        className="form-control mb-2"
                                        style={{ width: '100%' }}
                                        value={funcionarioOrigem}
                                        onChange={(e) => setFuncionarioOrigem(e.target.value)}
                                        placeholder="Funcionário Origem"
                                    />
                                    <input
                                        className="form-control mb-2"
                                        style={{ width: '100%' }}
                                        value={funcionarioDestino}
                                        onChange={(e) => setFuncionarioDestino(e.target.value)}
                                        placeholder="Funcionário Destino"
                                    />
                                    
                                </div>
                                
                                <div className="col-1">
                                    <button
                                        type="button"
                                        className="btn btn-outline-primary me-2"
                                        onClick={handleTrocarFuncionario}
                                    >
                                        Trocar
                                    </button>
                                    
                                </div> 
                                <div className="row">
                                    <div className="col-1">
                                        <button
                                            type="button"
                                            className="btn btn-outline-primary me-2"
                                            onClick={handleSalvarEscalaAlterada}
                                        >
                                            Salvar
                                        </button>
                                    </div>   
                                </div>
                            </div>
                        </div>
                    </>
                )}
                
                <div className="table-container mt-3">                
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Dia</th>
                                {postos && postos.map((posto) => (
                                    <th key={posto.idPostoTrabalho}>{posto.nmNome}</th>
                                ))}
                            </tr>
                        </thead>
                        <tbody>
                            {Array.from({ length: numDias }, (_, index) => (
                                <tr key={index + 1}>
                                    <td className="border">{index + 1}</td>
                                    {postos && postos.map((posto) => (
                                        <td key={posto.idPostoTrabalho}>
                                            {escalaAlterada
                                                .filter(item =>
                                                    new Date(item.dtDataServico).getDate() === index + 1 &&
                                                    item.idPostoTrabalho === posto.idPostoTrabalho
                                                )
                                                .map(item => (
                                                    <div
                                                        key={item.idEscalaPronta}
                                                        className={highlightedIds.includes(item.idFuncionario) ? 'highlight' : ''}
                                                    >
                                                        {obterNomeFuncionario(item.idFuncionario)}
                                                    </div>
                                                ))}
                                        </td>
                                    ))}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </>
    );
}

export default Exibicao;