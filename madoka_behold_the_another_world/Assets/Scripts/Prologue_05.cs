using UnityEngine;
using System.Collections;

public class Prologue_05 : EventBase
{
    // BGM
    public AudioClip m_kusabi;
    
    private AudioSource m_BGM;

    // SE
    public AudioClip m_Appear;

    // キャラクター
    public GameObject m_Homura;         // ほむら
    public GameObject m_Homura_Uniform; // 制服ほむら

    // 変身解除時のフレア
    public GameObject m_Frare;

	// Use this for initialization
	void Start () 
    {
        // カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].SetActive(false);
        }
        
        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にAudioManagerがつくので消しておく
            // BGM再生開始(この前と共通のためifの中）
            AudioManager.Instance.PlayBGM("kusabi");
        }
       
        EventInitialize();
	}
	
	// Update is called once per frame
	void Update () 
    {
        switch (xstory)
        {
            case 0:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_SURPRISE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "病院・・・！？";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 1:
                keybreak = true;
                FullClear();
                m_camera[0].SetActive(false);
                m_camera[1].SetActive(true);
                IncrementXstory();
                break;
            case 2:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_SURPRISE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "そんな、私はもう時間遡航はできないはずなのに・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 3:
                keybreak = true;
                FullClear();
                m_Homura.animation.Play("homura_hold_the_head_copy");
                IncrementXstory();
                break;
            case 4:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_PAIN;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "・・・まどかのリボンがない、それに、この姿は前と同じ・・・？";
                m_serif[1] = "まどか・・・どういうことなの？";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 5:
                keybreak = true;
                FullClear();
                m_Frare.transform.position = new Vector3(0, 0.88f, 0);
                m_Homura.transform.position = new Vector3(0, -100, 0);
                AudioSource.PlayClipAtPoint(m_Appear, m_Frare.transform.position);
                IncrementXstory();
                break;
            case 6:
                keybreak = true;
                StartCoroutine("TimeWait",1.0f);                
                break;
            case 7:
                keybreak = true;
                StopCoroutine("TimeWait");          // StartCoroutineで動かした関数はStopCorutineを使わないと無限に動き続けるので、用済みになった時点でStopCoroutineを呼んで殺すこと
                m_Homura_Uniform.transform.position = Vector3.zero;
                m_Frare.transform.position = new Vector3(0, -2, 0);
                IncrementXstory();
                break;
            case 8:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "とりあえず情報収集が先かしら・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 9:
                keybreak = true;
                FullClear();
                //m_camera[1].SetActive(false);
                //m_camera[2].SetActive(true);
                IncrementXstory();
                break;
            default:
                // 見滝原病院・ほむらの部屋へ飛ばす
                m_facetype = 0;
                m_drawname_jp = "";
                m_drawname_en = "";
                for (int i = 0; i < m_serif.Length; i++)
                {
                    m_serif[i] = "";
                }
                savingparameter.nowField = 5;
                savingparameter.beforeField = 8888;
                // パーティーを銃ほむらにする
                savingparameter.SetNowParty(0, (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA);
				FadeManager.Instance.LoadLevel("Mitakihara_Hospital_HomuraRoom", 1.0f);
                savingparameter.story = 2;
                break;
            //case 10:
            //    keybreak = false;
            //    m_serif[0] = "プレイありがとうございました！";
            //    m_serif[1] = "今回はここまでとなります";
            //    m_serif[2] = "ご意見、ご感想は「http://page.freett.com/tungus/index.htm」";
            //    m_serif[3] = "までお願いします！";               
            //    break;
            //default:
            //    // タイトルへ飛ばす
            //    m_facetype = 0;
            //    m_drawname_jp = "";
            //    m_drawname_en = "";
            //    for (int i = 0; i < m_serif.Length; i++)
            //    {
            //        m_serif[i] = "";
            //    }
            //    savingparameter.beforeField = 0;
            //    Application.LoadLevel("title");
            //    break;
        }
        EventUpdate();
	}
}
