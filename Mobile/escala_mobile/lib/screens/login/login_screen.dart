import 'package:flutter/material.dart';
import 'package:escala_mobile/screens/home/home_screen.dart';
import 'package:escala_mobile/screens/login/primeiro_acesso_screen.dart';
import 'package:escala_mobile/screens/login/esqueci_senha_screen.dart';
import 'package:escala_mobile/screens/login/redefinir_senha.dart'; // Importe a tela de Redefinir Senha
import 'package:escala_mobile/components/footer_component.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final TextEditingController _usuarioController = TextEditingController();
  final TextEditingController _senhaController = TextEditingController();
  String? _alertMessage;

  Future<void> _handleSubmit() async {
    final String usuario = _usuarioController.text.trim();
    final String senha = _senhaController.text.trim();

    // Verifica se os campos estão preenchidos
    if (usuario.isEmpty || senha.isEmpty) {
      setState(() {
        _alertMessage = "Preencha todos os campos!";
      });
      return;
    }

    // Simula autenticação bem-sucedida
    setState(() {
      _alertMessage = null; // Limpa mensagens de erro
    });

    // Navega para a tela inicial (HomeScreen)
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (context) => const HomeScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF7F9FC), // Cor de fundo suave
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const SizedBox(height: 50),
              Text(
                "Prefeitura Municipal de Maricá",
                textAlign: TextAlign.center, // Centraliza o texto
                style: TextStyle(
                  fontSize: 28, // Tamanho maior
                  fontWeight: FontWeight.bold,
                  color: const Color(0xFF003580), // Azul forte
                ),
              ),
              const SizedBox(height: 20),
              Image.asset(
                "assets/images/LogoDefesaCivil.png",
                height: 150,
              ),
              const SizedBox(height: 20),
              Text(
                "Login",
                style: TextStyle(
                  fontSize: 22, // Tamanho maior
                  fontWeight: FontWeight.bold,
                  color: const Color(0xFF003580), // Azul forte
                ),
              ),
              const SizedBox(height: 20),
              TextField(
                controller: _usuarioController,
                decoration: InputDecoration(
                  labelText: "Usuário",
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: _senhaController,
                obscureText: true,
                decoration: InputDecoration(
                  labelText: "Senha",
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _handleSubmit,
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size(double.infinity, 50),
                  backgroundColor: const Color(0xFF003580), // Azul forte
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
                child: const Text(
                  "Entrar",
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Colors.white, // Define a cor do texto como branco
                  ),
                ),
              ),
              const SizedBox(height: 20),
              if (_alertMessage != null)
                Container(
                  margin: const EdgeInsets.only(bottom: 16),
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.red[100],
                    border: Border.all(color: Colors.red),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    _alertMessage!,
                    style: const TextStyle(color: Colors.red, fontSize: 14),
                    textAlign: TextAlign.center,
                  ),
                ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  TextButton(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const PrimeiroAcessoScreen(),
                        ),
                      );
                    },
                    child: const Text(
                      "Primeiro Acesso",
                      style: TextStyle(fontSize: 16, color: Color(0xFF003580)),
                    ),
                  ),
                  TextButton(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => EsqueciSenhaScreen(),
                        ),
                      );
                    },
                    child: const Text(
                      "Esqueci minha senha",
                      style: TextStyle(fontSize: 16, color: Color(0xFF003580)),
                    ),
                  ),
                ],
              ),
              // Link temporário para Redefinir Senha
              Padding(
                padding: const EdgeInsets.only(top: 16),
                child: Align(
                  alignment: Alignment.center,
                  child: TextButton(
                    onPressed: () {
                      // Navega para a tela de Redefinir Senha
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => RedefinirSenhaScreen(token: "temp_token"), // Token temporário
                        ),
                      );
                    },
                    child: const Text(
                      "Redefinir Senha (Temporário)",
                      style: TextStyle(
                        fontSize: 16,
                        color: Colors.blue,
                        decoration: TextDecoration.underline, // Sublinha o texto
                      ),
                    ),
                  ),
                ),
              ),
              // Rodapé
              const FooterComponent(),
            ],
          ),
        ),
      ),
    );
  }
}