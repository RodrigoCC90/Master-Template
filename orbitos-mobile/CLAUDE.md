# OrbitOS Mobile - AI Development Guide

## Overview

This is the **Flutter** mobile application for OrbitOS. It provides native iOS and Android apps with offline-first capabilities for viewing canvases, processes, and organizational health on the go.

## Architecture

```
orbitos-mobile/
├── lib/
│   ├── core/                     # Shared infrastructure
│   │   ├── auth/                # Authentication logic
│   │   ├── config/              # App configuration
│   │   ├── di/                  # Dependency injection
│   │   ├── network/             # HTTP client, interceptors
│   │   ├── theme/               # Material theme
│   │   └── utils/               # Helpers
│   ├── data/                     # Data layer
│   │   ├── datasources/         # Remote and local data sources
│   │   ├── models/              # DTOs with JSON serialization
│   │   └── repositories/        # Repository implementations
│   ├── domain/                   # Business logic layer
│   │   ├── entities/            # Domain entities
│   │   ├── repositories/        # Repository interfaces
│   │   └── usecases/            # Use case implementations
│   ├── presentation/             # UI layer
│   │   ├── blocs/               # BLoC state management
│   │   ├── screens/             # Full screens
│   │   └── widgets/             # Reusable widgets
│   └── main.dart                # App entry point
├── test/
│   ├── unit/                    # Unit tests
│   ├── widget/                  # Widget tests
│   └── integration/             # Integration tests
└── .ai/                         # AI context files
```

## Tech Stack

| Component | Package | Version |
|-----------|---------|---------|
| State | flutter_bloc | 8.x |
| DI | get_it + injectable | 7.x |
| HTTP | dio | 5.x |
| Storage | hive | 2.x |
| Router | go_router | 12.x |
| Code Gen | freezed + json_serializable | - |
| Testing | mocktail | 1.x |

## Layer Dependencies

```
Presentation → Domain ← Data
     ↓           ↓        ↓
   BLoC      UseCases   Repos
```

**Rules:**
- Domain has NO Flutter dependencies (pure Dart)
- Data implements interfaces from Domain
- Presentation depends on Domain (not Data)
- Dependency injection connects layers

## Key Patterns

### 1. Entity (Domain Layer)

```dart
// lib/domain/entities/canvas.dart
import 'package:freezed_annotation/freezed_annotation.dart';

part 'canvas.freezed.dart';

@freezed
class Canvas with _$Canvas {
  const factory Canvas({
    required String id,
    required String organizationId,
    required String name,
    String? description,
    required CanvasType type,
    required CanvasState state,
    required int version,
    required DateTime createdAt,
    required DateTime updatedAt,
  }) = _Canvas;
}

enum CanvasType { businessModel, lean, valueProposition }
enum CanvasState { draft, active, archived }
```

### 2. Model (Data Layer)

```dart
// lib/data/models/canvas_model.dart
import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/canvas.dart';

part 'canvas_model.freezed.dart';
part 'canvas_model.g.dart';

@freezed
class CanvasModel with _$CanvasModel {
  const CanvasModel._();

  const factory CanvasModel({
    required String id,
    required String organizationId,
    required String name,
    String? description,
    required String type,
    required String state,
    required int version,
    required DateTime createdAt,
    required DateTime updatedAt,
  }) = _CanvasModel;

  factory CanvasModel.fromJson(Map<String, dynamic> json) =>
      _$CanvasModelFromJson(json);

  // Map to domain entity
  Canvas toEntity() => Canvas(
    id: id,
    organizationId: organizationId,
    name: name,
    description: description,
    type: CanvasType.values.byName(type),
    state: CanvasState.values.byName(state),
    version: version,
    createdAt: createdAt,
    updatedAt: updatedAt,
  );
}
```

### 3. Repository Interface (Domain)

```dart
// lib/domain/repositories/canvas_repository.dart
import '../entities/canvas.dart';

abstract class CanvasRepository {
  Future<List<Canvas>> getAll(String organizationId);
  Future<Canvas?> getById(String organizationId, String canvasId);
  Future<Canvas> create(String organizationId, CreateCanvasParams params);
  Future<Canvas> update(String organizationId, String canvasId, UpdateCanvasParams params);
  Future<void> delete(String organizationId, String canvasId);
}

class CreateCanvasParams {
  final String name;
  final String? description;
  final CanvasType type;

  CreateCanvasParams({
    required this.name,
    this.description,
    required this.type,
  });
}
```

