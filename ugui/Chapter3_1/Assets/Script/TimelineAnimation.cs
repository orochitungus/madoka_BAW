using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TimelineAnimation : MonoBehaviour 
{
    void StartAnimation()
    {
        // Mecanimで設定されたisClickトリガーを有効化
        GetComponent<Animator>().SetTrigger("isClick");
    }
}
