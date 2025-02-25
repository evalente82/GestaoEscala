import 'package:escala_mobile/services/ApiClient.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:escala_mobile/models/user_model.dart';
import 'package:escala_mobile/screens/login/login_screen.dart';
import 'package:escala_mobile/screens/permutas/permuta_screen.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:escala_mobile/services/notification_service.dart';

// Handler para mensagens em background
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
      child: MyApp(initialRoute: isLoggedIn ? '/home' : '/login'),
    ),
  );
}

class MyApp extends StatefulWidget {
  final String initialRoute;

  const MyApp({super.key, required this.initialRoute});

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
      print("Notificação em foreground recebida: ${message.notification?.title}");
      final userModel = Provider.of<UserModel>(context, listen: false);
      userModel.incrementNotificationCount();
      NotificationService.showNotification(
        message.notification?.title ?? "Nova Permuta",
        message.notification?.body ?? "Você recebeu uma nova solicitação de permuta.",
        0,
      );
    });

    FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) {
      print("App aberto pela notificação: ${message.notification?.title}");
      final userModel = Provider.of<UserModel>(context, listen: false);
      Navigator.pushNamed(context, '/home');
      userModel.clearNotificationCount();
    });

    String? token = await messaging.getToken();
    print("FCM Token: $token");
    if (token != null) {
      await _sendFcmTokenToBackend(token);
    }
  }

  Future<void> _sendFcmTokenToBackend(String fcmToken) async {
    final userModel = Provider.of<UserModel>(context, listen: false);
    if (userModel.idFuncionario.isNotEmpty) {
      // Passe o endpoint como String, não como Uri
      await ApiClient.post(
        "/users/updateFcmToken",
        {
          "idFuncionario": userModel.idFuncionario,
          "fcmToken": fcmToken,
        },
      );
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
      initialRoute: widget.initialRoute,
      routes: {
        '/login': (context) => const LoginScreen(),
        '/home': (context) => const PermutaScreen(),
      },
    );
  }
}