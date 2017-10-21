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
    public MenuControl Menucontrol;

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
	[SerializeField]
	private Sprite SayakaGodsibbFace;
	[SerializeField]
	private Sprite NagisaFace;
	[SerializeField]
	private Sprite DevilHomuraFace;
	[SerializeField]
	private Sprite MichelFace;


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
	private GameObject SkillWindow;

	/// <summary>
	/// システム画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject SystemWindow;

	/// <summary>
	/// パーティー画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject PartyWindow;

	/// <summary>
	/// セーブロード画面のオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject SaveLoadWindow;

	/// <summary>
	/// ポップアップ
	/// </summary>
	[SerializeField]
	private GameObject PopUp;

	/// <summary>
	/// ステータス画面のY座標
	/// </summary>
	private const float STATUSWINDOWYPOS = 190;

	/// <summary>
	/// ステータス画面のX座標
	/// </summary>
    private const float STATUSWINDOWXPOS = 800;

	/// <summary>
	/// キャラクター選択画面の行の最大幅
	/// </summary>
	private const int CHARACTERSELECTXLENGTH = 7;

	/// <summary>
	/// キーコンフィグ画面
	/// </summary>
	public GameObject KeyConfigScreen;

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
			if (Menucontrol == MenuControl.ROOT)
			{
				KeyInputController(ref MenuBarSelect, ref Dummy, Menubar.Length, 0);
				// 選択により各モードへ移行
				if (ControllerManager.Instance.Shot)
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
								"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Status呼び出し
								"oncomplete", "InsertItem",
								"oncompletetarget", gameObject
							));
							ItemDraw();
							// ITEMへ移行
							Menucontrol = MenuControl.ITEM;
							break;
						case (int)Menumode.SKILL:
							Menucontrol = MenuControl.SKILLCHARSELECT;
							break;
						case (int)Menumode.SYSTEM:
							// 選択するとSYSTEMへ移行
							AudioManager.Instance.PlaySE("OK");
							Menucontrol = MenuControl.SYSTEM;
							iTween.MoveTo(Root, iTween.Hash(
								// 移動先指定
								"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Status呼び出し
								"oncomplete", "InsertSystem",
								"oncompletetarget", gameObject
							));
							break;
						case (int)Menumode.PARTY:
							// 選択するとPARTYへ移行
							AudioManager.Instance.PlaySE("OK");
							// 選択可能な場合一人目選択へ
							if (savingparameter.UseableCharacterSelect)
							{
								Menucontrol = MenuControl.PARTYSELECT1;
							}
							else
							{
								Menucontrol = MenuControl.PARTYUNSELECT;
							}
							iTween.MoveTo(Root, iTween.Hash(
								// 移動先指定
								"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Party呼び出し
								"oncomplete", "InsertParty",
								"oncompletetarget", gameObject
							));
							break;
						case (int)Menumode.SAVE:
							// 選択するとSAVEへ移行
							AudioManager.Instance.PlaySE("OK");
							Menucontrol = MenuControl.SAVE;
							iTween.MoveTo(Root, iTween.Hash(
								// 移動先指定
								"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Status呼び出し
								"oncomplete", "InsertSaveLoad",
								"oncompletetarget", gameObject
							));
							break;
						case (int)Menumode.LOAD:
							// 選択するとLOADへ移行
							AudioManager.Instance.PlaySE("OK");
							Menucontrol = MenuControl.LOAD;
							iTween.MoveTo(Root, iTween.Hash(
								// 移動先指定
								"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
								// 移動時間指定
								"time", 0.5f,
								// 終了時Status呼び出し
								"oncomplete", "InsertSaveLoad",
								"oncompletetarget", gameObject
							));
							break;
						case 7:
							// 選択するとタイトルへ戻る最終確認を出す
							AudioManager.Instance.PlaySE("OK");
							iTween.ScaleTo(PopUp, iTween.Hash(
										// 拡大率指定
										"y", 1,
										// 拡大時間指定
										"time", 0.5f));
							// ポップアップに文字書き込み
							PopUp.GetComponent<MenuPopup>().PopupText.text = "タイトルに戻りますか？";
							Menucontrol = MenuControl.TITLE;
							break;
					}
				}
			}
			else if (Menucontrol == MenuControl.STATUSCHARSELECT)
			{
				KeyInputController(ref Dummy, ref CharSelect, 0, savingparameter.GetNowPartyNum());
				// 選択するとSTATUSに移行
				if (ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Root, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Status呼び出し
						"oncomplete", "InsertStatus",
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
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
					{
						Characterstatusroot[i].Frame.color = new Color(255, 255, 255);
					}
					Menucontrol = MenuControl.ROOT;
				}
			}
			else if (Menucontrol == MenuControl.STATUS)
			{
				// 強化したい項目を選ぶ
				KeyInputController(ref Status.GetComponent<MenuStatus>().NowSelect, ref Dummy, (int)StatusKind.TOTALSTATUSNUM, 0);

				// OKで各選択モードに入る
				if (ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					Status.GetComponent<MenuStatus>().SelectMode = true;
					if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.STR)
					{
						// 暫定値に現在のSTRを入れる
						Status.GetComponent<MenuStatus>().StrInterim = savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSSTR;
					}
					else if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.CON)
					{
						// 暫定値に現在のConを入れる
						Status.GetComponent<MenuStatus>().ConInterim = savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect));
						Menucontrol = MenuControl.STATUSCON;
					}
					else if (Status.GetComponent<MenuStatus>().NowSelect == (int)StatusKind.VIT)
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
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Status, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));

					Menucontrol = MenuControl.STATUSCHARSELECT;
				}
			}
			else if (Menucontrol == MenuControl.STATUSSTR)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().StrInterim, 0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetStrLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSSTR);
			}
			else if (Menucontrol == MenuControl.STATUSSTRFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
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
			else if (Menucontrol == MenuControl.STATUSCON)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().ConInterim, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetArousalLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSCON);
			}
			else if (Menucontrol == MenuControl.STATUSCONFINALCONFIRM)
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
			else if (Menucontrol == MenuControl.STATUSVIT)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().VitInterim, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetDefLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSVIT);
			}
			else if (Menucontrol == MenuControl.STATUSVITFINALCONFIRM)
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
			else if (Menucontrol == MenuControl.STATUSDEX)
			{
				KeyInputController(ref Dummy, ref Status.GetComponent<MenuStatus>().DexInterim, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)) + savingparameter.GetSkillPoint(savingparameter.GetNowParty(CharSelect)) + 1, 0, savingparameter.GetBulLevel(savingparameter.GetNowParty(CharSelect)));
				StatusReinforcement(MenuControl.STATUSDEX);
			}
			else if (Menucontrol == MenuControl.STATUSDEXFINALCONFIRM)
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
			else if (Menucontrol == MenuControl.ITEM)
			{
				KeyInputController(ref ItemWindow.GetComponent<MenuItemDraw>().NowSelect, ref Dummy, Item.itemspec.Length, 0);
				// 選択の場合ポップアップを出す
				if (ControllerManager.Instance.Shot)
				{
					// アイテムの種類を取得(ないところは-1になっている）
					if (ItemWindow.GetComponent<MenuItemDraw>().ItemKind[ItemWindow.GetComponent<MenuItemDraw>().NowSelect] >= 0)
					{
						iTween.ScaleTo(PopUp, iTween.Hash(
										// 拡大率指定
										"y", 1,
										// 拡大時間指定
										"time", 0.5f));
						// ポップアップに文字書き込み
						PopUp.GetComponent<MenuPopup>().PopupText.text = "このアイテムを装備しますか？";
						Menucontrol = MenuControl.ITEMFINALCONFIRM;
					}
				}
				// キャンセル
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(ItemWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));

					Menucontrol = MenuControl.ROOT;
				}
			}
			// アイテム装備最終確認
			else if (Menucontrol == MenuControl.ITEMFINALCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						savingparameter.SetNowEquipItem(ItemWindow.GetComponent<MenuItemDraw>().ItemKind[ItemWindow.GetComponent<MenuItemDraw>().NowSelect]);
						ReinforcementCancel();
						Menucontrol = MenuControl.ITEM;
						ItemDraw();
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.ITEM;
					}
				}
			}
			// スキルキャラ選択
			else if (Menucontrol == MenuControl.SKILLCHARSELECT)
			{
				KeyInputController(ref Dummy, ref CharSelect, 0, savingparameter.GetNowPartyNum());
				// 選択するとSKILLへ移行
				if (ControllerManager.Instance.Shot)
				{
					// 選択するとITEMへ移行
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(Root, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Skill呼び出し
						"oncomplete", "InsertSkill",
						"oncompletetarget", gameObject
					));
					// 選択したキャラは誰？
					int selectedCharacter = savingparameter.GetNowParty(CharSelect);
					// SKILL書き込み
					SkillWindow.GetComponent<MenuSkillDraw>().Initiallize(selectedCharacter);
					Menucontrol = MenuControl.SKILL;
				}
				// キャンセルでROOTに戻る
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
					{
						Characterstatusroot[i].Frame.color = new Color(255, 255, 255);
					}
					Menucontrol = MenuControl.ROOT;
				}
			}
			// スキル表示
			else if (Menucontrol == MenuControl.SKILL)
			{
				// 最大ページ取得
				int maxpage = SkillWindow.GetComponent<MenuSkillDraw>().MaxPage - 1;
				// 左右キーでページ送り
				KeyInputController(ref Dummy, ref SkillWindow.GetComponent<MenuSkillDraw>().NowPage, 0, maxpage);
				// キャンセルでSKILLCHARSELECTへ移行
				if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(SkillWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
					));
					Menucontrol = MenuControl.SKILLCHARSELECT;
				}
			}
			// システム表示
			else if (Menucontrol == MenuControl.SYSTEM)
			{
				// 方向キー上下で項目変更
				KeyInputController(ref SystemWindow.GetComponent<MenuSystemDraw>().NowSelect, ref Dummy, 4, 0);
				// BGM/SE/VOICEの制御はMenuSystemDrawでやる

				// キャンセルでROOTへ移行
				if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(SystemWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}

				// NowSelect = 3でショットキーを押すとキーコンフィグへ移行
				if (SystemWindow.GetComponent<MenuSystemDraw>().NowSelect == 3 && ControllerManager.Instance.Shot)
				{
					AudioManager.Instance.PlaySE("OK");
					KeyConfigScreen.SetActive(true);
					Menucontrol = MenuControl.KEYCONFIG;
				}

				// 説明文表示
				switch (SystemWindow.GetComponent<MenuSystemDraw>().NowSelect)
				{
					case 0:
						InformationText.text = "方向キー左右でBGMのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
						break;
					case 1:
						InformationText.text = "方向キー左右でSEのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
						break;
					case 2:
						InformationText.text = "方向キー左右でVOICEのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
						break;
					case 3:
						InformationText.text = "ショットを押すとキーコンフィグ画面へ移動します\nシャンプでひとつ前の画面に戻ります";
						break;
				}
			}
			else if (Menucontrol == MenuControl.PARTYSELECT1)
			{
				// 方向キー上下左右でカーソル移動
				KeyInputControllerCharacterSelect(ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, 2, CHARACTERSELECTXLENGTH);

				// キャンセルでROOTへ移行
				if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(PartyWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// 決定で一人目確定
				else if(ControllerManager.Instance.Shot && PartyWindow.GetComponent<MenuPartyDraw>().Menupartycharcursor[PartyWindow.GetComponent<MenuPartyDraw>().Nowselect].Useable)
				{
					AudioManager.Instance.PlaySE("OK");
					PartyWindow.GetComponent<MenuPartyDraw>().SelectDone(0);
					Menucontrol = MenuControl.PARTYSELECT2;
				}

				// 説明文を表示
				InformationText.text = "一人目のキャラクター（プレイヤーキャラクター）を選択してください。\nショットキーで決定し、ジャンプキーでキャンセルしてひとつ前の画面に戻ります";
			}
			else if(Menucontrol == MenuControl.PARTYSELECT2)
			{
				// 方向キー上下左右でカーソル移動
				KeyInputControllerCharacterSelect(ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, 2, CHARACTERSELECTXLENGTH);
				// キャンセルで最終確認へ
				if (ControllerManager.Instance.Jump)
				{
					PartyWindow.GetComponent<MenuPartyDraw>().FinalConfirmDone();
					PartyWindow.GetComponent<MenuPartyDraw>().Select.SetActive(false);
					PartyWindow.GetComponent<MenuPartyDraw>().FinalCheck.SetActive(true);
					Menucontrol = MenuControl.PARTYSELECTFINALCHECK;
				}
				// 決定で二人目確定
				else if (ControllerManager.Instance.Shot && PartyWindow.GetComponent<MenuPartyDraw>().Menupartycharcursor[PartyWindow.GetComponent<MenuPartyDraw>().Nowselect].Useable)
				{
					AudioManager.Instance.PlaySE("OK");
					PartyWindow.GetComponent<MenuPartyDraw>().SelectDone(1);
					Menucontrol = MenuControl.PARTYSELECT3;
				}
				// 説明文を表示
				InformationText.text = "二人目のキャラクターを選択してください。\nショットキーで決定し、ジャンプキーで確認画面に移動します";
			}
			else if(Menucontrol == MenuControl.PARTYSELECT3)
			{
				// 方向キー上下左右でカーソル移動
				KeyInputControllerCharacterSelect(ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, ref PartyWindow.GetComponent<MenuPartyDraw>().Nowselect, 2, CHARACTERSELECTXLENGTH);
				// キャンセルで最終確認へ
				if (ControllerManager.Instance.Jump)
				{
					PartyWindow.GetComponent<MenuPartyDraw>().FinalConfirmDone();
					PartyWindow.GetComponent<MenuPartyDraw>().Select.SetActive(false);
					PartyWindow.GetComponent<MenuPartyDraw>().FinalCheck.SetActive(true);
					Menucontrol = MenuControl.PARTYSELECTFINALCHECK;
				}
				// 決定で三人目確定
				else if (ControllerManager.Instance.Shot && PartyWindow.GetComponent<MenuPartyDraw>().Menupartycharcursor[PartyWindow.GetComponent<MenuPartyDraw>().Nowselect].Useable)
				{
					AudioManager.Instance.PlaySE("OK");
					PartyWindow.GetComponent<MenuPartyDraw>().SelectDone(2);
					PartyWindow.GetComponent<MenuPartyDraw>().FinalConfirmDone();
					PartyWindow.GetComponent<MenuPartyDraw>().Select.SetActive(false);
					PartyWindow.GetComponent<MenuPartyDraw>().FinalCheck.SetActive(true);
					Menucontrol = MenuControl.PARTYSELECTFINALCHECK;
				}
				// 説明文を表示
				InformationText.text = "三人目のキャラクターを選択してください。\nショットキーで決定し、ジャンプキーで確認画面に移動します";
			}
			else if (Menucontrol == MenuControl.PARTYUNSELECT)
			{
				// キャンセルでROOTへ移行
				if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(PartyWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// 説明文を表示
				InformationText.text = "現在はパーティーを選択できません\nシャンプでひとつ前の画面に戻ります";
			}
			else if(Menucontrol == MenuControl.PARTYSELECTFINALCHECK)
			{
				// 決定で確定
				if(ControllerManager.Instance.Shot)
				{
					// パーティー確定
					for(int i=0; i<3; i++)
					{
						int partymember = PartyWindow.GetComponent<MenuPartyDraw>().SelectedParty[i];
						savingparameter.SetNowParty(i, partymember);
					}
					// ROOTのグラフィック書き換え
					InsertRootCharacter();
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(PartyWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// キャンセルでROOTへ移行
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(PartyWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// 説明文を表示
				InformationText.text = "これでよろしいですか？。\nショットキーで決定し、ジャンプキーでキャンセルします";
			}
			// セーブ場所選択
			else if(Menucontrol == MenuControl.SAVE)
			{
				// 方向キー左右でページ送り・方向キー上下でファイル送り
				KeyInputControllerSaveFileSelect(ref SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect, ref SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage, 20, 5);
				// セーブファイル名
				string savefilename = @"save\" + (SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage * 10 + SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect + 1).ToString("D3") + ".sav";
				// 上書き時決定で最終確認へ移行
				if (ControllerManager.Instance.Shot)
				{
					// そのセーブファイルがすでに存在した場合はCHECKSELECTFINALへ
					if (System.IO.File.Exists(savefilename))
					{
						AudioManager.Instance.PlaySE("OK");
						Status.GetComponent<MenuStatus>().SelectMode = false;
						iTween.ScaleTo(PopUp, iTween.Hash(
								// 拡大率指定
								"y", 1,
								// 拡大時間指定
								"time", 0.5f
							));
						// ポップアップに文字書き込み
						PopUp.GetComponent<MenuPopup>().PopupText.text = "上書きしますがよろしいですか？";
						Menucontrol = MenuControl.SAVECONFIRM;
					}
					// 存在しない場合はセーブを実行
					else
					{
						SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().SaveDone(savefilename);	
						SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().SaveFileNameRebuild();
					}
				}
				// キャンセルでROOTへ移行
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(SaveLoadWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// 説明文を表示
				InformationText.text = "セーブする場所を選んでください。\nショットキーで決定し、ジャンプキーでルート画面に戻ります";
			}
			// 
			else if(Menucontrol == MenuControl.SAVECONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						// セーブファイル名
						string savefilename = @"save\" + (SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage * 10 + SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect + 1).ToString("D3") + ".sav";
						SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().SaveDone(savefilename);
						// セーブファイル表示名
						SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().SaveFileNameRebuild();
						iTween.ScaleTo(PopUp, iTween.Hash(
							// 拡大率指定
							"y", 0,
							// 拡大時間指定
							"time", 0.5f
						));
						Menucontrol = MenuControl.SAVE;
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.SAVE;
					}
				}
			}
			// ロードファイル選択
			else if (Menucontrol == MenuControl.LOAD)
			{
				// 方向キー左右でページ送り・方向キー上下でファイル送り
				KeyInputControllerSaveFileSelect(ref SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect, ref SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage, 20, 5);
				// ファイルが存在する場所で選択確定したら最終確認へ
				// セーブファイル名
				string savefilename = @"save\" + (SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage * 10 + SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect + 1).ToString("D3") + ".sav";
				// 上書き時決定で最終確認へ移行
				if (ControllerManager.Instance.Shot && System.IO.File.Exists(savefilename))
				{
					AudioManager.Instance.PlaySE("OK");
					Status.GetComponent<MenuStatus>().SelectMode = false;
					iTween.ScaleTo(PopUp, iTween.Hash(
							// 拡大率指定
							"y", 1,
							// 拡大時間指定
							"time", 0.5f
						));
					// ポップアップに文字書き込み
					PopUp.GetComponent<MenuPopup>().PopupText.text = "このファイルをロードします。よろしいですか？";
					Menucontrol = MenuControl.LOADCONFIRM;
				}
				// キャンセルでROOTへ移行
				else if (ControllerManager.Instance.Jump)
				{
					AudioManager.Instance.PlaySE("OK");
					iTween.MoveTo(SaveLoadWindow, iTween.Hash(
						// 移動先指定
						"position", new Vector3(STATUSWINDOWXPOS, STATUSWINDOWYPOS, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Root呼び出し
						"oncomplete", "InsertRoot",
						"oncompletetarget", gameObject
						));
					Menucontrol = MenuControl.ROOT;
				}
				// 説明文を表示
				InformationText.text = "ロードするファイルを選んでください。\nショットキーで決定し、ジャンプキーでルート画面に戻ります";
			}
			// ロード最終確認
			else if(Menucontrol == MenuControl.LOADCONFIRM)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						// ロードファイル名
						string savefilename = @"save\" + (SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage * 10 + SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect + 1).ToString("D3") + ".sav";
						// ロード処理開始
						// セーブファイルとなるオブジェクト
						SaveData sd = new SaveData();
						// 保存したファイルをロード
						sd = (SaveData)savingparameter.LoadFromBinaryFile(savefilename);
						// ロードした内容をsavingparameterへ書き込む
						// ストーリー進行度 
						savingparameter.story = sd.story;
						// キャラクターの配置位置
						savingparameter.nowposition.x = sd.nowposition_x;
						savingparameter.nowposition.y = sd.nowposition_y;
						savingparameter.nowposition.z = sd.nowposition_z;
						// キャラクターの配置角度
						savingparameter.nowrotation.x = sd.nowrotation_x;
						savingparameter.nowrotation.y = sd.nowrotation_y;
						savingparameter.nowrotation.z = sd.nowrotation_z;
						// アイテムボックスの開閉フラグ
						for (int i = 0; i < MadokaDefine.NUMOFITEMBOX; ++i)
						{
							savingparameter.itemboxopen[i] = sd.itemboxopen[i];
						}
						// 現在の所持金
						savingparameter.nowmoney = sd.nowmoney;
						// 現在のパーティー(護衛対象も一応パーティーに含める)
						for (int i = 0; i < 4; ++i)
						{
							savingparameter.SetNowParty(i, sd.nowparty[i]);
						}
						// キャラクター関連
						for (int i = 0; i < (int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM; ++i)
						{
							// 各キャラのレベル
							savingparameter.SetNowLevel(i, sd.nowlevel[i]);
							// 各キャラの現在の経験値
							savingparameter.SetExp(i, sd.nowExp[i]);
							// 各キャラのHP
							savingparameter.SetNowHP(i, sd.nowHP[i]);
							// 各キャラの最大HP
							savingparameter.SetMaxHP(i, sd.nowMaxHP[i]);
							// 各キャラの覚醒ゲージ
							savingparameter.SetNowArousal(i, sd.nowArousal[i]);
							// 各キャラの最大覚醒ゲージ
							savingparameter.SetMaxArousal(i);
							// 各キャラの攻撃力レベル
							savingparameter.SetStrLevel(i, sd.StrLevel[i]);
							// 各キャラの防御力レベル
							savingparameter.SetDefLevel(i, sd.DefLevel[i]);
							// 各キャラの残弾数レベル
							savingparameter.SetBulLevel(i, sd.BulLevel[i]);
							// 各キャラのブースト量レベル
							savingparameter.SetBoostLevel(i, sd.BoostLevel[i]);
							// 覚醒ゲージレベル
							savingparameter.SetArousalLevel(i, sd.ArousalLevel[i]);
							// ソウルジェム汚染率
							savingparameter.SetGemContimination(i, sd.GemContimination[i]);
							// スキルポイント
							savingparameter.SetSkillPoint(i, sd.SkillPoint[i]);
						}
						// 各アイテムの保持数
						for (int i = 0; i < Item.itemspec.Length; ++i)
						{
							savingparameter.SetItemNum(i, sd.m_numofItem[i]);
						}
						// 現在装備中のアイテム
						savingparameter.SetNowEquipItem(sd.m_nowequipItem);
						// 現在の場所
						savingparameter.nowField = sd.nowField;
						// 前にいた場所
						savingparameter.beforeField = 9999;
						// 該当の場所へ遷移する
						FadeManager.Instance.LoadLevel(SceneName.sceneName[savingparameter.nowField], 1.0f);
					}
				}
				// 選択キャンセル
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.LOAD;
					}
				}
			}
			// タイトルへ戻る
			else if(Menucontrol == MenuControl.TITLE)
			{
				KeyInputController(ref Dummy, ref PopUp.GetComponent<MenuPopup>().NowSelect, 0, 2);
				// 選択確定
				if (PopUp.GetComponent<MenuPopup>().NowSelect == 0)
				{
					if (ControllerManager.Instance.Shot)
					{
						FadeManager.Instance.LoadLevel("title", 1.0f);
					}
				}
				else
				{
					if (ControllerManager.Instance.Shot)
					{
						ReinforcementCancel();
						Menucontrol = MenuControl.ROOT;
					}
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
		this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.STATUSCHARSELECT || Menucontrol == MenuControl.SKILLCHARSELECT).Subscribe(_ =>
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
            if (Menucontrol == MenuControl.STATUSCHARSELECT)
            {
                InformationText.text = "強化したいキャラクターを選んでください";
            }
            else if(Menucontrol == MenuControl.SKILLCHARSELECT)
            {
                InformationText.text = "スキルを確認したいキャラクターを選んでください";
            }
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

		// ITEM状態
		this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.ITEM).Subscribe(_ =>
		{
			MenuItemDraw menuItemDraw = ItemWindow.GetComponent<MenuItemDraw>();
			// インフォメーションテキストの内容
			InformationText.text = menuItemDraw.ItemDescription[menuItemDraw.NowSelect];
		});

		// SKILL状態
		this.UpdateAsObservable().Where(_ => Menucontrol == MenuControl.SKILL).Subscribe(_ =>
		{
			MenuSkillDraw menuSkillDraw = SkillWindow.GetComponent<MenuSkillDraw>();
			// 現在のページを取得
			int nowpage = menuSkillDraw.NowPage;
			// 最大ページを取得
			int maxpage = menuSkillDraw.MaxPage;
			// キャラクターを取得
			int selectedcharacter = savingparameter.GetNowParty(CharSelect);

			// 現在のページが0なら←矢印を消す
			if (nowpage == 0)
			{
				menuSkillDraw.LeftArrow.color = new Color(1, 1, 1, 0);
				menuSkillDraw.RightArrow.color = new Color(1, 1, 1, 1);
			}
			// 現在のページが最大ページなら→矢印を消す
			else if(nowpage == maxpage - 2)
			{
				menuSkillDraw.LeftArrow.color = new Color(1, 1, 1, 1);
				menuSkillDraw.RightArrow.color = new Color(1, 1, 1, 0);
			}
			// それ以外なら両方表示
			else
			{
				menuSkillDraw.LeftArrow.color = new Color(1, 1, 1, 1);
				menuSkillDraw.RightArrow.color = new Color(1, 1, 1, 1);
			}

			// ページに応じて描画内容を選択
			// 通常
			if (nowpage < maxpage - 2)
			{
				menuSkillDraw.SkillDraw(nowpage, selectedcharacter);
			}
			// ファイナルマジック
			else
			{
				menuSkillDraw.FinalMagicDraw(selectedcharacter);
			}
		});
		
    }

	/// <summary>
	/// SystemWindowをprivateにしてしまったのでアクセス用
	/// </summary>
	/// <param name="nowselect"></param>
	public void MenuSystemDrawSelect(int nowselect)
	{
		SystemWindow.GetComponent<MenuSystemDraw>().NowSelect = nowselect;
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
		InsertRootCharacter();
    }
	/// <summary>
	/// STATUSのキャラにデータを入れる
	/// </summary>
	private void InsertRootCharacter()
	{
		for (int i = 0; i < Characterstatusroot.Length; i++)
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
				Characterstatusroot[i].gameObject.SetActive(true);
				// キャラの顔を出す
				// まどか
				if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = MadokaFace;
				}
				// さやか
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = SayakaFace;
				}
				// ほむら
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
				{
					Characterstatusroot[i].CharacterFace.sprite = HomuraFace;
				}
				// マミ
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MAMI)
				{
					Characterstatusroot[i].CharacterFace.sprite = MamiFace;
				}
				// 杏子
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_KYOKO)
				{
					Characterstatusroot[i].CharacterFace.sprite = KyokoFace;
				}
				// ゆま
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_YUMA)
				{
					Characterstatusroot[i].CharacterFace.sprite = YumaFace;
				}
				// キリカ
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = KirikaFace;
				}
				// 織莉子
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_ORIKO)
				{
					Characterstatusroot[i].CharacterFace.sprite = OrikoFace;
				}
				// 弓ほむら
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
				{
					Characterstatusroot[i].CharacterFace.sprite = HomuraBowFace;
				}
				// スコノシュート
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO)
				{
					Characterstatusroot[i].CharacterFace.sprite = SconosciutoFace;
				}
				// アルティメットまどか
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
				{
					Characterstatusroot[i].CharacterFace.sprite = UltimateMadokaFace;
				}
				// 円環のさやか
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA_GODSIBB)
				{
					Characterstatusroot[i].CharacterFace.sprite = SayakaGodsibbFace;
				}
				// デビルほむら
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_DEVIL_HOMURA)
				{
					Characterstatusroot[i].CharacterFace.sprite = DevilHomuraFace;
				}
				// なぎさ
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_NAGISA)
				{
					Characterstatusroot[i].CharacterFace.sprite = NagisaFace;
				}
				// ミッチェル
				else if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MICHEL)
				{
					Characterstatusroot[i].CharacterFace.sprite = MichelFace;
				}

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
	/// 上記のキャラ選択専用版
	/// </summary>
	/// <param name="variableUD"></param>
	/// <param name="variableLR"></param>
	/// <param name="lengthUD"></param>
	/// <param name="lengthLR"></param>
	private void KeyInputControllerCharacterSelect(ref int variableUD, ref int variableLR, int lengthUD, int lengthLR)
	{
		// 上
		if (!_PreTopInput && ControllerManager.Instance.Top)
		{
			// 中央列のみデビルほむらのカーソルに移動する
			if(variableUD == 14)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableUD = 10;
			}
			else if (variableUD > CHARACTERSELECTXLENGTH - 1)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableUD -= CHARACTERSELECTXLENGTH;
			}

		}
		// 下
		if(!_PreUnderInput && ControllerManager.Instance.Under)
		{
			// 中央列のみ弓ほむらのカーソルに移動する
			if(variableUD == 10)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableUD = 14;
			}
			else if(variableUD < CHARACTERSELECTXLENGTH)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableUD += CHARACTERSELECTXLENGTH;
			} 
		}
		// 左
		if (!_PreLeftInput && ControllerManager.Instance.Left)
		{
			if (variableLR != 0 && variableLR != CHARACTERSELECTXLENGTH && variableLR != 14)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableLR--;
			}
		}
		// 右
		if (!_PreRightInput && ControllerManager.Instance.Right)
		{
			if (variableLR != CHARACTERSELECTXLENGTH - 1 && variableLR < CHARACTERSELECTXLENGTH * 2 - 1)
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
	/// 上記のセーブファイル選択版
	/// </summary>
	/// <param name="variableUD"></param>
	/// <param name="variableLR"></param>
	/// <param name="lengthUD"></param>
	/// <param name="lengthLR"></param>
	private void KeyInputControllerSaveFileSelect(ref int variableUD, ref int variableLR, int lengthUD, int lengthLR)
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
		if (!_PreLeftInput && ControllerManager.Instance.Left)
		{
			if(variableLR == 0)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableLR = lengthLR - 1;
			}
			else
			{
				AudioManager.Instance.PlaySE("cursor");
				variableLR--;
			}
		}

		// 右
		if (!_PreRightInput && ControllerManager.Instance.Right)
		{
			if (variableLR < lengthLR - 1)
			{
				AudioManager.Instance.PlaySE("cursor");
				variableLR++;
			}
			else
			{
				AudioManager.Instance.PlaySE("cursor");
				variableLR = 0;
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
    /// スキル画面を入れる
    /// </summary>
    private void InsertSkill()
    {
        iTween.MoveTo(SkillWindow, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
    }

	/// <summary>
	/// システム画面を入れる
	/// </summary>
	private void InsertSystem()
	{
		iTween.MoveTo(SystemWindow, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
	}

	/// <summary>
	/// パーティー編成画面を入れる
	/// </summary>
	private void InsertParty()
	{
		PartyWindow.GetComponent<MenuPartyDraw>().Setup();
		iTween.MoveTo(PartyWindow, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
	}

	/// <summary>
	/// セーブ・ロード画面を入れる
	/// </summary>
	private void InsertSaveLoad()
	{
		SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().SaveDataOpen(0);
		SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowselect = 0;
		SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().Nowpage = 0;
		SaveLoadWindow.GetComponent<MenuSaveLoadDraw>().DrawCursor(0);
		iTween.MoveTo(SaveLoadWindow, new Vector3(320, STATUSWINDOWYPOS, 0), 0.5f);
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

    /// <summary>
    /// アイテム一覧を描画する
    /// </summary>
    public void ItemDraw()
    {
        // 所持アイテム一覧を表示する
        MenuItemDraw menuItemDraw = ItemWindow.GetComponent<MenuItemDraw>();

        // 所持個数を取得し、その分を描画する
        for (int i = 0; i < Item.itemspec.Length; ++i)
        {
            if (savingparameter.GetItemNum(i) > 0)
            {
                menuItemDraw.ItemName[i].text = Item.itemspec[i].Name();
                // 装備していた場合は装備と追記する
                if (savingparameter.GetNowEquipItemString() == Item.itemspec[i].Name())
                {
                    menuItemDraw.ItemName[i].text += "<装備中>";
                }
                menuItemDraw.ItemNum[i].text = savingparameter.GetItemNum(i).ToString("d2");
                menuItemDraw.ItemKind[i] = i;
                menuItemDraw.ItemDescription[i] = Item.itemspec[i].Information();
            }
            else
            {
                menuItemDraw.ItemName[i].text = "";
                menuItemDraw.ItemNum[i].text = "";
                menuItemDraw.ItemKind[i] = -1;
                menuItemDraw.ItemDescription[i] = "";
            }
        }
        menuItemDraw.NowSelect = 0;
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
    SKILLCHARSELECT,        // スキルキャラ選択
    SYSTEM,                 // システム
	KEYCONFIG,				// キーコンフィグ
	PARTYUNSELECT,			// パーティー選択不可
    PARTYSELECT1,           // パーティー選択一人目
	PARTYSELECT2,			// パーティー選択二人目
	PARTYSELECT3,			// パーティー選択三人目
	PARTYSELECTFINALCHECK,	// パーティー選択最終確認
    SAVE,                   // セーブ
	SAVECONFIRM,			// セーブ最終確認
    LOAD,                   // ロード
	LOADCONFIRM,			// ロード最終確認
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
