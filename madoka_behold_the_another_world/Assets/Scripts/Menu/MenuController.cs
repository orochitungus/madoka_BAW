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
	/// 強化するステータスの暫定値
	/// </summary>
	private int StatusInterim;

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
	private GameObject ItemWindow;

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

	/// <summary>
	/// ステータス画面のY座標
	/// </summary>
	private float STATUSWINDOWYPOS = 200;

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
						case (int)Menumode.STATUS:
							Menucontrol = MenuControl.STATUSCHARSELECT;
							break;
						case (int)Menumode.ITEM:
							// 選択するとITEMへ移行
							AudioManager.Instance.PlaySE("OK");
							iTween.MoveTo(Root, iTween.Hash(
								// 移動先指定
								"position", new Vector3(793, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Status呼び出し
								"oncomplete", "InsertItem",
								"oncompletetarget", gameObject
							));
							// 所持アイテム一覧を表示する
							MenuItemDraw menuItemDraw = ItemWindow.GetComponent<MenuItemDraw>();

							// 所持個数を取得し、その分を描画する
							for (int i = 0; i < Item.itemspec.Length; ++i)
							{
								if(savingparameter.GetItemNum(i) > 0)
								{
									menuItemDraw.ItemName[i].text = Item.itemspec[i].Name();
									menuItemDraw.ItemNum[i].text = savingparameter.GetItemNum(i).ToString("d2");
									menuItemDraw.ItemKind[i] = i;
								}
								else
								{
									menuItemDraw.ItemName[i].text = "";
									menuItemDraw.ItemNum[i].text = "";
									menuItemDraw.ItemKind[i] = -1;
								}
							}
							menuItemDraw.NowSelect = 0;
							// ITEMへ移行
							Menucontrol = MenuControl.ITEM;
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
						"position", new Vector3(793, STATUSWINDOWYPOS, 0),
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
				
				// OKで各選択モードに入る
				if(ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					Status.GetComponent<MenuStatus>().SelectMode = true;
					if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.STR)
					{
						// 暫定値に現在のSTRを入れる
						Status.GetComponent<MenuStatus>().StrInterim = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSSTR;
					}
					else if(Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.CON)
					{
						// 暫定値に現在のConを入れる
						Status.GetComponent<MenuStatus>().ConInterim = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSCON;
					}
					else if(Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.VIT)
					{
						// 暫定値に現在のVitを入れる
						Status.GetComponent<MenuStatus>().VitInterim = savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSVIT;
					}
					else if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.DEX)
					{
						// 暫定値に現在のDexを入れる
						Status.GetComponent<MenuStatus>().DexInterim = savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSDEX;
					}
					else if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.AGI)
					{
						// 暫定値に現在のAgiを入れる
						Status.GetComponent<MenuStatus>().AgiInterim = savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSAGI;
					}
				}
				// CANCELでSTATUCHARSELECTに戻る
				else if(ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Status, iTween.Hash(
						// 移動先指定
						"position", new Vector3(793, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));
					
					Menucontrol = MenuControl.STATUSCHARSELECT;
				}
			}
			else if(Menucontrol == MenuControl.STATUSSTR)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().StrInterim, 0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1,0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSSTR);
			}
			else if(Menucontrol == MenuControl.STATUSSTRFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if(PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementDone(MenuControl.STATUSSTRFINALCONFIRM);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.STATUSSTR;
					}
				}
			}
			else if(Menucontrol == MenuControl.STATUSCON)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().ConInterim, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSCON);
			}
			else if(Menucontrol == MenuControl.STATUSCONFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementDone(MenuControl.STATUSCONFINALCONFIRM);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.STATUSCON;
					}
				}
			}
			else if(Menucontrol == MenuControl.STATUSVIT)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().VitInterim, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSVIT);
			}
			else if(Menucontrol == MenuControl.STATUSVITFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementDone(MenuControl.STATUSVITFINALCONFIRM);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.STATUSVIT;
					}
				}
			}
			else if(Menucontrol == MenuControl.STATUSDEX)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().DexInterim, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSDEX);
			}
			else if(Menucontrol == MenuControl.STATUSDEXFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementDone(MenuControl.STATUSDEXFINALCONFIRM);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.STATUSDEX;
					}
				}
			}
			else if (Menucontrol == MenuControl.STATUSAGI)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().AgiInterim, 0, savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSAGI);
			}
			else if (Menucontrol == MenuControl.STATUSAGIFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementDone(MenuControl.STATUSAGIFINALCONFIRM);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.STATUSAGI;
					}
				}
			}
			// 装備アイテム選択
			else if(Menucontrol == MenuControl.ITEM)
			{
				KeyInputController(ref ItemWindow.GetComponent<MenuItemDraw>().NowSelect, ref Dummy, Item.itemspec.Length, 0);
				// 選択の場合ポップアップを出す
				if(ControllerManager.Instance.Shot)
				{
					// アイテムの種類を取得(ないところは-1になっている）
					if(ItemWindow.GetComponent<MenuItemDraw>().ItemKind[ItemWindow.GetComponent<MenuItemDraw>().NowSelect] >= 0)
					{
						
					}
				}
				// キャンセル
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(ItemWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(793, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));

					Menucontrol = MenuControl.ROOT;
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
					InformationText.text = "攻撃力を表します。一回の攻撃に与えられるダメージに影響します\n選択して左右で増減し、ショットキーで決定できます";
					break;
				case (int)StatusKind.CON:
					InformationText.text = "集中力を表します。マジックバーストの維持時間に影響します\n選択して左右で増減し、ショットキーで決定できます";
					break;
				case (int)StatusKind.VIT:
					InformationText.text = "防御力を表します。一回の攻撃を受けた時のダメージ軽減量に影響します\n選択して左右で増減し、ショットキーで決定できます";
					break;
				case (int)StatusKind.DEX:
					InformationText.text = "器用さを表します。各種武装の残弾数や持続時間に影響します\n選択して左右で増減し、ショットキーで決定できます";
					break;
				case (int)StatusKind.AGI:
					InformationText.text = "敏捷性を表します。ブースト量の消費に影響します\n選択して左右で増減し、ショットキーで決定できます";
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
	/// <param name="minLR">上下入力の最大値</param>
	/// <param name="minUD">左右入力の最大値</param>
    private void KeyInputController(ref int variableUD, ref int variableLR,int lengthUD, int lengthLR, int minUD = 0, int minLR = 0)
    {
        // 上
        if (!_PreTopInput && ControllerManager.Instance.Top)
        {
            if (variableUD > minUD)
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
            if(variableLR > minLR)
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
		iTween.MoveTo(Status, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
	}

	/// <summary>
	/// ルート画面を入れる
	/// </summary>
	private void InsertRoot()
	{
		iTween.MoveTo(Root, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
	}

	/// <summary>
	/// アイテム画面を入れる
	/// </summary>
	private void InsertItem()
	{
		iTween.MoveTo(ItemWindow, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
	}

	/// <summary>
	/// ステータス強化を実行したときの処理
	/// </summary>
	private void StatusReinforcement(MenuControl menucontrol)
	{
		switch (menucontrol)
		{
			case MenuControl.STATUSSTR:
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().StrInterim, 0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)));
				break;
			case MenuControl.STATUSCON:
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().ConInterim, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)));
				break;
			case MenuControl.STATUSVIT:
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().VitInterim, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)));
				break;
			case MenuControl.STATUSDEX:
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().DexInterim, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)));
				break;
			case MenuControl.STATUSAGI:
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().AgiInterim, 0, savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)));
				break;
		}

		// 選択確定時、ポップアップを出して最終確認する
		if (ControllerManager.Instance.Shot)
		{
			iTween.ScaleTo(PopUp, iTween.Hash(
				// 拡大率指定
				"y", 1,
				// 拡大時間指定
				"time", 0.5f
			));
			// PopUpを初期化
			PopUp.GetComponent<MenuPopup>().NowSelect = 1;
			// 文字列を書き込む
			switch (menucontrol)
			{
				case MenuControl.STATUSSTR:
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このように強化されます\n" + savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2") + "->"  + Status.GetComponent<MenuStatus>().StrInterim.ToString("d2") + "\nよろしいですか？";
					Menucontrol = MenuControl.STATUSSTRFINALCONFIRM;
					break;
				case MenuControl.STATUSCON:
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このように強化されます\n" + savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2") + "->"  + Status.GetComponent<MenuStatus>().ConInterim.ToString("d2") + "\nよろしいですか？";
					Menucontrol = MenuControl.STATUSCONFINALCONFIRM;
					break;
				case MenuControl.STATUSVIT:
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このように強化されます\n" + savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2") + "->"  + Status.GetComponent<MenuStatus>().VitInterim.ToString("d2") + "\nよろしいですか？";
					Menucontrol = MenuControl.STATUSVITFINALCONFIRM;
					break;
				case MenuControl.STATUSDEX:
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このように強化されます\n" + savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2") + "->"  + Status.GetComponent<MenuStatus>().DexInterim.ToString("d2") + "\nよろしいですか？";
					Menucontrol = MenuControl.STATUSDEXFINALCONFIRM;
					break;
				case MenuControl.STATUSAGI:
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このように強化されます\n" + savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2") + "->"  + Status.GetComponent<MenuStatus>().AgiInterim.ToString("d2") + "\nよろしいですか？";
					Menucontrol = MenuControl.STATUSAGIFINALCONFIRM;
					break;
			}			
			
		}
		// キャンセル時、元に戻す
		else if (ControllerManager.Instance.Jump)
		{
			AudioManager.Instance.PlaySE("cursor");
			Status.GetComponent<MenuStatus>().SelectMode = false;
			switch (menucontrol)
			{
				case MenuControl.STATUSSTR:
					Status.GetComponent<MenuStatus>().StrInterim = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect));
					break;
				case MenuControl.STATUSCON:
					Status.GetComponent<MenuStatus>().ConInterim = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect));
					break;
				case MenuControl.STATUSVIT:
					Status.GetComponent<MenuStatus>().VitInterim = savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect));
					break;
				case MenuControl.STATUSDEX:
					Status.GetComponent<MenuStatus>().DexInterim = savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect));
					break;
				case MenuControl.STATUSAGI:
					Status.GetComponent<MenuStatus>().AgiInterim = savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect));
					break;
			}
			Menucontrol = MenuControl.STATUS;
		}
	}

	/// <summary>
	/// 選択確定したときの処理
	/// </summary>
	/// <param name="menucontrol"></param>
	private void ReinforcementDone(MenuControl menucontrol)
	{
		AudioManager.Instance.PlaySE("OK");
		Status.GetComponent<MenuStatus>().SelectMode = false;
		iTween.ScaleTo(PopUp, iTween.Hash(
				// 拡大率指定
				"y", 0,
				// 拡大時間指定
				"time", 0.5f
			));
		Menucontrol = MenuControl.STATUS;
		switch (menucontrol)
		{
			case MenuControl.STATUSSTRFINALCONFIRM:
				{
					// 元のステータス
					int strOR = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect));
					// ステータスを新しい値に変える
					savingparameter.SetStrLevel(savingparameter.GetNowParty(CharSelect), Status.GetComponent<MenuStatus>().StrInterim);
					// スキルポイントを使った分減らす
					int usedskillpoint = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)) - strOR;
					int skillpointnext = savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) - usedskillpoint;
					savingparameter.SetSkillPoint(savingparameter.GetNowParty(CharSelect), skillpointnext);
					// 表記を反映させる
					Status.GetComponent<MenuStatus>().Str.text = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2");
					Status.GetComponent<MenuStatus>().SkillPoint.text = skillpointnext.ToString("d2");
				}
				break;
			case MenuControl.STATUSCONFINALCONFIRM:
				{
					// 元のステータス
					int conOR = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect));
					// ステータスを新しい値に変える
					savingparameter.SetArousalLevel(savingparameter.GetNowParty(CharSelect), Status.GetComponent<MenuStatus>().ConInterim);
					// スキルポイントを使った分減らす
					int useskillpoint = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)) - conOR;
					int skillpointnext = savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) - useskillpoint;
					// 表記を反映させる
					Status.GetComponent<MenuStatus>().Con.text = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2");
					Status.GetComponent<MenuStatus>().SkillPoint.text = skillpointnext.ToString("d2");
				}
				break;
			case MenuControl.STATUSVITFINALCONFIRM:
				{
					// 元のステータス
					int vitOR = savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect));
					// ステータスを新しい値に変える
					savingparameter.SetDefLevel(savingparameter.GetNowParty(CharSelect), Status.GetComponent<MenuStatus>().VitInterim);
					// スキルポイントを使った分減らす
					int useskillpoint = savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)) - vitOR;
					int skillpointnext = savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) - useskillpoint;
					// 表記を反映させる
					Status.GetComponent<MenuStatus>().Vit.text = savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2");
					Status.GetComponent<MenuStatus>().SkillPoint.text = skillpointnext.ToString("d2");
				}
				break;
			case MenuControl.STATUSDEXFINALCONFIRM:
				{
					// 元のステータス
					int dexOR = savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect));
					// ステータスを新しい値に変える
					savingparameter.SetBulLevel(savingparameter.GetNowParty(CharSelect), Status.GetComponent<MenuStatus>().DexInterim);
					// スキルポイントを使った分減らす
					int useskillpoint = savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)) - dexOR;
					int skillpointnext = savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) - useskillpoint;
					// 表記を反映させる
					Status.GetComponent<MenuStatus>().Dex.text = savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2");
					Status.GetComponent<MenuStatus>().SkillPoint.text = skillpointnext.ToString("d2");
				}
				break;
			case MenuControl.STATUSAGIFINALCONFIRM:
				{
					// 元のステータス
					int agiOR = savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect));
					// ステータスを新しい値に変える
					savingparameter.SetBoostLevel(savingparameter.GetNowParty(CharSelect), Status.GetComponent<MenuStatus>().AgiInterim);
					// スキルポイントを使った分減らす
					int useskillpoint = savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)) - agiOR;
					int skillpointnext = savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) - useskillpoint;
					// 表記を反映させる
					Status.GetComponent<MenuStatus>().Agi.text = savingparameter.GetBoostLevel(savingparameter.GetNowParty(CharSelect)).ToString("d2");
					Status.GetComponent<MenuStatus>().SkillPoint.text = skillpointnext.ToString("d2");
				}
				break;
		}
	}

	/// <summary>
	/// 選択キャンセルした時の処理
	/// </summary>
	private void ReinforcementCancel()
	{
		AudioManager.Instance.PlaySE("cursor");
		iTween.ScaleTo(PopUp, iTween.Hash(
				// 拡大率指定
				"y", 0,
				// 拡大時間指定
				"time", 0.5f
			));
	}
}

