import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../utils/app_styles.dart';
import '../widgets/custom_textfield.dart';
import '../widgets/custom_button.dart';
import 'redefinir_senha_screen.dart';

class EsqueciSenhaScreen extends StatefulWidget {
  const EsqueciSenhaScreen({super.key});

  @override
  EsqueciSenhaScreenState createState() => EsqueciSenhaScreenState();
}

class EsqueciSenhaScreenState extends State<EsqueciSenhaScreen> {
  final TextEditingController _emailController = TextEditingController();
  String? _errorMessage;

  void _enviarEmail() async {
    String email = _emailController.text;

    if (email.isEmpty) {
      setState(() {
        _errorMessage = "Preencha o campo de e-mail!";
      });
      return;
    }

    await Future.delayed(const Duration(seconds: 2));

    setState(() {
      _errorMessage = "E-mail enviado com sucesso!";
    });

    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const RedefinirSenhaScreen()),
    );
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
                      Text("Recuperação de Senha", style: AppStyles.title),
                      const SizedBox(height: 20),
                      CustomTextField(controller: _emailController, label: "E-mail"),
                      if (_errorMessage != null)
                        Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(_errorMessage!, style: const TextStyle(color: Colors.red)),
                        ),
                      CustomButton(label: "Enviar", onTap: _enviarEmail),
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
