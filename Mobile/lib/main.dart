import 'package:flutter/material.dart';
import 'package:gestao_escala_mobile/screens/login/login.screen.dart'; // Caminho da tela de login

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false, // Remove o banner de debug
      title: 'Gestão de Escalas',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const LoginScreen(), // Agora inicia diretamente na tela de login
    );
  }
}
