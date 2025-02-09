import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

class PermutaScreen extends StatefulWidget {
  const PermutaScreen({super.key});

  @override
  State<PermutaScreen> createState() => _PermutaScreenState();
}

class _PermutaScreenState extends State<PermutaScreen> {
  // Variáveis para armazenar os dados do formulário
  String _nomeSolicitante = "Endrigo Moura Valente"; // Nome do usuário logado
  String? _idEscalaSelecionada;
  String? _idFuncionarioSolicitado;
  String? _dataSelecionada;

  // Dados fictícios para testes visuais
  final List<Map<String, String>> _escalas = [
    {"id": "1", "nome": "Escala A - Janeiro"},
    {"id": "2", "nome": "Escala B - Fevereiro"},
    {"id": "3", "nome": "Escala C - Março"},
  ];

  final List<Map<String, String>> _funcionarios = [
    {"id": "101", "nome": "João Silva - 1234"},
    {"id": "102", "nome": "Maria Souza - 5678"},
    {"id": "103", "nome": "Carlos Pereira - 9101"},
  ];

  final List<String> _datas = [
    "2023-10-01",
    "2023-10-05",
    "2023-10-10",
  ];

  // Dados fictícios para a tabela de permutas
  final List<Map<String, dynamic>> _permutas = [
    {
      "nmNomeSolicitante": "Endrigo Moura Valente",
      "nmNomeSolicitado": "João Silva",
      "dtDataSolicitadaTroca": "2023-10-01",
      "dtSolicitacao": "2023-09-25",
      "nmNomeAprovador": "Maria Souza",
      "dtAprovacao": "2023-09-26",
    },
    {
      "nmNomeSolicitante": "Maria Souza",
      "nmNomeSolicitado": "Endrigo Moura Valente",
      "dtDataSolicitadaTroca": "2023-10-05",
      "dtSolicitacao": "2023-09-28",
      "nmNomeAprovador": "Carlos Pereira",
      "dtAprovacao": "2023-09-29",
    },
    {
      "nmNomeSolicitante": "Endrigo Moura Valente",
      "nmNomeSolicitado": "Carlos Pereira",
      "dtDataSolicitadaTroca": "2023-10-10",
      "dtSolicitacao": "2023-10-01",
      "nmNomeAprovador": null,
      "dtAprovacao": null,
    },
    {
      "nmNomeSolicitante": "João Silva",
      "nmNomeSolicitado": "Endrigo Moura Valente",
      "dtDataSolicitadaTroca": "2023-10-15",
      "dtSolicitacao": "2023-10-05",
      "nmNomeAprovador": "Maria Souza",
      "dtAprovacao": "2023-10-06",
    },
    {
      "nmNomeSolicitante": "Endrigo Moura Valente",
      "nmNomeSolicitado": "Maria Souza",
      "dtDataSolicitadaTroca": "2023-10-20",
      "dtSolicitacao": "2023-10-10",
      "nmNomeAprovador": "Carlos Pereira",
      "dtAprovacao": "2023-10-12",
    },
    {
      "nmNomeSolicitante": "Carlos Pereira",
      "nmNomeSolicitado": "Endrigo Moura Valente",
      "dtDataSolicitadaTroca": "2023-10-25",
      "dtSolicitacao": "2023-10-15",
      "nmNomeAprovador": null,
      "dtAprovacao": null,
    },
    {
      "nmNomeSolicitante": "Endrigo Moura Valente",
      "nmNomeSolicitado": "João Silva",
      "dtDataSolicitadaTroca": "2023-10-30",
      "dtSolicitacao": "2023-10-20",
      "nmNomeAprovador": "Maria Souza",
      "dtAprovacao": "2023-10-22",
    },
  ];

  // Função para formatar datas
  String _formatarData(String? dataISO) {
    if (dataISO == null || dataISO.isEmpty) return "N/A";
    final DateTime data = DateTime.parse(dataISO);
    return DateFormat("dd-MM-yyyy").format(data); // Formata a data como "dia-mês-ano"
  }

