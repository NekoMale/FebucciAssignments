using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

// Example MonoBehaviour that oscillates a transform position between two points.
public class CuboidCreator : MonoBehaviour
{
    [SerializeField]
    Vector3 m_Start = new Vector3(0f, -1f, 0f);

    [SerializeField]
    Vector3 m_End = new Vector3(1f, 1f, 1f);

    [SerializeField] Quaternion m_Rotation = Quaternion.identity;

    public Vector3 start
    {
        get => m_Start;
        set => m_Start = value;
    }

    public Vector3 end
    {
        get => m_End;
        set => m_End = value;
    }

    public Quaternion rotation
    {
        get => m_Rotation;
        set => m_Rotation = value;
    }
}