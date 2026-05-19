# **DOCUMENTO DE ESPECIFICACIÓN FUNCIONAL**

**Proyecto:** RetroRescue: The 90s Glitch

**Versión:** 1.0

**Estado:** Listo para Desarrollo

## **1. ESPECIFICACIÓN GENERAL DE UI/UX Y FLUJO DE INTERFACZ**

### **1.1. Configuración de Pantalla y Área de Interacción**

- **Orientación:** Vertical estricta (*Portrait*, relación de aspecto
  base 9:16, escalable mediante *Canvas Scaler* de Unity a 19.5:9 para
  pantallas modernas).

- **Zona Segura (*Safe Area*):** Los elementos críticos de UI
  (puntuación, temporizador, menús) deben posicionarse respetando el
  *Notch* superior y la barra de navegación inferior de Android.

- **Área de Input Táctil:** El 70% central de la pantalla se reserva
  para la interacción con el objeto analógico.

### **1.2. Flujo de Estados de un Nivel (Máquina de Estados Finita - FSM)**

Cada nivel debe implementar obligatoriamente los siguientes 5 estados
lógicos:

\[1. INIT\] ──\> \[2. INTRO_ANIM\] ──\> \[3. GAMEPLAY_INPUT\] ──┬──\>
\[4. SUCCESS\] ──\> Siguiente Nivel

└──\> \[5. FAILURE\] ──\> Reiniciar / Menu

1.  **INIT:** Carga asíncrona del Asset del nivel mediante
    *Addressables*. Datos inicializados a cero. Pantalla en negro.

2.  **INTRO_ANIM:** *Fade-in* de pantalla. Animación de entrada del
    objeto corrupto por el "Glitch" (duración fija: 1.2 segundos). El
    temporizador está congelado. No se acepta *input* del usuario.

3.  **GAMEPLAY_INPUT:** Se activa el temporizador de nivel. Se habilita
    la escucha de eventos táctiles (*Touch Events*). El bucle de juego
    comprueba las condiciones de victoria/derrota en cada frame (*Update
    loop*).

4.  **SUCCESS:** Se desactiva el *input*. Animación de limpieza del
    objeto. Desbloqueo del coleccionable. Disparador del evento de
    analítica log_level_clear.

5.  **FAILURE:** Se desactiva el *input*. Efecto visual de "pantalla
    rota/pixelada". Aparece UI flotante con botón "Insert Coin"
    (Reintentar). Disparador de analítica log_level_fail.

## **2. ESPECIFICACIÓN DETALLADA DE MECÁNICAS (MVP)**

### **MECÁNICA 1: El Casete y el Bolígrafo Bic (ID: MEC-001)**

#### **A. Elementos en Pantalla**

- **Sprite A (Fijo):** Cuerpo de un casete de audio clásico con dos
  engranajes dentados huecos (Izquierdo y Derecho).

- **Sprite B (Móvil):** Un bolígrafo Bic clásico inclinado a 45 grados.

- **Sprite C (Dinámico):** La cinta magnética enredada saliendo de la
  parte inferior del casete. Su forma física (*Shape*) cambia según el
  porcentaje de progreso.

#### **B. Lógica de Input y Captura de Gesto**

1.  **Fase Touch_Began:** El usuario toca la pantalla. El sistema
    calcula la distancia entre la posición del toque (\$P\_{touch}\$) y
    el centro del Engranaje Izquierdo (\$C\_{left}\$) o Derecho
    (\$C\_{right}\$).

    - Si \$Distancia \le R\_{activation}\$ (Radio de activación: 80
      píxeles), el Sprite B (Bolígrafo) se posiciona automáticamente en
      el centro del engranaje seleccionado y actúa como eje de rotación
      (\$Eje\_{rot}\$).

2.  **Fase Touch_Moved:** El usuario arrastra el dedo realizando un
    movimiento circular alrededor de \$Eje\_{rot}\$.

    - El sistema calcula el vector angular entre el frame anterior y el
      frame actual (\$\Delta\theta\$).

    - **Filtro de Sentido:** Solo se procesa \$\Delta\theta\$ si el giro
      es a favor de las agujas del reloj (*Clockwise*). Si es inverso,
      \$\Delta\theta = 0\$.

#### **C. Lógica de Progreso y Condiciones**

- **Variable de Progreso (\$Prog\$):** Rango \$\[0.0 - 100.0\]\$.
  Inicializa en \$0.0\$.

- **Fórmula de actualización (por frame):\**
  \$\$Prog\_{actual} = Prog\_{anterior} + (\|\Delta\theta\| \times
  K\_{rewind})\$\$\
  *(Donde \$K\_{rewind}\$ es una constante de ajuste de dificultad
  almacenada en el ScriptableObject del nivel).*

- **Condición de Victoria:** \$Prog \ge 100.0\$ antes de que
  \$Tiempo\_{restante} == 0\$.

- **Condición de Derrota:** \$Tiempo\_{restante} == 0\$ y \$Prog \<
  100.0\$.

- **Comportamiento Visual de la Cinta:** El Sprite C (Cinta) escala su
  eje Y hacia abajo de forma inversamente proporcional a \$Prog\$.
  Cuando \$Prog == 100.0\$, el Sprite C pasa a Alpha = 0 (Cinta
  totalmente recogida).

### **MECÁNICA 2: Sintonización de TV de Tubo (ID: MEC-002)**

#### **A. Elementos en Pantalla**

