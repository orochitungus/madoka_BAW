using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// 現在のモード
    /// </summary>
    private MenuControl Menucontrol;

    /// <summary>
    /// 選択時のスプライト
    /// </summary>
    [SerializeField]
    private Sprite SelectedSprite;

    /// <summary>
    /// 非選択時のスプライト
    /// </summary>
    [SerializeField]
    private Sprite UnSelectedSprite;

    /// <summary>
    /// MenuBarの画像
    /// </summary>
    [SerializeField]
    private Image[] Menubar;

    /// <summary>
    /// キャラクターステート表示部
    /// </summary>
    [SerializeField]
    private CharacterStatusRoot[] Characterstatusroot;

    /// <summary>
    /// MenuBarのどれが選択されているか
    /// </summary>
    private int MenuBarSelect;

	/// <summary>
	/// キャラクターの誰が選択されているか
	/// </summary>
	private int CharSelect;

    /// <summary>
    /// MenuBarInputで上下のみもしくは左右のみの入力の場合に入れるダミー変数
    /// </summary>
    private int Dummy;

    // 1F前の上入力
    private bool _PreTopInput;
    // 1F前の下入力
    private bool _PreUnderInput;

    // 1F前の左入力
    private bool _PreLeftInput;
    // 1F前の右入力
    private bool _PreRightInput;

    /// <summary>
	/// モードチェンジしたばかりか否か
	/// </summary>
	private bool ModeChangeDone;

    /// <summary>
    /// 各キャラの顔
    /// </summary>
    [SerializeField]
    private Sprite MadokaFace;
    [SerializeField]
    private Sprite SayakaFace;
    [SerializeField]
    private Sprite HomuraFace;
    [SerializeField]
    private Sprite MamiFace;
    [SerializeField]
    private Sprite KyokoFace;
    [SerializeField]
    private Sprite YumaFace;
    [SerializeField]
    private Sprite KirikaFace;
    [SerializeField]
    private Sprite OrikoFace;
    [SerializeField]
    private Sprite SconosciutoFace;
    [SerializeField]
    private Sprite HomuraBowFace;
    [SerializeField]
    private Sprite UltimateMadokaFace;
	// 追加があったらこの下に追加


	// インフォメーションのテキスト
	[SerializeField]
	private Text InformationText;

	/// <summary>
	/// ルート画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Root;

	/// <summary>
	/// ステータス画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Status;

	/// <summary>
	/// アイテム画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Item;

	/// <summary>
	/// スキル画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Skill;

	/// <summary>
	/// システム画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject System;

	/// <summary>
	/// パーティー画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject Party;

	/// <summary>
	/// セーブロード画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject SaveLoad;

	/// <summary>
	/// ポップアップ
	/// </summary>
	[SerializeField]
	private GameObject PopUp;

    void Awake()
    {
        // ControllerManagerがあるか判定
        if (GameObject.Find("ControllerManager") == null)
        {
            // 無ければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
            loadManager.name = "ControllerManager";
        }

        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   
        }

        // 初期化
        Initialize();
    }

	// Use this for initialization
	void Start ()
    {
        // アップキーストリーム（押下）
        var upkeydownstream = this.UpdateAsObservable().Where(_ => ControllerManager.Instance.Top);
        // アップキーダウンストリーム(解除)
        var upkeyupstream = this.UpdateAsObservable().Where(_ => !ControllerManager.Instance.Top);
        // ダウンキーストリーム(押下）
        var downkeydownstram = this.UpdateAsObservable().Where(_ => ControllerManager.Instance.Under);
        // ダウンキーストリーム（解除）
        var downkeyupstream = this.UpdateAsObservable().Where(_ => !ControllerManager.Instance.Under);

        // 長押し判定(1秒以上で上か下へ強制移動)
        // 上
        upkeydownstream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
        // 途中で離されたらストリームをリセット
        .TakeUntil(upkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
        {
            _PreTopInput = false;
        });
        // 下
        downkeydownstram.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
        // 途中で離されたらストリームをリセット
        .TakeUntil(downkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
        {
            _PreUnderInput = false;
        });
        // 短押し判定(前フレーム押してない）		
        this.UpdateAsObservable().Subscribe(_ =>
        {
            // 各モードごとに操作する変数を変える
            if(Menucontrol == MenuControl.ROOT)
            {
                KeyInputController(ref MenuBarSelect, ref Dummy, Menubar.Length, 0);
				// 選択により各モードへ移行
				if(ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					switch (MenuBarSelect)
					{
						case 0:
							Menucontrol = MenuControl.STATUSCHARSELECT;
							break;
					}
				}
            }
			else if(Menucontrol == MenuControl.STATUSCHARSELECT)
			{
				KeyInputController(ref Dummy, ref CharSelect, 0, savingparameter.GetNowPartyNum());
				// 選択するとSTATUSに移行
				if(ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Root, iTween.Hash(
						// 移動先指定
						"position", new Vector3(793,174,0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Status呼び出し
						"oncomplete","InsertStatus",
						"oncompletetarget", gameObject
					));					
					// 選択したキャラのSTATUSを表示する
					// 選択したキャラは誰？
					int selectedCharacter = savingparameter.GetNowParty(CharSelect);
					// Statusの中身を書き換える
					Status.GetComponent<MenuStatus>().Initialize(selectedCharacter);

					// STATUSへ移行
					Menucontrol = MenuControl.STATUS;
				}
				// CANCELでROOTに戻る
				else if(ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					for (int i = 0; i<MadokaDefine.MAXPARTYMEMBER; i++)
					{
						Characterstatusroot[i].Frame.color = new Color(255, 255, 255);						
					}
					Menucontrol = MenuControl.ROOT;
				}
			}
			else if(Menucontrol == MenuControl.STATUS)
			{
				// 強化したい項目を選ぶ
				KeyInputController(ref Status.GetComponent<MenuStatus>().NowSelect, ref Dummy, (int)StatusKind.TOTALSTATUSNUM, 0);
				// SHOTでポップアップを出し,強化ポイントを割り振らせる
				if(ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.ScaleTo(PopUp, iTween.Hash(
						// 拡大率指定
						"x", 1,
						"y", 1,
						// 拡大時間指定
						"time",0.5f
					));
				}
				// CANCELでSTATUCHARSELECTに戻る
				else if(ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Status, iTween.Hash(
						// 移動先指定
						"position", new Vector3(793, 174, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));

					Menucontrol = MenuControl.STATUSCHARSELECT;
				}
			}
        });

        // ルート状態
        this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.ROOT).Subscribe(_ =>
         {
             for(int i=0; i<Menubar.Length; i++)
             {
                 if(i==MenuBarSelect)
                 {
                     Menubar[i].sprite = SelectedSprite;
                 }
                 else
                 {
                     Menubar[i].sprite = UnSelectedSprite;
                 }
             }
			 // インフォメーションテキストの内容
			 switch (MenuBarSelect)
			 {
				 case 0:        // STATUS
					InformationText.text = "スキルポイントを割り振り、キャラクターを強化します";
					 break;
				 case 1:        // ITEM
					 InformationText.text = "装備するアイテムを決定します";
					 break;
				 case 2:        // SKILL	
					 InformationText.text = "コマンドを確認します";
					 break;
				 case 3:        // SYSTEM
					 InformationText.text = "ゲームの設定を変更します";
					 break;
				 case 4:        // PARTY
					 InformationText.text = "パーティーを変更します";
					 break;
				 case 5:        // SAVE
					 InformationText.text = "ゲームの進行を保存します";
					 break;
				 case 6:        // LOAD
					 InformationText.text = "ゲームの進行を呼び出します";
					 break;
				 case 7:        // TITLE
					 InformationText.text = "ゲームを終了し、タイトルに戻ります";
					 break;			
			 }     
         });

		// STATUSCHARASELECT状態
		this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.STATUSCHARSELECT).Subscribe(_ =>
		{
			for (int i = 0; i<MadokaDefine.MAXPARTYMEMBER; i++)
			{
				if (i==CharSelect)
				{
					Characterstatusroot[i].Frame.color = new Color(255, 0, 0);
				}
				else
				{
					Characterstatusroot[i].Frame.color = new Color(255, 255, 255);
				}
			}
			// インフォメーションテキストの内容
			InformationText.text = "強化したいキャラクターを選んでください";
		});

		// STATUS状態
		this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.STATUS).Subscribe(_ =>
		{
			// インフォメーションテキストの内容
			switch (Status.GetComponent<MenuStatus>().NowSelect)
			{
				case (int)StatusKind.STR:
					InformationText.text = "強化したい項目を選択してください\n攻撃力を表します。一回の攻撃に与えられるダメージに影響します";
					break;
				case (int)StatusKind.CON:
					InformationText.text = "強化したい項目を選択してください\n集中力を表します。マジックバーストの維持時間に影響します";
					break;
				case (int)StatusKind.VIT:
					InformationText.text = "強化したい項目を選択してください\n防御力を表します。一回の攻撃を受けた時のダメージ軽減量に影響します";
					break;
				case (int)StatusKind.DEX:
					InformationText.text = "強化したい項目を選択してください\n器用さを表します。各種武装の残弾数や持続時間に影響します";
					break;
				case (int)StatusKind.AGI:
					InformationText.text = "強化したい項目を選択してください\n敏捷性を表します。ブースト量の消費に影響します";
					break;
			}
		});

    }
	
	// Update is called once per frame
	void Update ()
    {
		
        
	}

    void OnEnable()
    {
        // 初期化
        Initialize();
    }

    /// <summary>
    /// 初期化を行う
    /// </summary>
    void Initialize()
    {
		// TODO:テスト用
		savingparameter.savingparameter_Init();


		// モードをROOTにする
		Menucontrol = MenuControl.ROOT;
        // MenubarのSTATUSを選択状態にする
       for(int i=0; i<Menubar.Length; i++)
        { 
            if(i==0)
            {
                Menubar[i].sprite = SelectedSprite;
            }
            else
            {
                Menubar[i].sprite = SelectedSprite;
            }
        }
        MenuBarSelect = 0;
		CharSelect = 0;

        // STATUSのキャラにデータを入れる
        for(int i=0; i<Characterstatusroot.Length; i++)
        {
            // 誰？
            int characterindex = savingparameter.GetNowParty(i);
            // NONEだった場合項目を消す
            if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_NONE)
            {
                Characterstatusroot[i].gameObject.SetActive(false);
            }            
            else
            {
				// キャラの顔を出す
				// まどか
				if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
                {
                    Characterstatusroot[i].CharacterFace.sprite = MadokaFace;
                }
                // さやか
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = SayakaFace;
				}
                // ほむら
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
				{
					Characterstatusroot[i].CharacterFace.sprite = HomuraFace;
				}
                // マミ
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MAMI)
				{
					Characterstatusroot[i].CharacterFace.sprite = MamiFace;
				}
                // 杏子
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_KYOKO)
				{
					Characterstatusroot[i].CharacterFace.sprite = KyokoFace;
				}
                // ゆま
				else if(characterindex ==(int)Character_Spec.CHARACTER_NAME.MEMBER_YUMA)
				{
					Characterstatusroot[i].CharacterFace.sprite = YumaFace;
				}
                // キリカ
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = KirikaFace;
				}
                // 織莉子
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_ORIKO)
				{
					Characterstatusroot[i].CharacterFace.sprite = OrikoFace;
				}
                // 弓ほむら
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
				{
					Characterstatusroot[i].CharacterFace.sprite = HomuraBowFace;
				}
                // スコノシュート
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO)
				{
					Characterstatusroot[i].CharacterFace.sprite = SconosciutoFace;
				}
                // アルティメットまどか
				else if(characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = UltimateMadokaFace;
				}
                // 円環のさやか

                // デビルほむら

                // なぎさ

                // ミッチェル

                // 上記以外は消す(テスト用の魔獣など）
				else
				{
					Characterstatusroot[i].CharacterFace.sprite = null;
				}
				// キャラの名前
				Characterstatusroot[i].CharacterNameJP.text = Character_Spec.Name[savingparameter.GetNowParty(i)];
				Characterstatusroot[i].CharacterNameEN.text = Character_Spec.NameEn[savingparameter.GetNowParty(i)];

				// レベル
				Characterstatusroot[i].CharacterLevel.text = "Level-" + savingparameter.GetNowLevel(savingparameter.GetNowParty(i)).ToString("d2");

				// HP
				Characterstatusroot[i].CharacterNowHP.text = savingparameter.GetNowHP(savingparameter.GetNowParty(i)).ToString("d4");
				Characterstatusroot[i].CharacterMaxHP.text = savingparameter.GetMaxHP(savingparameter.GetNowParty(i)).ToString("d4");

				// Magic
				Characterstatusroot[i].CharacterNowMagic.text = ((int)savingparameter.GetNowArousal(savingparameter.GetNowParty(i))).ToString("d4");
				Characterstatusroot[i].CharacterMaxMagic.text = ((int)savingparameter.GetMaxArousal(savingparameter.GetNowParty(i))).ToString("d4");

				// Str
				Characterstatusroot[i].CharacterNowStr.text = savingparameter.GetStrLevel(savingparameter.GetNowParty(i)).ToString("d2");
				// Con
				Characterstatusroot[i].CharacterNowCon.text = savingparameter.GetArousalLevel(savingparameter.GetNowParty(i)).ToString("d2");
				// Vit
				Characterstatusroot[i].CharacterNowVit.text = savingparameter.GetDefLevel(savingparameter.GetNowParty(i)).ToString("d2");
				// Dex
				Characterstatusroot[i].CharacterNowDex.text = savingparameter.GetBulLevel(savingparameter.GetNowParty(i)).ToString("d2");
				// Agi
				Characterstatusroot[i].CharacterNowAgi.text = savingparameter.GetBoostLevel(savingparameter.GetNowParty(i)).ToString("d2");
			}
        }

    }

    /// <summary>
    /// キー入力に応じて任意の変数を増減させる
    /// </summary>
    /// <param name="variableUD">上下入力で変化させる変数</param>
    /// <param name="variableLR">左右入力で変化させる変数</param>
    /// <param name="lengthUD">上下入力の最大値</param>
    /// <param name="lengthLR">左右入力の最大値</param>
    private void KeyInputController(ref int variableUD, ref int variableLR,int lengthUD, int lengthLR)
    {
        // 上
        if (!_PreTopInput && ControllerManager.Instance.Top)
        {
            if (variableUD > 0)
            {
                AudioManager.Instance.PlaySE("cursor");
                variableUD--;
            }
        }
        // 下
        if (!_PreUnderInput && ControllerManager.Instance.Under)
        {
            if (variableUD < lengthUD - 1)
            {
                AudioManager.Instance.PlaySE("cursor");
                variableUD++;
            }
        }

        // 左
        if(!_PreLeftInput && ControllerManager.Instance.Left)
        {
            if(variableLR > 0)
            {
                AudioManager.Instance.PlaySE("cursor");
                variableLR--;
            }
        }

        // 右
        if(!_PreRightInput && ControllerManager.Instance.Right)
        {
            if(variableLR < lengthLR - 1)
            {
                AudioManager.Instance.PlaySE("cursor");
                variableLR++;
            }
        }

        // 入力更新
        // 上
        _PreTopInput = ControllerManager.Instance.Top;
        // 下
        _PreUnderInput = ControllerManager.Instance.Under;
        // 左
        _PreLeftInput = ControllerManager.Instance.Left;
        // 右
        _PreRightInput = ControllerManager.Instance.Right;
        // ボタンを離すとモードチェンジ瞬間フラグを折る(一旦ボタンを離さないと次へ行かせられない）
        if (!ControllerManager.Instance.Shot)
        {
            ModeChangeDone = false;
        }
    }

	/// <summary>
	/// ステータス画面を入れる
	/// </summary>
	private void InsertStatus()
	{
		iTween.MoveTo(Status, new Vector3(320, 174, 0), 0.5f);
	}

	/// <summary>
	/// ルート画面を入れる
	/// </summary>
	private void InsertRoot()
	{
		iTween.MoveTo(Root, new Vector3(320, 174, 0), 0.5f);
	}
}

/// <summary>
/// 現在MenuBarのどれが選択されているか
/// </summary>
public enum MenuControl
{
    ROOT,                   // ルート
    STATUS,                 // ステータス
	STATUSCHARSELECT,		// ステータスキャラ選択
	STATUSPOINTSELECT,		// ステータスポイント選択
    ITEM,                   // アイテム
    SKILL,                  // スキル
    SYSTEM,                 // システム
    PARTY,                  // パーティー
    SAVE,                   // セーブ
    LOAD,                   // ロード
    TITLE                   // タイトル
}
