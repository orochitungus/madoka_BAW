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
            // キャラの顔を出す
            else
            {
                // まどか
                if (characterindex == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
                {
                    Characterstatusroot[i].CharacterFace.sprite = MadokaFace;
                }
                // さやか

                // ほむら

                // マミ

                // 杏子

                // ゆま

                // キリカ

                // 織莉子

                // 弓ほむら

                // スコノシュート

                // アルティメットまどか

                // 円環のさやか

                // デビルほむら

                // なぎさ

                // ミッチェル

                // 上記以外は消す
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
}

/// <summary>
/// 現在MenuBarのどれが選択されているか
/// </summary>
public enum MenuControl
{
    ROOT,                   // ルート
    STATUS,                 // ステータス
    ITEM,                   // アイテム
    SKILL,                  // スキル
    SYSTEM,                 // システム
    PARTY,                  // パーティー
    SAVE,                   // セーブ
    LOAD,                   // ロード
    TITLE                   // タイトル
}
