using UnityEngine;
using System.Collections;

public class Prologue_03 : EventBase
{
    // カメラモードのEnum
    private enum CameraMode
    {
        INTRO,      // 膝をつくほむら
        SCONO_SIDE, // スコノの斜め前
        HIGH        // 二人の上空
    };

    

    // BGM
    public AudioClip m_Wind;       // 風音

    private AudioSource m_BGM;

    // SE
    public AudioClip m_Down;        // ほむらが膝をついてダウンする
    public AudioClip m_Gass;        // スコノが瘴気を放つ時のSE

    // キャラクター
    public GameObject m_Homura;     // ほむら
    public GameObject m_Scono;      // スコノシュート
    public GameObject m_gass;       // ガスエフェクト

	// Use this for initialization
	void Start () 
    {
        // カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].SetActive(false);
        }
               
        EventInitialize();
		// BGM再生開始
		AudioManager.Instance.PlayBGM("Kankyou Kaze01-1");
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
                AudioSource.PlayClipAtPoint(m_Down, m_Homura.transform.position);
                m_Homura.animation.Play("homura_kneel_copy");
                xstory++;
                break;
            case 1:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_PAIN;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "ふふ、ここまでね・・・今となっては貴方に負けて消えるのもいい";
                m_serif[1] = "わ・・・";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 2:
                keybreak = true;
                this.m_camera[0].SetActive(false);
                this.m_camera[1].SetActive(true);
                m_Scono.animation.Play("Scono_put_hand_ahead_copy");
                xstory++;
                break;
            case 3:                
                keybreak = false;
                m_facetype = CharacterFace.SCONOSCIUTO_WRITE_NORMAL;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・・・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 4:
                keybreak = true;
                this.m_camera[1].SetActive(false);
                this.m_camera[2].SetActive(true);
                m_gass.transform.position = new Vector3(m_Homura.transform.position.x, m_Homura.transform.position.y, m_Homura.transform.position.z);
                AudioSource.PlayClipAtPoint(m_Gass, m_gass.transform.position);
                m_gass.GetComponent<ParticleSystem>().Play();
                FullClear();
                IncrementXstory();
                break;
            case 5:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_PAIN;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "さようなら、名も知らないだれか・・・今行くわ・・・まどか";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            default:
                // プロローグ４へ飛ばす
                m_facetype = 0;
                m_drawname_jp = "";
                m_drawname_en = "";
                for (int i = 0; i < m_serif.Length; i++)
                {
                    m_serif[i] = "";
                }
                savingparameter.beforeField = 0;
				FadeManager.Instance.LoadLevel("Prologue4", 1.0f);
                break;
        }
        EventUpdate();
	}
}
