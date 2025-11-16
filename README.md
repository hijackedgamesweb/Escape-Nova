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

> # 1. Introducción

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

> # 2. Arte

> # 3. Música

> # 4. Game Design

> # 5. Interfaces

> ## 5.1 Menú de inicio
> ## 5.2 Menú de partida
> ## 5.3 Menú de opciones
> ## 5.4 Overview Sistema Solar
> ## 5.5 Visión "Perfecta" del sistema solar
> ## 5.6 Astrario
> ## 5.7 Menú de diplomacia
> ## 5.8 Menú de misiones
> ## 5.9 Menú de investigación
> ## 5.10 Menú de almacén
> ## 5.11 Menú de construcción
> ## 5.12 Menú de habilidades

En este menú podemos ver todas las constelaciones que funcionan como árboles de habilidades. Es importante que quede claro qué árboles de habilidades están bloqueados/desbloqueados, y la posición de los nodos de cada árbol, dado que el jugador debe construir los planetas de forma específica para desbloquear dichas constelaciones, por lo que es importante que quede clara sus posiciones.

Esta interfaz debe tener:
Una visión global de todos los posibles árboles de habilidades.
  - Dos formas de estado para los árboles de habilidades:
    - Bloqueado.
    - Desbloqueado.
 -  Una visión específica para cada árbol de habilidades.
 -  Botones para cambiar entre árboles de habilidades.
 -  “Nodos” para cada posible mejora de cada árbol de habilidades. Cada nodo contiene:I
     - Información sobre la mejora.
     - Coste de la mejora.
     - Un botón para “Desbloquear” el nodo. Este botón debe ser diferente en caso de que el jugador tenga, o no, la cantidad necesaria de puntos para desbloquearlo.
- Cantidad de puntos de habilidad disponibles para usar (En una esquina, por ejemplo).


> # 6. Narrativa

> ## 6.1 Especies

> ### 6.1.1 Los Akki

**Descripción**

Desconocidos. Vienen de un planeta del que se tiene muy poca información. Son muy imprevisibles, parece que se comportan igual que los átomos, según la mecánica cuántica.

**Sistema político**

Desconocido. No se sabe cuál es su sistema, pero se cree que es una anarquía.
(Fuera del lore, nosotros sabemos que son una república).

**Lore**

Entre las incontables especies que habitan el universo, pocas generan tanta especulación como los Akki. Aparecen mencionados en los registros más antiguos y en los informes estelares más modernos, pero siempre envuelta en incertidumbre. Lo poco que sabemos de ellos proviene de observaciones indirectas, trayectorias calculadas y testimonios recogidos por exploradores que jamás lograron establecer un contacto pleno.

Los Akki están vinculados a un planeta lejano: Kepler 452b, un planeta con condiciones muy similares a las de la Tierra. Su cielo parece teñido de un azul oscuro por la composición de su atmósfera, y vastos océanos se reparten entre cordilleras antiguas que, según algunos informes, están cargadas de minerales poco comunes en nuestro sistema solar . Este entorno habría permitido el surgimiento de vida compleja, y posiblemente, de una civilización capaz de los viajes interestelares.

Sin embargo, la conexión entre Kepler 452b y los Akki nunca ha sido confirmada. Su supuesta trayectoria hacia nuestra galaxia, reconstruida a partir de patrones gravitacionales, bien podría haber sido un rastro falso, un modo deliberado de despistar a quienes intentasen seguir sus movimientos.

Los Akki parecen rechazar la exposición directa. Cuando se han dejado ver, lo han hecho de manera fugaz y sin conflicto aparente. Ningún informe recoge hostilidad ni intentos de dominación. Todo lo contrario, ya que su silencio parece calculado, como si solo desearan observar. Algunos historiadores imperiales sostienen que los Akki contemplan a las demás especies del cosmos como parte de un inmenso experimento social y evolutivo. Otros, más suspicaces, creen que esconden secretos que podrían alterar el equilibrio de poderes en más de una galaxia.

Aunque ningún escrito del Documento de Especies del Universo se atrevió a afirmar nada concreto, los estudios externos apuntan a que los Akki no siguen un régimen autocrático. Se habla de una república interplanetaria, donde las decisiones son tomadas en consejos colectivos. Si esto es cierto, su forma de gobierno sería singular dentro de Andrómeda, pues la mayoría de las especies registradas en dicha galaxia parecen regirse por democracias o dictaduras, salvando algunas excepciones.
Esto explicaría en parte su inclinación a la discreción donde un sistema republicano que se expone demasiado corre el riesgo de ser objetivo de civilizaciones peligrosas y agresivas. Su aparente “no intervención” en los asuntos de otras especies podría ser, en realidad, una política consciente de preservación, probablemente porque en el pasado ya tuvieron un altercado que podría haber acabado con su existencia.

En cuanto a sus costumbres, rituales o manifestaciones artísticas, todo son conjeturas. Sin embargo, hay un punto curioso, puesto que en las ocasiones en las que se han mostrado, algunos exploradores aseguran haber escuchado armonías rítmicas que parecían provenir de sus propias voces. Esto llevó a suponer que la música, o al menos las vibraciones sonoras, forman parte de su identidad cultural.

