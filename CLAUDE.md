# Estudio de agentes — reglas del proyecto

Este repositorio es un videojuego de Unity construido por un equipo de agentes
IA coordinados. Este archivo define las convenciones que **todos** los agentes
deben respetar. Ajusta los valores entre `<...>` a tu proyecto real.

## El equipo y quién decide
- **Product Owner (tú, el humano)**: decides el juego, la historia, los
  guiones y qué funcionalidades entran. Escribes o apruebas las historias,
  revisas los Pull Requests y detectas cuándo algo no es como lo quieres.
  **Ningún cambio se cierra sin tu aprobación.**
- **scrum-master**: orquesta el sprint y respeta tus compuertas de aprobación.
- **disenador**: redacta borradores de GDD/historias para que tú los apruebes.
- **programador-csharp**: implementa en Unity, trabaja en ramas y abre PRs.
- **qa**: valida errores técnicos antes de que un PR llegue a ti.
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
   - **Pasa** → qa comenta **"✅ Aprobado por QA"** y pone el label `qa-approved`.
     Ese es el **gate**: sin "Aprobado por QA" el PO no revisa.
6. **Solo con "Aprobado por QA", tú (PO) haces la prueba final** en Play mode y
   apruebas o pides cambios. Solo con tu aprobación se mergea y se cierra el issue.
7. **Comunicación con el PO:** si qa o el programador tienen dudas de diseño o
   necesitan una decisión tuya, te preguntan **en un comentario del PR/issue de
   GitHub** (te mencionan). No deciden la visión del juego por su cuenta.

**Regla del gate:** el orden es siempre *dev (autopruebas) → qa (Aprobado por QA)
→ PO (prueba final)*. Nada salta el paso de qa.

### Convenciones de comentarios y evidencia (trazabilidad)
- **Firma del rol al principio de cada comentario.** Todo comentario de un agente
  en un PR/issue empieza con su etiqueta de rol en negrita, en la primera línea:
  `**[DEV]**`, `**[QA]**`. Así se sabe de un vistazo quién habla. (El PO es humano
  y no necesita firmar.)
- **Plan de pruebas del dev.** Al pedir QA, el programador deja un comentario
  `**[DEV]**` con el **plan de pruebas** derivado de los criterios del ticket:
  la lista concreta de qué hay que probar. QA ejecuta ese plan (no inventa el
  suyo; puede sumar casos, pero cubre primero el del dev).
- **QA responde con evidencia.** QA contesta con un comentario `**[QA]**` que
  recorre el plan punto por punto (✅/❌) y **adjunta screenshots como evidencia**
  de las pruebas. Las capturas se guardan en `docs/qa/` (fuera de `Assets/`, para
  que Unity no las importe) y se embeben en el comentario con la URL raw del
  branch: `https://raw.githubusercontent.com/manucastelnovo/mi-juego/<rama>/docs/qa/<archivo>.png`.
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

## Convenciones de código (Unity / C#)
- Scripts en `Assets/Scripts/`, un `MonoBehaviour` por responsabilidad.
- Nombres en `PascalCase` para clases y métodos, `camelCase` para campos.
- Comentarios breves en español donde aporten; nada de código muerto.
- Nunca inventar APIs de Unity: si hay duda sobre una firma, leerla primero.

## Convenciones de assets
- `Assets/Art/Sprites`, `Assets/Art/Materials`, `Assets/Prefabs`, `Assets/Scenes`.
- Prefabs reutilizables en vez de configurar objetos sueltos.
- Solo assets propios o de licencia libre declarada.

## Etiquetas (labels) de GitHub
`epic` · `story` · `task` · `bug` · `art` · `asset`

Labels de proceso (ciclo de QA):
- `needs-qa` — el PR está listo y espera revisión de qa.
- `qa-approved` — qa dio "✅ Aprobado por QA"; habilitado para la prueba final del PO.

## Regla de oro
Sprints cortos, PRs pequeños, incrementos jugables. Terminar y que el PO lo
apruebe vale más que planificar de más. Ante cualquier duda creativa, se
pregunta al Product Owner; los agentes no deciden la visión del juego.
