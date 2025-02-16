import 'dart:convert';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/services/api_service.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:http/http.dart' as http;
import 'package:provider/provider.dart';

class PermutaScreen extends StatefulWidget {
  const PermutaScreen({super.key});

  @override
  State<PermutaScreen> createState() => _PermutaScreenState();
}

class _PermutaScreenState extends State<PermutaScreen> {
  // Variáveis para armazenar os dados do formulário
  String? _idEscalaSelecionada;
  String? _idFuncionarioSolicitado;
  String? _dataSelecionada;

  // Dados dinâmicos vindos da API
  List<Map<String, dynamic>> _escalas = [];
  List<Map<String, dynamic>> _funcionariosEscala = [];
  List<Map<String, dynamic>> _permutasSolicitadas = [];


  @override
  void initState() {
    super.initState();
    _carregarEscalasUsuarioLogado();
    _buscarPermutasSolicitadas();
  }

  List<Map<String, dynamic>> _datasTrabalhoUsuario = []; // 🔹 Armazena idEscala e data
  List<String> _datasFiltradasPorEscala = []; // 🔹 Apenas datas filtradas para exibição


 Future<void> _carregarEscalasUsuarioLogado() async {
  try {
    final userModel = Provider.of<UserModel>(context, listen: false);

    final String url = "${ApiService.baseUrl}/escalaPronta/BuscarPorFuncionario/${userModel.idFuncionario}";

    print("📡 Fazendo requisição para: $url");

    final response = await http.get(Uri.parse(url));

    if (response.statusCode == 200) {
      final List<dynamic> data = jsonDecode(response.body);

      // Remover duplicatas das escalas
      Set<String> escalasUnicas = {};
      final List<Map<String, dynamic>> escalas = data.map((e) {
        String escalaNome = "${e["nmNomeEscala"]} - ${DateFormat("MMMM").format(DateTime.parse(e["dtDataServico"]))}";
        
        if (escalasUnicas.contains(escalaNome)) {
          return null;
        }
        escalasUnicas.add(escalaNome);

        return {
          "id": e["idEscala"],
          "nome": escalaNome,
        };
      }).where((element) => element != null).cast<Map<String, dynamic>>().toList();

      // 💡 **Agora armazenamos um Map<String, dynamic> na lista de datas**
      final List<Map<String, dynamic>> todasAsDatas = data
          .map((e) => {
                "idEscala": e["idEscala"].toString(),
                "data": DateFormat("dd-MM-yyyy").format(DateTime.parse(e["dtDataServico"])),
              })
          .toList();

      setState(() {
        _escalas = escalas;
        _datasTrabalhoUsuario = todasAsDatas; // 💡 Agora _datasTrabalhoUsuario armazena um Map
        _datasFiltradasPorEscala = [];
      });

      print("✅ Escalas carregadas: ${_escalas.length}");
      print("✅ Datas carregadas: ${_datasTrabalhoUsuario}");
    } else {
      throw Exception("Erro ao carregar escalas. Código: ${response.statusCode}");
    }
  } catch (e) {
    print("❌ Exceção capturada: $e");
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Erro ao carregar escalas: $e")),
    );
  }
}

  void _filtrarDatasPorEscala(String idEscala) {
  setState(() {
    _datasFiltradasPorEscala = _datasTrabalhoUsuario
        .where((e) => e["idEscala"].toString() == idEscala) // 🔹 Comparação correta
        .map((e) => e["data"].toString()) // 🔹 Garante que seja String
        .toList();

    print("📅 Datas filtradas para a escala $idEscala: $_datasFiltradasPorEscala");
  });
}

  Future<void> _buscarFuncionariosEscala(String idEscala) async {
  try {
    final String urlEscala = "${ApiService.baseUrl}/escalaPronta/buscarPorId/$idEscala";
    final String urlFuncionarios = "${ApiService.baseUrl}/funcionario/buscarTodos";

    print("📡 Buscando funcionários da escala: $urlEscala");
    print("📡 Buscando lista completa de funcionários: $urlFuncionarios");

    final responseEscala = await http.get(Uri.parse(urlEscala));
    final responseFuncionarios = await http.get(Uri.parse(urlFuncionarios));

    if (responseEscala.statusCode == 200 && responseFuncionarios.statusCode == 200) {
      final List<dynamic> dataEscala = jsonDecode(responseEscala.body);
      final List<dynamic> dataFuncionarios = jsonDecode(responseFuncionarios.body);

      // Criamos um conjunto para armazenar apenas IDs únicos de funcionários na escala
      Set<String> idsFuncionariosEscala = dataEscala
          .map<String>((f) => f["idFuncionario"].toString())
          .toSet();

    // Filtramos apenas os funcionários que pertencem à escala selecionada
    final List<Map<String, dynamic>> funcionarios = dataFuncionarios
        .where((funcionario) =>
            idsFuncionariosEscala.contains(funcionario["idFuncionario"]?.toString() ?? ""))
        .map((f) => {
              "idFuncionario": f["idFuncionario"]?.toString() ?? "", // ✅ Garante que não seja null
              "nmNome": f["nmNome"] ?? "Nome Desconhecido", // ✅ Substitui null por um valor padrão
              "nrMatricula": f["nrMatricula"]?.toString() ?? "Sem Matrícula",
            })
        .toList();


      setState(() {
        _funcionariosEscala = funcionarios;
      });

      // 🔹 **Agora chamamos `_filtrarFuncionariosDisponiveis()`**
      _filtrarFuncionariosDisponiveis();

      print("✅ Funcionários carregados (${_funcionariosEscala.length} encontrados)");
    } else {
      throw Exception("Erro ao buscar funcionários. Código: ${responseEscala.statusCode} ou ${responseFuncionarios.statusCode}");
    }
  } catch (e) {
    print("❌ Erro ao buscar funcionários: $e");
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Erro ao buscar funcionários: $e")),
    );
  }
}

