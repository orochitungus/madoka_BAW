using UnityEngine;
using System.Collections;

public class ExEpisode1 : EventBase
{
	// 制御対象となるオブジェクト(フィールドに配置してあるオブジェクトを指定する)
	public GameObject Sayaka;		// さやか
	public GameObject Kyosuke;		// 恭介
	// FaceSelecterを持ったオブジェクト
	public FaceSelecter KyosukeFaceSelecter;
	public FaceSelecter SayakaFaceSelecter;
	// Use this for initialization
	void Start () 
	{
		// カメラ2以降を無効化する
		for (int i = 1; i < m_camera.Length; i++)
		{
			this.m_camera[i].SetActive(false);
		}
		// 共通ステートの初期化を行う
		EventInitialize();
		// BGM再生開始
		AudioManager.Instance.PlayBGM("Kaorimade_rintoshita");
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (xstory)
		{
			case 0:
				keybreak = true;
				FullClear();
				if(m_camera[0].transform.position.z < -2.0f)
				{ 
					IncrementXstory();
				}
				break;
			case 1:
				keybreak = false;
				m_facetype = 0;
				m_drawname_jp = "";
				m_drawname_en = "";
				m_serif[0] = "EXエピソード1";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 2:
				keybreak = true;
				m_camera[0].SetActive(false);
				m_camera[1].SetActive(true);
				Sayaka.animation.Play("sayaka_view_copy");
				FullClear();
				IncrementXstory();
				break;
			case 3:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_NORMAL;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "こりゃまた絶景だねえ。こうやって空から街を見下ろしたことなん";
				m_serif[1] = "てなかったよ";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 4:
				keybreak = true;
				m_camera[1].SetActive(false);
				m_camera[2].SetActive(true);
				Kyosuke.animation.Play("kyosuke_los_look_l_copy");
				FullClear();
				IncrementXstory();
				break;
			case 5:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "たしかにそうだね。うちは一軒家だし、さやかの家もそんなに高い";
				m_serif[1] = "ところにある部屋じゃなかったし";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 6:
				keybreak = true;
				FullClear();
				Sayaka.transform.rotation =  Quaternion.Euler(new Vector3(0.0f,310.3413f,0.0f));
				Sayaka.animation.Play("sayaka_atease_copy");
				SayakaFaceSelecter.ChangeFaceTexture(1);
				m_camera[2].SetActive(false);
				m_camera[3].SetActive(true);
				IncrementXstory();
				break;
			case 7:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_SADNESS;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "うん。そのさ、また、来てもいいかな";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 8:
				keybreak = true;
				FullClear();
				m_camera[3].SetActive(false);
				m_camera[2].SetActive(true);
				IncrementXstory();
				break;
			case 9:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_SMILE;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "もちろんだよ。正直入院生活は退屈だからね。来てくれると嬉しい";
				m_serif[1] = "よ";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 10:
				keybreak = true;
				FullClear();
				SayakaFaceSelecter.ChangeFaceTexture(2);
				m_camera[2].SetActive(false);
				m_camera[3].SetActive(true);
				IncrementXstory();
				break;
			case 11:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_SHY;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "そ、そっか。うん、そうするよ。これからもよろしくね、恭介";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 12:
				keybreak = true;
				FullClear();
				m_camera[3].SetActive(false);
				m_camera[2].SetActive(true);
				IncrementXstory();
				break;
			case 13:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "こちらこそよろしく、さやか";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 14:
				keybreak = true;
				SayakaFaceSelecter.ChangeFaceTexture(0);
				Sayaka.animation.Play("sayaka_yawn_copy");
				m_camera[2].SetActive(false);
				m_camera[3].SetActive(true);
				FullClear();
				IncrementXstory();
				break;
			case 15:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_NORMAL;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "へへ・・・ふあああ・・・";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 16:
				keybreak = true;
				FullClear();
				m_camera[3].SetActive(false);
				m_camera[4].SetActive(true);
				IncrementXstory();
				break;
			case 17:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "眠いのかいさやか？";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 18:
				keybreak = true;
				FullClear();
				SayakaFaceSelecter.ChangeFaceTexture(3);
				IncrementXstory();
				break;
			case 19:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_SMILE;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "あはは、最近徹夜多かったからねえ、生活リズム崩れてるのかも";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 20:
				keybreak = true;
				FullClear();
				m_camera[4].SetActive(false);
				m_camera[2].SetActive(true);
				Kyosuke.animation.Play("kyosuke_puthand_copy");
				IncrementXstory();
				break;
			case 21:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "そっか、事故にあったときから父さんたちと交代でずっとついてて";
				m_serif[1] = "くれたものね。少し寝ていきなよ。後で起こすから";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 22:
				keybreak = true;
				FullClear();
				m_camera[2].SetActive(false);
				m_camera[3].SetActive(true);
				SayakaFaceSelecter.ChangeFaceTexture(2);
				IncrementXstory();
				break;
			case 23:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_SHY;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "え、その・・・";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 24:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_SMILE;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "いいよ。昔は一緒に昼寝してたし今さらだよ、ね";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 25:
				keybreak = true;
				FullClear();
				m_camera[3].SetActive(false);
				m_camera[2].SetActive(true);
				Kyosuke.animation.Play("kyosuke_looklefthand_copy");
				IncrementXstory();
				break;
			case 26:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "・・・・・？";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 27:
				keybreak = true;
				FullClear();
				m_camera[2].SetActive(false);
				m_camera[4].SetActive(true);
				SayakaFaceSelecter.ChangeFaceTexture(0);
				Sayaka.animation.Play("sayaka_uniform_neutral_copy");
				IncrementXstory();
				break;
			case 28:
				keybreak = false;
				m_facetype = CharacterFace.SAYAKA_NORMAL;
				m_drawname_jp = "美樹さやか";
				m_drawname_en = "MIKI SAYAKA";
				m_serif[0] = "恭介？どしたの？";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 29:
				keybreak = true;
				FullClear();
				IncrementXstory();
				break;
			case 30:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_NORMAL;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "いや・・・なんでもない";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 31:
				keybreak = true;
				m_camera[4].SetActive(false);
				m_camera[2].SetActive(true);
				KyosukeFaceSelecter.ChangeFaceTexture(2);
				FullClear();
				IncrementXstory();
				break;
			case 32:
				keybreak = false;
				m_facetype = CharacterFace.KYOSUKE_HOSPITAL_HATE;
				m_drawname_jp = "上条恭介";
				m_drawname_en = "KAMIJOU KYOUSUKE";
				m_serif[0] = "（どういうことなんだ・・・左手の・・・感覚が・・・無い？）";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			case 33:
				keybreak = false;
				m_facetype = 0;
				m_drawname_jp = "Information";
				m_drawname_en = "";
				m_serif[0] = "このように、ゲーム中には様々なサブイベントが発生します。次の";
				m_serif[1] = "行動目標は上部のInformationに表示されますが、寄り";
				m_serif[2] = "道をするといいことが起こるかもしれません。積極的に探してみま";
				m_serif[3] = "しょう！";
				break;
			case 34:
				keybreak = false;
				m_facetype = 0;
				m_drawname_jp = "Information";
				m_drawname_en = "";
				m_serif[0] = "アイテム「魔女のひげ薬」を10個入手！";
				m_serif[1] = "";
				m_serif[2] = "";
				m_serif[3] = "";
				break;
			default:
				// アイテムを入手させる
				int itemKind = 0;
				savingparameter.SetItemNum(itemKind, savingparameter.GetItemNum(itemKind) + 10);
				// 見滝原病院106階へ飛ばす
				savingparameter.nowField = 7;
				savingparameter.beforeField = 1102;
				Application.LoadLevel("Mitakihara_Hospital_106F");
                savingparameter.story = 4;
				break;
		}
		EventUpdate();
	}
}
