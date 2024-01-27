using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEditor;
using UnityEngine;

[EditorTool("Cuboid Creator Tool", typeof(CuboidCreator))]
class CuboidCreatorTool : EditorTool, IDrawSelectedHandles
{
    static CuboidCreatorTool platformTool;

    [Shortcut("Activate Cuboid Creator Tool", typeof(SceneView), KeyCode.P)]
    static void CuboidCreatorToolShortcut()
    {
        if (Selection.GetFiltered<CuboidCreator>(SelectionMode.TopLevel).Length > 0)
            return;

        Selection.selectionChanged += ToolManager.SetActiveTool<CuboidCreatorTool>;
        Selection.selectionChanged += () => Selection.selectionChanged -= ToolManager.SetActiveTool<CuboidCreatorTool>;

        Selection.objects = new Object[] { FindObjectOfType<CuboidCreator>().gameObject };
    }

    [Shortcut("Create Cuboid Tool", typeof(SceneView), KeyCode.Y)]
    static void CreateCuboidToolShortcut()
    {
        if (platformTool == null)
        {
            Debug.Log("Activate Cube Creator tool first");
            return;
        }

        foreach (var obj in platformTool.targets)
        {
            if (obj is CuboidCreator platform)
            {
                Quaternion rotation = platform.rotation;
                Vector3 start = platform.start;
                Vector3 end = start + rotation * (platform.end - start);

                GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newCube.name = "Cuboid";
                newCube.transform.position = (start + end) * 0.5f;
                newCube.transform.localScale = platformTool.Abs(platform.end - start);
                newCube.transform.rotation = rotation;
            }
        }
    }

    public override void OnActivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Cuboid Creator Tool"), .1f);
        platformTool = this;
    }

    public override void OnWillBeDeactivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting Cuboid Creator Tool"), .1f);
        platformTool = null;
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView))
            return;

        foreach (var obj in targets)
        {
            if (!(obj is CuboidCreator platform))
                continue;

            EditorGUI.BeginChangeCheck();

            Quaternion rotation = platform.rotation;
            Vector3 start = platform.start;

            Handles.TransformHandle(ref start, ref rotation);

            Vector3 end = platform.start + rotation * (platform.end - platform.start);
            end = Handles.PositionHandle(end, rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(platform, "Set Platform Destinations");
                rotation.Normalize();
                platform.rotation = rotation;
                platform.start = start;
                platform.end = Quaternion.Inverse(rotation) * (end - start + rotation * start);
            }
        }
    }

    public void OnDrawHandles()
    {
        foreach (var obj in targets)
        {
            if (obj is CuboidCreator platform)
            {
                Quaternion rotation = platform.rotation;
                Vector3 start = platform.start;
                Vector3 end = start + rotation * (platform.end - start);


                Matrix4x4 currentMatrix = Handles.matrix;
                Handles.matrix = Matrix4x4.TRS((start + end) * 0.5f, rotation, Abs(platform.end - start));
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
                Handles.matrix = currentMatrix;
            }
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