using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UsefulTools.UtilityUnity.Runtime.Initialize;
using UsefulTools.UtilityUnity.Runtime.UtilityUnity;

public class CodeGenerator : EditorWindow
{
    private string _codeName = "NewCode";
    private string _code;
    private Vector2 _scrollPosition;
    private GenerateMode _generateMode;
    
    private GUIStyle _headerStyle;
    private GUIStyle _sectionStyle;

    private Func<string, string> _generateCodeFunc;

    //トグル表示
    private bool _showSimpleClass;
    private bool _showOthers;
    private bool _showOptions;

    private bool _isPushButton;
    private bool _isEmptyName;

    //コード改造
    private bool _isSerializable;
    private bool _useSummary;
    private AccessModifier _accessModifier;
    private OtherModifier _otherModifier;

    // シーン初期化コード用
    private string _sceneNamespace = "UsefulTools.Composition.Runtime.Boot";
    private string _sceneGenerationPath = "Assets/Code/Scripts/Composition/Boot";
    private bool _showSceneBoot = true;
    private bool _showClassTemplates = false;

    private void InitDefaultNamespace()
    {
        if (string.IsNullOrEmpty(_sceneNamespace) || _sceneNamespace == "UsefulTools.Composition.Runtime.Boot")
        {
            string projectName = Application.productName.Replace(" ", "");
            _sceneNamespace = $"{projectName}.Composition.Runtime.Boot";
        }
    }

    private void InitStyles()
    {
        if (_headerStyle == null)
        {
            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                margin = new RectOffset(5, 5, 10, 5)
            };
        }

