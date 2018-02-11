using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterManager : SingletonMonoBehaviour<ParameterManager>
{
	/// <summary>
	/// 各マップのシーンファイル名
	/// </summary>
	public MapSceneName Mapscenename;

	/// <summary>
	/// インフォメーションテキスト
	/// </summary>
	public Entity_Information EntityInformation;

	/// <summary>
	/// 全体マップから各フィールドへ移動するときの情報
	/// </summary>
	public MitakiharaCity_MapData MitakiharacityMapdata;

	/// <summary>
	/// 各ステージに移動したときのキャラクターの配置位置の情報
	/// </summary>
	public StagePositionRotData StagepositionrotData;

	private void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
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
