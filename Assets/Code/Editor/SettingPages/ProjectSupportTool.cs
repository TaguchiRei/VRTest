using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace UsefulTools.Editor
{
    /// <summary>
    /// フォルダ階層を表現するクラス
    /// </summary>
    [System.Serializable]
    public class DirectoryHierarchy
    {
        public string folderName;
        public string folderPath;
        public List<DirectoryHierarchy> children = new List<DirectoryHierarchy>();
        public bool isExpanded = false;
        public bool isIgnored = false; // gitignore対象かどうか

        public DirectoryHierarchy(string name, string path)
        {
            folderName = name;
            folderPath = path;
        }
    }

    public class ProjectSupportTool : SettingPageBase
    {
        public override string Name => "Project Support";

        // --- Package Keys ---
        private const string UniTaskUrlKey = "UsefulTools.Code.UniTaskUrl";
        private const string VContainerUrlKey = "UsefulTools.Code.VContainerUrl";
        private const string AddressablesPkgKey = "UsefulTools.Code.AddressablesPkg";

        private const string DefaultUniTaskUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
        private const string DefaultVContainerUrl = "https://github.com/hadashiA/VContainer.git";
        private const string DefaultAddressablesPkg = "com.unity.addressables";

        public static string UniTaskUrl { get => EditorPrefs.GetString(UniTaskUrlKey, DefaultUniTaskUrl); set => EditorPrefs.SetString(UniTaskUrlKey, value); }
        public static string VContainerUrl { get => EditorPrefs.GetString(VContainerUrlKey, DefaultVContainerUrl); set => EditorPrefs.SetString(VContainerUrlKey, value); }
        public static string AddressablesPkg { get => EditorPrefs.GetString(AddressablesPkgKey, DefaultAddressablesPkg); set => EditorPrefs.SetString(AddressablesPkgKey, value); }

        private static AddRequest _request;

        // --- Hierarchy Fields ---
        private static List<DirectoryHierarchy> _intendedStructure;
        private List<DirectoryHierarchy> _currentStructure;
        private bool _showIntended = true;
        private bool _showCurrent = true;
        private bool _showGitIgnore = true;

        private HashSet<string> _gitIgnoreEntries = new HashSet<string>();
        private string _gitIgnorePath;

        public override void Initialize()
        {
            _gitIgnorePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".gitignore");
            RefreshIntendedStructure();
            LoadGitIgnore();
            ScanCurrentStructure();
        }

        private void RefreshIntendedStructure()
        {
            _intendedStructure = new List<DirectoryHierarchy>();
            var art = new DirectoryHierarchy("Art", "Assets/Art");
            art.children.Add(new DirectoryHierarchy("Materials", "Assets/Art/Materials"));
            art.children.Add(new DirectoryHierarchy("Models", "Assets/Art/Models"));
            art.children.Add(new DirectoryHierarchy("Textures", "Assets/Art/Textures"));

            var code = new DirectoryHierarchy("Code", "Assets/Code");
            code.children.Add(new DirectoryHierarchy("Scripts", "Assets/Code/Scripts"));
            code.children.Add(new DirectoryHierarchy("Editor", "Assets/Code/Editor"));

            _intendedStructure.Add(art);
            _intendedStructure.Add(code);
            _intendedStructure.Add(new DirectoryHierarchy("Audio", "Assets/Audio"));
            _intendedStructure.Add(new DirectoryHierarchy("Docs", "Assets/Docs"));
            _intendedStructure.Add(new DirectoryHierarchy("Level", "Assets/Level"));
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Project Setup & Hierarchy Support", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 1. Package Import Settings
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Package Import URLs", EditorStyles.miniBoldLabel);
                UniTaskUrl = EditorGUILayout.TextField("UniTask URL", UniTaskUrl);
                VContainerUrl = EditorGUILayout.TextField("VContainer URL", VContainerUrl);
                AddressablesPkg = EditorGUILayout.TextField("Addressables Package", AddressablesPkg);

                EditorGUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Import UniTask")) Install(UniTaskUrl);
                    if (GUILayout.Button("Import VContainer")) Install(VContainerUrl);
                    if (GUILayout.Button("Import Addressables")) Install(AddressablesPkg);
                }
            }

            EditorGUILayout.Space();

            // 2. Intended Hierarchy
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _showIntended = EditorGUILayout.Foldout(_showIntended, "Intended Structure Preview", true);
                if (_showIntended)
                {
                    DrawHierarchyList(_intendedStructure, 0, false);
                    if (GUILayout.Button("Apply Intended Structure", GUILayout.Height(25)))
                    {
                        ApplyStructure(_intendedStructure);
                        ScanCurrentStructure();
                    }
                }
            }

            EditorGUILayout.Space();

            // 3. Current Hierarchy & GitIgnore
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _showCurrent = EditorGUILayout.Foldout(_showCurrent, ".gitignore Management & Folder Structure", true);
                    if (GUILayout.Button("Scan", GUILayout.Width(60))) ScanCurrentStructure();
                }

                if (_showCurrent)
                {
                    EditorGUILayout.HelpBox("Select folders to add to or remove from .gitignore.", MessageType.Info);
                    
                    if (_currentStructure == null || _currentStructure.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Press 'Scan' to visualize structure.", MessageType.None);
                    }
                    else
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            DrawHierarchyList(_currentStructure, 0, true);
                        }
                    }

                    EditorGUILayout.Space(5);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Save to .gitignore", GUILayout.Height(30)))
                        {
                            SaveGitIgnore();
                        }
                        if (GUILayout.Button("Reload .gitignore", GUILayout.Width(120), GUILayout.Height(30)))
                        {
                            LoadGitIgnore();
                            ScanCurrentStructure();
                        }
                    }
                }
            }
        }

        private void DrawHierarchyList(List<DirectoryHierarchy> list, int depth, bool showIgnoreToggle)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(depth * 16);
                    bool hasChildren = item.children != null && item.children.Count > 0;
                    
                    // Foldout
                    Rect rect = GUILayoutUtility.GetRect(16, 16, GUILayout.ExpandWidth(false));
                    if (hasChildren) item.isExpanded = EditorGUI.Foldout(rect, item.isExpanded, "", true);
                    else GUILayout.Space(16);

                    // Folder Icon
                    bool exists = AssetDatabase.IsValidFolder(item.folderPath);
                    GUILayout.Label(exists ? "📁" : "⚪", GUILayout.Width(18));
                    
                    // Folder Name
                    EditorGUILayout.LabelField(item.folderName, EditorStyles.miniLabel);

                    if (showIgnoreToggle && exists)
                    {
                        GUILayout.FlexibleSpace();
                        bool prevIgnored = item.isIgnored;
                        item.isIgnored = EditorGUILayout.ToggleLeft("Ignore", item.isIgnored, GUILayout.Width(60));
                        
                        // 子階層も再帰的に変更するかどうか（オプション）
                        if (prevIgnored != item.isIgnored && Event.current.shift)
                        {
                            SetIgnoreRecursive(item, item.isIgnored);
                        }
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }
                }
                if (item.isExpanded && item.children != null && item.children.Count > 0) DrawHierarchyList(item.children, depth + 1, showIgnoreToggle);
            }
        }

        private void SetIgnoreRecursive(DirectoryHierarchy item, bool ignore)
        {
            item.isIgnored = ignore;
            foreach (var child in item.children) SetIgnoreRecursive(child, ignore);
        }

        private void ScanCurrentStructure()
        {
            _currentStructure = new List<DirectoryHierarchy>();
            if (!Directory.Exists(Application.dataPath)) return;

            string[] dirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                string folderName = Path.GetFileName(dir);
                _currentStructure.Add(BuildHierarchyRecursive("Assets/" + folderName));
            }
        }

        private DirectoryHierarchy BuildHierarchyRecursive(string relativePath)
        {
            var hierarchy = new DirectoryHierarchy(Path.GetFileName(relativePath), relativePath);
            
            // gitignoreの状態を確認
            string ignorePath = relativePath.EndsWith("/") ? relativePath : relativePath + "/";
            hierarchy.isIgnored = _gitIgnoreEntries.Contains(ignorePath);

            string systemPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, relativePath);
            if (Directory.Exists(systemPath))
            {
                foreach (var subDir in Directory.GetDirectories(systemPath))
                {
                    hierarchy.children.Add(BuildHierarchyRecursive(relativePath + "/" + Path.GetFileName(subDir)));
                }
            }
            return hierarchy;
        }

        private void LoadGitIgnore()
        {
            _gitIgnoreEntries.Clear();
            if (File.Exists(_gitIgnorePath))
            {
                string[] lines = File.ReadAllLines(_gitIgnorePath);
                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !trimmed.StartsWith("#"))
                    {
                        _gitIgnoreEntries.Add(trimmed);
                    }
                }
            }
        }

        private void SaveGitIgnore()
        {
            List<string> lines = new List<string>();
            if (File.Exists(_gitIgnorePath))
            {
                lines = File.ReadAllLines(_gitIgnorePath).ToList();
            }

            // 現在の構造から ignore すべきパスを収集
            HashSet<string> currentUIEntries = new HashSet<string>();
            CollectIgnoredPaths(_currentStructure, currentUIEntries);

            // 既存の行を整理
            // 1. UIに表示されているパスに関連する既存のエントリを一度クリア
            lines.RemoveAll(line => {
                string trimmed = line.Trim();
                return trimmed.StartsWith("Assets/") && (trimmed.EndsWith("/") || !trimmed.Contains("."));
            });

            // 2. UIでチェックされているパスを追加
            foreach (var path in currentUIEntries)
            {
                lines.Add(path);
            }

            // 3. 重複削除と空行整理
            var finalLines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Distinct().ToList();

            try
            {
                File.WriteAllLines(_gitIgnorePath, finalLines, System.Text.Encoding.UTF8);
                Debug.Log($"[UsefulTools] .gitignore updated. Path: {_gitIgnorePath}");
                LoadGitIgnore(); // 再読み込み
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UsefulTools] Failed to save .gitignore: {e.Message}");
            }
        }

        private void CollectIgnoredPaths(List<DirectoryHierarchy> list, HashSet<string> result)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                if (item.isIgnored)
                {
                    string path = item.folderPath.EndsWith("/") ? item.folderPath : item.folderPath + "/";
                    result.Add(path);
                }
                CollectIgnoredPaths(item.children, result);
            }
        }

        private void ApplyStructure(List<DirectoryHierarchy> list)
        {
            foreach (var item in list)
            {
                if (!AssetDatabase.IsValidFolder(item.folderPath))
                {
                    string parent = Path.GetDirectoryName(item.folderPath).Replace("\\", "/");
                    string folder = Path.GetFileName(item.folderPath);
                    if (AssetDatabase.IsValidFolder(parent))
                    {
                        AssetDatabase.CreateFolder(parent, folder);
                    }
                }
                if (item.children.Count > 0) ApplyStructure(item.children);
            }
            AssetDatabase.Refresh();
        }

        private void Install(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) return;
            _request = Client.Add(identifier);
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (_request == null) { EditorApplication.update -= Progress; return; }
            if (_request.IsCompleted)
            {
                if (_request.Status == StatusCode.Success) Debug.Log("[UsefulTools] Package installed successfully.");
                else if (_request.Status >= StatusCode.Failure) Debug.LogError($"[UsefulTools] Package install failed: {_request.Error.message}");
                EditorApplication.update -= Progress;
                _request = null;
            }
        }
    }
}
