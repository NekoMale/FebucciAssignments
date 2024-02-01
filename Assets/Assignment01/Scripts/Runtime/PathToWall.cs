using UnityEngine;

namespace Core.Assignment01
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PathToWall : MonoBehaviour
    {
        // -- path --
        [SerializeField] Vector3[] path = System.Array.Empty<Vector3>();

        // -- height --
        const float DefaultHeight = 4;
        [SerializeField] bool useCustomHeight;
        [SerializeField] float customHeight = 4;
        float GetHeight() => useCustomHeight ? customHeight : DefaultHeight;

        MeshFilter meshFilter;

        public MeshFilter MeshFilter
        {
            get
            {
                if (meshFilter) return meshFilter;

                if (TryGetComponent(out meshFilter))
                    return meshFilter;

                Debug.LogError($"No Mesh Filter Component in GameObject {name}", gameObject);
                return null;
            }
        }

        void Awake()
        {
            GenerateNewPath();
            GenerateWallMesh();
        }

        [ContextMenu("Generate Path")]
        void GenerateNewPath()
        {
            path = new Vector3[Random.Range(5, 15)];
            Vector3 startPos = Random.insideUnitSphere * 2;
            startPos.y = 0;
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = startPos;
                startPos += Random.insideUnitSphere * 2;
                startPos.y = 0;
            }
        }

        [ContextMenu("Generate Wall Mesh")]
        void GenerateWallMesh()
        {
            if (path.Length == 0) // added cause of context menu without any check
            {
                return;
            }

            Vector3 heightVector = Vector3.up * GetHeight();

            Vector3[] vertices = new Vector3[path.Length * 6]; // Every path point has a face, every face has 6 vertices
            int[] triangles = new int[path.Length * 6]; // Every path point has a face, every face has 6 vertex indexes

            for (int pathIndex = 0; pathIndex < path.Length - 1; pathIndex++)
            {
                int meshIndex = pathIndex * 6;

                SetMeshVertices(ref vertices, heightVector, meshIndex, pathIndex);
                SetFrontMeshTriangles(ref triangles, meshIndex);
            }
            
            Mesh mesh = new Mesh() { name = "WallMesh" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            MeshFilter.mesh = mesh;
        }

        [ContextMenu("Generate Wall Mesh Double-Face")]
        void GenerateWallMeshDoubleFace()
        {
            if (path.Length == 0) // added cause of context menu without any check
            {
                return;
            }

            Vector3 heightVector = Vector3.up * GetHeight();

            Vector3[] vertices = new Vector3[path.Length * 12]; // Every path point has 2 faces (front - back), every face has 6 vertices
            int[] triangles = new int[path.Length * 12]; // Every path point has 2 faces (front - back), every face has 6 vertex indexes

            for (int pathIndex = 0; pathIndex < path.Length - 1; pathIndex++)
            {
                // Front face settings
                int meshIndex = pathIndex * 6;

                SetMeshVertices(ref vertices, heightVector, meshIndex, pathIndex);
                SetFrontMeshTriangles(ref triangles, meshIndex);

                // Back face settings
                meshIndex += path.Length * 6;

                SetMeshVertices(ref vertices, heightVector, meshIndex, pathIndex);
                SetBackMeshTriangles(ref triangles, meshIndex);
            }

            Mesh mesh = new Mesh() { name = "DoubleFaceWallMesh" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            MeshFilter.mesh = mesh;
        }

        private void SetMeshVertices(ref Vector3[] vertices, Vector3 heightVector, int meshIndex, int pathIndex)
        {
            vertices[meshIndex] = path[pathIndex];
            vertices[meshIndex + 1] = path[pathIndex + 1];
            vertices[meshIndex + 2] = path[pathIndex] + heightVector;
            vertices[meshIndex + 3] = path[pathIndex + 1];
            vertices[meshIndex + 4] = path[pathIndex + 1] + heightVector;
            vertices[meshIndex + 5] = path[pathIndex] + heightVector;
        }

        private void SetFrontMeshTriangles(ref int[] triangles, int meshIndex)
        {
            triangles[meshIndex] = meshIndex;
            triangles[meshIndex + 1] = meshIndex + 1;
            triangles[meshIndex + 2] = meshIndex + 2;
            triangles[meshIndex + 3] = meshIndex + 3;
            triangles[meshIndex + 4] = meshIndex + 4;
            triangles[meshIndex + 5] = meshIndex + 5;
        }

        private void SetBackMeshTriangles(ref int[] triangles, int meshIndex)
        {
            triangles[meshIndex] = meshIndex + 2;
            triangles[meshIndex + 1] = meshIndex + 1;
            triangles[meshIndex + 2] = meshIndex;
            triangles[meshIndex + 3] = meshIndex + 5;
            triangles[meshIndex + 4] = meshIndex + 4;
            triangles[meshIndex + 5] = meshIndex + 3;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Super quick debug to preview how things should be

            if (path == null || path.Length == 0) return;

            for (var i = 0; i < path.Length; i++)
                UnityEditor.Handles.Label(path[i], $"{i}");

            Gizmos.color = Color.red;
            float height = GetHeight();
            for (var i = 0; i < path.Length - 1; i++)
            {
                var btmLeft = path[i];
                var btmRight = path[i + 1];
                var topLeft = btmLeft + Vector3.up * height;
                var topRight = btmRight + Vector3.up * height;
                Gizmos.DrawLine(btmLeft, btmRight);
                Gizmos.DrawLine(btmLeft, topLeft);
                Gizmos.DrawLine(btmRight, topRight);
                Gizmos.DrawLine(topLeft, topRight);
            }
            Gizmos.color = Color.white;
        }
#endif
    }
}