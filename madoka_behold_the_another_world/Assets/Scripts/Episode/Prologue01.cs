using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utage;
using UniRx;
using UniRx.Triggers;

public class Prologue01 : MonoBehaviour 
{
	/// <summary>
	/// カメラ１
	/// </summary>
	public Camera Camera1;

	/// <summary>
	/// ストーリーの進行度
	/// </summary>
	public int Xstory;

	/// <summary>
	/// 宴呼び出し
	/// </summary>
	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine;

	/// <summary>
	/// ほむらのアニメーター
	/// </summary>
	public Animator HomuraAnimator;

	/// <summary>
	/// キュゥべえのアニメーター
	/// </summary>
	public Animator KyubeyAnimator;

	/// <summary>
	/// 魔獣のアニメーター
	/// </summary>
	public Animator MajyuAnimator;

	/// <summary>
	/// 魔獣
	/// </summary>
	public GameObject Majyu1;

	// 魔獣出現時のSE
	public AudioClip Appearmajyu_se;

	/// <summary>
	/// スカイボックスのマテリアル
	/// </summary>
	public Material Sky;


	/// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	public AdvUguiManager2 Advuguimanager2;

	
	void Awake()
	{
        // ADVEngine呼び出し
        engine = GameObject.Find("AdvEngine").GetComponent<AdvEngine>();

        // ControllerManagerがあるか判定
        if (GameObject.Find("ControllerManager") == null)
        {
            // 無ければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
            loadManager.name = "ControllerManager";
        }


        // QBとほむらのモーションを初期化
        HomuraAnimator.SetTrigger("neutral");
		KyubeyAnimator.SetTrigger("neutral");

		// Xstory初期化
		Xstory = 0;

        // スカイボックス変更
        RenderSettings.skybox = Sky;

        // カメラを初期配置
        Camera1.transform.localPosition = new Vector3(-35, -103, -650);
        Camera1.fieldOfView = 60;
        Camera1.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    // Use this for initialization
    void Start () 
	{
		StartCoroutine(CoTalk("Prologue01"));
	}

	void Update()
	{
		// ショットキーの入力受け取り
		if (ControllerManager.Instance.Shot)
		{
			Advuguimanager2.OnInput();
		}
	}
	
	
	/// <summary>
	/// 「宴」のシナリオを再生する
	/// </summary>
	/// <param name="scenarioLabel"></param>
	/// <returns></returns>
	private IEnumerator CoTalk(string scenarioLabel)
    {
		// カメラを移動させる
		iTween.MoveTo(Camera1.gameObject, new Vector3(254.3377f, 4.437881f, 128.03f), 3.0f);

		yield return new WaitForSeconds(3.0f);
		// カメラをほむらに向ける
		Camera1.transform.localPosition = new Vector3(-34.6f, -105.2179f, -670.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 44;

		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

        //「宴」のポーズ終了待ち
        while (!Engine.IsPausingScenario)
        {
            yield return 0;			
		}

		// カメラをQBに向ける
		Camera1.transform.localPosition = new Vector3(-36.32f, -106.53f, -673.81f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 29;
		// キュゥべえの向きを変える
		KyubeyAnimator.SetTrigger("show_ru");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをほむらに向ける
		Camera1.transform.localPosition = new Vector3(-34.6f, -105.2179f, -670.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 44;
		// ほむらの向きを変える
		HomuraAnimator.SetTrigger("see_lu");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをQBに向ける
		Camera1.transform.localPosition = new Vector3(-36.32f, -106.53f, -673.81f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 29;
		// キュゥべえの向きを変える
		KyubeyAnimator.SetTrigger("show_ru_back");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラを二人の後に向ける
		Camera1.transform.localPosition = new Vector3(-35, -103, -690);
		Camera1.fieldOfView = 60;
		Camera1.transform.localRotation = Quaternion.Euler(Vector3.zero);

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		// 魔獣出現
		Majyu1.gameObject.SetActive(true);
		AudioSource.PlayClipAtPoint(Appearmajyu_se, Majyu1.transform.position);
		Camera1.transform.localPosition = new Vector3(-37.60001f, -105.7f, -621.8f);
		Camera1.fieldOfView = 25;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-10,0,0));

		// シナリオ2つめ開始
		Engine.JumpScenario("Prologue01_2");

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}
		Camera1.transform.localPosition = new Vector3(-34.6f, -105.2179f, -670.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 44;

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}
	}

}
