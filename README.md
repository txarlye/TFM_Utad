# Flight Simulator VR - Simulador de vuelo basado en realidad virtual

[![Video Demo](https://img.youtube.com/vi/RnSyyRkkCCA/maxresdefault.jpg)](https://www.youtube.com/watch?v=RnSyyRkkCCA)

## 📋 Descripción del Proyecto

Este proyecto forma parte del **Trabajo de Fin de Máster (TFM)** de la Universidad de Valladolid (UTAD). Se trata de una **aplicación formativa para pilotos basada en realidad virtual** que simula el vuelo de un avión F5 en un entorno inmersivo.

### 🎯 Objetivos
- Desarrollar una herramienta de formación para pilotos utilizando tecnologías de realidad virtual
- Crear una experiencia inmersiva que mejore la conciencia situacional y el control de vuelo
- Implementar un sistema de misiones y objetivos para el entrenamiento estructurado

## ✨ Características Principales

- **Simulación de vuelo en realidad virtual** con física realista
- **Integración con Cesium** para entornos geográficos realistas
- **Modelado 3D detallado** de la cabina del F5
- **Sistema de misiones** (Destruir objetivos, Pasar por anillos, Carrera cronometrada)
- **Audio espacial 3D** para mayor inmersión
- **Instrumentos de vuelo** interactivos (altímetro, velocímetro, tacómetro, etc.)
- **Sistema de puntuación** y mejores tiempos
- **Persistencia de datos** entre sesiones

## 🛠️ Requisitos del Sistema

### Software
- **Unity 2021.3.16f1** (LTS)
- **XR Interaction Toolkit 2.3.2**
- **TextMesh Pro**
- **Cesium for Unity** (opcional, para entornos realistas)
- **OpenXR Plugin**

### Hardware
- **Dispositivo VR compatible** (Oculus/Meta Quest, HTC Vive, etc.)
- **PC con especificaciones VR** (GPU compatible con VR)

## 📁 Estructura del Proyecto

```
TFM_Utad/
├── Assets/
│   ├── Scripts/
│   │   ├── Airplane/                    # Sistema de avión
│   │   │   ├── AirplanePhysics.cs       # Física del avión
│   │   │   ├── AirplaneControls.cs      # Controles VR
│   │   │   ├── AirplaneCharacteristics.cs # Parámetros del avión
│   │   │   └── Instruments/              # Instrumentos de vuelo
│   │   │       ├── IP_Airplane_Altimeter.cs
│   │   │       ├── IP_Airplane_Airspeed.cs
│   │   │       └── IP_Airplane_Engine.cs
│   │   ├── BehaviourManagers/            # Gestores principales
│   │   │   ├── GameManager.cs           # Gestión de estados del juego
│   │   │   ├── MissionManager.cs        # Gestión de misiones
│   │   │   ├── ObjectiveManager.cs      # Gestión de objetivos
│   │   │   ├── UIManager.cs             # Gestión de interfaz
│   │   │   ├── DataManager.cs           # Persistencia de datos
│   │   │   └── PoolManager.cs           # Sistema de pooling
│   │   ├── UI/                          # Interfaz de usuario
│   │   │   ├── UIManager.cs
│   │   │   ├── InfoPanel.cs
│   │   │   └── MinimapLine.cs
│   │   └── Utils/                       # Utilidades
│   │       ├── Singleton.cs
│   │       └── MoveForward.cs
│   ├── Scenes/                          # Escenas del juego
│   │   ├── 1 Start Scene.unity         # Escena de inicio/lobby
│   │   └── 2 Game Scene con mapas copiaSeg.unity # Escena de vuelo
│   ├── Prefabs/                         # Prefabs del juego
│   ├── Materials/                       # Materiales 3D
│   ├── Textures/                        # Texturas
│   ├── Audio/                           # Archivos de audio
│   │   ├── Airplanes/                   # Sonidos de aviones
│   │   ├── Airport/                     # Sonidos de aeropuerto
│   │   ├── Background/                  # Música de fondo
│   │   └── VFX/                         # Efectos de sonido
│   ├── CesiumSettings/                  # Configuración de Cesium
│   │   └── Resources/
│   │       └── CesiumRuntimeSettings.asset
│   └── XR/                             # Configuración XR
├── ProjectSettings/                     # Configuración del proyecto Unity
├── Packages/                           # Dependencias del proyecto
├── .img/                              # Diagramas y documentación
│   ├── GameManager.png                 # Diagrama UML del GameManager
│   ├── modelado.png                    # Proceso de modelado 3D
│   └── UI giroscopio..png              # Interfaz de instrumentos
├── .gitignore                          # Archivos ignorados por Git
├── README.md                           # Este archivo
└── LICENSE                             # Licencia del proyecto
```

## 🏗️ Arquitectura del Código

El proyecto está basado en el **patrón Singleton** con múltiples Managers que coordinan diferentes aspectos del juego:

### GameManager
Controla los estados principales del juego:
- **Estados**: `Playing`, `Paused`, `Won`, `Lost`, `Ended`
- **Métodos principales**:
  - `StartGame()` - Inicia el juego
  - `PauseGame()` - Pausa el juego
  - `WinGame()` - Finaliza con victoria
  - `LoseGame()` - Finaliza con derrota

### MissionManager
Gestiona los tipos de misión disponibles:
- **Tipos de misión**: `DestroyTargets`, `PassThroughRings`, `TimedRace`
- **Métodos principales**:
  - `Load()` - Carga la misión
  - `Spawn()` - Genera objetivos
  - `Despawn()` - Elimina objetivos

### ObjectiveManager
Controla los objetivos individuales:
- **Estados**: `Inactive`, `Active`, `Completed`
- **Métodos principales**:
  - `PassThroughRings()` - Pasar por anillos
  - `ObjectiveDestroyed()` - Objetivo destruido
  - `ActivateNextRing()` - Activar siguiente anillo

### UIManager
Actualiza todos los elementos de interfaz en tiempo real:
- **Información mostrada**: Puntuación, velocidad, altitud, distancia a objetivos
- **Métodos principales**:
  - `GetCurrentTime()` - Obtener tiempo actual
  - `StopGameTime()` - Detener cronómetro
  - `ShowFinalCanvas()` - Mostrar pantalla final
  - `UpdateScore()` - Actualizar puntuación

### Sistema de Avión
- **AirplanePhysics**: Física y fuerzas aerodinámicas
- **AirplaneControls**: Input de controles VR
- **AirplaneCharacteristics**: Parámetros del avión
- **Instrumentos**: Altímetro, velocímetro, tacómetro, combustible, etc.

### Diagrama UML
Ver `.img/GameManager.png` para las relaciones completas entre clases.

## 🚀 Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/TFM_Utad.git
cd TFM_Utad
```

### 2. Abrir en Unity
1. Abrir **Unity Hub**
2. Añadir proyecto existente
3. Seleccionar la carpeta `TFM_Utad`
4. Asegurarse de usar **Unity 2021.3.16f1**

### 3. Configurar XR (Opcional)
1. Ir a **Window > XR Plugin Management**
2. Instalar **OpenXR Plugin**
3. Configurar para tu dispositivo VR

### 4. Configurar Cesium (Opcional)
1. Ir a **Window > Cesium**
2. Configurar token de Cesium Ion (opcional)
3. Seleccionar ubicación geográfica

### 5. Ejecutar el proyecto
1. Abrir la escena `1 Start Scene`
2. Presionar **Play** en Unity
3. Conectar dispositivo VR

## 🎮 Controles

### Controles VR
- **Movimiento**: Controles de mano VR
- **Interacción**: Botones y palancas de la cabina
- **Teleportación**: Sistema de movimiento VR

### Controles de Prueba (Teclado)
Para pruebas sin hardware VR:
- **AD / Flechas horizontales**: Movimiento horizontal
- **WS / Flechas verticales**: Movimiento vertical
- **Espacio**: Disparo de ráfaga
- **B**: Disparo de misil
- **M**: Acelerar
- **N**: Reducir velocidad

## 📊 Persistencia de Datos

El proyecto utiliza **PlayerPrefs** para almacenar datos persistentes:
- **Tipo de misión seleccionada**
- **Mejor tiempo por misión**
- **Configuraciones del usuario**

Los datos se guardan automáticamente al cerrar la aplicación.

## 🛠️ Herramientas de Desarrollo

### Software Utilizado
- **Unity 2021.3.16f1** - Motor de juego
- **3D Studio Max** - Modelado 3D
- **AutoCAD** - Diseño técnico
- **Photoshop** - Texturas y UI
- **After Effects** - Efectos visuales

### Librerías y Plugins
- **XR Interaction Toolkit** - Interacción VR
- **Cesium for Unity** - Entornos geográficos
- **TextMesh Pro** - Texto de alta calidad
- **Universal Render Pipeline** - Renderizado

## 📈 Proceso de Desarrollo

### 1. Modelado 3D
- Modelado de la cabina del F5 en **3D Studio Max**
- Texturizado con **Photoshop**
- Importación a Unity como prefabs

### 2. Programación
- Implementación de la física del avión
- Sistema de misiones y objetivos
- Interfaz de usuario y HUD

### 3. Integración VR
- Configuración de XR Interaction Toolkit
- Implementación de controles VR
- Optimización para dispositivos VR

### Modo de Prueba
- Controles de teclado para pruebas sin VR
- Sistema de debug en tiempo real
- Logs detallados del comportamiento del avión

## 📄 Licencia

Este proyecto está bajo la **Unity Companion License** para proyectos dependientes de Unity.

## 👥 Créditos

- **Desarrollador**: Txarlye (cítame o invítame a un café)
- **Universidad**: U-TAD
- **Tutor**: [Nombre del Tutor]
- **Año**: 2023

### Recursos Externos
- **Modelos 3D**: Recursos gratuitos de Unity Asset Store
- **Audio**: Kenney Audio Pack
- **Texturas**: Texturas personalizadas y de dominio público

## 📞 Contacto

Para preguntas sobre el proyecto o colaboraciones:
- **Email**: txarlye@gmail.com
- **GitHub**: txarlye

---

**¡Disfruta volando en realidad virtual! ✈️**