Encima, los símbolos encontrados en algunos fragmentos atribuidos a su paso recuerdan a geometrías fractales, como si la repetición infinita fuese una constante en su pensamiento. Se dice que ven el universo en patrones interminables, en ciclos que nunca se rompen.
La verdad sobre los Akki sigue fuera de nuestro alcance. ¿Son simplemente observadores cósmicos? ¿Se trata de una civilización antigua que perdió interés en expandirse y prefiere custodiar su propio conocimiento? ¿O acaso juegan un papel silencioso en la balanza entre galaxias, interviniendo solo en momentos críticos de la historia?

Sea cual sea la respuesta, una cosa es cierta, y es que la mera existencia de los Akki recuerda a todas las especies que el universo guarda secretos que desafían nuestro entendimiento.

> ### 6.1.2 Los Halxi

**Descripción**

Tóxicos. Su planeta es altamente tóxico, cualquier tipo de interacción con esta especie puede ser mortal.

**Sistema político**

Democracia. Tienen uno de los mejores sistemas de la especie, ya que son capaces de ponerse de acuerdo en todo al tener una red neuronal conexa, lo que quiere decir que todos están conectados mentalmente y son capaces de entender lo que cada uno siente y piensa.

**Lore**

Los Halxi son una especie cuya biología resulta tan peligrosa como fascinante, ya que dependen de un gas que resulta letal para la mayoría de las especies conocidas, y al mismo tiempo, son extremadamente vulnerables al oxígeno y al nitrógeno, dos elementos que para nosotros son sinónimos de vida, pero que para ellos significan dolor y una muerte lenta y agónica. Debido a esta condición, su ecosistema y su sociedad se han desarrollado en un aislamiento casi absoluto, pues el simple contacto físico directo con un Halxi puede acabar en la muerte de cualquier otra especie, del mismo modo que la exposición a nuestro aire acabaría con la suya.

A lo largo de su historia, esta dualidad de fragilidad y letalidad ha marcado profundamente su cultura, ya que no solo han aprendido a mantener un control absoluto sobre su entorno, sino que también han desarrollado una manera de relacionarse con una cautela extrema. El respeto por las distancias físicas se convirtió en una norma social inquebrantable, y el contacto entre individuos se sustituyó por rituales de comunicación a distancia, con señales químicas y sonoras que no requieren proximidad.

En el plano político, los Halxi no siempre tuvieron el sistema que hoy les caracteriza. En sus orígenes se organizaron como una república, sin embargo, las constantes disputas internas y los cambios de modelo de gobierno llevaron a enfrentamientos prolongados, guerras civiles devastadoras y divisiones profundas entre clanes familiares. Estas luchas, conocidas en sus crónicas como un periodo oscuro, destruyeron grandes extensiones de terreno y pusieron en riesgo la supervivencia de la especie, ya que la inestabilidad política se reflejaba directamente en la incapacidad de mantener un desarrollo estable.

Tras siglos de alternancia entre república y democracia, y después de lo que sus registros llaman la Gran Pelea Decisiva, se estableció un nuevo sistema político que transformó por completo a los Halxi. Desde entonces, todas las decisiones deben aprobarse únicamente si existe unanimidad absoluta, ya que consideran que ningún pueblo puede prosperar si una sola voz permanece en desacuerdo. Esta exigencia de consenso total permitió acabar con las divisiones violentas, aunque con el tiempo también se descubrió que el modelo presenta dificultades, ya que la unanimidad, aunque garantiza la paz, puede ralentizar el progreso y frenar decisiones necesarias en momentos de urgencia.

El recuerdo de su pasado turbulento ha mantenido a los Halxi firmes en esta convicción, pues prefieren un avance lento y consensuado a correr el riesgo de regresar a la violencia interna que casi los destruye. Su sociedad vive con la memoria constante de aquellas disputas y con la certeza de que la unidad, aunque imperfecta, es la única manera de evitar la extinción que un día estuvo demasiado cerca.

En sus primeros tiempos, los Halxi vivieron organizados en clanes aislados, cada uno dominando una región y defendiendo sus recursos con una ferocidad constante. Los conflictos eran inevitables, ya que los clanes disputaban tanto los territorios más ricos en el gas que les daba vida, como las tierras donde crecían hongos y algas que servían de alimento. Estas guerras primitivas no tenían un fin político claro, eran luchas por la supervivencia inmediata, y en muchas ocasiones acababan con la destrucción de zonas enteras que quedaban inhabitables por generaciones. A este periodo se le conoce como la Era de los Venenos, ya que las armas eran simples mezclas químicas
que se lanzaban sobre los rivales, devastando tanto al enemigo como al propio terreno. Algunas de estas armas estaban hechas con fragmentos de oxígeno, todavía se desconoce el origen de este elemento, y cómo se incorporó dentro del planeta.
Con el paso de los siglos, y tras pérdidas constantes, algunos líderes comenzaron a plantear la necesidad de acuerdos que evitaran la autodestrucción, así nacieron los primeros pactos de tregua entre clanes, que consistían en dividir territorios sin invadirlos y respetar las reservas de gas de cada comunidad. Uno de los primeros documentos registrados es el Pacto de Tharal-xi, considerado un antecedente de su política moderna, donde por primera vez se estableció que ninguna decisión de expansión o guerra podía tomarse si todos los clanes no estaban de acuerdo. Aunque aquel pacto duró poco y terminó rompiéndose, su eco quedó grabado en la memoria cultural de los Halxi, como una idea a la que siempre volverían.

