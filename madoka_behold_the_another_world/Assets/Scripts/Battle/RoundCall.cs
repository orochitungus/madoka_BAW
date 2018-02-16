using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundCall : MonoBehaviour 
{
	/// <summary>
	/// 背景
	/// </summary>
	public Image Background;

	/// <summary>
	/// クリア条件
	/// </summary>
	public Text ClearCondition;

	/// <summary>
	/// カウンター表示部分
	/// </summary>
	public Text Leftuntil;

	/// <summary>
	/// Ready Yourself表示部分
	/// </summary>
	public Text Ready;

	/// <summary>
	/// Engauge表示部分
	/// </summary>
	public Text Engauge;

	public RoundCallState Roundcallstate;

	/// <summary>
	/// クリア条件の内容
	/// </summary>
	public string ClearcondtionText;

	/// <summary>
	/// 開始までのカウンター
	/// </summary>
	private float Counter;

	/// <summary>
	/// ラウンドコールが実行中であるか否か
	/// </summary>
	public bool RoundCallDone;

	/// <summary>
	/// ポーズコントローラー
	/// </summary>
	private GameObject Pausecontroller;

	/// <summary>
	/// ラウンドコール時のキャラを映すカメラ
	/// </summary>
	public Camera RoundCallCamera;

	void Awake()
	{
		// ステート初期化
		Roundcallstate = RoundCallState.CLEARCONDITION;
		// クリア条件以外の文字列を消去
		ClearCondition.gameObject.SetActive(true);
		Leftuntil.gameObject.SetActive(false);
		Ready.gameObject.SetActive(false);
		Engauge.gameObject.SetActive(false);

		// ControllerManagerがあるか判定
		if (GameObject.Find("ControllerManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
			loadManager.name = "ControllerManager";
		}
		// PauseManagerがあるか判定
		if (GameObject.Find("PauseManager") == null)
		{
			// 無ければ作る
			Pausecontroller = (GameObject)Instantiate(Resources.Load("PauseManager"));
			Pausecontroller.name = "PauseManager";
		}
		// ParameterManagerがあるか判定
		if (GameObject.Find("ParameterManager") == null)
		{
			// 無ければ作る
			GameObject parameterManager = (GameObject)Instantiate(Resources.Load("ParameterManager"));
			parameterManager.name = "ParameterManager";
		}
		RoundCallDone = true;

		
	}

	// Use this for initialization
	void Start () 
	{
		// RoundCall以外Pauseする
		// ポーズコントローラー取得		
		PauseControllerInputDetector pcid = GameObject.Find("PauseManager").GetComponent<PauseManager>().RoundCallPauseController;
		// 時間を止める
		pcid.ProcessButtonPress();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (Roundcallstate)
		{
			case RoundCallState.CLEARCONDITION:
				Clearcondtion();
				break;
			case RoundCallState.READY:
				ReadyCount();
				break;
			case RoundCallState.ENGAUGE:
				EngaugeCount();
				break;			
		}
	}

		
	void Clearcondtion()
	{
		// クリア条件を表示
		ClearCondition.text = "クリア条件：" + ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;
		// ショットキーを押されたら次へ
		if (ControllerManager.Instance.Shot)
		{
			Roundcallstate = RoundCallState.READY;
			Counter = MadokaDefine.ROUNDCALLCOUNT;
			ClearCondition.gameObject.SetActive(false);
			Leftuntil.gameObject.SetActive(true);
			Ready.gameObject.SetActive(true);
		}

		// RoundCallCameraの位置と角度を決定
		int settingPosition = 0;
		for(int i=0; i<ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list.Count; ++i)
		{
			if(savingparameter.beforeField == ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list[i].StagefromIndex)
			{
				settingPosition = i;
				break;
			}
		}
		// キャラを映すようにする
		float xpos0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Xpos;
		float ypos0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Ypos;
		float zpos0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Zpos;
		Vector3 charpos = new Vector3(xpos0, ypos0, zpos0);
		float xrot0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Xrot;
		float yrot0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Yrot;
		float zrot0 = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[settingPosition * 3].Zrot;
		Vector3 charrot = new Vector3(xrot0, yrot0, zrot0);
		// 角度は180度反転する
		float yrot = charrot.y - 180;
		// 配置位置
		float xpos = charpos.x + 3 * Mathf.Sin(Mathf.Deg2Rad * charrot.y);
		float zpos = charpos.z + 3 * Mathf.Cos(Mathf.Deg2Rad * charrot.y);

		RoundCallCamera.transform.position = new Vector3(xpos, charpos.y + 3.5f, zpos);
		RoundCallCamera.transform.rotation = Quaternion.Euler(new Vector3(charrot.x, yrot, charrot.z));
	}

	void ReadyCount()
	{
		Leftuntil.text = Counter.ToString("f2");
		Counter -= 0.017f;
		// 残り1秒を割ったら次へ
		if (Counter < 0.5f)
		{
			Leftuntil.gameObject.SetActive(false);
			Ready.gameObject.SetActive(false);
			Engauge.gameObject.SetActive(true);
			Roundcallstate = RoundCallState.ENGAUGE;
		}
		else if(Counter > 2.5f)
		{
			Ready.text = "3";
		}
		else if(Counter > 1.5f)
		{
			Ready.text = "2";
		}
		else
		{
			Ready.text = "1";
		}
	}
	
	

	void EngaugeCount()
	{
		Counter -= 0.017f;
		// 残り0秒を割ったらゲームスタート
		if (Counter < 0.0f)
		{
			Background.gameObject.SetActive(false);
			Roundcallstate = RoundCallState.START;
			// PauseControllerを取得
			PauseControllerInputDetector pcid = GameObject.Find("PauseManager").GetComponent<PauseManager>().RoundCallPauseController;
			// 時間を動かす
			pcid.ProcessButtonPress();
			RoundCallDone = false;
			// ラウンドコールカメラを切る
			RoundCallCamera.gameObject.SetActive(false);
		}
	}
}

public enum RoundCallState
{
	/// <summary>
	/// クリア条件表示
	/// </summary>
	CLEARCONDITION,
	/// <summary>
	/// READY YOURSELF表示
	/// </summary>
	READY,
	/// <summary>
	/// Engauge表示
	/// </summary>
	ENGAUGE,
	/// <summary>
	/// 処理終了
	/// </summary>
	START,
}
