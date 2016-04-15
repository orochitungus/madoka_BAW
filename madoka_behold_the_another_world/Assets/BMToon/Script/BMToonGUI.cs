#if UNITY_EDITOR

using UnityEngine;
using System;

namespace UnityEditor
{
    public class BMToonGUI : ShaderGUI
    {

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,       // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Add // Physically plausible transparency mode, implemented as alpha pre-multiply
        }

        MaterialEditor m_MaterialEditor;
        bool m_FirstTimeApply = true;
        static private string verNo = "ver1.0.0";

        public float fCutoff = 0.5f;

        MaterialProperty blendMode = null;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);

            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;

            ShaderPropertiesGUI(material);
            base.OnGUI(materialEditor, props);

            if (m_FirstTimeApply)
            {
                m_FirstTimeApply = false;
            }
        }
        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUILayout.LabelField("-BMToon-", verNo);
            EditorGUILayout.Space();

            var mode = (BlendMode)blendMode.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = (BlendMode)EditorGUILayout.Popup("Rendering Mode", (int)mode, Enum.GetNames(typeof(BlendMode)));
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float)mode;
                foreach (var obj in blendMode.targets)
                {
                    SetupMaterialWithBlendMode((Material)obj, (BlendMode)material.GetFloat("_Mode"));
                }
            }

            EditorGUI.BeginChangeCheck();

            switch ((BlendMode)material.GetFloat("_Mode"))
            {
                case BlendMode.Cutout:
                    fCutoff = material.GetFloat("_Cutoff");
                    fCutoff = EditorGUILayout.Slider("CutOff", fCutoff, 0,1);// _Cutoff("アルファカット値", Range(0.0, 1.0)) = 0.5
                    break;
            }

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                //SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
                
                foreach (var obj in blendMode.targets)
                {
                    SetupMaterialWithBlendMode((Material)obj, (BlendMode)material.GetFloat("_Mode"));
                }

            }
        }

        public void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.SetFloat("_Cutoff",fCutoff);
                    material.DisableKeyword("_ALPHATEST2_ON");
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case BlendMode.Add:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }
    }
}
#endif