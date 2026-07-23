# Guia de assets - Vigilia

Esta guia te permite redibujar cualquier sprite a mano, o meter un pack
descargado de internet, sin pedirle nada a un agente. Esta escrita a partir
de lo que el proyecto hace hoy (no de lo que deberia hacer), verificado
archivo por archivo.

---

## 1. Donde va cada cosa

| Carpeta | Que guarda | Estado hoy |
|---|---|---|
| Assets/Art/Sprites/ | Todos los .png del juego (personajes, terreno, monedas, etc.) | Existe y en uso: Player.png, Ground.png, Platform.png, Coin.png |
| Assets/Art/Materials/ | Materiales visuales (shaders, tints) | Todavia no existe. Hoy los SpriteRenderer usan el material Sprites-Default de Unity; no hace falta crear nada aca hasta que un sprite necesite un material especial |
| Assets/Prefabs/ | Un prefab por "cosa" del juego (moneda, jugador, enemigo...) | Existe Coin.prefab. El resto (Player, plataformas) hoy vive suelto en la escena, no como prefab - ver nota en la seccion 3 |
| Assets/Scenes/ | Las escenas del juego | Game.unity es la unica escena |

Nota aparte: existe Assets/Physics/PlayerSinFriccion.physicsMaterial2D, un
material de fisica (friccion de colliders), no de arte. No lo toques al
cambiar sprites.

---

## 2. Lista de sprites actuales: tamano, PPU, pivot y donde van

Lo verifique en el .meta de cada sprite actual (campo spritePixelsToUnits y
spritePivot) y en Game.unity / Coin.prefab (donde se usa cada uno):

| Sprite | Tamano real (px) | PPU en el .meta | Pivot | Donde va |
|---|---|---|---|---|
| Player.png | 64x64 | 64 | Centro (0.5, 0.5) | Objeto `Player`, suelto en `Assets/Scenes/Game.unity` (no tiene prefab propio hoy) |
| Ground.png | 64x64 | 64 | Centro (0.5, 0.5) | Objeto `Ground`, suelto en `Assets/Scenes/Game.unity`, mostrado en modo Tiled |
| Platform.png | 64x32 | 64 | Centro (0.5, 0.5) | Objetos `Platform_1` y `Platform_2`, sueltos en `Assets/Scenes/Game.unity`, mostrados en modo Tiled |
| Coin.png | 64x64 | 64 | Centro (0.5, 0.5) | `Assets/Prefabs/Coin.prefab` |

**Esta es la lista de lo que existe hoy en el repo, no la lista final del
juego.** Los sprites que todavia faltan (enemigos, jefe, armas, proyectiles,
4 variantes de color del jugador, fondo de arena) estan enumerados con sus
tamanos y pivots propuestos en el **brief de arte aprobado en #70** — no los
repito aca porque ese brief todavia no lo aprobo el PO y la lista puede
cambiar. Cuando se produzcan, se agregan a esta tabla siguiendo el mismo
formato.

**Inconsistencia a tener en cuenta - tamano de la Moneda:** el brief de #70
propone la Moneda en **32x32 px**, pero el `Coin.png` que existe hoy en el
repo mide **64x64 px** (el doble). Si el dia de manana se produce la moneda
segun el brief a 32x32 px y se la importa con PPU 64 sin ajustar nada mas, va
a ocupar la mitad del tamano en el mundo que la moneda actual (0.5x0.5
unidades en vez de 1x1), y el collider de `Coin.prefab` (radio 0.5, ver
seccion 4) le va a quedar grande. Antes de reemplazarla, decidir si el
tamano final es 32x32 (y achicar el collider a la mitad) o si se mantiene
64x64 (y se avisa que el brief quedo desactualizado en ese punto).

### PPU: 64, unico para todo el juego

Los cuatro coinciden en 64 PPU, y es el mismo valor que propone el brief de
arte aprobado en #70. Por que tiene que ser uno solo para todo el juego?
Porque 1 unidad de mundo en Unity equivale a 64 pixeles de sprite: la
velocidad del jugador, la altura del salto y el tamano de los colliders
estan todos calibrados asumiendo esa equivalencia. Si un sprite nuevo usa
otro PPU, se ve mas grande o mas chico que el resto sin que cambies ningun
numero a proposito. (El encuadre de la Main Camera es un tema aparte, ver
nota al final de esta seccion.)

Si un asset descargado viene en otra resolucion (por ejemplo, un pack de
128x128 px por sprite): al importarlo a Assets/Art/Sprites/, abri su
Inspector y en Pixels Per Unit pone el valor que corresponda para que el
dibujo mida lo mismo en el mundo que el original que reemplaza. La cuenta es:

```
PPU nuevo = (ancho o alto del sprite nuevo, en px) / (tamano en unidades que debe ocupar)
```

Ejemplo: si el Player actual mide 64x64 px a 64 PPU (o sea, 1x1 unidad), y
bajas un sprite de reemplazo de 128x128 px que tiene que ocupar el mismo
espacio, hay que ponerle 128 PPU a ese sprite en particular. No hace falta
cambiar el PPU de los demas: cada .png tiene su propio ajuste de importacion
independiente, siempre que el resultado en unidades de mundo sea el mismo.

