import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class AuthService {
  static const String baseUrl = "http://192.168.0.8:7207"; // Substitua pelo seu backend

  // Método para login
  static Future<Map<String, dynamic>> login(String usuario, String senha) async {
    final response = await http.post(
      Uri.parse("$baseUrl/login/autenticar"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"usuario": usuario, "senha": senha}),
    );

    if (response.statusCode == 200) {
      final responseData = jsonDecode(response.body);
      return {
        "success": true,
        "token": responseData["token"],
        "nomeUsuario": responseData["nomeUsuario"],
        "permissoes": responseData["permissoes"],
      };
    } else {
      return {
        "success": false,
        "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao fazer login.",
      };
    }
  }

  // Método para salvar o token JWT localmente
  static Future<void> saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('jwt_token', token);
  }

  // Método para recuperar o token JWT salvo
  static Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }

  // Método para limpar o token JWT (logout)
  static Future<void> clearToken() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('jwt_token');
  }

  // Método para solicitar redefinição de senha (enviar e-mail)
  static Future<Map<String, dynamic>> resetPassword(String email) async {
    final response = await http.post(
      Uri.parse("$baseUrl/login/esqueci-senha"), // Endpoint para solicitar redefinição de senha
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"email": email}),
    );

    if (response.statusCode == 200) {
      final responseData = jsonDecode(response.body);
      return {
        "success": true,
        "message": responseData["mensagem"], // Mensagem de sucesso do backend
      };
    } else {
      return {
        "success": false,
        "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao solicitar redefinição.",
      };
    }
  }

  // Método para redefinir senha com token
  static Future<Map<String, dynamic>> resetPasswordWithToken(String token, String novaSenha) async {
    final response = await http.post(
      Uri.parse("$baseUrl/login/redefinir-senha"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"token": token, "novaSenha": novaSenha}),
    );

    if (response.statusCode == 200) {
      final responseData = jsonDecode(response.body);
      return {
        "success": true,
        "message": responseData["mensagem"],
      };
    } else {
      return {
        "success": false,
        "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao redefinir senha.",
      };
    }
  }

  // Método para registrar um novo usuário
  static Future<Map<String, dynamic>> register(String usuario, String senha) async {
    final response = await http.post(
      Uri.parse("$baseUrl/login/Incluir"), // Endpoint para registro de novo usuário
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"usuario": usuario, "senha": senha}),
    );

    if (response.statusCode == 200) {
      final responseData = jsonDecode(response.body);
      return {
        "success": true,
        "message": responseData["mensagem"], // Mensagem de sucesso do backend
      };
    } else {
      return {
        "success": false,
        "message": jsonDecode(response.body)["mensagem"] ?? "Erro ao criar acesso.",
      };
    }
  }
}