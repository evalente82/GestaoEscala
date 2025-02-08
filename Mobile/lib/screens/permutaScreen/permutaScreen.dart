import 'package:flutter/material.dart';

class PermutaScreen extends StatelessWidget {
  const PermutaScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Permutas"),
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
