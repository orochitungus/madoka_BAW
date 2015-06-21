using UnityEngine;
using System.Collections;

public class Prologue_02 : EventBase 
{
    // カメラモードのenum
    private enum CameraMode
    {
        INTRO,          // 開幕のイマジカショック・グラウンドゼロ全景
        HOMURA_DOWN,    // 膝をつくほむら
    }

    // 現在有効になっているカメラ
    CameraMode m_CameraMode;

    // BGM
    public AudioClip m_Wind;       // 風音
    public AudioClip m_Fuly;       // BGM「Fuly」

    // SE
    public AudioClip m_Down;
    public AudioClip m_Walk;
    public AudioClip m_Wear;       // スコノの変身音
    public AudioClip m_Drawn_Bow;  // ほむらの抜刀音 

    
    // 操作対象キャラ
    public GameObject m_Homura;         // ほむら
    public GameObject m_Scono_nocort;   // スコノ白衣なし
    public GameObject m_Scono_cort;     // スコノ白衣つき
    public GameObject m_Homura_Bow;     // ほむらの弓フック

    // スコノシュートのフレア
    public GameObject m_Frare;

    // カメラ1の再生速度
    private const float m_Camera1Speed = 0.1f;
    // スコノの歩行速度
    private const float m_SconoWalkSpeed = 0.018f;
    	
