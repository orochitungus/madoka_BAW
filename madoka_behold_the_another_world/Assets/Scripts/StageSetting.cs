using UnityEngine;
using System.Collections;
using System.Linq;

public class StageSetting : MonoBehaviour 
{
	// メンバー変数
	/// <summary>
	/// SavingParameterの初期化の有無
	/// </summary>	
	public bool InitializeSavingParameter;

	/// <summary>
	/// BGMを止めたい場合これをONに
	/// </summary>	
	public bool StopBGM;

	/// <summary>
	/// 現在のステージをテストステージにしたいならここをＯＮ
	/// </summary>	
	public bool IsTestStage;


	/// <summary>
	/// 現在のステージをクエストステージにしたいならここをON
	/// </summary>	
	public bool IsQuestStage;

	/// <summary>
	/// キャラクターの初期配置場所のインデックス
	/// </summary>	
	private int SettingPosition;

	/// <summary>
	/// 環境光のオブジェクト
	/// </summary>
	public GameObject DLight;

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;
   
    // 鳴らす足音の種類
    public enum FootType
    {
        FootType_Normal,    // コンクリ等の堅い足音
        FootType_Wood,      // 木の床
        FootType_Jari,      // 砂利道（開始ステージと6章・最終章の崩壊見滝原を含む）
        FootType_Snow,      // 雪道
        FootType_Carpet,    // カーペット
    };

    
    // 鳴らす足音
    public FootType footType;
    // monodevelopのバグで、enumのPublicは拾えないらしいのでアクセサを用意する
    public int getFootType()
    {
        int footstep = 0;
        switch (footType)
        {
            case FootType.FootType_Normal:
                footstep = 0;
                break;
            case FootType.FootType_Wood:
                footstep = 1;
                break;
            case FootType.FootType_Jari:
                footstep = 2;
                break;
            case FootType.FootType_Snow:
                footstep = 3;
                break;
            case FootType.FootType_Carpet:
                footstep = 4;
                break;
            default:
                break;
        }
        return footstep;
    }
    