### 4. Repository Implementation (Data)

```dart
// lib/data/repositories/canvas_repository_impl.dart
import 'package:injectable/injectable.dart';
import '../../domain/entities/canvas.dart';
import '../../domain/repositories/canvas_repository.dart';
import '../datasources/canvas_remote_datasource.dart';
import '../datasources/canvas_local_datasource.dart';

@Injectable(as: CanvasRepository)
class CanvasRepositoryImpl implements CanvasRepository {
  final CanvasRemoteDataSource _remote;
  final CanvasLocalDataSource _local;

  CanvasRepositoryImpl(this._remote, this._local);

  @override
  Future<List<Canvas>> getAll(String organizationId) async {
    try {
      final models = await _remote.getAll(organizationId);
      await _local.cacheCanvases(models);
      return models.map((m) => m.toEntity()).toList();
    } catch (e) {
      // Offline fallback
      final cached = await _local.getCachedCanvases(organizationId);
      return cached.map((m) => m.toEntity()).toList();
    }
  }

  // ... other methods
}
```

### 5. Use Case (Domain)

```dart
// lib/domain/usecases/get_canvases.dart
import 'package:injectable/injectable.dart';
import '../entities/canvas.dart';
import '../repositories/canvas_repository.dart';

@injectable
class GetCanvases {
  final CanvasRepository _repository;

  GetCanvases(this._repository);

  Future<List<Canvas>> call(String organizationId) {
    return _repository.getAll(organizationId);
  }
}
```

### 6. BLoC (Presentation)

```dart
// lib/presentation/blocs/canvas/canvas_bloc.dart
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:injectable/injectable.dart';
import '../../../domain/entities/canvas.dart';
import '../../../domain/usecases/get_canvases.dart';

part 'canvas_bloc.freezed.dart';
part 'canvas_event.dart';
part 'canvas_state.dart';

@injectable
class CanvasBloc extends Bloc<CanvasEvent, CanvasState> {
  final GetCanvases _getCanvases;

  CanvasBloc(this._getCanvases) : super(const CanvasState.initial()) {
    on<_LoadCanvases>(_onLoadCanvases);
  }

  Future<void> _onLoadCanvases(
    _LoadCanvases event,
    Emitter<CanvasState> emit,
  ) async {
    emit(const CanvasState.loading());
    try {
      final canvases = await _getCanvases(event.organizationId);
      emit(CanvasState.loaded(canvases));
    } catch (e) {
      emit(CanvasState.error(e.toString()));
    }
  }
}
```

```dart
// lib/presentation/blocs/canvas/canvas_event.dart
part of 'canvas_bloc.dart';

@freezed
class CanvasEvent with _$CanvasEvent {
  const factory CanvasEvent.loadCanvases(String organizationId) = _LoadCanvases;
  const factory CanvasEvent.createCanvas(CreateCanvasParams params) = _CreateCanvas;
  const factory CanvasEvent.deleteCanvas(String canvasId) = _DeleteCanvas;
}
```

```dart
// lib/presentation/blocs/canvas/canvas_state.dart
part of 'canvas_bloc.dart';

@freezed
class CanvasState with _$CanvasState {
  const factory CanvasState.initial() = _Initial;
  const factory CanvasState.loading() = _Loading;
  const factory CanvasState.loaded(List<Canvas> canvases) = _Loaded;
  const factory CanvasState.error(String message) = _Error;
}
```

### 7. Screen (Presentation)

