class AuthConfig {
  // Azure AD / Entra ID Configuration
  // Replace these with your actual values
  static const String clientId = 'YOUR_MOBILE_CLIENT_ID';
  static const String authority =
      'https://login.microsoftonline.com/YOUR_TENANT_ID';
  static const String redirectUri = 'msauth://com.orbitos.orbitos_mobile/callback';

  // API Configuration
  static const String apiBaseUrl = 'https://localhost:7001';
  static const List<String> apiScopes = [
    'api://YOUR_API_CLIENT_ID/access_as_user',
  ];

  // iOS specific
  static const String iosRedirectUri =
      'msauth.com.orbitos.orbitosmobile://auth';

  // Android specific
  static const String androidRedirectUri =
      'msauth://com.orbitos.orbitos_mobile/callback';
}
