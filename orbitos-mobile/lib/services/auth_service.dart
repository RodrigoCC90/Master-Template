import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:msal_flutter/msal_flutter.dart';
import '../config/auth_config.dart';

class AuthService {
  PublicClientApplication? _pca;
  String? _accessToken;
  String? _accountId;

  Future<void> initialize() async {
    try {
      _pca = await PublicClientApplication.createPublicClientApplication(
        AuthConfig.clientId,
        authority: AuthConfig.authority,
        iosRedirectUri: AuthConfig.iosRedirectUri,
        androidRedirectUri: AuthConfig.androidRedirectUri,
      );
    } catch (e) {
      debugPrint('MSAL initialization error: $e');
      rethrow;
    }
  }

  Future<bool> get isInitialized async => _pca != null;

  String? get accessToken => _accessToken;

  Future<bool> signIn() async {
    if (_pca == null) {
      throw Exception('MSAL not initialized');
    }

    try {
      final result = await _pca!.acquireToken(AuthConfig.apiScopes);
      _accessToken = result.accessToken;
      _accountId = result.account?.id;
      return true;
    } on MsalException catch (e) {
      debugPrint('MSAL sign in error: ${e.errorMessage}');
      return false;
    }
  }

  Future<bool> signInSilently() async {
    if (_pca == null) {
      throw Exception('MSAL not initialized');
    }

    try {
      final result = await _pca!.acquireTokenSilent(AuthConfig.apiScopes);
      _accessToken = result.accessToken;
      _accountId = result.account?.id;
      return true;
    } on MsalException catch (e) {
      debugPrint('MSAL silent sign in error: ${e.errorMessage}');
      return false;
    }
  }

  Future<void> signOut() async {
    if (_pca == null) {
      throw Exception('MSAL not initialized');
    }

    try {
      await _pca!.logout();
      _accessToken = null;
      _accountId = null;
    } on MsalException catch (e) {
      debugPrint('MSAL sign out error: ${e.errorMessage}');
      rethrow;
    }
  }

  Future<String?> getAccessToken() async {
    if (_accessToken != null) {
      return _accessToken;
    }

    // Try silent authentication first
    final success = await signInSilently();
    if (success) {
      return _accessToken;
    }

    return null;
  }
}
