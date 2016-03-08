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
		// 一定時間後ロゴ出現        
		yield return new WaitForSeconds(1.0f);
		LogoAppear.SetTrigger("Start");
		yield return new WaitForSeconds(3.0f);
		// コントローラー設定をやっていたらタイトルへ、していなかったらコントローラー設定へ
		// タイトルへ遷移
		if (PlayerPrefs.GetInt("ControllerSetting") == 2)
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
