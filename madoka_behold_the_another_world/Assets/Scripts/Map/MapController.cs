using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MapController : MonoBehaviour 
{
	/// <summary>
	/// BGMの名前
	/// </summary>
	public string BGMName;

	/// <summary>
	/// どこから来たかというフラグ
	/// </summary>
	public int FromCode;

	/// <summary>
	/// 移動可能領域
	/// </summary>
	List<MitakiharaCity_MapData.Param> Movepoint = new List<MitakiharaCity_MapData.Param>();

	/// <summary>
	/// カメラ
	/// </summary>
	public Camera MainCamera;

	/// <summary>
	/// インフォメーションテキスト表示部
	/// </summary>
	public Text Information;

	/// <summary>
	/// 日本語名表示部
	/// </summary>
	public Text NameJP;

	/// <summary>
	/// 英語名表示部
	/// </summary>
	public Text NameEN;

	/// <summary>
	/// 現在選択中のマップのインデックス
	/// </summary>
	private int NowIndex;

	/// <summary>
	/// ポップアップ
	/// </summary>
	public GameObject PopUp;

	/// <summary>
	/// 1F前の左入力
	/// </summary>
	private bool PreLeftInput;

	/// <summary>
	/// 1F前の右入力
	/// </summary>
	private bool PreRightInput;

	public GameObject LeftArrow;
	public GameObject RightArrow;

	public MitakiharaCity_MapData MapData;


	void Awake()
	{

		// AudioManagerがあるか判定
		if (GameObject.Find("AudioManager") == null)
		{
			// なければ作る
			GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
			am.name = "AudioManager";   // このままだと名前にCloneがつくので消しておく
		}		
		// EventSystemがあるか判定
		if (GameObject.Find("EventSystem") == null)
		{
			// 無ければ作る
			GameObject eventSystem = (GameObject)Instantiate(Resources.Load("EventSystem"));
			eventSystem.name = "EventSystem";
		}
		// FadeManagerがあるか判定
		if (GameObject.Find("FadeManager") == null)
		{
			// 無ければ作る
			GameObject fadeManager = (GameObject)Instantiate(Resources.Load("FadeManager"));
			fadeManager.name = "FadeManager";
		}
		// LoadManagerがあるか判定
		if (GameObject.Find("LoadManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("LoadManager"));
			loadManager.name = "LoadManager";
		}
		// PauseManagerがあるか判定
		if (GameObject.Find("PauseManager") == null)
		{
			// 無ければ作る
			GameObject pauseManager = (GameObject)Instantiate(Resources.Load("PauseManager"));
			pauseManager.name = "PauseManager";
		}
		// ControllerManagerがあるか判定
		if (GameObject.Find("ControllerManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
			loadManager.name = "ControllerManager";
		}
		// ParameterManagerがあるか判定
		if (GameObject.Find("ParameterManager") == null)
		{
			// 無ければ作る
			GameObject parameterManager = (GameObject)Instantiate(Resources.Load("ParameterManager"));
			parameterManager.name = "ParameterManager";
		}

		// 現在移動可能な個所をリストアップしてリストにする
		for (int i=0; i < MapData.sheets[0].list.Count; i++)
		{
			if(savingparameter.story >= MapData.sheets[0].list[i].LowStory && savingparameter.story <= MapData.sheets[0].list[i].HighStory)
			{
				Movepoint.Add(MapData.sheets[0].list[i]);
				// 現在位置の場合フラグを追加
				if(savingparameter.beforeField == MapData.sheets[0].list[i].FromCode)
				{
					Movepoint[i].NowPosition = true;
				}
				else
				{
					Movepoint[i].NowPosition = false;
				}
			}
		}

		// リストの中で現在の位置にカメラをセットしてインフォメーションと現在位置を表示する
		for(int i=0; i<Movepoint.Count; i++)
		{
			if(Movepoint[i].NowPosition)
			{
				SetMapData(i);
				// インデックスを保持
				NowIndex = i;
			}
		}

		// BGMを変更する
		AudioManager.Instance.PlayBGM(BGMName);
		// Informationに書き込む
		Information.text = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;
	}

	// Use this for initialization
	void Start () 
	{
		// 場所選択
		this.UpdateAsObservable().Where(_ => PopUp.gameObject.transform.localScale.y == 0).Subscribe(_ =>
		{
			// 左
			if (!PreLeftInput && ControllerManager.Instance.Left)
			{
				if (NowIndex > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowIndex--;
					SetMapData(NowIndex);
				}
			}
			// 右
			if (!PreRightInput && ControllerManager.Instance.Right)
			{
				if (NowIndex < Movepoint.Count - 1)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowIndex++;
					SetMapData(NowIndex);
				}
			}

			// 入力更新
			// 上
			PreLeftInput = ControllerManager.Instance.Left;
			// 下
			PreRightInput = ControllerManager.Instance.Right;

			// 選択確定
			if(ControllerManager.Instance.Shot)
			{
				AudioManager.Instance.PlaySE("OK");
				iTween.ScaleTo(PopUp, iTween.Hash(
										// 拡大率指定
										"y", 1,
										// 拡大時間指定
										"time", 0.5f));
				// ポップアップに文字書き込み
				PopUp.GetComponent<MenuPopup>().PopupText.text = "ここに移動しますか？";
			}
			
		});

		// ポップアップ確認
		this.UpdateAsObservable().Where(_ => PopUp.gameObject.transform.localScale.y == 1).Subscribe(_ =>
		{
			// 左
			if (!PreLeftInput && ControllerManager.Instance.Left)
			{
				if (PopUp.GetComponent<MenuPopup>().NowSelect > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					PopUp.GetComponent<MenuPopup>().NowSelect--;
				}
			}
			// 右
			if (!PreRightInput && ControllerManager.Instance.Right)
			{
				if (PopUp.GetComponent<MenuPopup>().NowSelect < 1)
				{
					AudioManager.Instance.PlaySE("cursor");
					PopUp.GetComponent<MenuPopup>().NowSelect++;
				}
			}
			// 選択確定
			if (ControllerManager.Instance.Shot)
			{
				// OK
				if(PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					AudioManager.Instance.PlaySE("OK");
					savingparameter.beforeField = FromCode;
					savingparameter.nowField = Movepoint[NowIndex].ForCode;
					FadeManager.Instance.LoadLevel(Movepoint[NowIndex].StageFileName, 1.0f);
				}
				// キャンセル
				else
				{
					AudioManager.Instance.PlaySE("cursor");
					iTween.ScaleTo(PopUp, iTween.Hash(
										// 拡大率指定
										"y", 0,
										// 拡大時間指定
										"time", 0.5f));
				}
			}

		});
	}

	/// <summary>
	/// カメラ、場所名などを表示する
	/// </summary>
	/// <param name="index"></param>
	private void SetMapData(int index)
	{
		NameJP.text = Movepoint[index].NAME_JP;
		NameEN.text = Movepoint[index].NAME_EN;
		MainCamera.transform.position = new Vector3(Movepoint[index].CameraPosX, Movepoint[index].CameraPosY, Movepoint[index].CameraPosZ);
		MainCamera.transform.rotation = Quaternion.Euler(new Vector3(Movepoint[index].CameraRotX, Movepoint[index].CameraRotY,Movepoint[index].CameraRotZ));
		MainCamera.fieldOfView = Movepoint[index].FieldOfView;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// 左端、右端についたらLeftArrow,RightArrowをそれぞれ消す
		if(NowIndex == 0)
		{
			LeftArrow.SetActive(false);
		}
		else
		{
			LeftArrow.SetActive(true);
		}
		if(NowIndex == Movepoint.Count - 1)
		{
			RightArrow.SetActive(false);
		}
		else
		{
			RightArrow.SetActive(true);
		}
	}
}