La república fue el primer sistema centralizado que intentaron instaurar, uniendo varios clanes bajo representantes elegidos. Sin embargo, esta etapa estuvo marcada por continuas disputas sobre quién debía gobernar, qué regiones tenían más peso en las votaciones y cómo se debían repartir los recursos. La falta de confianza entre clanes condujo a que cada ciclo de gobierno se viera interrumpido por acusaciones, traiciones y, finalmente, rebeliones abiertas. A este tiempo se lo conoce como los Años de Fragmentación, ya que ningún sistema político conseguía perdurar y los cambios constantes debilitaban a toda la especie.

El punto más oscuro llegó con la Gran Pelea Decisiva, un conflicto devastador que no solo enfrentó a clanes y regiones, sino también a familias enteras que se dividieron por lealtades diferentes. Durante generaciones, la sociedad Halxi se desgarró a sí misma, ciudades enteras fueron abandonadas, y la población se redujo a cifras alarmantes. Los crónicos describen este tiempo con frases estremecedoras, como “hermano contra hermano respirando el mismo veneno”, reflejando que ni los lazos de sangre lograron resistir la era de odio. En aquel periodo, surgieron voces que pedían unidad, aunque fueron inhibidas por la violencia.

No obstante, incluso en esos momentos de desesperanza, algunos líderes mantuvieron la fe en que una nueva forma de gobierno podría salvar a los Halxi. Entre ellos destacó la figura de Theral-xi, un visionario que durante los últimos años de la Gran Pelea reunió a representantes de todos los bandos y pronunció las palabras que se convirtieron en lema de su pueblo: “No hay victoria posible si un Halxi es condenado al silencio, no hay futuro posible si la muerte se convierte en el precio de nuestras disputas”. Con este discurso, Theral-xi consiguió lo que nadie había logrado, convencer a todos los clanes de que solo un sistema basado en la unanimidad podría evitar que las viejas heridas volvieran a abrirse.

Así se fundó la democracia Halxi, que no se parece a ninguna otra conocida en el cosmos. En ella, cada decisión, por pequeña que sea, requiere el acuerdo de todos, desde el ciudadano más humilde hasta el líder de la presidencia. Este modelo, aunque lento y en ocasiones problemático, permitió la reconstrucción del planeta y el fin definitivo de las guerras civiles. Se redactó entonces el Tratado de Ehnar-xi, considerado la piedra angular de su cultura política, donde quedó escrito que jamás volverían a tomar una decisión sin que todas las voces fueran escuchadas y aceptadas.

Tras siglos de oscuridad, los Halxi entraron en un periodo de lucidez, donde su sociedad comenzó a florecer. Las ciudades se reconstruyeron, surgieron academias de ciencia dedicadas al estudio de su atmósfera y al perfeccionamiento de tecnologías para mantener su delicado equilibrio vital, y la cultura se volcó en recordar el pasado para no repetirlo. Sus obras de arte, sus escritos y su música están impregnados de una misma idea, la de la unidad absoluta como salvación.

No obstante, con el paso del tiempo también se revelaron las limitaciones de este sistema, ya que la unanimidad ralentizaba cualquier avance y en ocasiones impedía reaccionar con rapidez a amenazas externas. Algunos críticos señalaban que el exceso de consenso podía ser tan peligroso como la división del pasado, sin embargo, el peso de la memoria colectiva y el miedo a repetir la Gran Pelea mantiene a los Halxi firmes en su decisión, convencidos de que preferirán siempre la lentitud a la violencia.

La historia de los Halxi es, en definitiva, una sucesión de catástrofes y renacimientos, de veneno y esperanza, de guerras que casi los borraron de su planeta y de pactos que los salvaron en el último momento. Sus frases célebres, como la de Theral-xi, resuenan todavía en cada asamblea, recordándoles que no hay mayor victoria que escuchar todas las voces, y que su verdadero triunfo no fue vencer en las guerras, sino aprender a dejar de librarlas.

> ### 6.1.3 Los Skulg

**Descripción**

Peligrosos. Sus intenciones no son buenas, hay que vigilarlos de cerca.

**Sistema político**

Dictadura. Tienen un líder centrado en la gestión de los militares, encargados de atacar diferentes planetas, con la intención de dominarlos.

**Lore**

Desde el primer momento, su sociedad se rigió en torno a una sola idea, una idea que no pregunta ni discute, una idea que exige obediencia y resultados, y esa idea la sembró Skudig, un dictador que no toleró la duda y que convirtió a su pueblo en una máquina de conquista. Bajo su mando se abolió la vida civil tal y como la entendemos, todo se subordinó a la guerra, la economía pasó a alimentar arsenales, la ciencia se volvió ingeniería de asedio, y la religión se transformó en culto a la victoria, todas las piezas encajaron con la precisión de engranajes que no conocen fallos.

Su progresión fue una estrategia de varias capas pensada para dominar y mantener el dominio. Primero consolidaron su mundo natal, eliminando desacuerdos y profesionalizando la fuerza, convirtiendo a la población en personal militar y técnico, con entrenamiento, disciplina y una cultura que celebra la obediencia. Luego transformaron su sistema solar en fábrica y fortaleza, cada planeta especializado en un rol logístico y mineral, y desde ahí proyectaron poder, enviando oleadas de conquista que no buscaban meramente ocupar, sino integrar.

