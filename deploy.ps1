# デプロイスクリプト - WebGLビルドをgh-pagesブランチにプッシュ
# 使い方: .\deploy.ps1

param(
    [string]$BuildPath = "C:\Users\kobe_\Unity\AvatarScape\Build",
    [string]$Message = "Deploy WebGL build"
)

$RepoRoot = $PSScriptRoot
$TempDir = Join-Path $env:TEMP "avatarscape-deploy"

Write-Host "=== AvatarScape Deploy Script ===" -ForegroundColor Cyan

# ビルドフォルダの確認
if (-not (Test-Path "$BuildPath\index.html")) {
    Write-Host "Error: Build not found at $BuildPath" -ForegroundColor Red
    Write-Host "Please build WebGL first in Unity." -ForegroundColor Yellow
    exit 1
}

Write-Host "Build found at: $BuildPath" -ForegroundColor Green

# 一時フォルダを準備
if (Test-Path $TempDir) {
    Remove-Item -Recurse -Force $TempDir
}
New-Item -ItemType Directory -Path $TempDir | Out-Null

# gh-pagesブランチをクローン（shallow）
Write-Host "Cloning gh-pages branch..." -ForegroundColor Yellow
git clone --branch gh-pages --single-branch --depth 1 "$(git remote get-url origin)" $TempDir 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to clone gh-pages branch" -ForegroundColor Red
    exit 1
}

# 古いファイルを削除（.gitは残す）
Get-ChildItem $TempDir -Exclude ".git" | Remove-Item -Recurse -Force

# ビルドファイルをコピー
Write-Host "Copying build files..." -ForegroundColor Yellow
Copy-Item -Path "$BuildPath\*" -Destination $TempDir -Recurse

# コミット＆プッシュ
Push-Location $TempDir
try {
    git add -A
    
    # 変更があるか確認
    $changes = git status --porcelain
    if (-not $changes) {
        Write-Host "No changes to deploy." -ForegroundColor Yellow
        exit 0
    }
    
    git commit -m $Message
    git push --force origin gh-pages
    
    Write-Host "" 
    Write-Host "=== Deploy Complete! ===" -ForegroundColor Green
    Write-Host "URL: https://bekosantux.github.io/AvatarScape/" -ForegroundColor Cyan
}
finally {
    Pop-Location
}

# クリーンアップ
Remove-Item -Recurse -Force $TempDir
