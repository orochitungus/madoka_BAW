using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;


public class Prologue08 : MonoBehaviour 
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
	/// 詢子
	/// </summary>
	[SerializeField]
	private Animator Junko;

	/// <summary>
	/// ミヒャエラ
	/// </summary>
	[SerializeField]
	private Animator Michaela;

	/// <summary>
	/// 宴の表示部分
	/// </summary>
	[SerializeField]
	private AdvUguiManager2 Advuguimanager2;

	private void Awake()
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

		// 各キャラクターのモーション初期化
		// ほむら
		Homura.Play("homura_carrysoulgem");
		// 詢子
		Junko.Play("junko_idle");
		// ミヒャエラ
		Michaela.Play("michaela1_idle");

		// キャラクター初期配置
		// ほむら
		Homura.gameObject.transform.localPosition = new Vector3(106.554f, 0, 78.23817f);
		Homura.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 0);

		// 詢子
		Junko.gameObject.transform.localPosition = new Vector3(126.38f,0, 77.27f);
		Junko.gameObject.transform.localRotation = Quaternion.Euler(0,270,0);

		// ミヒャエラ
		Michaela.gameObject.transform.localPosition = new Vector3(126.66f, 9.18f, 77.31f);
		Michaela.gameObject.transform.localRotation = Quaternion.Euler(0, 270, 0);

		// カメラ初期化
		Camera1.transform.localPosition = new Vector3(113.1f, 2.6f, 73.3f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, -52.795f,0));
		Camera1.fieldOfView = 30;
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk("Prologue08"));
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

		// カメラを詢子に向ける
		Camera1.transform.localPosition = new Vector3(114.77f, 2.84f, 73.89f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-7.670001f, 70.622f, -0.278f));
		Camera1.fieldOfView = 43;

		// BGM再生
		AudioManager.Instance.PlayBGM("Renegades");

		// シーン再生
		engine.ResumeScenario();

		// ミヒャエラ下りてくる
		iTween.MoveTo(Michaela.gameObject, iTween.Hash("y", 4.3f, "islocal", true, "time", 0.5f));

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ミヒャエラ詠唱ポーズとる
		Michaela.Play("michaela1_chant");
		AudioManager.Instance.PlaySE("SoulGem");
		Junko.Play("junko_holdhead");
		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをほむらに向ける
		Camera1.transform.localPosition = new Vector3(113.1f, 2.6f, 73.3f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, -52.795f, 0));
		Camera1.fieldOfView = 30;

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// 詢子を結界前へ移動
		Junko.gameObject.transform.localPosition = new Vector3(84, -23.92f, 336.3f);
		Junko.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

		// ほむらを結界前へ移動
		Homura.gameObject.transform.localPosition = new Vector3(83.86f, -24, 326.47f);
		Homura.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

		// カメラを結界前へ移動
		Camera1.transform.localPosition = new Vector3(83.84f, -18.5f, 334.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-7.670001f, 0, -0.278f));
		Camera1.fieldOfView = 30;

		// カメラを後ろへ引く
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", 85.62f, "y", -21.84f, "z", 309.87f, "islocal", true, "time", 3.0f));

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラを移動
		Camera1.transform.localPosition = new Vector3(82.1f, -22.2f, 352.2f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-3.019f, 180, -0.276f));
		Camera1.fieldOfView = 30;

		// ほむらを抜き打ちのポーズにして突進
		Homura.Play("homura_slash");
		iTween.MoveTo(Homura.gameObject, iTween.Hash("z", 340.9f, "islocal", true, "time",1.0f));

		// SE再生
		AudioManager.Instance.PlaySE("sen_ge_panchi10");

		// 詢子をダウンさせる
		Junko.Play("junko_down");

		// 上記のイベントが終わるまで待つ
		yield return new WaitForSeconds(2.0f);

		// カメラを再度移動
		Camera1.transform.localPosition = new Vector3(83.6f, -21.97f, 326.2f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-3.019f, 0, -0.276f));
		Camera1.fieldOfView = 30;

		// ほむらをもとのポーズに戻す
		Homura.Play("homura_neutral");

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		// フラグ変更
		savingparameter.story = 6;

		// 元の場所に飛ばす
		// フィールドを芸術家の魔女の結界１へ
		//savingparameter.nowField = 10;
		//savingparameter.beforeField = 902;
		FadeManager.Instance.LoadLevel("title", 1.0f);
	}
}
