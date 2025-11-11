using System.Diagnostics;
using System.Collections.Specialized;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.Dynamics;
using EUI = FZTools.EditorUtils.UI;
using ELayout = FZTools.EditorUtils.Layout;
using static FZTools.AvatarUtils;

using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;

namespace FZTools
{
    public class FZEasyBoneProxy : EditorWindow
    {
        [SerializeField] GameObject targetAvatar = null;
        [SerializeField] List<FZEasyBoneProxyObjectInfo> objAndBones = new List<FZEasyBoneProxyObjectInfo>();
        VRCAvatarDescriptor AvatarDescriptor => targetAvatar != null ? targetAvatar.GetComponent<VRCAvatarDescriptor>() : null;
        string TargetAvatarName => targetAvatar?.gameObject?.name;
        Vector2 scrollPos;
        

        [MenuItem("FZTools/EasyBoneProxy(β)")]
        private static void OpenWindow()
        {
            var window = GetWindow<FZEasyBoneProxy>();
            window.titleContent = new GUIContent("EasyBoneProxy(β)");
        }

        private void OnGUI()
        {
            ELayout.Horizontal(() =>
            {
                EUI.Space();
                ELayout.Vertical(() =>
                {
                    ELayout.Scroll(ref scrollPos, () =>
                    {
                        EUI.Space(2);
                        EUI.Label("Target Avatar");
                        EUI.ChangeCheck(
                            () => EUI.ObjectField<GameObject>(ref targetAvatar),
                            () =>
                            {
                            });
                        EUI.Space();

                        var serializedObject = new SerializedObject(this);
                        serializedObject.Update();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("objAndBones"), true);
                        serializedObject.ApplyModifiedProperties();
                        EUI.Space(2);
                        EUI.Space(2);
                        EUI.Button("自動位置合わせ", AutoPositioning);
                        EUI.Space(2);
                        EUI.Button("実行", PutOn);
                        EUI.Space(2);
                    });
                });
                EUI.Space();
            });

        }

        private void PutOn()
        {
            Animator animator = targetAvatar.GetComponent<Animator>();
            if (animator == null) return;

            objAndBones.ForEach(info =>
            {
                info.installation.transform.SetParent(targetAvatar.transform);
                
                var boneTransform = animator.GetBoneTransform(info.bone);
                if (boneTransform == null) return;

                // MA Bone Proxyの追加
                var maBoneProxy = info.installation.GetComponent<ModularAvatarBoneProxy>();
                if (maBoneProxy == null)
                {
                    maBoneProxy = info.installation.AddComponent<ModularAvatarBoneProxy>();
                }
                maBoneProxy.target = boneTransform;
                maBoneProxy.attachmentMode = BoneProxyAttachmentMode.AsChildKeepWorldPose;
            });
        }
        
        private void AutoPositioning()
        {
            Animator animator = targetAvatar.GetComponent<Animator>();
            if (animator == null) return;

            objAndBones.ForEach(info =>
            {
                var boneTransform = animator.GetBoneTransform(info.bone);
                if (boneTransform == null) return;

                info.installation.transform.position = boneTransform.position;
            });
        }
    }
}