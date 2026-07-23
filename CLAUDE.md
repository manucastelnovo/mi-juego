# Estudio de agentes — reglas del proyecto

Este repositorio es un videojuego de Unity construido por un equipo de agentes
IA coordinados. Este archivo define las convenciones que **todos** los agentes
deben respetar. Ajusta los valores entre `<...>` a tu proyecto real.

## El equipo y quién decide
- **Product Owner (tú, el humano)**: decides el juego, la historia, los
  guiones y qué funcionalidades entran. Escribes o apruebas las historias y
  detectas cuándo algo no es como lo quieres. **Las decisiones de visión son
  tuyas y solo tuyas**, pero el trabajo técnico ya no espera tu firma: qa
  mergea lo que aprueba (ver "Quién mergea").
- **scrum-master**: orquesta el sprint y respeta tus compuertas de aprobación.
- **disenador**: redacta borradores de GDD/historias para que tú los apruebes.
- **programador-csharp**: implementa en Unity, trabaja en ramas y abre PRs.
- **qa**: valida errores técnicos y **mergea a `main`** lo que aprueba.
- **artista**: gestiona sprites, materiales y prefabs.

## Datos del proyecto
- Repositorio de issues/PRs: `manucastelnovo/mi-juego`
- Rama principal protegida: `main`
- Project (tablero) "Mi Juego - Sprint Board" (nº 1, owner manucastelnovo) con
  campos `Status` (Todo/In Progress/Done) y `Sprint` (single-select: Backlog,
  Sprint 1, Sprint 2, Sprint 3). El sprint activo se marca con el campo `Sprint`.
- Motor: Unity `6000.5.4f1` · Render pipeline: `Built-in` (creado por CLI)
- **Plataforma objetivo: móvil (Android)** — juegos 2D para celular. Optimizar
  para pantallas táctiles y rendimiento en dispositivos de gama media.
- Nota: el proyecto se creó vacío por CLI. Las primeras tareas del sprint
  incluyen añadir los paquetes 2D (`com.unity.feature.2d`) y cambiar el
  build target a Android.

## Flujo de trabajo (Scrum + PRs)
1. Tú das una historia o idea al **scrum-master**.
2. El **disenador** propone el desglose en épicas/historias/tareas → **tú lo
   apruebas** antes de que entre al sprint.
3. El **programador** toma cada historia y trabaja en una rama. Antes de pedir
   revisión hace **autopruebas propias** (mini-QA): compila, entra a Play mode,
   consola limpia y repasa los criterios de aceptación. Las documenta en el PR
   bajo **"Autopruebas del dev"**.
4. El programador abre el **PR**, le pone el label `needs-qa` y pide revisión a
   **qa** en un comentario (`@qa listo para revisar`).
5. **qa** valida lo técnico: Play mode, lee la consola, verifica cada criterio
   uno por uno. En el PR deja **comentarios de mejora** (no bloqueantes) y puede
   **conversar con el programador** las veces que haga falta (ida y vuelta).
   - **Falla** (error de consola, criterio incumplido, regresión) → pide cambios
     en el PR o abre un `bug`; el dev corrige en la misma rama y qa revuelve.
   - **Pasa** → qa comenta **"✅ Aprobado por QA"**, pone el label `qa-approved`
     y **mergea el PR a `main`**, cerrando el issue con el `Closes #n`.
6. **Quién mergea:** por defecto **mergea qa**, sin esperar al PO. El PO no es
   un paso obligatorio del ciclo: revisa cuando quiere y siempre puede pedir
   cambios después, en un issue nuevo.
   **Excepciones — qa NO mergea y le pregunta al PO** cuando:
   - **Falta una definición de diseño**: el criterio del ticket es ambiguo, o
     cumplirlo obliga a decidir algo de la visión del juego (cómo se siente,
     cómo se ve, qué es "suficiente"). Los agentes no deciden la visión.
   - El PR **cambia algo que el PO ya había aprobado** (arte, controles,
     balance, textos) o toca la identidad del juego.
   - qa **no pudo verificar** un criterio y ese criterio es la razón de ser del
     ticket. Aprobar a ciegas está prohibido: se dice qué no se pudo probar.
   En esas excepciones qa deja `qa-approved`, **no mergea**, y menciona al PO en
   el PR explicando exactamente qué decisión necesita.
