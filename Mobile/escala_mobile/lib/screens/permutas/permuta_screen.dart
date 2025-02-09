import 'package:flutter/material.dart';

class PermutaScreen extends StatelessWidget {
  const PermutaScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Página Inicial"),
      ),
      body: const Center(
        child: Text("Bem-vindo à Página Inicial!"),
      ),
    );
  }
}