import React, { useEffect, useState } from "react";
import axios from "axios";
import AlertPopup from '../AlertPopup/AlertPopup';
import { jsPDF } from 'jspdf'; // Importe jsPDF

function EscalaExtraList() {
  const [escalaExtra, setEscalaExtra] = useState([]);
    const [alertProps, setAlertProps] = useState({
        show: false,
        type: "info",
        title: "",
        message: "",
        onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
    });

    function BuscarTodos(){
        const fetchData = async () => {
      try {
        const response = await axios.get('http://localhost:8080/solicitacaoEscalaExtra/listar');
        console.log(response.data);
        setEscalaExtra(response.data);
      } catch (error) {
            setAlertProps({
                show: true,
                type: "error",
                title: "Erro",
                message: "Não foi possível carregar os dados da Escala Extra.",
            });
      }
    };
    fetchData();
    }

  useEffect(() => {
    BuscarTodos();
    
  }, []);

    function handleDelete(idEscalaExtra) {
        setAlertProps({
            show: true,
            type: "confirm",
            title: "Confirmar exclusão",
            message: "Tem certeza que deseja excluir este registro?",
            onConfirm: () => {
                DeleteEscalaExtra(idEscalaExtra);
                setAlertProps((prev) => ({ ...prev, show: false }));
            },
            onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
        });
    }

    function DeleteEscalaExtra(idEscalaExtra) {
        axios
            .delete(`http://localhost:8080/solicitacaoEscalaExtra/deletar/${idEscalaExtra}`) // Replace with your actual delete endpoint
            .then(() => {
                setEscalaExtra(escalaExtra.filter((escala) => escala.idEscalaExtra !== idEscalaExtra));
                setAlertProps({
                    show: true,
                    type: "success",
                    title: "Sucesso",
                    message: "Registro excluído com sucesso!",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            })
            .catch(() => {
                setAlertProps({
                    show: true,
                    type: "error",
                    title: "Erro",
                    message: "Não foi possível excluir o registro.",
                    onClose: () => setAlertProps((prev) => ({ ...prev, show: false })),
                });
            });
    }

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  };

  const formatTime = (dateString) => {
      const date = new Date(dateString);
      const hours = String(date.getHours()).padStart(2, '0');
      const minutes = String(date.getMinutes()).padStart(2, '0');
      return `${hours}:${minutes}`;
  };

  const handleGerarPDF = () => {
      const pdf = new jsPDF('portrait', 'mm', 'a4');
      const margemEsquerda = 10;
      const larguraTotal = 190;
      let yAtual = 20;

      // ✅ Cabeçalho Principal
      pdf.setFillColor(0, 40, 120);
      pdf.setDrawColor(0, 0, 0);
      pdf.rect(margemEsquerda, yAtual, larguraTotal, 23, "DF");

      pdf.setTextColor(255, 255, 255);
      pdf.setFont("Helvetica", "bold");
      pdf.setFontSize(14);

      const titulo = [
            `Listagem de Escalas Extras`,
            `${new Date().toLocaleDateString()}`,
      ];

      titulo.forEach((linha, index) => {
            const textWidth = pdf.getTextWidth(linha);
            pdf.text(linha, margemEsquerda + (larguraTotal - textWidth) / 2, yAtual + 10 + index * 6);
        });

      yAtual += 30;

      // ✅ Agrupar por Setor
      const escalasPorSetor = {};
      escalaExtra.forEach(escala => {
          if (!escalasPorSetor[escala.nmSetor]) {
              escalasPorSetor[escala.nmSetor] = [];
          }
          escalasPorSetor[escala.nmSetor].push(escala);
      });

      // ✅ Iterar por Setor e Adicionar ao PDF
      for (const setor in escalasPorSetor) {
          if (escalasPorSetor.hasOwnProperty(setor)) {
              // ✅ Cabeçalho do Setor (Estilo Bloco Vermelho)
              pdf.setFillColor(180, 30, 30);
              pdf.setDrawColor(0, 0, 0);
              pdf.rect(margemEsquerda, yAtual, larguraTotal, 15, "DF");

              pdf.setTextColor(255, 255, 255);
              pdf.setFontSize(18);
              pdf.setFont("Helvetica", "bold");

              const textWidthSetor = pdf.getTextWidth(setor);
              pdf.text(setor, margemEsquerda + (larguraTotal - textWidthSetor) / 2, yAtual + 10);

              yAtual += 15 + 5; // Espaço após o título do setor

              // ✅ Listagem de Escalas Extras do Setor
              pdf.setTextColor(0, 0, 0);
              pdf.setFontSize(10);
              escalasPorSetor[setor].forEach(item => {
                const linhaTexto = `${item.nmFuncionario} - ${formatDate(item.dtEscalaExtra)} - ${formatTime(item.dtEscalaExtra)} - ${item.nmEscalaExtra}`;
                const textoDividido = pdf.splitTextToSize(linhaTexto, larguraTotal - 20);
                textoDividido.forEach(linha => {
                    pdf.text(linha, margemEsquerda, yAtual);
                    yAtual += 5;
                });
                yAtual += 5; // Espaço entre os registros
                if (yAtual > pdf.internal.pageSize.getHeight() - margemEsquerda - 10) {
                    pdf.addPage();
                    yAtual = 20;
              }
            });
            yAtual += 10; // Espaço entre os setores

            if (yAtual > pdf.internal.pageSize.getHeight() - margemEsquerda - 10) {
                pdf.addPage();
                yAtual = 20;
          }
        }
    }
    pdf.save("Escala_Extra.pdf"); // Movido para fora do loop
  };

  return (
    <div className="container">
      <h3 className="text-center mb-3">Listagem de Escalas Extras</h3>
      <div className="text-center mb-3">
                <button
                    onClick={handleGerarPDF}
                    type="button"
                    className="btn btn-primary me-2"
                >
                    Gerar PDF
                </button>
                <button
                    onClick={() => BuscarTodos()}
                    type="button"
                    className="btn btn-outline-primary me-2"
                >
                    Atualizar
                </button>
            </div>
      <table className="table">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Setor</th>
            <th>Data</th>
            <th>Hora</th>
            <th>Escala_Extra</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {escalaExtra.map((item) => (
            <tr key={item.idEscalaExtra}>
              <td>{item.nmFuncionario}</td>
              <td>{item.nmSetor}</td>
              <td>{formatDate(item.dtEscalaExtra)}</td>
              <td>{formatTime(item.dtEscalaExtra)}</td>
              <td>{item.nmEscalaExtra}</td>
              <td style={{ width: "10px", whiteSpace: "nowrap" }}>
                    <button
                        onClick={() => handleDelete(item.idEscalaExtra)}
                        type="button"
                        className="btn btn-danger btn-sm"
                    >
                        Delete
                    </button>
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
            onClose={alertProps.onClose}
            onConfirm={alertProps.onConfirm}
        />
    </div>
  );
}

export default EscalaExtraList;