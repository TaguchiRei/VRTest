using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
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

    private bool _showSimpleClass;
    private bool _showOthers;
    private bool _showOptions;

    private bool _isPushButton;

    private bool _isSerializable;
    private bool _useSummary;
    private AccessModifier _accessModifier;
    private OtherModifier _otherModifier;

    private string _sceneNamespace = "UsefulTools.Composition.Runtime.Boot";
    private string _sceneGenerationPath = "Assets/Code/Scripts/Composition/Boot";
    private bool _showSceneBoot = true;
    private bool _showClassTemplates = false;

    private sealed class InitializerInfo
    {
        public InitializerBase Instance;
        public Type Type;
        public string VariableName;
        public List<Type> InjectableInterfaces = new();
    }

    [MenuItem("UsefulTools/Code Generator")]
    public static void ShowWindow()
    {
        GetWindow<CodeGenerator>("Code Generator");
    }

    private void OnEnable()
    {
        InitDefaultNamespace();

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

    private void InitDefaultNamespace()
    {
        if (string.IsNullOrEmpty(_sceneNamespace) ||
            _sceneNamespace == "UsefulTools.Composition.Runtime.Boot")
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

    public void OnGUI()
    {
        InitStyles();

        EditorGUILayout.LabelField("Useful Tools: Code Generator", _headerStyle);
        EditorGUILayout.Space();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        DrawSceneBootSection();

        EditorGUILayout.Space();

        DrawClassTemplatesSection();

        GUILayout.EndScrollView();
    }

    private void DrawSceneBootSection()
    {
        EditorGUILayout.BeginVertical(_sectionStyle);

        _showSceneBoot = EditorGUILayout.Foldout(
            _showSceneBoot,
            new GUIContent(" Scene Boot", EditorGUIUtility.IconContent("SceneAsset Icon").image),
            true);

        if (_showSceneBoot)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            _sceneNamespace =
                EditorGUILayout.TextField("Namespace", _sceneNamespace);

            _sceneGenerationPath =
                EditorGUILayout.TextField("Generation Path", _sceneGenerationPath);

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
    }

    private void DrawClassTemplatesSection()
    {
        EditorGUILayout.BeginVertical(_sectionStyle);

        _showClassTemplates = EditorGUILayout.Foldout(
            _showClassTemplates,
            new GUIContent(" Class Templates", EditorGUIUtility.IconContent("cs Script Icon").image),
            true);

        if (_showClassTemplates)
        {
            EditorGUI.indentLevel++;

            DrawNameField();

            EditorGUILayout.Space();

            DrawOptions();

            EditorGUILayout.Space();

            DrawCodePreview();

            EditorGUILayout.Space();

            DrawTemplateButtons();

            EditorGUILayout.Space();

            DrawGenerateButton();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawNameField()
    {
        EditorGUILayout.LabelField("Class Name", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        _codeName = EditorGUILayout.TextField(_codeName);

        if (EditorGUI.EndChangeCheck())
        {
            _isPushButton = true;
            _codeName = ToPascalCase(_codeName);
        }
    }

    private void DrawOptions()
    {
        _showOptions = EditorGUILayout.Foldout(_showOptions, "Options", true);

        if (_showOptions)
        {
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            _useSummary = EditorGUILayout.Toggle("Use Summary", _useSummary);

            IsSerializable();

            _accessModifier =
                (AccessModifier)EditorGUILayout.EnumPopup(
                    "Access Modifier",
                    _accessModifier);

            _otherModifier =
                (OtherModifier)EditorGUILayout.EnumPopup(
                    "Other Modifier",
                    _otherModifier);

            if (EditorGUI.EndChangeCheck())
            {
                _isPushButton = true;
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawCodePreview()
    {
        EditorGUILayout.LabelField("Code Preview", EditorStyles.boldLabel);

        if (_isPushButton)
        {
            _code = _generateCodeFunc(_codeName);
            _isPushButton = false;
        }

        _code = EditorGUILayout.TextArea(_code, GUILayout.Height(200));
    }

    private void DrawGenerateButton()
    {
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
    }

    private void DrawTemplateButtons()
    {
        EditorGUILayout.LabelField("Template Selection", EditorStyles.boldLabel);

        _showSimpleClass =
            EditorGUILayout.Foldout(_showSimpleClass, "Simple Templates", true);

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
                }),
                ("Container", () =>
                {
                    _generateCodeFunc = GetContainerCode;
                    _generateMode = GenerateMode.Container;
                })
            });
        }
    }

    private void DrawButtonGroup((string label, Action action)[] buttons)
    {
        const int columnCount = 2;

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

    private void IsSerializable()
    {
        if (_generateMode == GenerateMode.SimpleCs ||
            _generateMode == GenerateMode.Struct)
        {
            _isSerializable =
                EditorGUILayout.Toggle("Is Serializable", _isSerializable);
        }
    }

    private string GetOtherModifier()
    {
        return _otherModifier == OtherModifier.None
            ? ""
            : _otherModifier.ToString().ToLower();
    }

    private void GenerateCode(string code)
    {
        string selectedPath =
            EditorUtility.OpenFolderPanel(
                "Select Folder",
                ProjectWindowSelector.GetSelectedFolderPath(),
                _codeName);

        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(Application.dataPath))
            {
                var folderPath =
                    "Assets" + selectedPath.Substring(Application.dataPath.Length);

                var path = GenerateCsFile(folderPath, code);

                ProjectWindowSelector.SelectAsset(path);
            }
            else
            {
                Debug.LogWarning("Assetsフォルダ内を選択してください");
            }
        }
    }

    private string GenerateCsFile(string path, string code)
    {
        var generatedPath = Path.Combine(path, _codeName + ".cs");

        File.WriteAllText(generatedPath, code);

        AssetDatabase.Refresh();

        return generatedPath;
    }

    private void GenerateSceneBoot()
    {
        if (EditorApplication.isCompiling)
        {
            Debug.LogWarning("Scripts are compiling. Please wait.");
            return;
        }

        AssetDatabase.Refresh();

        _codeName = SceneManager.GetActiveScene().name;

        if (string.IsNullOrEmpty(_codeName))
        {
            Debug.LogError("Scene name is empty.");
            return;
        }

        var initializers = FindObjectsByType<InitializerBase>(
                FindObjectsSortMode.None)
            .Where(x => x != null)
            .OrderBy(x => x.InitializationOrder)
            .ToList();

        var initializerInfos = BuildInitializerInfos(initializers);

        string bootCode =
            GetSceneBootCode(_codeName, initializerInfos);

        string containerCode =
            GetSceneContainerCode(_codeName);

        if (!Directory.Exists(_sceneGenerationPath))
        {
            Directory.CreateDirectory(_sceneGenerationPath);
        }

        string bootPath =
            Path.Combine(_sceneGenerationPath, $"{_codeName}Boot.cs");

        string containerPath =
            Path.Combine(_sceneGenerationPath, $"{_codeName}Container.cs");

        File.WriteAllText(bootPath, bootCode);
        File.WriteAllText(containerPath, containerCode);

        AssetDatabase.Refresh();

        Debug.Log($"Generated Scene Boot at {bootPath}");
    }

    private List<InitializerInfo> BuildInitializerInfos(
        List<InitializerBase> objects)
    {
        var result = new List<InitializerInfo>();

        var typeCount = new Dictionary<Type, int>();

        foreach (var obj in objects)
        {
            MonoScript monoScript = MonoScript.FromMonoBehaviour(obj);

            if (monoScript == null)
            {
                continue;
            }

            Type latestType = monoScript.GetClass();

            if (latestType == null)
            {
                continue;
            }

            if (latestType.IsAbstract)
            {
                continue;
            }

            if (!typeCount.ContainsKey(latestType))
            {
                typeCount[latestType] = 0;
            }

            string baseVarName =
                char.ToLower(latestType.Name[0]) +
                latestType.Name.Substring(1);

            string varName =
                typeCount[latestType] == 0
                    ? baseVarName
                    : $"{baseVarName}{typeCount[latestType]}";

            typeCount[latestType]++;

            var info = new InitializerInfo
            {
                Instance = obj,
                Type = latestType,
                VariableName = varName,
                InjectableInterfaces = GetInjectInterfaces(latestType)
            };

            result.Add(info);
        }

        return result;
    }

    private List<Type> GetInjectInterfaces(Type type)
    {
        return type.GetInterfaces()
            .Where(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(IInjectable<>) ||
                    i.GetGenericTypeDefinition() == typeof(IInjectable<,>) ||
                    i.GetGenericTypeDefinition() == typeof(IInjectable<,,>) ||
                    i.GetGenericTypeDefinition() == typeof(IInjectable<,,,>)
                ))
            .ToList();
    }

    private string GetSceneBootCode(
        string sceneName,
        List<InitializerInfo> objects)
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

        sb.AppendLine(
            $"        [SerializeField] private {sceneName}Container _container;");

        sb.AppendLine();

        foreach (var info in objects)
        {
            sb.AppendLine(
                $"        [SerializeField] private {info.Type.Name} _{info.VariableName};");
        }

        sb.AppendLine();

        sb.AppendLine("        private void Start()");
        sb.AppendLine("        {");
        sb.AppendLine("            Inject();");
        sb.AppendLine("            Initialize();");
        sb.AppendLine("        }");

        sb.AppendLine();

        sb.AppendLine("        private void Inject()");
        sb.AppendLine("        {");

        foreach (var info in objects)
        {
            foreach (var injectInterface in info.InjectableInterfaces)
            {
                GenerateInjectCode(sb, info, injectInterface);
            }
        }

        sb.AppendLine("        }");

        sb.AppendLine();

        sb.AppendLine("        private void Initialize()");
        sb.AppendLine("        {");

        foreach (var info in objects)
        {
            sb.AppendLine(
                $"            if (_{info.VariableName} != null) _{info.VariableName}.Initialize();");
        }

        sb.AppendLine("        }");

        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private void GenerateInjectCode(
        StringBuilder sb,
        InitializerInfo info,
        Type injectInterface)
    {
        var args = injectInterface.GetGenericArguments();

        var conditions = new List<string>
        {
            $"_{info.VariableName} != null"
        };

        var argNames = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            string argVarName =
                $"arg_{info.VariableName}_{i}";

            conditions.Add(
                $"_container.TryGet<{args[i].Name}>(out var {argVarName})");

            argNames.Add(argVarName);
        }

        sb.AppendLine(
            $"            if ({string.Join(" && ", conditions)})");

        sb.AppendLine("            {");

        sb.AppendLine(
            $"                _{info.VariableName}.Inject({string.Join(", ", argNames)});");

        sb.AppendLine("            }");
    }

    private string GetSceneContainerCode(string sceneName)
    {
        string className = sceneName + "Container";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");

        sb.AppendLine();

        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine($"namespace {_sceneNamespace}");
            sb.AppendLine("{");
        }

        string indent = !string.IsNullOrEmpty(_sceneNamespace)
            ? "    "
            : "";

        sb.AppendLine($"{indent}[DefaultExecutionOrder(-1000)]");
        sb.AppendLine($"{indent}public sealed class {className} : MonoBehaviour");
        sb.AppendLine($"{indent}{{");

        sb.AppendLine($"{indent}    private static {className} _instance;");
        sb.AppendLine();

        sb.AppendLine(
            $"{indent}    private readonly Dictionary<Type, object> _container = new();");

        sb.AppendLine();

        sb.AppendLine($"{indent}    public static void Register<T>(T instance)");
        sb.AppendLine($"{indent}    {{");

        sb.AppendLine($"{indent}        var type = typeof(T);");
        sb.AppendLine();

        sb.AppendLine(
            $"{indent}        if (_instance._container.ContainsKey(type))");

        sb.AppendLine($"{indent}        {{");

        sb.AppendLine(
            $"{indent}            Debug.LogWarning($\"{{type.Name}} already registered.\");");

        sb.AppendLine($"{indent}            return;");

        sb.AppendLine($"{indent}        }}");

        sb.AppendLine();

        sb.AppendLine(
            $"{indent}        _instance._container.Add(type, instance);");

        sb.AppendLine($"{indent}    }}");

        sb.AppendLine();

        sb.AppendLine($"{indent}    public bool TryGet<T>(out T result)");
        sb.AppendLine($"{indent}    {{");

        sb.AppendLine(
            $"{indent}        if (_container.TryGetValue(typeof(T), out var value))");

        sb.AppendLine($"{indent}        {{");

        sb.AppendLine($"{indent}            result = (T)value;");
        sb.AppendLine($"{indent}            return true;");

        sb.AppendLine($"{indent}        }}");

        sb.AppendLine();

        sb.AppendLine($"{indent}        result = default;");
        sb.AppendLine($"{indent}        return false;");

        sb.AppendLine($"{indent}    }}");

        sb.AppendLine();

        sb.AppendLine($"{indent}    private void Awake()");
        sb.AppendLine($"{indent}    {{");

        sb.AppendLine($"{indent}        _instance = this;");

        sb.AppendLine($"{indent}    }}");

        sb.AppendLine($"{indent}}}");

        if (!string.IsNullOrEmpty(_sceneNamespace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private string GetSimpleCsCode(string className)
    {
        string code =
            _isSerializable
                ? "using System;\n[Serializable]\n"
                : "";

        code = _useSummary
            ? code + @"
/// <summary>
/// 
/// </summary>
"
            : code;

        code +=
            $@"{_accessModifier.ToString().ToLower()} {GetOtherModifier()} class {className}
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

{_accessModifier.ToString().ToLower()} {GetOtherModifier()} class {className} : MonoBehaviour
{{
    
}}";

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
{_accessModifier.ToString().ToLower()} {GetOtherModifier()} class {className} : ScriptableObject
{{
    
}}";

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
{_accessModifier.ToString().ToLower()} {GetOtherModifier()} class {className} : EditorWindow
{{
    [MenuItem(""Window/UsefulTools/{className}"")]
    public static void ShowWindow()
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
        string code =
            _isSerializable
                ? "using System;\n[Serializable]\n"
                : "";

        code +=
            $@"{_accessModifier.ToString().ToLower()} struct {structName}
{{
    
}}";

        return code;
    }

    private string GetEnumCode(string enumName)
    {
        return
            $@"{_accessModifier.ToString().ToLower()} enum {enumName}
{{
    
}}";
    }

    private string GetInterfaceCode(string interfaceName)
    {
        return
            $@"{_accessModifier.ToString().ToLower()} interface {interfaceName}
{{
    
}}";
    }

    private string GetContainerCode(string className)
    {
        return GetSceneContainerCode(className);
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();

        sb.Append(char.ToUpper(input[0]));

        bool beforeSeparator = false;

        for (int i = 1; i < input.Length; i++)
        {
            bool separator = IsSeparator(input[i]);

            if (separator)
            {
                beforeSeparator = true;
                continue;
            }

            sb.Append(
                beforeSeparator
                    ? char.ToUpper(input[i])
                    : input[i]);

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
        SceneBoot,
        Container
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