- **Sprite A (Fijo):** El marco de una televisión analógica de los 80.

- **Material/Shader (Pantalla):** Un shader que reproduce ruido estático
  ("nieve") blanco y gris mezclado con líneas de barrido entrelazadas.

- **Sprite B y C (Móviles):** Dos varillas de antena (Antena L,
  Antena R) que pivotan desde la base superior de la TV.

- **Sprite D (Móvil):** Una rueda giratoria analógica (Sintonizador) en
  el lateral derecho de la TV.

#### **B. Variables de Control y Objetivos**

Cada sesión genera tres valores objetivo aleatorios (*Targets*) ocultos
y tres variables modificables por el usuario:

- \$Target\_{AntenaL}\$ \| Rango de ángulo: \$\[15^\circ - 165^\circ\]\$

- \$Target\_{AntenaR}\$ \| Rango de ángulo: \$\[15^\circ - 165^\circ\]\$

- \$Target\_{Rueda}\$ \| Rango rotacional: \$\[0^\circ - 360^\circ\]\$

#### **C. Cálculo de Claridad de Imagen (\$Clarity\$)**

En cada frame, el sistema evalúa la desviación absoluta del usuario
respecto a los objetivos:

\$\$Diff\_{Total} = \|AnguloL - Target\_{AntenaL}\| + \|AnguloR -
Target\_{AntenaR}\| + \|AnguloRueda - Target\_{Rueda}\|\$\$

La Claridad (\$C\$) se normaliza en un rango \$\[0.0 - 1.0\]\$:

\$\$C = 1.0 - \left(\frac{Diff\_{Total}}{Max\_{Error}}\right)\$\$

*(Si \$C \< 0\$, se fuerza a \$0.0\$).*

#### **D. Comportamiento Dinámico del Shader y Condiciones**

- **Opacidad del Ruido:** La transparencia de la estática del Shader de
  la pantalla es igual a \$1.0 - C\$. Si \$C == 1.0\$, la pantalla se ve
  100% nítida mostrando un dibujo animado retro de fondo.

- **Condición de Victoria:** El jugador debe mantener un nivel de
  Claridad \$C \ge 0.95\$ de forma continua durante un tiempo de
  fijación (\$T\_{hold} = 2.0\$ segundos). Si \$C\$ baja de \$0.95\$, el
  temporizador de fijación se resetea a cero.

## **3. MATRIZ DE EVENTOS DE FEEDBACK HÁPTICO Y AUDIO (FMOD)**

Para cumplir el pilar de "jugabilidad jugosa", los desarrolladores deben
mapear los siguientes disparadores (*Triggers*) exactos en los sistemas
de Audio y Vibración:

| **ID Evento** | **Origen / Mecánica** | **Condición de Disparo** | **Acción de Audio (FMOD Event)** | **Acción Háptica (Android Haptic)** |
|----|----|----|----|----|
| EV_TAP_SNAP | General / MEC-001 | Éxito al encajar el boli en el engranaje. | PlayOneShot("event:/FX/Bic_Snap") | Vibrate(EFFECT_CLICK) (Toque seco corto) |
| EV_TAPE_LOOP | MEC-001 | Mientras \$\Delta\theta \> 0\$ en fase Touch_Moved. | PlayLoop("event:/FX/Tape_Roll") con parámetro Speed mapeado al valor de \$\Delta\theta\$. | VibrateContinuous(Intensity) proporcional a \$\Delta\theta\$. |
| EV_KNOB_TICK | MEC-002 | Cada vez que el ángulo de la rueda cambia en múltiplos de \$10^\circ\$. | PlayOneShot("event:/FX/Knob_Click") | Vibrate(EFFECT_THUD) (Micro-impulso) |
| EV_STATIC_AUDIO | MEC-002 | En cada frame del estado GAMEPLAY_INPUT. | Modificar volumen de event:/Loops/White_Noise a \$(1.0 - C)\$ y volumen de event:/Music/Retro_Show a \$C\$. | VibrateContinuous(Rumble) si \$C \< 0.25\$ (Simula temblor de mala señal). |
| EV_WIN_LEVEL | General | Cambio a estado SUCCESS. | PlayOneShot("event:/Stingers/Win_Jingle") | Vibrate(EFFECT_HEAVY_CLICK) seguido de ráfaga de éxito. |

## **4. ESPECIFICACIÓN DE DATOS (ESTRUCTURA DEL SCRIPTABLEOBJECT)**

El backend de diseño de niveles se gestionará mediante objetos de datos
puros. Los desarrolladores implementarán la clase LevelDataSO con los
siguientes campos obligatorios para asegurar la trazabilidad con QA:

C#

\[CreateAssetMenu(fileName = "NewLevelData", menuName =
"RetroRescue/Level Data")\]

public class LevelDataSO : ScriptableObject

{

\[Header("QA & Trazabilidad")\]

public string levelID; // Ejemplo: "REQ-M2-L5-CASSETTE"

public int orderIndex; // Posición en el mapa de niveles

\[Header("Configuración de Config")\]

public GameObject levelPrefab; // Contenedor de Sprites del nivel

public float levelDurationSeconds; // Tiempo límite de juego (ej. 15.0f)

\[Header("Mecánica Específica")\]

public MechanicType mechanicType; // Enum: CASSETTE, TV_TUNING

public float difficultyModifier; // Multiplicador K para el progreso

\[Header("Audio Events")\]

public string mainAudioEventPath; // Ruta del evento FMOD

}
