import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiService {
  static const String baseUrl = "https://gestao-escala-back-175014489605.southamerica-east1.run.app"; // Substitua pelo seu backend

  // Busca as escalas ativas
  Future<List<Map<String, dynamic>>> buscarEscalasAtivas() async {
    final response = await http.get(Uri.parse("$baseUrl/escala/buscarTodos"));
    if (response.statusCode == 200) {
      final List<dynamic> data = jsonDecode(response.body);
      return data
          .where((e) => e["isAtivo"] == true && e["isGerada"] == true)
          .map((e) => {"id": e["idEscala"], "nmNomeEscala": e["nmNomeEscala"], "nrMesReferencia": e["nrMesReferencia"]})
          .toList();
    } else {
      throw Exception("Erro ao buscar escalas.");
    }
  }

  // Busca os funcionários da escala selecionada
  Future<List<Map<String, dynamic>>> buscarFuncionariosPorEscala(String idEscala) async {
    final response = await http.get(Uri.parse("$baseUrl/escalaPronta/buscarPorId/$idEscala"));
    if (response.statusCode == 200) {
      final List<dynamic> data = jsonDecode(response.body);
      return data.map((f) => {"idFuncionario": f["idFuncionario"], "nmNome": f["nmNome"], "nrMatricula": f["nrMatricula"]}).toList();
    } else {
      throw Exception("Erro ao buscar funcionários da escala.");
    }
  }

  // Busca as datas de trabalho do solicitante
  Future<List<String>> buscarDatasTrabalho(String idEscala, String matricula) async {
    try {
      final response = await http.get(Uri.parse("$baseUrl/escalaPronta/buscarDatasPorFuncionario/$idEscala/$matricula"));

      if (response.statusCode == 200) {
        // Decodifica a resposta JSON
        final List<dynamic> data = jsonDecode(response.body);

        // Converte a lista de dynamic para List<String>
        final List<String> datas = data.map((item) => item.toString()).toList();

        return datas; // Retorna a lista convertida
      } else {
        throw Exception("Erro ao buscar datas de trabalho. Código: ${response.statusCode}");
      }
    } catch (e) {
      throw Exception("Erro ao buscar datas de trabalho: $e");
    }
  }

  // Busca as escalas ativas para o funcionário logado
  Future<List<Map<String, dynamic>>> buscarEscalasPorFuncionario(String idFuncionario) async {
    try {
      final response = await http.get(Uri.parse("$baseUrl/escalaPronta/BuscarPorFuncionario/$idFuncionario"));

      if (response.statusCode == 200) {
        // Decodifica a resposta JSON
        final List<dynamic> data = jsonDecode(response.body);

        // Converte os dados em uma lista de mapas com os campos desejados
        return data.map((e) {
          return {
            "id": e["idEscala"],
            "nmNomeEscala": e["nmNomeEscala"],
            "nrMesReferencia": e["nrMesReferencia"],
          };
        }).toList();
      } else {
        throw Exception("Erro ao buscar escalas por funcionário. Código: ${response.statusCode}");
      }
    } catch (e) {
      throw Exception("Erro ao buscar escalas por funcionário: $e");
    }
  }
}