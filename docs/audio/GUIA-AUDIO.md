# Guía de Audio — Vigilia

Esta guía es para el PO: acá está **todo lo que hay que componer** para el juego, con
carpeta, duración, si necesita loop y qué tiene que transmitir cada pieza. El objetivo
es que puedas abrir tu programa de música hoy mismo y empezar a producir, **sin esperar
a que el sistema de audio esté programado** (llega en el Sprint 7, épica E6).

No hace falta saber nada de Unity ni de código para usar esta guía.

---

## 1. Dónde va cada archivo

Los archivos de audio del juego viven en dos carpetas:

| Carpeta | Qué va ahí |
|---|---|
| `Assets/Audio/Musica` | Las 6 pistas de música (sección 2) |
| `Assets/Audio/SFX` | Los efectos de sonido (sección 3) |

**Hoy esas carpetas todavía no existen** en el proyecto (el sistema de audio se
construye en el Sprint 7). Eso no es un problema para vos: podés **componer ya**
siguiendo esta guía, y cuando el sistema esté armado, poner tu música va a ser así
de simple:

1. Copiás el archivo terminado a `Assets/Audio/Musica` o `Assets/Audio/SFX`, según
   corresponda.
2. Lo arrastrás al campo que le corresponde en el Inspector de Unity (por ejemplo,
   el campo "Música de menú" en el objeto que maneja el audio del menú).
3. Listo — suena. No hay que tocar código ni pedirle nada a un programador.

**Convención de nombre de archivo** (para que no haya dudas de cuál es cuál):
usá minúsculas, sin espacios ni tildes, y el prefijo `musica_` o `sfx_`. Ejemplos:
`musica_menu.ogg`, `musica_arena.ogg`, `sfx_espadazo.wav`, `sfx_moneda.wav`.

---

## 2. Música — lista exacta de pistas

Seis pistas en total. Todas menos victoria y derrota van en loop, porque suenan
mientras el jugador está en una pantalla o situación de duración variable.

| Pista | Momento del juego | Duración sugerida | ¿Loop? | Qué tiene que transmitir |
|---|---|---|---|---|
| **Menú** | Pantalla de inicio (Crear sala / Buscar sala) | 60-90 s | Sí | Misterio tranquilo, invita a entrar; es lo primero que escucha el jugador, no puede cansar en loop |
| **Sala de espera** | Mientras se espera a que se sumen jugadores y el host apriete Empezar | 30-60 s | Sí | Más liviana que el menú, algo de anticipación ("ya casi arrancamos"), sin tensión todavía |
| **Arena** | Las 10 oleadas normales (oleadas 1 a 10) | 60-90 s | Sí | Tensión sostenida, oscura, que aguante escuchada en loop durante una partida de 5-7 min sin volverse repetitiva ni cansadora |
| **Jefe** | Oleada 11, la pelea final | 60-90 s | Sí | Más intensa y urgente que la de arena; tiene que sentirse como "esto es lo último y lo más difícil" |
| **Victoria** | Pantalla de victoria, al derrotar al jefe | 5-15 s | No | Triunfo, cierre positivo, corto y directo |
| **Derrota** | Pantalla de derrota, al caer todos los jugadores | 5-10 s | No | Cierre sombrío pero breve, sin humillar al jugador — invita a reintentar, no a rendirse |

---

## 3. Efectos de sonido — lista exacta, por familia

17 efectos en total, agrupados en 4 familias.

### Del jugador

| Efecto | Cuándo suena | Duración aprox. | Qué tiene que transmitir |
|---|---|---|---|
| Espadazo | Al ejecutar el ataque con espada (cualquier dirección) | 0.2-0.4 s | Golpe seco y rápido, sin eco largo (se repite mucho) |
| Salto | Al saltar | 0.2-0.3 s | Impulso liviano |
| Recibir daño | Al perder un punto de vida | 0.3-0.5 s | Dolor/impacto, distinto y más fuerte que el espadazo para que se note que "me pegaron a mí" |
| Caer (quedar caído) | Al llegar a 0 de vida y quedar arrodillado | 0.5-1 s | Momento grave, de derrota personal — distinto a "recibir daño" |
| Revivir | Al ser revivido por un compañero | 0.5-1 s | Alivio, algo esperanzador |
| Disparar | Al usar un arma de disparo dropeada | 0.2-0.3 s | Distinto en timbre al espadazo, para que se note el cambio de arma sin mirar el HUD |

