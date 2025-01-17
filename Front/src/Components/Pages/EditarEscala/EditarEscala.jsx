
import React, { useEffect, useState } from 'react';
import NavBar from './NavBar';
import axios from 'axios';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

function EditarEscala() {
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);

    useEffect(() => {
        BuscaEscala(2);
        BuscaPostos();
        BuscaEscalaPronta(2);
        BuscaFuncionarios();
    }, []);

    function obterQuantidadeDiasNoMes(ano, mes) {
        const ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
        return ultimoDiaDoMes;
    }

    function BuscaEscala(id) {
        axios
            .get(`https://localhost:7187/api/Escala/${id}`)
            .then((response) => {
                setEscala(response.data);
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaPostos() {
        axios
            .get(`https://localhost:7187/api/PostoTrabalho`)
            .then((response) => {
                setPostos(response.data);
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaEscalaPronta(id) {
        axios
            .get(`https://localhost:7187/api/EscalaPronta/${id}`)
            .then((response) => {
                setBuscaEscalaPronta(response.data);
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaFuncionarios() {
        axios
            .get(`https://localhost:7187/api/funcionario`)
            .then((response) => {
                setFuncionarios(response.data);
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function getUniquePostoTrabalhoIds() {
        if (buscaEscalaPronta) {
            const uniqueIds = [...new Set(buscaEscalaPronta.map((escala) => escala.postoTrabalhoId))];
            return uniqueIds;
        }
        return [];
    }

    function DiaMes() {
        return (
            <div className="diasDoMes">
                <div className="col">
                    <div className="col">
                        <div className="card" style={{ width: '50px', height: '35px' }}>
                            <div className="card-body bg-primary text-white" style={{ marginBottom: '10px', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.0rem' }}>
                                Dias
                            </div>
                        </div>

                        {Array.from({ length: numDias }, (_, index) => (
                            <div key={index + 1} className="col">
                                <div className="card" style={{ width: '50px', height: '35px' }}>
                                    <div className="card-body" style={{ marginBottom: 10, height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                        {index + 1}
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        );
    }

    function PessoaPorPosto({ idPostoTrabalho }) {
        if (buscaEscalaPronta === null) {
            return <div>Carregando...</div>;
        }

        const escalasPorPosto = buscaEscalaPronta.filter((escala) => escala.postoTrabalhoId === idPostoTrabalho);

        const nomePosto = postos.find((posto) => posto.idPostoTrabalho === idPostoTrabalho)?.nomePosto;

        // Agrupar os funcionários por dia
        const funcionariosPorDia = {};
        escalasPorPosto.forEach((escala) => {
            const dia = new Date(escala.dataServico).getDate();
            if (!funcionariosPorDia[dia]) {
                funcionariosPorDia[dia] = [];
            }
            if (!funcionariosPorDia[dia].includes(escala.funcionarioId)) {
                funcionariosPorDia[dia].push(escala.funcionarioId);
            }
        });

        return (
            <div className="pessoaPorPosto">
                <div className="card">
                    <div className="nomePosto bg-primary text-white p-1" style={{ fontSize: '1.0rem' }}>
                        {nomePosto}
                    </div>
                </div>

                {Array.from({ length: numDias }, (_, index) => {
                    const dia = index + 1;
                    const funcionariosIds = funcionariosPorDia[dia];
                    const funcionariosNomes = funcionariosPorDia[dia].map((funcionarioId) => {
                        const funcionario = funcionarios.find((funcionario) => funcionario.idFuncionario === funcionarioId);
                        return funcionario ? funcionario.nome : 'Sem funcionário';
                    });

                    return (
                        <div key={`dia-${dia}`} className="pessoa">
                            <div className="card">
                                <div className="nomesFuncionarios" style={{ marginBottom: 9, whiteSpace: 'nowrap' }}>
                                    {funcionariosNomes.join(' / ')}
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>
        );
    }


    const numDias = escala ? obterQuantidadeDiasNoMes(2023, escala.mesReferencia) : 0;

    return (
        <>
            <div className="container">
                <NavBar />
                <h3 className="text-center mb-3">Editar Escala</h3>
                {/* <NomePostos /> Removido */}
                <div className="row">
                    <div className="col">
                        <div className="row">
                            <div className="col">
                                <DiaMes />
                            </div>
                            <div className="col d-flex"> {/* Adicionado a classe d-flex */}
                                {getUniquePostoTrabalhoIds().map((postoTrabalhoId) => (
                                    <div key={postoTrabalhoId} className="col">
                                        <PessoaPorPosto idPostoTrabalho={postoTrabalhoId} />
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}


export default EditarEscala;