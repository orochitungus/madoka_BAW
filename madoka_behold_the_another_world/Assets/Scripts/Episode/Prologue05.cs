using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;


public class Prologue05 : MonoBehaviour 
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
	/// 魔法少女のほむら
	/// </summary>
	[SerializeField]
	private Animator HomuraMagicaAnimator;

	/// <summary>
	/// 制服のほむら
	/// </summary>
	[SerializeField]
	private Animator HomuraAnimator;

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
		HomuraMagicaAnimator.SetTrigger("neutral");

		// カメラ初期化
		Camera1.transform.localPosition = new Vector3(0, 3.206f, 3.11f);
		Camera1.fieldOfView = 60;
		Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk("Prologue05"));
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
		iTween.MoveTo(Camera1.gameObject, iTween.Hash("z", 7.86f, "islocal", true, "time", 3.0f));
		yield return new WaitForSeconds(3.1f);

		//「宴」のシナリオを呼び出す
		Engine.JumpScenario(scenarioLabel);

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}

		// ほむらのポーズを変える
		HomuraMagicaAnimator.SetTrigger("hold_the_head");

		// シーン再生
		engine.ResumeScenario();

		//「宴」のポーズ終了待ち
		while (!Engine.IsPausingScenario)
		{
			yield return 0;
		}
		// 魔法少女ほむらをディアクティブに
		HomuraMagicaAnimator.gameObject.SetActive(false);
		HolyBlast.SetActive(true);
		AudioManager.Instance.PlaySE("SoulGem");
		yield return new WaitForSeconds(0.5f);
		HomuraAnimator.gameObject.SetActive(true);

		// シーン再生
		engine.ResumeScenario();

		//「宴」の終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}

		// 次のシーンへ移動
		FadeManager.Instance.LoadLevel("title", 1.0f);
	}
}