Cuando los Skulg atacan un sistema, no improvisan, aplican una rutina fría y metódica, primero aseguran la superioridad orbital, luego aíslan al objetivo, cortan suministros e infraestructuras críticas, y paralelamente lanzan operaciones psicológicas para quebrar la voluntad de resistencia. Ofrecen que se rindan, ello no es debilidad, es táctica, porque si aceptas ser integrado te conviertes en una pieza útil, si te resistes te muestran con claridad cuál será el precio de la obstinación, el cual no es un final deseado. La ocupación Skulg es eficiente, transforma economías, reasigna élites y reorienta recursos para servir a la metrópoli, y lo más peligroso, establece colonias militarizadas que se convierten en trampas logísticas para cualquier fuerza que intente revertir la conquista.
Su tecnología es práctica y diseñada para imponer, no para el lujo. Tienen flotas multitudinarias de asalto, naves capaces de proyectar potencia orbital sostenida, plataformas de control que pueden desconectar redes de defensa y cortar comunicaciones, y sistemas de vigilancia que rastrean patrones políticos y sociales con una frialdad que recuerda al funcionamiento de un algoritmo. También cuentan con herramientas destinadas a la supresión de resistencias, no como espectáculo sangriento, sino como método, donde nos encontramos con jaulas logísticas, bloqueos de suministros, sabotaje de sistemas de soporte, ingeniería inversa de defensas planetarias para convertirlas en trampas que impidan la recuperación del enemigo.

En sus campañas no faltan los ejemplos de eficacia brutal, planetas que cayeron en semanas tras cerco sistemático, sistemas que fueron transformados en astilleros bajo control Skulg en menos de un ciclo planetario, y colonias que, tras la integración, enviaron recursos para la siguiente ola de expansión. Así fue como durante eones fueron tejiendo una red, y por eso son temidos, porque no atacan solo por saquear, atacan para incorporar, y cada mundo nuevo aumenta su capacidad de proyectar fuerza.

Ahora, sobre lo que pueden llegar a hacer, piensa en esto con frialdad estratégica, porque lo que ves en su historial es aplicable a cualquier objetivo ya que pueden paralizar una región de la galaxia si neutralizan nodos logísticos, pueden cortar el comercio interestelar, pueden infiltrar redes de poder y reemplazar líderes por administradores leales, y si se lo proponen, pueden acabar con sistemas enteros hasta forzar una rendición total y aceptar las condiciones del conquistador. No es fantasía, es cómo funciona su maquinaria, paso a paso, cálculo tras cálculo, movimiento tras movimiento.

Pero no son invencibles, y te lo digo porque conocer sus límites es la base de cualquier defensa. Su modelo depende de cadenas de suministro extensas, de gestión de minerales y de control de infraestructura. Si les cortas el acceso a recursos críticos, si frenas su capacidad de reponer naves o reclutar material humano, su expansión se ralentiza y su estabilidad interna se puede tensar hasta ceder. Además, su centralismo los hace vulnerables a golpes precisos contra centros de mando, a campañas de sabotaje que degraden su logística, y a alianzas que cierren rutas de avance. Pero no hay que olvidar la convicción absoluta de toda la población, no puedes solo acabar con el líder, ya que la lealtad es el punto más fuerte de esta especie.
Una frase célebre del Skudig fue: “Sometemos la galaxia, por derecho y por hierro”. Frase interesante, porque no muestra frialdad, muestra convicción.

Sus primeras campañas se centraron en su sistema natal, y en la batalla de Skerog demostraron su capacidad para aplicar estrategias rápidas y decisivas, aislando facciones rebeldes y obligando a la rendición o sustitución de los líderes por administradores leales. Cada mundo conquistado debía integrarse plenamente a su maquinaria militar y administrativa, con recursos minerales y armamentísticos, infraestructura y población reorganizados para sostener la expansión posterior.
Una vez consolidado su sistema natal, los Skulg iniciaron la expansión estelar inicial, y en la batalla de Teyrran’s Skering tomaron un cinturón de planetas estratégicos en menos de un año, utilizando cada mundo conquistado como base para operaciones futuras, lo que se convirtió en un sello distintivo de su forma de operar. Su capacidad para coordinar flotas, recursos y tropas permitió que sistemas enteros cayeran antes de que la resistencia pudiera organizarse, y su planificación logística aseguraba que ningún territorio quedara sin supervisión. Esto lo conseguían puesto a que incorporaban en su sistema militar a las diferentes civilizaciones secuestradas, obligándoles a formar parte de dicho sistema, y siendo fieles al dictador.
Los logros de los Skulg se multiplicaron con el tiempo, conquistando sistemas enteros, y cuando alguna especie se resistía de manera prolongada, aplicaban el método conocido como “corte del liderazgo”, eliminando a los mandos y tomando el control absoluto de las instituciones, dejando solo la infraestructura y población necesarias para sostener la próxima fase de expansión. Un ejemplo extremo fue el del planeta de Vethar, cuya población resistió y desapareció de los registros galácticos, mientras su planeta se transformaba en un nodo Skulg. Se habla de que la civilización de Vethar sufrió el peor de los destinos, pero con cómo funciona la campaña militar y dictatorial, se pinta todo con rosas, pero la verdad es que fue la batalla más dura de los Skulg, ya que la civilización atacada opuso una gran fuerza campal, y se acercaron mucho a acabar con una parte muy importante de los Skulg.

