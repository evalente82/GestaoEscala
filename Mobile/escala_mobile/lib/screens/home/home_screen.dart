import 'package:escala_mobile/screens/escalas/escala_screen.dart';
import 'package:escala_mobile/screens/permutas/permuta_screen.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart'; // Para usar SystemNavigator

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          // Espaço maior na parte superior
          const SizedBox(height: 20),
          // Área com Nome, Matrícula e Ícone de Sair
          Container(
            color: Colors.grey[200], // Fundo cinza claro
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 16.0),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                // Nome e Matrícula (Centralizado)
                Expanded(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Text(
                        "Bem vindo",
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Colors.black,
                        ),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 4),
                      Text(
                        "Endrigo Moura Valente Mat. 1234",
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.normal,
                          color: Colors.black,
                        ),
                        textAlign: TextAlign.center,
                        overflow: TextOverflow.visible,
                      ),
                    ],
                  ),
                ),
                // Ícone de Sair
                IconButton(
                  onPressed: _handleExitApp,
                  icon: Icon(Icons.logout, color: Colors.black),
                  splashRadius: 20,
                ),
              ],
            ),
          ),
          // Degradê que começa abaixo do texto e continua até o final da tela
          Expanded(
            child: Container(
              decoration: const BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    Color(0xFF003580), // Azul forte
                    Colors.white, // Branco
                  ],
                ),
              ),
              child: Column(
                children: [
                  // Logo da Defesa Civil
                  Expanded(
                        child: Align(
                          alignment: Alignment(0, -0.5), // Ajuste o valor de Y (-0.2, -0.3, etc.)
                          child: Image.asset(
                            "assets/images/LogoDefesaCivil.png",
                            height: 350,
                          ),
                        ),
                      ),
                  // Botões de Navegação
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 20),
                    child: Column(
                      children: [
                        ElevatedButton(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) => const EscalaScreen(),
                              ),
                            );
                          },
                          style: ElevatedButton.styleFrom(
                            minimumSize: Size(MediaQuery.of(context).size.width * 0.9, 60),
                            backgroundColor: const Color(0xFF003580),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10),
                            ),
                          ),
                          child: const Text(
                            "Escalas",
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: Colors.white,
                            ),
                          ),
                        ),
                        const SizedBox(height: 20),
                        ElevatedButton(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) => const PermutaScreen(),
                              ),
                            );
                          },
                          style: ElevatedButton.styleFrom(
                            minimumSize: Size(MediaQuery.of(context).size.width * 0.9, 60),
                            backgroundColor: const Color(0xFF003580),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10),
                            ),
                          ),
                          child: const Text(
                            "Permutas",
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Função para lidar com o fechamento do app
  Future<void> _handleExitApp() async {
    bool? shouldExit = await showDialog<bool>(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Sair"),
          content: const Text("Deseja realmente sair do aplicativo?"),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(false); // Cancelar
              },
              child: const Text("Cancelar"),
            ),
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(true); // Confirmar
              },
              child: const Text("Sair"),
            ),
          ],
        );
      },
    );

    // Verifica se o widget ainda está montado antes de usar o BuildContext
    if (shouldExit == true && mounted) {
      if (Theme.of(context).platform == TargetPlatform.iOS) {
        // Para iOS, exibe uma mensagem ou minimiza o app
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("O app foi minimizado.")),
        );
        // Coloca o app em segundo plano (minimiza)
        SystemChannels.platform.invokeMethod('SystemNavigator.pop');
      } else {
        // Para Android, fecha o app completamente
        SystemNavigator.pop();
      }
    }
  }
}