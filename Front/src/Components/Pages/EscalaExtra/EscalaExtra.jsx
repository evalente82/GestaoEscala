
import React, { useEffect, useState } from "react";
import axios from "axios";
import AlertPopup from '../AlertPopup/AlertPopup';
import PropTypes from 'prop-types';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

// =================================================================================
// Componente da Lista (Com Filtros e Exportação Reintegrados)
// =================================================================================
function EscalaExtraList({ ShowForm }) {
  const [escalaExtra, setEscalaExtra] = useState([]); 
  const [filteredData, setFilteredData] = useState([]); 

  const [searchText, setSearchText] = useState("");
  const [statusFilters, setStatusFilters] = useState({});
  const [setorFilters, setSetorFilters] = useState({});
  const [escalaExtraNomeFilters, setEscalaExtraNomeFilters] = useState({});

  const [alertProps, setAlertProps] = useState({
    show: false,
    type: "info",
    title: "",
    message: "",
    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
  });

  const BuscarTodos = async () => {
    try {
      const response = await axios.get('http://localhost:8080/solicitacaoEscalaExtra/listar');
      setEscalaExtra(response.data);
    } catch (error) {
      setAlertProps({ show: true, type: "error", title: "Erro", message: "Não foi possível carregar os dados." });
    }
  };

  useEffect(() => {
    BuscarTodos();
  }, []);

  useEffect(() => {
    if (escalaExtra.length > 0) {
      const uniqueStatus = [...new Set(escalaExtra.map(item => item.statusInscricao))];
      setStatusFilters(uniqueStatus.reduce((acc, status) => ({ ...acc, [status]: false }), {}));

      const uniqueSetores = [...new Set(escalaExtra.map(item => item.nmSetor))];
      setSetorFilters(uniqueSetores.reduce((acc, setor) => ({ ...acc, [setor]: false }), {}));

      const uniqueNomes = [...new Set(escalaExtra.map(item => item.nmEscalaExtra))];
      setEscalaExtraNomeFilters(uniqueNomes.reduce((acc, nome) => ({ ...acc, [nome]: false }), {}));
    }
  }, [escalaExtra]);

  useEffect(() => {
    let data = [...escalaExtra];
    if (searchText) {
      const searchLower = searchText.toLowerCase();
      data = data.filter(item =>
        item.nmFuncionario?.toLowerCase().includes(searchLower) ||
        item.nmSetor?.toLowerCase().includes(searchLower) ||
        item.nmEscalaExtra?.toLowerCase().includes(searchLower)
      );
    }
    const activeStatus = Object.keys(statusFilters).filter(k => statusFilters[k]);
    if (activeStatus.length > 0) data = data.filter(item => activeStatus.includes(item.statusInscricao));

    const activeSetores = Object.keys(setorFilters).filter(k => setorFilters[k]);
    if (activeSetores.length > 0) data = data.filter(item => activeSetores.includes(item.nmSetor));

    const activeNomes = Object.keys(escalaExtraNomeFilters).filter(k => escalaExtraNomeFilters[k]);
    if (activeNomes.length > 0) data = data.filter(item => activeNomes.includes(item.nmEscalaExtra));
    
    setFilteredData(data);
  }, [searchText, statusFilters, setorFilters, escalaExtraNomeFilters, escalaExtra]);

  const handleFilterChange = (setter, key) => setter(prev => ({ ...prev, [key]: !prev[key] }));

  const formatDate = (dateString) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('pt-BR', {timeZone: 'UTC'}).format(date);
  };

  const formatTime = (dateString) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('pt-BR', { hour: '2-digit', minute: '2-digit', timeZone: 'UTC' }).format(date);
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
    const [/*parteData*/, parteHoraCompleta] = dateString.split('T');
    const fracaoDeSegundos = parteHoraCompleta.substring(parteHoraCompleta.indexOf('.') + 1, parteHoraCompleta.length - 1);
    return `${dia}-${mes}-${ano} ${hora}:${minutos}:${segundos}.${fracaoDeSegundos.substring(0,4)}`;
  };

  function handleDelete(idEscalaExtra) {
    setAlertProps({
      show: true,
      type: "confirm",
      title: "Confirmar exclusão",
      message: "Tem certeza que deseja excluir este registro?",
      onConfirm: () => DeleteEscalaExtra(idEscalaExtra),
    });
  }

  function DeleteEscalaExtra(idEscalaExtra) {
    axios.delete(`http://localhost:8080/solicitacaoEscalaExtra/deletar/${idEscalaExtra}`)
      .then(() => {
        setAlertProps({
          show: true, type: "success", title: "Sucesso", message: "Registro excluído com sucesso!",
          onClose: () => {
            setAlertProps(p => ({ ...p, show: false }));
            BuscarTodos();
          }
        });
      })
      .catch(() => {
        setAlertProps({ show: true, type: "error", title: "Erro", message: "Não foi possível excluir o registro."});
      });
  }

  const handleGerarPDF = () => {
    if (filteredData.length === 0) {
      setAlertProps({ show: true, type: "info", title: "Aviso", message: "Nenhum dado filtrado para gerar o PDF." });
      return;
    }
    
    const pdf = new jsPDF('p', 'pt', 'a4');
    const colunas = ["Nome", "Setor", "Data", "Hora", "Escala Extra", "Status", "Data Cadastro"];
    const dadosOrdenados = [...filteredData].sort((a, b) => a.nmSetor.localeCompare(b.nmSetor));
    const dados = dadosOrdenados.map(item => [
      item.nmFuncionario,
      item.nmSetor,
      formatDate(item.dtEscalaExtra),
      formatTime(item.dtEscalaExtra),
      item.nmEscalaExtra,
      item.statusInscricao,
      formatarDataCustomizada(item.dtCriacao),
    ]);

    autoTable(pdf, {
        head: [colunas],
        body: dados,
        startY: 60,
        headStyles: { fillColor: [0, 40, 120] },
        didDrawPage: function(data) {
            pdf.setFontSize(18);
            pdf.setTextColor(40);
            pdf.text('Listagem de Inscrições em Escalas Extras', data.settings.margin.left, 40);
        }
    });

    pdf.save("inscricoes_escala_extra.pdf");
  };

  const handleGerarCSV = () => {
    if (filteredData.length === 0) {
      setAlertProps({ show: true, type: "info", title: "Aviso", message: "Nenhum dado filtrado para gerar o CSV." });
      return;
    }
    
    const colunas = ["Nome", "Setor", "Data", "Hora", "Escala Extra", "Status", "Data de Inscrição"];
    const escapeCSV = (field) => {
        if (field === null || field === undefined) return "";
        const str = String(field);
        if (str.includes(',') || str.includes('"') || str.includes('\n')) {
            return `"${str.replace(/"/g, '""')}"`;
        }
        return str;
    };
    const dadosCSV = filteredData.map(item => [
        escapeCSV(item.nmFuncionario),
        escapeCSV(item.nmSetor),
        escapeCSV(formatDate(item.dtEscalaExtra)),
        escapeCSV(formatTime(item.dtEscalaExtra)),
        escapeCSV(item.nmEscalaExtra),
        escapeCSV(item.statusInscricao),
        escapeCSV(formatarDataCustomizada(item.dtCriacao))
    ].join(','));
    
    const csvContent = [colunas.join(','), ...dadosCSV].join('\n');
    const blob = new Blob([`\uFEFF${csvContent}`], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", "inscricoes_escala_extra.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <div>
      <h3 className="text-center mb-3">Listagem de Inscrições em Escalas Extras</h3>
      <div className="text-center mb-3">
        <div className="btn-group me-2">
            <button type="button" className="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                Gerar
            </button>
            <ul className="dropdown-menu">
                <li><button className="dropdown-item" type="button" onClick={handleGerarPDF}>PDF</button></li>
                <li><button className="dropdown-item" type="button" onClick={handleGerarCSV}>CSV</button></li>
            </ul>
        </div>
        <button onClick={BuscarTodos} type="button" className="btn btn-outline-primary">
            Atualizar
        </button>
      </div>
      
      <div className="card p-3 mb-3">
        <div className="row">
          <div className="col-12 mb-3">
            <input
              type="text"
              className="form-control"
              placeholder="Pesquisar por nome do funcionário, setor ou escala..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
            />
          </div>
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
      
      <table className="table table-striped table-hover">
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
                <button onClick={() => ShowForm(item)} type="button" className="btn btn-primary btn-sm me-2">Editar</button>
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

// =================================================================================
// Componente do Formulário de Edição
// =================================================================================
function EscalaExtraForm({ escala, ShowList }) {
  const [statusDisponiveis, setStatusDisponiveis] = useState([]);
  const [statusSelecionado, setStatusSelecionado] = useState(escala.statusInscricao || "");
  
  const [alertProps, setAlertProps] = useState({
    show: false,
    type: "info",
    title: "",
    message: "",
    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
  });

  useEffect(() => {
    const buscarStatus = async () => {
      try {
        const response = await axios.get('http://localhost:8080/solicitacaoEscalaExtra/BuscarStatusInscricao');
        setStatusDisponiveis(response.data);
      } catch (error) {
        setAlertProps({ show: true, type: "error", title: "Erro", message: "Não foi possível carregar a lista de status." });
      }
    };
    buscarStatus();
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    const url = `http://localhost:8080/solicitacaoEscalaExtra/AlterarStatusExtra/${escala.idEscalaExtra}?statusInscricao=${statusSelecionado}`;
    axios.put(url, {})
      .then(() => {
        setAlertProps({
          show: true, type: "success", title: "Sucesso", message: "Status atualizado com sucesso!",
          onClose: () => {
            setAlertProps(p => ({ ...p, show: false }));
            ShowList();
          }
        });
      })
      .catch((error) => {
        const errorMessage = error.response?.data?.mensagem || "Falha ao atualizar o status.";
        setAlertProps({ show: true, type: "error", title: "Erro", message: errorMessage });
      });
  };

  return (
    <>
      <h2 className="text-center mb-3">Editar Inscrição</h2>
      <div className="row">
        <div className="col-lg-8 mx-auto">
          <form onSubmit={handleSubmit}>
            <div className="row mb-3">
                <label className="col-sm-4 col-form-label">ID Inscrição</label>
                <div className="col-sm-8"><input readOnly className="form-control-plaintext" value={escala.idEscalaExtra || ''} /></div>
            </div>
            <div className="row mb-3">
                <label className="col-sm-4 col-form-label">Funcionário</label>
                <div className="col-sm-8"><input readOnly className="form-control-plaintext" value={escala.nmFuncionario || ''} /></div>
            </div>
            <div className="row mb-3">
                <label className="col-sm-4 col-form-label">Escala</label>
                <div className="col-sm-8"><input readOnly className="form-control-plaintext" value={escala.nmEscalaExtra || ''} /></div>
            </div>
             <div className="row mb-3">
                <label className="col-sm-4 col-form-label">Setor</label>
                <div className="col-sm-8"><input readOnly className="form-control-plaintext" value={escala.nmSetor || ''} /></div>
            </div>
            <div className="row mb-3">
                <label htmlFor="statusSelect" className="col-sm-4 col-form-label"><strong>Status</strong></label>
                <div className="col-sm-8">
                    <select
                        id="statusSelect"
                        className="form-select"
                        value={statusSelecionado}
                        onChange={(e) => setStatusSelecionado(e.target.value)}
                        required
                    >
                        <option value="" disabled>Selecione um status...</option>
                        {statusDisponiveis.map(status => (
                            <option key={status} value={status}>{status}</option>
                        ))}
                    </select>
                </div>
            </div>
            <div className="row mt-4">
                <div className="offset-sm-4 col-sm-4 d-grid">
                    <button type="submit" className="btn btn-primary">Salvar Alteração</button>
                </div>
                <div className="col-sm-4 d-grid">
                    <button type="button" className="btn btn-secondary" onClick={ShowList}>Cancelar</button>
                </div>
            </div>
          </form>
        </div>
      </div>
      <AlertPopup {...alertProps} />
    </>
  );
}

// =================================================================================
// Componente Principal (Page) que controla a exibição
// =================================================================================
export function EscalaExtraPage() {
  const [content, setContent] = useState('list');
  const [escalaParaEditar, setEscalaParaEditar] = useState(null);

  function ShowList() {
    setContent('list');
    setEscalaParaEditar(null);
  }

  function ShowForm(escala) {
    setEscalaParaEditar(escala);
    setContent('form');
  }

  return (
    <div className="container my-4">
        {content === 'list' && <EscalaExtraList ShowForm={ShowForm} />}
        {content === 'form' && <EscalaExtraForm escala={escalaParaEditar} ShowList={ShowList} />}
    </div>
  );
}

// Definição de propTypes para os componentes
EscalaExtraList.propTypes = {
    ShowForm: PropTypes.func.isRequired,
};

EscalaExtraForm.propTypes = {
    ShowList: PropTypes.func.isRequired,
    escala: PropTypes.object.isRequired,
};

// Exporta o componente principal para ser usado nas rotas
export default EscalaExtraPage;