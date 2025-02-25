import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart'; // Adicione esta linha

class AuthService {
  static const String baseUrl = "http://10.0.2.2:7207";

  static Future<Map<String, dynamic>> login(String usuario, String senha) async {
    try {
      final url = Uri.parse("$baseUrl/login/autenticar");
      print("📡 Enviando requisição para: $url");

      final response = await http.post(
        url,
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"usuario": usuario, "senha": senha}),
      );

      print("🔹 Status Code: ${response.statusCode}");
      print("🔹 Resposta: ${response.body}");

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        if (responseData.containsKey("token")) {
          return {
            "success": true,
            "token": responseData["token"],
            "refreshToken": responseData["refreshToken"] ?? "",
            "nomeUsuario": responseData["nomeUsuario"],
            "matricula": responseData["matricula"] ?? "",
            "idFuncionario": responseData["idFuncionario"] ?? "",
          };
        } else {
          return {"success": false, "message": "Resposta inválida do servidor."};
        }
      } else {
        return {
          "success": false,
          "message": response.body.isNotEmpty
              ? jsonDecode(response.body)["mensagem"] ?? "Erro desconhecido."
              : "Erro ao conectar ao servidor__LOGIN.",
        };
      }
    } catch (e) {
      print("❌ Erro durante a requisição: $e");
      return {"success": false, "message": "Erro de conexão com o servidor__LOGIN: $e"};
    }
  }

  static Future<void> saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('jwt_token', token);
  }

  static Future<void> saveRefreshToken(String refreshToken) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('refresh_token', refreshToken);
  }

  static Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }

  static Future<String?> getRefreshToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('refresh_token');
  }

  static Future<void> clearTokens() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('jwt_token');
    await prefs.remove('refresh_token');
  }

  static Future<Map<String, dynamic>> resetPassword(String email) async {
    try {
      final response = await http.post(
        Uri.parse("$baseUrl/login/esqueci-senha"),
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"email": email}),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {"success": true, "message": responseData["mensagem"]};
      } else {
        return {
          "success": false,
          "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao solicitar redefinição.",
        };
      }
    } catch (e) {
      return {"success": false, "message": "Erro de conexão com o servidor."};
    }
  }

  static Future<Map<String, dynamic>> resetPasswordWithToken(String token, String novaSenha) async {
    try {
      final response = await http.post(
        Uri.parse("$baseUrl/login/redefinir-senha"),
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"token": token, "novaSenha": novaSenha}),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {"success": true, "message": responseData["mensagem"]};
      } else {
        return {
          "success": false,
          "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao redefinir senha.",
        };
      }
    } catch (e) {
      return {"success": false, "message": "Erro de conexão com o servidor."};
    }
  }

  static Future<Map<String, dynamic>> register(String usuario, String senha) async {
    try {
      final response = await http.post(
        Uri.parse("$baseUrl/login/Incluir"),
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"usuario": usuario, "senha": senha}),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {"success": true, "message": responseData["mensagem"]};
      } else {
        return {
          "success": false,
          "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao criar acesso.",
        };
      }
    } catch (e) {
      return {"success": false, "message": "Erro de conexão com o servidor."};
    }
  }

  static Future<Map<String, dynamic>> refreshToken(String refreshToken) async {
    try {
      final url = Uri.parse("$baseUrl/login/refresh");
      final response = await http.post(
        url,
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"refreshToken": refreshToken}),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {
          "success": true,
          "token": responseData["token"],
          "refreshToken": responseData["refreshToken"],
        };
      } else {
        return {"success": false, "message": "Falha ao renovar token"};
      }
    } catch (e) {
      return {"success": false, "message": "Erro ao renovar token: $e"};
    }
  }
}