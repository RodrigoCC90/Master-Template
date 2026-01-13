import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/auth_config.dart';
import 'auth_service.dart';

class ApiService {
  final AuthService _authService;

  ApiService(this._authService);

  Future<Map<String, String>> _getHeaders() async {
    final token = await _authService.getAccessToken();
    return {
      'Content-Type': 'application/json',
      if (token != null) 'Authorization': 'Bearer $token',
    };
  }

  Future<T> get<T>(String endpoint) async {
    final headers = await _getHeaders();
    final response = await http.get(
      Uri.parse('${AuthConfig.apiBaseUrl}$endpoint'),
      headers: headers,
    );

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return json.decode(response.body) as T;
    }

    throw ApiException(response.statusCode, response.body);
  }

  Future<T> post<T>(String endpoint, Map<String, dynamic> data) async {
    final headers = await _getHeaders();
    final response = await http.post(
      Uri.parse('${AuthConfig.apiBaseUrl}$endpoint'),
      headers: headers,
      body: json.encode(data),
    );

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return json.decode(response.body) as T;
    }

    throw ApiException(response.statusCode, response.body);
  }

  Future<T> put<T>(String endpoint, Map<String, dynamic> data) async {
    final headers = await _getHeaders();
    final response = await http.put(
      Uri.parse('${AuthConfig.apiBaseUrl}$endpoint'),
      headers: headers,
      body: json.encode(data),
    );

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return json.decode(response.body) as T;
    }

    throw ApiException(response.statusCode, response.body);
  }

  Future<void> delete(String endpoint) async {
    final headers = await _getHeaders();
    final response = await http.delete(
      Uri.parse('${AuthConfig.apiBaseUrl}$endpoint'),
      headers: headers,
    );

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(response.statusCode, response.body);
    }
  }
}

class ApiException implements Exception {
  final int statusCode;
  final String message;

  ApiException(this.statusCode, this.message);

  @override
  String toString() => 'ApiException: $statusCode - $message';
}
