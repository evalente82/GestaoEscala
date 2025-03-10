import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      pt: {
        translation: {
          // NavBar
          "Departamentos": "Departamentos",
          "Departamento": "Departamento",
          "Cargo": "Cargo",
          "Funcionários": "Funcionários",
          "Configurações": "Configurações",
          "Setor": "Setor",
          "Postos": "Postos",
          "Tipo Escala": "Tipo Escala",
          "Escalas": "Escalas",
          "Permutas": "Permutas",
          "Perfis e Funcionalidades": "Perfis e Funcionalidades",
          "Perfil": "Perfil",
          "Funcionalidade": "Funcionalidade",
          "Perfil Funcionalidade": "Perfil Funcionalidade",
          "Cargo Perfis": "Cargo Perfis",
          "Sair": "Sair",
          "Usuário": "Usuário",
          "footer.copyright1": "© 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS",
          "footer.copyright2": "© Todos os direitos reservados à VCORP Sistem",

          // Login
          "main_title": "Prefeitura Municipal de Maricá",
          "login_title": "Defesa Civil de Maricá",
          "Login": "Login",
          "Usuário": "Usuário",
          "Senha": "Senha",
          "Primeiro Acesso": "Primeiro Acesso",
          "Esqueci minha senha": "Esqueci minha senha",
          "Entrar": "Entrar",
          "Erro ao fazer login": "Erro ao fazer login. Tente novamente."
        }
      },
      en: {
        translation: {
          // NavBar
          "Departamentos": "Departments",
          "Departamento": "Department",
          "Cargo": "Role",
          "Funcionários": "Employees",
          "Configurações": "Settings",
          "Setor": "Sector",
          "Postos": "Posts",
          "Tipo Escala": "Shift Type",
          "Escalas": "Shifts",
          "Permutas": "Swaps",
          "Perfis e Funcionalidades": "Profiles and Features",
          "Perfil": "Profile",
          "Funcionalidade": "Feature",
          "Perfil Funcionalidade": "Profile Feature",
          "Cargo Perfis": "Role Profiles",
          "Sair": "Logout",
          "Usuário": "User",
          "footer.copyright1": "© 2023 - MARICÁ CIVIL DEFENSE SHIFT CONTROL",
          "footer.copyright2": "© All rights reserved to VCORP Sistem",

          // Login
          "main_title": "Maricá Municipal Prefecture",
          "login_title": "Maricá Civil Defense",
          "Login": "Login",
          "Usuário": "Username",
          "Senha": "Password",
          "Primeiro Acesso": "First Access",
          "Esqueci minha senha": "Forgot my password",
          "Entrar": "Sign In",
          "Erro ao fazer login": "Error logging in. Try again."
        }
      },
      fr: {
        translation: {
          // NavBar
          "Departamentos": "Départements",
          "Departamento": "Département",
          "Cargo": "Poste",
          "Funcionários": "Employés",
          "Configurações": "Paramètres",
          "Setor": "Secteur",
          "Postos": "Postes",
          "Tipo Escala": "Type d'échelle",
          "Escalas": "Échelles",
          "Permutas": "Permutations",
          "Perfis e Funcionalidades": "Profils et Fonctionnalités",
          "Perfil": "Profil",
          "Funcionalidade": "Fonctionnalité",
          "Perfil Funcionalidade": "Fonctionnalité de Profil",
          "Cargo Perfis": "Profils de Poste",
          "Sair": "Déconnexion",
          "Usuário": "Utilisateur",
          "footer.copyright1": "© 2023 - CONTRÔLE DES ÉCHELLES DE LA DÉFENSE CIVILE DE MARICÁ",
          "footer.copyright2": "© Tous droits réservés à VCORP Sistem",

          // Login
          "main_title": "Préfecture Municipale de Maricá",
          "login_title": "Défense Civile de Maricá",
          "Login": "Connexion",
          "Usuário": "Nom d'utilisateur",
          "Senha": "Mot de passe",
          "Primeiro Acesso": "Premier Accès",
          "Esqueci minha senha": "Mot de passe oublié",
          "Entrar": "Se connecter",
          "Erro ao fazer login": "Erreur de connexion. Réessayez."
        }
      },
      es: {
        translation: {
          // NavBar
          "Departamentos": "Departamentos",
          "Departamento": "Departamento",
          "Cargo": "Cargo",
          "Funcionários": "Empleados",
          "Configurações": "Configuraciones",
          "Setor": "Sector",
          "Postos": "Puestos",
          "Tipo Escala": "Tipo de Escala",
          "Escalas": "Escalas",
          "Permutas": "Permutas",
          "Perfis e Funcionalidades": "Perfiles y Funcionalidades",
          "Perfil": "Perfil",
          "Funcionalidade": "Funcionalidad",
          "Perfil Funcionalidade": "Funcionalidad de Perfil",
          "Cargo Perfis": "Perfiles de Cargo",
          "Sair": "Salir",
          "Usuário": "Usuario",
          "footer.copyright1": "© 2023 - CONTROL DE ESCALAS DE DEFENSA CIVIL DE MARICÁ",
          "footer.copyright2": "© Todos los derechos reservados a VCORP Sistem",

          // Login
          "main_title": "Prefectura Municipal de Maricá",
          "login_title": "Defensa Civil de Maricá",
          "Login": "Inicio de sesión",
          "Usuário": "Usuario",
          "Senha": "Contraseña",
          "Primeiro Acesso": "Primer Acceso",
          "Esqueci minha senha": "Olvidé mi contraseña",
          "Entrar": "Entrar",
          "Erro ao fazer login": "Error al iniciar sesión. Intenta de nuevo."
        }
      },
      de: {
        translation: {
          // NavBar
          "Departamentos": "Abteilungen",
          "Departamento": "Abteilung",
          "Cargo": "Position",
          "Funcionários": "Mitarbeiter",
          "Configurações": "Einstellungen",
          "Setor": "Sektor",
          "Postos": "Posten",
          "Tipo Escala": "Schichttyp",
          "Escalas": "Schichten",
          "Permutas": "Tausch",
          "Perfis e Funcionalidades": "Profile und Funktionen",
          "Perfil": "Profil",
          "Funcionalidade": "Funktion",
          "Perfil Funcionalidade": "Profilfunktion",
          "Cargo Perfis": "Positionsprofile",
          "Sair": "Abmelden",
          "Usuário": "Benutzer",
          "footer.copyright1": "© 2023 - MARICÁ ZIVILSCHUTZ SCHICHTKONTROLLE",
          "footer.copyright2": "© Alle Rechte vorbehalten an VCORP Sistem",

          // Login
          "main_title": "Städtische Präfektur Maricá",
          "login_title": "Zivilschutz Maricá",
          "Login": "Anmeldung",
          "Usuário": "Benutzername",
          "Senha": "Passwort",
          "Primeiro Acesso": "Erster Zugriff",
          "Esqueci minha senha": "Passwort vergessen",
          "Entrar": "Einloggen",
          "Erro ao fazer login": "Fehler beim Anmelden. Versuchen Sie es erneut."
        }
      }
    },
    lng: "pt", // Idioma padrão: Português (Brasil)
    fallbackLng: "en", // Caso o idioma não esteja disponível, usa inglês
    interpolation: {
      escapeValue: false, // React já escapa valores
    },
  });

export default i18n;