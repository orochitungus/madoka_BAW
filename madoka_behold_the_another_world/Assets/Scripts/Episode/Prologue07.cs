using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class Prologue07 : MonoBehaviour 
{
	/// <summary>
	/// カメラ１
	/// </summary>
	[SerializeField]
	private Camera Camera1;

	/// <summary>
	/// カメラ２（グレースケール用イメージエフェクト付き）
	/// </summary>
	[SerializeField]
	private Camera Camera2;

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
	/// ほむら魔法少女
	/// </summary>
	[SerializeField]
	private Animator HomuraMagica;

	/// <summary>
	/// まどか
	/// </summary>
	[SerializeField]
	private Animator Madoka;

	/// <summary>
	/// エイミー
	/// </summary>
	[SerializeField]
	private Animator Amy;

	/// <summary>
	/// キュウべえ
	/// </summary>
	[SerializeField]
	private Animator Kyubey;

	/// <summary>
	/// 車
	/// </summary>
	[SerializeField]
	private GameObject Car;

	/// <summary>
	/// 変身エフェクト
	/// </summary>
	[SerializeField]
	private GameObject HolyBlast;


	/// <summary>
	/// 宴の表示部分を呼び出す
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

		// 各キャラクターのモーション初期化
		// ほむら
		Homura.SetTrigger("neutral");
		// ほむら魔法少女
		HomuraMagica.SetTrigger("neutral");
		// まどか
		Madoka.SetTrigger("Squat");
		// エイミー
		Amy.SetTrigger("Idle");
		// キュウべえ
		Kyubey.SetTrigger("neutral");

		// カメラ初期化
		Camera1.transform.localPosition = new Vector3(10.74f, 5.36f, 12.6f);
		Camera1.fieldOfView = 30;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));
		Camera2.enabled = false;

		// BGM変更
		AudioManager.Instance.PlayBGM("The parting Day");
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk("Prologue07"));
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

		// カメラをまどかとエイミーのそばに寄せる
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", 32.32f,"y", 3.98f, "z", 12.79f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(3.1f);

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// エイミー、車道に向かって走っていく
		// エイミーを振り向かせる
		iTween.RotateTo(Amy.gameObject, iTween.Hash("y", 0.0f, "islocal", true, "time", 0.5f));
		// エイミーを走らせる
		Amy.SetTrigger("Run");
		// エイミーを道路まで移動させる
		iTween.MoveTo(Amy.gameObject, iTween.Hash("z", 24.13f, "islocal", true, "time", 12.0f));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをほむらに戻す
		Camera1.transform.localPosition = new Vector3(12.7f, 5.36f, 14.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをまどかに向ける
		Camera1.transform.localPosition = new Vector3(33, 5.36f, 13.5f);
		
	
		// まどかを走らせる
		Madoka.SetTrigger("Run");
		iTween.MoveTo(Madoka.gameObject, iTween.Hash("z", 24.13f, "islocal", true, "time", 12.0f));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラを車に向ける
		Camera1.transform.localPosition = new Vector3(10.74f, 5.36f, 12.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 80, 0));

		// 車をカメラ方向に移動させる
		iTween.MoveTo(Car.gameObject, iTween.Hash("x", 50.0f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(1.1f);

		// カメラをほむらに戻す
		Camera1.transform.localPosition = new Vector3(12.7f, 5.36f, 14.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ほむらを変身させる
		AudioManager.Instance.PlaySE("SoulGem");
		HolyBlast.SetActive(true);
		HomuraMagica.gameObject.SetActive(true);
		Homura.gameObject.SetActive(false);

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// 画面を灰色にする(イメージエフェクトはオンオフできないっぽいのでカメラ２と１を切り替える）
		Camera1.enabled = false;
		Camera2.enabled = true;
		Camera2.transform.localPosition = new Vector3(12.7f, 5.36f, 14.6f);
		Camera2.fieldOfView = 30;
		Camera2.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));
		
		AudioManager.Instance.PlaySE("SoulGem");

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// まどかとエイミーをもとの場所へ戻す(itweenの命令が残っているのでitweenで戻す）
		iTween.MoveTo(Madoka.gameObject, iTween.Hash("z", 6.81f, "islocal", true, "time", 0.0f));
		Madoka.SetTrigger("Idle");
		iTween.MoveTo(Amy.gameObject, iTween.Hash("z", 9.28f, "islocal", true, "time", 0.0f));
		Amy.gameObject.transform.rotation = Quaternion.Euler(0,180,0);
		Amy.SetTrigger("Idle");

		// カメラをまどかとエイミーに向ける
		Camera1.enabled = true;
		Camera2.enabled = false;
		Camera1.transform.localPosition = new Vector3(15.67f, 4.64f, 15.26f);
		Camera1.fieldOfView = 30;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 125.871f, 0));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをほむらに戻す
		Camera1.transform.localPosition = new Vector3(12.7f, 5.36f, 14.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ほむら頭を押さえる
		HomuraMagica.SetTrigger("hold_the_head");

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをキュウべえに向ける
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", 15.67f, "y", 21.08f, "z", 15.26f, "islocal", true, "time", 3.0f));
		iTween.RotateTo(Camera1.gameObject, iTween.Hash("y", 93.089f, "islocal", true, "time", 3.0f));
		Camera1.fieldOfView = 6;
		yield return new WaitForSeconds(3.1f);

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをまどかに戻す
		Camera1.transform.localPosition = new Vector3(15.67f, 4.64f, 15.26f);
		Camera1.fieldOfView = 30;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 125.871f, 0));
		Madoka.SetTrigger("Squat");

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// カメラをほむらに戻す
		Camera1.transform.localPosition = new Vector3(12.7f, 5.36f, 14.6f);
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 240, 0));

		//「宴」のシナリオを呼び出す
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		// フラグ変更
		savingparameter.story = 5;

		// タイトルに飛ばす
		FadeManager.Instance.LoadLevel("title", 1.0f);
	}
}
