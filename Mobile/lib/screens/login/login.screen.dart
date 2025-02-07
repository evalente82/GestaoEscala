import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:gestao_escala_mobile/screens/HomeScreen.dart';
import '../utils/app_styles.dart';
import '../widgets/custom_textfield.dart';
import '../widgets/custom_button.dart';
import 'esqueci_senha_screen.dart';
import 'primeiro_acesso_screen.dart';
import 'redefinir_senha_screen.dart';

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
  await Future.delayed(const Duration(seconds: 2));

  // Simulação de sucesso no login
  Navigator.pushReplacement(
    context,
    MaterialPageRoute(builder: (context) => const HomeScreen()),
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
                      Text("Defesa Civil de Maricá", style: AppStyles.title),
                      const SizedBox(height: 20),
                      CustomTextField(controller: _usuarioController, label: "Usuário"),
                      CustomTextField(controller: _senhaController, label: "Senha", isPassword: true),
                      if (_errorMessage != null)
                        Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(_errorMessage!, style: const TextStyle(color: Colors.red)),
                        ),
                      CustomButton(label: "Entrar", onTap: _login),
                      const SizedBox(height: 10),
                      TextButton(
                        onPressed: () {
                          Navigator.push(context, MaterialPageRoute(builder: (context) => const PrimeiroAcessoScreen()));
                        },
                        child: const Text("Primeiro Acesso"),
                      ),
                      TextButton(
                        onPressed: () {
                          Navigator.push(context, MaterialPageRoute(builder: (context) => const EsqueciSenhaScreen()));
                        },
                        child: const Text("Esqueci minha senha"),
                      ),
                      const SizedBox(height: 20),
                      TextButton(
                        onPressed: () {
                          Navigator.push(context, MaterialPageRoute(builder: (context) => const RedefinirSenhaScreen()));
                        },
                        child: const Text("Testar Redefinição de Senha", style: TextStyle(color: Colors.blue)),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const FooterWidget(), // Rodapé adicionado aqui
          ],
        ),
      ),
    );
  }
}

// 🔹 Componente de rodapé separado para reutilização futura
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
