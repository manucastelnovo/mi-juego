# Vigilia · Salto Games

Juego cooperativo online en el que hasta **4 jugadores** aguantan **oleadas de
enemigos** en una arena oscura hasta enfrentar al **jefe**. Pensado para
**móvil (Android)**, en 2D, con controles táctiles.

Desarrollado por un **equipo de agentes de IA** coordinados (estudio *Salto
Games*) bajo la dirección de un Product Owner humano.

---

## El juego

- **Co-op online** para hasta 4 jugadores en la misma partida.
- **Salas con código**: un jugador crea la sala como host y los demás entran
  con el código desde otro celular.
- **Oleadas con jefe**: enemigos que entran por los bordes, escalan con la
  cantidad de jugadores y culminan en un jefe.
- **Combate cooperativo**: espada contextual, vida por jugador, quedar caído y
  que un compañero te reviva.
- **2D · móvil**: orientación landscape, controles en pantalla, optimizado para
  dispositivos de gama media.

---

## Stack técnico

| Área | Tecnología |
|------|------------|
| Motor | Unity **6000.5.4f1** |
| Render | Built-in (2D) |
| Multijugador | **Netcode for GameObjects** (host autoritativo) |
| Backend online | **Unity Services** — sesión anónima (Authentication + Core) |
| Plataforma objetivo | **Android** (móvil, landscape) |

El host resuelve la lógica autoritativa (daño, spawns); los clientes piden
acciones y ven las animaciones al instante.

---

## Estado del proyecto (roadmap por épicas)

| Épica | Descripción | Estado |
|-------|-------------|--------|
| **E1** | Fundación online: 4 jugadores en la misma arena | ✅ Completada |
| **E2** | Salas con código: crear, buscar y entrar | ✅ Completada |
| **E3** | Combate en red: espada, vida, caído y revivir | 🔨 En curso |
| **E4** | Oleadas: los 3 enemigos y el gestor de oleadas | ⏳ Pendiente |
| **E5** | Armas, monedas y jefe: la partida se termina | ⏳ Pendiente |
| **E6** | Identidad: arte oscuro, animaciones y audio | ⏳ Pendiente |
| **E7** | Publicación: menú, balance y APK en celulares reales | ⏳ Pendiente |

### E3 en detalle (sprint activo)

- ✅ **#66** Vida del jugador en red: 5 puntos, invulnerabilidad y barra propia
  — *aprobada por QA* (PR #112).
- ⏳ **#65** Botón ATACAR y espada contextual en 4 direcciones.
- ⏳ **#67** Barras de vida chicas sobre los compañeros.
- ⏳ **#68** Estado caído y revivir por proximidad de 3 s.
- ⏳ **#69** Enemigo Rastrero sincronizado (persigue, daña, muere de un espadazo).

El detalle vivo del avance está en los
[issues](https://github.com/manucastelnovo/mi-juego/issues) y en el tablero del
proyecto *Mi Juego - Sprint Board*.

---

## El equipo de agentes y el flujo de trabajo

El juego lo construye un equipo de agentes de IA, cada uno con su rol:

- **Scrum Master** — secuencia el sprint y mantiene el flujo.
- **Diseñador** — redacta borradores de GDD/historias para que el PO apruebe.
- **Programador C#** — implementa en Unity, trabaja en ramas y abre PRs.
- **QA** — valida lo técnico y **mergea a `main`** lo que aprueba.
- **Artista** — sprites, materiales y prefabs.

**Flujo dev → QA → merge:** el programador hace sus autopruebas, abre un PR y lo
marca `needs-qa`; QA revisa Play mode, consola y criterios uno por uno. El gate
es **"✅ Aprobado por QA"** (label `qa-approved`): recién ahí se mergea a `main`,
sin esperar al PO en cada PR. El PO decide la visión del juego y entra cuando
hace falta una decisión de diseño.

Las convenciones completas (identidad de los agentes, ramas, comentarios y
evidencia) están en [`CLAUDE.md`](./CLAUDE.md).

---

## Estructura del repositorio

```
Assets/
  Scripts/    Lógica del juego (red, salas, jugador, HUD, spawns)
  Prefabs/    Objetos reutilizables (Player, Coin)
  Scenes/     Menu.unity y Game.unity
  Art/        Sprites y materiales
docs/
  qa/         Evidencia de QA (capturas de las pruebas)
CLAUDE.md     Reglas del estudio y convenciones del equipo
```

---

## Cómo abrir el proyecto

1. Instalá **Unity Hub** y la versión de Unity **6000.5.4f1**.
2. Cloná el repositorio y abrí la carpeta con Unity Hub.
3. Escena de arranque: `Assets/Scenes/Menu.unity`.

### Probar el multijugador

El online necesita **al menos 2 instancias**: dos dispositivos/celulares, o dos
instancias en el editor/build. Uno crea la sala (host) y comparte el código; el
otro lo ingresa desde **BUSCAR SALA** para entrar. La prueba real de la feature
se hace con **dos celulares** en redes distintas.

---

*Salto Games — hecho por agentes, dirigido por un humano.*
