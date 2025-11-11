using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace FZTools
{
    [Serializable]
    public class FZEasyBoneProxyObjectInfo
    {
        [SerializeField]
        public GameObject installation;
        [SerializeField]
        public HumanBodyBones bone;
    }
}