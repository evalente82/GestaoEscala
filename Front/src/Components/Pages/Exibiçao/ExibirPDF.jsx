
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";
import { useParams } from 'react-router-dom';
import './Exibicao.css';
import jsPDF from 'jspdf';

const ExibirPDF = () => {
    const [pdfUrl, setPdfUrl] = useState(null);
    const [escala, setEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [funcionarios, setFuncionarios] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const { idEscala } = useParams('71175b9c-a573-4daf-9f2d-a1cfd582b669');
    const [showEditContent, setShowEditContent] = useState(false);
    const [funcionarioOrigem, setFuncionarioOrigem] = useState('');
    const [funcionarioDestino, setFuncionarioDestino] = useState('');
    const [escalaAlterada, setEscalaAlterada] = useState([]);
    const [highlightedIds, setHighlightedIds] = useState([]); // IDs a serem destacados

    useEffect(() => {
        BuscaEscala('71175b9c-a573-4daf-9f2d-a1cfd582b669');
        BuscaFuncionarios();
        BuscaEscalaPronta('71175b9c-a573-4daf-9f2d-a1cfd582b669');
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

    function obterNomeMes(numeroMes) {
        const meses = [
            "JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO",
            "JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO"
        ];
        
        return meses[numeroMes - 1] || "MÊS INVÁLIDO"; // Retorna o mês ou "MÊS INVÁLIDO" se o número for inválido
    }


    const handleGerarPDF = () => {
        const pdf = new jsPDF('portrait', 'mm', 'a4');
        const margemEsquerda = 10;
        const larguraTotal = 190;
        let yAtual = 20;
    
        // ✅ Cabeçalho
        pdf.setFillColor(0, 40, 120);
        pdf.setDrawColor(0, 0, 0);
        pdf.rect(margemEsquerda, yAtual, larguraTotal, 23, "DF");
    
        pdf.setTextColor(255, 255, 255);
        pdf.setFont("Helvetica", "bold");
        pdf.setFontSize(14);
    
        const titulo = [
            "GRUPAMENTO DE PREVENÇÃO E SALVAMENTO AQUÁTICO",
            "FEVEREIRO 2025",
            "(12H X 60H / 08:00H ÀS 20:00H)"
        ];
    
        titulo.forEach((linha, index) => {
            const textWidth = pdf.getTextWidth(linha);
            pdf.text(linha, margemEsquerda + (larguraTotal - textWidth) / 2, yAtual + 10 + index * 6);
        });
    
        yAtual += 23;
    
        // ✅ "SETOR A"
        pdf.setFillColor(180, 30, 30);
        pdf.setDrawColor(0, 0, 0);
        pdf.rect(margemEsquerda, yAtual, larguraTotal, 15, "DF");
    
        pdf.setTextColor(255, 255, 255);
        pdf.setFontSize(18);
        pdf.setFont("Helvetica", "bold");
    
        const setorTitulo = "SETOR A";
        const textWidthSetor = pdf.getTextWidth(setorTitulo);
        pdf.text(setorTitulo, margemEsquerda + (larguraTotal - textWidthSetor) / 2, yAtual + 10);
    
        yAtual += 15;
    
        // 🔹 Organiza os dados por Posto de Trabalho
        const postosAgrupados = {};
        buscaEscalaPronta.forEach(item => {
            if (!postosAgrupados[item.idPostoTrabalho]) {
                postosAgrupados[item.idPostoTrabalho] = [];
            }
            postosAgrupados[item.idPostoTrabalho].push(item);
        });
    
        Object.keys(postosAgrupados).forEach(idPostoTrabalho => {
            const nomePosto = postos.find(p => p.idPostoTrabalho === idPostoTrabalho)?.nmNome || "Posto Desconhecido";
    
            // ✅ Posto de Trabalho - Bloco Amarelo
            pdf.setFillColor(255, 255, 0);
            pdf.setDrawColor(0, 0, 0);
            pdf.rect(margemEsquerda, yAtual, larguraTotal, 12, "DF");
    
            pdf.setTextColor(0, 0, 0);
            pdf.setFontSize(12);
    
            const postoTitulo = `${nomePosto} – 12X60  08H ÀS 20H`;
            const textWidthPosto = pdf.getTextWidth(postoTitulo);
            pdf.text(postoTitulo, margemEsquerda + (larguraTotal - textWidthPosto) / 2, yAtual + 8);
    
            yAtual += 12;
    
            // 🔹 Agrupando funcionários e dias trabalhados
            const funcionariosAgrupados = {};
            postosAgrupados[idPostoTrabalho].forEach(item => {
                if (!funcionariosAgrupados[item.idFuncionario]) {
                    funcionariosAgrupados[item.idFuncionario] = [];
                }
                funcionariosAgrupados[item.idFuncionario].push(item.dtDataServico);
            });
    
            // 🔹 Remover funcionários com nome "Desconhecido"
            Object.keys(funcionariosAgrupados).forEach(idFuncionario => {
                if (obterNomeFuncionario(idFuncionario) === "Desconhecido") {
                    delete funcionariosAgrupados[idFuncionario];
                }
            });
    
            // 🔹 Criando estrutura correta da tabela
            const gruposLetras = ["A", "B", "C", "D", "E", "F"];
            const larguraColuna1 = 30;
            const larguraColuna2 = 80;
            const larguraColuna3 = 80;
            const alturaLinha = 12;
    
            // 🔹 Criando grupos com base nos dias trabalhados
            const agrupamentoDias = {};
            Object.entries(funcionariosAgrupados).forEach(([idFuncionario, dias]) => {
                const chaveDias = dias
                    .map(data => new Date(data).getDate().toString().padStart(2, '0'))
                    .sort()
                    .join("-");
    
                if (!agrupamentoDias[chaveDias]) {
                    agrupamentoDias[chaveDias] = [];
                }
                agrupamentoDias[chaveDias].push(idFuncionario);
            });
    
            // 🔹 Se não houver funcionários, adiciona SEM FUNCIONÁRIO para cada grupo de dias (A, B, C)
            if (Object.keys(agrupamentoDias).length === 0) {
                gruposLetras.slice(0, 3).forEach(letra => {
                    agrupamentoDias[letra] = ["SEM FUNCIONÁRIO"];
                });
            }
    
            let grupoIndex = 0;
            Object.entries(agrupamentoDias).forEach(([diasTrabalhados, funcionariosNoGrupo]) => {
                // ✅ Coluna 1 (Grupo - A, B, C, etc.)
                pdf.setFillColor(255, 140, 0);
                pdf.setDrawColor(0, 0, 0);
                pdf.rect(margemEsquerda, yAtual, larguraColuna1, alturaLinha, "DF");
    
                pdf.setTextColor(0, 0, 0);
                pdf.setFontSize(14);
                pdf.setFont("Helvetica", "bold");
                pdf.text(gruposLetras[grupoIndex % gruposLetras.length], margemEsquerda + 12, yAtual + 8);
    
                // ✅ Coluna 2 (Nome dos Funcionários)
                pdf.setFillColor(255, 255, 255);
                pdf.rect(margemEsquerda + larguraColuna1, yAtual, larguraColuna2, alturaLinha, "DF");
    
                pdf.setFontSize(12);
                pdf.setFont("Helvetica", "normal");
                const nomesFuncionarios = funcionariosNoGrupo
                    .map(idFuncionario => (idFuncionario === "SEM FUNCIONÁRIO" ? "SEM FUNCIONÁRIO" : obterNomeFuncionario(idFuncionario)))
                    .join("\n");
                pdf.text(nomesFuncionarios, margemEsquerda + larguraColuna1 + 5, yAtual + 6);
    
                // ✅ Coluna 3 (Dias Trabalhados)
                pdf.setFillColor(255, 255, 255);
                pdf.rect(margemEsquerda + larguraColuna1 + larguraColuna2, yAtual, larguraColuna3, alturaLinha, "DF");
    
                pdf.setFontSize(12);
    
                // 🔹 Substitui as letras A, B, C na coluna de dias por "-"
                const diasFormatados = ["A", "B", "C"].includes(diasTrabalhados) ? "-" : diasTrabalhados;
                pdf.text(diasFormatados || "-", margemEsquerda + larguraColuna1 + larguraColuna2 + 10, yAtual + 8);
    
                yAtual += alturaLinha;
                grupoIndex++;
    
                // 🔹 Adiciona nova página se necessário
                if (yAtual > 280) {
                    pdf.addPage();
                    yAtual = 20;
                }
            });
        });
    
        const pdfBlob = pdf.output("blob");
        const pdfUrl = URL.createObjectURL(pdfBlob);
        setPdfUrl(pdfUrl);
    };
    
    
    
    
    
    
    

    

    return (
        <div className="container mt-3">
            <h1>Exibição da Escala</h1>
            
            <button
                type="button"
                className="btn btn-outline-primary me-2"
                onClick={handleGerarPDF}
            >
                Gerar PDF
            </button>

            {pdfUrl && (
                <div className="pdf-container mt-3">
                    <iframe src={pdfUrl} width="100%" height="500px" title="Pré-visualização do PDF"></iframe>
                </div>
            )}
        </div>
    );
};

export default ExibirPDF;