Entre sus campañas más reconocidas se encuentran el asalto a Korrat, donde bloquearon el sistema entero y desactivaron comunicaciones estratégicas, y la ofensiva de Leyrrant, donde integraron planetas completos en su estructura administrativa y establecieron colonias militarizadas que sirvieron como bases de operaciones para nuevas conquistas. Cada victoria aumentaba su influencia, sus recursos y su capacidad de proyectar poder, consolidando su reputación como la especie más temida de la galaxia.

> ### 6.1.4 Los Mippip

**Descripción**

Amigables. Tienen la mejor de las intenciones, siempre quieren ayudar, y se nota que son empáticos con afán de prosperar como especie, mezclándose con otras.

**Sistema político**

Comunistas. Son conocidos por su empatía, y creen que la mejor manera para gestionar un planeta es consiguiendo que todos tengan los mismos derechos, tanto sociales, como materiales.

**Lore**

Los Mippip son una especie famosa por su bondad y por la armonía de su sociedad, sin embargo esa misma perfección provoca recelo y envidia en muchas civilizaciones, por eso son a la vez los más queridos y los más odiados, su rasgo más distintivo es una conexión cerebral colectiva que permite compartir pensamientos y emociones sin necesidad de palabras, lo que ha hecho posible una organización social comunista que funciona con una coherencia casi perfecta, y gracias a esa conexión han diseñado instituciones que priorizan el bienestar común, la educación universal, la gestión sostenible de recursos y la resolución de conflictos por consenso, lo que atrajo migración desde muchos mundos y convirtió su planeta en un refugio deseado por quienes buscan una vida diferente.

La historia de los Mippip puede contarse como una sucesión de hitos donde la empatía organizada produjo cambios que trascendieron generaciones, inicialmente surgieron como grupos dispersos que aprendieron a sincronizar sus pensamientos mediante técnicas meditativas y neuroenlaces rudimentarios, luego consolidaron la Gran Compartición, un proceso social que amplió la red neuronal colectiva a toda la población, y con ello nació la primera forma de planificación completa, planificación que permitió eliminar la pobreza material interna, acabar con la violencia estructural y crear una economía dedicada a la abundancia compartida en vez de a una acumulación de objetos y pertenencias individual.

Con el paso del tiempo los Mippip se volvieron un factor clave en la diplomacia interplanetaria, formando alianzas que respondían tanto a motivos “humanitarios” como estratégicos, entre esas alianzas destacó el Pacto de Lumenip, una coalición de mundos costeros dedicada a proteger rutas migratorias y a coordinar ayuda en catástrofes, también integraron la Concordia de Asterip, una red de intercambio científico que facilitó medicinas, tecnologías de purificación y técnicas de restauración ecológica, sus convoyes humanitarios conocidos como las Caravanas Brillantes llevaron suministros y personal especializado a zonas devastadas por guerras o desastres, y en muchos archivos galácticos los Mippip aparecen como mediadores efectivos cuando la telepatía colectiva les permitió detectar malentendidos antes de que escalaran en conflictos abiertos.

Aun así su estabilidad no fue siempre tan sólida, en su historia existen momentos de tensión que llevaron a la promulgación de normas estrictas para proteger la integridad de la red mental, entre esas reglas la más controvertida fue la prohibición de mezclarse genéticamente con especies externas, una norma adoptada para preservar lo que ellos llaman la Dinastía, la razón oficial sostiene que la mezcla puede alterar los patrones de empatía e individualidad que sustentan la cooperación, y algunos veteranos con siglos de vida advierten que incluso pequeños cambios en la arquitectura neurológica podrían fracturar la red, por eso la política reproductiva fue regulada, y por eso la prohibición se volvió un tabú social que pocos se atreven a cuestionar públicamente.

A nivel político y cultural su influencia se extendió también a la creación de instituciones educativas interplanetarias, bibliotecas abiertas y protocolos de cuidado emocional que otras especies adoptaron parcialmente, su música y su arte se propagaron como terapia cultural, y su forma de gobierno basado en asambleas conectadas por la red sirvió de modelo para pequeñas comunidades que aspiraban a replicar su utopía a escala local.

Sobre la cuestión de por qué los Skulg nunca han conquistado abiertamente su planeta existen varias teorías que circulan entre historiadores y espías, la primera teoría sugiere que los corredores estratégicos de los Skulg simplemente no han pasado lo bastante cerca en órbitas peligrosas, lo cual resulta poco probable dado que los Skulg controlan rutas vecinas, la segunda teoría plantea que los Skulg consideran inútil someter a una población que no aporta mano de obra adecuada a su maquinaria bélica, esto podría tener sentido si el valor estratégico del planeta fuera bajo para la economía militar skulg, y la tercera teoría, la más difundida y la que muchos creen más verosímil, afirma que hubo un acuerdo secreto entre ambas civilizaciones.

