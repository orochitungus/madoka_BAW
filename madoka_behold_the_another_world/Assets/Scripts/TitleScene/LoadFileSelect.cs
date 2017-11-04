using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class LoadFileSelect : MonoBehaviour 
{
	public Text []Files;
	public Text []Cursor;

	public int NowSelect;
	public int NowPage;

	public TitleCanvas Titlecanvas;

	public List<string> SaveDaraInformation = new List<string>();

	public bool ModeSelectDone;

	// 1F前の上入力
	private bool _PreTopInput;
	// 1F前の下入力
	private bool _PreUnderInput;

	// 1F前の左入力
	private bool _PreLeftInput;
	// 1F前の右入力
	private bool _PreRightInput;

	// Use this for initialization
	void Start()
	{

		// 入力取得
		// 短押し判定(前フレーム押してない）		
		this.UpdateAsObservable().Subscribe(_ =>
		{
			// 上
			if (!_PreTopInput && ControllerManager.Instance.Top)
			{
				if (NowSelect > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect--;
					Cursor[NowSelect + 1].gameObject.SetActive(false);
					Cursor[NowSelect].gameObject.SetActive(true);
				}
			}
			// 下
			if (!_PreUnderInput && ControllerManager.Instance.Under)
			{
				if (NowSelect < 19)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect++;
					Cursor[NowSelect - 1].gameObject.SetActive(false);
					Cursor[NowSelect].gameObject.SetActive(true);
				}
			}
			// 左
			if(!_PreLeftInput && ControllerManager.Instance.Left)
			{
				AudioManager.Instance.PlaySE("cursor");
				if (NowPage == 0)
				{					
					NowPage = 4;
				}
				else
				{
					NowPage--;
				}
				// ページに描きこむ
				for (int i = 0; i<20; i++)
				{
					Files[i].text = SaveDaraInformation[NowPage * 20 + i];
				}
			}
			// 右
			if(!_PreRightInput && ControllerManager.Instance.Right)
			{
				AudioManager.Instance.PlaySE("cursor");
				if (NowPage == 4)
				{
					NowPage = 0;
				}
				else
				{
					NowPage++;
				}
				// ページに描きこむ
				for (int i = 0; i<20; i++)
				{
					Files[i].text = SaveDaraInformation[NowPage * 20 + i];
				}
			}

			// 選択確定
			if (ControllerManager.Instance.Shot && !ModeSelectDone)
			{
				AudioManager.Instance.PlaySE("OK");
				// ロード処理開始
				// 対象のファイルが存在するか確認する
				string savefilename = @"save\" + (NowPage * 20 + NowSelect + 1).ToString("D3") + ".sav";
				// 存在した場合、ロード開始
				if (System.IO.File.Exists(savefilename))
				{
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
			// キャンセル
			else if (ControllerManager.Instance.Jump && !ModeSelectDone)
			{
				AudioManager.Instance.PlaySE("cursor");
				Titlecanvas.TitleCanvasAnimator.Play(Titlecanvas.LoadFileSelectClose);
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
				ModeSelectDone = false;
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// 起動時に実行する
	/// </summary>
	public void Initialize()
	{
		NowPage = 0;
		NowSelect = 0;
		ModeSelectDone = false;
		// data.savの中身を読み取り、m_dateの中に入れる
		// StreamReader の新しいインスタンスを生成する
		System.IO.StreamReader cReader = (new System.IO.StreamReader(@"save\save.sav", System.Text.Encoding.UTF8));
		// 読み込みできる文字がなくなるまで繰り返す
		while (cReader.Peek() >= 0)
		{
			// ファイルを 1 行ずつ読み込む
			SaveDaraInformation.Add(cReader.ReadLine());
		}
		// cReader を閉じる
		cReader.Close();
		// 最初のページに描きこむ
		for(int i=0; i<20; i++)
		{
			Files[i].text = SaveDaraInformation[i];
		}
		// 最初のカーソル以外を消す
		for (int i = 1; i < 20; i++)
		{
			Cursor[i].gameObject.SetActive(false);
		}
	}
}
