using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UsefulTools.Editor;

public class AudioImportTab : EditorWindow
{
    public enum AudioCategory { BGM, SE }

    private class ImportItem
    {
        public AudioClip Clip;
        public string Path;
        public string NewName;
        public AudioCategory Category;
    }

    private List<ImportItem> _items = new List<ImportItem>();
    private int _currentIndex = 0;
    private Vector2 _scrollPos;

    public static void ShowWindow(List<string> paths)
    {
        var window = GetWindow<AudioImportTab>("Audio Import Tool");
        window.minSize = new Vector2(400, 300);

        bool added = false;
        foreach (var path in paths)
        {
            if (window._items.Any(i => i.Path == path)) continue;

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null) continue;

            AudioCategory initialCategory = AudioCategory.SE;

            if (AudioSupportTool.AutoCategoryEnabled)
            {
                initialCategory = (clip.length >= AudioSupportTool.AutoCategoryThreshold) ? AudioCategory.BGM : AudioCategory.SE;
            }
            else
            {
                string lowerPath = path.ToLower().Replace("\\", "/");
                if (lowerPath.Contains("/bgm/")) initialCategory = AudioCategory.BGM;
                else if (lowerPath.Contains("/se/")) initialCategory = AudioCategory.SE;
                else initialCategory = (clip.length >= AudioSupportTool.AutoCategoryThreshold) ? AudioCategory.BGM : AudioCategory.SE;
            }

            window._items.Add(new ImportItem
            {
                Clip = clip,
                Path = path,
                NewName = Path.GetFileNameWithoutExtension(path),
                Category = initialCategory
            });
            added = true;
        }
        
        if (added)
        {
            window.Show();
            window.Focus();
        }
    }

    private void OnGUI()
    {
        if (_items == null || _items.Count == 0 || _currentIndex >= _items.Count)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.inspectorDefaultMargins))
            {
                EditorGUILayout.HelpBox("全ての処理が完了しました。", MessageType.Info);
                if (GUILayout.Button("Close", GUILayout.Height(30))) Close();
            }
            return;
        }

        var current = _items[_currentIndex];

        using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
        {
            _scrollPos = scroll.scrollPosition;
            using (new EditorGUILayout.VerticalScope(EditorStyles.inspectorDefaultMargins))
            {
                EditorGUILayout.LabelField($"Audio Import Queue: {_currentIndex + 1} / {_items.Count}", EditorStyles.boldLabel);
                
                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Target Clip", current.Clip.name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Current Path", current.Path, EditorStyles.miniLabel);

                    EditorGUILayout.Space();

                    current.NewName = EditorGUILayout.TextField("New File Name", current.NewName);
                    
                    // 日本語が含まれている場合の警告表示
                    if (HasJapanese(current.NewName))
                    {
                        EditorGUILayout.HelpBox("日本語のファイル名は許可されていません。適用すると削除されます。", MessageType.Error);
                    }

                    current.Category = (AudioCategory)EditorGUILayout.EnumPopup("Category", current.Category);
                }

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Apply & Next", GUILayout.Height(40)))
                    {
                        if (ApplyCurrent()) _currentIndex++;
                        if (_currentIndex >= _items.Count) FinalizeImport();
                    }

                    if (GUILayout.Button("Skip", GUILayout.Height(40)))
                    {
                        _currentIndex++;
                        if (_currentIndex >= _items.Count) FinalizeImport();
                    }
                }
                
                EditorGUILayout.Space(10);
                
                if (GUILayout.Button("Apply All Remaining"))
                {
                    if (EditorUtility.DisplayDialog("Confirm", "残りの全ファイルをインポートしますか？（日本語ファイル名は削除されます）", "Yes", "No"))
                    {
                        ApplyAllRemaining();
                    }
                }
            }
        }
    }

    private bool ApplyCurrent()
    {
        var current = _items[_currentIndex];
        
        // 日本語が含まれている場合は削除してスキップ
        if (HasJapanese(current.NewName))
        {
            Debug.LogError($"[UsefulTools] Japanese characters detected in New Name: {current.NewName}. Deleting asset.");
            AssetDatabase.DeleteAsset(current.Path);
            return true;
        }

        string targetFolder = (current.Category == AudioCategory.BGM) ? AudioSupportTool.BGMFolder : AudioSupportTool.SEFolder;
        EnsureFolderExists(targetFolder);
        AssetDatabase.Refresh();

        return MoveAsset(current, targetFolder);
    }

    private bool MoveAsset(ImportItem item, string targetFolder)
    {
        string extension = Path.GetExtension(item.Path);
        string newPath = (targetFolder.EndsWith("/") ? targetFolder : targetFolder + "/") + item.NewName + extension;
        newPath = newPath.Replace("\\", "/");

        if (item.Path == newPath) return true;

        if (AssetDatabase.LoadAssetAtPath<Object>(newPath) != null)
        {
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
        }

        AudioImportWatcher.MarkAsProcessing(newPath);
        AudioImportWatcher.MarkAsProcessing(item.Path);

        string error = AssetDatabase.MoveAsset(item.Path, newPath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError($"[UsefulTools] Failed to move: {error} (Target: {newPath})");
            return false;
        }
        else
        {
            item.Path = newPath;
            return true;
        }
    }

    private void EnsureFolderExists(string folderPath)
    {
        folderPath = folderPath.Replace("\\", "/");
        if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath)) return;

        string[] folders = folderPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            if (string.IsNullOrEmpty(folders[i])) continue;
            string nextPath = currentPath + "/" + folders[i];
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            }
            currentPath = nextPath;
        }
    }

    private void ApplyAllRemaining()
    {
        EnsureFolderExists(AudioSupportTool.BGMFolder);
        EnsureFolderExists(AudioSupportTool.SEFolder);
        AssetDatabase.Refresh();

        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = _currentIndex; i < _items.Count; i++)
            {
                _currentIndex = i;
                ApplyCurrent();
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        _currentIndex = _items.Count;
        FinalizeImport();
    }

    private void FinalizeImport()
    {
        AssetDatabase.SaveAssets();
        if (AudioSupportTool.GenerateEnumEnabled)
        {
            AudioEnumGenerator.Generate();
        }
        Close();
    }

    private bool HasJapanese(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        return System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FFF]");
    }
}
