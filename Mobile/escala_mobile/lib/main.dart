import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/screens/login/login_screen.dart';
import 'package:intl/intl.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:flutter_localizations/flutter_localizations.dart'; // Importe correto

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Inicializa os dados de localidade para pt_BR
  await initializeDateFormatting('pt_BR', null);

  final userModel = UserModel();
  await userModel.loadUserFromToken();
  runApp(
    ChangeNotifierProvider(
      create: (_) => userModel,
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Escala Mobile',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      supportedLocales: const [
        Locale('pt', 'BR'),
      ],
      locale: const Locale('pt', 'BR'),
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      home: const LoginScreen(),
    );
  }
}