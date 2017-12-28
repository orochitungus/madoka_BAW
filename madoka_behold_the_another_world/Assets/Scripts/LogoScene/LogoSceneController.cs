using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LogoSceneController : MonoBehaviour 
{    
    /// <summary>
    /// Logo制御を行うアニメーション
    /// </summary>
    public Animator LogoAppear;

    void Awake()
    {
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
        if(GameObject.Find("LoadManager") == null)
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
        // PauseManagerがあるか判定
        //if (GameObject.Find("PauseManager") == null)
        //{
        //    // 無ければ作る
        //    GameObject pauseManager = (GameObject)Instantiate(Resources.Load("PauseManager"));
        //    pauseManager.name = "PauseManager";
        //}
    }

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(AppearCompanyLogo());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// 一定時間後にTungus-Villageの文字列を出し、さらに一定時間後やっていなかったらコントローラー設定に飛ばし、やっていたらタイトルに飛ばす
	/// </summary>
	/// <returns></returns>
	private IEnumerator AppearCompanyLogo()
	{
		// saveフォルダがない場合、PlayerPrefsのキーコンフィグを初期化する
		if (!System.IO.Directory.Exists("save"))
		{
			PlayerPrefs.SetInt("TenKeySetup", 0);
			PlayerPrefs.SetInt("RightStickSetup", 0);
		}
		// 一定時間後ロゴ出現        
		yield return new WaitForSeconds(1.0f);
		LogoAppear.SetTrigger("Start");
		yield return new WaitForSeconds(3.0f);
		// 音関係を初期化（設定されてなければ1で設定）
		if(PlayerPrefs.GetFloat("BGMVolume") == 0)
		{
			PlayerPrefs.SetFloat("BGMVolume",1.0f);
		}
		if(PlayerPrefs.GetFloat("SEVolume") == 0)
		{
			PlayerPrefs.SetFloat("SEVolume", 1.0f);
		}
		if(PlayerPrefs.GetFloat("VoiceVolue") == 0)
		{
			PlayerPrefs.SetFloat("VoiceVolue", 1.0f);
		}

		int x = PlayerPrefs.GetInt("TenKeySetup");
		int y = PlayerPrefs.GetInt("RightStickSetup");

		Debug.Log("x:" + x);
		Debug.Log("y:" + y);

		// コントローラー設定をやっていたらタイトルへ、していなかったらコントローラー設定へ
		// タイトルへ遷移
		if (PlayerPrefs.GetInt("TenKeySetup") != 0 && PlayerPrefs.GetInt("RightStickSetup") != 0)
		{
			FadeManager.Instance.LoadLevel("title", 1.0f);
		}
		// キーコンフィグへ遷移
		else
		{
			FadeManager.Instance.LoadLevel("KeyConfig", 1.0f);
		}
        
	}
}
