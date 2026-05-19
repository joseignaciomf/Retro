# **DOCUMENTO DE DISEÑO TÉCNICO (TDD)**

**Proyecto:** RetroRescue: The 90s Glitch

**Entorno de Desarrollo Base:** Unity 6 LTS (Long Term Support)

**Lenguaje:** C# (.NET 8 / C# 12 compatible)

## **1. ENTORNO DE DESARROLLO (IDE) Y CONFIGURACIÓN BASE**

Para mantener la homogeneidad del código y evitar conflictos de formateo
en Git, todo el equipo utilizará las mismas herramientas:

- **IDE Recomendado:** **JetBrains Rider** (Versión 2024.x o superior) o
  **Visual Studio Code** con la extensión oficial *Unity Extension Pack*
  (Microsoft).

- **EditorConfig:** Se incluirá un archivo .editorconfig en la raíz del
  proyecto para forzar de forma automática:

  - Indentación: 4 espacios (no tabuladores).

  - Saltos de línea: LF (Unix/Android compatible).

  - Llaves K&R modificadas (apertura en nueva línea para clases y
    métodos).

- **Android SDK/NDK:** Se gestionará estrictamente a través del *Unity
  Hub* utilizando la versión instalada por defecto con el módulo de
  Android de Unity 6 LTS. No se permitirán rutas locales externas a SDKs
  personalizadas para evitar fallos de compilación en Gradle.

## **2. GESTIÓN DE VERSIONES Y POLÍTICA DE GIT**

El control de versiones se gestionará en **GitHub/GitLab** aplicando una
estrategia basada en **GitFlow Simplificado**.

### **2.1. Git LFS (Large File Storage)**

Dado que el juego contiene assets binarios pesados (sprites Pixel Art
sin comprimir, pistas de audio de FMOD), es obligatorio inicializar
**Git LFS** antes del primer commit. El archivo .gitattributes
rastreará:

Plaintext

\*.png filter=lfs diff=lfs merge=lfs -text

\*.wav filter=lfs diff=lfs merge=lfs -text

\*.mp3 filter=lfs diff=lfs merge=lfs -text

\*.fspro filter=lfs diff=lfs merge=lfs -text (Proyectos FMOD)

### **2.2. Estrategia de Ramas (Branching)**

- **main:** Código de producción 100% estable. Solo se actualiza
  mediante *Pull Requests* (PR) desde release/.

- **develop:** Rama de integración diaria. Todos los desarrolladores
  mezclan aquí.

- **feature/REQ-XXXX:** Ramas de trabajo individuales. Nacen de develop
  y mueren en develop. El nombre debe coincidir con el ID del requisito
  del Blueprint (ej. feature/MEC-001-cassette).

### **2.3. Formato de Commits (Conventional Commits)**

Los commits deben seguir la estructura: tipo(ámbito): descripción corta

- feat(core): añadido sistema de control del estado del nivel (FSM)

- fix(mec-001): corregido cálculo del ángulo de rotación del bolígrafo

- asset(audio): integrados bancos de sonido FMOD para el mundo 1

## **3. SISTEMA DE PAQUETES Y PLUGINS DE TERCEROS (DEPENDENCIAS)**

Para evitar el "infierno de dependencias", solo se autoriza el uso de
los siguientes paquetes a través del **Unity Package Manager (UPM)** y
wrappers oficiales:

1.  **Unity Addressables (v1.22+):** Para la gestión de carga y descarga
    asíncrona de assets desde el servidor CDN (escalabilidad de
    niveles).

2.  **New Input System (v1.7+):** Gestión nativa de eventos táctiles
    multitouch de baja latencia. Queda prohibido el uso del viejo
    Input.GetTouch.

3.  **FMOD Unity Integration:** Plugin oficial de FMOD para conectar el
    motor de audio interactivo con Unity. Reemplaza por completo al
    componente *AudioSource* nativo.

4.  **Firebase SDK (Auth + Crashlytics):** Gestión de inicio de sesión
    con Google y analíticas de fallos en tiempo real.

5.  **UniTask:** Plugin para optimizar la programación asíncrona
    (async/await) en Unity, reduciendo la asignación de memoria en el
    *Garbage Collector* en comparación con las Corrutinas tradicionales.

## **4. ESTRUCTURA DE CARPETAS DEL PROYECTO (Assets/)**