**Nota sobre la camara (no confundir con lo de arriba):** la Main Camera de
`Game.unity` hoy esta en modo **perspectiva** (`orthographic: 0`, field of
view: 60), no ortografica - el campo `orthographic size: 5` existe en la
escena pero no se usa mientras la camara este en perspectiva. Este documento
no cubre como calibrar el encuadre; el tema esta anotado en el **issue #97**.

---

## 3. Como reemplazar el arte de algo sin romper nada

Cada objeto visual es un SpriteRenderer con el sprite asignado a mano en
el Inspector, no por codigo. Lo confirme leyendo los cinco scripts de
Assets/Scripts/ (BotonDireccion.cs, ContadorMonedas.cs, GestorRed.cs,
Moneda.cs, PlayerController.cs): ninguno usa Resources.Load, ninguno busca
un sprite por nombre de archivo, y ninguno menciona siquiera la clase
SpriteRenderer hoy (no hay flip de sprite ni ningun otro manejo de sprite
por codigo en `main` en este momento).

Que significa esto para vos: para reemplazar un dibujo,

1. Arrastra el .png nuevo a Assets/Art/Sprites/ (reemplazando el archivo
   viejo, o como uno nuevo).
2. Ajusta su Inspector (PPU, filtro, compresion - ver seccion 5).
3. Selecciona el objeto en la escena (o el prefab, si ya existe uno) y
   arrastra el sprite nuevo al campo Sprite de su Sprite Renderer.
4. Nada de C# se toca ni se rompe: el codigo no sabe ni le importa que
   imagen se esta mostrando.

Nota sobre prefabs (importante, leela antes de tocar Ground o las
plataformas): hoy Coin es el unico prefab de arte (Assets/Prefabs/Coin.prefab);
ahi alcanza con abrir el prefab y arrastrar el sprite nuevo, y el cambio se
propaga solo. Player, Ground, Platform_1 y Platform_2 todavia viven sueltos
dentro de Game.unity, no como prefabs - asi que para reemplazarlos hoy hay
que seleccionarlos directamente en la escena. El plan del juego preve pasar
a "un prefab por cosa" cuando se armen los enemigos y variantes de personaje
(a partir de la epica E1); cuando eso pase, el mismo paso 3 de arriba se hace
una vez en el prefab y listo.

---

## 4. Colliders: tamano explicito, separado del dibujo

El tamano del collider nunca sale del sprite ni se recalcula solo. Lo
verifique en Game.unity y en Coin.prefab: cada BoxCollider2D /
CircleCollider2D tiene su m_Size (o m_Radius) escrito a mano, y en el caso
de Ground y las plataformas, el sprite se muestra en modo Tiled (repetido)
para cubrir un ancho que no tiene nada que ver con el tamano del archivo
original de 64 px:

| Objeto | Sprite (tamano real) | Como se muestra | Tamano del collider |
|---|---|---|---|
| Ground | Ground.png 64x64 px | Tiled, 20x1 unidades | BoxCollider2D 20x1 |
| Platform_1 | Platform.png 64x32 px | Tiled, 4x0.5 unidades | BoxCollider2D 4x0.5 |
| Platform_2 | Platform.png 64x32 px | Tiled, 3.5x0.5 unidades | BoxCollider2D 3.5x0.5 |
| Player | Player.png 64x64 px | 1x1 unidad (normal) | BoxCollider2D 1x1 |
| Coin (prefab) | Coin.png 64x64 px, escala 0.5 | 0.5x0.5 unidades | CircleCollider2D radio 0.5 (mundo: 0.25) |

Por que importa: si metes un sprite nuevo con otra silueta (una moneda
mas flaca, un enemigo con alas que sobresalen del cuerpo), el collider no
se ajusta solo. Eso es a proposito: asi el balance del juego (que tan lejos
hay que estar para que algo te toque, cuanto mide el suelo) no cambia porque
alguien cambio un dibujo.

Como ajustarlo si el arte nuevo tiene otra forma:
1. Selecciona el objeto (o el prefab) y mira su collider en el Inspector
   (Box Collider 2D, Circle Collider 2D, etc., segun el objeto).
2. Edita Size (o Radius) y Offset a mano, comparando visualmente con el
   nuevo sprite en la Scene View, hasta que la zona solida/de contacto
   coincida con la silueta que se ve bien (ni mas chica -se atraviesa-, ni
   mas grande -golpea en el aire-).
3. Si el sprite se muestra en modo Tiled (como Ground y las plataformas),
   el tamano del collider es independiente del tamano del archivo: podes
   poner cualquier Size en el Sprite Renderer (pestana Draw Mode: Tiled) y
   ajustar el collider al mismo valor, sin importar cuantos pixeles mida el
   .png de origen.

---

## 5. Formato de archivo y ajustes de importacion (mobile)

