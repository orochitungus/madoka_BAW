using UnityEngine;
using System.Collections;

public class TitleCanvas : MonoBehaviour 
{
	/// <summary>
	/// TitleCanvasのAnimator.主にステート管理に使う
	/// </summary>
	public Animator TitleCanvasAnimator;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnEnable()
	{
		
	}

	public void AppearDone()
	{
		TitleCanvasAnimator.SetBool("Appear", true);
	}
}