  // Filtra as permutas para exibir apenas aquelas envolvendo o usuário logado
  List<Map<String, dynamic>> get _permutasFiltradas {
    return _permutas;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Solicitar Permuta"),
        backgroundColor: const Color(0xFF003580),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Formulário de Solicitação de Permuta
            ..._buildForm(),

            // Tabela de Permutas
            const SizedBox(height: 24),
            Center(
              child: Text(
                "Histórico de Permutas",
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
            ),
            const SizedBox(height: 8),
            DataTable(
              dataRowHeight: 40, // Altura reduzida para melhorar a visualização
              columnSpacing: 10, // Espaçamento reduzido entre colunas
              columns: const [
                DataColumn(label: Text("Solicit.", textAlign: TextAlign.center)),
                DataColumn(label: Text("Solicitado", textAlign: TextAlign.center)),
                DataColumn(label: Text("Data Troca", textAlign: TextAlign.center)),
                DataColumn(label: Text("Aprov.", textAlign: TextAlign.center)), // Coluna corrigida
              ],
              rows: _permutasFiltradas.map((permuta) {
                bool isAprovada = permuta["nmNomeAprovador"] != null; // Verifica se há aprovador
                return DataRow(cells: [
                  DataCell(Text(
                    _getPrimeiroNome(permuta["nmNomeSolicitante"]),
                    textAlign: TextAlign.center,
                  )),
                  DataCell(Text(
                    _getPrimeiroNome(permuta["nmNomeSolicitado"]),
                    textAlign: TextAlign.center,
                  )),
                  DataCell(Text(
                    _formatarData(permuta["dtDataSolicitadaTroca"]),
                    textAlign: TextAlign.center,
                  )),
                  DataCell(Center(child: Icon(
                    isAprovada ? Icons.check_box : Icons.check_box_outline_blank,
                    color: isAprovada ? Colors.green : Colors.red,
                    size: 18, // Tamanho reduzido do ícone
                  ))), // Checkbox somente leitura
                ]);
              }).toList(),
            ),
          ],
        ),
      ),
    );
  }

  // Constrói o formulário de solicitação de permuta
  List<Widget> _buildForm() {
    return [
      // Nome do Solicitante
      Text(
        "Solicitante",
        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
      ),
      const SizedBox(height: 8),
      Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: Colors.grey[200],
          borderRadius: BorderRadius.circular(8),
        ),
        child: Text(_nomeSolicitante),
      ),
      const SizedBox(height: 16),

      // Dropdown de Escalas
      Text(
        "Selecione a Escala",
        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
      ),
      const SizedBox(height: 8),
      DropdownButtonFormField<String>(
        value: _idEscalaSelecionada,
        decoration: InputDecoration(
          border: OutlineInputBorder(),
          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        ),
        items: _escalas.map((escala) {
          return DropdownMenuItem<String>(
            value: escala["id"],
            child: Text(escala["nome"]!),
          );
        }).toList(),
        onChanged: (value) {
          setState(() {
            _idEscalaSelecionada = value;
          });
        },
        hint: const Text("Selecione uma escala"),
      ),
      const SizedBox(height: 16),

      // Dropdown de Funcionários
      Text(
        "Selecione o Solicitado",
        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
      ),
      const SizedBox(height: 8),
      DropdownButtonFormField<String>(
        value: _idFuncionarioSolicitado,
        decoration: InputDecoration(
          border: OutlineInputBorder(),
          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        ),
        items: _funcionarios.map((funcionario) {
          return DropdownMenuItem<String>(
            value: funcionario["id"],
            child: Text(funcionario["nome"]!),
          );
        }).toList(),
        onChanged: (value) {
          setState(() {
            _idFuncionarioSolicitado = value;
          });
        },
        hint: const Text("Selecione um funcionário"),
      ),
      const SizedBox(height: 16),

      // Dropdown de Datas
      Text(
        "Selecione a Data",
        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
      ),
      const SizedBox(height: 8),
      DropdownButtonFormField<String>(
        value: _dataSelecionada,
        decoration: InputDecoration(
          border: OutlineInputBorder(),
          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        ),
        items: _datas.map((data) {
          return DropdownMenuItem<String>(
            value: data,
            child: Text(data),
          );
        }).toList(),
        onChanged: (value) {
          setState(() {
            _dataSelecionada = value;
          });
        },
        hint: const Text("Selecione uma data"),
      ),
      const SizedBox(height: 24),

      // Botões de Ação
      Row(
        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
        children: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context); // Retorna à tela anterior
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
            child: const Text(
              "Cancelar",
              style: TextStyle(fontSize: 16, color: Colors.white),
            ),
          ),
          ElevatedButton(
            onPressed: () {
              if (_idEscalaSelecionada == null ||
                  _idFuncionarioSolicitado == null ||
                  _dataSelecionada == null) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text("Preencha todos os campos.")),
                );
                return;
              }
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text("Permuta solicitada com sucesso!")),
              );
              Navigator.pop(context); // Volta para a tela anterior
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFF003580),
              padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
            child: const Text(
              "Solicitar",
              style: TextStyle(fontSize: 16, color: Colors.white),
            ),
          ),
        ],
      ),
    ];
  }

  // Função para obter o primeiro nome de um nome completo
  String _getPrimeiroNome(String? nomeCompleto) {
    if (nomeCompleto == null || nomeCompleto.isEmpty) return "N/A";
    return nomeCompleto.split(" ").first;
  }
}