	void Awake() 
    {
        m_CameraMode = CameraMode.INTRO;
        // カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].SetActive(false);
        }
        // m_BGMをコンポーネントに追加
        //m_BGM = gameObject.AddComponent<AudioSource>();     
        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にAudioManagerがつくので消しておく            
        }
        // BGM再生開始
        AudioManager.Instance.PlayBGM("Kankyou Kaze01-1");
        EventInitialize();
	}

 	
	// Update is called once per frame
	void Update () 
    {
        switch (xstory)
        {
            case 0:
                keybreak = true;
                //m_BGM.clip = m_Wind;        // 音色をチャンネルに紐付け
                //m_BGM.volume = 0.5f;
                //m_BGM.Play();               // 再生開始
                //m_BGM.loop = true;
                m_camera[0].animation.Play("prolouge2_camera1");
                m_camera[0].animation["prolouge2_camera1"].speed = m_Camera1Speed;                
                xstory++;
                break;
            case 1:
                keybreak = true;
                if (m_Accumulated_time > 10.0f && m_CameraMode == CameraMode.INTRO)
                {
                    // カメラ１のアニメを止める
                    this.m_camera[0].SetActive(false);
                    // カメラ２を実行する
                    this.m_camera[1].SetActive(true);

                    // カメラワーク変更フラグを立てておく
                    m_CameraMode = CameraMode.HOMURA_DOWN;
                    IncrementXstory();
                }
                break;
            case 2:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_SADNESS;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "イマジカショック・グラウンドゼロ・・・・・終わりが始まった場";
                m_serif[1] = "所・・・か";
                break;
            case 3:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_SADNESS;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "・・・もう魔力の残りも少ない、か・・・まどか・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 4:
                keybreak = true;
                AudioSource.PlayClipAtPoint(m_Down, m_Homura.transform.position);
                m_Homura.animation.Play("homura_kneel_copy");
                this.m_camera[1].SetActive(false);
                this.m_camera[2].SetActive(true);
                FullClear();
                IncrementXstory();
                break;
            case 5:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_SADNESS;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "まどか、私がんばったよね・・・だからもう・・・！？";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 6:
                keybreak = true;
                FullClear();
                // スコノをテレポートさせる
                m_Scono_nocort.transform.position = new Vector3(960, 141.5f, 4762);
                m_Scono_nocort.transform.rotation = Quaternion.Euler(0, 180, 0);
                // 4カメラに切り替える
                this.m_camera[2].SetActive(false);
                this.m_camera[3].SetActive(true);
                // スコノを歩行モーションにする
                m_Scono_nocort.animation.Play("Scono_walk_copy");
                // BGMを切り替える
                //m_BGM.clip = m_Fuly;
                //m_BGM.volume = 0.5f;
                //m_BGM.Play();               // 再生開始
                //m_BGM.loop = true;     
                AudioManager.Instance.PlayBGM("Fury");
                IncrementXstory();
                break;
            case 7:
                keybreak = true;
                // スコノが規定位置に来ると次のシーンへ
                if (m_Scono_nocort.transform.position.z < 4751)
                {
                    m_Scono_nocort.animation.Play("Scono_neutral_copy");
                    IncrementXstory();
                }
                else
                {
                    // 歩行SEを鳴らす
                    FootSE(m_Walk, 0.5f, m_Scono_nocort.transform.position);
                    //AudioSource.PlayClipAtPoint(m_Walk, m_Scono_nocort.transform.position);
                    // スコノを移動させる
                    m_Scono_nocort.transform.position = new Vector3(m_Scono_nocort.transform.position.x, m_Scono_nocort.transform.position.y, m_Scono_nocort.transform.position.z - m_SconoWalkSpeed);
                    
                }
                break;
            case 8:
                keybreak = false;
                m_facetype = CharacterFace.SCONOSCIUTO_NORMAL;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 9:
                this.m_camera[2].SetActive(true);
                this.m_camera[3].SetActive(false);
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_SURPRISE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "まだ私以外にも人間・・・いえ、魔法少女が残っていたなんてね・";
                m_serif[1] = "・・初めて見る顔だけど、名前くらいは聞いておこうかしら";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 10:
                this.m_camera[2].SetActive(false);
                this.m_camera[3].SetActive(true);
                keybreak = false;
                m_facetype = CharacterFace.SCONOSCIUTO_NORMAL;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 11:
                keybreak = true;
                FullClear();
                this.m_camera[1].SetActive(true);
                this.m_camera[3].SetActive(false);
                m_Homura.animation.Play("homura_standup_copy");
                AudioSource.PlayClipAtPoint(m_Walk, m_Homura.transform.position);
                IncrementXstory();
                break;
            case 12:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "・・・グリーフシードが欲しいの？いいわ、もう私には必要ないも";
                m_serif[1] = "の。持って行きなさい";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 13:
                keybreak = true;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                FullClear();
                m_Homura.animation.Play("homura_showgreefseed_copy");
                IncrementXstory();
                break;
            case 14:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;               
                break;
            case 15:
                keybreak = true;
                FullClear();
                this.m_camera[1].SetActive(false);
                this.m_camera[3].SetActive(true);
                m_Scono_nocort.animation.Play("Scono_pay_an_arm_copy");
                m_Accumulated_time = 0;
                IncrementXstory();
                break;
            case 16:
                keybreak = true;
                FullClear();
                if(m_Accumulated_time > 0.8f)
                {
                    m_Frare.transform.position = new Vector3(m_Scono_nocort.transform.position.x, 144.6683f, m_Scono_nocort.transform.position.z);
                }
                // 腕を振り終わったら、フレアを出して白衣モードに
                if (m_Accumulated_time > 1.0f)
                {                    
                    m_Scono_cort.transform.position = new Vector3(m_Scono_nocort.transform.position.x, m_Scono_nocort.transform.position.y, m_Scono_nocort.transform.position.z);
                    m_Scono_cort.transform.rotation = Quaternion.Euler(0, 180, 0);
                    m_Scono_nocort.transform.position = Vector3.zero;
                    AudioSource.PlayClipAtPoint(m_Wear, m_Scono_cort.transform.position);
                    m_Accumulated_time = 0.0f;
                    IncrementXstory();
                }
                break;
            case 17:
                keybreak = true;
                if (m_Accumulated_time > 1.0f)
                {
                    m_Frare.transform.position = Vector3.zero;
                    IncrementXstory();
                }
                break;
            case 18:                
                keybreak = false;
                m_facetype = CharacterFace.SCONOSCIUTO_WRITE_NORMAL;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・・・・！！";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 19:
                keybreak = false;
                FullClear();
                this.m_camera[1].SetActive(true);
                this.m_camera[3].SetActive(false);
                var C_Weapon = m_Homura_Bow.GetComponent<HockControl>();
                C_Weapon.EquipWeapon();
                // 抜刀時のSEを鳴らす
                AudioSource.PlayClipAtPoint(m_Drawn_Bow, transform.position);
                // ほむらに抜刀ポーズをとらせる
                m_Homura.animation.Play("reload_arrow_homura_ribon_event2_copy");
                // 表情を変える
                var Face = m_Homura.GetComponentInChildren<FaceSelecter>(); // 対象オブジェクトに取り付けたスクリプト名を指定する              
                Face.ChangeFaceTexture("homura_seriousface");
                IncrementXstory();
                break;
            case 20:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_HATE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "今更私と争って何に・・・！？いや、まあいいわ。最後にただ消え";
                m_serif[1] = "るのではなく、あなたの酔狂に付き合って消えるのも一興ね！";
                m_serif[2] = "";
                m_serif[3] = "";
                savingparameter.story = 1;  // ストーリー進行度変更
                break;
            default:
                // VSスコノシュート戦へ飛ばす
                m_facetype = 0;
                m_drawname_jp = "";
                m_drawname_en = "";
                for (int i = 0; i < m_serif.Length; i++)
                {
                    m_serif[i] = "";
                }
                savingparameter.nowField = 4;
                savingparameter.beforeField = 0;
                Application.LoadLevel("Imagica_Shock_Ground_Zero_VSScono");
                break;
        }
        EventUpdate();
	}
}
