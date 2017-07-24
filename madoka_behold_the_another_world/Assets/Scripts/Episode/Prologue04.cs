using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class Prologue04 : MonoBehaviour 
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
	/// アルティメットまどかのアニメーター
	/// </summary>
	public Animator UltimateMadoka;

	/// <summary>
	/// まどか顕現時のスパーク
	/// </summary>
	public GameObject HolyBlast;

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
		HomuraAnimator.SetTrigger("float");
		UltimateMadoka.SetTrigger("neutral");

		// スカイボックス変更
		RenderSettings.skybox = Sky;

		// カメラを初期配置
		Camera1.transform.localPosition = new Vector3(-5.089996f, -104.83f, 366.01f);
		Camera1.fieldOfView = 14;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-1.021f, 302.491f, 1.603f));
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk("Prologue04"));

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
		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}
	}
}
