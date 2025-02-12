import 'dart:convert';
import 'package:escala_mobile/services/api_service.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:provider/provider.dart';
import 'package:escala_mobile/models/user_model.dart';
import 'package:logger/logger.dart';
import 'package:intl/intl.dart';

// 🔹 Instância do Logger
final logger = Logger();

class EscalaScreen extends StatefulWidget {
  const EscalaScreen({super.key});

  @override
  State<EscalaScreen> createState() => _EscalaScreenState();
}

class _EscalaScreenState extends State<EscalaScreen> {
  String? _idEscalaSelecionada;
  List<Map<String, dynamic>> _escalas = [];
  Map<String, String> _postos = {};
  List<String> _postosFiltrados = [];
  List<Map<String, dynamic>> _escalaPronta = [];
  Map<String, String> _funcionarios = {};

  @override
  void initState() {
    super.initState();
    _carregarEscalasUsuarioLogado();
    _carregarPostos();
    _carregarFuncionarios();
  }

  Future<void> _carregarEscalasUsuarioLogado() async {
    try {
      final userModel = Provider.of<UserModel>(context, listen: false);
      final String url = "${ApiService.baseUrl}/escalaPronta/BuscarPorFuncionario/${userModel.idFuncionario}";
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        Set<String> escalasUnicas = {};
        final List<Map<String, dynamic>> escalas = data
            .map((e) {
              String escalaNome = "${e["nmNomeEscala"] ?? "Sem Nome"}";
              if (escalasUnicas.contains(escalaNome)) return null;
              escalasUnicas.add(escalaNome);
              return {
                "id": e["idEscala"]?.toString() ?? "",
                "nome": escalaNome,
              };
            })
            .where((element) => element != null)
            .cast<Map<String, dynamic>>()
            .toList();

        setState(() {
          _escalas = escalas;
        });
      }
    } catch (e) {
      logger.e("Erro ao carregar escalas: $e");
    }
  }

  Future<void> _carregarPostos() async {
    try {
      final String url = "${ApiService.baseUrl}/PostoTrabalho/buscarTodos";
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        setState(() {
          _postos = {for (var posto in data) posto["idPostoTrabalho"].toString(): posto["nmNome"].toString()};
        });
      }
    } catch (e) {
      logger.e("Erro ao carregar postos: $e");
    }
  }

  Future<void> _carregarFuncionarios() async {
    try {
      final String url = "${ApiService.baseUrl}/Funcionario/buscarTodos";
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        setState(() {
          _funcionarios = {for (var func in data) func["idFuncionario"].toString(): func["nmNome"].toString()};
        });
      }
    } catch (e) {
      logger.e("Erro ao carregar funcionários: $e");
    }
  }

  Future<void> _filtrarPostosPorEscala(String idEscala) async {
    try {
      final String url = "${ApiService.baseUrl}/escalaPronta/buscarPorId/$idEscala";
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);

        List<Map<String, dynamic>> escalaFiltrada = data.map((e) {
          return {
            "dtDataServico": e["dtDataServico"] ?? "",
            "idPostoTrabalho": e["idPostoTrabalho"]?.toString() ?? "",
            "idFuncionario": e["idFuncionario"]?.toString() ?? ""
          };
        }).toList();

        Set<String> idsPostosNaEscala = escalaFiltrada.map((e) => e["idPostoTrabalho"].toString()).toSet();
        List<String> postosFiltrados = idsPostosNaEscala.map((id) => _postos[id] ?? "Posto Desconhecido").toList();

        setState(() {
          _escalaPronta = escalaFiltrada;
          _postosFiltrados = postosFiltrados;
        });
      }
    } catch (e) {
      logger.e("Erro ao carregar postos da escala: $e");
    }
  }

  /// 🔹 **Agrupa a escala por Dia e Posto**
  Map<String, Map<String, List<String>>> _agruparEscala() {
    Map<String, Map<String, List<String>>> escalaAgrupada = {};

    for (var item in _escalaPronta) {
      String diaCompleto = item["dtDataServico"];
      String diaFormatado = DateFormat("MM-dd").format(DateTime.parse(diaCompleto));
      String posto = _postos[item["idPostoTrabalho"]] ?? "Posto Desconhecido";
      String funcionario = item["idFuncionario"] == "00000000-0000-0000-0000-000000000000"
          ? "Sem Funcionário"
          : _funcionarios[item["idFuncionario"]] ?? "Funcionário Desconhecido";

      if (!escalaAgrupada.containsKey(diaFormatado)) {
        escalaAgrupada[diaFormatado] = {};
      }

      if (!escalaAgrupada[diaFormatado]!.containsKey(posto)) {
        escalaAgrupada[diaFormatado]![posto] = [];
      }

      escalaAgrupada[diaFormatado]![posto]!.add(funcionario);
    }

    return escalaAgrupada;
  }

  @override
  Widget build(BuildContext context) {
    Map<String, Map<String, List<String>>> escalaAgrupada = _agruparEscala();

    return Scaffold(
      appBar: AppBar(
        title: const Text("Escala", style: TextStyle(color: Colors.white)),
        backgroundColor: const Color(0xFF003580),
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: DropdownButtonFormField<String>(
              value: _idEscalaSelecionada,
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              ),
              items: _escalas.map((escala) {
                return DropdownMenuItem<String>(
                  value: escala["id"],
                  child: Text(escala["nome"]),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _idEscalaSelecionada = value;
                  _filtrarPostosPorEscala(value!);
                });
              },
              hint: const Text("Selecione uma escala"),
            ),
          ),

         Align(
            alignment: Alignment.centerLeft, // 🔹 Mantém o botão alinhado à esquerda
            child: Padding(
              padding: const EdgeInsets.only(left: 16), // 🔹 Ajuste o valor para espaçamento da esquerda
              child: ElevatedButton(
                onPressed: () {
                  // Implementação futura do PDF
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
            ),
          ),



          if (_escalaPronta.isNotEmpty) ...[
            Expanded(
              child: SingleChildScrollView(
                scrollDirection: Axis.vertical,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    DataTable(
                      columns: [const DataColumn(label: Text("Dia"))],
                      rows: escalaAgrupada.keys.map((dia) => DataRow(cells: [DataCell(Text(dia))])).toList(),
                    ),
                    Expanded(
                      child: SingleChildScrollView(
                        scrollDirection: Axis.horizontal,
                        child: DataTable(
                          columns: _postosFiltrados.isNotEmpty
                              ? _postosFiltrados.map((posto) => DataColumn(label: Text(posto))).toList()
                              : [],
                          rows: escalaAgrupada.keys.map((dia) => DataRow(
                            cells: _postosFiltrados.map((posto) {
                              List<String> funcionarios = escalaAgrupada[dia]?[posto] ?? ["-"];
                              return DataCell(Column(children: funcionarios.map((func) => Text(func)).toList()));
                            }).toList(),
                          )).toList(),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}
