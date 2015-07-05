using UnityEngine;
using System.Collections;

/// <summary>
/// カットインのアニメ出現を制御する
/// </summary>
[RequireComponent(typeof(Animator))]
public class CutinAnimation : MonoBehaviour 
{
    public void StartAnimation()
    {
        // Mecanimで設定されたAutoトリガーを有効化
        GetComponent<Animator>().SetTrigger("Auto");
    }


    public void EndAnimation()
    {
        // Mecanimのアニメーションを切る
        GetComponent<Animator>().SetTrigger("End");
    }
	
}
