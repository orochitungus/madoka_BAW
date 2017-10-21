using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuSaveLoadDraw : MonoBehaviour 
{
	/// <summary>
	/// カーソル部分
	/// </summary>
	public Text Cursor;

	/// <summary>
	/// セーブデータ部分
	/// </summary>
	public Text SaveDataText;

	/// <summary>
	/// 現在選択中のセーブデータ(0-19の間）
	/// </summary>
	public int Nowselect;

	/// <summary>
	/// 現在選択中のページ
	/// </summary>
	public int Nowpage;


	// データ作成
	private string[] Savedata = new string[MadokaDefine.SAVEDATANUM];

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ =>
		{
			DrawCursor(Nowselect);
			SaveDataDraw(Nowpage);
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// セーブデータ一覧を作成する
	/// </summary>
	/// <param name="nowpage"></param>
	public void SaveDataOpen(int nowpage)
	{
		
		// saveフォルダがない場合、saveフォルダを作る
		if (!System.IO.Directory.Exists("save"))
		{
			//System.IO.DirectoryInfo di = 
			System.IO.Directory.CreateDirectory(@"save");
		}
		// save.savがない場合、save.savを作る
		if (!System.IO.File.Exists(@"save\save.sav"))
		{
			// 以下のように埋める
			// 01:----/--/--/--:-- ----------------------
			// 02:----/--/--/--:-- ----------------------
			for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
			{
				Savedata[i] = (i + 1).ToString("D3") + ":----/--/-- --:-- ----------------------";
			}
			// savedataを保存する
			System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
			//書き込むファイルを開く
			System.IO.StreamWriter sw = new System.IO.StreamWriter(@"save\save.sav", false, enc);
			for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
			{
				sw.WriteLine(Savedata[i]);
			}
			sw.Close();
		}
		// save.savを開く	
		System.IO.StreamReader sr = new System.IO.StreamReader(@"save\save.sav");
		for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
		{
			Savedata[i] = sr.ReadLine();
		}
		
	}

	/// <summary>
	/// セーブデータ一覧を描画する
	/// </summary>
	/// <param name="nowpage"></param>
	public void SaveDataDraw(int nowpage)
	{
		SaveDataText.text = "";
		// ページごとに描画する
		for (int i = 20 * nowpage; i < 20 * nowpage + 20; i++)
		{
			SaveDataText.text += Savedata[i] + "\n";
		}
	}

	/// <summary>
	/// カーソルを描画する
	/// </summary>
	/// <param name="nowselect"></param>
	public void DrawCursor(int nowselect)
	{
		Cursor.text = "";
		for(int i=0; i<20; i++)
		{
			if(i == nowselect)
			{
				Cursor.text += ">\n";
			}
			else
			{
				Cursor.text += "\n";
			}
		}
	}

	/// <summary>
	/// セーブを実行する
	/// </summary>
	public void SaveDone(string savefilename)
	{
		// セーブファイルとなるオブジェクト
		SaveData sd = new SaveData();
		// sdに保存する内容を記述する
		// ストーリー進行度
		sd.story = savingparameter.story;
		// キャラクターの配置位置
		sd.nowposition_x = savingparameter.nowposition.x;
		sd.nowposition_y = savingparameter.nowposition.y;
		sd.nowposition_z = savingparameter.nowposition.z;
		// キャラクターの配置角度
		sd.nowrotation_x = savingparameter.nowrotation.x;
		sd.nowrotation_y = savingparameter.nowrotation.y;
		sd.nowrotation_z = savingparameter.nowrotation.z;
		// アイテムボックスの開閉フラグ
		for (int i = 0; i < MadokaDefine.NUMOFITEMBOX; ++i)
		{
			sd.itemboxopen[i] = savingparameter.itemboxopen[i];
		}
		// 現在の所持金
		sd.nowmoney = savingparameter.nowmoney;
		// 現在のパーティー(護衛対象も一応パーティーに含める)
		for (int i = 0; i < 4; ++i)
		{
			sd.nowparty[i] = savingparameter.GetNowParty(i);
		}
		// キャラクター関連
		for (int i = 0; i < (int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM; ++i)
		{
			// 各キャラのレベル
			sd.nowlevel[i] = savingparameter.GetNowLevel(i);
			// 各キャラの現在の経験値
			sd.nowExp[i] = savingparameter.GetNowExp(i);
			// 各キャラのHP
			sd.nowHP[i] = savingparameter.GetNowHP(i);
			// 各キャラの最大HP
			sd.nowMaxHP[i] = savingparameter.GetMaxHP(i);
			// 各キャラの覚醒ゲージ
			sd.nowArousal[i] = savingparameter.GetNowArousal(i);
			// 各キャラの最大覚醒ゲージ
			sd.nowMaxArousal[i] = savingparameter.GetMaxArousal(i);
			// 各キャラの攻撃力レベル
			sd.StrLevel[i] = savingparameter.GetStrLevel(i);
			// 各キャラの防御力レベル
			sd.DefLevel[i] = savingparameter.GetDefLevel(i);
			// 各キャラの残弾数レベル
			sd.BulLevel[i] = savingparameter.GetBulLevel(i);
			// 各キャラのブースト量レベル
			sd.BoostLevel[i] = savingparameter.GetBoostLevel(i);
			// 覚醒ゲージレベル
			sd.ArousalLevel[i] = savingparameter.GetArousalLevel(i);
			// ソウルジェム汚染率
			sd.GemContimination[i] = savingparameter.GetGemContimination(i);
			// スキルポイント
			sd.SkillPoint[i] = savingparameter.GetSkillPoint(i);
		}
		// 各アイテムの保持数
		for (int i = 0; i < Item.itemspec.Length; ++i)
		{
			sd.m_numofItem[i] = savingparameter.GetItemNum(i);
		}
		// 現在装備中のアイテム
		sd.m_nowequipItem = savingparameter.GetNowEquipItem();
		// 現在の場所
		sd.nowField = savingparameter.nowField;
		// 前にいた場所
		sd.beforeField = savingparameter.beforeField;
		// オブジェクトの内容をファイルに保存する
		savingparameter.SaveToBinaryFile(sd, savefilename);
	}

	/// <summary>
	/// セーブファイルの名前を書き換える
	/// </summary>
	public void SaveFileNameRebuild()
	{
		// 表記するファイル名を作る
		string drawfilename = "";
		// ファイル名のインデックスを取得
		drawfilename = (Nowpage * 10 + Nowselect + 1).ToString("D3") + ":";
		// 現在の日付と時刻を取得
		DateTime dt = DateTime.Now;
		drawfilename = drawfilename + dt.Year.ToString("D4") + "/" + dt.Month.ToString("D2") + "/" + dt.Day + "/" + dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2");
		// 現在の場所を取得
		drawfilename = drawfilename + " " + StagePosition.m_StageName[savingparameter.nowField];
		// ファイル一覧を書き換える
		Savedata[Nowpage * 10 + Nowselect] = drawfilename;
		// ファイル名一覧を書き換える
		System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
		//書き込むファイルを開く
		System.IO.StreamWriter sr = new System.IO.StreamWriter(@"save\save.sav", false, enc);
		for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
		{
			sr.WriteLine(Savedata[i]);
		}
		sr.Close();
	}
}
