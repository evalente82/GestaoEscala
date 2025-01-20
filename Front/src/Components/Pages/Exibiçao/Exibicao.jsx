import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";
import { useParams } from 'react-router-dom';

export function Exibicao() {
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const { idEscala } = useParams();
    const [showEditContent, setShowEditContent] = useState(false);

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

    function obterQuantidadeDiasNoMes(ano, mes) {
        const ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
        return ultimoDiaDoMes;
    }

    function BuscaEscala(id) {
        axios
            .get(`https://localhost:7207/escala/buscarPorId/${id}`)
            .then((response) => {
                setEscala(response.data);
                console.log('buscando escala !');
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function BuscaPostos(idDepartamento) {
        axios
            .get(`https://localhost:7207/PostoTrabalho/buscarTodos`)
            .then((response) => {
                const postosFiltrados = response.data.filter(posto => posto.idDepartamento === idDepartamento)
                setPostos(postosFiltrados);
                console.log(postos);
                console.log(response.data);
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

    const numDias = escala ? obterQuantidadeDiasNoMes(2025, escala.nrMesReferencia) : 0;

    return (
        <>
            <NavBar />
            <div className="container">
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
                        <div className="container">
                            <div className="row">
                                <div className="col-4">
                                <input className="form-control mb-2" style={{ width: '100%' }}></input>
                                    <div>
                                    <input className="form-control mb-2" style={{ width: '100%' }}></input>
                                    </div>
                                </div>
                                <div className="col-2">                        
                                </div>
                            </div>
                        </div>
                        
                        <div className="container">
                            <div className="row">
                                <div className="col-1">
                                    <button
                                        type="button"
                                        className="btn btn-outline-primary me-2"
                                    >
                                        Trocar
                                    </button>
                                </div>
                            </div>
                        </div>
                    </>
                )}
                
                <div className="table-container">                
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
                                            {buscaEscalaPronta && buscaEscalaPronta
                                                .filter(item => new Date(item.dtDataServico).getDate() === index + 1 && item.idPostoTrabalho === posto.idPostoTrabalho)
                                                .map(item => (
                                                    <div key={item.idEscalaPronta}>
                                                        {obterNomeFuncionario(item.idFuncionario)}
                                                    </div>
                                                ))
                                            }
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