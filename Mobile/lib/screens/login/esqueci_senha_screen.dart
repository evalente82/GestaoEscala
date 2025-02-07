import 'package:flutter/material.dart';
import 'redefinir_senha_screen.dart';

class EsqueciSenhaScreen extends StatelessWidget {
 const EsqueciSenhaScreen({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text("Esqueci Minha Senha")),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text("Digite seu email para redefinir a senha."),
            ElevatedButton(
              onPressed: () {
                Navigator.push(context, MaterialPageRoute(builder: (context) => RedefinirSenhaScreen()));
              },
              child: Text("Redefinir Senha"),
            ),
          ],
        ),
      ),
    );
  }
}
