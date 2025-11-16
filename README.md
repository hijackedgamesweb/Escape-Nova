# GDD - Nova

![PORTADA]()

## ÍNDICE

```
1. Introducción
  1.1 Título del juego
  1.2 Identidad del juego
  1.3 Pilares del Diseño
  1.4 Resumen de Género/Historia/Mecánicas
  1.5 Características

2. Arte
  2.1 Pilares del arte
  2.2 Estética general del juego
  2.3 Apartado visual
  2.4 Concept art
    2.4.1 Sistema solar y planetas
    2.4.2 Personajes y civilizaciones
    2.4.3 Interfaces
      2.4.3.1 Menú de inicio
      2.4.3.2 Menú de opcionnes
      2.4.3.3 Overwview Sistema Solar
      2.4.3.4 Visión "Perfecta" del sistema solar
      2.4.3.5 Astrario
      2.4.3.6 Menú de diplomacia
      2.4.3.7 Menú de misiones
      2.4.3.8 Menú de investigación
      2.4.3.9 Menú de almacén
      2.4.3.10  Menú de habilidades

3. Música
  3.1 Ambiente sonoro
    3.1.1 Main Menu
    3.1.2 Horizon
    3.1.3 Escape

4. Game Design
  4.1 Pilares del Game Design
  4.2 Mecánicas principales
    4.2.1 Gestión de recursos
      4.2.1.1 Recursos
        4.2.1.1.1 Materiales
        4.2.1.1.2 Objetos
        4.2.1.1.3 Tesoros
    4.2.2 Tiempo
    4.2.3 Entridades espaciales construibles
      4.2.3.1 Planetas
      4.2.3.2 Creación de planetas
      4.2.3.3 Satélites
    4.2.4 Storage Terminal and Research Station (S.T.A.R.S)
      4.2.4.1 Almacén Central
      4.2.4.2 Centro de investigación
    4.2.5 Rama de habilidades
      4.2.5.1 Constelaciones
      4.2.5.2 Puntos de habilidad
      4.2.5.3 Nodos
    4.2.6 Civilizaciones
      4.2.6.1 Especies
      4.2.6.2 Acciones
    4.2.7 Eventos
      4.2.7.1 Eventos aleatorios
      4.2.7.2 Eventos semi-aleatorios
      4.2.7.3 Eventos no-aleatorios
    4.2.8 Objetivos
      4.2.8.1 Acciones que pueden afectar a los objetivos
      4.2.8.2 Principales
      4.2.8.3 Peticiones
      4.2.8.4 Hitos
    4.2.9 Sistema político y de interacción
      4.2.9.1 Descubrimiento de otras especies
      4.2.9.2 Acciones con otras especies
      4.2.9.3 Variables de comportamiento de especies y sistemas políticos
        4.2.9.3.1 Acciones del jugador que afectan a las variables de las especies
      4.2.9.4 Sistema bélico
    4.2.10 Astrario
  4.3 Mecánicas secundarias
    4.3.1 Niveles de dificultad
    4.3.2 Agujeros de gusano
    4.3.3 Religión/Creencias
  4.4 Diagramas de flujo
    4.4.1 Menú de inicio
    4.4.2 Partida

5. Interfaces
  5.1 Menú de inicio
  5.2 Menú de partida
  5.3 Menú de opciones
  5.4 Overview del sistema solar
  5.5 Visión "perfecta" del sistema solar
  5.6 Astrario
  5.7 Menú de diplomacia
  5.8 Menú de misiones
  5.9 Menú de investigación
  5.10 Menú de almacén
  5.11 Menú de construcción
  5.12 Menú de habilidades

6. Narrativa
  6.1 Especies
    6.1.1 Los Akki
    6.1.2 Los Halxi
    6.1.3 Los Skulg
    6.1.4 Los Mippip
    6.1.5 Los Handoull
  6.2 Nombres de los personajes
    6.2.1 Los Akki
    6.2.2 Los Halxi
    6.2.3 Los Skulg
    6.2.4 Los Mippip
    6.2.5 Los Handoull

7. Mapa de sistemas
  7.1 Mapa de sistemas General
  7.2 Sistema de Gestión y Recursos
  7.3 Sistema de Construcción y Expansión
  7.4 Sistema de Progresión y Habilidades
  7.5 Sistema de Eventos

8. Sistema de interfaces

9. Monetización

10. Hoja de ruta del desarrollo
  10.1 Plataformas
  10.2 Audiencia
  10.3 Fases de desarrollo
    10.4 Alpha
    10.4 Beta
    10.4 Gold
    10.4 Fecha de lanzamiento

```

> ## 1. Introducción

> ## 1.1 Título del Juego

Se ha decidido que el nombre del juego será, de forma provisional, Nova.
Nova hace referencia a una explosión estelar en un sistema solar, suceso que tendrá lugar al final del juego, a modo de “Game Over”, debiendo el jugador escapar de este sistema antes de que esto suceda para alzarse con la victoria.

> ## 1.2 Identidad del Juego

Nova es un juego de gestión de recursos con una ambientación espacial, donde el jugador debe expandir su propio sistema solar para conseguir escapar del mismo previamente a que este colapse y explote. Para ello, puede construir planetas para conseguir recursos o para conseguir interactuar con otras especies que ayudarán al jugador en su objetivo.

> ## 1.3 Pilares del Diseño

El juego se ha diseñado en base a estos tres pilares:
  - **Gestión de recursos**: Construcción de planetas para obtener recursos, intercambio de materiales, mejoras de diferentes tipos, etc.
  - **Toma de decisiones**: A lo largo del juego sucederán eventos (tanto aleatorios, como programados) que conllevarán la toma de decisiones por parte del jugador, teniendo estas repercusiones directas en el transcurso del juego. Ej. Cambios en la relación con otras especies, destrucción de planetas…
  - **Sensación de urgencia**: Debido a la inminente destrucción del sistema solar, el jugador se encontrará constantemente bajo una sensación de urgencia. Otras misiones y encargos del juego también contarán con un tiempo límite para ser completadas, pudiendo conllevar también consecuencias negativas en caso de no completarlas, reforzando esa sensación.

> ## 1.4 Resumen de Género/Historia/Mecánicas

En Nova, el jugador es el emperador de un sistema solar,  debiendo: recolectar y gestionar recursos, expandir su civilización construyendo y gestionando planetas, y gestionar un sistema político (en la interacción con otras civilizaciones).

Todo este proceso debe llevarse a cabo bajo la amenaza de una inminente supernova que erradicará todo el sistema solar dentro de un plazo determinado de tiempo.

El objetivo principal del juego será, entonces, huir del sistema solar, ya bien sea:
  - Construyendo un artilugio que te permita migrar a otro sistema solar vacío.
  - Erradicando a otra especie y ocupando su sistema solar.
  - Pactando con otras civilizaciones para convivir con ellos en sus respectivos sistemas solares.
Características

De esta manera, las características y elementos únicos que dotan de una identidad propia al videojuego son: 

  - **Gestión y expansión** de un sistema solar.
  - **Ambientación espacial**.
  - **Rama de habilidades** basadas en constelaciones.
  - Estética **Pixel-Art**.
  - **Eventos** semi-aleatorios en **consecuencia de tus acciones**.
  - **Objetivos a corto, medio y largo plazo** que alteran el transcurso de la partida.

> ## 2. Arte

> ## 3. Música

> ## 4. Game Design

> ## 5. Interfaces

> ## 6. Narrativa

> ## 7. Mapa de sistemas

> ## 8. Sistema de interfaces

> ## 9. Monetización

> ## 10. Hoja de ruta del desarrollo
