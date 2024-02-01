using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Assignment03
{
    [CreateAssetMenu]
    public class Board : ScriptableObject
    {
        [SerializeField] public Node Root;
        [SerializeField] public List<Node> Nodes;

        /// <summary>
        /// Create a new node asset and add it to its node list Nodes.<br/>
        /// The new node asset will be added to board asset
        /// </summary>
        /// <param name="nodeType">The node type to create</param>
        /// <returns>The created node</returns>
        public Node CreateNode(Type nodeType)
        {
            Undo.IncrementCurrentGroup();

            Node newNode = CreateInstance(nodeType) as Node;
            newNode.Guid = GUID.Generate().ToString();
            newNode.name = nodeType.Name + " (" + newNode.Guid.Substring(0, 5) + ")";
            
            Undo.RecordObject(this, "");

            if (Root == null)
            {
                Root = newNode;
            }
            Nodes.Add(newNode);

            Undo.RegisterCreatedObjectUndo(newNode, "bp");

            AssetDatabase.AddObjectToAsset(newNode, this);
            AssetDatabase.SaveAssets();

            Undo.SetCurrentGroupName("Board (" + name + " - New Node)");

            return newNode;
        }

        /// <summary>
        /// Delete passed node from its node list Nodes and from Root if it is.<br/>
        /// Deleted node will be deleted from asset too.
        /// </summary>
        /// <param name="deletingNode">The node to delete</param>
        public void DeleteNode(Node deletingNode)
        {
            Undo.IncrementCurrentGroup();

            Undo.RecordObject(this, "");
            Nodes.Remove(deletingNode);

            if (deletingNode == Root)
            {
                if (Nodes.Count > 0)
                {
                    Root = Nodes[0];
                }
                else
                {
                    Root = null;
                }
            }

            Undo.DestroyObjectImmediate(deletingNode);
            AssetDatabase.SaveAssets();

            Undo.SetCurrentGroupName("Board (" + name + " - Delete Node)");
        }

        /// <summary>
        /// Makes a node child of another node at childIndex position
        /// </summary>
        /// <param name="fromNode">Parent node</param>
        /// <param name="toNode">Child node</param>
        /// <param name="childIndex">Index where child has to be add</param>
        public void CreateLink(Node fromNode, Node toNode, int childIndex)
        {
            Undo.RecordObject(fromNode, "Adding child to " + fromNode.name);
            
            fromNode.AddChild(toNode, childIndex);

            EditorUtility.SetDirty(fromNode);
        }

        /// <summary>
        /// Remove a child node from parent node
        /// </summary>
        /// <param name="fromNode">Parent node where remove child</param>
        /// <param name="toNode">Child node to remove</param>
        public void DeleteLink(Node fromNode, Node toNode)
        {
            Undo.RecordObject(fromNode, "Removing child from " + fromNode.name);

            fromNode.RemoveChild(toNode);

            EditorUtility.SetDirty(fromNode);
        }

        /// <summary>
        /// Transform main datas in a dictionary containing string as keys and object as values<br/>
        /// string keys contain data names.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> datas = new Dictionary<string, object>();
            datas["board-name"] = name;
            datas["board-root"] = Root.Guid;

            Dictionary<string, object> nodes = new Dictionary<string, object>();
            foreach(Node node in Nodes)
            {
                Dictionary<string, object> nodeDatas = new Dictionary<string, object>();
                nodeDatas[node.Guid + "-name"] = node.name;
                nodeDatas[node.Guid + "-description"] = node.Description;
                string[] childrenGuids = new string[node.GetChildren().Length];
                int childIndex = -1;
                foreach(Node child in node.GetChildren())
                {
                    childIndex++;
                    if(child == null) continue;
                    childrenGuids[childIndex] = child.Guid;
                }
                nodeDatas[node.Guid +"-children"] = childrenGuids;

                nodes[node.Guid] = nodeDatas;
            }
            datas["board-nodes"] = nodes;

            return datas;
        }
    }
}