    void Awake()
    {
		// CutinSystemがあるか判定(戦闘パートのみ)
		if (GameObject.Find("CutinSystem") == null && !IsQuestStage)
		{
			// 無ければ作る
			GameObject cutinSystem = (GameObject)Instantiate(Resources.Load("CutinSystem"));
			cutinSystem.name = "CutinSystem";
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
		// TalkSystemがあるか判定（非戦闘シーン限定）
		if (GameObject.Find("TalkSystem") == null && IsQuestStage)
		{
			GameObject talkSystem = (GameObject)Instantiate(Resources.Load("TalkSystem"));
			talkSystem.name = "TalkSystem";
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
		// 最初のステージのみSavingParameterの初期化を行う（フラグはPublicでInspectorで外部から制御）
		if (InitializeSavingParameter)
        {
            savingparameter.savingparameter_Init();
        }

		// テストステージだった場合はnowFieldを0にする
		if (IsTestStage)
		{
			savingparameter.nowField = 0;
		}
        // 配置候補が複数ある場合があるので、何処に配置するか決める
        SettingPosition = 0;
        for(int i=0; i<ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list.Count; ++i)
		{
            if(savingparameter.beforeField == ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list[i].StagefromIndex)
			{
                SettingPosition = i;
                break;
            }
        }

        // 既定の値に基づいて開始時の規定位置にPCキャラクターを配置してカメラの方向を決定する（フラグで設定）
        // ロードするキャラクターの数（クエストパートは1体しかロードしない）
        int numofLoadCharacter = 3;
		if (IsQuestStage)
		{
			numofLoadCharacter = 1;
		}
		else
		{

		}
        for (int i = 0; i < numofLoadCharacter; i++)
        {            
            // いる場合
            if (savingparameter.GetNowParty(i) != 0)
            {
                // ロードしてきた場合
                if (savingparameter.beforeField == 9999)
                {
                    // 配置位置
                    Vector3 SetPosPlayer = savingparameter.nowposition;
                    // 配置角度
                    Vector3 SetRotPlayer = savingparameter.nowrotation;
                    // キャラクターをロードする
                    // クエストパート
                    if (IsQuestStage)
                    {
                        Instantiate(QuestCharacter[savingparameter.GetNowParty(i)], SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
					// バトルパート
                    else
                    {
                        Instantiate(BattleCharacter[savingparameter.GetNowParty(i)], SetPosPlayer, Quaternion.Euler(SetRotPlayer));
						// 現在HP
						Battleinterfacecontroller.NowPlayerHP[i] = savingparameter.GetNowHP(savingparameter.GetNowParty(i));
						// 最大HP
						Battleinterfacecontroller.MaxPlayerHP[i] = savingparameter.GetMaxHP(savingparameter.GetNowParty(i));
                    }
                }
                // それ以外の場合
                else
                {
					// 配置位置
					float xpos = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Xpos;
					float ypos = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Ypos;
					float zpos = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Zpos;
					Vector3 SetPosPlayer = new Vector3(xpos, ypos, zpos);
					// 配置角度
					float xrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Xrot;
					float yrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Yrot;
					float zrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + i].Zrot;
					Vector3 SetRotPlayer = new Vector3(xrot, yrot, zrot);
					// キャラクターをロードする
					// クエストパート
					if (IsQuestStage)
                    {
                        Instantiate(QuestCharacter[savingparameter.GetNowParty(i)], SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
                    else
                    {
                        Instantiate(BattleCharacter[savingparameter.GetNowParty(i)], SetPosPlayer, Quaternion.Euler(SetRotPlayer));
						// 現在HP
						Battleinterfacecontroller.NowPlayerHP[i] = savingparameter.GetNowHP(savingparameter.GetNowParty(i));
						// 最大HP
						Battleinterfacecontroller.MaxPlayerHP[i] = savingparameter.GetMaxHP(savingparameter.GetNowParty(i));
					}
                }
            }
			// いない場合
			else
			{
				Battleinterfacecontroller.NowPlayerHP[i] = 0;
				Battleinterfacecontroller.MaxPlayerHP[i] = 0;
				// 顔アイコンを消す
				Battleinterfacecontroller.Playerbg[i].gameObject.SetActive(false);
			}
        }
        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にCloneがつくので消しておく
            // BGM再生開始(0はないので鳴らさない）
            if (savingparameter.nowField > 0 && !StopBGM)
            {
				AudioManager.Instance.PlayBGM(ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list[SettingPosition].BGM);
            }
        }
        // あった場合、変化フラグがあった場合かロードした場合のみ再生
        else
        {
            if(ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list[SettingPosition].ChangeBGM || savingparameter.beforeField == 9999)
			{
				AudioManager.Instance.PlayBGM(ParameterManager.Instance.StagecodeData.sheets[savingparameter.nowField].list[SettingPosition].BGM);
			}
        }

		// 環境光をストーリー進行状況に応じて変える
		DirectionalLightSetting();
	}

	/// <summary>
	/// 環境光を設定する
	/// </summary>
	public void DirectionalLightSetting()
	{
		int stageindex = savingparameter.nowField;
		int targetindex = 0;
		// 現在のストーリー進行度がStageSkyDataのどこにあるか判定する
		for(int i=0; i<ParameterManager.Instance.StageskyData.sheets[stageindex].list.Count; i++)
		{
			int minStory = ParameterManager.Instance.StageskyData.sheets[stageindex].list[i].MinStory;
			int maxStory = ParameterManager.Instance.StageskyData.sheets[stageindex].list[i].MaxStory;
			if(savingparameter.story >= minStory && savingparameter.story <= maxStory)
			{
				targetindex = i;
				break;
			}
		}
		// 環境光の角度を設定する
		float xrot = ParameterManager.Instance.StageskyData.sheets[stageindex].list[targetindex].RotateX;
		float yrot = ParameterManager.Instance.StageskyData.sheets[stageindex].list[targetindex].RotateY;
		float zrot = ParameterManager.Instance.StageskyData.sheets[stageindex].list[targetindex].RotateZ;

		DLight.transform.rotation = Quaternion.Euler(xrot, yrot, zrot);
	}

	/// <summary>
	/// 戦闘用キャラクター（順番は下のCharacterFileNameに合わせること）
	/// </summary>
	public GameObject []BattleCharacter;

	/// <summary>
	/// クエストパート用キャラクター
	/// </summary>
	public GameObject []QuestCharacter;

       
    

	// Use this for initialization
	void Start () 
    {
		player1Setting = false;
		// 二人以上いる場合
		if (savingparameter.GetNowPartyNum() >= 2)
		{
			player2Setting = false;
		}
		else
		{
			player2Setting = true;
		}
		// 三人以上いる場合
		if (savingparameter.GetNowPartyNum() == 3)
		{
			player3Setting = false;
		}
		else
		{
			player3Setting = true;
		}
	}
	
    // 配置したか否か(Startでロードした段階ではまだオブジェクトが生成されていない
    private bool player1Setting;
	private bool player2Setting;
	private bool player3Setting;

	/// <summary>
	/// 初期化完了フラグ
	/// </summary>
	private bool settingComplete = false;

	// Update is called once per frame
	void Update () 
    {
		if (!settingComplete)
		{
			Initialise();
		}        
	}

    // 初期化
    void Initialise()
    {
		// 一人目
		if (!player1Setting)
		{
			if (!SettingCharacter(0))
			{
				return;
			}
			// プレイヤーが一人だけのときならSettingCharacter(0)が成功した時点で初期化終了
			else
			{
				if(savingparameter.GetNowPartyNum() == 1)
				{
					settingComplete = true; 
				}
				player1Setting = true;
			}
		}

        // クエストパートでは２、３人目は出さないので飛ばす
        if (!IsQuestStage)
        {
			// 二人目
			if(!player2Setting)
			{
				if(!SettingCharacter(1))
				{
					return;
				}				
				else
				{
					player2Setting = true;									
				}
			}
            // 三人目
			if(!player3Setting)
			{
				if(!SettingCharacter(2))
				{
					return;
				}				
				else
				{
					player3Setting = true;
				}
			}

			// プレイヤーが二人のときで両方成功してたら初期化終了
			if(savingparameter.GetNowPartyNum() == 2)
			{
				if(player1Setting && player2Setting)
				{
					settingComplete = true;
				}
			}

			// プレイヤーが三人のときで両方成功してたら初期化終了
			if(savingparameter.GetNowPartyNum() == 3)
			{
				if(player1Setting && player2Setting && player3Setting)
				{
					settingComplete = true;
				}
			}
		}

	}

    // ロードしたキャラをセッティングする
    // partynumber[in]  :パーティーの何人目か
    private bool SettingCharacter(int partynumber)
    {
        // Findでロードしたキャラクターを探す
        GameObject PlayerCharacter;
		// 戦闘ステージ
		if (!IsQuestStage)
		{
			Debug.Log("Name_OR:" + ObjectName.CharacterFileName[savingparameter.GetNowParty(partynumber)]);
			PlayerCharacter = GameObject.Find(ObjectName.CharacterFileName[savingparameter.GetNowParty(partynumber)] + "(Clone)");
		}
		else
			PlayerCharacter = GameObject.Find(ObjectName.CharacterFileName_Quest[savingparameter.GetNowParty(partynumber)] + "(Clone)");
        // ロードが終わるまでタイムラグがあるようなので一旦待つ
        if (PlayerCharacter == null)
        {
            return false;
        }
        // Cloneがつくのでリネームする
        // 戦闘ステージ
        if (!IsQuestStage)
            PlayerCharacter.name = ObjectName.CharacterFileName[savingparameter.GetNowParty(partynumber)];
        else
            PlayerCharacter.name = ObjectName.CharacterFileName_Quest[savingparameter.GetNowParty(partynumber)];
                
        
        // 戦闘用
        if (!IsQuestStage)
        {
            // そのキャラクターをPLAYERかALLYと設定する(戦闘時のみ)
            CharacterControlBase playerCharacter = PlayerCharacter.GetComponentInChildren<CharacterControlBase>();
            if (partynumber == 0)
            {
                playerCharacter.IsPlayer = CharacterControlBase.CHARACTERCODE.PLAYER;
				// 処理順の関係でカメラONOFFはここでやる
				var target = playerCharacter.MainCamera.GetComponentInChildren<Player_Camera_Controller>();
				target.GetComponent<Camera>().enabled = true;
			}
            else
            {
                playerCharacter.IsPlayer = CharacterControlBase.CHARACTERCODE.PLAYER_ALLY;
				// 処理順の関係でカメラONOFFはここでやる
				var target = playerCharacter.MainCamera.GetComponentInChildren<Player_Camera_Controller>();
				target.GetComponent<Camera>().enabled = false;
			}
            // キャラクターの向きを決める
            // カメラ
            Player_Camera_Controller playerCamera = PlayerCharacter.GetComponentInChildren<Player_Camera_Controller>();

            if (playerCamera != null)
            {
				// 本体の向きを変える
				// 配置角度
				float xrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Xrot;
				float yrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Yrot;
				float zrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Zrot;
				Vector3 bodyRot = new Vector3(xrot, yrot, zrot);
                playerCharacter.transform.rotation = Quaternion.Euler(bodyRot);
                // カメラの向きを変える              
                playerCamera.RotY += bodyRot.y;
            }
            else
            {
                return false;
            }
        }
        // クエストパート用
        else
        {
            CharacterControlBaseQuest playerCharacter = PlayerCharacter.GetComponentInChildren<CharacterControlBaseQuest>();
            // キャラクターの向きを決める
            // カメラ
            Player_Camera_Controller playerCamera = PlayerCharacter.GetComponentInChildren<Player_Camera_Controller>();
            if (playerCamera != null)
            {
				// 本体の向きを変える
				// 配置角度
				float xrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Xrot;
				float yrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Yrot;
				float zrot = ParameterManager.Instance.StagepositionrotData.sheets[savingparameter.nowField].list[SettingPosition * 3 + partynumber].Zrot;
				Vector3 bodyRot = new Vector3(xrot, yrot, zrot);
				playerCharacter.transform.rotation = Quaternion.Euler(bodyRot);
                // カメラの向きを変える              
                playerCamera.RotY += bodyRot.y;
            }
            else
            {
                return false;
            }
        }
        
        return true;
    }
}