/// <summary>
/// 現在の操作モード
///  </summary>
public enum MenuControl
{
    ROOT,                   // ルート
    STATUS,                 // ステータス
	STATUSCHARSELECT,		// ステータスキャラ選択
	STATUSPOINTSELECT,		// ステータスポイント選択
	STATUSSTR,				// STR選択
	STATUSSTRFINALCONFIRM,	// STR最終確認
	STATUSCON,				// CON選択
	STATUSCONFINALCONFIRM,	// CON最終確認
	STATUSVIT,				// VIT選択
	STATUSVITFINALCONFIRM,	// VIT最終確認
	STATUSDEX,				// DEX選択
	STATUSDEXFINALCONFIRM,	// DEX最終確認
	STATUSAGI,				// AGI選択
	STATUSAGIFINALCONFIRM,	// AGI最終確認
    ITEM,                   // アイテム
	ITEMFINALCONFIRM,		// アイテム最終確認
    SKILL,                  // スキル
    SYSTEM,                 // システム
    PARTY,                  // パーティー
    SAVE,                   // セーブ
    LOAD,                   // ロード
    TITLE                   // タイトル
}

public enum Menumode
{
	STATUS,
	ITEM,                   // アイテム
	SKILL,                  // スキル
	SYSTEM,                 // システム
	PARTY,                  // パーティー
	SAVE,                   // セーブ
	LOAD,                   // ロード
	TITLE                   // タイトル
}
