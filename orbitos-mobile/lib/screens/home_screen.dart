import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  String? _apiStatus;
  bool _isTestingApi = false;

  Future<void> _testApiConnection() async {
    setState(() {
      _isTestingApi = true;
      _apiStatus = null;
    });

    try {
      final authService = Provider.of<AuthService>(context, listen: false);
      final apiService = ApiService(authService);

      final response =
          await apiService.get<Map<String, dynamic>>('/api/auth/ping-auth');
      setState(() {
        _apiStatus = 'Connected as ${response['user']}';
      });
    } catch (e) {
      setState(() {
        _apiStatus = 'Error: $e';
      });
    } finally {
      setState(() {
        _isTestingApi = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('OrbitOS'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await context.read<AuthProvider>().signOut();
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Dashboard',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 24),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'API Connection Test',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 16),
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: _isTestingApi ? null : _testApiConnection,
                        child: Text(
                            _isTestingApi ? 'Testing...' : 'Test Connection'),
                      ),
                    ),
                    if (_apiStatus != null) ...[
                      const SizedBox(height: 16),
                      Text(
                        _apiStatus!,
                        style: TextStyle(
                          color: _apiStatus!.startsWith('Error')
                              ? Colors.red
                              : Colors.green,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
