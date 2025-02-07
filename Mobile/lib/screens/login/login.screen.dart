import 'package:flutter/material.dart';
import '../utils/app_styles.dart';
import '../widgets/custom_textfield.dart';
import '../widgets/custom_button.dart';
import 'esqueci_senha_screen.dart';
import 'primeiro_acesso_screen.dart';
import 'dart:developer';


class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});
  @override
  LoginScreenState createState() => LoginScreenState();
}

class LoginScreenState extends State<LoginScreen> {
  final TextEditingController _usuarioController = TextEditingController();
  final TextEditingController _senhaController = TextEditingController();
  String? _errorMessage;

  void _login() async {
    String usuario = _usuarioController.text;
    String senha = _senhaController.text;

    if (usuario.isEmpty || senha.isEmpty) {
      setState(() {
        _errorMessage = "Preencha todos os campos!";
      });
      return;
    }

    // Simulação de requisição à API
    await Future.delayed(Duration(seconds: 2));

    // Aqui entraria a lógica para chamar a API e autenticar o usuário
    log("Usuário: $usuario | Senha: $senha");("Usuário: $usuario | Senha: $senha");

    // Se der erro, exiba uma mensagem
    setState(() {
      _errorMessage = "Erro ao fazer login, tente novamente.";
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Image.asset("assets/logo.png", height: 100),
              SizedBox(height: 20),
              Text("Defesa Civil de Maricá", style: AppStyles.title),
              SizedBox(height: 20),
              CustomTextField(controller: _usuarioController, label: "Usuário"),
              CustomTextField(controller: _senhaController, label: "Senha", isPassword: true),
              if (_errorMessage != null)
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text(_errorMessage!, style: TextStyle(color: Colors.red)),
                ),
              CustomButton(label: "Entrar", onTap: _login),
              SizedBox(height: 10),
              TextButton(
                onPressed: () {
                  Navigator.push(context, MaterialPageRoute(builder: (context) => PrimeiroAcessoScreen()));
                },
                child: Text("Primeiro Acesso"),
              ),
              TextButton(
                onPressed: () {
                  Navigator.push(context, MaterialPageRoute(builder: (context) => EsqueciSenhaScreen()));
                },
                child: Text("Esqueci minha senha"),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
