import 'package:flutter/material.dart';
import 'package:escala_mobile/utils/jwt_utils.dart'; // Decodificação do JWT
import 'package:escala_mobile/services/auth_service.dart';

class UserModel with ChangeNotifier {
  String _userName = ""; // Nome do usuário
  String _userMatricula = ""; // Matrícula do usuário
  String _idFuncionario = ""; // GUID do funcionário

  // Getters
  String get userName => _userName;
  String get userMatricula => _userMatricula;
  String get idFuncionario => _idFuncionario;

  // Método para atualizar os dados do usuário
  void setUser(String name, String matricula, String idFuncionario) {
    _userName = name;
    _userMatricula = matricula;
    _idFuncionario = idFuncionario;
    notifyListeners();
  }

  // Método para carregar os dados do usuário a partir do JWT salvo
  Future<void> loadUserFromToken() async {
    String? token = await AuthService.getToken();
    if (token != null && token.isNotEmpty) {
      final decodedToken = decodeJwt(token);
      if (decodedToken.isNotEmpty) {
        _userName = decodedToken["unique_name"] ?? ""; // Alterado para `unique_name`
        _userMatricula = decodedToken["Matricula"] ?? "";
        _idFuncionario = decodedToken["IdFuncionario"] ?? ""; 
        notifyListeners();
      }
    }
  }

  // Método para limpar os dados do usuário (logout)
  void clearUser() {
    _userName = "";
    _userMatricula = "";
    _idFuncionario = "";
    notifyListeners();
  }
}
