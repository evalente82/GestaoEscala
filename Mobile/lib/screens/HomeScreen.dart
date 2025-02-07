import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../screens/escalaScreen/escalascreen.dart';
import '../screens/permutaScreen/permutaScreen.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // 🔹 Remove a cor de fundo do app bar para integrar ao degradê
      appBar: PreferredSize(
        preferredSize: const Size.fromHeight(70), // 🔹 Aumenta a altura do cabeçalho
        child: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [Color(0xFF1565C0), Color(0xFF64B5F6)],
            ),
          ),
          child: SafeArea(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  // 🔹 Texto fixo de boas-vindas
                  const Text(
                    "Bem vindo Endrigo Valente Mat. 1234",
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),

                  // 🔹 Botão de sair
                  IconButton(
                    icon: const Icon(Icons.exit_to_app, color: Colors.white),
                    onPressed: () {
                      SystemNavigator.pop(); // Fecha o aplicativo
                    },
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
      body: Stack(
        children: [
          // 🔹 Fundo com degradê azul -> branco
          Positioned.fill(
            child: Container(
              decoration: const BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    Color(0xFF1565C0), // Azul forte
                    Color(0xFF64B5F6), // Azul claro
                    Colors.white, // Branco no final
                  ],
                ),
              ),
            ),
          ),

          // 🔹 Conteúdo principal
          Column(
            children: [
              // 🔹 Logo da Defesa Civil
              Padding(
                padding: const EdgeInsets.only(top: 30),
                child: Center(
                  child: Image.asset(
                    "assets/images/LogoDefesaCivil.png",
                    height: 300, // 🔹 Tamanho ajustado
                  ),
                ),
              ),
              const Spacer(), // 🔹 Adiciona espaçamento automático

              // 🔹 Botões de menu ajustados
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 30), // 🔹 Ajuste da margem lateral
                child: Column(
                  children: [
                    _buildMenuButton(
                      context,
                      label: "Escala",
                      icon: Icons.schedule,
                      screen: const EscalaScreen(),
                    ),
                    const SizedBox(height: 20), // 🔹 Espaço entre os botões
                    _buildMenuButton(
                      context,
                      label: "Permutas",
                      icon: Icons.swap_horiz,
                      screen: const PermutaScreen(),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 30), // 🔹 Espaço antes do rodapé
              const FooterWidget(), // 🔹 Rodapé fixo
            ],
          ),
        ],
      ),
    );
  }

  // 🔹 Método auxiliar para criar botões de menu maiores e ajustados
  Widget _buildMenuButton(BuildContext context, {required String label, required IconData icon, required Widget screen}) {
    return SizedBox(
      width: double.infinity, // 🔹 Ocupa toda a largura disponível
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.orange,
          padding: const EdgeInsets.symmetric(vertical: 18), // 🔹 Altura maior para melhor toque
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        ),
        onPressed: () {
          Navigator.push(context, MaterialPageRoute(builder: (context) => screen));
        },
        icon: Icon(icon, color: Colors.white, size: 28), // 🔹 Ícone maior
        label: Text(label, style: const TextStyle(color: Colors.white, fontSize: 18, fontWeight: FontWeight.bold)),
      ),
    );
  }
}

// 🔹 Rodapé reutilizável
class FooterWidget extends StatelessWidget {
  const FooterWidget({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 10),
      color: Colors.white,
      child: Column(
        children: const [
          Text(
            "© 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS",
            style: TextStyle(fontSize: 12, color: Colors.black54),
            textAlign: TextAlign.center,
          ),
          Text(
            "© Todos os direitos reservados à VCORP Sistem",
            style: TextStyle(fontSize: 12, color: Colors.black54),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