El proyecto se organizará por **Sistemas e hitos**, no por tipo de
archivo, para facilitar el aislamiento de componentes:

Plaintext

Assets/

├── \_Project/

│ ├── Architecture/ \# GameManagers, EventBrokers, FSMs

│ ├── Features/ \# Lógica funcional por mecánica

│ │ ├── Cassette/ \# Scripts, Animaciones y Prefabs del Casete

│ │ └── TVTuning/ \# Scripts, Shaders y Prefabs de la TV

│ ├── UI/ \# Canvas del juego, Menús, HUD

│ └── Shared/ \# Materiales, Tipografías Retro, Extensiones C#

├── AddressableAssets/ \# Sprites y Prefabs configurados como
Addressables

└── Plugins/ \# FMOD, Firebase, UniTask (no modificar)

## **5. PATRÓN ARQUITECTÓNICO: EVENT-DRIVEN / SCRIPTABLE OBJECTS**

Para garantizar un acoplamiento cero (*Loose Coupling*), los sistemas se
comunicarán mediante un **Event Broker** global basado en Actions de C#.
Ningún componente de juego (Script del casete) llamará directamente al
UI o al sistema de sonido.

### **Código Base: Implementación del Event Broker Central (GameEvents.cs)**

C#

using System;

using UnityEngine;

namespace RetroRescue.Architecture

{

public static class GameEvents

{

// Evento para el cambio de estados del juego (FSM)

public static Action\<LevelState\> OnLevelStateChanged;

// Evento de progreso de mecánicas (MEC-001 / MEC-002)

// Pasa el ID del nivel y el porcentaje actual (0.0f a 100.0f)

public static Action\<string, float\> OnMechanicProgressUpdated;

// Disparador de eventos de feedback (Audio + Háptico)

public static Action\<string\> OnFeedbackTriggered; // Pasa el ID del
Evento (ej: "EV_TAP_SNAP")

public static void RaiseLevelStateChanged(LevelState newState) =\>
OnLevelStateChanged?.Invoke(newState);

public static void RaiseMechanicProgressUpdated(string levelId, float
progress) =\> OnMechanicProgressUpdated?.Invoke(levelId, progress);

public static void RaiseFeedbackTriggered(string eventId) =\>
OnFeedbackTriggered?.Invoke(eventId);

}

public enum LevelState

{

Init,

IntroAnim,

GameplayInput,

Success,

Failure

}

}

### **Ejemplo de Consumo en Código de Desarrollo (AudioFeedbackController.cs)**

C#

using UnityEngine;

using RetroRescue.Architecture;

namespace RetroRescue.Features.Audio

{

public class AudioFeedbackController : MonoBehaviour

{

private void OnEnable()

{

GameEvents.OnFeedbackTriggered += HandleFeedback;

}

private void OnDisable()

{

GameEvents.OnFeedbackTriggered -= HandleFeedback;

}

private void HandleFeedback(string eventId)

{

switch (eventId)

{

case "EV_TAP_SNAP":

// Llamada al motor FMOD

FMODUnity.RuntimeManager.PlayOneShot("event:/FX/Bic_Snap");

break;

case "EV_WIN_LEVEL":

FMODUnity.RuntimeManager.PlayOneShot("event:/Stingers/Win_Jingle");

break;

// Resto de la matriz de eventos funcional...

}

}

}

}

## **6. INTEGRACIÓN CONTINUA (CI/CD) Y AUTOMATIZACIÓN de QA**

Para cumplir la meta de una trazabilidad y calidad de nivel 10,
configuraremos un pipeline automatizado con **GitHub Actions**:

1.  **Validación de PR (Integración Continua):** Cada vez que se abra
    una Pull Request hacia develop, el servidor de CI ejecutará el
    **Unity Test Framework**. Si algún test unitario falla (por ejemplo,
    si el cálculo matemático de la sintonización da un error fuera de
    rango), la PR se bloquea automáticamente y no se puede mezclar el
    código.

2.  **Despliegue Continuo (CD):** Cada viernes a las 18:00, el pipeline
    compilará de forma automática el proyecto generando un archivo .aab
    (Android App Bundle) firmado con la clave de desarrollo, y lo subirá
    directamente al canal de **Pruebas Internas de Google Play Console**
    mediante la API de desarrolladores de Google. El equipo de QA
    recibirá una notificación en sus terminales Android para testear el
    fin de semana.
