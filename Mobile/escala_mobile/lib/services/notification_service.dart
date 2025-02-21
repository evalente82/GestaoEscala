import 'package:flutter_local_notifications/flutter_local_notifications.dart';

class NotificationService {
  static final FlutterLocalNotificationsPlugin _notificationsPlugin =
      FlutterLocalNotificationsPlugin();

  static Future<void> init() async {
    const AndroidInitializationSettings initializationSettingsAndroid =
        AndroidInitializationSettings('@mipmap/ic_launcher');
    const InitializationSettings initializationSettings = InitializationSettings(
      android: initializationSettingsAndroid,
    );
    await _notificationsPlugin.initialize(initializationSettings);
  }

  static Future<void> showNotification(String title, String body, int id) async {
    const AndroidNotificationDetails androidDetails = AndroidNotificationDetails(
      'permuta_channel',
      'Permutas',
      channelDescription: 'Notificações de permutas',
      importance: Importance.max,
      priority: Priority.high,
      showWhen: true,
    );
    const NotificationDetails notificationDetails =
        NotificationDetails(android: androidDetails);
    await _notificationsPlugin.show(id, title, body, notificationDetails);
  }

  static Future<void> updateBadge(int count) async {
    // Para Android, o badge é gerenciado pelo launcher; aqui usamos apenas a notificação
    // Para iOS, seria necessário integrar com flutter_app_badge
    await showNotification(
      'Nova notificação de permuta',
      'Você tem $count permutas pendentes.',
      0, // ID fixo para sobrescrever a notificação anterior
    );
  }
}