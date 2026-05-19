# **DOCUMENTO MASTER BLUEPRINT: RETRORESCUE: THE 90S GLITCH**

## **1. VISIÓN GENERAL Y PILARES DEL PRODUCTO**

- **Nombre en Clave:** *Project RetroGlitch*

- **Género:** Pasatiempo / Puzle Táctil Hipercasual.

- **Público Objetivo Utama (Target):** Generación X y Millennials (35-50
  años). Usuarios económicamente solventes que buscan sesiones cortas de
  juego (1-3 min) basadas en la nostalgia.

- **Plataforma Inicial:** Android (Google Play Store).

- **Modelo de Negocio:** Free-to-Play (AdMob Rewarded Videos + In-App
  Purchases estéticos y funcionales no intrusivos).

### **Pilares de Diseño (Innegociables)**

1.  **Jugabilidad Monomanual (*One-Hand Experience*):** Todo el juego
    debe poder controlarse con un solo pulgar.

2.  **Jugabilidad Jugosa (*Juiciness*):** Cada acción táctil debe
    generar una respuesta visual (partículas, filtros) y háptica
    inmediata.

3.  **Fidelidad Sonora Interactiva:** El sonido no es ambiental; es una
    mecánica de juego reactiva.

## **2. NARRATIVA SISTÉMICA Y PROGRESIÓN (EL GUION DE TRABAJO)**

La historia se integra directamente en la arquitectura del juego
mediante un **Mapa Cronológico Inverso**.

- **Protagonista:** Álex, 44 años. Un reflejo del jugador actual.

- **Conflicto:** El "Glitch" destruye el pasado. Reparar el objeto es
  salvar el recuerdo.

- **Estructura de Mundos (Escalabilidad):** El juego se lanza con 3
  épocas (10 niveles por época). La arquitectura debe permitir añadir
  "Mundos" mediante parches sin alterar el núcleo de la app.

\[Presente: 2026\] ──\> \[Mundo 1: Los 80 (8-bit)\] ──\> \[Mundo 2:
Principios 90 (16-bit)\] ──\> \[Mundo 3: Finales 90 (3D)\]

## **3. ARQUITECTURA DE SOFTWARE Y STACK TECNOLÓGICO**

Diseñada para minimizar el acoplamiento de código y maximizar la
trazabilidad y la escalabilidad.

### **Stack Técnico**

- **Motor:** Unity LTS (Versión de Soporte a Largo Plazo) en C#.

- **Arquitectura de Código:** Patrón **MVVM (Model-View-ViewModel)** o
  **Event-Driven Architecture (Arquitectura Dirigida por Eventos)**. Los
  sistemas (Audio, Vibración, Puntuación) no se conocen entre sí;
  reaccionan a eventos globales emitidos por el motor de niveles.

- **Gestión de Datos:** **ScriptableObjects** de Unity para definir los
  niveles. Cada nivel es un archivo de datos independiente que contiene:
  *ID del objeto, Ruta del Asset, Umbral de victoria, Configuración de
  audio y Patrón háptico*.

- **Audio Pipeline:** FMOD Sandbox integrado en Unity para audio
  dinámico y adaptativo.

- **Backend & Analíticas:** Firebase (Auth de Google, Crashlytics para
  reporte de errores en tiempo real y Remote Config para ajustar la
  dificultad de los niveles sin actualizar la app).

## **4. PLANIFICACIÓN DE FASES DE PRODUCCIÓN (ROADMAP)**

Aplicamos metodología Agile (Scrum) dividida en hitos cerrados para
evitar el *Scope Creep* (desviación de objetivos).

### **Fase 1: Pre-producción y Core Loop (Semanas 1-2)**

- **Entregable:** *Greybox Prototype*. Niveles sin arte final donde se
  testea la fricción del dedo recreando el giro del boli Bic en el
  casete y la sintonización de la antena.

- **Objetivo de Calidad:** Validar que la mecánica táctil es divertida
  por sí misma.

### **Fase 2: Producción MVP Alpha (Semanas 3-4)**

- **Entregable:** Versión jugable con 2 niveles completos con arte Pixel
  Art final, filtros CRT y audio interactivo integrado a través de FMOD.

- **Objetivo de Calidad:** Implementación de la primera tubería
  (*pipeline*) de QA automatizado en Unity.

### **Fase 3: Integración de Sistemas y Beta (Semana 5)**

- **Entregable:** Menús, sistema de guardado en la nube con Google Play
  Services, tienda de micropagos y SDK de anuncios AdMob.

- **Objetivo de Calidad:** Pruebas de estrés de guardado y conectividad.

### **Fase 4: Soft Launch y QA de Campo (Semana 6)**

- **Entregable:** Publicación controlada en Google Play (Beta abierta
  para un país/región específica).

- **Objetivo de Calidad:** Monitorización de métricas de retención (Día
  1 y Día 7) y recolección de informes de cuelgues (*crashes*) mediante
  Firebase.

## **5. ESTRATEGIA DE CALIDAD (QA), TRAZABILIDAD Y ESCALABILIDAD**

Para garantizar una cadena de calidad de nivel "Diez", el desarrollo se
regirá bajo tres normas estrictas:

### **A. Trazabilidad Absoluta (User Story -\> Código -\> Test)**

Cada mecánica descrita en el guion tendrá un identificador único. Por
ejemplo: REQ-M2-L5-CASSETTE.

- El programador usará este código en los comentarios de su código y en
  los commits de Git.

- El equipo de QA usará este mismo código para diseñar el caso de prueba
  automatizado.

- Si un nivel falla, sabremos exactamente qué requisito de diseño o qué
  línea de código lo está provocando.

### **B. Escalabilidad de Contenido (*AssetBundles*)**

Para evitar que la app pese gigabytes, el juego base solo incluirá el
Mundo 1. Los Mundos 2 y 3 se descargarán bajo demanda desde los
servidores de Google usando **Unity Addressables**. Esto permite lanzar
nuevos niveles cada mes sin obligar al usuario a actualizar la app desde
la tienda.

### **C. Pipeline de QA Multi-Dispositivo**

Dado que Android sufre de fragmentación, estableceremos una matriz de
pruebas automatizadas en tres niveles de hardware:

1.  **Gama Baja (Android Go / 2GB RAM):** Test de optimización de
    memoria (el juego debe correr a 30 FPS estables).

2.  **Gama Media (Xiaomi/Samsung estándar):** Test de respuesta táctil y
    audio.

3.  **Gama Alta (Google Pixel / Samsung S-Series):** Test de vibración
    HD de alta precisión.
