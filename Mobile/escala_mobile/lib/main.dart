import 'package:escala_mobile/services/ApiClient.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/screens/login/login_screen.dart';
import 'package:escala_mobile/screens/home/home_screen.dart'; // Importe a HomeScreen
import 'package:escala_mobile/screens/permutas/permuta_screen.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:escala_mobile/services/notification_service.dart';

Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();
  print("Notificação em background recebida: ${message.notification?.title}");
  await NotificationService.showNotification(
    message.notification?.title ?? "Nova Permuta",
    message.notification?.body ?? "Você recebeu uma nova solicitação de permuta.",
    0,
  );
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp();
  await initializeDateFormatting('pt_BR', null);
  await NotificationService.init();

  FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

  final userModel = UserModel();
  bool isLoggedIn = await userModel.loadUserFromToken();

  runApp(
    ChangeNotifierProvider(
      create: (_) => userModel,
      child: MyApp(isLoggedIn: isLoggedIn),
    ),
  );
}

class MyApp extends StatefulWidget {
  final bool isLoggedIn;

  const MyApp({super.key, required this.isLoggedIn});

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
    await messaging.requestPermission();

    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      final userModel = Provider.of<UserModel>(context, listen: false);
      userModel.incrementNotificationCount();
      NotificationService.showNotification(
        message.notification?.title ?? "Nova Permuta",
        message.notification?.body ?? "Você recebeu uma nova solicitação de permuta.",
        0,
      );
    });

    FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) {
      final userModel = Provider.of<UserModel>(context, listen: false);
      Navigator.pushNamed(context, '/home');
      userModel.clearNotificationCount();
    });

    String? fcmToken = await messaging.getToken();
    if (fcmToken != null) {
      await _sendFcmTokenToBackend(fcmToken);
    }
  }

  Future<void> _sendFcmTokenToBackend(String fcmToken) async {
    final userModel = Provider.of<UserModel>(context, listen: false);
    if (userModel.idFuncionario.isNotEmpty) {
      final response = await ApiClient.post(
        "/users/updateFcmToken",
        {
          "idFuncionario": userModel.idFuncionario,
          "fcmToken": fcmToken,
        },
      );
      print("📡 FCM Token enviado: ${response["statusCode"]} - ${response["body"]}");
    }
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Escala Mobile',
      theme: ThemeData(primarySwatch: Colors.blue),
      supportedLocales: const [Locale('pt', 'BR')],
      locale: const Locale('pt', 'BR'),
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      home: widget.isLoggedIn ? const HomeScreen() : const LoginScreen(), // Alterado para HomeScreen
      routes: {
        '/login': (context) => const LoginScreen(),
        '/home': (context) => const HomeScreen(), // Rota ajustada para HomeScreen
        '/permutas': (context) => const PermutaScreen(), // Adicionei uma rota para PermutaScreen
      },
    );
  }
}