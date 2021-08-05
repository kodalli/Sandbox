using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {

    public class ReanimatorGraphEditorWindow : EditorWindow {
        [MenuItem("Reanimator/Resolution Graph")]
        public static void ShowWindow()
        {
            ReanimatorGraphEditorWindow wnd = GetWindow<ReanimatorGraphEditorWindow>();
            wnd.titleContent = new GUIContent("ReanimatorGraph");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (!(Selection.activeObject is ResolutionGraph)) return false;
            ShowWindow();
            return true;
        }
        private const string visualTreePath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uxml";
        private const string styleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uss";

        private ResolutionGraph resolutionGraph;
        
        private ReanimatorGraphView editorGraph;
        private ToolbarMenu toolbarMenu;
        private InspectorCustomControl inspectorPanel;
        private VisualElement animationPreviewPanel;
        private TwoPanelInspector twoPanelInspector;
        private ToolbarButton saveButton;
        private ToolbarButton loadButton;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(styleSheet);

            editorGraph = root.Q<ReanimatorGraphView>();
            
            inspectorPanel = root.Q<InspectorCustomControl>();
            animationPreviewPanel = root.Q<VisualElement>("animation-preview");
            twoPanelInspector = root.Q<TwoPanelInspector>("TwoPanelInspector");
            
            toolbarMenu = root.Q<ToolbarMenu>();
            saveButton = root.Q<ToolbarButton>("save-button");
            loadButton = root.Q<ToolbarButton>("load-button");
            
            var resolutionGraphs = Helpers.LoadAssetsOfType<ResolutionGraph>();
            resolutionGraphs.ForEach(graph => {
                toolbarMenu.menu.AppendAction($"{graph.name}", (a) => {
                    Selection.activeObject = graph;
                    resolutionGraph = Selection.activeObject as ResolutionGraph;
                    editorGraph.Initialize(this, resolutionGraph);
                    EditorGUIUtility.PingObject(graph);
                    EditorApplication.delayCall += () => { editorGraph.FrameAll(); };
                });
            });

            saveButton.clicked += () => {
                resolutionGraph.saveData = Helpers.SaveService(editorGraph);
            };
            loadButton.clicked += () => {
                Helpers.SaveService(editorGraph).LoadFromSubAssets();
            };
            
            editorGraph.onNodeSelected = DrawNodeProperties;
        }

        private void DrawNodeProperties(ReanimatorGraphNode graphNode)
        {
            twoPanelInspector.Initialize(inspectorPanel, animationPreviewPanel);
            twoPanelInspector.DrawNodeProperties(graphNode);
        }

        private void OnSelectionChange()
        {
            //EditorApplication.delayCall += () => {
            //    ResolutionGraph graph = Selection.activeObject as ResolutionGraph;
            //    SelectTree(graph);
            //};
        }

        private void OnDisable()
        {
            //resolutionGraph.saveData = Helpers.SaveService(editorGraph);
        }
        
    }
}