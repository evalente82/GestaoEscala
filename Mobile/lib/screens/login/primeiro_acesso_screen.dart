import 'package:flutter/material.dart';

class PrimeiroAcessoScreen extends StatelessWidget {
  const PrimeiroAcessoScreen({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text("Primeiro Acesso")),
      body: Center(child: Text("Tela de Primeiro Acesso")),
    );
  }
}
