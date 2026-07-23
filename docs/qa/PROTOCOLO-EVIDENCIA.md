# Protocolo de evidencia de QA para multijugador

Checklist de trabajo para QA en Vigilia. Nace del punto ciego de #21 (una
captura vieja del Game View aprobó un PR que no funcionaba) y se vuelve
crítico ahora que **QA mergea a `main` sin paso del PO**: una evidencia mal
tomada ya no la atrapa nadie más. Este documento es lo único que separa un
bug de la rama principal.

## 1. Ranking de evidencia válida

De más a menos confiable. Si podés dar el nivel de arriba, no te conformes
con el de abajo.

1. **Estado de objetos verificado por código** (el más fuerte). Inspeccionar
   en Play mode, por código: posiciones, velocidades, cantidad de instancias,
   valores de componentes. Si hace falta forzar una colisión, usar
   `Physics2D.Simulate` en modo `Script` para que ocurra de verdad, no
   esperar a que "se vea".
2. **Consola limpia, leída después de limpiarla.** Limpiar la consola antes
   de la prueba (`Clear on Play` o botón Clear), correr la acción, releer.
   Si no la limpiaste antes, un error viejo puede colarse como si fuera
   nuevo, o al revés.
3. **Captura de pantalla.** Es el nivel más débil y **nunca alcanza sola**:
   el Game View puede devolver un cuadro viejo o directamente vacío cuando
   Unity corre en contexto automatizado (frame loop congelado). Una captura
   solo sirve como respaldo visual de algo que ya probaste por código o por
   consola, no como prueba en sí misma.

**Regla:** todo criterio de aceptación que hable de comportamiento (se
mueve, salta, colisiona, se sincroniza) se valida con 1 o 2. La captura
acompaña, no reemplaza.

## 2. Verificar multijugador: las dos puntas

Un bug de red casi siempre aparece de un solo lado. Mirar solo el host (o
solo un cliente) no prueba nada.

- Usar los **jugadores virtuales de Multiplayer Play Mode** para tener host
  + al menos un cliente corriendo a la vez.
- Para cada criterio de comportamiento, repetir la verificación (nivel 1 o 2
  del ranking) **en el host y en cada cliente por separado**. No alcanza con
  verificar en uno y asumir que el otro está igual.
- Consola: limpiar y leer **la consola del host y la de cada cliente**,
  como entradas independientes. Un warning/error puede aparecer solo en uno.
- Si un criterio depende de "todos ven lo mismo" (ej. posiciones
  sincronizadas, sin duplicados), comparar el estado de objetos leído en
  host contra el leído en cliente, no solo mirar cada uno por separado.

## 3. Detectar que el editor está mintiendo

| Señal | Qué significa | Qué hacer |
|---|---|---|
| Cambiaste un script pero el comportamiento no varía, o el error de consola no coincide con la línea actual del archivo | El editor no recompiló (pasa si la ventana de Unity no tiene foco) | Forzar reimport del script afectado. Si sigue sin reflejar el cambio, pedirle al humano que reinicie el editor. No sigas probando sobre una compilación vieja. |
| La captura muestra la escena en un estado que no corresponde a lo que acabás de hacer, o llega en blanco | Frame viejo o vacío del Game View | Descartar la captura como evidencia por sí sola; bajar al ranking de nivel 1/2 (estado por código, consola) |
| Las llamadas al puente MCP quedan en reintento de reconexión durante varios minutos | El puente Unity↔MCP se cayó | No hay editor disponible: dejar de intentar verificar, documentarlo en el PR (ver sección 4) y avisar que hace falta reconexión/reinicio |

## 4. Qué hacer cuando algo no se puede verificar

Regla innegociable: **se declara explícitamente en el PR y no se mergea** si
el criterio no verificado es la razón de ser del ticket. Nunca dar por buena
una prueba que no corrió.

Plantilla para el PR:

```
### 🧪 QA · Salto Games
**No verificado:** <criterio de aceptación exacto>
**Motivo:** <editor caído / no recompiló / frame congelado / requiere sensación en vivo>
**Estado:** pendiente — lo confirma el PO jugando / se reintenta cuando <condición>
```

Si el criterio no verificado es de los que dependen de "cómo se siente"
(latencia percibida, tirones, sensación de control) — eso **nunca** lo
valida QA por definición: se marca directamente como **"lo confirma el PO
jugando"**, sin intentar reemplazarlo con una métrica.

## 5. Ruido conocido: NO son fallas

No reportar como bug, no bloquear el PR por esto:

- Warning de **Input Manager** (viejo, previo a cualquier cambio de este proyecto).
- Warning de **Dynamic Batching** (viejo, previo, sin relación con features nuevas).
- `[Package Manager Window] UPM server is not running` — normal en los
  jugadores virtuales de Multiplayer Play Mode, no indica un problema.

## 6. Capturas: dónde van y cómo enlazarlas

- Guardar las capturas en `docs/qa/` (fuera de `Assets/`, así Unity no las
  importa como asset).
- El repo es **privado**: las URL `raw.githubusercontent.com` no renderizan
  (el proxy de imágenes de GitHub no se autentica ahí). No usarlas.
- En el comentario del PR, dejar **enlaces clicables al blob**, no imágenes
  embebidas:
  `[descripción](https://github.com/manucastelnovo/mi-juego/blob/<rama>/docs/qa/<archivo>.png)`
- Si querés que la imagen se vea inline, la única forma es arrastrarla al
  cuadro de comentario en el navegador (no se puede por CLI).

## Referencia desde el ciclo de QA

Este protocolo aplica siempre que QA valide algo con jugadores múltiples
(host + cliente/s). Usarlo junto con el gate normal de QA (`needs-qa` →
"✅ Aprobado por QA") descrito en `CLAUDE.md`.