void _filtrarFuncionariosDisponiveis() {
  final userModel = Provider.of<UserModel>(context, listen: false);

  setState(() {
    _funcionariosEscala = _funcionariosEscala.where((funcionario) {
      // Lista de datas em que o solicitante está escalado para essa escala
      final List<String> datasSolicitante = _datasTrabalhoUsuario
          .where((e) =>
              e["idFuncionario"] == userModel.idFuncionario &&
              e["idEscala"] == _idEscalaSelecionada)
          .map((e) => e["data"].toString())
          .toList();

      // Verifica se esse funcionário está trabalhando no mesmo dia que o solicitante
      bool trabalhaNoMesmoDia = _datasTrabalhoUsuario.any((f) =>
          f["idFuncionario"] == funcionario["idFuncionario"] &&
          f["idEscala"] == _idEscalaSelecionada &&
          datasSolicitante.contains(f["data"].toString()));

      return !trabalhaNoMesmoDia; // Somente retorna funcionários disponíveis
    }).toList();
  });

  print("✅ Funcionários disponíveis filtrados: ${_funcionariosEscala.length}");
}

Future<void> _enviarSolicitacaoPermuta() async {
  try {
    if (_idEscalaSelecionada == null ||
        _idFuncionarioSolicitado == null ||
        _dataSelecionada == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Preencha todos os campos.")),
      );
      return;
    }

    final userModel = Provider.of<UserModel>(context, listen: false);
    final String url = "${ApiService.baseUrl}/permutas/Incluir";


    final Map<String, dynamic> permutaData = {
      "dtDataSolicitadaTroca": DateFormat("yyyy-MM-dd'T'00:00:00.000'Z'").format(
          DateFormat("dd-MM-yyyy").parse(_dataSelecionada!)), // 🔹 Ajuste de formato
      "dtSolicitacao": DateTime.now().toUtc().toIso8601String(),
      "idEscala": _idEscalaSelecionada,
      "idFuncionarioSolicitado": _idFuncionarioSolicitado,
      "idFuncionarioSolicitante": userModel.idFuncionario,
      "nmNomeSolicitado": _funcionariosEscala
          .firstWhere((f) => f["idFuncionario"] == _idFuncionarioSolicitado)["nmNome"],
      "nmNomeSolicitante": userModel.userName
    };

    print("📡 Enviando requisição para: $url");
    print("📤 Dados enviados: $permutaData");

    final response = await http.post(
      Uri.parse(url),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode(permutaData),
    );

    if (response.statusCode == 200) {
      print("✅ Permuta cadastrada com sucesso!");

      // 🟢 Exibir modal de sucesso e limpar os selects
      _buscarPermutasSolicitadas();
      _mostrarDialogoSucesso();
    } else {
      throw Exception("Erro ao cadastrar permuta. Código: ${response.statusCode}");
    }
  } catch (e) {
    print("❌ Erro ao enviar permuta: $e");
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Erro ao enviar permuta: $e")),
    );
  }
}

void _mostrarDialogoSucesso() {
  showDialog(
    context: context,
    barrierDismissible: false, // O usuário não pode fechar tocando fora
    builder: (BuildContext context) {
      return AlertDialog(
        title: const Text("Permuta Solicitada"),
        content: const Text("A solicitação de permuta foi enviada com sucesso!"),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.of(context).pop(); // Fecha o modal
              _limparCampos(); // Limpa os selects
              // _buscarPermutasSolicitadas();//recarrega a grid
            },
            child: const Text("OK"),
          ),
        ],
      );
    },
  );
}

void _limparCampos() {
  setState(() {
    _idEscalaSelecionada = null;
    _idFuncionarioSolicitado = null;
    _dataSelecionada = null;
    _funcionariosEscala = [];
    _datasFiltradasPorEscala = [];
  });
}