### De los enemigos

| Efecto | Cuándo suena | Duración aprox. | Qué tiene que transmitir |
|---|---|---|---|
| Aparecer | Cuando un enemigo entra por el borde de la arena | 0.3-0.6 s | Aviso — el jugador tiene que poder reaccionar a este sonido antes de ver el bicho en pantalla (pilar "siempre ves de qué morís") |
| Ser golpeado | Cuando un enemigo recibe un espadazo o disparo y no muere | 0.2-0.4 s | Confirma que el golpe conectó |
| Morir | Cuando un enemigo muere | 0.3-0.6 s | Distinto del anterior — confirma que ya no hay que seguir pegándole |
| Ataque del Tirador | Cuando el enemigo Tirador dispara | 0.2-0.4 s | Reconocible a distancia, para poder esquivar sin verlo en pantalla |

### De la partida

| Efecto | Cuándo suena | Duración aprox. | Qué tiene que transmitir |
|---|---|---|---|
| Moneda | Al recoger una moneda (cura 1 de vida) | 0.2-0.4 s | Recompensa, algo positivo y breve |
| Empieza oleada | Al arrancar cada oleada (1 a 10) | 0.5-1 s | Aviso claro de "ya vienen" — tiene que cortar por encima de la música (ver sección 6) |
| Empieza el jefe | Al arrancar la oleada 11 | 1-2 s | Más grande y ominoso que "empieza oleada" — este es EL aviso importante de la partida |
| Victoria | Al derrotar al jefe, antes o junto con la música de victoria | 1-2 s | Un "sting" corto que marca el instante exacto de la victoria |
| Derrota | Al caer todos los jugadores, antes o junto con la música de derrota | 1-2 s | Un "sting" corto que marca el instante exacto de la derrota |

### De interfaz

| Efecto | Cuándo suena | Duración aprox. | Qué tiene que transmitir |
|---|---|---|---|
| Botón | Al tocar cualquier botón de menú/UI | 0.1-0.2 s | Feedback táctil simple, no debe cansar (se toca muchas veces) |
| Entrar a la sala | Al entrar a una sala de espera (crear o buscar por código) | 0.3-0.6 s | Confirmación de que la conexión funcionó |

---

## 4. Formato recomendado

En celular importa el **peso del APK** (cuánto pesa para descargar/instalar) y la
**memoria disponible** en un teléfono de gama media. Por eso música y efectos no
usan el mismo formato:

| | Música | Efectos (SFX) |
|---|---|---|
| **Formato** | OGG Vorbis (comprimido) | WAV (sin comprimir) |
| **Frecuencia** | 44.1 kHz | 44.1 kHz |
| **Canales** | Estéreo | Mono (ahorra la mitad del peso y en un celular no se nota la diferencia) |
| **Por qué** | Las pistas son largas (60-90 s); comprimidas pesan una fracción y el celular las va reproduciendo sin cargarlas enteras en memoria | Los efectos son cortitos pero suenan todo el tiempo (cada espadazo, cada moneda); sin comprimir se reproducen al instante, sin el pequeño retraso de descomprimir cada vez que suenan |

En criollo: **la música se guarda liviana porque es larga, los efectos se guardan
livianos porque se repiten muchísimo** (mono en vez de estéreo) **y rápidos de
reproducir** (sin comprimir, aunque ocupen un poco más cada uno, son archivos
chiquitos igual).

---

## 5. Cómo exportar un loop que no se corte (en criollo)

Un loop "corta" cuando se nota el pegado entre el final y el principio: un click,
un silencio raro, o un cambio de ritmo. Para que no pase:

- **Cortá el archivo en un número entero de compases**, no "a ojo" en segundos.
  Si tu pista está en 4/4 a un tempo fijo, que dure exactamente 8, 16 o 32
  compases — así el final engancha con el principio en el mismo lugar del ritmo.
