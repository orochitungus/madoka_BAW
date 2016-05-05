using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class TitleController : MonoBehaviour 
{
	/// <summary>
	/// タイトル画面描画キャンバス
	/// </summary>
	public Canvas TitleCanvas;

	/// <summary>
	/// オープニング描画用カメラのAnimator
	/// </summary>	
	public Animator OPCamera;

	/// <summary>
	/// TitleCanvasのAnimator.主にステート管理に使う
	/// </summary>
	public Animator TitleCanvasAnimator;

	/// <summary>
	/// Mainmenuの制御
	/// </summary>
	public MainmenuController Mainmanucontroller;

	

	void Awake()
	{
		// FPS設定
		Application.targetFrameRate = 60;

		// AudioManagerがあるか判定
		if (GameObject.Find("AudioManager") == null)
		{
			// なければ作る
			GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
			am.name = "AudioManager";   // このままだと名前にAudioManagerがつくので消しておく
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
		// ControllerManagerがあるか判定
		if (GameObject.Find("ControllerManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
			loadManager.name = "ControllerManager";
		}
	}

	// Use this for initialization
	void Start () 
	{
		// BGMを再生
		AudioManager.Instance.PlayBGM("Snow");
		// Animatorを起動
		OPCamera.SetBool("Standby", true);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	

	/// <summary>
	/// Canvasがアクティブになったときにタイトル画面出現アニメを再生する
	/// </summary>
	public void CanvasAnimeSetup()
	{
		TitleCanvasAnimator.SetBool("Apeear", true);
	}
		
}
