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
- Project (tablero) con campos `Status` (Todo/In Progress/Done) e `Iteration`.
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
