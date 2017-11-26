using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class ExEpisode01 : MonoBehaviour 
{
	/// <summary>
	/// カメラ１
	/// </summary>
	[SerializeField]
	private Camera Camera1;

	/// <summary>
	/// 宴呼び出し
	/// </summary>
	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	[SerializeField]
	private AdvEngine engine;

	/// <summary>
	/// さやか
	/// </summary>
	[SerializeField]
	private Animator Sayaka;

	/// <summary>
	/// 恭介
	/// </summary>
	[SerializeField]
	private Animator Kyosuke;

	/// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	[SerializeField]
	private AdvUguiManager2 Advuguimanager2;


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
		// さやか
		Sayaka.SetTrigger("Idle");
		// 恭介
		Kyosuke.SetTrigger("SitDown");
		// カメラの位置を初期化
		Camera1.transform.localPosition = new Vector3(-15.639f, 3.09f, -1.835f);
		Camera1.fieldOfView = 30;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

		// BGM再生開始
		AudioManager.Instance.PlayBGM("Kaorimade_rintoshita");
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk("EXEpisode1"));
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
		// カメラをさやかたちのそばに移動させる
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", -2.16f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(3.1f);

		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// さやかを恭介のほうに向ける
		Sayaka.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, -30, 0));
		// 恭介の顔をさやかのほうに向ける
		Kyosuke.SetTrigger("SitDownLookBack");

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}
		// アイテムを入手させる
		int itemKind = 0;
		savingparameter.SetItemNum(itemKind, savingparameter.GetItemNum(itemKind) + 10);
		// 見滝原病院106階へ飛ばす
		savingparameter.nowField = 5;
		savingparameter.beforeField = 1102;
		savingparameter.story = 4;
		savingparameter.exEpisode[0] = true;
		FadeManager.Instance.LoadLevel("MitakiharaHospital106F", 1.0f);
		
	}
}
