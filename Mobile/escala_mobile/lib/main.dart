import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/screens/login/login_screen.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';

// Handler para mensagens em background
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();
  print("Notificação em background recebida: ${message.notification?.title}");
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Inicializa Firebase
  await Firebase.initializeApp();

  // Inicializa localidade para pt_BR
  await initializeDateFormatting('pt_BR', null);

  // Configura handler para mensagens em background
  FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

  final userModel = UserModel();
  await userModel.loadUserFromToken();

  runApp(
    ChangeNotifierProvider(
      create: (_) => userModel,
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  _MyAppState createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  @override
  void initState() {
    super.initState();
    _setupFirebaseMessaging();
  }

  void _setupFirebaseMessaging() async {
    FirebaseMessaging messaging = FirebaseMessaging.instance;

    // Solicita permissão para notificações (iOS requer isso, Android é automático)
    await messaging.requestPermission();

    // Escuta mensagens em foreground
    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      print("Notificação em foreground recebida: ${message.notification?.title}");
      // Aqui você pode atualizar o estado do app (ex.: contador de notificações)
    });

    // Escuta quando o app é aberto a partir de uma notificação
    FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) {
      print("App aberto pela notificação: ${message.notification?.title}");
    });

    // Obtém o token FCM para enviar notificações ao dispositivo
    String? token = await messaging.getToken();
    print("FCM Token: $token");
    // Salve este token no backend associado ao usuário para enviar notificações específicas
  }

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