Esa teoría más probable mantiene que en algún momento remoto los Skulg sí llegaron al planeta Mippip, donde aparentemente, se rompió la regla de la no reproducción, y que de esas uniones nacieron descendencias híbridas, esta versión aparece sobre todo en relatos clandestinos y en documentos filtrados, sin embargo no hay pruebas públicas irrefutables, por eso la historia permanece en el terreno de la conjetura y la acusación, lo que sí es cierto es que la idea provoca una dimensión moral y política compleja, dado que los Mippip mantienen una conexión mental global, la presencia de híbridos implicaría efectos impredecibles y, probablemente, caóticos, sobre sus redes conexas.

Si se confirma la hipótesis de hibridación, las consecuencias posibles son varias y de gran alcance, al unir arquitecturas neurológicas distintas podría surgir una nueva dinámica mental, por un lado la mezcla podría corroer la uniformidad empática, generando disonancias que fragmenten la red, por otro lado podría introducir nuevas perspectivas que enriquezcan la red con capacidades cognitivas distintas, esa dualidad es la que aterra a los veteranos y fascina a los teóricos de la evolución social y biológica, además existe el riesgo de que híbridos con lealtad dividida se conviertan en palancas políticas, usados por potencias externas para manipular o debilitar el consenso Mippip, o bien podrían actuar como puente entre culturas, facilitando alianzas inéditas.

Las implicaciones prácticas también incluyen debates sobre ciudadanía, si los híbridos son reconocidos como Mippip plenos o como una nueva casta, y cómo integrar mentalmente a individuos con componentes genéticos de otra especie sin romper la red.

A lo largo de su historia los Mippip forjaron alianzas que los hicieron relevantes más allá del idealismo, su contribución médica con técnicas de sincronización neural salvó poblaciones enteras de plagas psicosomáticas, su capacidad para mediar en conflictos interestelares los colocó como garantes de tratados de paz temporales, y su política de abrir las puertas a refugiados durante las Guerras de Sundarip consolidó su imagen “humanitaria”, esas acciones explican por qué muchos sistemas los defienden políticamente, y por qué el rumor sobre los Skulg y la supuesta hibridación tiene efectos inmediatos sobre las relaciones galácticas.

Finalmente, la personalidad colectiva de los Mippip se define por una mezcla de ternura institucionalizada y rigor ético, ellos actúan como comunidad que cuida, pero también como guardas de una tradición que temen perder, esa paradoja alimenta la envidia de quienes desearían una sociedad semejante y la hostilidad de quienes se sienten moral o materialmente desplazados por su ejemplo, de modo que el planeta de los Mippip sigue siendo un foco de división, admiración y sospecha en la galaxia, y la verdad sobre los Skulg, si alguna vez se hace pública, podría reescribir el carácter completa de su comunidad.

> ### 6.1.5 Los Handoull

**Descripción**

De poco fiar. Hay que tener cuidado, ya que son unos profesionales de la manipulación, y nunca se sabe cuáles son sus verdaderas intenciones.

**Sistema político**

No se sabe. Dicen que son democráticos, pero parece más una dictadura.

**Lore**

Los Handoull se conocen como maestros de la manipulación, su nombre inspira desconfianza y fascinación a partes iguales, y su poder real radica en dos cosas, primero en la posesión de un material escaso y casi imprescindible en industrias avanzadas, y segundo en su habilidad para convertir cualquier trato en una herramienta a su favor, por eso muchos planetas negocian con ellos, aunque sus traiciones sean proverbiales.

Su sociedad se estructuró alrededor de clanes comerciales que funcionan como estados dentro del estado, cada clan maneja rutas, almacenes, tecnologías y redes de influencia, y aunque públicamente mantienen códigos de comercio que parecen rígidos y honorables, en la práctica esos códigos son líneas maestras que les permiten interpretar las leyes de otros mundos a su conveniencia, así operan desde la sombra, con contratos que dejan siempre una cláusula oculta.
El material que comercian se llama cristal de cuarzo, un mineral cristalino que posee propiedades únicas para la construcción de vínculos neuronales artificiales y para la estabilización de matrices cuánticas de transporte, sin cristal de cuarzo muchas de las tecnologías de comunicación, de medicina neural y de propulsión de salto pierden eficiencia de forma dramática, por consiguiente controlar el flujo de cristal de cuarzo equivale a controlar la infraestructura básica de muchas civilizaciones, y eso convierte a los Handoull en interlocutores necesarios, aunque peligrosos.

Sus métodos de manipulación son variados y refinados, usan el encanto diplomático como “pre-trampa”, ofrecen ayuda tecnológica que viene con dependencia técnica, financian partidos políticos que luego compran favores, infiltran redes culturales con artistas y sacerdotes que propagan ideas que suavizan la aceptación de cambios legislativos, en el terreno económico crean monopolios encubiertos donde el precio sube y baja según la conveniencia de una operación particular, asimismo mantienen bancos de información que conocen los puntos débiles de gobernantes, y con esa inteligencia operan chantajes, o directamente fuerzan acuerdos que parecen voluntarios.

