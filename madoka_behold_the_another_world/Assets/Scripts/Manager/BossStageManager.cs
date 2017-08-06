using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 規定のボスキャラを倒したら次のシーンに遷移させる
/// </summary>
public class BossStageManager : MonoBehaviour 
{

	/// <summary>
	/// ボスオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Boss;

	/// <summary>
	/// 次に遷移するシーンの名前
	/// </summary>
	[SerializeField]
	private string NextSceneName;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// ボスが消滅したら次のシーンへ遷移する
		if(Boss == null)
		{
			FadeManager.Instance.LoadLevel(NextSceneName, 1.0f);
		}	
	}
}
