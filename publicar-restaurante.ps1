# ==========================================================================
#  Publica la app C# (Blazor WASM) del restaurante a la carpeta servida
#  por GitHub Pages y la sube al repo.
#
#  Uso:
#     .\publicar-restaurante.ps1            # publica y hace commit + push
#     .\publicar-restaurante.ps1 -NoPush    # publica pero NO sube a GitHub
# ==========================================================================
param([switch]$NoPush)

$ErrorActionPreference = "Stop"
$repo    = $PSScriptRoot
$appDir  = Join-Path $repo "restaurante-app"
$dst     = Join-Path $repo "restaurante"
$baseHref = "/waltermontiel/restaurante/"

Write-Host "==> Publicando (Release)..." -ForegroundColor Cyan
dotnet publish $appDir -c Release -o (Join-Path $appDir "publish") --nologo -v quiet
$src = Join-Path $appDir "publish\wwwroot"
if (-not (Test-Path $src)) { throw "No se encontro la salida de publish en $src" }

Write-Host "==> Copiando a $dst ..." -ForegroundColor Cyan
if (Test-Path $dst) { Remove-Item $dst -Recurse -Force }
New-Item -ItemType Directory -Path $dst | Out-Null
Copy-Item "$src\*" $dst -Recurse -Force

# Ajustar base href para la subruta de GitHub Pages
$idx = Join-Path $dst "index.html"
(Get-Content $idx -Raw) -replace '<base href="/" />', "<base href=""$baseHref"" />" | Set-Content $idx -Encoding utf8

# .nojekyll para que se sirva la carpeta _framework
New-Item -ItemType File -Path (Join-Path $dst ".nojekyll") -Force | Out-Null

Write-Host "==> Listo. base href = $baseHref" -ForegroundColor Green

if ($NoPush) {
    Write-Host "==> -NoPush: no se sube a GitHub. Revisa con 'dotnet run' o git status." -ForegroundColor Yellow
    return
}

Write-Host "==> Subiendo a GitHub..." -ForegroundColor Cyan
git -C $repo add -A restaurante restaurante-app
git -C $repo -c user.name="wmontiel123" -c user.email="walterlorenzopy@hotmail.com" commit -m "Republicar app de restaurante"
git -C $repo push

Write-Host "==> Hecho. Demo: https://wmontiel123.github.io$baseHref" -ForegroundColor Green
