using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class Humanizer : EditorWindow
    {
        private ReferenceObjectMotionCaptureTool referenceObjectMotionCaptureTool = new ReferenceObjectMotionCaptureTool();
        private IKHandlesCreatorTool ikHandlesCreatorTool = new IKHandlesCreatorTool();
        private IKHandlesReferenceAttacherTool ikHandlesReferenceAttacherTool = new IKHandlesReferenceAttacherTool();
        private AnimationAdjusterTool animationAdjusterTool = new AnimationAdjusterTool();

        [MenuItem("Tools/Animation/Humanizer")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            Humanizer window = (Humanizer)EditorWindow.GetWindow(typeof(Humanizer));
        }

        // Window has been selected
        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            referenceObjectMotionCaptureTool.DrawHandles();
            animationAdjusterTool.DrawHandles();
        }

        void OnGUI()
        {
            referenceObjectMotionCaptureTool.Draw();

            ikHandlesCreatorTool.Draw();

            ikHandlesReferenceAttacherTool.Draw();

            animationAdjusterTool.Draw();
        }
    }

    [System.Serializable]
    public class ReferenceObjectMotionCaptureTool
    {
        private GameObject animationSourceGameObject;
        private Editor animationSourceGameObjectEditor;

        private Animator animator;
        private int animationSelected;
        private string[] animationOptions;

        private GameObject referenceGameObject;
        private Editor referenceGameObjectEditor;

        private GameObject referenceDuplicate;

        private AnimationClip referenceMotion;

        private bool saveAsLegacy;

        private string preferredPath = "Assets/";

        private bool showKeyframes;
        private List<Transform> keyframes = new List<Transform>();

        public void Draw()
        {
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Reference object motion capture", EditorUtils.LabelStyleBoldHeading);

            EditorUtils.ColumnsSameWidth(
                rect,
                new Action<float>[]
                {
                    (width) => EditorUtils.ObjectAndPreviewColumn("Animation source", ref animationSourceGameObject, ref animationSourceGameObjectEditor, width),
                    (width) => EditorUtils.ObjectAndPreviewColumn("Reference", ref referenceGameObject, ref referenceGameObjectEditor, width),
                });

            if (animationSourceGameObject != null)
            {
                animator = animationSourceGameObject.GetComponent<Animator>();

                var clips = animator.runtimeAnimatorController.animationClips;

                animationOptions = new string[clips.Length];

                for (int i = 0; i < animationOptions.Length; i++)
                    animationOptions[i] = clips[i].name;

                animationSelected = EditorUtils.PopupSelectionColumn("Animation", animationOptions, animationSelected, rect.width);
            }

            saveAsLegacy = EditorGUILayout.Toggle("Save as legacy animation", saveAsLegacy);

            preferredPath = EditorGUILayout.TextField("Preferred path", preferredPath);

            showKeyframes = EditorGUILayout.Toggle("Show keyframes", showKeyframes);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Bake reference motion"))
                BakeReferenceMotion();

            EditorGUILayout.Space();
        }

        public void DrawHandles()
        {
            if (showKeyframes)
            {
                keyframes.RemoveAll(keyframe => keyframe == null);

                foreach (var keyframe in keyframes)
                    Handles.PositionHandle(keyframe.position, keyframe.rotation);

                var cachedColor = Handles.color;

                Handles.color = Color.white;

                for (int i = 0; i < keyframes.Count - 1; i++)
                    Handles.DrawLine(keyframes[i].position, keyframes[i + 1].position);

                Handles.color = cachedColor;
            }
        }

        private void BakeReferenceMotion()
        {
            if (animationSourceGameObject == null
                || referenceGameObject == null
                || animator == null)
                return;

            #region Get target animation clip

            var clip = animator.runtimeAnimatorController.animationClips[animationSelected];

            var clipName = clip.name;

            var clipHash = Animator.StringToHash(clipName);

            #endregion

            #region Create reference duplicate's anchor

            GameObject anchor = new GameObject("Anchor");

            anchor.transform.SetParent(animationSourceGameObject.transform);

            anchor.transform.position = referenceGameObject.transform.position;

            anchor.transform.rotation = referenceGameObject.transform.rotation;

            #endregion

            #region Create reference duplicate

            referenceDuplicate = GameObject.Instantiate(referenceGameObject);

            referenceDuplicate.transform.SetParent(anchor.transform, true);

            #endregion

            #region Create keyframe anchor

            var keyframeAnchor = new GameObject("Animation");

            keyframeAnchor.transform.SetParent(anchor.transform);

            keyframeAnchor.transform.localPosition = Vector3.zero;

            keyframeAnchor.transform.localRotation = Quaternion.identity;

            keyframeAnchor.transform.localScale = Vector3.zero;

            #endregion

            #region Create animation clip

            var secondsPerFrame = 1f / clip.frameRate;

            var durationInSeconds = clip.length;

            var durationInFrames = Mathf.RoundToInt(durationInSeconds / secondsPerFrame);

            referenceMotion = new AnimationClip();

            referenceMotion.legacy = saveAsLegacy;

            referenceMotion.frameRate = clip.frameRate;

            keyframes.Clear();

            #endregion

            EditorAnimationPlayer.Activate(
                secondsPerFrame,
                durationInFrames,
                (currentSecond) => BakeFrame(keyframeAnchor.transform, animator, clipHash, currentSecond, durationInSeconds),
                () => { return FinishBaking(clipHash); });
        }

        private void BakeFrame(Transform anchor, Animator animator, int clipHash, float currentSecond, float durationInSeconds)
        {
            EditorAnimationPlayer.SetAnimationFrame(animator, clipHash, currentSecond / durationInSeconds);

            AddKeyframe(currentSecond, anchor);
        }

        private void AddKeyframe(float currentSecond, Transform anchor)
        {
            GameObject keyframeGameObject = new GameObject(string.Format("Keyframe {0}", currentSecond));

            keyframeGameObject.transform.SetParent(anchor);

            keyframeGameObject.transform.localScale = Vector3.zero;

            keyframeGameObject.transform.position = referenceGameObject.transform.position;

            keyframeGameObject.transform.rotation = referenceGameObject.transform.rotation;

            keyframeGameObject.AddComponent<Timestamp>().Time = currentSecond;

            keyframes.Add(keyframeGameObject.transform);
        }

        private bool FinishBaking(int clipHash)
        {
            #region Fill animation clip with curves

            var referenceTransformLocalPositionX = new AnimationCurve();
            var referenceTransformLocalPositionY = new AnimationCurve();
            var referenceTransformLocalPositionZ = new AnimationCurve();

            var referenceTransformRotationX = new AnimationCurve();
            var referenceTransformRotationY = new AnimationCurve();
            var referenceTransformRotationZ = new AnimationCurve();
            var referenceTransformRotationW = new AnimationCurve();

            foreach (var keyframe in keyframes)
            {
                float currentSecond = keyframe.GetComponent<Timestamp>().Time;

                referenceTransformLocalPositionX.AddKey(currentSecond, keyframe.localPosition.x);
                referenceTransformLocalPositionY.AddKey(currentSecond, keyframe.localPosition.y);
                referenceTransformLocalPositionZ.AddKey(currentSecond, keyframe.localPosition.z);

                referenceTransformRotationX.AddKey(currentSecond, keyframe.localRotation.x);
                referenceTransformRotationY.AddKey(currentSecond, keyframe.localRotation.y);
                referenceTransformRotationZ.AddKey(currentSecond, keyframe.localRotation.z);
                referenceTransformRotationW.AddKey(currentSecond, keyframe.localRotation.w);
            }

            referenceMotion.SetCurve("", typeof(Transform), "localPosition.x", referenceTransformLocalPositionX);
            referenceMotion.SetCurve("", typeof(Transform), "localPosition.y", referenceTransformLocalPositionY);
            referenceMotion.SetCurve("", typeof(Transform), "localPosition.z", referenceTransformLocalPositionZ);

            referenceMotion.SetCurve("", typeof(Transform), "localRotation.x", referenceTransformRotationX);
            referenceMotion.SetCurve("", typeof(Transform), "localRotation.y", referenceTransformRotationY);
            referenceMotion.SetCurve("", typeof(Transform), "localRotation.z", referenceTransformRotationZ);
            referenceMotion.SetCurve("", typeof(Transform), "localRotation.w", referenceTransformRotationW);

            #endregion

            #region Create animation asset and attach it to reference duplicate

            var clipName = string.Format("{0} reference motion", referenceGameObject.name);

            var path = EditorUtility.SaveFilePanel(
                "Save reference motion animation clip",
                preferredPath,
                string.Format("{0}.anim", clipName),
                "anim");

            if (path.Length != 0 && EditorUtils.PathIsValidForProject(path))
            {
                var relativePath = "Assets" + path.Substring(Application.dataPath.Length);

                AssetDatabase.CreateAsset(referenceMotion, relativePath);

                
                int lastOccurence = relativePath.LastIndexOf('/');

                preferredPath = relativePath.Substring(0, lastOccurence);


                var actualClipName = relativePath.Substring(lastOccurence).Split('.')[0];

                if (saveAsLegacy)
                {
                    var animationComponent = referenceDuplicate.AddComponent<Animation>();

                    animationComponent.AddClip(referenceMotion, "Motion");
                }
                else
                {
                    var animatorComponent = referenceDuplicate.AddComponent<Animator>();

                    var controllerName = referenceGameObject.name;

                    path = EditorUtility.SaveFilePanel(
                        "Save reference motion animator controller",
                        preferredPath,
                        string.Format("{0}.controller", controllerName),
                        "controller");

                    if (path.Length != 0 && EditorUtils.PathIsValidForProject(path))
                    {
                        relativePath = "Assets" + path.Substring(Application.dataPath.Length);

                        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(relativePath);

                        var rootStateMachine = controller.layers[0].stateMachine;

                        var state = rootStateMachine.AddState(actualClipName);

                        rootStateMachine.AddEntryTransition(state);

                        state.motion = referenceMotion;

                        animatorComponent.runtimeAnimatorController = controller;
                    }
                }
            }

            #endregion

            #region Cleanup

            referenceDuplicate.transform.localPosition = Vector3.zero;

            referenceDuplicate.transform.localRotation = Quaternion.identity;

            EditorAnimationPlayer.SetAnimationFrame(animator, clipHash, 0f);

            #endregion

            return true;
        }
    }

    [System.Serializable]
    public class IKHandlesCreatorTool
    {
        private Transform selectedBone;
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private float weightTreshold = 0.9f;
        private bool selectBonesRecursively;
        private bool mergeBones;
        private bool selectOnlyActiveOnes;
        private bool showTargetVerticesConvexHull;

        private Mesh snapshot;

        public void Draw()
        {
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("IK handles generation", EditorUtils.LabelStyleBoldHeading);

            EditorGUILayout.Space();

            EditorUtils.ObjectAndLabelRow<Transform>("Bone", ref selectedBone);

            EditorUtils.ObjectAndLabelRow<SkinnedMeshRenderer>("Mesh", ref skinnedMeshRenderer);

            weightTreshold = EditorUtils.FloatAndLabelRow("Min bone weight", weightTreshold);

            selectBonesRecursively = EditorUtils.BoolAndLabelRow("Select bones recursively", selectBonesRecursively);

            if (selectBonesRecursively)
            {
                mergeBones = EditorUtils.BoolAndLabelRow("Merge selected bones vertices", mergeBones);

                selectOnlyActiveOnes = EditorUtils.BoolAndLabelRow("Select only active in hierarchy", selectOnlyActiveOnes);
            }

            showTargetVerticesConvexHull = EditorUtils.BoolAndLabelRow("Show target vertices convex hull", showTargetVerticesConvexHull);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create IK handle"))
                CreateIKHandle();

            EditorGUILayout.Space();
        }

        private void CreateIKHandle()
        {
            #region Create snapshot

            snapshot = new Mesh();

            skinnedMeshRenderer.BakeMesh(snapshot);

            #endregion

            #region Find target bones

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            int[] targetBones = FindTargetBones(skinnedMesh);

            #endregion

            #region Create handles for bones

            if (mergeBones)
                CreateHandleForBones(skinnedMesh, targetBones);
            else
                for (int i = 0; i < targetBones.Length; i++)
                    CreateHandleForBones(skinnedMesh, new[] { targetBones[i] });

            #endregion
        }

        private int[] FindTargetBones(Mesh mesh)
        {
            List<int> result = new List<int>();

            var revelantBones = skinnedMeshRenderer.bones;

            AddBonesRecursively(selectedBone, revelantBones, result);

            return result.ToArray();
        }

        private void AddBonesRecursively(Transform bone, Transform[] revelantBones, List<int> result)
        {
            int boneIndex = Array.IndexOf(revelantBones, bone);

            if (boneIndex == -1)
            {
                Debug.LogWarning(string.Format("[Humanizer] TRANSFORM IRREVELANT : {0}", bone.name));

                return;
            }

            result.Add(boneIndex);

            if (selectBonesRecursively)
                foreach (Transform child in bone)
                    if (!selectOnlyActiveOnes || bone.gameObject.activeInHierarchy)
                        AddBonesRecursively(child, revelantBones, result);
        }

        private void CreateHandleForBones(Mesh skinnedMesh, int[] targetBones)
        {
            #region Find vertices affected by target bones

            int[] verticesAffectedByTargetBones = FindVerticesAffectedByTargetBones(skinnedMesh, targetBones);

            if (verticesAffectedByTargetBones.Length == 0)
                return;

            #endregion

            #region Create convex hull from vertices

            var center = CalculateCenterOfConvexMesh(verticesAffectedByTargetBones);

            List<Vector3> verticesList = new List<Vector3>();

            foreach (var index in verticesAffectedByTargetBones)
                verticesList.Add(snapshot.vertices[index] - center);

            Vector3[] verticesArray = verticesList.ToArray();

            int[] indicies = ConvexHull.Generate(verticesArray);

            #endregion

            #region Create minimum bounding box from convex hull

            Mesh targetVisualMesh = new Mesh();

            targetVisualMesh.vertices = verticesArray;

            targetVisualMesh.triangles = indicies;

            Quaternion rotation = GetMinimumBoundingBox(targetVisualMesh);

            #endregion

            #region Create handle

            GameObject handle = new GameObject("Handle");

            handle.transform.SetParent(mergeBones ? selectedBone : skinnedMeshRenderer.bones[targetBones[0]], true); //(selectedBone, true);

            //WARNING! The order of setting rotation before position is VITAL here
            handle.transform.rotation = skinnedMeshRenderer.transform.rotation * rotation;

            handle.transform.position = skinnedMeshRenderer.transform.position + RotateAround(center, Vector3.zero, skinnedMeshRenderer.transform.rotation);

            #endregion

            #region Create box collider to visualize bounding box

            var meshFilter = handle.AddComponent<MeshFilter>();

            meshFilter.mesh = targetVisualMesh;


            var meshRenderer = handle.AddComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

            meshRenderer.sharedMaterial.SetColor("_Color", Color.red);

            //Note that meshRenderer.Bounds, mesh.Bounds, boxCollider.Bounds and custom bounds made from boxCollider.center and boxCollider.size are
            //four different bounds (except meshRenderer.Bounds and boxCollider.Bounds - they're the same)
            //So to have a proper bounding box we need to put a collider first. Stupid yet working solution
            var boxCollider = handle.AddComponent<BoxCollider>();


            var boneHook = handle.AddComponent<BoneHook>();

            boneHook.Bounds = new Bounds(boxCollider.center, boxCollider.size); //boxCollider.bounds;

            boneHook.Color = Color.green;

            GameObject.DestroyImmediate(boxCollider);

            #endregion

            #region Clean up if necessary

            if (!showTargetVerticesConvexHull)
            {
                GameObject.DestroyImmediate(meshFilter);

                GameObject.DestroyImmediate(meshRenderer);
            }

            #endregion
        }

        private int[] FindVerticesAffectedByTargetBones(Mesh mesh, int[] targetBones)
        {
            var boneWeights = mesh.boneWeights;

            var bones = skinnedMeshRenderer.bones;

            List<int> result = new List<int>();

            for (int i = 0; i < boneWeights.Length; i++)
            {
                BoneWeight boneWeight = boneWeights[i];

                for (int j = 0; j < targetBones.Length; j++)
                {
                    int targetBoneIndex = targetBones[j];

                    if (boneWeight.boneIndex0 == targetBoneIndex && boneWeight.weight0 > weightTreshold
                        || boneWeight.boneIndex1 == targetBoneIndex && boneWeight.weight1 > weightTreshold
                        || boneWeight.boneIndex2 == targetBoneIndex && boneWeight.weight2 > weightTreshold
                        || boneWeight.boneIndex3 == targetBoneIndex && boneWeight.weight3 > weightTreshold)
                    {
                        result.Add(i);
                    }
                }
            }

            return result.ToArray();
        }

        private Vector3 CalculateCenterOfConvexMesh(int[] verticesAffectedByTargetBones)
        {
            Vector3 xmin, xmax, ymin, ymax, zmin, zmax;

            xmin = ymin = zmin = Vector3.one * float.PositiveInfinity;

            xmax = ymax = zmax = Vector3.one * float.NegativeInfinity;

            foreach (var index in verticesAffectedByTargetBones)
            {
                Vector3 p = snapshot.vertices[index];

                if (p.x < xmin.x) xmin = p;
                if (p.x > xmax.x) xmax = p;
                if (p.y < ymin.y) ymin = p;
                if (p.y > ymax.y) ymax = p;
                if (p.z < zmin.z) zmin = p;
                if (p.z > zmax.z) zmax = p;
            }

            var xSpan = (xmax - xmin).sqrMagnitude;
            var ySpan = (ymax - ymin).sqrMagnitude;
            var zSpan = (zmax - zmin).sqrMagnitude;

            var dia1 = xmin;
            var dia2 = xmax;

            var maxSpan = xSpan;

            if (ySpan > maxSpan)
            {
                maxSpan = ySpan;

                dia1 = ymin; dia2 = ymax;
            }

            if (zSpan > maxSpan)
            {
                dia1 = zmin; dia2 = zmax;
            }

            var center = (dia1 + dia2) * 0.5f;

            var sqRad = (dia2 - center).sqrMagnitude;

            var radius = Mathf.Sqrt(sqRad);

            foreach (var index in verticesAffectedByTargetBones)
            {
                Vector3 p = snapshot.vertices[index];

                float d = (p - center).sqrMagnitude;

                if (d > sqRad)
                {
                    var r = Mathf.Sqrt(d);

                    radius = (radius + r) * 0.5f;

                    sqRad = radius * radius;

                    var offset = r - radius;

                    center = (radius * center + offset * p) / r;
                }
            }

            return center;
        }

        private Quaternion GetMinimumBoundingBox(Mesh mesh)
        {
            float minSize = float.MaxValue;

            Vector3[] result = null;

            Quaternion resultRotation = Quaternion.identity;

            var vertices = mesh.vertices;

            int[] indices = mesh.triangles;

            int maxValue = indices.Length - indices.Length % 3;

            for (int i = 0; i < maxValue; i += 3)
            {
                Vector3 normal = GetPolygonNormalVector(vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]]);

                Quaternion rotation = Quaternion.LookRotation(-normal);

                Vector3[] newVertices = new Vector3[vertices.Length];

                Array.Copy(vertices, newVertices, vertices.Length);

                RotateAround(newVertices, Vector3.zero, rotation);

                mesh.vertices = newVertices;

                mesh.RecalculateNormals();

                mesh.RecalculateBounds();

                float newSize = mesh.bounds.size.magnitude;

                if (newSize < minSize)
                {
                    minSize = newSize;

                    result = newVertices;

                    resultRotation = rotation;
                }
            }

            mesh.vertices = result;

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            Quaternion inverseRotation = Quaternion.identity * Quaternion.Inverse(resultRotation);

            return inverseRotation;
        }

        private Vector3 GetPolygonNormalVector(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);

            var norm = Vector3.Normalize(dir);

            return norm;
        }

        private void RotateAround(Vector3[] vertices, Vector3 pivot, Quaternion rotation)
        {
            for (int n = 0; n < vertices.Length; n++)
            {
                vertices[n] = rotation * (vertices[n] - pivot) + pivot;
            }
        }

        private Vector3 RotateAround(Vector3 position, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (position - pivot) + pivot;
        }
    }

    [System.Serializable]
    public class IKHandlesReferenceAttacherTool
    {
        private Transform selectedReference;
        private Transform leftHandHandle;
        private Transform rightHandHandle;

        public void Draw()
        {
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("IK handles placement on reference", EditorUtils.LabelStyleBoldHeading);

            EditorGUILayout.Space();

            EditorUtils.ObjectAndLabelRow<Transform>("Reference", ref selectedReference);

            EditorUtils.ObjectAndLabelRow<Transform>("Left hand handle", ref leftHandHandle);

            EditorUtils.ObjectAndLabelRow<Transform>("Right hand handle", ref rightHandHandle);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Place IK handles on the reference"))
                PlaceIKHandlesOnReference();

            EditorGUILayout.Space();
        }

        private void PlaceIKHandlesOnReference()
        {
            if (selectedReference == null)
                return;

            if (leftHandHandle == null && rightHandHandle == null)
                return;

            #region Create handles keeper

            GameObject handlesKeeper = new GameObject("Handles");

            handlesKeeper.transform.SetParent(selectedReference);

            handlesKeeper.transform.localPosition = Vector3.zero;

            handlesKeeper.transform.localRotation = Quaternion.identity;

            #endregion

            #region Attach handles

            if (leftHandHandle != null)
                PlaceHandle("Left hand handle", leftHandHandle, handlesKeeper.transform);

            if (rightHandHandle != null)
                PlaceHandle("Right hand handle", rightHandHandle, handlesKeeper.transform);

            #endregion
        }

        private void PlaceHandle(string name, Transform handle, Transform keeper)
        {
            GameObject handleDuplicate = GameObject.Instantiate(handle.gameObject, keeper, true);

            handleDuplicate.name = name;

            //return handleDuplicate.transform;
        }
    }

    [System.Serializable]
    public class AnimationAdjusterTool
    {
        private GameObject targetGameObject;
        private Editor targetGameObjectEditor;

        private Transform targetsLeftHandHandle;
        private Transform targetsRightHandHandle;

        private GameObject referenceGameObject;
        private Editor referenceGameObjectEditor;

        private Transform referencesLeftHandHandle;
        private Transform referencesRightHandHandle;

        private Animator targetsAnimator;
        private int targetsAnimationSelected;

        private string[] targetsAnimationOptions;

        private Animator referencesAnimator;
        private int referencesAnimationSelected;

        private string[] referencesAnimationOptions;

        private AnimationClip adjustedAnimation;

        private bool useTargetsMotionRoot;

        private bool showGradient;

        //They've seen some shit
        private List<GradientDescentIteration> leftHandHistory = new List<GradientDescentIteration>();
        private List<GradientDescentIteration> rightHandHistory = new List<GradientDescentIteration>();

        public void Draw()
        {
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Adjust animation to reference", EditorUtils.LabelStyleBoldHeading);

            EditorUtils.ColumnsSameWidth(
                rect,
                new Action<float>[]
                {
                    (width) =>
                    {
                        EditorUtils.ObjectAndPreviewColumn("Animation target", ref targetGameObject, ref targetGameObjectEditor, width);

                        EditorUtils.ObjectAndLabelRow<Transform>("Left hand handle", ref targetsLeftHandHandle);

                        EditorUtils.ObjectAndLabelRow<Transform>("Right hand handle", ref targetsRightHandHandle);

                        if (targetGameObject != null)
                        {
                            targetsAnimator = targetGameObject.GetComponent<Animator>();

                            var clips = targetsAnimator.runtimeAnimatorController.animationClips;

                            targetsAnimationOptions = new string[clips.Length];

                            for (int i = 0; i < targetsAnimationOptions.Length; i++)
                                targetsAnimationOptions[i] = clips[i].name;

                            targetsAnimationSelected = EditorUtils.PopupSelectionColumn("Animation", targetsAnimationOptions, targetsAnimationSelected, rect.width);
                        }

                        if (targetGameObject != null && targetGameObject.GetComponent<IKHook>() != null)
                        {
                            if (GUILayout.Button("Select IK hook"))
                                Selection.activeGameObject = targetGameObject;

                            if (GUILayout.Button("Detach IK hook"))
                                GameObject.DestroyImmediate(targetGameObject.GetComponent<IKHook>());
                        }
                    },
                    (width) =>
                    {
                        EditorUtils.ObjectAndPreviewColumn("Reference", ref referenceGameObject, ref referenceGameObjectEditor, width);

                        EditorUtils.ObjectAndLabelRow<Transform>("Left hand handle", ref referencesLeftHandHandle);

                        EditorUtils.ObjectAndLabelRow<Transform>("Right hand handle", ref referencesRightHandHandle);

                        if (referenceGameObject != null)
                        {
                            referencesAnimator = referenceGameObject.GetComponent<Animator>();

                            var clips = referencesAnimator.runtimeAnimatorController.animationClips;

                            referencesAnimationOptions = new string[clips.Length];

                            for (int i = 0; i < referencesAnimationOptions.Length; i++)
                                referencesAnimationOptions[i] = clips[i].name;

                            referencesAnimationSelected = EditorUtils.PopupSelectionColumn("Animation", referencesAnimationOptions, referencesAnimationSelected, rect.width);
                        }
                    },
                });

            useTargetsMotionRoot = EditorGUILayout.Toggle("Use target's motion root", useTargetsMotionRoot);

            showGradient = EditorGUILayout.Toggle("Show gradient", showGradient);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Attach IK handles to reference"))
                AttachIKTargetsToReference();

            if (GUILayout.Button("Adjust animation to reference"))
                AdjustAnimationToReference();
        }

        public void DrawHandles()
        {
            if (showGradient)
            {
                foreach (var iteration in leftHandHistory)
                {
                    var cachedColor = Handles.color;

                    Handles.PositionHandle(iteration.CurrentPosition, iteration.CurrentRotation);

                    Handles.color = Color.green;

                    Handles.DrawLine(iteration.CurrentPosition, iteration.PositionWithGradient);

                    Handles.PositionHandle(iteration.PositionWithGradient, iteration.RotationWithGradient);

                    Handles.color = Color.red;

                    Handles.DrawLine(iteration.CurrentPosition, iteration.NewPosition);

                    Handles.PositionHandle(iteration.NewPosition, iteration.NewRotation);

                    Handles.color = cachedColor;
                }

                foreach (var iteration in rightHandHistory)
                {
                    var cachedColor = Handles.color;

                    Handles.PositionHandle(iteration.CurrentPosition, iteration.CurrentRotation);

                    Handles.color = Color.green;

                    Handles.DrawLine(iteration.CurrentPosition, iteration.PositionWithGradient);

                    Handles.PositionHandle(iteration.PositionWithGradient, iteration.RotationWithGradient);

                    Handles.color = Color.red;

                    Handles.DrawLine(iteration.CurrentPosition, iteration.NewPosition);

                    Handles.PositionHandle(iteration.NewPosition, iteration.NewRotation);

                    Handles.color = cachedColor;
                }
            }
        }

        private void AttachIKTargetsToReference()
        {
            #region Select a target animation

            var clip = targetsAnimator.runtimeAnimatorController.animationClips[targetsAnimationSelected];

            var clipName = clip.name;

            var clipHash = Animator.StringToHash(clipName);

            #endregion

            targetsAnimator.logWarnings = false;

            var ikHook = targetsAnimator.gameObject.GetComponent<IKHook>();

            if (ikHook == null)
                ikHook = targetsAnimator.gameObject.AddComponent<IKHook>();

            ikHook.animator = targetsAnimator;

            ikHook.leftHandPositionWeight = ikHook.rightHandPositionWeight = ikHook.leftHandRotationWeight = ikHook.rightHandRotationWeight = 1f; // 0f;

            ikHook.leftHandIKGoal = null;

            ikHook.rightHandIKGoal = null;

            ikHook.DirectControl = false;

            if (targetsLeftHandHandle != null)
                ikHook.leftHandIKGoal = PlaceIKTarget(referencesLeftHandHandle); //(targetsLeftHandHandle, referencesLeftHandHandle, distanceFromIKTargetToHandle, rotationBetweenIKTargetAndHandle);

            if (targetsRightHandHandle != null)
                ikHook.rightHandIKGoal = PlaceIKTarget(referencesRightHandHandle); //(targetsRightHandHandle, referencesRightHandHandle, distanceFromIKTargetToHandle, rotationBetweenIKTargetAndHandle);

            GradientDescentIteration leftHandIteration = new GradientDescentIteration()
            {
                IKGoal = targetsLeftHandHandle != null
                    ? ikHook.leftHandIKGoal.transform
                    : null,
                ReferencesHandle = referencesLeftHandHandle,
                TargetsHandle = targetsLeftHandHandle
            };

            GradientDescentIteration rightHandIteration = new GradientDescentIteration()
            {
                IKGoal = targetsRightHandHandle != null
                    ? ikHook.rightHandIKGoal.transform
                    : null,
                ReferencesHandle = referencesRightHandHandle,
                TargetsHandle = targetsRightHandHandle
            };

            leftHandHistory.Clear();

            rightHandHistory.Clear();

            EditorAnimationPlayer.Activate(
                0f,
                100,
                (currentSecond) =>
                {
                    GradientDescent(ref leftHandIteration, ref rightHandIteration, targetsAnimator, clipHash);
                },
                () =>
                {
                    ikHook.DirectControl = true;

                    return true;
                },
                () => {
                    return leftHandIteration.DistanceSatisfying
                        && leftHandIteration.AngleSatisfying
                        && rightHandIteration.DistanceSatisfying
                        && rightHandIteration.AngleSatisfying;
                });
        }

        private GameObject PlaceIKTarget(Transform referenceHandle) //(Transform handle, Transform referenceHandle, Vector3 distanceFromIKTargetToHandle, Quaternion rotationBetweenIKTargetAndHandle)
        {
            GameObject ikTarget = new GameObject("IK target");

            ikTarget.transform.SetParent(referenceHandle);

            ikTarget.transform.localPosition = Vector3.zero; //distanceFromIKTargetToHandle;

            ikTarget.transform.localRotation = Quaternion.identity; //rotationBetweenIKTargetAndHandle;

            ikTarget.transform.localScale = Vector3.one;

            return ikTarget;
        }

        private void GradientDescent(ref GradientDescentIteration leftHandGDIteration, ref GradientDescentIteration rightHandGDIteration, Animator targetsAnimator, int clipHash, float timeNormalized = 0f)
        {
            #region Set up

            float sampleDistance = 0.01f; //0.1f;

            float sampleAngle = 1f;

            float satisfyingDistanceError = 0.01f;

            float satisfyingAngleError = 1f;

            float distanceLearningRate = 0.008f;

            float rotationLearningRate = 10f;

            #endregion

            EditorAnimationPlayer.SetAnimationFrame(targetsAnimator, clipHash, timeNormalized);

            #region Measure F(x)

            if (targetsLeftHandHandle != null)
                leftHandGDIteration = SetUp(leftHandGDIteration, sampleDistance, sampleAngle, satisfyingDistanceError, satisfyingAngleError);

            if (targetsRightHandHandle != null)
                rightHandGDIteration = SetUp(rightHandGDIteration, sampleDistance, sampleAngle, satisfyingDistanceError, satisfyingAngleError);

            EditorAnimationPlayer.SetAnimationFrame(targetsAnimator, clipHash, timeNormalized);

            #endregion

            #region Measure F(x + sample) and calculate gradient

            if (targetsLeftHandHandle != null && (!leftHandGDIteration.DistanceSatisfying || !leftHandGDIteration.AngleSatisfying))
                leftHandGDIteration = CalculateGradient(leftHandGDIteration, sampleDistance, sampleAngle, distanceLearningRate, rotationLearningRate);

            if (targetsRightHandHandle != null && (!rightHandGDIteration.DistanceSatisfying || !rightHandGDIteration.AngleSatisfying))
                rightHandGDIteration = CalculateGradient(rightHandGDIteration, sampleDistance, sampleAngle, distanceLearningRate, rotationLearningRate);

            EditorAnimationPlayer.SetAnimationFrame(targetsAnimator, clipHash, 0f);

            #endregion

            if (targetsLeftHandHandle != null)
            {
                leftHandGDIteration.PositionWithGradient = leftHandGDIteration.IKGoal.position;
                leftHandGDIteration.RotationWithGradient = leftHandGDIteration.IKGoal.rotation;
            }

            if (targetsRightHandHandle != null)
            {
                rightHandGDIteration.PositionWithGradient = rightHandGDIteration.IKGoal.position;
                rightHandGDIteration.RotationWithGradient = rightHandGDIteration.IKGoal.rotation;
            }

            if (targetsLeftHandHandle != null)
            {
                leftHandHistory.Add(leftHandGDIteration);

                Debug.Log(string.Format(
                    "Progress (left): prev dist: {0} new dist: {1} prev angle: {2} new angle: {3}",
                    leftHandGDIteration.CurrentDistance,
                    leftHandGDIteration.NewDistance,
                    leftHandGDIteration.CurrentAngle,
                    leftHandGDIteration.NewAngle));
            }

            if (targetsRightHandHandle != null)
            {
                rightHandHistory.Add(rightHandGDIteration);

                Debug.Log(string.Format(
                    "Progress (right): prev dist: {0} new dist: {1} prev angle: {2} new angle: {3}",
                    rightHandGDIteration.CurrentDistance,
                    rightHandGDIteration.NewDistance,
                    rightHandGDIteration.CurrentAngle,
                    rightHandGDIteration.NewAngle));
            }

            return;
        }

        private GradientDescentIteration SetUp(
            GradientDescentIteration iteration, 
            float sampleDistance, 
            float sampleAngle, 
            float satisfyingDistanceError, 
            float satisfyingAngleError)
        {
            iteration.CurrentPosition = iteration.IKGoal.position;

            iteration.CurrentRotation = iteration.IKGoal.rotation;


            iteration.DistanceBetweenHandles = iteration.ReferencesHandle.position - iteration.TargetsHandle.position;

            iteration.CurrentDistance = iteration.DistanceBetweenHandles.magnitude;

            iteration.DistanceSatisfying = iteration.CurrentDistance < satisfyingDistanceError;


            iteration.RotationBetweenHandles = iteration.TargetsHandle.rotation * Quaternion.Inverse(iteration.ReferencesHandle.rotation);

            iteration.CurrentAngle = Quaternion.Angle(Quaternion.identity, iteration.RotationBetweenHandles);

            iteration.AngleSatisfying = iteration.CurrentAngle < satisfyingAngleError;


            if (!iteration.DistanceSatisfying)
            {
                iteration.SampleVector = iteration.DistanceBetweenHandles.normalized * sampleDistance;

                iteration.IKGoal.position += iteration.SampleVector;
            }

            if (!iteration.AngleSatisfying)
            {
                iteration.SampleQuaternion = Quaternion.Slerp(
                    Quaternion.identity,
                    iteration.RotationBetweenHandles,
                    sampleAngle / Quaternion.Angle(Quaternion.identity, iteration.RotationBetweenHandles));

                var test = iteration.TargetsHandle.rotation * iteration.RotationBetweenHandles;

                /*
                var test = iteration.TargetsHandle.rotation;

                for (int i = 0; i < (int)(Quaternion.Angle(Quaternion.identity, iteration.RotationBetweenHandles) / sampleAngle); i++)
                    test *= iteration.SampleQuaternion;
                */

                Debug.Log(string.Format("Original: {0} Target: {1} Test: {2} Angle: {3}", iteration.TargetsHandle.rotation.ToNormalString(), iteration.ReferencesHandle.rotation.ToNormalString(), test.ToNormalString(), Quaternion.Angle(test, iteration.ReferencesHandle.rotation)));

                iteration.IKGoal.rotation *= iteration.SampleQuaternion;
            }

            return iteration;
        }

        private GradientDescentIteration CalculateGradient(
            GradientDescentIteration iteration,
            float sampleDistance,
            float sampleAngle,
            float distanceLearningRate,
            float rotationLearningRate)
        {
            iteration.NewPosition = iteration.IKGoal.position;

            iteration.NewRotation = iteration.IKGoal.rotation;


            iteration.NewDistanceBetweenHandles = iteration.ReferencesHandle.position - iteration.TargetsHandle.position;

            iteration.NewDistance = iteration.NewDistanceBetweenHandles.magnitude;


            iteration.NewRotationBetweenHandles = iteration.TargetsHandle.rotation * Quaternion.Inverse(iteration.ReferencesHandle.rotation);

            iteration.NewAngle = Quaternion.Angle(Quaternion.identity, iteration.NewRotationBetweenHandles);


            if (!iteration.DistanceSatisfying)
            {
                iteration.DistanceGradient = (iteration.NewDistance - iteration.CurrentDistance) / sampleDistance;

                iteration.DistanceVectorGradient = iteration.DistanceBetweenHandles.normalized * iteration.DistanceGradient * distanceLearningRate;

                iteration.IKGoal.position = iteration.CurrentPosition - iteration.DistanceVectorGradient;
            }

            if (!iteration.AngleSatisfying)
            {
                iteration.RotationGradient = (iteration.NewAngle - iteration.CurrentAngle) / sampleAngle;

                iteration.RotationQuaternionGradient = Quaternion.Slerp(
                    Quaternion.identity,
                    iteration.RotationBetweenHandles,
                    (iteration.RotationGradient * rotationLearningRate) / Quaternion.Angle(Quaternion.identity, iteration.RotationBetweenHandles));

                iteration.IKGoal.rotation = iteration.CurrentRotation * Quaternion.Inverse(iteration.RotationQuaternionGradient);
            }

            return iteration;
        }

        public struct GradientDescentIteration
        {
            public Transform IKGoal;
            public Transform ReferencesHandle;
            public Transform TargetsHandle;

            public Vector3 CurrentPosition;
            public Quaternion CurrentRotation;

            public Vector3 DistanceBetweenHandles;
            public float CurrentDistance;
            public bool DistanceSatisfying;

            public Quaternion RotationBetweenHandles;
            public float CurrentAngle;
            public bool AngleSatisfying;

            public Vector3 SampleVector;
            public Quaternion SampleQuaternion;

            public Vector3 NewPosition;
            public Quaternion NewRotation;

            public Vector3 NewDistanceBetweenHandles;
            public float NewDistance;

            public Quaternion NewRotationBetweenHandles;
            public float NewAngle;

            public float DistanceGradient;
            public Vector3 DistanceVectorGradient;

            public float RotationGradient;
            public Quaternion RotationQuaternionGradient;

            public Vector3 PositionWithGradient;
            public Quaternion RotationWithGradient;
        }

        private void AdjustAnimationToReference()
        {
            #region Select target's animation

            var targetsClip = targetsAnimator.runtimeAnimatorController.animationClips[targetsAnimationSelected];

            var targetsClipName = targetsClip.name;

            var targetsClipHash = Animator.StringToHash(targetsClipName);

            #endregion

            #region Select reference's animation

            var referencesClip = referencesAnimator.runtimeAnimatorController.animationClips[targetsAnimationSelected];

            var referencesClipName = referencesClip.name;

            var referencesClipHash = Animator.StringToHash(referencesClipName);

            #endregion

            #region Create animation clip

            var secondsPerFrame = 1f / targetsClip.frameRate;

            var durationInSeconds = targetsClip.length;

            var durationInFrames = Mathf.RoundToInt(durationInSeconds / secondsPerFrame);

            var animationData = HumanoidAnimationRecorder.PrepareAnimationData(targetsAnimator, targetGameObject, targetsClip);

            #endregion

            targetsAnimator.logWarnings = false;

            var ikHook = targetsAnimator.gameObject.GetComponent<IKHook>();

            if (ikHook == null)
                ikHook = targetsAnimator.gameObject.AddComponent<IKHook>();

            ikHook.animator = targetsAnimator;

            ikHook.leftHandPositionWeight = ikHook.rightHandPositionWeight = ikHook.leftHandRotationWeight = ikHook.rightHandRotationWeight = 1f;

            EditorAnimationPlayer.Activate(
                secondsPerFrame,
                durationInFrames,
                (currentSecond) =>
                {
                    EditorAnimationPlayer.SetAnimationFrame(referencesAnimator, referencesClipHash, currentSecond / durationInSeconds);

                    EditorAnimationPlayer.SetAnimationFrame(targetsAnimator, targetsClipHash, currentSecond / durationInSeconds);

                    HumanoidAnimationRecorder.BakeFrame(animationData, currentSecond);
                },
                () =>
                {
                    if (useTargetsMotionRoot)
                        HumanoidAnimationRecorder.ReplaceMotionRoot(animationData, targetsClip);

                    HumanoidAnimationRecorder.BakeAnimation(animationData, "Assets/Adjusted animation.anim");

                    ikHook.DirectControl = true;

                    EditorAnimationPlayer.SetAnimationFrame(referencesAnimator, referencesClipHash, 0f);

                    EditorAnimationPlayer.SetAnimationFrame(targetsAnimator, targetsClipHash, 0f);

                    return true;
                });
        }
    }
}