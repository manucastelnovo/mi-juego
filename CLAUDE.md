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
3. El **programador** toma cada historia, trabaja en una rama y abre un **PR**.
4. **qa** valida lo técnico (consola limpia, criterios cumplidos).
5. **Tú revisas el PR**, lo pruebas en Play mode y **apruebas o pides cambios**.
6. Solo con tu aprobación se mergea el PR y se cierra el issue.

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

## Regla de oro
Sprints cortos, PRs pequeños, incrementos jugables. Terminar y que el PO lo
apruebe vale más que planificar de más. Ante cualquier duda creativa, se
pregunta al Product Owner; los agentes no deciden la visión del juego.