7. **Comunicación con el PO:** si qa o el programador tienen dudas de diseño o
   necesitan una decisión tuya, te preguntan **en un comentario del PR/issue de
   GitHub** (te mencionan). No deciden la visión del juego por su cuenta.

**Regla del gate:** el orden es siempre *dev (autopruebas) → qa (Aprobado por QA
y merge)*. **Nada salta el paso de qa**, y qa nunca revisa su propio trabajo.
El PO entra cuando hace falta una decisión de diseño, no en cada PR.

### Convenciones de comentarios y evidencia (trazabilidad)
- **Tarjeta de rol al inicio (Salto Games).** Cada comentario de un agente empieza con su
  tarjeta de rol (ver "Identidad de los agentes en GitHub"), p. ej.
  `### 🧪 QA · Salto Games`. Así se sabe quién habla. (El PO comenta con su cuenta, sin
  tarjeta.)
- **Comentarios resumidos y claros.** Título corto + 2–5 bullets en lenguaje de
  funcionalidades. Nada de dumps técnicos ni de logs.
- **Plan de pruebas del dev.** Al pedir QA, el programador deja un comentario
  `**[DEV]**` con el **plan de pruebas** derivado de los criterios del ticket:
  la lista concreta de qué hay que probar. QA ejecuta ese plan (no inventa el
  suyo; puede sumar casos, pero cubre primero el del dev).
- **QA responde con evidencia.** QA contesta con un comentario `**[QA]**` que
  recorre el plan punto por punto (✅/❌) y **adjunta screenshots como evidencia**
  de las pruebas. **El repo es privado**, así que las URL `raw.githubusercontent.com`
  **no renderizan** inline (el proxy de imágenes de GitHub no se autentica → 404).
  Reglas de evidencia:
  - **Para que se vea inline:** el humano/QA **arrastra las capturas al cuadro de
    comentario** en el navegador; GitHub las sube a `user-attachments` y sí se
    embeben para colaboradores logueados. (No hay API de adjuntos, no se hace por CLI.)
  - **Vía CLI (sin drag&drop):** guardar las capturas en `docs/qa/` (fuera de
    `Assets/`, para que Unity no las importe), commitearlas, y en el comentario
    dejar **enlaces clicables** al archivo (blob), no imágenes embebidas:
    `[descripción](https://github.com/manucastelnovo/mi-juego/blob/<rama>/docs/qa/<archivo>.png)`.
    Abren logueado en una pestaña; no intentar embeber con `raw` (sale roto).
- **Conversación en el PR.** El ida y vuelta dev↔qa vive en comentarios del PR,
  cada uno firmado. Las preguntas al PO también van ahí, firmadas y mencionándolo.

## Estrategia de ramas
- `main` — siempre jugable y estable. Nadie pushea directo; solo se entra por PR.
- `feat/<issue>-<slug>` — una rama por historia. PR pequeño hacia `main`.
- `epic/<slug>` — rama larga para una feature grande. Las historias de esa
  épica se integran ahí; al final se abre **un PR consolidado `epic/... → main`**
  para que el PO revise la feature completa de una vez.
- `bug/<issue>-<slug>` — correcciones puntuales.

## GitHub vía `gh` CLI (sin MCP)
Todos los agentes operan GitHub con el CLI `gh` (ya autenticado), no con MCP.
Repo: `manucastelnovo/mi-juego`. Project: nº `1`, owner `manucastelnovo`.
En Windows, si `gh` no está en el PATH, usar la ruta completa
`"C:\Program Files\GitHub CLI\gh.exe"`.

Comandos base:
```bash
# Crear una historia
gh issue create --repo manucastelnovo/mi-juego --title "Como jugador, quiero saltar" \
  --body "Criterios: ..." --label story

# Comentar / cerrar / ver
gh issue comment <n> --repo manucastelnovo/mi-juego --body "QA OK"
gh issue close <n>   --repo manucastelnovo/mi-juego
gh issue view <n>    --repo manucastelnovo/mi-juego

# Añadir un issue al tablero
gh project item-add 1 --owner manucastelnovo --url <url-del-issue>

# Abrir un Pull Request (desde la rama de trabajo)
gh pr create --base main --head feat/<n>-slug --title "..." --body "Closes #<n>"
```

