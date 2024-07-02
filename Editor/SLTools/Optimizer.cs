// The code was written and tested for Unity 2019.4f
// The author does not warrant performance on other versions of the engine

using SLTools.Utils;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SLTools
{
    public sealed class Optimizer : EditorWindow
    {
        private static Material _sharedRegular;
        private static Material _sharedTransparent;

        #region Front-end Methods

        // Validating Methods
        [MenuItem("GameObject/SLTools/Optimize/Cube [Only Selected Objects]", true)]
        private static bool ValidateSelectedObjectIsCube()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.All(MeshUtils.IsCube);
        }

        [MenuItem("GameObject/SLTools/Optimize/Cube [Only Childs In Selected Objects]", true)]
        private static bool ValidateAnyCubeInChild()
        {
            var childGOs = GameObjectUtils.GetChildGameObjects(Selection.gameObjects, false);

            return childGOs.Length > 0 &&
                   childGOs.All(MeshUtils.IsCube);
        }


        // GUI Methods
        [MenuItem("GameObject/SLTools/Optimize/Cube [Only Selected Objects]", false, -10)]
        public static void OptimizeSelectedCube(MenuCommand command)
        {
            var cubeGO = command.context as GameObject;

            ProcessCube(cubeGO);
        }


        [MenuItem("GameObject/SLTools/Optimize/Cube [Only Childs In Selected Objects]", false, -10)]
        public static void OptimizeChildCubes(MenuCommand command)
        {
            var parentGO = command.context as GameObject;
            var cubesGO = GameObjectUtils.GetChildGameObjects(parentGO, false).Where(MeshUtils.IsCube).ToArray();

            foreach (var cubeGO in cubesGO)
            {
                ProcessCube(cubeGO);
            }
        }

        #endregion

        #region Back-end Methods

        private static void ProcessCube(GameObject cubeGO)
        {
            var color = Color.white;
            var isCollidable = true;
            var isVisible = true;

            if (cubeGO.transform.TryGetComponent<PrimitiveComponent>(out var primitiveComponent))
            {
                color = primitiveComponent.Color;
                isCollidable = primitiveComponent.Collidable;
                isVisible = primitiveComponent.Visible;
            }

            var quads = new GameObject[6];
            quads[0] = SpawnQuad(cubeGO.transform, Vector3.up, "up", color, isCollidable, isVisible);
            quads[1] = SpawnQuad(cubeGO.transform, Vector3.down, "down", color, isCollidable, isVisible);
            quads[2] = SpawnQuad(cubeGO.transform, Vector3.forward, "forward", color, isCollidable, isVisible);
            quads[3] = SpawnQuad(cubeGO.transform, Vector3.back, "back", color, isCollidable, isVisible);
            quads[4] = SpawnQuad(cubeGO.transform, Vector3.left, "left", color, isCollidable, isVisible);
            quads[5] = SpawnQuad(cubeGO.transform, Vector3.right, "right", color, isCollidable, isVisible);

            OptimizeCube(cubeGO);

            GameObjectUtils.SetParentForArray(quads, cubeGO.transform);
        }

        private static GameObject SpawnQuad(Transform cubeTransform, Vector3 side, string sideName, Color color, bool isCollidable, bool isVisible)
        {
            var rotation = Quaternion.LookRotation(side);

            // I would do Quaternion.Inverse, but it doesn't affect forward and backward
            rotation *= Quaternion.Euler(Vector3.up * 180.0F);

            var sideGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            sideGO.name = $"{cubeTransform.name}_{sideName}";
            sideGO.SafeSetTag("Quad");

            var transform = sideGO.transform;
            transform.SetParent(cubeTransform);
            transform.localPosition = side * 0.5F;
            transform.localRotation = rotation;
            transform.localScale = Vector3.one;
            transform.SetParent(cubeTransform.parent);

            DestroyImmediate(sideGO.GetComponent<MeshCollider>());

            var primitiveComponent = sideGO.AddComponent<PrimitiveComponent>();
            primitiveComponent.Color = color;
            primitiveComponent.Collidable = isCollidable;
            primitiveComponent.Visible = isVisible;

            if (_sharedRegular == null)
                _sharedRegular = (Material)Resources.Load("Materials/Regular");

            if (_sharedTransparent == null)
                _sharedTransparent = (Material)Resources.Load("Materials/Transparent");

            var meshRenderer = sideGO.GetComponent<MeshRenderer>();
            var material = color.a >= 1f ? _sharedRegular : _sharedTransparent;
            meshRenderer.sharedMaterial = new Material(material);
            meshRenderer.sharedMaterial.color = color;

            return sideGO;
        }

        private static void OptimizeCube(GameObject cubeGO)
        {
            // Optionally, delete it if it doesn't work on your version of Unity.
            if (PrefabUtility.IsAnyPrefabInstanceRoot(cubeGO))
            {
                PrefabUtility.UnpackPrefabInstance(cubeGO, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            var transform = cubeGO.transform;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            foreach (var component in cubeGO.GetComponents<Component>())
            {
                if (component is Transform)
                    continue;

                DestroyImmediate(component);
            }

            cubeGO.name += "_optimized";
        }


        #endregion
    }
}