```dart
// lib/presentation/screens/canvases/canvas_list_screen.dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../blocs/canvas/canvas_bloc.dart';
import '../../widgets/canvas_card.dart';

class CanvasListScreen extends StatelessWidget {
  const CanvasListScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Canvases')),
      body: BlocBuilder<CanvasBloc, CanvasState>(
        builder: (context, state) {
          return state.when(
            initial: () => const SizedBox.shrink(),
            loading: () => const Center(child: CircularProgressIndicator()),
            loaded: (canvases) => ListView.builder(
              itemCount: canvases.length,
              itemBuilder: (context, index) => CanvasCard(
                canvas: canvases[index],
                onTap: () => _navigateToCanvas(context, canvases[index]),
              ),
            ),
            error: (message) => Center(child: Text('Error: $message')),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showCreateDialog(context),
        child: const Icon(Icons.add),
      ),
    );
  }

  void _navigateToCanvas(BuildContext context, Canvas canvas) {
    // Navigate to canvas detail
  }

  void _showCreateDialog(BuildContext context) {
    // Show create canvas dialog
  }
}
```

## Implementing from Specs

When implementing a feature (e.g., Canvas List):

### Step 1: Read the Spec
```bash
cat ../specs/L1-ui/screens/SCR-001-main-dashboard.json
cat ../specs/L4-data/entities/ENT-005-canvas.json
```

### Step 2: Create Domain Entity
```dart
// lib/domain/entities/canvas.dart
// Map fields from ENT-005
```

### Step 3: Create Data Model
```dart
// lib/data/models/canvas_model.dart
// JSON serialization from OpenAPI schema
```

### Step 4: Create Repository
```dart
// lib/domain/repositories/canvas_repository.dart  (interface)
// lib/data/repositories/canvas_repository_impl.dart (implementation)
```

### Step 5: Create Use Cases
```dart
// lib/domain/usecases/get_canvases.dart
// lib/domain/usecases/create_canvas.dart
// lib/domain/usecases/update_canvas.dart
```

### Step 6: Create BLoC
```dart
// lib/presentation/blocs/canvas/canvas_bloc.dart
```

### Step 7: Create Screen
```dart
// lib/presentation/screens/canvases/canvas_list_screen.dart
```

### Step 8: Write Tests
```dart
// test/unit/domain/usecases/get_canvases_test.dart
// test/widget/screens/canvas_list_screen_test.dart
```

## Running the App

```bash
# Get dependencies
flutter pub get

# Generate code (freezed, json_serializable)
flutter pub run build_runner build --delete-conflicting-outputs

# Run on device/emulator
flutter run

# Run on specific device
flutter run -d <device_id>

# Run in release mode
flutter run --release
```

## Running Tests

```bash
# All tests
flutter test

# Specific directory
flutter test test/unit/

# With coverage
flutter test --coverage

# Integration tests
flutter test integration_test/
```

## Code Generation

After modifying freezed/json_serializable classes:

```bash
# One-time build
flutter pub run build_runner build

# Watch mode
flutter pub run build_runner watch

# Clean and rebuild
flutter pub run build_runner build --delete-conflicting-outputs
```

## Dependency Injection Setup

```dart
// lib/core/di/injection.dart
import 'package:get_it/get_it.dart';
import 'package:injectable/injectable.dart';

import 'injection.config.dart';

final getIt = GetIt.instance;

@InjectableInit()
void configureDependencies() => getIt.init();
```

```dart
// lib/main.dart
void main() {
  configureDependencies();
  runApp(const OrbitOSApp());
}
```

## Environment Configuration

```dart
// lib/core/config/env.dart
abstract class Env {
  static const String apiUrl = String.fromEnvironment(
    'API_URL',
    defaultValue: 'http://localhost:5000/api/v1',
  );
}
```

Run with environment:
```bash
flutter run --dart-define=API_URL=https://api.orbitos.io/v1
```

## Theme (Material 3)

```dart
// lib/core/theme/app_theme.dart
import 'package:flutter/material.dart';

class AppTheme {
  static ThemeData get light => ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.fromSeed(
      seedColor: const Color(0xFF0969da),
    ),
  );

  static ThemeData get dark => ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.fromSeed(
      seedColor: const Color(0xFF0969da),
      brightness: Brightness.dark,
    ),
  );
}
```

## Security Checklist (SOC 2)

Before any PR:
- [ ] No secrets in code
- [ ] API keys in secure storage (flutter_secure_storage)
- [ ] Certificate pinning enabled
- [ ] Biometric auth for sensitive actions
- [ ] Data encrypted at rest
- [ ] No sensitive data in logs