Existen historias que se cuentan en los registros diplomáticos y en los pasillos de los consulares, historias que explican por qué a pesar de la traición la gente vuelve a negociar con los Handoull, una de las más citadas se denomina la Negociación de Glass Harbor, donde un sistema entero estaba al borde de colapso por una plaga neural que afectaba a las redes de enlace, los Handoull llegaron ofreciendo cristal de cuarzo refinado y protocolos de restauración, salvaron ciudades enteras en semanas, y al mismo tiempo firmaron una cláusula de suministro que ató al sistema a pagos a futuro y a concesiones territoriales, muchos recuerdan la ayuda como un milagro, y otros recuerdan la cláusula como prisión, de ese modo, mano amable y mano firme quedaron unidas para siempre.
Otra historia relevante es la Traición de Rensaar, donde un consorcio de mundos pequeños confió en un acuerdo de defensa común, los Handoull midieron la financiación y suministraron materiales para fábricas enfocadas en elementos de defensa, y cuando llegó el conflicto mayor, la red de suministros fue direccionada a un actor externo con el que los Handoull tenían un pacto, la defensa de Rensaar cayó por falta de repuestos, y en los archivos se encontró luego el contrato firmado con lenguaje ambiguo, esa operación enseñó a muchos a desconfiar, pero también enseñó a otros a aceptar el riesgo, porque Rensaar había tenido apoyo tecnológico y eso permitió a sus ciudadanos emigrar a mundos aliados con recursos, por tanto el sabor fue mixto, beneficio inmediato para la población, trauma institucional para sus gobernantes.

Con las especies que ya conocemos, los Handoull han tenido encuentros que combinan utilidad y deslealtad, con los Mippip se establecieron rutas humanitarias y comerciales que trajeron prosperidad a refugiados, los Mippip valoraron la eficiencia logística de los Handoull, y sin embargo entre las leyendas circula una teoría que coloca a intermediarios como facilitadores en tratos oscuros relacionados con la hibridación y las desapariciones, nadie ha demostrado de forma pública la implicación directa de los Handoull en la supuesta prostitución, pero su papel de corredores de recursos los hace sospechosos por capacidad y hábito, con los Skulg los Handoull negociaron y a veces facilitaron suministros que los Skulg necesitaban para mantener su maquinaria de guerra, esa relación fue pragmática y peligrosa, los Handoull supieron, a menudo, equilibrar ventajas mutuas con posibilidades de traición calculada, y con los Halxi y los Akki actuaron como puente comercial, aprovechando la rareza de cristal de cuarzo para proveer tecnologías adaptadas a ecosistemas sensibles y, a la vez, extrayendo cuotas de influencia.

¿Por qué entonces siguen confiando en ellos algunos, aunque no todos, y aunque los antecedentes sean oscuros, la explicación es múltiple y económica, política y cultural a la vez? Primero, la ley de la necesidad, cuando el cristal de cuarzo es imprescindible la elección se vuelve pragmática, y los líderes calculan que los beneficios superan los riesgos, segundo, la reputación selectiva, los Handoull ofrecen resultados verificables en plazos cortos, y en entornos donde el tiempo importa más que la moral, eso pesa mucho, tercero, la estructura de recompensas y castigos, los Handoull mantienen redes de seguridad con mercenarios, con archivos comprometedores y con aliados en posiciones clave, por lo que traicionarlos puede costar caro, y cuarto, la alternativa no siempre es mejor, cuando no hay otros proveedores el actor racional opta por el mal menor.

Además, existe un componente psicológico, los Handoull son expertos en crear narrativas que minimizan su traición, presentan cada ruptura como un malentendido, o como una evolución necesaria del contrato, y acompañan la traición con gestos de reparación que funcionan como amortiguadores de ira, esas acciones condicionan la memoria política de algunas sociedades, provocan amnesia selectiva sobre los daños causados, y por eso, en ciertas generaciones la relación se normaliza.
No todos confían en los Handoull, y en algunos sectores la desconfianza se institucionalizó, existen coaliciones de seguridad que bloquean sus importaciones, gremios de mercaderes que boicotean sus rutas y tribunales interestelares que procesan contratos fraudulentos cuando emergen pruebas, pero incluso esas instituciones a veces negocian en secreto, porque el cristal de cuarzo abre posibilidades tecnológicas que otros recursos no permiten.

En la práctica los Handoull ofrecen tres tipos de trato, entrega directa a mercados regulados, acuerdos de co-producción donde retienen la propiedad intelectual y la mayor parte del beneficio, y pactos de asistencia estratégica donde su apoyo viene condicionado a concesiones políticas, y cada formato implica distintas garantías y distintas posibilidades de traición, por tanto, las especies aprenden, a veces con dolor, a elegir el formato según su fuerza y su paciencia.
Existen también relatos de honor inesperado, episodios donde los Handoull rechazaron un beneficio enorme para mantener una relación a largo plazo con un socio que les proporcionaba algo intangible, información privilegiada sobre rutas de salto, bases encubiertas para sus carteras, y esos ejemplos sirven como excusa de confianza, los comerciantes suelen citar esas ocasiones para argumentar que negociar con ellos puede ser rentable si se hace con astucia.

Finalmente, su habilidad para manipular es también su fragilidad, quien logra cortar sus rutas de información y neutralizar sus redes de chantaje obliga a los Handoull a actuar con más transparencia, y cuando varios actores coordinan sanciones y alternativas tecnológicas, la influencia Handoull se reduce, por eso existen grupos de presión que promueven la apertura de mercados y la inversión en materiales sustitutos, porque debilitar la posición de los Handoull resulta estratégico para reducir vulnerabilidades sistémicas.
En resumen, los Handoull son a la vez necesarios y peligrosos, su control del cristal de cuarzo los convierte en proveedores imprescindibles, su destreza diplomática los hace capaces de soldar alianzas, y su inclinación a la traición transforma cada trato en una ecuación de riesgo, por eso sus relaciones con otras especies son complejas, a veces duraderas, a veces breves y llenas de amargura, y por eso en el tejido político de la galaxia su presencia marca el interés pragmático, dejando un legado que genera riqueza, desconfianza y debates sobre cuánto se debe confiar en aquellos que dominan lo que todos desean.

