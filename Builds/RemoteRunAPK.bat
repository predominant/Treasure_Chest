@echo off

Set filename=%~1
For %%A in ("%filename%") do (
    Set APKFolder=%%~dpA
    Set APKName=%%~nA
)

echo.Beginning Starting ADB Build process
echo.Exporting %APKName% from %APKFolder%



Set ANDROID_SDK="M:\Program Files\Android\sdk"
Set ADB_DIR=%ANDROID_SDK%"\platform-tools"
Set Prefix=com
Set Company=LogicalDragonGames
Set APK=com.%Company%.%APKName%




@echo on

CD "%ADB_DIR%"


adb uninstall %APK%
adb forward tcp:54999 localabstract:Unity-com.LogicalDragonGames.TreasureChest
adb install -r "%~1"
adb shell am start -n com.LogicalDragonGames.TreasureChest/com.unity3d.player.UnityPlayerNativeActivity

PAUSE