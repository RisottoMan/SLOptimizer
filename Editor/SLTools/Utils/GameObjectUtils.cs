using System.Collections.Generic;
using UnityEngine;

namespace SLTools.Utils
{
    public static class GameObjectUtils
    {
        public static GameObject[] GetChildGameObjects(GameObject parent, bool includeParents = false)
        {
            var result = new List<GameObject>();

            if (includeParents)
            {
                result.Add(parent);
            }

            foreach (Transform childTransform in parent.transform)
            {
                result.AddRange(GetChildGameObjects(childTransform.gameObject, true));
            }

            return result.ToArray();
        }

        public static GameObject[] GetChildGameObjects(GameObject[] parents, bool includeParents = false)
        {
            var result = new List<GameObject>();

            foreach (var parent in parents)
            {
                result.AddRange(GetChildGameObjects(parent, includeParents));
            }

            return result.ToArray();
        }

        public static void SetParentForArray(GameObject[] gameObjects, Transform newParent)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.transform.parent = newParent;
            }
        }

        /// <summary>
        /// If the specified tag is not in the project,
        /// it will be added and then installed on the game object.
        /// </summary>
        public static bool SafeSetTag(this GameObject gameObject, string tag)
        {
            if (TagExtensions.AddTagIfNotExists(tag))
            {
                gameObject.tag = tag;
                return true;
            }

            return false;
        }
    }
}
