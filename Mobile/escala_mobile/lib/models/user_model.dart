import 'package:flutter/material.dart';
import 'package:escala_mobile/utils/jwt_utils.dart';
import 'package:escala_mobile/services/auth_service.dart';

class UserModel with ChangeNotifier {
  String _userName = "";
  String _userMatricula = "";
  String _idFuncionario = "";
  String _token = "";
  String _refreshToken = "";
  int _notificationCount = 0;

  String get userName => _userName;
  String get userMatricula => _userMatricula;
  String get idFuncionario => _idFuncionario;
  String get token => _token;
  String get refreshToken => _refreshToken;
  int get notificationCount => _notificationCount;

  void setUser(String name, String matricula, String idFuncionario, {String? token, String? refreshToken}) {
    _userName = name;
    _userMatricula = matricula;
    _idFuncionario = idFuncionario;
    _token = token ?? _token;
    _refreshToken = refreshToken ?? _refreshToken;
    notifyListeners();
  }

  Future<bool> loadUserFromToken() async {
    String? token = await AuthService.getToken();
    String? refreshToken = await AuthService.getRefreshToken();
    if (token != null && token.isNotEmpty && refreshToken != null && refreshToken.isNotEmpty) {
      final decodedToken = decodeJwt(token);
      if (decodedToken.isNotEmpty) {
        final exp = decodedToken['exp'] as int?;
        if (exp != null && DateTime.now().millisecondsSinceEpoch ~/ 1000 < exp) {
          _userName = decodedToken["unique_name"] ?? "";
          _userMatricula = decodedToken["Matricula"] ?? "";
          _idFuncionario = decodedToken["IdFuncionario"] ?? "";
          _token = token;
          _refreshToken = refreshToken;
          notifyListeners();
          return true;
        } else {
          return await _refreshUserToken();
        }
      }
    }
    return false;
  }

  Future<bool> _refreshUserToken() async {
    try {
      final response = await AuthService.refreshToken(_refreshToken);
      if (response["success"]) {
        final newToken = response["token"];
        final newRefreshToken = response["refreshToken"];
        await AuthService.saveToken(newToken);
        await AuthService.saveRefreshToken(newRefreshToken);
        final decodedToken = decodeJwt(newToken);
        _userName = decodedToken["unique_name"] ?? "";
        _userMatricula = decodedToken["Matricula"] ?? "";
        _idFuncionario = decodedToken["IdFuncionario"] ?? "";
        _token = newToken;
        _refreshToken = newRefreshToken;
        notifyListeners();
        return true;
      }
    } catch (e) {
      print("Erro ao renovar token: $e");
    }
    return false;
  }

  void clearUser() {
    _userName = "";
    _userMatricula = "";
    _idFuncionario = "";
    _token = "";
    _refreshToken = "";
    _notificationCount = 0;
    notifyListeners();
  }

  void incrementNotificationCount() {
    _notificationCount++;
    notifyListeners();
  }

  void clearNotificationCount() {
    _notificationCount = 0;
    notifyListeners();
  }
}