Future<void> _buscarPermutasSolicitadas() async {
  try {
    final userModel = Provider.of<UserModel>(context, listen: false);
    final String url = "${ApiService.baseUrl}/permutas/PermutaFuncionarioPorId/${userModel.idFuncionario}";

    print("📡 Buscando permutas solicitadas: $url");

    final response = await http.get(Uri.parse(url));

    if (response.statusCode == 200) {
      final List<dynamic> data = jsonDecode(response.body);

      setState(() {
        _permutasSolicitadas = data.map((p) => {
          "solicitante": p["nmNomeSolicitante"],
          "solicitado": p["nmNomeSolicitado"],
          "dataSolicitadaTroca": _formatarData(p["dtDataSolicitadaTroca"]),
          "aprovado": p["nmAprovador"] != null, // Se `nmAprovador` for null, será `false`
        }).toList();
      });

      print("✅ Permutas carregadas: ${_permutasSolicitadas.length}");
    } else {
      throw Exception("Erro ao buscar permutas. Código: ${response.statusCode}");
    }
  } catch (e) {
    print("❌ Erro ao buscar permutas: $e");
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Erro ao buscar permutas: $e")),
    );
  }
}


  String _formatarData(String? dataISO) {
    if (dataISO == null || dataISO.isEmpty) return "N/A";
    final DateTime data = DateTime.parse(dataISO);
    return DateFormat("dd-MM-yyyy").format(data); // Formata a data como "dia-mês-ano"
  }

  @override
  Widget build(BuildContext context) {
    final userModel = Provider.of<UserModel>(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text(
          "Solicitar Permuta",
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
              child: Text(userModel.userName),
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
                child: Text(escala["nome"]),
              );
            }).toList(),
            onChanged: (value) {
              if (value != null) {
                setState(() {
                  _idEscalaSelecionada = value;
                  _buscarFuncionariosEscala(value);
                  _filtrarDatasPorEscala(value); // 🔹 Agora filtramos as datas da escala selecionada
                });
              }
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
              items: _funcionariosEscala.isNotEmpty
                  ? _funcionariosEscala.map((funcionario) {
                      return DropdownMenuItem<String>(
                        value: funcionario["idFuncionario"],
                        child: Text("${funcionario["nmNome"]} - ${funcionario["nrMatricula"]}"),
                      );
                    }).toList()
                  : [],
              onChanged: (value) {
                setState(() {
                  _idFuncionarioSolicitado = value;
                });
              },
              hint: Text(_funcionariosEscala.isNotEmpty ? "Selecione um funcionário" : "Nenhum funcionário disponível"),
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
              items: _datasFiltradasPorEscala.isNotEmpty
                  ? _datasFiltradasPorEscala.map((data) {
                      return DropdownMenuItem<String>(
                        value: data,
                        child: Text(data), // Exibe a data já formatada como dd-MM-yyyy
                      );
                    }).toList()
                  : [],
              onChanged: (value) {
                setState(() {
                  _dataSelecionada = value;
                });
              },
              hint: Text(_datasFiltradasPorEscala.isNotEmpty ? "Selecione uma data" : "Nenhuma data disponível"),
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
                  onPressed: _enviarSolicitacaoPermuta,
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
                ),],
            ),
            // Tabela de Permutas Solicitadas
            const SizedBox(height: 20),
            Text(
              "Permutas Solicitadas",
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),

            _permutasSolicitadas.isNotEmpty
              ? SingleChildScrollView(
                  scrollDirection: Axis.horizontal,
                  child: DataTable(
                    columnSpacing: 12.0, // 🔹 Reduzimos um pouco para melhor distribuição
                    columns: const [
                      DataColumn(
                        label: Text(
                          "Solicitante",
                          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14), // 🔹 Fonte menor
                        ),
                      ),
                      DataColumn(
                        label: Text(
                          "Solicitado",
                          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14), // 🔹 Fonte menor
                        ),
                      ),
                      DataColumn(
                        label: Text(
                          "Data",
                          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14), // 🔹 Fonte menor
                        ),
                      ),
                      DataColumn(
                        label: Align(
                          alignment: Alignment.centerLeft, // 🔹 Puxa um pouco para a esquerda
                          child: Text(
                            "Autorizado",
                            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14), // 🔹 Fonte menor
                          ),
                        ),
                      ),
                    ],
                    rows: _permutasSolicitadas.map((p) {
                      return DataRow(cells: [
                        DataCell(Text(p["solicitante"], style: TextStyle(fontSize: 13))),
                        DataCell(Text(p["solicitado"], style: TextStyle(fontSize: 13))),
                        DataCell(Text(p["dataSolicitadaTroca"], style: TextStyle(fontSize: 13))),
                        DataCell(
                          Align(
                            alignment: Alignment.centerLeft, // 🔹 Alinha o checkbox à esquerda
                            child: Checkbox(
                              value: p["aprovado"],
                              onChanged: null, // 🔹 Checkbox readonly
                            ),
                          ),
                        ),
                      ]);
                    }).toList(),
                  ),
                )
              : const Text("Nenhuma permuta encontrada."),
          ],
        ),
        
      ),
    );
  }
}