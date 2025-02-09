import 'package:flutter/material.dart';

class EscalaScreen extends StatefulWidget {
  const EscalaScreen({super.key});

  @override
  State<EscalaScreen> createState() => _EscalaScreenState();
}

class _EscalaScreenState extends State<EscalaScreen> {
  // Variáveis para armazenar os dados da escala
  String? _escalaSelecionada;
  final List<String> _escalas = ["Abril", "Maio"]; // Escalas fictícias

  // Dados fictícios das escalas
  final Map<String, List<Map<String, dynamic>>> _dadosEscalas = {
    "Abril": [
    {"dia": "01", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas", "G": "Sofia", "H": "Miguel"},
    {"dia": "02", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João", "G": "Isabela", "H": "Enzo"},
    {"dia": "03", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria", "G": "Valentina", "H": "Arthur"},
    {"dia": "04", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos", "G": "Helena", "H": "Bernardo"},
    {"dia": "05", "A": "Pedro", "B": "Lucas", "C": "João", "D": "Maria", "E": "Carlos", "F": "Ana", "G": "Laura", "H": "Davi"},
    {"dia": "06", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro", "G": "Beatriz", "H": "Felipe"},
    {"dia": "07", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas", "G": "Alice", "H": "Gabriel"},
    {"dia": "08", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João", "G": "Maria Eduarda", "H": "Matheus"},
    {"dia": "09", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria", "G": "Julia", "H": "Pedro Henrique"},
    {"dia": "10", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos", "G": "Yasmin", "H": "Lucas Gabriel"},
    {"dia": "11", "A": "Pedro", "B": "Lucas", "C": "João", "D": "Maria", "E": "Carlos", "F": "Ana", "G": "Isabelle", "H": "Guilherme"},
    {"dia": "12", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro", "G": "Sophia", "H": "Samuel"},
    {"dia": "13", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas", "G": "Manuela", "H": "Rafael"},
    {"dia": "14", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João", "G": "Giovanna", "H": "Enzo Gabriel"},
    {"dia": "15", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria", "G": "Luiza", "H": "João Vitor"},
    {"dia": "16", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos", "G": "Gabriela", "H": "João Paulo"},
    {"dia": "17", "A": "Pedro", "B": "Lucas", "C": "João", "D": "Maria", "E": "Carlos", "F": "Ana", "G": "Fernanda", "H": "Thiago"},
    {"dia": "18", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro", "G": "Amanda", "H": "Eduardo"},
    {"dia": "19", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas", "G": "Juliana", "H": "Vitor"},
    {"dia": "20", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João", "G": "Carolina", "H": "Leonardo"},
    {"dia": "21", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria", "G": "Larissa", "H": "Rodrigo"},
    {"dia": "22", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos", "G": "Camila", "H": "Bruno"},
    {"dia": "23", "A": "Pedro", "B": "Lucas", "C": "João", "D": "Maria", "E": "Carlos", "F": "Ana", "G": "Letícia", "H": "Vinicius"},
    {"dia": "24", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro", "G": "Patricia", "H": "Gustavo"},
    {"dia": "25", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas", "G": "Renata", "H": "Daniel"},
    {"dia": "26", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João", "G": "Aline", "H": "Marcelo"},
    {"dia": "27", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria", "G": "Sandra", "H": "Fábio"},
    {"dia": "28", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos", "G": "Marcia", "H": "André"},
    {"dia": "29", "A": "Pedro", "B": "Lucas", "C": "João", "D": "Maria", "E": "Carlos", "F": "Ana", "G": "Silvia", "H": "Roberto"},
    {"dia": "30", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro", "G": "Adriana", "H": "Paulo"}
  ],
    "Maio": [
      {"dia": "01", "A": "Lucas", "B": "João", "C": "Maria", "D": "Carlos", "E": "Ana", "F": "Pedro"},
      {"dia": "02", "A": "João", "B": "Maria", "C": "Carlos", "D": "Ana", "E": "Pedro", "F": "Lucas"},
      {"dia": "03", "A": "Maria", "B": "Carlos", "C": "Ana", "D": "Pedro", "E": "Lucas", "F": "João"},
      {"dia": "04", "A": "Carlos", "B": "Ana", "C": "Pedro", "D": "Lucas", "E": "João", "F": "Maria"},
      {"dia": "05", "A": "Ana", "B": "Pedro", "C": "Lucas", "D": "João", "E": "Maria", "F": "Carlos"},
      // Adicione mais dias conforme necessário
    ],
  };

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          "Escala",
          style: TextStyle(color: Colors.white),
        ),
        backgroundColor: const Color(0xFF003580),
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Dropdown de Escalas
            Text(
              "Selecione a Escala",
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            DropdownButtonFormField<String>(
              value: _escalaSelecionada,
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              ),
              items: _escalas.map((escala) {
                return DropdownMenuItem<String>(
                  value: escala,
                  child: Text(escala),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _escalaSelecionada = value;
                });
              },
              hint: const Text("Selecione uma escala"),
            ),

            // Botão Gerar PDF
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: () {
                // Função para gerar PDF (ainda não implementada)
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text("Função Gerar PDF ainda não implementada.")),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF003580),
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: const Text(
                "Gerar PDF",
                style: TextStyle(fontSize: 16, color: Colors.white),
              ),
            ),

            // Título da Escala Selecionada
            if (_escalaSelecionada != null) ...[
              const SizedBox(height: 16),
              Text(
                "Escala de $_escalaSelecionada",
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              SingleChildScrollView(
                scrollDirection: Axis.horizontal, // Scroll horizontal
                child: DataTable(
                  dataRowHeight: 40, // Altura reduzida para melhorar a visualização
                  columnSpacing: 10, // Espaçamento reduzido entre colunas
                  columns: [
                    DataColumn(label: Text("Dia")),
                    DataColumn(label: Text("Posto A")),
                    DataColumn(label: Text("Posto B")),
                    DataColumn(label: Text("Posto C")),
                    DataColumn(label: Text("Posto D")),
                    DataColumn(label: Text("Posto E")),
                    DataColumn(label: Text("Posto F")),
                  ],
                  rows: _dadosEscalas[_escalaSelecionada]!.map((linha) {
                    return DataRow(cells: [
                      DataCell(Text(linha["dia"])), // Renderiza o valor da chave "dia"
                      DataCell(Text(linha["A"])),
                      DataCell(Text(linha["B"])),
                      DataCell(Text(linha["C"])),
                      DataCell(Text(linha["D"])),
                      DataCell(Text(linha["E"])),
                      DataCell(Text(linha["F"])),
                    ]);
                  }).toList(),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}