> ## 6.2 Nombres de personajes

> ### 6.2.1 Los Akki

Empiezan por Akk’ y luego el nombre. 
Ejemplo: Akk’Olix, su nombre es Olix.

> ### 6.2.2 Los Halxi

Acaba su nombre por “-xi”. 
Ejemplo: Alio-xi, su nombre es Aljo.

> ### 6.2.3 Los Skulg

Empiezan por Sk y acaban en g. 
Ejemplo: Skunog, su nombre es uno.

> ### 6.2.4 Los Mippip

tienen Mip en el nombre. 
Ejemplo: Mipanteo, se llama anteo.

> ### 6.2.5 Los Handoull

Tienen ‘ll al final del nombre. 
Ejemplo: Hull’oll, y si fuera mujer, Hull’all.

> # 7. Mapa de sistemas

A lo largo de esta sección se definirá el mapa de sistemas principales del juego y luego se desglosan los subsistemas de cada sistema principal.

> # 8. Sistema de interfaces

El sistema de interfaces y estados del juego se compone por:
  - **BaseUIScreen**: Una clase base de la que heredan las distintas UIs del juego, MainMenuScreen, InGameScreen, PauseScreen… Estas clases se pueden componer de distintos botones, componentes, sliders, etc. Y son las UI que el GameStateFactory asignará a cada estado.
  - **GameStateFactory**: Clase que genera los distintos estados distintos evitando la necesidad de pasar estar pasando constantemente una UI y el stateManager en cada constructor del estado.
  - **UIRepository**: Una clase central que recoge todos los BaseUIScreen del Canvas y que tiene una función para devolver la UI deseada. Esta clase sirve para registrar las UI de cada estado en el factory desde el GameManager.
  - **StateManager**: Se encarga de settear y obtener el estado actual.

Para registrar un nuevo estado con su nueva UI se siguen los siguientes pasos:
  - Se crea un nuevo estado que herede de AState.
  - Se crea una nueva UIScreen que herede de BaseUIState.
  - Se registra la UI al nuevo estado con la funcion RegisterUI del stateFactory en el GameManager.
  - Se añade el estado nuevo posible a crear en el Create<T> del StateFactory
  - En el GameObject de la UI se añade la clase screen y se referencian los componentes necesarios.


> # 9. Monetización

En lo relativo al modelo de monetización del juego, se ha decidido que siga un modelo Free to Play, pudiendo acceder de forma gratuita a todos los contenidos del juego. En caso de continuar el desarrollo del juego después del transcurso de la asignatura, ampliando sus contenidos y su alcance, se optará por un modelo Buy to Play, contando con un pago único por una copia digital del videojuego.

Sin embargo, entre tanto, se plantean diversas opciones para lograr beneficio incluso si el juego sigue un modelo Free to Play.

La primera es un sistema de donativos, implementado por la propia página de Itch.io, que permite a los jugadores elegir si quieren descargar el juego gratuitamente o donar una cantidad de dinero a su elección a los desarrolladores.

Se plantea también la posibilidad de vender, de forma separada, un álbum con la música del juego y un libro de concept art con los diseños y bocetos de las distintas interfaces, personajes, etc… que se realizen durante el desarrollo, con anotaciones de los artistas o de los game designers para dar algo más de contexto y contenido de interés para los jugadores más apasionados por el producto que busquen conocer más acerca del desarrollo del mismo. Ambos productos serían vendidos de forma digital a través de la página de Itch.io, a modo de pago único por cada una o una versión con ambos productos por un precio menor a la compra de ambos por separado.

> # 10. Hoja de ruta del desarrollo

> ## 10.1 Plataformas

  - Itchio (Web)      

> ## 10.2 Audiencia

  - **21-32 años**
  - Jugadores **experimentados**
  - **Intereses**: Buscan la perfección mecánica y la comprensión de las mecánicas a un nivel muy profundo para optimizar su estrategia de juego.

> ## 10.3 Fases de desarrollo

> ### 10.3.1 Alpha

Para esta primera fase del desarrollo, se elaborará el GDD a partir del cual se desarrollará el producto completo. Además, se programarán los sistemas base del proyecto, los cuales cimentarán todo el desarrollo. Se desarrollará también una demo técnica que permitirá ver una versión semi-funcional del videojuego para comprender las mecánicas básicas.

> ### 10.3.2 Beta

Para la versión Beta se desarrollarán todos los sistemas de juego hasta el punto de poder jugar una partida completa de principio a fin.

> ### 10.3.3 Gold

Para la versión Gold se trabajarán todos los errores y se harán los ajustes necesarios a las mecánicas para garantizar su funcionalidad. Además, de haber tiempo, se podrá implementar contenido extra, como finales alternativos, misiones o nuevos hitos.

> ### 10.3.4 Fecha de lanzamiento

El juego se publicará a mediados-finales de diciembre. Se publicará a través de Itch.io de forma gratuita para poder ser descargado y jugado a través de navegador.
