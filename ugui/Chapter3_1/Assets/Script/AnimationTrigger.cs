using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour 
{	
	void Update () 
    {
	    // 左クリックでこのオブジェクトに対しStartAnimationメソッドがコールされる
        if (Input.GetMouseButtonDown(0))
        {
            SendMessage("StartAnimation");
        }
	}
}
