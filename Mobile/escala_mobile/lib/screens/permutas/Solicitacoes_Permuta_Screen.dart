import 'dart:convert';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/services/HttpInterceptor%20.dart';
import 'package:escala_mobile/services/api_service.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

class SolicitacoesPermutasScreen extends StatefulWidget {
  const SolicitacoesPermutasScreen({super.key});

  @override
  State<SolicitacoesPermutasScreen> createState() => _SolicitacoesPermutasScreenState();
}

class _SolicitacoesPermutasScreenState extends State<SolicitacoesPermutasScreen> {
  List<Map<String, dynamic>> _permutasSolicitadas = [];

  @override
  void initState() {
    super.initState();
    _buscarPermutasSolicitadas();
  }

  Future<void> _buscarPermutasSolicitadas() async {
  try {
    final userModel = Provider.of<UserModel>(context, listen: false);
    final String url = "${ApiService.baseUrl}/permutas/PermutaFuncionarioPorId/${userModel.idFuncionario}";
    final response = await ApiClient.get(url);

    if (response.statusCode == 200) {
      final List<dynamic> data = jsonDecode(response.body);
      setState(() {
        _permutasSolicitadas = data
            .where((p) =>
                p["nmNomeSolicitante"] != null &&
                p["dtDataSolicitadaTroca"] != null &&
                (p["idFuncionarioSolicitante"] == userModel.idFuncionario ||
                 p["idFuncionarioSolicitado"] == userModel.idFuncionario))
            .map((p) => {
                  "idPermuta": p["idPermuta"]?.toString() ?? "",
                  "solicitante": p["nmNomeSolicitante"] ?? "",
                  "dataSolicitadaTroca": p["dtDataSolicitadaTroca"] != null
                      ? _formatarData(p["dtDataSolicitadaTroca"])
                      : "",
                  "aprovado": p["nmAprovador"] != null,
                  "isSolicitante": p["idFuncionarioSolicitante"] == userModel.idFuncionario,
                })
            .toList();
      });
      if (_permutasSolicitadas.isNotEmpty) {
        userModel.incrementNotificationCount(); // Incrementa contador
      }
    } else {
      throw Exception("Erro ao buscar permutas. Código: ${response.statusCode}");
    }
  } catch (e) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Erro ao buscar permutas: $e")),
    );
  }
}
  Future<void> _aprovarPermuta(String idPermuta, String data) async {
    try {
      final String url = "${ApiService.baseUrl}/permutas/Aprovar/$idPermuta";
      final userModel = Provider.of<UserModel>(context, listen: false);

      final Map<String, dynamic> aprovacaoData = {
        "idAprovador": userModel.idFuncionario,
        "nmAprovador": userModel.userName,
        "dtAprovacao": DateTime.now().toUtc().toIso8601String(),
      };

      print("📡 Aprovando permuta: $url");
      print("📤 Dados enviados: $aprovacaoData");

      final response = await ApiClient.put(url, aprovacaoData);

      if (response.statusCode == 200) {
        print("✅ Permuta aprovada com sucesso!");
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Permuta aprovada com sucesso!")),
        );
        _buscarPermutasSolicitadas();
      } else {
        throw Exception("Erro ao aprovar permuta. Código: ${response.statusCode}");
      }
    } catch (e) {
      print("❌ Erro ao aprovar permuta: $e");
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao aprovar permuta: $e")),
      );
    }
  }

  Future<void> _recusarPermuta(String idPermuta, String data) async {
    try {
      final String url = "${ApiService.baseUrl}/permutas/Recusar/$idPermuta";
      final userModel = Provider.of<UserModel>(context, listen: false);

      final Map<String, dynamic> recusaData = {
        "idAprovador": userModel.idFuncionario,
        "nmAprovador": userModel.userName,
        "dtRecusa": DateTime.now().toUtc().toIso8601String(),
      };

      print("📡 Recusando permuta: $url");
      print("📤 Dados enviados: $recusaData");

      final response = await ApiClient.put(url, recusaData);

      if (response.statusCode == 200) {
        print("✅ Permuta recusada com sucesso!");
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Permuta recusada com sucesso!")),
        );
        _buscarPermutasSolicitadas();
      } else {
        throw Exception("Erro ao recusar permuta. Código: ${response.statusCode}");
      }
    } catch (e) {
      print("❌ Erro ao recusar permuta: $e");
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao recusar permuta: $e")),
      );
    }
  }

  String _formatarData(String? dataISO) {
    if (dataISO == null || dataISO.isEmpty) return "N/A";
    final DateTime data = DateTime.parse(dataISO);
    return DateFormat("dd/MM/yyyy").format(data); // Ajustei para dd/MM/aaaa
  }

  void _mostrarConfirmacaoAprovar(String idPermuta, String data) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Confirmar Aprovação"),
          content: Text("Tem certeza que deseja aceitar a permuta na data $data?"),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(); // Fecha o pop-up sem ação
              },
              child: const Text("Cancelar"),
            ),
            ElevatedButton(
              onPressed: () {
                Navigator.of(context).pop(); // Fecha o pop-up
                _aprovarPermuta(idPermuta, data); // Dispara o evento
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF003580),
              ),
              child: const Text("OK"),
            ),
          ],
        );
      },
    );
  }

  void _mostrarConfirmacaoRecusar(String idPermuta, String data) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Confirmar Recusa"),
          content: Text("Tem certeza que deseja recusar a permuta na data $data?"),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(); // Fecha o pop-up sem ação
              },
              child: const Text("Cancelar"),
            ),
            ElevatedButton(
              onPressed: () {
                Navigator.of(context).pop(); // Fecha o pop-up
                _recusarPermuta(idPermuta, data); // Dispara o evento
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red,
              ),
              child: const Text("OK"),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          "Solicitações de Permutas",
          style: TextStyle(color: Colors.white),
        ),
        backgroundColor: const Color(0xFF003580),
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const SizedBox(height: 20),
            Center(
              child: Text(
                "Permutas Solicitadas",
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
            ),
            const SizedBox(height: 8),
            _permutasSolicitadas.isNotEmpty
                ? Center(
                    child: DataTable(
                      columnSpacing: 15,
                      columns: const [
                        DataColumn(
                          label: Text(
                            "Solicitante",
                            style: TextStyle(fontSize: 16),
                          ),
                        ),
                        DataColumn(
                          label: Text(
                            "Data",
                            style: TextStyle(fontSize: 16),
                          ),
                        ),
                        DataColumn(
                          label: Text(
                            "Aprovar",
                            style: TextStyle(fontSize: 16),
                          ),
                        ),
                        DataColumn(
                          label: Text(
                            "Recusar",
                            style: TextStyle(fontSize: 16),
                          ),
                        ),
                      ],
                    rows: _permutasSolicitadas.map((p) {
  return DataRow(cells: [
    DataCell(Text(p["solicitante"] ?? "", style: const TextStyle(fontSize: 14))),
    DataCell(Text(p["dataSolicitadaTroca"] ?? "", style: const TextStyle(fontSize: 14))),
    DataCell(
      p["aprovado"] || p["isSolicitante"]
          ? const SizedBox.shrink()
          : ElevatedButton(
              onPressed: () => _mostrarConfirmacaoAprovar(p["idPermuta"], p["dataSolicitadaTroca"], true),
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF003580),
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                minimumSize: const Size(60, 30),
              ),
              child: const Text("Aprovar", style: TextStyle(fontSize: 12, color: Colors.white)),
            ),
              ),
              DataCell(
                p["aprovado"] || p["isSolicitante"]
                    ? const SizedBox.shrink()
                    : ElevatedButton(
                        onPressed: () => _mostrarConfirmacaoRecusar(p["idPermuta"], p["dataSolicitadaTroca"], true),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.red,
                          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                          minimumSize: const Size(60, 30),
                        ),
                        child: const Text("Recusar", style: TextStyle(fontSize: 12, color: Colors.white)),
                      ),
              ),
            ]);
          }).toList(),
                    ),
                  )
                : const Center(
                    child: Text("Nenhuma solicitação de permuta encontrada."),
                  ),
          ],
        ),
      ),
    );
  }
}