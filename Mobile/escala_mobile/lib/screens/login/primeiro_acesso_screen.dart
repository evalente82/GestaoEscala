import 'package:flutter/material.dart';
import 'package:escala_mobile/services/auth_service.dart';
import 'package:escala_mobile/components/footer_component.dart';

class PrimeiroAcessoScreen extends StatefulWidget {
  const PrimeiroAcessoScreen({super.key});

  @override
  State<PrimeiroAcessoScreen> createState() => _PrimeiroAcessoScreenState();
}

class _PrimeiroAcessoScreenState extends State<PrimeiroAcessoScreen> {
  final TextEditingController _usuarioController = TextEditingController();
  final TextEditingController _senhaController = TextEditingController();
  final TextEditingController _confirmarSenhaController = TextEditingController();
  String? _alertMessage;
  String? _successMessage;

  // Função para validar o formato do e-mail
  bool _isValidEmail(String email) {
    final emailRegex = RegExp(r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$');
    return emailRegex.hasMatch(email);
  }

  Future<void> _handleSubmit() async {
    final String usuario = _usuarioController.text.trim();
    final String senha = _senhaController.text.trim();
    final String confirmarSenha = _confirmarSenhaController.text.trim();

    // Validação dos campos
    if (usuario.isEmpty || senha.isEmpty || confirmarSenha.isEmpty) {
      setState(() {
        _alertMessage = "Preencha todos os campos!";
        _successMessage = null;
      });
      return;
    }

    // Validação do e-mail
    if (!_isValidEmail(usuario)) {
      setState(() {
        _alertMessage = "E-mail inválido! Insira um e-mail no formato correto.";
        _successMessage = null;
      });
      return;
    }

    // Validação das senhas
    if (senha != confirmarSenha) {
      setState(() {
        _alertMessage = "As senhas não coincidem.";
        _successMessage = null;
      });
      return;
    }

    setState(() {
      _alertMessage = null;
      _successMessage = "Criando acesso...";
    });

    try {
      final response = await AuthService.register(usuario, senha);

      if (!mounted) return; // Verifica se o widget ainda está montado

      if (response["success"]) {
        setState(() {
          _successMessage = "Cadastro realizado com sucesso! Redirecionando...";
          _alertMessage = null;
        });
        await Future.delayed(const Duration(seconds: 3)); // Aguarda 3 segundos

        if (!mounted) return; // Verifica novamente antes de navegar

        Navigator.pop(context); // Volta para a tela de login
      } else {
        setState(() {
          _alertMessage = response["message"] ?? "Erro ao criar acesso.";
          _successMessage = null;
        });
      }
    } catch (error) {
      if (!mounted) return; // Verifica se o widget ainda está montado

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
                "Primeiro Acesso",
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
                controller: _usuarioController,
                keyboardType: TextInputType.emailAddress,
                decoration: InputDecoration(
                  labelText: "E-mail",
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
              const SizedBox(height: 16),
              TextField(
                controller: _confirmarSenhaController,
                obscureText: true,
                decoration: InputDecoration(
                  labelText: "Confirmar Senha",
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
                  "Criar Acesso",
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