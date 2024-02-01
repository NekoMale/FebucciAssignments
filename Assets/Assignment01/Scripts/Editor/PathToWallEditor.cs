using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Assignment01
{
    [CustomEditor(typeof(PathToWall))]
    class PathToWallEditor : Editor
    {
        bool areExtrasShown = false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Hide "path" field insied foldout
            areExtrasShown = EditorGUILayout.Foldout(areExtrasShown, "Extras", true, EditorStyles.foldoutHeader);
            if(areExtrasShown)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("path"));
                EditorGUI.indentLevel--;
            }
            

            // Has PathToWall to use custom height?
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomHeight"));

            // if so, draw customHeight field
            if (serializedObject.FindProperty("useCustomHeight").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("customHeight"));
            }

            // if generate new path is pressed
            if (GUILayout.Button("Generate New Path"))
            {
                // call private method "Generate new path"
                target.GetType().GetMethod("GenerateNewPath", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
            }

            GUI.enabled = serializedObject.FindProperty("path").arraySize > 0; // Enable GUI only if there is a path to build

            // if generate wall mesh is pressed
            if (GUILayout.Button("Generate Wall Mesh"))
            {
                // call private method "Generate Wall Mesh"
                target.GetType().GetMethod("GenerateWallMesh", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
            }

            // if generate double-face wall mesh is pressed
            if (GUILayout.Button("Generate Double-Face Wall Mesh"))
            {
                // call private method "Generate Wall Mesh Double Face"
                target.GetType().GetMethod("GenerateWallMeshDoubleFace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
            }

            GUI.enabled = true; // Re-enable GUI

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
