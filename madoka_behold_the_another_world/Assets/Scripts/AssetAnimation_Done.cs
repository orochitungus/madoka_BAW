using UnityEngine;
using System.Collections;

public class AssetAnimation_Done : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
    // アニメを再生する
    public void PlayAnimation(string animationname)
    {
        this.GetComponent<Animation>().Play(animationname);
    }
}
