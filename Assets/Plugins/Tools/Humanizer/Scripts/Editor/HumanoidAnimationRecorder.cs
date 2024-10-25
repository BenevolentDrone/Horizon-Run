using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public static class HumanoidAnimationRecorder
    {
        private static Dictionary<string, string> TraitPropMap = new Dictionary<string, string>
        {
            {"Left Thumb 1 Stretched", "LeftHand.Thumb.1 Stretched"},
            {"Left Thumb Spread", "LeftHand.Thumb.Spread"},
            {"Left Thumb 2 Stretched", "LeftHand.Thumb.2 Stretched"},
            {"Left Thumb 3 Stretched", "LeftHand.Thumb.3 Stretched"},
            {"Left Index 1 Stretched", "LeftHand.Index.1 Stretched"},
            {"Left Index Spread", "LeftHand.Index.Spread"},
            {"Left Index 2 Stretched", "LeftHand.Index.2 Stretched"},
            {"Left Index 3 Stretched", "LeftHand.Index.3 Stretched"},
            {"Left Middle 1 Stretched", "LeftHand.Middle.1 Stretched"},
            {"Left Middle Spread", "LeftHand.Middle.Spread"},
            {"Left Middle 2 Stretched", "LeftHand.Middle.2 Stretched"},
            {"Left Middle 3 Stretched", "LeftHand.Middle.3 Stretched"},
            {"Left Ring 1 Stretched", "LeftHand.Ring.1 Stretched"},
            {"Left Ring Spread", "LeftHand.Ring.Spread"},
            {"Left Ring 2 Stretched", "LeftHand.Ring.2 Stretched"},
            {"Left Ring 3 Stretched", "LeftHand.Ring.3 Stretched"},
            {"Left Little 1 Stretched", "LeftHand.Little.1 Stretched"},
            {"Left Little Spread", "LeftHand.Little.Spread"},
            {"Left Little 2 Stretched", "LeftHand.Little.2 Stretched"},
            {"Left Little 3 Stretched", "LeftHand.Little.3 Stretched"},
            {"Right Thumb 1 Stretched", "RightHand.Thumb.1 Stretched"},
            {"Right Thumb Spread", "RightHand.Thumb.Spread"},
            {"Right Thumb 2 Stretched", "RightHand.Thumb.2 Stretched"},
            {"Right Thumb 3 Stretched", "RightHand.Thumb.3 Stretched"},
            {"Right Index 1 Stretched", "RightHand.Index.1 Stretched"},
            {"Right Index Spread", "RightHand.Index.Spread"},
            {"Right Index 2 Stretched", "RightHand.Index.2 Stretched"},
            {"Right Index 3 Stretched", "RightHand.Index.3 Stretched"},
            {"Right Middle 1 Stretched", "RightHand.Middle.1 Stretched"},
            {"Right Middle Spread", "RightHand.Middle.Spread"},
            {"Right Middle 2 Stretched", "RightHand.Middle.2 Stretched"},
            {"Right Middle 3 Stretched", "RightHand.Middle.3 Stretched"},
            {"Right Ring 1 Stretched", "RightHand.Ring.1 Stretched"},
            {"Right Ring Spread", "RightHand.Ring.Spread"},
            {"Right Ring 2 Stretched", "RightHand.Ring.2 Stretched"},
            {"Right Ring 3 Stretched", "RightHand.Ring.3 Stretched"},
            {"Right Little 1 Stretched", "RightHand.Little.1 Stretched"},
            {"Right Little Spread", "RightHand.Little.Spread"},
            {"Right Little 2 Stretched", "RightHand.Little.2 Stretched"},
            {"Right Little 3 Stretched", "RightHand.Little.3 Stretched"},
        };

        public static AnimationData PrepareAnimationData(Animator animator, GameObject targetGameObject, AnimationClip source)
        {
            return new AnimationData()
            {
                Animator = animator,
                Handler = new HumanPoseHandler(animator.avatar, targetGameObject.transform),
                HumanPose = default(HumanPose),
                Curves = PrepareCurvesDictionary(),
                Result = PrepareResult(source)
            };
        }

        private static Dictionary<string, AnimationCurve> PrepareCurvesDictionary()
        {
            var curves = new Dictionary<string, AnimationCurve>();

            curves.Add("RootT.x", new AnimationCurve());
            curves.Add("RootT.y", new AnimationCurve());
            curves.Add("RootT.z", new AnimationCurve());

            curves.Add("LeftFootT.x", new AnimationCurve());
            curves.Add("LeftFootT.y", new AnimationCurve());
            curves.Add("LeftFootT.z", new AnimationCurve());

            curves.Add("RightFootT.x", new AnimationCurve());
            curves.Add("RightFootT.y", new AnimationCurve());
            curves.Add("RightFootT.z", new AnimationCurve());

            curves.Add("RootQ.x", new AnimationCurve());
            curves.Add("RootQ.y", new AnimationCurve());
            curves.Add("RootQ.z", new AnimationCurve());
            curves.Add("RootQ.w", new AnimationCurve());

            curves.Add("LeftFootQ.x", new AnimationCurve());
            curves.Add("LeftFootQ.y", new AnimationCurve());
            curves.Add("LeftFootQ.z", new AnimationCurve());
            curves.Add("LeftFootQ.w", new AnimationCurve());

            curves.Add("RightFootQ.x", new AnimationCurve());
            curves.Add("RightFootQ.y", new AnimationCurve());
            curves.Add("RightFootQ.z", new AnimationCurve());
            curves.Add("RightFootQ.w", new AnimationCurve());

            for (int i = 0; i < HumanTrait.MuscleCount; i++)
            {
                var muscle = HumanTrait.MuscleName[i];

                if (TraitPropMap.ContainsKey(muscle))
                {
                    muscle = TraitPropMap[muscle];
                }

                curves.Add(muscle, new AnimationCurve());
            }

            return curves;
        }

        private static AnimationClip PrepareResult(AnimationClip source)
        {
            var result = new AnimationClip();

            result.legacy = false;

            result.frameRate = source.frameRate;

            AnimationUtility.SetAnimationClipSettings(result, AnimationUtility.GetAnimationClipSettings(source));

            return result;
        }

        public static void BakeFrame(AnimationData animationData, float currentSecond)
        {
            animationData.Handler.GetHumanPose(ref animationData.HumanPose);

            var bodyRootPosition = animationData.Animator.transform.localPosition;

            var bodyRootRotation = animationData.Animator.transform.localRotation;

            var bodyBoneData = new BoneData()
            {
                Position = animationData.HumanPose.bodyPosition,
                Rotation = animationData.HumanPose.bodyRotation
            };

            var leftFootBone = animationData.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);

            var leftFootBoneData = new BoneData()
            {
                Position = leftFootBone.position,
                Rotation = leftFootBone.rotation
            };

            var rightFootBone = animationData.Animator.GetBoneTransform(HumanBodyBones.RightFoot);

            var rightFootBoneData = new BoneData()
            {
                Position = rightFootBone.position,
                Rotation = rightFootBone.rotation
            };

            leftFootBoneData = GetIKGoalBoneData(animationData.Animator.avatar, animationData.Animator.humanScale, AvatarIKGoal.LeftFoot, bodyBoneData, leftFootBoneData);
            rightFootBoneData = GetIKGoalBoneData(animationData.Animator.avatar, animationData.Animator.humanScale, AvatarIKGoal.RightFoot, bodyBoneData, rightFootBoneData);

            var bodyPosition = bodyBoneData.Position;
            var bodyRotation = bodyBoneData.Rotation;

            var leftFootIKPosition = leftFootBoneData.Position;
            var leftFootIKRotation = leftFootBoneData.Rotation;

            var rightFootIKPosition = rightFootBoneData.Position;
            var rightFootIKRotation = rightFootBoneData.Rotation;

            var muscles = new float[animationData.HumanPose.muscles.Length];

            for (int i = 0; i < muscles.Length; i++)
            {
                muscles[i] = animationData.HumanPose.muscles[i];
            }


            // body position
            {
                animationData.Curves["RootT.x"].AddKey(currentSecond, bodyPosition.x);
                animationData.Curves["RootT.y"].AddKey(currentSecond, bodyPosition.y);
                animationData.Curves["RootT.z"].AddKey(currentSecond, bodyPosition.z);

            }
            // Leftfoot position
            {
                animationData.Curves["LeftFootT.x"].AddKey(currentSecond, leftFootIKPosition.x);
                animationData.Curves["LeftFootT.y"].AddKey(currentSecond, leftFootIKPosition.y);
                animationData.Curves["LeftFootT.z"].AddKey(currentSecond, leftFootIKPosition.z);
            }
            // Rightfoot position
            {
                animationData.Curves["RightFootT.x"].AddKey(currentSecond, rightFootIKPosition.x);
                animationData.Curves["RightFootT.y"].AddKey(currentSecond, rightFootIKPosition.y);
                animationData.Curves["RightFootT.z"].AddKey(currentSecond, rightFootIKPosition.z);
            }
            // body rotation
            {
                animationData.Curves["RootQ.x"].AddKey(currentSecond, bodyRotation.x);
                animationData.Curves["RootQ.y"].AddKey(currentSecond, bodyRotation.y);
                animationData.Curves["RootQ.z"].AddKey(currentSecond, bodyRotation.z);
                animationData.Curves["RootQ.w"].AddKey(currentSecond, bodyRotation.w);
            }
            // Leftfoot rotation
            {
                animationData.Curves["LeftFootQ.x"].AddKey(currentSecond, leftFootIKRotation.x);
                animationData.Curves["LeftFootQ.y"].AddKey(currentSecond, leftFootIKRotation.y);
                animationData.Curves["LeftFootQ.z"].AddKey(currentSecond, leftFootIKRotation.z);
                animationData.Curves["LeftFootQ.w"].AddKey(currentSecond, leftFootIKRotation.w);
            }
            // Rightfoot rotation
            {
                animationData.Curves["RightFootQ.x"].AddKey(currentSecond, rightFootIKRotation.x);
                animationData.Curves["RightFootQ.y"].AddKey(currentSecond, rightFootIKRotation.y);
                animationData.Curves["RightFootQ.z"].AddKey(currentSecond, rightFootIKRotation.z);
                animationData.Curves["RightFootQ.w"].AddKey(currentSecond, rightFootIKRotation.w);
            }

            // muscles
            for (int i = 0; i < HumanTrait.MuscleCount; i++)
            {
                var muscle = HumanTrait.MuscleName[i];

                if (TraitPropMap.ContainsKey(muscle))
                {
                    muscle = TraitPropMap[muscle];
                }

                animationData.Curves[muscle].AddKey(currentSecond, muscles[i]);
            }
        }

        private static BoneData GetIKGoalBoneData(Avatar avatar, float humanScale, AvatarIKGoal avatarIKGoal, BoneData animatorBodyPositionRotation, BoneData skeletonBoneData)
        {
            int humanId = (int)HumanIDFromAvatarIKGoal(avatarIKGoal);

            if (humanId == (int)HumanBodyBones.LastBone)
                throw new InvalidOperationException("Invalid human id.");

            MethodInfo methodGetAxisLength = typeof(Avatar).GetMethod("GetAxisLength", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodGetAxisLength == null)
                throw new InvalidOperationException("Cannot find GetAxisLength method.");

            MethodInfo methodGetPostRotation = typeof(Avatar).GetMethod("GetPostRotation", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodGetPostRotation == null)
                throw new InvalidOperationException("Cannot find GetPostRotation method.");

            Quaternion postRotation = (Quaternion)methodGetPostRotation.Invoke(avatar, new object[] { humanId });

            var goalBoneData = new BoneData()
            {
                Position = skeletonBoneData.Position,
                Rotation = skeletonBoneData.Rotation * postRotation
            };

            if (avatarIKGoal == AvatarIKGoal.LeftFoot || avatarIKGoal == AvatarIKGoal.RightFoot)
            {
                // Here you could use animator.leftFeetBottomHeight or animator.rightFeetBottomHeight rather than GetAxisLenght
                // Both are equivalent but GetAxisLength is the generic way and work for all human bone
                float axislength = (float)methodGetAxisLength.Invoke(avatar, new object[] { humanId });

                Vector3 footBottom = new Vector3(axislength, 0, 0);

                goalBoneData.Position += (goalBoneData.Rotation * footBottom);
            }

            // IK goal are in avatar body local space

            Quaternion invRootRotation = Quaternion.Inverse(animatorBodyPositionRotation.Rotation);

            goalBoneData.Position = invRootRotation * (goalBoneData.Position - animatorBodyPositionRotation.Position);

            goalBoneData.Rotation = invRootRotation * goalBoneData.Rotation;

            goalBoneData.Position /= humanScale;

            return goalBoneData;
        }

        private static HumanBodyBones HumanIDFromAvatarIKGoal(AvatarIKGoal avatarIKGoal)
        {
            HumanBodyBones humanId = HumanBodyBones.LastBone;

            switch (avatarIKGoal)
            {
                case AvatarIKGoal.LeftFoot:
                    humanId = HumanBodyBones.LeftFoot;
                    break;

                case AvatarIKGoal.RightFoot:
                    humanId = HumanBodyBones.RightFoot;
                    break;

                case AvatarIKGoal.LeftHand:
                    humanId = HumanBodyBones.LeftHand;
                    break;

                case AvatarIKGoal.RightHand:
                    humanId = HumanBodyBones.RightHand;
                    break;
            }

            return humanId;
        }

        public struct BoneData
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        public static void ReplaceMotionRoot(AnimationData animationData, AnimationClip animationClip)
        {
            var bindings = AnimationUtility.GetCurveBindings(animationClip);

            foreach (var binding in bindings)
            {
                if (binding.propertyName == "RootT.x")
                    animationData.Curves["RootT.x"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootT.y")
                    animationData.Curves["RootT.y"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootT.z")
                    animationData.Curves["RootT.z"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootQ.x")
                    animationData.Curves["RootQ.x"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootQ.y")
                    animationData.Curves["RootQ.y"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootQ.z")
                    animationData.Curves["RootQ.z"] = AnimationUtility.GetEditorCurve(animationClip, binding);

                if (binding.propertyName == "RootQ.w")
                    animationData.Curves["RootQ.w"] = AnimationUtility.GetEditorCurve(animationClip, binding);
            }
        }

        public static void BakeAnimation(AnimationData animationData, string path)
        {
            foreach (var curveName in animationData.Curves.Keys)
                animationData.Result.SetCurve("", typeof(Animator), curveName, animationData.Curves[curveName]);

            AssetDatabase.CreateAsset(animationData.Result, path);
        }
    }

    public struct AnimationData
    {
        public Animator Animator;
        public HumanPoseHandler Handler;
        public HumanPose HumanPose;
        public Dictionary<string, AnimationCurve> Curves;
        public AnimationClip Result;
    }
}
