#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build
$project="IsoUnity"
$p = (Get-Location).tostring()
$unity = "C:\\Program Files\\Unity\\Unity.exe"
$arguments = "-batchmode -nographics -silent-crashes -logFile $p\\unity.log -projectPath $p -exportPackage Assets/IsoUnity $p\\Build\\$project.unitypackage -quit"

Write-Output "Creating Build dir"
New-Item -ItemType directory -Path .\Build\
Write-Output "Attempting to build $project for Windows"
Start-Process $unity $arguments -Wait 

Write-Output 'Logs from build'
Get-Content .\unity.log
Write-Output 'Build file dir:'
Get-ChildItem .\Build\