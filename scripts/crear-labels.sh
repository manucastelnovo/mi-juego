#!/usr/bin/env bash
# crear-labels.sh
# Crea los labels de Scrum en tu repo de GitHub usando el CLI `gh`.
#
# Uso (Git Bash / Linux / macOS, desde la raíz del repo):
#   ./scripts/crear-labels.sh
#   REPO="tu-usuario/mi-juego" ./scripts/crear-labels.sh
#
# Requiere: gh instalado y autenticado (gh auth login).

set -euo pipefail

# Si defines REPO, se lo pasamos a gh; si no, usa el repo actual.
REPO_ARG=()
if [[ -n "${REPO:-}" ]]; then
  REPO_ARG=(--repo "$REPO")
fi

# nombre|color|descripcion
labels=(
  "epic|6f42c1|Sistema grande del juego. Contiene historias."
  "story|0e8a16|Historia de usuario jugable con criterios de aceptacion."
  "task|1d76db|Tarea tecnica concreta bajo una historia."
  "bug|d73a4a|Fallo tecnico detectado por QA o el Product Owner."
  "art|e99695|Trabajo visual: sprites, materiales, animacion."
  "asset|fbca04|Gestion e importacion de assets del proyecto."
)

for entry in "${labels[@]}"; do
  IFS='|' read -r name color desc <<< "$entry"
  echo "Creando label '$name'..."
  # --force actualiza el label si ya existe.
  gh label create "$name" --color "$color" --description "$desc" --force "${REPO_ARG[@]}"
done

echo "Listo. Labels creados/actualizados."
