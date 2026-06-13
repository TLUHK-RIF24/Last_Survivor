using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public static class MacOSCleanBuilder
{
    private const string DefaultFileName = "LastSurvivor_Clean";

    [MenuItem("Last Survivor/Build/macOS Clean App")]
    public static void BuildMacOSCleanApp()
    {
        string outputPath = EditorUtility.SaveFilePanel(
            "Build macOS clean app",
            Path.Combine(MacOSBuildCacheUtility.ProjectRoot, "Builds/macOS"),
            DefaultFileName,
            "app");

        if (string.IsNullOrEmpty(outputPath))
        {
            return;
        }

        if (Directory.Exists(outputPath))
        {
            bool replace = EditorUtility.DisplayDialog(
                "Replace existing macOS build?",
                "The selected .app already exists. It must be removed first so Unity cannot reuse stale player data.",
                "Replace",
                "Cancel");

            if (!replace)
            {
                return;
            }

            FileUtil.DeleteFileOrDirectory(outputPath);
        }

        MacOSBuildCacheUtility.ClearMacOSPlayerDataCache();

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray(),
            locationPathName = outputPath,
            target = BuildTarget.StandaloneOSX,
            targetGroup = BuildTargetGroup.Standalone,
            options = BuildOptions.CleanBuildCache | BuildOptions.StrictMode
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new BuildPlayerWindow.BuildMethodException(
                $"macOS clean build failed: {summary.result}");
        }

        EditorUtility.RevealInFinder(outputPath);
    }
}

public sealed class MacOSBuildCacheCleaner : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.StandaloneOSX)
        {
            return;
        }

        MacOSBuildCacheUtility.ClearMacOSPlayerDataCache();
    }
}

internal static class MacOSBuildCacheUtility
{
    private const string MacOSPlayerDataCacheRelativePath = "Library/PlayerDataCache/OSXUniversal2";

    public static string ProjectRoot => Path.GetFullPath(
        Path.Combine(UnityEngine.Application.dataPath, ".."));

    public static void ClearMacOSPlayerDataCache()
    {
        string playerDataCachePath = Path.Combine(
            ProjectRoot,
            MacOSPlayerDataCacheRelativePath);

        if (Directory.Exists(playerDataCachePath))
        {
            UnityEngine.Debug.Log($"[MacOSBuildCacheCleaner] Removing {playerDataCachePath}");
            FileUtil.DeleteFileOrDirectory(playerDataCachePath);
        }
        else
        {
            UnityEngine.Debug.Log($"[MacOSBuildCacheCleaner] No cache found at {playerDataCachePath}");
        }
    }
}
