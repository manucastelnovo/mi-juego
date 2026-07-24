# Traspaso de contexto — Vigilia (de Research a los roles que siguen)

> Lo escribe el rol **Investigador/Research** para pasar el contexto a quien
> continúe (**scrum-master**, **narrativa**, **disenador**), respetando roles.
> El PO ya aprobó la dirección; acá está todo empaquetado para no re-preguntar.

## 0. Aviso de coordinación (importante)
Hay **varios agentes trabajando el repo en paralelo** (worktrees en
`docs/vigilia-para-jugadores` y `feat/26-landscape`, además del árbol principal).
Durante este trabajo, un proceso **sobrescribió dos veces** archivos HTML de
Vigilia con un aviso falso ("generado por error / no hubo dirección aprobada")
y **cerró el PR #120**. Ese aviso es **falso**: el PO respondió el cuestionario y
aprobó la dirección (registrado en la conversación con Research). Antes de tocar
nada, **alinear las sesiones/agentes** para no pisarse.

## 1. Qué es Vigilia (dirección FIRME del PO)
- Run-and-gun de acción (tipo Metal Slug), **co-op online 4**, **terror folclórico (+16)**.
- Ambientación: **folclore guaraní-paraguayo**. Los enemigos son los **7 hijos
  malditos de Taú y Keraná**; el jefe final es **Moñái** (el que reúne a los siete).
- **Porãsy** = narradora/guía. Epílogo: su sacrificio → se vuelve **Venus**.
- **2 personajes** seleccionables: **guerrero** y **cazadora**.
- **Pixel art**. Textos/voces **bilingües ES + guaraní**.
- **1 golpe y morís + rescate** de compañeros. Armas **temáticas del folclore**
  (fuego sagrado, ofrendas, palmera pindó como refugio).
- **Pombero** = jefe secreto/oculto (OJO: es de otro ciclo mítico, NO es hermano
  de los siete).
- **Orden de niveles aprobado:** 1) Teju Jagua (cuevas, MVP) · 2) Jasy Jateré
  (siesta) · 3) Mbói Tu'ĩ (esteros) · 4) Kurupí (bosque, rasgo del mito **real
  pero sutil**) · 5) Ao Ao (cerros) · 6) Luisón (cementerio) · 7) **Moñái** (final).
- **MVP:** 1 nivel (Teju Jagua) + su boss + narrativa mínima.

## 2. Qué existe ya (artefactos)
- `docs/research/pombero-siete-hermanos.html` — informe de folclore con **fuentes citadas**.
- `docs/research/brief-vigilia-direccion.html` — brief con TODAS las decisiones del PO.
- `docs/research/cuestionario-vigilia.html` — cuestionario que respondió el PO.
- `docs/narrativa/biblia-vigilia.html` — **biblia narrativa (borrador)**: premisa,
  arco de Porãsy, los 7 actos, jefe secreto, rebanada MVP. (El PO pidió conservarla.)
- Los 3 de `docs/research/` están commiteados en la rama `origin/docs/vigilia-research`
  (el PR #120 fue **cerrado** por otro proceso, no mergeado — reabrir o re-PR si se quiere).

## 3. Quién sigue y con qué (respetando roles)
- **scrum-master:** dueño de los issues/sprint. Ya hay abiertos (creados por Research,
  reasignar/adoptar como corresponda): **#121** (narrativa) y **#119** (disenador).
  Decidir si se conservan, reescriben o cierran; ubicar tarjetas en el tablero.
- **narrativa:** formalizar la **biblia** a partir del borrador `biblia-vigilia.html`
  (issue #121). Resolver con el PO las preguntas abiertas (nombres de los 2 personajes,
  si la historia personal de la cazadora entra en la trama, qué da el Pombero, alcance
  del guaraní, cuánto de la maldición va en la intro).
- **disenador:** con la biblia + el brief, redactar el **GDD** y desglosar
  **épica → historias**, arrancando por el **MVP (Nivel 1 / Teju Jagua)** (issue #119).
- **artista-personajes / artista-terreno:** cuando el disenador defina el MVP, briefs de
  arte (Teju Jagua, guerrero, cazadora, cueva/cerro) con aprobación del PO.

## 4. Research pendiente (lo hace el Investigador, ya pedido por el PO)
Para el MVP, después de la biblia: (1) referencias visuales de pixel art de Teju Jagua +
personajes, (2) **nombres guaraníes** para guerrero y cazadora, (3) mini-glosario guaraní
para los textos bilingües. Sin empezar aún; a pedido del PO.

## 5. Regla de oro para quien siga
La **visión la decide el PO**. Todo lo marcado en los documentos como "sugerencia" o
"ficción de juego" es propuesta, separada del "hecho citado" del mito. No re-decidir lo
que el PO ya fijó (sección 1); sí resolver con él lo marcado "A DEFINIR".
