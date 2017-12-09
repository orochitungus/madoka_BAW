using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour 
{
	/// <summary>
	/// BGMの名前
	/// </summary>
	public string BGMName;



	void Awake()
	{

		// AudioManagerがあるか判定
		if (GameObject.Find("AudioManager") == null)
		{
			// なければ作る
			GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
			am.name = "AudioManager";   // このままだと名前にCloneがつくので消しておく
		}		
		// EventSystemがあるか判定
		if (GameObject.Find("EventSystem") == null)
		{
			// 無ければ作る
			GameObject eventSystem = (GameObject)Instantiate(Resources.Load("EventSystem"));
			eventSystem.name = "EventSystem";
		}
		// FadeManagerがあるか判定
		if (GameObject.Find("FadeManager") == null)
		{
			// 無ければ作る
			GameObject fadeManager = (GameObject)Instantiate(Resources.Load("FadeManager"));
			fadeManager.name = "FadeManager";
		}
		// LoadManagerがあるか判定
		if (GameObject.Find("LoadManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("LoadManager"));
			loadManager.name = "LoadManager";
		}
		// PauseManagerがあるか判定
		if (GameObject.Find("PauseManager") == null)
		{
			// 無ければ作る
			GameObject pauseManager = (GameObject)Instantiate(Resources.Load("PauseManager"));
			pauseManager.name = "PauseManager";
		}
		// ControllerManagerがあるか判定
		if (GameObject.Find("ControllerManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
			loadManager.name = "ControllerManager";
		}

		// 現在移動可能な個所をリストアップしてリストにする

		// リストの中で現在の位置にカメラをセットしてインフォメーションと現在位置を表示する

		// BGMを変更する
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// ショットキー、左入力、右入力を受け付ける
		// ショットキーを押す→ポップアップを出してここへの移動の最終確認をとる
		// 左か右を押す→リストの隣の値へ
	}
}
