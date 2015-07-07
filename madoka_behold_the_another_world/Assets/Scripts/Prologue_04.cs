using UnityEngine;
using System.Collections;

// 
public class Prologue_04 : EventBase 
{
   
    // BGM
    public AudioClip m_kusabi;

    private AudioSource m_BGM;

    // SE
    public AudioClip m_Appear;  // まどかがテレポートしてくる時のフレアと同じSE（スコノの白衣着用と共通）

    // キャラクター
    public GameObject m_Homura;     // ほむら
    public GameObject m_UMadoka;    // アルティメットまどか

    // まどかテレポート時のフレア
    public GameObject m_Frare;

	// Use this for initialization
	void Start () 
    {
        // カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].SetActive(false);
        }
        // ほむらのポーズを変更
        m_Homura.animation.Play("homura_float_copy");
        EventInitialize();
		// BGM再生開始
		AudioManager.Instance.PlayBGM("kusabi");
	}
	
	// Update is called once per frame
	void Update () 
    {
        switch (xstory)
        {
            case 0:
                keybreak = true;
                FullClear();
                //m_BGM.clip = m_kusabi;        // 音色をチャンネルに紐付け
                //m_BGM.volume = 0.5f;
                //m_BGM.Play();               // 再生開始
                //m_BGM.loop = true;
                IncrementXstory();
                break;
            case 1:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_SADNESS;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "これで会えるわね、まどか・・・";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 2:
                keybreak = true;
                FullClear();
                m_camera[0].SetActive(false);
                m_camera[1].SetActive(true);
                AudioSource.PlayClipAtPoint(m_Appear, m_Frare.transform.position);
                IncrementXstory();
                break;
            case 3:
                keybreak = false;
                m_facetype = 0;
                m_drawname_jp = "？？";
                m_drawname_en = "";
                m_serif[0] = "・・・魔法少女・・・を・・・集めて・・・ほむらちゃん・・・私";
                m_serif[1] = "の元へ・・・たどり着いて";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 4:
                keybreak = true;
                FullClear();
                m_UMadoka.transform.position = new Vector3(0, -0.01029837f, 6.361344f);
                m_Frare.transform.position = new Vector3(0, -10000, 0);
                AudioSource.PlayClipAtPoint(m_Appear, m_Frare.transform.position);
                IncrementXstory();
                break;
            case 5:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_SURPRISE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "まどか！？たどり・・・着く？";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 6:
                keybreak = true;
                FullClear();
                m_camera[1].SetActive(false);
                m_camera[2].SetActive(true);
                IncrementXstory();
                break;
            case 7:
                keybreak = false;
                m_facetype = CharacterFace.ULTIMATE_MADOKA_HATE;
                m_drawname_jp = "鹿目まどか";
                m_drawname_en = "KANAME MADOKA";
                m_serif[0] = "私たちが出会った，あの時間，あの世界・・・そこで・・・みんな";
                m_serif[1] = "と・・・私の元に";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 8:
                keybreak = true;
                FullClear();
                m_Homura.animation.Play("homura_float_put_hand_ahead_copy");
                IncrementXstory();
                break;
            case 9:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_SADNESS;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "そんな・・・！？";
                m_serif[1] = "私は何をすれば・・・";
                m_serif[2] = "";
                m_serif[3] = "";                
                break;
            case 10:
                keybreak = true;
                FullClear();
                m_Frare.transform.position = new Vector3(0, 2.859508f, 0);
                AudioSource.PlayClipAtPoint(m_Appear, m_Frare.transform.position);
                IncrementXstory();
                break;
            case 11:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_PAIN;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "きゃ・・・！？";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 12:
                keybreak = true;
                FullClear();
                m_Homura.transform.position = new Vector3(0,-10000,0);
                AudioSource.PlayClipAtPoint(m_Appear, m_Frare.transform.position);
                IncrementXstory();
                break;
            case 13:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_MAGICA_PAIN;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "まどかぁぁぁぁぁ！";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 14:
                m_camera[1].SetActive(true);
                m_camera[2].SetActive(false);
                keybreak = false;
                m_facetype = CharacterFace.ULTIMATE_MADOKA_PAIN;
                m_drawname_jp = "鹿目まどか";
                m_drawname_en = "KANAME MADOKA";
                m_serif[0] = "・・・・・・・・・・・";
                m_serif[1] = "私には、ここまでしかできない";
                m_serif[2] = "今は、「乗せられる」しかない・・・";
                m_serif[3] = "ごめんね、ほむらちゃん・・・";
                break;
            default:
                // プロローグ５へ飛ばす
                m_facetype = 0;
                m_drawname_jp = "";
                m_drawname_en = "";
                for (int i = 0; i < m_serif.Length; i++)
                {
                    m_serif[i] = "";
                }
                savingparameter.beforeField = 0;
				FadeManager.Instance.LoadLevel("Prologue5", 1.0f);
                break;
        }
        EventUpdate();
	}
}
