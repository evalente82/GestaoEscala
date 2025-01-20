import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";
import { useParams } from 'react-router-dom';

export function Exibicao() {
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [indiceInicial, setIndiceInicial] = useState(0);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const { idEscala } = useParams();

    useEffect(() => {
        BuscaEscala(idEscala);
        BuscaPostos();
        BuscaFuncionarios();
        BuscaEscalaPronta(idEscala);
    }, []);

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

    function BuscaPostos() {
        axios
            .get(`https://localhost:7207/PostoTrabalho/buscarTodos`)
            .then((response) => {
                setPostos(response.data);
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
                <h1>Exibição da escala</h1>
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