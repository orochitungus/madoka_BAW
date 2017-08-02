using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utage;
using UniRx;
using UniRx.Triggers;

public class Prologue02 : MonoBehaviour
{
    /// <summary>
	/// カメラ１
	/// </summary>
	public Camera Camera1;

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
    /// スコノシュートのアニメーター
    /// </summary>
    public Animator SconosciutoAnimator;

    /// <summary>
    /// スコノシュート白衣のアニメーター
    /// </summary>
    public Animator SconosciutoWhiteCortAnimator;

    /// <summary>
    /// スコノシュート
    /// </summary>
    public GameObject Sconosciuto;

    /// <summary>
    /// スコノシュート白衣
    /// </summary>
    public GameObject SconosciutoWhiteCort;

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

        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("AudioManager"));
            loadManager.name = "AudioManager";
        }

        // モーションを初期化する
        HomuraAnimator.SetTrigger("neutral");
        SconosciutoWhiteCortAnimator.SetTrigger("neutral");
        SconosciutoAnimator.SetTrigger("fall");

        // スカイボックス変更
        RenderSettings.skybox = Sky;

        // カメラを初期配置
        Camera1.transform.localPosition = new Vector3(-23.10001f, -27.6f, 257.5f);
        Camera1.fieldOfView = 18;
        Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-1.117f,0,0));

		// BGM再生開始
		AudioManager.Instance.PlayBGM("Kankyou Kaze01-1");
	}

    // Use this for initialization
    void Start ()
    {
		StartCoroutine(CoTalk("Prologue02"));
	}
	
	// Update is called once per frame
	void Update ()
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
        iTween.RotateTo(Camera1.gameObject, iTween.Hash("x", 35,"islocal", true, "time", 3.0f));
        yield return new WaitForSeconds(3.1f);

		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

		// カメラをほむらに向ける
		Camera1.transform.localPosition = new Vector3(-24.3f, -104.5f, 376);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Camera1.fieldOfView = 14;
				

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// 音を出してほむらを跪かせる
		AudioManager.Instance.PlaySE("ashioto_jari");
		HomuraAnimator.SetTrigger("kneel");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// スコノシュートをアクティブにする
		Sconosciuto.SetActive(true);

		// スコノシュートを落下させる
		iTween.MoveTo(Sconosciuto, iTween.Hash
		(
			"y", -107.87f,
			"islocal", true,
			"time", 1.0f
		));
		AudioManager.Instance.PlaySE("ashioto_jari");
		AudioManager.Instance.PlayBGM("Fury");
		yield return new WaitForSeconds(1.1f);

		// シーン再生
		engine.ResumeScenario();

		// カメラをスコノに向ける
		Camera1.transform.localPosition = new Vector3(-24.3f, -104.5f, 341);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		Camera1.fieldOfView = 14;
				
		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ほむら立ちあがる
		HomuraAnimator.SetTrigger("standup");

		// シーン再生
		engine.ResumeScenario();

		Camera1.transform.localPosition = new Vector3(-31, -104.5f, 341);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 29.934f, 0));
		Camera1.fieldOfView = 31;
				
		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// スコノを変身させる
		Sconosciuto.transform.position = new Vector3(0, 0, 0);
		SconosciutoWhiteCort.transform.localPosition = new Vector3(-24.2f, -107.87f, 360.65f);
		AudioManager.Instance.PlaySE("87043__runnerpack__weapappear");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ほむら手を出す
		HomuraAnimator.SetTrigger("showgreefseed");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// スコノシュート拒絶する
		SconosciutoWhiteCortAnimator.SetTrigger("pay_an_arm");

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		savingparameter.story = 1;  // ストーリー進行度変更


	}
}
