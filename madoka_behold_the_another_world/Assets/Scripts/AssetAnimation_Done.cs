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
    // �A�j�����Đ�����
    public void PlayAnimation(string animationname)
    {
        this.animation.Play(animationname);
    }
}
