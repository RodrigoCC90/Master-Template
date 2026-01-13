# OrbitOS Authentication Setup

This document describes how to configure Microsoft Identity Platform (Entra ID) authentication across all OrbitOS applications.

## Azure AD App Registrations

You need to create **two** app registrations in the Azure Portal:

### 1. API App Registration (Backend)

1. Go to Azure Portal → Microsoft Entra ID → App registrations → New registration
2. Name: `OrbitOS API`
3. Supported account types: Choose based on your needs:
   - Single tenant: Only users in your organization
   - Multitenant: Users from any organization
4. Redirect URI: Leave blank (API doesn't need one)
5. After creation:
   - Go to "Expose an API"
   - Set Application ID URI: `api://YOUR_CLIENT_ID`
   - Add a scope: `access_as_user` (Admin and users can consent)

**Values to note:**
- Application (client) ID → `YOUR_API_CLIENT_ID`
- Directory (tenant) ID → `YOUR_TENANT_ID`

### 2. Client App Registration (Web & Mobile)

1. Go to Azure Portal → Microsoft Entra ID → App registrations → New registration
2. Name: `OrbitOS Client`
3. Supported account types: Same as API
4. Redirect URIs:
   - Web: `http://localhost:3000` (development)
   - Mobile (iOS): `msauth.com.orbitos.orbitosmobile://auth`
   - Mobile (Android): `msauth://com.orbitos.orbitos_mobile/callback`
5. After creation:
   - Go to "API permissions"
   - Add permission → My APIs → OrbitOS API → `access_as_user`
   - Grant admin consent (if required)

**Values to note:**
- Application (client) ID → `YOUR_CLIENT_ID`

---

## Configuration Files

### Backend (.NET API)

File: `orbitos-api/src/OrbitOS.Api/appsettings.json`

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_API_CLIENT_ID",
    "Audience": "api://YOUR_API_CLIENT_ID",
    "Scopes": "access_as_user"
  }
}
```

### Frontend (Nuxt Web)

File: `orbitos-web/.env`

```env
NUXT_PUBLIC_MSAL_CLIENT_ID=YOUR_CLIENT_ID
NUXT_PUBLIC_MSAL_AUTHORITY=https://login.microsoftonline.com/YOUR_TENANT_ID
NUXT_PUBLIC_MSAL_REDIRECT_URI=http://localhost:3000
NUXT_PUBLIC_API_BASE_URL=https://localhost:7001
NUXT_PUBLIC_API_SCOPES=api://YOUR_API_CLIENT_ID/access_as_user
```

### Mobile (Flutter)

File: `orbitos-mobile/lib/config/auth_config.dart`

```dart
class AuthConfig {
  static const String clientId = 'YOUR_CLIENT_ID';
  static const String authority = 'https://login.microsoftonline.com/YOUR_TENANT_ID';
  static const String apiBaseUrl = 'https://your-api-url.com';
  static const List<String> apiScopes = [
    'api://YOUR_API_CLIENT_ID/access_as_user',
  ];
}
```

---

## iOS Configuration

Add to `orbitos-mobile/ios/Runner/Info.plist`:

```xml
<key>CFBundleURLTypes</key>
<array>
  <dict>
    <key>CFBundleURLSchemes</key>
    <array>
      <string>msauth.com.orbitos.orbitosmobile</string>
    </array>
  </dict>
</array>
<key>LSApplicationQueriesSchemes</key>
<array>
  <string>msauthv2</string>
  <string>msauthv3</string>
</array>
```

## Android Configuration

Add to `orbitos-mobile/android/app/src/main/AndroidManifest.xml` inside `<application>`:

```xml
<activity
    android:name="com.microsoft.identity.client.BrowserTabActivity">
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data
            android:scheme="msauth"
            android:host="com.orbitos.orbitos_mobile"
            android:path="/callback" />
    </intent-filter>
</activity>
```

---

## Testing the Setup

### 1. Start the API

```bash
cd orbitos-api
dotnet run --project src/OrbitOS.Api
```

### 2. Test unauthenticated endpoint

```bash
curl https://localhost:7001/api/auth/ping
```

### 3. Start the Web app

```bash
cd orbitos-web
npm install
npm run dev
```

### 4. Sign in and test authenticated endpoint

After signing in via the web UI, the dashboard has a "Test API Connection" button that calls the authenticated `/api/auth/ping-auth` endpoint.

---

## Troubleshooting

### Token validation errors
- Ensure the API's `Audience` matches the client's scope prefix (`api://YOUR_API_CLIENT_ID`)
- Verify the tenant ID matches across all configurations

### CORS errors
- Update `Cors:AllowedOrigins` in the API's `appsettings.json`

### Mobile redirect errors
- Ensure redirect URIs are registered in the Azure Portal
- Verify the bundle/package ID matches the redirect URI scheme
