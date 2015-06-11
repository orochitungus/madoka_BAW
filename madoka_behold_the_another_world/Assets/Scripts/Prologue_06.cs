using UnityEngine;
using System.Collections;

public class Prologue_06 : EventBase 
{
    // 動作するキャラクター（恭介と車椅子は別なので、それぞれ定義）
    [SerializeField] GameObject m_Homura;
    [SerializeField] GameObject m_Sayaka;
    [SerializeField] GameObject m_Kyosuke;
    [SerializeField] GameObject m_WheelChair;
    [SerializeField] GameObject m_Scono;
    [SerializeField] GameObject m_DoorEvL;
    [SerializeField] GameObject m_DoorEvR;

    // SE
    [SerializeField] AudioClip[] m_ses;
    // FaceSelecterを持ったオブジェクト
    [SerializeField] FaceSelecter m_HomuraFaceSelecter;
    [SerializeField] FaceSelecter m_SayakaFaceSelecter;
    [SerializeField] FaceSelecter m_KyosukeFaceSelecter;
    [SerializeField] FaceSelecter m_SconoFaceSelecter;

	// Use this for initialization
	void Start () 
    {
        // カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].gameObject.SetActive(false);
        }
        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にAudioManagerがつくので消しておく
        }
        // BGM再生開始
        AudioManager.Instance.PlayBGM("Kaorimade_rintoshita");
        EventInitialize();
        // カメラ1を動かす
        m_camera[0].animation.Play("prologue6_camera1");
	}
	
	// Update is called once per frame
	void Update () 
    {
	    switch(xstory)
        {
            case 0:
                keybreak = true;
                FullClear();
                // カメラ1を恭介たちに寄せてから次へ行く
                if (m_camera[0].transform.position.z > 30)
                {
                    IncrementXstory();
                }
                break;
            case 1:
                keybreak = false;
                m_facetype = CharacterFace.SAYAKA_NORMAL;
                m_drawname_jp = "美樹さやか";
                m_drawname_en = "MIKI SAYAKA";
                m_serif[0] = "大丈夫、恭介？辛くない";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 2:
                keybreak = true;
                m_camera[0].SetActive(false);
                m_camera[1].SetActive(true);
                m_Kyosuke.animation.Play("kyosuke_sitdown_lookback_copy");
                FullClear();
                IncrementXstory();
                break;
            case 3:
                keybreak = false;
                m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
                m_drawname_jp = "上条恭介";
                m_drawname_en = "KAMIJOU KYOUSUKE";
                m_serif[0] = "ああ、ごめんねさやか、わざわざ来てもらって";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 4:
                keybreak = true;
                FullClear();
                // さやかを笑顔にする
                m_SayakaFaceSelecter.ChangeFaceTexture(1);
                IncrementXstory();                
                break;
            case 5:
                keybreak = false;
                m_facetype = CharacterFace.SAYAKA_SMILE;
                m_drawname_jp = "美樹さやか";
                m_drawname_en = "MIKI SAYAKA";
                m_serif[0] = "いいんだよ。あたしが好きでやっていることなんだし。それによう";
                m_serif[1] = "やく集中治療室から出られたんだから、久しぶりにゆっくり話もし";
                m_serif[2] = "たかったし";
                m_serif[3] = "";
                break;
            case 6:
                keybreak = true;
                FullClear();
                // 恭介の顔を戻す
                m_Kyosuke.animation.Play("kyosuke_sitdown_lookback_return_copy");
                // 恭介の表情を変更する
                m_KyosukeFaceSelecter.ChangeFaceTexture(1);
                IncrementXstory();
                break;
            case 7:
                keybreak = false;
                m_facetype = CharacterFace.KYOSUKE_HOSPITAL_SADNESS;
                m_drawname_jp = "上条恭介";
                m_drawname_en = "KAMIJOU KYOUSUKE";
                m_serif[0] = "そうだね、いままでさやかには随分と心配をかけたね、ごめん";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 8:
                keybreak = true;
                FullClear();
                m_SayakaFaceSelecter.ChangeFaceTexture(0);
                IncrementXstory();
                break;
            case 9:
                keybreak = false;
                m_facetype = CharacterFace.SAYAKA_NORMAL;
                m_drawname_jp = "美樹さやか";
                m_drawname_en = "MIKI SAYAKA";
                m_serif[0] = "気にしない気にしない・・・でも個室病棟なんて凄いじゃない";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 10:
                keybreak = true;
                FullClear();
                m_SayakaFaceSelecter.ChangeFaceTexture(1);
                IncrementXstory();
                break;
            case 11:
                keybreak = false;
                m_facetype = CharacterFace.SAYAKA_SMILE;
                m_drawname_jp = "美樹さやか";
                m_drawname_en = "MIKI SAYAKA";
                m_serif[0] = "よっ、このブルジョワめー";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 12:
                keybreak = true;
                FullClear();
                m_KyosukeFaceSelecter.ChangeFaceTexture(0);
                IncrementXstory();
                break;
            case 13:
                keybreak = false;
                m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
                m_drawname_jp = "上条恭介";
                m_drawname_en = "KAMIJOU KYOUSUKE";
                m_serif[0] = "僕はどうでもよかったんだけど、父さんがゆっくり養生しろってう";
                m_serif[1] = "るさくてさ。さやかにもしっかり礼をしておけって言われたし";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 14:
                keybreak = true;
                FullClear();
                m_SayakaFaceSelecter.ChangeFaceTexture(2);
                IncrementXstory();
                break;
            case 15:
                keybreak = false;
                m_facetype = CharacterFace.SAYAKA_SHY;
                m_drawname_jp = "美樹さやか";
                m_drawname_en = "MIKI SAYAKA";
                m_serif[0] = "え、あ、あははー。別にいいよそんな・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 16:
                keybreak = true;
                FullClear();
                m_camera[0].SetActive(true);
                m_camera[1].SetActive(false);
                m_SayakaFaceSelecter.ChangeFaceTexture(0);
                // エレベーターのドアを開ける
                m_DoorEvL.animation.Play("mitakiharahospital_evdoor_l_open_copy");
                m_DoorEvR.animation.Play("mitakiharahospital_evdoor_r_open_copy");
                // さやかたちを歩かせる
                m_Sayaka.animation.Play("sayaka_push_wc_walk_copy");
                m_WheelChair.animation.Play("wheelchair_move_copy");
                m_Sayaka.rigidbody.velocity = new Vector3(2.0f, 0, 0);
                m_WheelChair.rigidbody.velocity = new Vector3(2.0f, 0, 0);
                IncrementXstory();
                break;
            case 17:
                keybreak = true;
                FullClear();
                if (m_Sayaka.transform.position.x > 30)
                {
                    m_camera[0].SetActive(false);
                    m_camera[2].SetActive(true);
                    IncrementXstory();
                }
                else
                {
                    FootSE(m_ses[0], 0.5f, m_Sayaka.transform.position);
                }
                break;
            case 18:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "今のは・・・美樹さやかと上条恭介！？・・・本当にここはあのと";
                m_serif[1] = "きの見滝原なの・・・？";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 19:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "・・・もしこの世界が本当にあのときの見滝原なら・・・まどかに";
                m_serif[1] = "逢える・・・？";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 20:
                keybreak = true;
                FullClear();
                m_camera[2].SetActive(false);
                m_camera[3].SetActive(true);
                m_camera[3].animation.Play("prolouge6_camera4");
                IncrementXstory();
                break;
            case 21:
                keybreak = true;
                FullClear();
                if (m_camera[3].transform.position.x < 4.0f)
                {
                    IncrementXstory();
                }
                break;
            case 22:
                keybreak = false;
                m_facetype = CharacterFace.SCONOSCIUTO_NORMAL;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・・・・・・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 23:
                keybreak = true;
                FullClear();
                IncrementXstory();
                break;
            default:
                // 見滝原病院56階へ飛ばす
                savingparameter.nowField = 6;
                savingparameter.beforeField = 7777;
                Application.LoadLevel("Mitakihara_Hospital_56F");
                savingparameter.story = 3;
                break;
        }
        EventUpdate();
	}
}
