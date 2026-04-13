using UnityEditor;
using UnityEngine;
using UsefulTools.Editor;
using System.Collections.Generic;
using System.Linq;

public class AudioImportWatcher : AssetPostprocessor
{
    private static HashSet<string> _processingPaths = new HashSet<string>();

    // 全てのアセットインポート完了時に一括で呼ばれる
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 今回の処理（リネーム等）によるインポートは無視
        var newImports = importedAssets.Where(path => !_processingPaths.Contains(path)).ToList();
        
        // オーディオファイルのみを抽出（拡張子で判別）
        var audioPaths = newImports.Where(path => 
            path.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase) || 
            path.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase) || 
            path.EndsWith(".ogg", System.StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".aiff", System.StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".m4a", System.StringComparison.OrdinalIgnoreCase)
        ).ToList();

        if (audioPaths.Count > 0)
        {
            // インポートされたファイルをキューに渡してウィンドウを表示
            AudioImportTab.ShowWindow(audioPaths);
        }

        // 処理済みリストから今回移動・追加されたものを消す（次回の通常インポートに備える）
        foreach (var path in importedAssets) _processingPaths.Remove(path);
        foreach (var path in movedAssets) _processingPaths.Remove(path);
    }

    public static void MarkAsProcessing(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        _processingPaths.Add(path.Replace("\\", "/"));
    }
}
