# Ce script ouvre une nouvelle console PowerShell en admin et lance ton serveur
$cheminServeur = Split-Path -Parent $MyInvocation.MyCommand.Path
$executable = "Server.exe"

Start-Process powershell -Verb RunAs -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$cheminServeur'; .\\$executable"
)
