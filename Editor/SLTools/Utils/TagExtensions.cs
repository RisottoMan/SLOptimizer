using UnityEditor;
using UnityEngine;

public static class TagExtensions
{
    /// <summary>
    /// Add a tag to the project if it is missing.
    /// </summary>
    public static bool AddTagIfNotExists(string tag)
    {
        var tagManagerAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

        if (tagManagerAssets.Length > 0)
        {
            var tagManager = new SerializedObject(tagManagerAssets[0]);
            var tagsProperty = tagManager.FindProperty("tags");
            var tagsContainsMesh = false;

            if (tagsProperty.arraySize > 0)
            {
                for (var j = 0; j < tagsProperty.arraySize; j++)
                {
                    if (tagsProperty.GetArrayElementAtIndex(j).stringValue.Equals(tag))
                    {
                        tagsContainsMesh = true;
                        break;
                    }
                }
            }

            if (!tagsContainsMesh)
            {
                var index = tagsProperty.arraySize;
                tagsProperty.InsertArrayElementAtIndex(index);
                var newTag = tagsProperty.GetArrayElementAtIndex(index);

                newTag.stringValue = tag;
                tagManager.ApplyModifiedProperties();
            }

            return true;
        }
        else
        {
            Debug.LogError($"[TagExtensions] Asset \"ProjectSettings/TagManager.asset\" was not found!");
            return false;
        }
    }
}
