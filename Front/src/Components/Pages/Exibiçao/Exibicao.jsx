import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";

export function Exibicao() {
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [indiceInicial, setIndiceInicial] = useState(0);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);

    useEffect(() => {
        BuscaEscala(2);
        BuscaPostos();
        BuscaFuncionarios();
        BuscaEscalaPronta(2);
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

    function handleProximo() {
        const proximoIndiceInicial = indiceInicial + 5;
        if (proximoIndiceInicial < postos.length) {
            setIndiceInicial(proximoIndiceInicial);
        }
    }

    function handleAnterior() {
        const anteriorIndiceInicial = indiceInicial - 5;
        if (anteriorIndiceInicial >= 0) {
            setIndiceInicial(anteriorIndiceInicial);
        }
    }

    function handleDragEnd(result) {
        if (!result.destination) {
            return;
        }

        const funcionarioId = result.draggableId.split('-')[0];
        const origemPostoTrabalhoId = result.source.droppableId;
        const destinoPostoTrabalhoId = result.destination.droppableId;

        const novaEscalaAtualizada = buscaEscalaPronta.map((escala) => ({ ...escala }));

        const escalaAtualizada = novaEscalaAtualizada.find(
            (escala) =>
                escala.funcionarioId === funcionarioId &&
                escala.postoTrabalhoId === origemPostoTrabalhoId
        );

        if (escalaAtualizada) {
            escalaAtualizada.postoTrabalhoId = destinoPostoTrabalhoId;
            setEscala({ ...escala });
        }

        setBuscaEscalaPronta(novaEscalaAtualizada);
    }

    if (
        escala === null ||
        postos === null ||
        buscaEscalaPronta === null ||
        funcionarios === null
    ) {
        return (
            <>
                <NavBar />
                <h1>Exibição da escala</h1>
                <p>Carregando...</p>
            </>
        );
    }

    const numDias = obterQuantidadeDiasNoMes(2025, escala.mesReferencia);
    const postosExibicao = postos;

    const escalaExibicao = buscaEscalaPronta !== null ? buscaEscalaPronta : buscaEscalaPronta;

    return (
        <>
            <NavBar />
            <div className="container">
                <h1>Exibição da escala</h1>
                <div className="table-container">
                    <div className="navigation-buttons">
                        <button onClick={handleAnterior} disabled={indiceInicial === 0}>
                            Anterior
                        </button>
                        <button onClick={handleProximo} disabled={indiceInicial + 5 >= postos.length}>
                            Próximo
                        </button>
                    </div>
                    <DragDropContext onDragEnd={handleDragEnd}>
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Dia</th>
                                    {postos.map((posto) => (
                                        <th key={posto.idPostoTrabalho}>{posto.nomePosto}</th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {Array.from({ length: numDias }, (_, index) => (
                                    <tr key={index + 1}>
                                        <td className="border">{index + 1}</td>
                                        {postosExibicao.map((posto, postoIndex) => {
                                            const diaServico = index + 1;
                                            const funcionariosSet = new Set();
                                            escalaExibicao.forEach((escala) => {
                                                const dataServicoDia = new Date(escala.dataServico).getDate();
                                                if (
                                                    dataServicoDia === diaServico &&
                                                    escala.postoTrabalhoId === posto.idPostoTrabalho
                                                ) {
                                                    funcionariosSet.add(escala.funcionarioId);
                                                }
                                            });

                                            const funcionariosDia = Array.from(funcionariosSet).map((funcionarioId) => {
                                                const funcionario = funcionarios.find(
                                                    (funcionario) => funcionario.idFuncionario === funcionarioId
                                                );
                                                return funcionario ? funcionario.nome : '';
                                            });

                                            return (
                                                <td key={posto.idPostoTrabalho}>
                                                    <Droppable droppableId={posto.idPostoTrabalho.toString()}>
                                                        {(provided) => (
                                                            <div
                                                                ref={provided.innerRef}
                                                                {...provided.droppableProps}
                                                                className="border"
                                                            >
                                                                {funcionariosDia.map((funcionario, funcionarioIndex) => {
                                                                    const draggableId = `${funcionario.idFuncionario}-${posto.idPostoTrabalho.toString()}-${funcionarioIndex}`;
                                                                    return (
                                                                        <Draggable
                                                                            key={draggableId}
                                                                            draggableId={draggableId}
                                                                            index={funcionarioIndex}
                                                                        >
                                                                            {(provided) => (
                                                                                <div
                                                                                    ref={provided.innerRef}
                                                                                    {...provided.draggableProps}
                                                                                    {...provided.dragHandleProps}
                                                                                    className="border p-2"
                                                                                >
                                                                                    {funcionario}
                                                                                </div>
                                                                            )}
                                                                        </Draggable>
                                                                    );
                                                                })}
                                                                {provided.placeholder}
                                                            </div>
                                                        )}
                                                    </Droppable>
                                                </td>
                                            );
                                        })}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </DragDropContext>
                </div>
            </div>
        </>
    );
}

export default Exibicao;