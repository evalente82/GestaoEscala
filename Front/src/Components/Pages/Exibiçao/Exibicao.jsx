import React, { useEffect, useState } from 'react';
import axios from 'axios';
import NavBar from "../../Menu/NavBar";
import { useParams } from 'react-router-dom';
import './Exibicao.css';
import jsPDF from 'jspdf';
import { useAuth } from "../AuthContext";
import AlertPopup from '../AlertPopup/AlertPopup';
import FormPopup from "../AlertPopup/FormPopup";


export function Exibicao() {
    const [escala, setEscala] = useState(null);
    const [tipoEscala, setTipoEscala] = useState(null);
    const [postos, setPostos] = useState(null);
    const [setores, setSetores] = useState([]);
    const [departamentos, setDepartamentos] = useState([]);
    const [nomeDepartamento, setNomeDepartamento] = useState("Desconhecido");
    const [funcionarios, setFuncionarios] = useState(null);
    const [buscaEscalaPronta, setBuscaEscalaPronta] = useState(null);
    const { idEscala } = useParams();
    const [showEditContent, setShowEditContent] = useState(false);
    const [showIncluirContent, setShowIncluirContent] = useState(false);
    const [showDeleteContent, setShowDeleteContent] = useState(false);
    const [funcionarioOrigem, setFuncionarioOrigem] = useState('');
    const [funcionarioDestino, setFuncionarioDestino] = useState('');
    const [escalaAlterada, setEscalaAlterada] = useState([]);
    const [highlightedIds, setHighlightedIds] = useState([]); // IDs a serem destacados
    const { permissoes } = useAuth();
    const possuiPermissao = (permissao) => permissoes.includes(permissao);
    const [showIncluirPopup, setShowIncluirPopup] = useState(false);
    const numDias = escala ? obterQuantidadeDiasNoMes(2025, escala.nrMesReferencia) : 0;
    
    //console.log("📢 Estado atual do showIncluirPopup:", showIncluirPopup);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onConfirm: null,
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })), 
    });

    const handleCloseAlert = () => {
        setAlertProps(prev => ({
            ...prev,
            show: false,
            onConfirm: null, // Garante que não mantém a ação de confirmação antiga
        }));
    };    

    const [novoFuncionario, setNovoFuncionario] = useState({
        idFuncionario: "",
        idPostoTrabalho: "",
        dtDataServico: ""
    });

    useEffect(() => {
        BuscaEscala(idEscala);
        BuscaFuncionarios();
        BuscaEscalaPronta(idEscala);
        
    }, []);

    useEffect(() => {
        if (escala) {
            BuscaPostos(escala.idDepartamento);
            BuscarTipoEscalaPorId(escala.idTipoEscala)
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

    useEffect(() => {
        BuscaSetores();
    }, []);

    useEffect(() => {
        BuscarDepartamentos(setDepartamentos);
    }, []);

    useEffect(() => {
        if (escala && departamentos.length > 0) {
            const departamentoEncontrado = departamentos.find(dep => dep.idDepartamento === escala.idDepartamento);
            if (departamentoEncontrado) {
                setNomeDepartamento(departamentoEncontrado.nmDepartamento);
            } else {
                console.warn(`⚠️ Departamento não encontrado para idDepartamento: ${escala.idDepartamento}`);
                setNomeDepartamento("Desconhecido");
            }
        }
    }, [escala, departamentos]); // ⚡️ Executa quando escala ou departamentos mudam
    
    useEffect(() => {
        console.log("📢 Estado atual do showIncluirPopup:", showIncluirPopup);
        console.log("📢 Funcionários disponíveis:", funcionarios);
        console.log("📢 Postos disponíveis:", postos);
    }, [showIncluirPopup]);    
    
    function BuscaSetores() {
        axios
            .get("https://localhost:7207/setor/buscarTodos")
            .then((response) => {
                setSetores(response.data);
                console.log("Setores carregados:", response.data);
            })
            .catch((error) => {
                console.error("Erro ao buscar setores:", error);
            });
    }

    function obterQuantidadeDiasNoMes(ano, mes) {
        const ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
        return ultimoDiaDoMes;
    }

    function BuscarTipoEscalaPorId(idTipoEscala) {
        axios.get(`https://localhost:7207/tipoEscala/buscarPorId/${idTipoEscala}`)
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

    function BuscarDepartamentos() {
        const fetchData = async () => {
            try {
                const response = await axios.get("https://localhost:7207/departamento/buscarTodos");
                setDepartamentos(response.data);
                console.log('Departamentos');
                console.log(response.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchData();
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
            setAlertProps({
                show: true,
                type: "info",
                title: "Aviso",
                message: "Não há dados para salvar!",
                onClose: () => setAlertProps((prev) => ({ ...prev, show: false }))
            });
            return;
        }
    
        try {
            const response = await axios.put("https://localhost:7207/escala/SalvarEscalaAlterada", escalaAlterada);
    
            if (response.status === 200) {
                // 🔹 Atualiza os dados antes de exibir o alerta
                await BuscaEscalaPronta(idEscala);
    
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso!",
                    message: "Escala salva com sucesso!",
                    onClose: () => {
                        setAlertProps((prev) => ({ ...prev, show: false }));
                    }
                });
            } else {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Erro ao salvar a escala.",
                });
            }
        } catch (error) {
            console.error("Erro ao salvar a escala:", error);
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Erro ao salvar a escala.",
            });
        }
    };

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
    
        const departamentoDaEscala = () => {
            if (!escala || !escala.idDepartamento) {
                console.warn("⚠️ escala.idDepartamento não está definido!");
                return 'Desconhecido';
            }
        
            if (!departamentos || departamentos.length === 0) {
                console.warn("⚠️ Lista de departamentos vazia ou não carregada!");
                return 'Desconhecido';
            }
        
            const departamento = departamentos.find(dep => dep.idDepartamento === escala.idDepartamento);
        
            if (!departamento) {
                console.warn(`⚠️ Departamento não encontrado para idDepartamento: ${escala.idDepartamento}`);
                return 'Desconhecido';
            }
        
            return departamento.nmDescricao;
        };
        
        // ✅ Testando a função
        console.log("🏢 Nome do Departamento:", departamentoDaEscala());
        
        const nomeDepartamento = departamentoDaEscala();
        const titulo = [
            `${nomeDepartamento}`,
            obterNomeMes(escala.nrMesReferencia) + " - " + buscaEscalaPronta[0].dtDataServico.substring(0, 4),
            `${tipoEscala.nrHorasTrabalhada} x ${tipoEscala.nrHorasFolga} Horário ${tipoEscala.nmDescricao}`
        ];
    
        titulo.forEach((linha, index) => {
            const textWidth = pdf.getTextWidth(linha);
            pdf.text(linha, margemEsquerda + (larguraTotal - textWidth) / 2, yAtual + 10 + index * 6);
        });
    
        yAtual += 23;
    
        // 🔹 Organiza os dados por Setor e Posto de Trabalho
        const setoresAgrupados = {};
        buscaEscalaPronta.forEach(item => {
            const idSetor = postos.find(p => p.idPostoTrabalho === item.idPostoTrabalho)?.idSetor || "SetorDesconhecido";
    
            if (!setoresAgrupados[idSetor]) {
                setoresAgrupados[idSetor] = {};
            }
            if (!setoresAgrupados[idSetor][item.idPostoTrabalho]) {
                setoresAgrupados[idSetor][item.idPostoTrabalho] = [];
            }
            setoresAgrupados[idSetor][item.idPostoTrabalho].push(item);
        });
    
        // 🔹 Ordena os setores em ordem alfabética antes de iterar
        const setoresOrdenados = Object.keys(setoresAgrupados)
            .map(idSetor => ({
                idSetor,
                nomeSetor: setores.find(s => s.idSetor === idSetor)?.nmNome || "Setor Desconhecido"
            }))
            .sort((a, b) => a.nomeSetor.localeCompare(b.nomeSetor));
    
        // 🔹 Iterando sobre cada setor e seus postos sem alterar a lógica de exibição dos funcionários e dias
        setoresOrdenados.forEach(({ idSetor, nomeSetor }) => {
            // ✅ Adiciona o Setor no PDF
            pdf.setFillColor(180, 30, 30);
            pdf.setDrawColor(0, 0, 0);
            pdf.rect(margemEsquerda, yAtual, larguraTotal, 15, "DF");
    
            pdf.setTextColor(255, 255, 255);
            pdf.setFontSize(18);
            pdf.setFont("Helvetica", "bold");
    
            const textWidthSetor = pdf.getTextWidth(nomeSetor);
            pdf.text(nomeSetor, margemEsquerda + (larguraTotal - textWidthSetor) / 2, yAtual + 10);
    
            yAtual += 15;
    
            // 🔹 Mantendo a estrutura original dos postos e dias trabalhados
            Object.keys(setoresAgrupados[idSetor]).forEach(idPostoTrabalho => {
                const nomePosto = postos.find(p => p.idPostoTrabalho === idPostoTrabalho)?.nmNome || "Posto Desconhecido";
    
                // ✅ Posto de Trabalho - Bloco Amarelo
                pdf.setFillColor(255, 255, 0);
                pdf.setDrawColor(0, 0, 0);
                pdf.rect(margemEsquerda, yAtual, larguraTotal, 12, "DF");
    
                pdf.setTextColor(0, 0, 0);
                pdf.setFontSize(12);
    
                const postoTitulo = `${nomePosto} `;
                const textWidthPosto = pdf.getTextWidth(postoTitulo);
                pdf.text(postoTitulo, margemEsquerda + (larguraTotal - textWidthPosto) / 2, yAtual + 8);
    
                yAtual += 12;
    
                // 🔹 Mantendo a estrutura correta de agrupamento de funcionários e dias trabalhados
                const funcionariosAgrupados = {};
                setoresAgrupados[idSetor][idPostoTrabalho].forEach(item => {
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
                const alturaLinhaBase = 12;
    
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
    
                // 🔹 Se não houver funcionários, adiciona "-" na coluna de DIAS
                if (Object.keys(agrupamentoDias).length === 0) {
                    agrupamentoDias["-"] = ["SEM FUNCIONÁRIO"];
                }
    
                let grupoIndex = 0;
                Object.entries(agrupamentoDias).forEach(([diasTrabalhados, funcionariosNoGrupo]) => {
                    // ✅ Mantendo os dias trabalhados dentro da borda
                    const diasFormatados = funcionariosNoGrupo.includes("SEM FUNCIONÁRIO") ? "-" : diasTrabalhados;
                    const linhasDias = pdf.splitTextToSize(diasFormatados, larguraColuna3 - 8);
                    let alturaExtra = (linhasDias.length - 1) * 6;
    
                    // ✅ Coluna 1 (Grupo)
                    pdf.setFillColor(255, 140, 0);
                    pdf.setDrawColor(0, 0, 0);
                    pdf.rect(margemEsquerda, yAtual, larguraColuna1, alturaLinhaBase + alturaExtra, "DF");
    
                    pdf.setTextColor(0, 0, 0);
                    pdf.setFontSize(14);
                    pdf.setFont("Helvetica", "bold");
                    pdf.text(gruposLetras[grupoIndex % gruposLetras.length], margemEsquerda + 12, yAtual + 8);
    
                    // ✅ Coluna 2 (Nome dos Funcionários)
                    pdf.setFillColor(255, 255, 255);
                    pdf.rect(margemEsquerda + larguraColuna1, yAtual, larguraColuna2, alturaLinhaBase + alturaExtra, "DF");
    
                    pdf.setFontSize(12);
                    pdf.setFont("Helvetica", "normal");
                    const nomesFuncionarios = funcionariosNoGrupo
                        .map(idFuncionario => (idFuncionario === "SEM FUNCIONÁRIO" ? "SEM FUNCIONÁRIO" : obterNomeFuncionario(idFuncionario)))
                        .join("\n");
                    pdf.text(nomesFuncionarios, margemEsquerda + larguraColuna1 + 5, yAtual + 6);
    
                    // ✅ Coluna 3 (Dias Trabalhados)
                    pdf.setFillColor(255, 255, 255);
                    pdf.rect(margemEsquerda + larguraColuna1 + larguraColuna2, yAtual, larguraColuna3, alturaLinhaBase + alturaExtra, "DF");
    
                    pdf.setFontSize(12);
                    linhasDias.forEach((linha, index) => {
                        pdf.text(linha, margemEsquerda + larguraColuna1 + larguraColuna2 + 5, yAtual + 8 + (index * 6));
                    });
    
                    yAtual += alturaLinhaBase + alturaExtra;
                    grupoIndex++;
    
                    if (yAtual > 280) {
                        pdf.addPage();
                        yAtual = 20;
                    }
                });
    
                yAtual += 0;
            });
        });
    
        pdf.save("Escala_Praia_Modelo.pdf");
    };
    
    const handleAbrirIncluirFuncionario = (idPostoTrabalho, dia) => {
        if (!buscaEscalaPronta || buscaEscalaPronta.length === 0) {
            //console.warn("⚠️ Nenhuma escala pronta encontrada para determinar o ano e o mês!");
            return;
        }
    
        // 🔹 Obtém o ano e o mês da primeira ocorrência da escala pronta
        const primeiraOcorrencia = buscaEscalaPronta[0];
        const ano = new Date(primeiraOcorrencia.dtDataServico).getFullYear();
        const mes = String(new Date(primeiraOcorrencia.dtDataServico).getMonth() + 1).padStart(2, "0"); // Ajusta para dois dígitos
    
        //console.log(`🟢 Abrindo popup de inclusão para posto: ${idPostoTrabalho}, dia: ${dia}, mês: ${mes}, ano: ${ano}`);
    
        setNovoFuncionario({
            idFuncionario: "", // Começa vazio para o usuário escolher
            idPostoTrabalho, // Já vem pré-preenchido
            dtDataServico: `${ano}-${mes}-${String(dia).padStart(2, "0")}` // Data formatada corretamente
        });
    
        setShowIncluirPopup(true); // ✅ Atualiza o estado corretamente
    };    
    
    const handleConfirmarInclusao = async () => {
        if (!novoFuncionario.idFuncionario || !novoFuncionario.idPostoTrabalho) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Todos os campos são obrigatórios!"
            });
            return;
        }
    
        // 🔹 Converte a data para o formato "YYYY-MM-DD"
        const dataFormatada = new Date(novoFuncionario.dtDataServico)
            .toISOString()
            .split('T')[0];
    
        const dadosInclusao = {
            idEscala,
            idPostoTrabalho: novoFuncionario.idPostoTrabalho,
            idFuncionario: novoFuncionario.idFuncionario,
            dtDataServico: dataFormatada
        };
    
        try {
            const response = await axios.post("https://localhost:7207/escalaPronta/IncluirFuncionario", dadosInclusao);
            if (response.status === 201) {
                setShowIncluirPopup(false); // Fecha o popup primeiro
                
                // 🔹 Aguarda a atualização da escala antes de fechar o alerta
                await BuscaEscalaPronta(idEscala);
                
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso!",
                    message: "Funcionário adicionado com sucesso!",
                    onClose: () => {
                        setAlertProps((prev) => ({ ...prev, show: false }));
                    }
                });
            } else {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Erro ao adicionar funcionário."
                });
            }
        } catch (error) {
            console.error("Erro ao adicionar funcionário:", error);
            
            const mensagemErro = error.response?.data?.mensagem || "Erro desconhecido.";
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: mensagemErro
            });
        }
    };

    const handleRemoverFuncionario = async (item) => {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmação",
            message: `Tem certeza que deseja remover ${obterNomeFuncionario(item.idFuncionario)}?`,
            onConfirm: async () => {
                const payload = {
                    idFuncionario: item.idFuncionario,
                    idEscala
                };
    
                try {
                    const response = await axios.delete("https://localhost:7207/escalaPronta/DeletarOcorrenciaFuncionario", { data: payload });
    
                    if (response.status === 200) {
                        // 🔹 Atualiza a escala ANTES de fechar a modal de sucesso
                        await BuscaEscalaPronta(idEscala);
    
                        setAlertProps({
                            show: true,
                            type: "success",
                            title: "Sucesso!",
                            message: "Funcionário removido com sucesso!",
                            onClose: () => {
                                setAlertProps((prev) => ({ ...prev, show: false }));
                            }
                        });
                    } else {
                        setAlertProps({
                            show: true,
                            type: "error",
                            title: "Erro",
                            message: "Erro ao remover funcionário.",
                        });
                    }
                } catch (error) {
                    console.error("Erro ao remover funcionário:", error);
                    setAlertProps({
                        show: true,
                        type: "error",
                        title: "Erro",
                        message: "Erro ao remover funcionário.",
                    });
                }
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    };
    
    return (
        <>
            <div className="container mt-3">
                <div className="text-center mb-3">
                <h1 >Exibição da Escala {escala ? escala.nmNomeEscala : 'Carregando...'}</h1>
                {possuiPermissao("EditarEscalas") && (
                <button
                    type="button"
                    className="btn btn-outline-primary me-2"
                    onClick={() => setShowEditContent(!showEditContent)}
                >
                    EDITAR
                </button>)}
                </div>
                
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
                                    <div className="col-5">
                                        <button
                                            type="button"
                                            className="btn btn-outline-primary me-2"
                                            onClick={handleGerarPDF}
                                        >
                                            Gerar PDF
                                        </button>
                                    </div>   
                                </div>
                            </div>
                        </div>
                    </>
                )}      

                {showIncluirPopup && (
                    <FormPopup
                        title="Adicionar Funcionário"
                        show={showIncluirPopup}
                        onConfirm={handleConfirmarInclusao}
                        onClose={() => setShowIncluirPopup(false)}
                    >
                        <div>
                            <label>Funcionário:</label>
                            <select
                                className="form-control"
                                value={novoFuncionario.idFuncionario}
                                onChange={(e) => setNovoFuncionario(prev => ({ ...prev, idFuncionario: e.target.value }))}
                            >
                                <option value="">Selecione...</option>
                                {funcionarios?.length > 0 ? (
                                    funcionarios
                                        .filter(func => func.idCargo === escala?.idCargo)
                                        .map(func => (
                                            <option key={func.idFuncionario} value={func.idFuncionario}>
                                                {func.nmNome}
                                            </option>
                                        ))
                                ) : (
                                    <option disabled>Carregando funcionários...</option>
                                )}
                            </select>
                        </div>

                        <div className="mt-2">
                            <label>Posto de Trabalho:</label>
                            <select
                                className="form-control"
                                value={novoFuncionario.idPostoTrabalho}
                                onChange={(e) => setNovoFuncionario(prev => ({ ...prev, idPostoTrabalho: e.target.value }))} 
                            >
                                <option value="">Selecione...</option>
                                {postos?.length > 0 ? (
                                    postos.map(posto => (
                                        <option key={posto.idPostoTrabalho} value={posto.idPostoTrabalho}>
                                            {posto.nmNome}
                                        </option>
                                    ))
                                ) : (
                                    <option disabled>Carregando postos...</option>
                                )}
                            </select>
                        </div>

                        <div className="mt-2">
                            <label>Data do Serviço:</label>
                            <input
                                type="text"
                                className="form-control"
                                value={novoFuncionario.dtDataServico}
                                disabled
                            />
                        </div>
                    </FormPopup>
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
            {postos && postos.map((posto) => {
                const funcionariosNoPosto = escalaAlterada.filter(item =>
                    new Date(item.dtDataServico).getDate() === index + 1 &&
                    item.idPostoTrabalho === posto.idPostoTrabalho
                );

                return (
                    <td key={posto.idPostoTrabalho} className="position-relative">
                        {funcionariosNoPosto.length > 0 ? (
                            funcionariosNoPosto.map(item => {
                                const isFuncionarioDesconhecido = item.idFuncionario === "00000000-0000-0000-0000-000000000000" ||
                                obterNomeFuncionario(item.idFuncionario) === "Desconhecido";
                                    return (
                                        <div key={item.idEscalaPronta} className="d-flex justify-content-start align-items-center">
                                            {possuiPermissao("EditarEscalas") && (
                                                <div className="btn-container">
                                                    {isFuncionarioDesconhecido ? (
                                                        <button
                                                            className="btn btn-xs btn-outline-success small-btn"
                                                            onClick={() => handleAbrirIncluirFuncionario(posto.idPostoTrabalho, index + 1)}
                                                            title="Adicionar funcionário"
                                                        >
                                                            ➕
                                                        </button>
                                                    ) : (
                                                        <button
                                                            className="btn btn-xs btn-outline-danger small-btn"
                                                            onClick={() => handleRemoverFuncionario(item)}
                                                            title="Remover funcionário"
                                                        >
                                                            ❌
                                                        </button>
                                                    )}
                                                </div>
                                            )}
                                            <span className={`nome-funcionario ${highlightedIds.includes(item.idFuncionario) ? 'highlight' : ''}`}>
                                                {isFuncionarioDesconhecido ? "Desconhecido" : obterNomeFuncionario(item.idFuncionario)}
                                            </span>
                                        </div>
                                    );
                            })
                        ) : (
                            <div className="d-flex justify-content-start align-items-center">
                                {possuiPermissao("EditarEscalas") && (
                                    <div className="btn-container">
                                        <button
                                            className="btn btn-xs btn-outline-success small-btn"
                                            onClick={() => handleAbrirIncluirFuncionario(posto.idPostoTrabalho, index + 1)}
                                            title="Adicionar funcionário"
                                            >
                                            ➕
                                        </button>
                                    </div>
                                )}
                                <span className="text-muted">Desconhecido</span>
                            </div>
                        )}
                    </td>
                );
            })}
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
                        onClose={handleCloseAlert} // ✅ Agora está chamando a função correta
                    />                                  
                </div>
            </div>
        </>
    );
}

export default Exibicao;