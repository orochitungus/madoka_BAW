// ビルド時は切ること

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AnimationPostprocessor : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        // don't do anything when animation doesn't exist.
        if (go.animation.GetClipCount() == 0)
            return;

        // backup all duplicated clips for SetAnimationClips() at the end.
        List<AnimationClip> clips = new List<AnimationClip>();

        // duplicate all animation clips
        foreach (AnimationState a in go.animation)
        {
            // copy clip data from original
            AnimationClip copyedClip = new AnimationClip();
            foreach (var c in AnimationUtility.GetAllCurves(a.clip, true))
                AnimationUtility.SetEditorCurve(copyedClip, c.path, c.type, c.propertyName, c.curve);

            // create duplicated animation asset
            string fbxPath = AssetDatabase.GetAssetPath(a.clip);
            string anmPath = fbxPath.Substring(0, fbxPath.LastIndexOf('/') + 1) + a.name + "_copy.anim";
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
        AnimationUtility.SetAnimationClips(go.animation, clips.ToArray());

        // set default clip
        go.animation.clip = clips[0];
    }
}
