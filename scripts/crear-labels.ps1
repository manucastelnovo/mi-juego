# crear-labels.ps1
# Crea los labels de Scrum en tu repo de GitHub usando el CLI `gh`.
#
# Uso (PowerShell, desde la raíz del repo o indicando --repo):
#   ./scripts/crear-labels.ps1
#   ./scripts/crear-labels.ps1 -Repo "tu-usuario/mi-juego"
#
# Requiere: gh instalado y autenticado (gh auth login).

param(
    [string]$Repo = ""
)

# Si no se pasa -Repo, gh usa el repo del directorio actual.
$repoArg = if ($Repo -ne "") { @("--repo", $Repo) } else { @() }

# Definición de labels: nombre, color (hex sin #), descripción.
$labels = @(
    @{ name = "epic";  color = "6f42c1"; desc = "Sistema grande del juego. Contiene historias." },
    @{ name = "story"; color = "0e8a16"; desc = "Historia de usuario jugable con criterios de aceptacion." },
    @{ name = "task";  color = "1d76db"; desc = "Tarea tecnica concreta bajo una historia." },
    @{ name = "bug";   color = "d73a4a"; desc = "Fallo tecnico detectado por QA o el Product Owner." },
    @{ name = "art";   color = "e99695"; desc = "Trabajo visual: sprites, materiales, animacion." },
    @{ name = "asset"; color = "fbca04"; desc = "Gestion e importacion de assets del proyecto." }
)

foreach ($l in $labels) {
    Write-Host "Creando label '$($l.name)'..." -ForegroundColor Cyan
    # --force actualiza el label si ya existe (color/descripcion).
    gh label create $l.name --color $l.color --description $l.desc --force @repoArg
}

Write-Host "Listo. Labels creados/actualizados." -ForegroundColor Green
