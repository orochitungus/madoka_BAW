using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class Prologue06 : MonoBehaviour
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
	/// ほむら
	/// </summary>
	[SerializeField]
	private Animator Homura;

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
	/// 車椅子 
	/// </summary>
	[SerializeField]
	private Animator WheelChair;

	/// <summary>
	/// さやか・恭介・車椅子をまとめたオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject SayakaKyosuke;

	/// <summary>
	/// スコノシュート
	/// </summary>
	[SerializeField]
	private Animator Sconosciuto;

	/// <summary>
	/// エレベーターの扉左
	/// </summary>
	[SerializeField]
	private Animation EVDoorL;

	/// <summary>
	/// エレベーターの扉右
	/// </summary>
	[SerializeField]
	private Animation EVDoorR;

	/// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	[SerializeField]
	private AdvUguiManager2 Advuguimanager2;

	// Use this for initialization
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

		// ParameterManagerがあるか判定
		if (GameObject.Find("ParameterManager") == null)
		{
			// 無ければ作る
			GameObject parameterManager = (GameObject)Instantiate(Resources.Load("ParameterManager"));
			parameterManager.name = "ParameterManager";
		}

		// モーションを初期化する
		// ほむら
		Homura.SetTrigger("neutral");
		// さやか
		Sayaka.SetTrigger("PushWcStop");
		// 恭介
		Kyosuke.SetTrigger("SitDown");
		// 車椅子
		WheelChair.SetTrigger("Stop");
		// スコノシュート
		Sconosciuto.SetTrigger("neutral");

		// カメラ初期化
		Camera1.transform.localPosition = new Vector3(26.11f, 2.78f, 14.9f);
		Camera1.fieldOfView = 37;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
	}

	// Use this for initialization
	void Start()
	{
		StartCoroutine(CoTalk("Prologue06"));
	}

	// Update is called once per frame
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
		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをさやかたちのそばに移動させる
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("z", 44.39f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(3.1f);
		
		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// 恭介振り向く
		Kyosuke.SetTrigger("SitDownLookBack");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// エレベーターの到着音を鳴らす
		AudioManager.Instance.PlaySE("crrect_answer1");
		yield return new WaitForSeconds(0.2f);
		// エレベーターの扉を開く
		EVDoorL.Play("mitakiharahospital_evdoor_l_open");
		EVDoorR.Play("mitakiharahospital_evdoor_r_open");
		yield return new WaitForSeconds(0.2f);
		// 恭介の顔を戻す
		Kyosuke.SetTrigger("SitDownLookBackRetrun");
		// さやかを歩かせる
		Sayaka.SetTrigger("PushWcWalk");
		// 車椅子を動かす
		WheelChair.SetTrigger("Move");
		// さやか＆恭介を動かす
		iTween.MoveTo(SayakaKyosuke, iTween.Hash("x", -8.0f, "islocal", true, "time", 10.0f));
		// 一定時間待って次へ行く
		yield return new WaitForSeconds(3.1f);

		Camera1.transform.localPosition = new Vector3(26.11f, 2.78f, 14.9f);
		Camera1.fieldOfView = 37;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// スコノシュートにカメラを切り替える
		Camera1.transform.localPosition = new Vector3(27.86f, 2.78f, 5.58f);
		Camera1.fieldOfView = 37;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
		// カメラをスコノシュートに移動させる
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", 0f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(3.1f);

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		// フラグを切り替えて元の場所へ移動させる
		// ストーリー変更
		savingparameter.story = 3;
		// フィールドを見滝原病院56階へ
		savingparameter.nowField = 4;
		savingparameter.beforeField = 7777;
		// 次のシーンへ移動
		FadeManager.Instance.LoadLevel("MitakiharaHospital56F", 1.0f);
	}

}