- **El último instante del archivo tiene que sonar igual que el primero.**
  Si la pista arranca con silencio y un golpe de batería, el final no puede
  terminar en medio de un acorde sostenido — tiene que llegar limpio, justo antes
  de donde repetiría ese mismo golpe.
- **No le pongas fade in ni fade out en las puntas.** Un fade out mata el loop:
  se va a escuchar un pozo de silencio cada vez que repite. La pista tiene que
  cortar en seco, a volumen pleno.
- **Cuidado con la reverb y el delay que "se arrastran".** Si el último acorde
  tiene una cola de reverb larga, esa cola se corta de golpe al reiniciar el
  loop y se nota. Si tu pista usa mucha reverb, cortá la cola antes de exportar
  o usá menos en la pista.
- **Probalo poniendo el archivo en loop 3 o 4 veces seguidas** en tu propio
  reproductor y escuchá los empalmes con atención (auriculares, no parlante de
  notebook). Si en algún empalme se te escapa un "clic" o un salto, todavía no
  está listo.

---

## 6. Reglas de mezcla para celular

El juego se escucha en el parlante chico de un teléfono, no en un equipo de
sonido. Tres reglas:

1. **Los efectos importantes van por encima de la música.** Espadazo, recibir
   daño, moneda y sobre todo "empieza oleada" / "empieza el jefe" tienen que
   escucharse claro aunque la música esté sonando. Si al mezclar un efecto se
   pierde debajo de la música, el efecto está mal balanceado, no hay que bajar
   la música del juego entero.
2. **Nada satura.** Un parlante de celular no aguanta picos fuertes: se distorsiona
   y suena feo. Dejá margen (headroom) al exportar — nada que llegue al tope del
   medidor — y si un sonido es naturalmente muy fuerte (el ataque del jefe, un
   "sting" de victoria), comprimilo/limitalo un poco en vez de subir el volumen general.
3. **La música nunca tapa el aviso de oleada.** "Empieza oleada" y "empieza el
   jefe" son la señal más importante del juego (avisan una amenaza antes de que
   golpee — es un pilar de diseño). Si hace falta, la música puede bajar un
   toque justo en ese instante (un "duck" de medio segundo) para que el aviso
   se escuche siempre nítido.

---

## 7. Volumen de música y efectos, por separado

La pantalla de pausa va a tener **dos controles de volumen independientes**: uno
para música y otro para efectos. El jugador va a poder, por ejemplo, bajar la
música y dejar los efectos al máximo (o al revés) sin que uno afecte al otro.
Esto es parte del sistema que se construye en el Sprint 7 — no es algo que la
guía te pida hacer al componer, pero explica por qué conviene mezclar música y
efectos como capas separadas y no como un solo archivo mezclado.

---

## 8. Nota importante: esto no bloquea nada

**La falta de música no frena ningún sprint.** El juego funciona perfectamente
en silencio — cada sistema (oleadas, combate, salas) se prueba y se aprueba sin
un solo sonido. Podés:

- Componer con calma, en el orden que prefieras.
- Entregar las pistas **de a una**, no hace falta tener las 6 + los 17 efectos
  listos para arrancar. Cada archivo que entregues se suma apenas esté.
- Empezar por las que más te copen o las que sean más cortas de producir
  (los efectos son más rápidos que la música de arena, por ejemplo).

---

## Decisiones abiertas para el PO

Estas son cosas que el equipo no puede decidir por vos — quedan anotadas para
que las resuelvas cuando quieras, no bloquean nada de lo de arriba:

- **Victoria/derrota, ¿un archivo o dos?** La guía separa un "sting" corto de
  SFX (sección 3, "de la partida") y una música corta (sección 2). Si preferís
  resolverlo con un solo archivo que cumpla las dos funciones, es tu decisión —
  avisá y ajustamos la guía.
- **¿La pista de arena cambia de intensidad a medida que avanzan las oleadas**
  (de la 1 a la 10), o es la misma pista pareja toda la partida? El plan del
  juego no lo fija; si querés que suba la tensión hacia el final, hace falta
  definir en qué oleada "sube un cambio" antes de componer las variantes.
- **¿La música del jefe es una pista totalmente distinta o una variación de la
  de arena** (mismo tema, más intensa)? Cualquiera de las dos funciona con esta
  guía, es una preferencia tuya de composición.