**Jerarquía épica → historia → tarea:** como `gh` no tiene comando directo de
sub-issues, el desglose se representa con **task lists** en el cuerpo del issue
padre, referenciando a los hijos por número (GitHub las trata como sub-issues):
```
## Historias
- [ ] #12 Salto del personaje
- [ ] #13 Gravedad
```

**Estado del tablero (Status / Sprint):** mover tarjetas por columnas requiere
IDs de campo/opción (`gh project item-edit`) y es engorroso. Para no bloquear el
flujo, los agentes usan **labels + estado open/closed** del issue como fuente de
verdad; el Product Owner (o un ajuste manual) coloca las tarjetas en el tablero.
El estado "hecho" = issue cerrado por el merge del PR (`Closes #n`).

## Identidad de los agentes en GitHub (tarjetas de rol — estudio "Salto Games")
Todos los agentes comentan con **la cuenta del PO** (`manucastelnovo`): GitHub no permite
varias identidades dentro de una misma cuenta. Para saber quién habla, **cada comentario
empieza con la tarjeta de rol** del estudio (primera línea, encabezado con emoji):

| Rol | Tarjeta (primera línea del comentario) |
|-----|----------------------------------------|
| scrum-master | `### 📋 Scrum · Salto Games` |
| disenador | `### 🎯 Diseño · Salto Games` |
| programador-csharp | `### 🧑‍💻 Dev · Salto Games` |
| qa | `### 🧪 QA · Salto Games` |
| artista | `### 🎨 Arte · Salto Games` |

El PO (humano) comenta con su cuenta, **sin tarjeta**. Las llamadas `gh` son las normales
(sin tokens especiales).

### Estilo de comentarios (resumido y claro)
Debajo de la tarjeta: **título de una línea + 2–5 bullets** en lenguaje de
**funcionalidades** (qué se hizo / se pide / se decidió), sin volcados técnicos ni dumps de
logs.

## Convenciones de código (Unity / C#)
- Scripts en `Assets/Scripts/`, un `MonoBehaviour` por responsabilidad.
- Nombres en `PascalCase` para clases y métodos, `camelCase` para campos.
- Comentarios breves en español donde aporten; nada de código muerto.
- Nunca inventar APIs de Unity: si hay duda sobre una firma, leerla primero.

## Convenciones de assets
- `Assets/Art/Sprites`, `Assets/Art/Materials`, `Assets/Prefabs`, `Assets/Scenes`.
- Prefabs reutilizables en vez de configurar objetos sueltos.
- Solo assets propios o de licencia libre declarada.
- Sprites móviles: **PNG con alpha**, tamaño en px pensado para pantallas de gama media,
  con su **PPU** (Pixels Per Unit) coherente con la escala del juego.

### Flujo de arte (brief en GitHub → assets en repo + Google Drive)
1. **Brief en GitHub** (lo redacta Diseño/Arte en el issue `art`/`asset`): plantilla —
   *estilo, paleta, tamaño px + PPU, lista de sprites, formato (PNG alpha)*.
2. **Gate del brief:** el **PO aprueba el brief** en GitHub antes de que Arte produzca
   (los agentes no deciden la visión).
3. **Producción y doble destino:** el sprite game-ready va a `Assets/Art/Sprites` en el
   **repo** (Unity lo necesita) **y** una copia se sube a **Google Drive**, en la carpeta
   espejo **`Salto Games/Assets/Sprites`** (`Materials`, etc.). Drive es la superficie de
   **revisión/archivo** del PO.
4. **Entrega en GitHub:** Arte deja un comentario (cuenta *Arte — Salto Games*) con el
   preview del sprite y el **enlace a Drive**, y pide revisión.
5. **Gate final:** QA valida técnico (sprites asignados, colliders intactos, consola
   limpia) → el **PO aprueba lo visual**.

## Etiquetas (labels) de GitHub
`epic` · `story` · `task` · `bug` · `art` · `asset`

Labels de proceso (ciclo de QA):
- `needs-qa` — el PR está listo y espera revisión de qa.
- `qa-approved` — qa dio "✅ Aprobado por QA"; habilitado para la prueba final del PO.

## Regla de oro
Sprints cortos, PRs pequeños, incrementos jugables. Terminar y que el PO lo
apruebe vale más que planificar de más. Ante cualquier duda creativa, se
pregunta al Product Owner; los agentes no deciden la visión del juego.
