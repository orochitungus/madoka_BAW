using UnityEngine;
using System.Collections;

public class StageSetting : MonoBehaviour 
{
    // メンバー変数
    //・SavingParameterの初期化の有無
    public bool m_InitializeSavingParameter;
    // BGMを止めたい場合これをONに
    public bool m_stopBGM;      
    // 現在のステージをテストステージにしたいならここをＯＮ
    public bool m_isTestStage;
    // 現在のステージをクエストステージにしたいならここをON
    public bool m_isQuestStage;
 
    // キャラクターの初期配置場所のインデックス
    private int m_settingPosition;


   
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
		

        // 最初のステージのみSavingParameterの初期化を行う（フラグはPublicでInspectorで外部から制御）
        if (m_InitializeSavingParameter)
        {
            savingparameter.savingparameter_Init();
        }

		// テストステージだった場合はnowFieldを0にする
		if (m_isTestStage)
		{
			savingparameter.nowField = 0;
		}
        // 配置候補が複数ある場合があるので、何処に配置するか決める
        m_settingPosition = 0;
        for (int i = 0; i < StageCode.stagefromindex[savingparameter.nowField].Length; ++i)
        {
            if (savingparameter.beforeField == StageCode.stagefromindex[savingparameter.nowField][i])
            {
                m_settingPosition = i;
                break;
            }
        }

