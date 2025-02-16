import 'dart:convert';

Map<String, dynamic> decodeJwt(String token) {
  try {
    final parts = token.split('.');
    if (parts.length != 3) return {};
    final payload = utf8.decode(base64Url.decode(parts[1]));
    return jsonDecode(payload);
  } catch (e) {
    return {};
  }
}

bool hasPermission(String permission, String token) {
  final decodedToken = decodeJwt(token);
  final permissions = List<String>.from(decodedToken['Permissao'] ?? []);
  return permissions.contains(permission);
}