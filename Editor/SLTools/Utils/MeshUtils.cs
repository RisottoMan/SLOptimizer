using System.Linq;
using UnityEngine;

namespace SLTools.Utils
{
    public static class MeshUtils
    {
        public static bool IsCube(GameObject gameObject)
        {
            return CompareMeshName(gameObject, "Cube", "Cube Instance");
        }

        public static bool CompareMeshName(GameObject gameObject, params string[] meshNames)
        {
            if (gameObject == null) return false;

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            return meshFilter != null && meshNames.Contains(meshFilter.sharedMesh.name);
        }
    }
}
