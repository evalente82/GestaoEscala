import React, { useEffect, useState } from "react";
import axios from "axios";
import AlertPopup from '../AlertPopup/AlertPopup';


function EscalaExtraList() {
  const [escalaExtra, setEscalaExtra] = useState([]); // Dados originais da API
  const [filteredData, setFilteredData] = useState([]); // Dados que serão exibidos na tela

  // --- Novos estados para os filtros ---
  const [searchText, setSearchText] = useState("");
  const [statusFilters, setStatusFilters] = useState({
    Confirmado: false,
    FilaDeEspera: false,
  });
  const [setorFilters, setSetorFilters] = useState({});
  const [escalaExtraNomeFilters, setEscalaExtraNomeFilters] = useState({});

  const [alertProps, setAlertProps] = useState({
    show: false,
    type: "info",
    title: "",
    message: "",
    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
  });

  // Função para buscar os dados da API
  const BuscarTodos = async () => {
    try {
      const response = await axios.get('http://localhost:8080/solicitacaoEscalaExtra/listar');
      setEscalaExtra(response.data); // Armazena os dados originais
    } catch (error) {
      setAlertProps({
        show: true,
        type: "error",
        title: "Erro",
        message: "Não foi possível carregar os dados da Escala Extra.",
      });
    }
  };

  useEffect(() => {
    BuscarTodos();
  }, []);

  // --- Novo useEffect para inicializar os filtros dinâmicos ---
  useEffect(() => {
    if (escalaExtra.length > 0) {
      // Cria os filtros de setor dinamicamente
      const uniqueSetores = [...new Set(escalaExtra.map(item => item.nmSetor))];
      const setorInitialState = uniqueSetores.reduce((acc, setor) => ({ ...acc, [setor]: false }), {});
      setSetorFilters(setorInitialState);

      // Cria os filtros de nome de escala extra dinamicamente
      const uniqueNomes = [...new Set(escalaExtra.map(item => item.nmEscalaExtra))];
      const escalaExtraInitialState = uniqueNomes.reduce((acc, nome) => ({ ...acc, [nome]: false }), {});
      setEscalaExtraNomeFilters(escalaExtraInitialState);
    }
  }, [escalaExtra]); // Roda sempre que os dados originais mudarem

  // --- Novo useEffect para aplicar os filtros ---
  useEffect(() => {
    let data = [...escalaExtra];

    // 1. Filtrar por texto de pesquisa
    if (searchText) {
      const searchLower = searchText.toLowerCase();
      data = data.filter(item =>
        item.nmFuncionario?.toLowerCase().includes(searchLower) ||
        item.nmSetor?.toLowerCase().includes(searchLower) ||
        item.nmEscalaExtra?.toLowerCase().includes(searchLower)
      );
    }

    // 2. Filtrar por Checkboxes de Status
    const activeStatusFilters = Object.keys(statusFilters).filter(key => statusFilters[key]);
    if (activeStatusFilters.length > 0) {
      data = data.filter(item => activeStatusFilters.includes(item.statusInscricao));
    }

    // 3. Filtrar por Checkboxes de Setor
    const activeSetorFilters = Object.keys(setorFilters).filter(key => setorFilters[key]);
    if (activeSetorFilters.length > 0) {
      data = data.filter(item => activeSetorFilters.includes(item.nmSetor));
    }

    // 4. Filtrar por Checkboxes de Nome da Escala
    const activeEscalaExtraFilters = Object.keys(escalaExtraNomeFilters).filter(key => escalaExtraNomeFilters[key]);
    if (activeEscalaExtraFilters.length > 0) {
      data = data.filter(item => activeEscalaExtraFilters.includes(item.nmEscalaExtra));
    }
    
    setFilteredData(data);
  }, [searchText, statusFilters, setorFilters, escalaExtraNomeFilters, escalaExtra]);


  // Função para manipular a mudança nos checkboxes
  const handleFilterChange = (filterStateSetter, filterKey) => {
    filterStateSetter(prev => ({ ...prev, [filterKey]: !prev[filterKey] }));
  };

  // Funções de formatação e delete (sem alterações)
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  };

  const formatTime = (dateString) => {
    const date = new Date(dateString);
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${hours}:${minutes}`;
  };

  const formatarDataCustomizada = (dateString) => {
    if (!dateString || !dateString.includes('T') || !dateString.endsWith('Z')) return ""; 
    const dataUTC = new Date(dateString);
    const dataAjustada = new Date(dataUTC.getTime() - (3 * 60 * 60 * 1000));
    const dia = String(dataAjustada.getUTCDate()).padStart(2, '0');
    const mes = String(dataAjustada.getUTCMonth() + 1).padStart(2, '0');
    const ano = dataAjustada.getUTCFullYear();
    const hora = String(dataAjustada.getUTCHours()).padStart(2, '0');
    const minutos = String(dataAjustada.getUTCMinutes()).padStart(2, '0');
    const segundos = String(dataAjustada.getUTCSeconds()).padStart(2, '0');
    const [parteData, parteHoraCompleta] = dateString.split('T');
    const fracaoDeSegundos = parteHoraCompleta.substring(parteHoraCompleta.indexOf('.') + 1, parteHoraCompleta.length - 1);
    return `${dia}-${mes}-${ano} H:${hora}:${minutos}:${segundos}.${fracaoDeSegundos}Z`;
  };

    function handleDelete(idEscalaExtra) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeleteEscalaExtra(idEscalaExtra);
            },
        });
    }

    function DeleteEscalaExtra(idEscalaExtra) {
        axios.delete(`http://localhost:8080/solicitacaoEscalaExtra/deletar/${idEscalaExtra}`)
            .then(() => {
                setAlertProps({
                    show: true, type: "success", title: "Sucesso",
                    message: "Registro excluído com sucesso!",
                    onClose: () => {
                        setAlertProps(prev => ({ ...prev, show: false }));
                        BuscarTodos(); // Recarrega os dados após a exclusão
                    }
                });
            })
            .catch(() => {
                setAlertProps({ show: true, type: "error", title: "Erro", message: "Não foi possível excluir o registro."});
            });
    }
    
  const handleGerarPDF = () => {
    const pdf = new jsPDF('p', 'pt', 'a4');
    const colunas = ["Nome", "Setor", "Data", "Hora", "Escala Extra", "Status"];
    const dados = filteredData.map(item => [
      item.nmFuncionario,
      item.nmSetor,
      formatDate(item.dtEscalaExtra),
      formatTime(item.dtEscalaExtra),
      item.nmEscalaExtra,
      item.statusInscricao
    ]);

    pdf.autoTable({
        head: [colunas],
        body: dados,
        startY: 60,
        headStyles: { fillColor: [0, 40, 120] },
        didDrawPage: function(data) {
            pdf.setFontSize(18);
            pdf.setTextColor(40);
            pdf.text('Listagem de Escalas Extras', data.settings.margin.left, 40);
        }
    });

    pdf.save("Escala_Extra.pdf");
  };

  return (
    <div className="container">
      <h3 className="text-center mb-3">Listagem de Escalas Extras</h3>
      <div className="text-center mb-3">
        <button onClick={handleGerarPDF} type="button" className="btn btn-primary me-2">Gerar PDF</button>
        <button onClick={BuscarTodos} type="button" className="btn btn-outline-primary me-2">Atualizar</button>
      </div>
      
      {/* --- NOVA ÁREA DE FILTROS --- */}
      <div className="card p-3 mb-3">
        <h5>Filtros</h5>
        <div className="row">
          {/* Filtro de Pesquisa */}
          <div className="col-12 mb-3">
            <input
              type="text"
              className="form-control"
              placeholder="Pesquisar por nome do funcionário, setor ou escala..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
            />
          </div>

          {/* Filtros de Checkbox */}
          <div className="col-12">
            <div className="d-flex flex-column align-items-start">
              <div>
                <strong>Status:</strong>
                {Object.keys(statusFilters).map(key => (
                  <div className="form-check form-check-inline ms-2" key={key}>
                    <input className="form-check-input" type="checkbox" id={`status-${key}`} checked={statusFilters[key]} onChange={() => handleFilterChange(setStatusFilters, key)} />
                    <label className="form-check-label" htmlFor={`status-${key}`}>{key}</label>
                  </div>
                ))}
              </div>
              <div>
                <strong>Setor:</strong>
                {Object.keys(setorFilters).map(key => (
                  <div className="form-check form-check-inline ms-2" key={key}>
                    <input className="form-check-input" type="checkbox" id={`setor-${key}`} checked={setorFilters[key]} onChange={() => handleFilterChange(setSetorFilters, key)} />
                    <label className="form-check-label" htmlFor={`setor-${key}`}>{key}</label>
                  </div>
                ))}
              </div>
              <div>
                <strong>Escala Extra:</strong>
                {Object.keys(escalaExtraNomeFilters).map(key => (
                  <div className="form-check form-check-inline ms-2" key={key}>
                    <input className="form-check-input" type="checkbox" id={`escala-${key}`} checked={escalaExtraNomeFilters[key]} onChange={() => handleFilterChange(setEscalaExtraNomeFilters, key)} />
                    <label className="form-check-label" htmlFor={`escala-${key}`}>{key}</label>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <table className="table">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Setor</th>
            <th>Data</th>
            <th>Hora</th>
            <th>Escala Extra</th>
            <th>Status</th>
            <th>Data Cadastro</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {/* Mapeia os dados JÁ FILTRADOS */}
          {filteredData.map((item) => (
            <tr key={item.idEscalaExtra}>
              <td>{item.nmFuncionario}</td>
              <td>{item.nmSetor}</td>
              <td>{formatDate(item.dtEscalaExtra)}</td>
              <td>{formatTime(item.dtEscalaExtra)}</td>
              <td>{item.nmEscalaExtra}</td>
              <td>{item.statusInscricao}</td>
              <td>{formatarDataCustomizada(item.dtCriacao)}</td>
              <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                <button onClick={() => handleDelete(item.idEscalaExtra)} type="button" className="btn btn-danger btn-sm">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <AlertPopup {...alertProps} />
    </div>
  );
}

export default EscalaExtraList;