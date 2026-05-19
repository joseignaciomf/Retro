RetroRescue: The 90s Glitch - v1.0 scaffold

Archivos añadidos (esqueleto):
- Assets/_Project/Architecture/GameEvents.cs
- Assets/_Project/Features/Cassette/CassetteController.cs
- Assets/_Project/Features/Cassette/CassetteInput.cs
- Assets/_Project/Features/Cassette/TapeVisualController.cs
- Assets/_Project/Features/LevelDataSO.cs
- Assets/_Project/Features/Shared/AudioFeedbackController.cs
- Assets/_Project/UI/HUDProgressController.cs
- Assets/Editor/LevelPrefabBuilder.cs
- .editorconfig
- .gitattributes
- .gitignore
- .github/workflows/unity-ci.yml (placeholder)

Instrucciones rápidas:
1. Abrir este proyecto con Unity 6 LTS.
2. Esperar a que Unity procese los assets y compile los scripts.
3. En Unity Editor, abrir el menú: RetroRescue -> Build Example Prefabs.
   - Esto creará prefabs en `Assets/Prefabs` y assets `LevelDataSO` en `Assets/Levels`.
4. Abrir el prefab `Assets/Prefabs/Level_Cassette_Example.prefab`:
   - Verificar que el GameObject raíz tenga `CassetteController`, `CassetteInput`.
   - Asignar referencias si es necesario (LeftGear / RightGear / Tape).
5. Crear un Canvas con `Slider` y `Text`, añadir `HUDProgressController` y enlazar las referencias.
6. Importar FMOD (opcional) para audio; si no está, el `AudioFeedbackController` usa logs por defecto.

Siguientes pasos sugeridos:\n- Añadir CI: se ha añadido el workflow '.github/workflows/unity-editmode-tests.yml' que ejecuta las pruebas EditMode en cada push/PR.\n  * Requisitos: configurar la variable de repositorio 'UNITY_VERSION' (ej: '6.0.0f1') y opcionalmente el secret 'UNITY_LICENSE' si se requiere activación.\n
- Implementar la mecánica `MEC-002` (TV Tuning) y su controlador en `Assets/_Project/Features/TVTuning`.
- Añadir tests unitarios para la lógica de progreso y cálculo de ángulos.
- Configurar Git LFS antes del primer commit para los assets binarios.

Notas:
- Los prefabs y assets se crean mediante un Editor script en `Assets/Editor/LevelPrefabBuilder.cs`.
- Si Unity muestra errores de compilación, abrir la consola y corregirlos antes de ejecutar el menú.

Shader y FMOD de prueba:
- Shader: Assets/Shaders/TVNoise.shader (propiedades: _NoiseOpacity, _Clarity, _Speed). Crear un material con este shader y asignarlo al screenRenderer del prefab TV para probar la estática dinámica.
- FMOD: Assets/FMOD/meta/events.json contiene rutas de eventos de ejemplo. Importa tu proyecto FMOD y mapea estos eventos a las rutas indicadas. Si FMOD no está disponible, AudioFeedbackController escribe logs de evento para pruebas.

