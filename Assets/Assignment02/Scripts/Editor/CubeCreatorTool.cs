using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEditor;
using UnityEngine;

namespace Core.Assignment02
{
    [EditorTool("Cube Creator Tool", typeof(CubeCreator))]
    class CubeCreatorTool : EditorTool, IDrawSelectedHandles
    {
        static CubeCreatorTool platformTool;

        [Shortcut("Activate Cube Creator Tool", typeof(SceneView), KeyCode.P)]
        static void OpenCubeCreatorToolShortcut()
        {
            if (Selection.GetFiltered<CubeCreator>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<CubeCreatorTool>();
                return;
            }

            CubeCreator cubeCreatorGameObject = FindObjectOfType<CubeCreator>();

            if (cubeCreatorGameObject == null)
            {
                SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Add a Cube Creator component in scene first"), .1f);
                return;
            }

            Selection.activeGameObject = cubeCreatorGameObject.gameObject;

            Selection.selectionChanged += ToolManager.SetActiveTool<CubeCreatorTool>;
            Selection.selectionChanged += () => Selection.selectionChanged -= ToolManager.SetActiveTool<CubeCreatorTool>;
        }

        [Shortcut("Create Cube Tool", typeof(SceneView), KeyCode.Y)]
        static void CreateCubeToolShortcut()
        {
            if (platformTool == null)
            {
                SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Activate Cube Creator tool first"), .1f);
                return;
            }

            // can handle multiple CubeCreators
            foreach (var obj in platformTool.targets)
            {
                if (obj is not CubeCreator platform)
                {
                    continue;
                }
                Quaternion rotation = platform.Rotation; // get cube rotation
                Vector3 start = platform.Start; // get cube start position
                Vector3 end = start + rotation * (platform.End - start); // get rotated cube end position

                GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube); // create new cube
                newCube.name = "Cube";
                // apply cube settings to cube
                newCube.transform.position = (start + end) * 0.5f;
                newCube.transform.localScale = platformTool.Abs(platform.End - start);
                newCube.transform.rotation = rotation;
            }
        }

        public override void OnActivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Cube Creator Tool"), .1f);
            platformTool = this;
        }

        public override void OnWillBeDeactivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting CubeH Creator Tool"), .1f);
            platformTool = null;
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (window is not SceneView)
                return;

            foreach (var obj in targets)
            {
                if (obj is not CubeCreator platform)
                    continue;

                EditorGUI.BeginChangeCheck();

                Quaternion rotation = platform.Rotation; // get rotation of current cube to create
                Vector3 start = platform.Start; // get start position of current cube to create
                Handles.TransformHandle(ref start, ref rotation); // create handle at start position with rotation

                // rotate end position in order to make it follows the cube rotation
                Vector3 end = platform.Start + rotation * (platform.End - platform.Start);
                end = Handles.PositionHandle(end, rotation); // create handle at rotated end position with rotation

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(platform, "Set Platform Destinations");

                    rotation.Normalize(); // normalize rotation
                                          // assigning new values to cube creator
                    platform.Rotation = rotation;
                    platform.Start = start;
                    // set the end position as it was not rotated
                    platform.End = Quaternion.Inverse(rotation) * (end - start + rotation * start);
                }
            }
        }

        public void OnDrawHandles()
        {
            foreach (var obj in targets)
            {
                if (obj is not CubeCreator platform)
                {
                    continue;
                }
                Quaternion rotation = platform.Rotation;
                Vector3 start = platform.Start;
                Vector3 rotatedEnd = start + rotation * (platform.End - start);

                Matrix4x4 currentMatrix = Handles.matrix;
                Handles.matrix = Matrix4x4.TRS((start + rotatedEnd) * 0.5f, rotation, Abs(platform.End - start));
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
                Handles.matrix = currentMatrix;
            }
        }

        public Vector3 Abs(Vector3 a)
        {
            a.x = Mathf.Abs(a.x);
            a.y = Mathf.Abs(a.y);
            a.z = Mathf.Abs(a.z);
            return a;
        }
    }
}