        if (_sectionStyle == null)
        {
            _sectionStyle = new GUIStyle("HelpBox")
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
        }
    }


    [MenuItem("UsefulTools/Code Generator")]
    public static void ShowWindow()
    {
        GetWindow<CodeGenerator>("Code Generator");
    }

    private void OnEnable()
    {
        _generateCodeFunc = GetSimpleCsCode;
        _generateMode = GenerateMode.SimpleCs;
        _showOptions = true;
        _showSimpleClass = true;
        _showOthers = true;
        _showClassTemplates = true;

        var activeScene = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(activeScene.name))
        {
            _codeName = activeScene.name;
        }
    }

    public void OnGUI()
    {
        InitStyles();

        EditorGUILayout.LabelField("Useful Tools: Code Generator", _headerStyle);
        EditorGUILayout.Space();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        #region SceneBoot

        EditorGUILayout.BeginVertical(_sectionStyle);
        _showSceneBoot = EditorGUILayout.Foldout(_showSceneBoot,
            new GUIContent(" Scene Boot", EditorGUIUtility.IconContent("SceneAsset Icon").image), true);
        if (_showSceneBoot)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            _sceneNamespace = EditorGUILayout.TextField("Namespace", _sceneNamespace);
            _sceneGenerationPath = EditorGUILayout.TextField("Generation Path", _sceneGenerationPath);

            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            if (GUILayout.Button("Generate Scene Boot & Container", GUILayout.Height(30)))
            {
                GenerateSceneBoot();
            }

            GUI.backgroundColor = Color.white;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.Space();

        #region Class Templates

        EditorGUILayout.BeginVertical(_sectionStyle);
        _showClassTemplates = EditorGUILayout.Foldout(_showClassTemplates,
            new GUIContent(" Class Templates", EditorGUIUtility.IconContent("cs Script Icon").image), true);
        if (_showClassTemplates)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();

            #region Name Field

            EditorGUILayout.LabelField("Class Name", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            _codeName = EditorGUILayout.TextField(_codeName);
            if (EditorGUI.EndChangeCheck())
            {
                _isPushButton = true;
                _codeName = ToPascalCase(_codeName);
            }

            #endregion

            EditorGUILayout.Space();

            #region Options

            _showOptions = EditorGUILayout.Foldout(_showOptions, "Options", true);
            if (_showOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                _useSummary = EditorGUILayout.Toggle("Use Summary", _useSummary);
                IsSerializable();
                _accessModifier = (AccessModifier)EditorGUILayout.EnumPopup("Access Modifier", _accessModifier);
                _otherModifier = (OtherModifier)EditorGUILayout.EnumPopup("Other Modifier", _otherModifier);

                if (EditorGUI.EndChangeCheck())
                {
                    _isPushButton = true;
                }

                EditorGUI.indentLevel--;
            }

            #endregion

            EditorGUILayout.Space();

            #region Code Preview

            EditorGUILayout.LabelField("Code Preview", EditorStyles.boldLabel);
            if (_isPushButton)
            {
                _code = _generateCodeFunc(_codeName);
                _isPushButton = false;
            }

            _code = EditorGUILayout.TextArea(_code, GUILayout.Height(200));

            #endregion

            EditorGUILayout.Space();

            #region Templates

            DrawTemplateButtons();

            #endregion

            EditorGUILayout.Space();

            #region Final Generate Button

            if (string.IsNullOrEmpty(_codeName))
            {
                EditorGUILayout.HelpBox("No Code name provided", MessageType.Error);
            }
            else
            {
                GUI.backgroundColor = new Color(0.7f, 0.8f, 1f);
                if (GUILayout.Button("Generate Code File", GUILayout.Height(30)))
                {
                    GenerateCode(_code);
                }

                GUI.backgroundColor = Color.white;
            }

            #endregion

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();

        #endregion

        GUILayout.EndScrollView();
    }

    private void DrawTemplateButtons()
    {
        EditorGUILayout.LabelField("Template Selection", EditorStyles.boldLabel);

        _showSimpleClass = EditorGUILayout.Foldout(_showSimpleClass, "Simple Templates", true);
        if (_showSimpleClass)
        {
            DrawButtonGroup(new[]
            {
                ("SimpleCS", (Action)(() =>
                {
                    _generateCodeFunc = GetSimpleCsCode;
                    _generateMode = GenerateMode.SimpleCs;
                })),
                ("MonoBehaviour", () =>
                {
                    _generateCodeFunc = GetSimpleMonoBehaviourScript;
                    _generateMode = GenerateMode.MonoBehaviour;
                }),
                ("ScriptableObject", () =>
                {
                    _generateCodeFunc = GetSimpleScriptableObjectScript;
                    _generateMode = GenerateMode.ScriptableObject;
                }),
                ("EditorWindow", () =>
                {
                    _generateCodeFunc = GetSimpleEditorWindowScript;
                    _generateMode = GenerateMode.EditorWindow;
                })
            });
        }

        _showOthers = EditorGUILayout.Foldout(_showOthers, "Others", true);
        if (_showOthers)
        {
            DrawButtonGroup(new[]
            {
                ("Struct", (Action)(() =>
                {
                    _generateCodeFunc = GetStructCode;
                    _generateMode = GenerateMode.Struct;
                })),
                ("Enum", () =>
                {
                    _generateCodeFunc = GetEnumCode;
                    _generateMode = GenerateMode.Enum;
                }),
                ("Interface", () =>
                {
                    _generateCodeFunc = GetInterfaceCode;
                    _generateMode = GenerateMode.Interface;
                })
            });
        }
    }

    private void DrawButtonGroup((string label, Action action)[] buttons)
    {
        int columnCount = 2;
        for (int i = 0; i < buttons.Length; i += columnCount)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < columnCount; j++)
            {
                if (i + j < buttons.Length)
                {
                    if (GUILayout.Button(buttons[i + j].label, GUILayout.Height(25)))
                    {
                        buttons[i + j].action();
                        _isPushButton = true;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    #region Options

    private void IsSerializable()
    {
        if (_generateMode == GenerateMode.SimpleCs || _generateMode == GenerateMode.Struct)
        {
            _isSerializable = EditorGUILayout.Toggle("Is Serializable", _isSerializable);
        }
    }

    private string GetOtherModifier()
    {
        if (_otherModifier == OtherModifier.None)
        {
            return "";
        }
        else
        {
            return _otherModifier.ToString().ToLower();
        }
    }

    #endregion

    #region GenerateCode

    private void GenerateCode(string code)
    {
        string selectedPath =
            EditorUtility.OpenFolderPanel("Select Folder", ProjectWindowSelector.GetSelectedFolderPath(), _codeName);
        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(Application.dataPath))
            {
                var folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                var p = GenerateCsFile(folderPath, code);
                ProjectWindowSelector.SelectAsset(p);
            }
            else
            {
                Debug.LogWarning("Assetsフォルダ内を選択してください");
            }
        }
    }

    /// <summary>
    /// CSファイルを生成する。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="code"></param>
    private string GenerateCsFile(string path, string code)
    {
        var generatedPath = Path.Combine(path, _codeName + ".cs");
        File.WriteAllText(generatedPath, code);
        AssetDatabase.Refresh();

        Debug.Log("EditorWindowスクリプトを生成しました: " + path);
        return generatedPath;
    }

    #endregion

    #region GetCode

    private string GetSimpleCsCode(string className)
    {
        string code = _isSerializable ? "using System;\n[Serializable]\n" : "";
        code = _useSummary
            ? code + @"
/// <summary>
/// 
/// </summary>
"
            : code;

        code += $@"{_accessModifier.ToString().ToLower()} {GetOtherModifier()} class {className}
{{
    
}}";
        return code;
    }

    private string GetSimpleMonoBehaviourScript(string className)
    {
        string code = "using UnityEngine";

        code = _useSummary
            ? code + @"
/// <summary>
/// 
/// </summary>
"
            : code;

        code += $@";

{_accessModifier.ToString().ToLower()} {GetOtherModifier()}  class {className} : MonoBehaviour
{{
    
}}

";
        return code;
    }

    private string GetSimpleScriptableObjectScript(string className)
    {
        string code = "using UnityEngine;";
        code += _useSummary
            ? @"
/// <summary>
/// 
/// </summary>
"
            : "";

        code += $@"
[CreateAssetMenu(fileName = ""{className}"", menuName = ""ScriptableObjects/{className}"")]
{_accessModifier.ToString().ToLower()} {GetOtherModifier()}  class {className} : ScriptableObject
{{
    
}}
";
        return code;
    }

    private string GetSimpleEditorWindowScript(string className)
    {
        string code = "using UnityEngine;\nusing UnityEditor;";

        code += _useSummary
            ? @"
/// <summary>
/// 
/// </summary>
"
            : "";

        code += $@"
{_accessModifier.ToString().ToLower()} {GetOtherModifier()}  class {className} : EditorWindow
{{
    [MenuItem(""Window/UsefulTools/{className}"")]
    {_accessModifier.ToString().ToLower()} static void ShowWindow()
    {{
        GetWindow<{className}>(""{className}"");
    }}

    private void OnGUI()
    {{

    }}
}}";

        return code;
    }

    private string GetStructCode(string structName)
    {
        string code = _isSerializable ? "using System;\n[Serializable]\n" : "";
        code += $@"{_accessModifier.ToString().ToLower()}  struct {structName}
{{
    
}}";

        return code;
    }

    private string GetEnumCode(string enumName)
    {
        return $@"{_accessModifier.ToString().ToLower()} enum {enumName} 
{{
    
}}";
    }

    private string GetInterfaceCode(string interfaceName)
    {
        return $@"{_accessModifier.ToString().ToLower()}  interface {interfaceName}
{{
    
}}";
    }

    #endregion

    #region SceneBoot Logic

    private void GenerateSceneBoot()
    {
        if (string.IsNullOrEmpty(_codeName))
        {
            Debug.LogError("Code Name (Scene Name) is empty.");
            return;
        }

        // Find all concrete InitializableMonoBehaviour in the active scene
        var initializables = FindObjectsOfType<InitializableMonoBehaviour>()
            .Where(obj => !obj.GetType().IsAbstract)
            .OrderBy(obj => obj.InitializationOrder)
            .ToList();

        string bootCode = GetSceneBootCode(_codeName, initializables);
        string containerCode = GetSceneContainerCode(_codeName);

        // Ensure directory exists
        if (!Directory.Exists(_sceneGenerationPath))
        {
            Directory.CreateDirectory(_sceneGenerationPath);
        }

        // Generate Boot file (Overwrite)
        string bootPath = Path.Combine(_sceneGenerationPath, _codeName + "Boot.cs");
        File.WriteAllText(bootPath, bootCode);

        // Generate Container file (Do NOT overwrite if exists)
        string containerPath = Path.Combine(_sceneGenerationPath, _codeName + "Container.cs");
        if (!File.Exists(containerPath))
        {
            File.WriteAllText(containerPath, containerCode);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Generated Scene Boot at {bootPath}");
    }

    private string GetSceneBootCode(string sceneName, List<InitializableMonoBehaviour> objects)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UsefulTools.UtilityUnity.Runtime.UtilityUnity;");
        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine($"namespace {_sceneNamespace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"    public class {sceneName}Boot : MonoBehaviour");
        sb.AppendLine("    {");
        sb.AppendLine($"        [SerializeField] private {sceneName}Container _container;");
        sb.AppendLine("");

        // Field definitions for each instance
        var instanceToVarName = new Dictionary<InitializableMonoBehaviour, string>();
        var typeCount = new Dictionary<Type, int>();

        foreach (var obj in objects)
        {
            var type = obj.GetType();
            if (!typeCount.ContainsKey(type)) typeCount[type] = 0;
            
            string baseVarName = char.ToLower(type.Name[0]) + type.Name.Substring(1);
            string varName = typeCount[type] == 0 ? baseVarName : $"{baseVarName}{typeCount[type]}";
            instanceToVarName[obj] = varName;
            typeCount[type]++;

            sb.AppendLine($"        [SerializeField] private {type.Name} _{varName};");
        }

        sb.AppendLine("");
        sb.AppendLine("        private void Start()");
        sb.AppendLine("        {");
        sb.AppendLine("            Inject();");
        sb.AppendLine("            Initialize();");
        sb.AppendLine("        }");
        sb.AppendLine("");
        sb.AppendLine("        private void Inject()");
        sb.AppendLine("        {");

        foreach (var obj in objects)
        {
            var type = obj.GetType();
            var varName = instanceToVarName[obj];

            if (IsInjectable(type, out var injectInterface))
            {
                var args = injectInterface.GetGenericArguments();
                var conditions = new List<string>();
                
                // Instance itself is already a field, so we just check if it's assigned
                conditions.Add($"_{varName} != null");

                var argNames = new List<string>();
                for (int i = 0; i < args.Length; i++)
                {
                    var argVarName = $"arg{varName}_{i}";
                    conditions.Add($"_container.Get<{args[i].Name}>(out var {argVarName})");
                    argNames.Add(argVarName);
                }

                sb.AppendLine($"            if ({string.Join(" && ", conditions)})");
                sb.AppendLine("            {");
                sb.AppendLine($"                _{varName}.Inject({string.Join(", ", argNames)});");
                sb.AppendLine("            }");
            }
        }

        sb.AppendLine("        }");
        sb.AppendLine("");
        sb.AppendLine("        private void Initialize()");
        sb.AppendLine("        {");

        foreach (var obj in objects)
        {
            var varName = instanceToVarName[obj];
            sb.AppendLine($"            if (_{varName} != null) _{varName}.Initialize();");
        }

        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private string GetSceneContainerCode(string sceneName)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine($"namespace {_sceneNamespace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"    public class {sceneName}Container : MonoBehaviour");
        sb.AppendLine("    {");
        sb.AppendLine("        public bool Get<T>(out T result)");
        sb.AppendLine("        {");
        sb.AppendLine("            result = default;");
        sb.AppendLine("            return false;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private bool IsInjectable(Type type, out Type injectInterface)
    {
        injectInterface = type.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType && (
                i.GetGenericTypeDefinition() == typeof(IInjectable<>) ||
                i.GetGenericTypeDefinition() == typeof(IInjectable<,>) ||
                i.GetGenericTypeDefinition() == typeof(IInjectable<,,>) ||
                i.GetGenericTypeDefinition() == typeof(IInjectable<,,,>)
            ));
        return injectInterface != null;
    }

    #endregion

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        StringBuilder sb = new StringBuilder();

        sb.Append(char.ToUpper(input[0]));
        bool beforeSeparator = false;

        for (int i = 1; i < input.Length; i++)
        {
            var separator = IsSeparator(input[i]);
            if (separator)
            {
                beforeSeparator = true;
                continue;
            }

            sb.Append(beforeSeparator ? char.ToUpper(input[i]) : input[i]);

            beforeSeparator = false;
        }

        return sb.ToString();
    }

    private bool IsSeparator(char c)
    {
        return c == ' ' ||
               c == '_' ||
               c == '-' ||
               c == '/' ||
               c == '\n' ||
               c == '\r';
    }

    private enum GenerateMode
    {
        SimpleCs,
        MonoBehaviour,
        ScriptableObject,
        EditorWindow,
        Struct,
        Enum,
        Interface,
        SceneBoot
    }

    private enum AccessModifier
    {
        Public,
        Protected,
        Internal,
        Private,
    }

    private enum OtherModifier
    {
        None,
        Abstract,
        Sealed,
        Static,
    }
}