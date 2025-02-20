import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class ApiClient {
  static const String baseUrl = ""; // Backend local

  // 🔹 Método para obter o token salvo
  static Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }

  // 🔹 Método GET com autenticação automática
  static Future<http.Response> get(String endpoint) async {
    final token = await _getToken();
    final url = Uri.parse("$baseUrl$endpoint");

    print("📡 GET: $url");

    final response = await http.get(
      url,
      headers: {
        "Content-Type": "application/json",
        if (token != null) "Authorization": "Bearer $token",
      },
    );

    return response;
  }

  // 🔹 Método POST com autenticação automática
  static Future<http.Response> post(String endpoint, Map<String, dynamic> body) async {
    final token = await _getToken();
    final url = Uri.parse("$baseUrl$endpoint");

    print("📡 POST: $url");
    print("📤 Enviando: ${jsonEncode(body)}");

    final response = await http.post(
      url,
      headers: {
        "Content-Type": "application/json",
        if (token != null) "Authorization": "Bearer $token",
      },
      body: jsonEncode(body),
    );

    return response;
  }

  // 🔹 Método PUT com autenticação automática
  static Future<http.Response> put(String endpoint, Map<String, dynamic> body) async {
    final token = await _getToken();
    final url = Uri.parse("$baseUrl$endpoint");

    print("📡 PUT: $url");
    print("📤 Enviando: ${jsonEncode(body)}");

    final response = await http.put(
      url,
      headers: {
        "Content-Type": "application/json",
        if (token != null) "Authorization": "Bearer $token",
      },
      body: jsonEncode(body),
    );

    return response;
  }

  // 🔹 Método DELETE com autenticação automática
  static Future<http.Response> delete(String endpoint) async {
    final token = await _getToken();
    final url = Uri.parse("$baseUrl$endpoint");

    print("📡 DELETE: $url");

    final response = await http.delete(
      url,
      headers: {
        "Content-Type": "application/json",
        if (token != null) "Authorization": "Bearer $token",
      },
    );

    return response;
  }
}