Todos los sprites del juego son PNG con canal alpha. Lo confirme en los
cuatro .meta: alphaIsTransparency: 1 en los cuatro.

Ajustes de importacion que hoy usan los cuatro sprites (revisalos en el
Inspector del .png, con Texture Type: Sprite (2D and UI)):

| Ajuste | Valor actual | Por que importa en gama media |
|---|---|---|
| Filter Mode | Point (no filter) | Con arte flat/cartoon sin degrades finos, el filtrado bilineal solo difumina bordes y gasta GPU sin mejorar nada. Mantenelo en Point salvo que el estilo nuevo tenga gradientes suaves que se vean mejor con Bilinear. |
| Compresion (Android, Standalone, etc.) | Compressed (calidad Normal, sin override especifico por plataforma) | Reduce memoria de video y tiempo de carga en celulares de gama media. No lo cambies a None/RGBA32 salvo que veas artefactos de color visibles - ahi conviene revisar caso por caso, no desactivar compresion para todo el proyecto. |
| Max Size | 2048 | Ningun sprite actual se acerca a ese tamano (todos <= 64x64 px); dejalo asi, no hace falta subirlo para arte 2D chico. |
| Mip Maps | Desactivado en Player, Ground, Platform; activado en Coin (inconsistencia existente, no corregida en esta guia) | Los sprites nunca se alejan en profundidad respecto de la camara, asi que los mipmaps no aportan nada y solo ocupan memoria extra (mas alla del modo de camara - ver nota sobre la camara en la seccion 2 e issue #97). Para assets nuevos, dejar Mip Maps desactivado, salvo que el diseno cambie eso. |
| Pivot | Centro (0.5, 0.5) en los cuatro | Mantenelo centrado salvo que el brief de un sprite puntual pida otra cosa (ver Sprite Border/pivot en el brief aprobado en #70 si aplica). |

Al importar un asset nuevo o de un pack de internet:
1. Copialo a Assets/Art/Sprites/ como PNG con alpha (si viene en otro
   formato, exportalo a PNG antes).
2. Abri su Inspector y confirma Texture Type: Sprite (2D and UI).
3. Ajusta el PPU segun la seccion 2.
4. Deja Filter Mode: Point y Mip Maps: desactivado, salvo que el estilo
   del pack tenga razones visuales concretas para cambiarlo.
5. No toques la compresion por plataforma a menos que se vea mal en el
   dispositivo real - es un ajuste que conviene decidir mirando el
   resultado, no de antemano.

---

## 6. Animaciones: reemplazar cuadros no toca C#

El proyecto hoy no tiene animaciones por clips todavia (no hay Animator
configurado en la escena ni .anim en el repo), pero el plan del juego
(epica E6, brief de arte de #70) define animaciones minimas por clip
(caminar, atacar, recibir dano, caido, morir) para el jugador y cada
enemigo. La convencion a seguir cuando se agreguen:

- Cada animacion es un Animation Clip (.anim) que referencia una secuencia
  de sprites (los "cuadros"), controlado por un Animator Controller en el
  prefab.
- Reemplazar los cuadros de una animacion (por ejemplo, "caminar") es editar
  el clip y arrastrar los sprites nuevos en el mismo orden - el codigo no
  conoce los nombres de los sprites ni de los clips que arma el Animator
  internamente, asi que no hay nada que romper en C#.
- Si el sprite nuevo tiene otro tamano de silueta, la animacion se ve bien
  pero acordate de revisar el collider por separado (seccion 4): la
  animacion no cambia hitboxes.

---

## 7. Checklist: "agregue un asset nuevo"

Segui estos pasos en orden, sin pensar de mas:

1. [ ] Copiar el .png (con alpha) a Assets/Art/Sprites/.
2. [ ] Confirmar Texture Type: Sprite (2D and UI) en su Inspector.
3. [ ] Calcular y poner el PPU correcto (seccion 2) para que ocupe el mismo
   tamano en el mundo que lo que reemplaza.
4. [ ] Dejar Filter Mode: Point y Mip Maps: desactivado, salvo razon visual
   concreta para cambiarlo.
5. [ ] Si el asset es para algo que ya existe (jugador, moneda, terreno):
   seleccionar el objeto o prefab en la escena y arrastrar el sprite nuevo
   al campo Sprite de su Sprite Renderer.
6. [ ] Revisar el collider del objeto (Box/Circle Collider 2D): si la
   silueta nueva es muy distinta a la vieja, ajustar Size/Radius/Offset a
   mano - nunca asumir que se ajusto solo.
7. [ ] Si el asset trae animacion, armar/editar el clip correspondiente con
   los cuadros nuevos; no hace falta tocar ningun script.
8. [ ] Entrar a Play Mode y mirar que se vea bien y que el personaje/objeto
   siga chocando donde antes (mismo lugar, ni atraviesa ni flota).

---

Lista de sprites, paleta y tamanos propuestos para el arte definitivo: segun
el brief de arte aprobado en #70 (todavia pendiente de aprobacion del PO al
momento de escribir esta guia).
