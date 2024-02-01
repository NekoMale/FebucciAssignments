using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;

namespace Core.Assignment03
{
    public class BoardEditor : EditorWindow
    {
        VisualElement root;
        BoardView boardView;

        [MenuItem("Assignments 3/Board Editor")]
        public static void OpenBoardEditor()
        {
            GetWindow<BoardEditor>();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is Board)
            {
                OpenBoardEditor();
                return true;
            }
            return false;

        }

        private void CreateGUI()
        {
            root = rootVisualElement;

            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment03/Styles/BoardView.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(root);

            boardView = root.Q<BoardView>();

            root.Q<Button>("export-button").clicked += ExportBoardToJson;

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is not Board board)
            {
                return;
            }

            // bind title label with editing board name
            Label titleLabel = root.Q<Label>("title-label");
            titleLabel.bindingPath = "m_Name";
            titleLabel.Bind(new SerializedObject(board));

            boardView.CreateBoardView(board);
        }

        private void ExportBoardToJson()
        {
            Board board = boardView.activeBoard;

            // Get datas to export in json file from board
            Dictionary<string, object> toJsonfy = board.ToDictionary();

            string json = DictionaryToJson(toJsonfy);

            Debug.Log(board.name + " board exported at path:\n" + Application.persistentDataPath);
            File.WriteAllText(Application.persistentDataPath + "/" + board.name + ".json", json);
        }

        private void OnDisable()
        {
            boardView.Disable();
        }

        /// <summary>
        /// Transform a dictionary in a json string
        /// </summary>
        /// <param name="toJsonfy">Dictionary to transform</param>
        /// <returns>string with json datas</returns>
        private string DictionaryToJson(Dictionary<string, object> toJsonfy)
        {
            string json = "{";
            foreach (KeyValuePair<string, object> kvp in toJsonfy)
            {
                json += "\"" + kvp.Key + "\": " + ToJson(kvp.Value);
            }
            json = json[..^2] + "}";
            return json;
        }

        /// <summary>
        /// Transform a IEnumerable in a json string
        /// </summary>
        /// <param name="toJsonfy">System.Collections.IEnumerable to transform</param>
        /// <returns>string with json datas</returns>
        private string IEnumerableToJson(System.Collections.IEnumerable toJsonfy)
        {
            string json = "[";
            foreach (object kvp in toJsonfy)
            {
                json += ToJson(kvp);
            }
            json = json.Substring(0, json.Length - 2) + "]";
            return json;
        }

        /// <summary>
        /// Transform an object in a json string checking which type object is
        /// </summary>
        /// <param name="kvp">object to transform in json</param>
        /// <returns>string with json datas</returns>
        private string ToJson(object kvp)
        {
            string json = "";

            if (kvp is null)
            {
                json += "\"\", ";
            }
            else if (kvp is Dictionary<string, object> nestedDictionary)
            {
                json += DictionaryToJson(nestedDictionary) + ", ";
            }
            else if (kvp is not string && kvp is System.Collections.IEnumerable collection)
            {
                json += IEnumerableToJson(collection) + ", ";
            }
            else
            {
                json += "\"" + kvp + "\", ";
            }

            return json;
        }
    }
}