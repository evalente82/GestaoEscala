import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../utils/app_styles.dart';
import '../widgets/custom_textfield.dart';
import '../widgets/custom_button.dart';

class RedefinirSenhaScreen extends StatefulWidget {
  const RedefinirSenhaScreen({super.key});

  @override
  RedefinirSenhaScreenState createState() => RedefinirSenhaScreenState();
}

class RedefinirSenhaScreenState extends State<RedefinirSenhaScreen> {
  final TextEditingController _senhaController = TextEditingController();
  final TextEditingController _confirmarSenhaController = TextEditingController();
  String? _errorMessage;

  void _redefinirSenha() async {
    String senha = _senhaController.text;
    String confirmarSenha = _confirmarSenhaController.text;

    if (senha.isEmpty || confirmarSenha.isEmpty) {
      setState(() {
        _errorMessage = "Preencha todos os campos!";
      });
      return;
    }

    if (senha != confirmarSenha) {
      setState(() {
        _errorMessage = "As senhas não coincidem!";
      });
      return;
    }

    await Future.delayed(const Duration(seconds: 2));

    setState(() {
      _errorMessage = "Senha redefinida com sucesso!";
    });

    Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.exit_to_app, color: Colors.red),
            onPressed: () {
              SystemNavigator.pop();
            },
          ),
        ],
      ),
      body: SafeArea(
        child: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                child: Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      SizedBox(height: MediaQuery.of(context).size.height * 0.05),
                      Image.asset("assets/images/LogoDefesaCivil.png", height: 200),
                      const SizedBox(height: 20),
                      Text("Redefinir Senha", style: AppStyles.title),
                      const SizedBox(height: 20),
                      CustomTextField(controller: _senhaController, label: "Nova Senha", isPassword: true),
                      CustomTextField(controller: _confirmarSenhaController, label: "Confirmar Nova Senha", isPassword: true),
                      if (_errorMessage != null)
                        Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(_errorMessage!, style: const TextStyle(color: Colors.red)),
                        ),
                      CustomButton(label: "Redefinir", onTap: _redefinirSenha),
                      TextButton(
                        onPressed: () => Navigator.pop(context),
                        child: const Text("Voltar ao Login"),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const FooterWidget(),
          ],
        ),
      ),
    );
  }
}

// 🔹 Componente de rodapé reutilizável
class FooterWidget extends StatelessWidget {
  const FooterWidget({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 10),
      color: Colors.white,
      child: Column(
        children: [
          const Text(
            "© 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS",
            style: TextStyle(fontSize: 12, color: Colors.black54),
            textAlign: TextAlign.center,
          ),
          const Text(
            "© Todos os direitos reservados à VCORP Sistem",
            style: TextStyle(fontSize: 12, color: Colors.black54),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