        // 既定の値に基づいて開始時の規定位置にPCキャラクターを配置してカメラの方向を決定する（フラグで設定）
        // ロードするキャラクターの数（クエストパートは1体しかロードしない）
        int numofLoadCharacter = 3;
        if (m_isQuestStage)
            numofLoadCharacter = 1;
        for (int i = 0; i < numofLoadCharacter; i++)
        {            
            // いない場合を除く
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
                    if (m_isQuestStage)
                    {
                        Instantiate(Resources.Load(m_CharacterFileName_Quest[savingparameter.GetNowParty(i)]), SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
                    else
                    {
                        Instantiate(Resources.Load(m_CharacterFileName[savingparameter.GetNowParty(i)]), SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
                }
                // それ以外の場合
                else
                {
                    // 配置位置
                    Vector3 SetPosPlayer = StagePosition.m_InitializeCharacterPos[savingparameter.nowField][m_settingPosition][i];
                    // 配置角度
                    Vector3 SetRotPlayer = StagePosition.m_InitializeCharacterRot[savingparameter.nowField][m_settingPosition][i];
                    // キャラクターをロードする
                    // クエストパート
                    if (m_isQuestStage)
                    {
                        Instantiate(Resources.Load(m_CharacterFileName_Quest[savingparameter.GetNowParty(i)]), SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
                    else
                    {
                        Instantiate(Resources.Load(m_CharacterFileName[savingparameter.GetNowParty(i)]), SetPosPlayer, Quaternion.Euler(SetRotPlayer));
                    }
                }
            }
        }
        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にCloneがつくので消しておく
            // BGM再生開始(0はないので鳴らさない）
            if (savingparameter.nowField > 0 && !m_stopBGM)
            {
                AudioManager.Instance.PlayBGM(StageCode.stageBGM[savingparameter.nowField]);
            }
        }
        // あった場合、変化フラグがあった場合かロードした場合のみ再生
        else
        {
            if (StageCode.changebgm[savingparameter.nowField][m_settingPosition] || savingparameter.beforeField == 9999)
            {
                AudioManager.Instance.PlayBGM(StageCode.stageBGM[savingparameter.nowField]);
            }
        }
		// CutinSystemがあるか判定(戦闘パートのみ)
		if (GameObject.Find("CutinSystem") == null && !m_isQuestStage)
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
		if(GameObject.Find("TalkSystem") == null && m_isQuestStage)
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
    }

    // ロードするキャラクターのファイル名(ルート・戦闘用）
    private string[] m_CharacterFileName = new string[]
    {
        "",                         // なし
        "",                         // 鹿目まどか
        "",                         // 美樹さやか
        "",                         // 暁美ほむら（銃）
        "",                         // 巴マミ
        "",                         // 佐倉杏子
        "",                         // 千歳ゆま
        "",                         // 呉キリカ
        "",                         // 美国織莉子
        "Sconosciuto_Battle_Use",   // スコノシュート（PC)
        "Homura_Final_Battle_Use",  // 暁美ほむら（弓）
        "",                         // アルティメットまどか（まどかの覚醒とは一応別扱い）
    };

    // ロードするキャラクターのファイル名(ルート・クエストパート用）
    private string[] m_CharacterFileName_Quest = new string[]
    {
        "",                                 // なし
        "",                                 // 鹿目まどか
        "",                                 // 美樹さやか
        "Homura_Quest_Use",                 // 暁美ほむら（銃）
        "",                                 // 巴マミ
        "",                                 // 佐倉杏子
        "",                                 // 千歳ゆま
        "",                                 // 呉キリカ
        "",                                 // 美国織莉子
        "Scono_battle_use",                 // スコノシュート（PC)
        "Homura_Quest_Use",                 // 暁美ほむら（弓）
        "",                                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
    };
   
    //// ロードするキャラクターのファイル名（本体・戦闘用）
    //private string[] m_CharacterFileNameCore = new string[]
    //{
    //    "",                                 // なし
    //    "",                                 // 鹿目まどか
    //    "",                                 // 美樹さやか
    //    "",                                 // 暁美ほむら（銃）
    //    "",                                 // 巴マミ
    //    "",                                 // 佐倉杏子
    //    "",                                 // 千歳ゆま
    //    "",                                 // 呉キリカ
    //    "",                                 // 美国織莉子
    //    "Scono_battle_use",                 // スコノシュート（PC)
    //    "homura_ribon_magica_battle_use",   // 暁美ほむら（弓）
    //    "",                                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
    //};

    //// ロードするキャラクターのファイル名（本体・クエストパート用）
    //private string[] m_CharacterFileNameCore_Quest = new string[]
    //{
    //    "",                                 // なし
    //    "",                                 // 鹿目まどか
    //    "",                                 // 美樹さやか
    //    "homura_uniform_use",               // 暁美ほむら（銃）
    //    "",                                 // 巴マミ
    //    "",                                 // 佐倉杏子
    //    "",                                 // 千歳ゆま
    //    "",                                 // 呉キリカ
    //    "",                                 // 美国織莉子
    //    "Scono_battle_use",                 // スコノシュート（PC)
    //    "homura_uniform_use",               // 暁美ほむら（弓）
    //    "",                                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
    //};

	// Use this for initialization
	void Start () 
    {
		
        m_setting = false;
	}
	
    // 配置したか否か(Startでロードした段階ではまだオブジェクトが生成されていない
    private bool m_setting;

	// Update is called once per frame
	void Update () 
    {
        Initialise();        
	}

    // 初期化
    void Initialise()
    {
        if (!m_setting)
        {            
            if (!SettingCharacter(0))
            {
                return;
            }

            // クエストパートでは２、３人目は出さないので飛ばす
            if (!m_isQuestStage)
            {
                //・2キャラ目、3キャラ目がいる場合はその左右に配置する(いない場合は除く)
                for (int i = 1; i < 3; i++)
                {
                    // いない場合を除く
                    if (savingparameter.GetNowParty(i) != 0)
                    {
                        SettingCharacter(i);
                    }
                }
            }
            m_setting = true;
        }
    }

    // ロードしたキャラをセッティングする
    // partynumber[in]  :パーティーの何人目か
    private bool SettingCharacter(int partynumber)
    {
        // Findでロードしたキャラクターを探す
        GameObject PlayerCharacter;
        // 戦闘ステージ
        if (!m_isQuestStage)
            PlayerCharacter = GameObject.Find(m_CharacterFileName[savingparameter.GetNowParty(partynumber)] + "(Clone)");
        else
            PlayerCharacter = GameObject.Find(m_CharacterFileName_Quest[savingparameter.GetNowParty(partynumber)] + "(Clone)");
        // ロードが終わるまでタイムラグがあるようなので一旦待つ
        if (PlayerCharacter == null)
        {
            return false;
        }
        // Cloneがつくのでリネームする
        // 戦闘ステージ
        if (!m_isQuestStage)
            PlayerCharacter.name = m_CharacterFileName[savingparameter.GetNowParty(partynumber)];
        else
            PlayerCharacter.name = m_CharacterFileName_Quest[savingparameter.GetNowParty(partynumber)];
                
        
        // 戦闘用
        if (!m_isQuestStage)
        {
            // そのキャラクターをPLAYERかALLYと設定する(戦闘時のみ)
            CharacterControl_Base playerCharacter = PlayerCharacter.GetComponentInChildren<CharacterControl_Base>();
            if (partynumber == 0)
            {
                playerCharacter.m_isPlayer = CharacterControl_Base.CHARACTERCODE.PLAYER;
            }
            else
            {
                playerCharacter.m_isPlayer = CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY;
            }
            // キャラクターの向きを決める
            // カメラ
            Player_Camera_Controller playerCamera = PlayerCharacter.GetComponentInChildren<Player_Camera_Controller>();

            if (playerCamera != null)
            {
                // 本体の向きを変える
                Vector3 bodyRot = StagePosition.m_InitializeCharacterRot[savingparameter.nowField][m_settingPosition][partynumber];
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
            CharacterControl_Base_Quest playerCharacter = PlayerCharacter.GetComponentInChildren<CharacterControl_Base_Quest>();
            // キャラクターの向きを決める
            // カメラ
            Player_Camera_Controller playerCamera = PlayerCharacter.GetComponentInChildren<Player_Camera_Controller>();
            if (playerCamera != null)
            {
                // 本体の向きを変える
                Vector3 bodyRot = StagePosition.m_InitializeCharacterRot[savingparameter.nowField][m_settingPosition][partynumber];
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
