import 'package:flutter/material.dart';

class EscalaScreen extends StatelessWidget {
  const EscalaScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Escala"),
        centerTitle: true,
      ),
      body: const Center(
        child: Text(
          "Em Construção...",
          style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
