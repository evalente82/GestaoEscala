import 'package:flutter/material.dart';
import 'package:escala_mobile/services/auth_service.dart';
import 'package:escala_mobile/components/footer_component.dart';

class RedefinirSenhaScreen extends StatefulWidget {
  final String token;
  const RedefinirSenhaScreen({super.key, required this.token});

  @override
  State<RedefinirSenhaScreen> createState() => _RedefinirSenhaScreenState();
}

class _RedefinirSenhaScreenState extends State<RedefinirSenhaScreen> {
  final TextEditingController _novaSenhaController = TextEditingController();
  final TextEditingController _confirmarSenhaController = TextEditingController();
  String? _alertMessage;
  String? _successMessage;

  Future<void> _handleRedefinirSenha() async {
    final String novaSenha = _novaSenhaController.text.trim();
    final String confirmarSenha = _confirmarSenhaController.text.trim();

    // Verifica se os campos estão preenchidos
    if (novaSenha.isEmpty || confirmarSenha.isEmpty) {
      setState(() {
        _alertMessage = "Preencha todos os campos!";
        _successMessage = null;
      });
      return;
    }

    // Verifica se as senhas coincidem
    if (novaSenha != confirmarSenha) {
      setState(() {
        _alertMessage = "As senhas não coincidem!";
        _successMessage = null;
      });
      return;
    }

    setState(() {
      _alertMessage = null;
      _successMessage = "Redefinindo senha...";
    });

    try {
      final response = await AuthService.resetPasswordWithToken(widget.token, novaSenha);

      // Verifica se o widget ainda está montado antes de usar o BuildContext
      if (!mounted) return;

      if (response["success"]) {
        setState(() {
          _successMessage = response["message"];
          _alertMessage = null;
        });
        await Future.delayed(const Duration(seconds: 3)); // Aguarda 3 segundos

        // Verifica novamente se o widget ainda está montado antes de navegar
        if (!mounted) return;

        Navigator.pop(context); // Volta para a tela de login
      } else {
        setState(() {
          _alertMessage = response["message"] ?? "Erro ao redefinir senha.";
          _successMessage = null;
        });
      }
    } catch (error) {
      // Verifica se o widget ainda está montado antes de usar o BuildContext
      if (!mounted) return;

      setState(() {
        _alertMessage = "Erro ao conectar ao servidor. Tente novamente.";
        _successMessage = null;
      });
    }
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
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 28,
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
                "Redefinir Senha",
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: const Color(0xFF003580), // Azul forte
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
              if (_successMessage != null)
                Container(
                  margin: const EdgeInsets.only(bottom: 16),
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.green[100],
                    border: Border.all(color: Colors.green),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    _successMessage!,
                    style: const TextStyle(color: Colors.green, fontSize: 14),
                    textAlign: TextAlign.center,
                  ),
                ),
              TextField(
                controller: _novaSenhaController,
                obscureText: true,
                decoration: InputDecoration(
                  labelText: "Nova Senha",
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: _confirmarSenhaController,
                obscureText: true,
                decoration: InputDecoration(
                  labelText: "Confirmar Nova Senha",
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _handleRedefinirSenha,
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size(double.infinity, 50),
                  backgroundColor: const Color(0xFF003580), // Azul forte
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10), // Bordas arredondadas
                  ),
                ),
                child: const Text(
                  "Redefinir Senha",
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Colors.white, // Texto branco
                  ),
                ),
              ),
              const SizedBox(height: 20),
              TextButton(
                onPressed: () {
                  Navigator.pop(context); // Volta para a tela de login
                },
                child: const Text(
                  "Voltar para Login",
                  style: TextStyle(fontSize: 16, color: Colors.blue),
                ),
              ),
              const FooterComponent(),
            ],
          ),
        ),
      ),
    );
  }
}