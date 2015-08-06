#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="IsoUnity"

mkdir $(pwd)/Build/
echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -exportPackage Assets\IsoUnity "$(pwd)/Build/IsoUnity.unitypackage" \
  -quit

echo 'Logs from build'
cat $(pwd)/unity.log