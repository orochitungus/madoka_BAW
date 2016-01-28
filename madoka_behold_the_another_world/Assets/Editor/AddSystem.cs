// ビルド時は切ること

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;

#pragma warning disable 0618
public class AnimationPostprocessor : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        // don't do anything when animation doesn't exist.
        if (go.GetComponent<Animation>().GetClipCount() == 0)
            return;

        // backup all duplicated clips for SetAnimationClips() at the end.
        List<AnimationClip> clips = new List<AnimationClip>();

        // duplicate all animation clips
        foreach (AnimationState a in go.GetComponent<Animation>())
        {
            // copy clip data from original
            AnimationClip copyedClip = new AnimationClip();
            foreach (var c in AnimationUtility.GetAllCurves(a.clip, true))
                AnimationUtility.SetEditorCurve(copyedClip, c.path, c.type, c.propertyName, c.curve);

            // create duplicated animation asset
           // string fbxPath = AssetDatabase.GetAssetPath(a.clip);
            //string anmPath = fbxPath.Substring(0, fbxPath.LastIndexOf('/') + 1) + a.name + "_copy.anim";
            string anmPath = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + a.name + "_copy.anim";
            if (File.Exists(anmPath))
            {
                // copy animation events if exists
                AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(anmPath, typeof(AnimationClip)) as AnimationClip;
                AnimationUtility.SetAnimationEvents(copyedClip, AnimationUtility.GetAnimationEvents(oldClip));
            }
            AssetDatabase.CreateAsset(copyedClip, anmPath);

            clips.Add(copyedClip);
        }
        AssetDatabase.Refresh();

        // swap animation list to duplicated clips.
        AnimationUtility.SetAnimationClips(go.GetComponent<Animation>(), clips.ToArray());

        // set default clip
        go.GetComponent<Animation>().clip = clips[0];
    }
}

//public class CurvesTransferer
//{

//    const string duplicatePostfix = "_copy";

//    [MenuItem("Assets/Transfer Clip Curves to Copy")]
//    static void CopyCurvesToDuplicate()
//    {
//        // Get selected AnimationClip
//        AnimationClip imported = Selection.activeObject as AnimationClip;
//        if (imported == null)
//        {
//            Debug.Log("Selected object is not an AnimationClip");
//            return;
//        }

//        // Find path of copy
//        string importedPath = AssetDatabase.GetAssetPath(imported);
//        string copyPath = importedPath.Substring(0, importedPath.LastIndexOf("/"));
//        copyPath += "/" + imported.name + duplicatePostfix + ".anim";

//        // Get copy AnimationClip
//        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
//        if (copy == null)
//        {
//            Debug.Log("No copy found at " + copyPath);
//            return;
//        }

//        // Copy curves from imported to copy
//        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(imported, true);
//        for (int i = 0; i < curveDatas.Length; i++)
//        {
//            AnimationUtility.SetEditorCurve(
//                copy,
//                curveDatas[i].path,
//                curveDatas[i].type,
//                curveDatas[i].propertyName,
//                curveDatas[i].curve
//            );
//        }

//        Debug.Log("Copying curves into " + copy.name + " is done");
//    }
//}

public class CurvesTransferer
{
    const string duplicatePostfix = "_copy";

    static void CopyClip(string importedPath, string copyPath)
    {
        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        AnimationClip newClip = new AnimationClip();
        newClip.name = src.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, copyPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Transfer Clip Curves to Copy")]
    static void CopyCurvesToDuplicate()
    {
        // Get selected AnimationClip
        AnimationClip imported = Selection.activeObject as AnimationClip;
        if (imported == null)
        {
            Debug.Log("Selected object is not an AnimationClip");
            return;
        }

        // Find path of copy
        string importedPath = AssetDatabase.GetAssetPath(imported);
        string copyPath = importedPath.Substring(0, importedPath.LastIndexOf("/"));
        copyPath += "/" + imported.name + duplicatePostfix + ".anim";

        CopyClip(importedPath, copyPath);

        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        if (copy == null)
        {
            Debug.Log("No copy found at " + copyPath);
            return;
        }
        // Copy curves from imported to copy
        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(imported, true);
        for (int i = 0; i < curveDatas.Length; i++)
        {
            AnimationUtility.SetEditorCurve(
                copy,
                curveDatas[i].path,
                curveDatas[i].type,
                curveDatas[i].propertyName,
                curveDatas[i].curve
            );
        }

        Debug.Log("Copying curves into " + copy.name + " is done");
    }
}
