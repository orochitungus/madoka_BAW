using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class Prologue01 : MonoBehaviour 
{
	/// <summary>
	/// カメラ１
	/// </summary>
	public Camera Camera1;

	/// <summary>
	/// カメラ２
	/// </summary>
	public Camera Camera2;

	/// <summary>
	/// ストーリーの進行度
	/// </summary>
	public int Xstory;

	/// <summary>
	/// 宴呼び出し
	/// </summary>
	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine;
	public string scenarioLabel;

	/// <summary>
	/// ほむらのアニメーター
	/// </summary>
	public Animator HomuraAnimator;

	/// <summary>
	/// キュゥべえのアニメーター
	/// </summary>
	public Animator KyubeyAnimator;

	/// <summary>
	/// 制御を受け付けるか否か
	/// </summary>
	public bool IsControllable;

	void Awake()
	{
		// QBとほむらのモーションを初期化
		HomuraAnimator.SetTrigger("neutral");
		KyubeyAnimator.SetTrigger("neutral");

		// Xstory初期化
		Xstory = 0;


	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
