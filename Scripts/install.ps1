$unitysetup = "UnitySetup64.exe"
$unitysetupargs = "/S"

Write-Output 'Downloading from http://netstorage.unity3d.com/unity/649f48bbbf0f/Windows64EditorInstaller/UnitySetup64-5.4.1f1.exe: '
curl -o $unitysetup http://netstorage.unity3d.com/unity/649f48bbbf0f/Windows64EditorInstaller/UnitySetup64-5.4.1f1.exe

Write-Output 'Installing UnitySetup64.exe'
Start-Process $unitysetup $unitysetupargs -Wait 