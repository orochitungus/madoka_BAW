// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	/// <summary>
	/// 章データ
	/// </summary>
	public class AdvChapterData : ScriptableObject
	{
		//章の名前
		public string ChapterName { get { return chapterName; } }
		[SerializeField]
		string chapterName = "";

		//インポートされたデータのリスト
		public List<AdvImportBook> DataList { get { return dataList; } }
		[SerializeField]
		List<AdvImportBook> dataList = new List<AdvImportBook>();

		public List<StringGrid> SettingList { get { return settingList; } }
		[SerializeField]
		List<StringGrid> settingList = new List<StringGrid>();


		public bool IsInited { get; set; }

		public void Init(string name)
		{
			this.chapterName = name;
		}

		public void MakeSettingImportData(AdvMacroManager macroManager)
		{
			foreach (var book in DataList)
			{
				foreach (var sheet in book.GridList)
				{
					string sheetName = sheet.SheetName;
					if (AdvSheetParser.IsDisableSheetName(sheetName))
					{
						Debug.LogError(sheetName + " is invalid name");
						continue;
					}

					if (AdvSheetParser.IsSettingsSheet(sheetName))
					{
						settingList.Add(sheet);
					}
					else
					{
						macroManager.TryAddMacroData(sheet.SheetName, sheet);
					}
				}
			}
		}

		public void MakeScenarioImportData(AdvSettingDataManager dataManager, AdvMacroManager macroManager)
		{
			foreach (var book in DataList)
			{
				if (book.Reimport)
				{
					book.MakeImportData(dataManager, macroManager);
				}
			}
		}


		//起動時初期化
		public void BootInit(AdvSettingDataManager settingDataManager)
		{
			IsInited = true;
			//設定データの初期化
			foreach (var grid in settingList)
			{
				IAdvSetting data = AdvSheetParser.FindSettingData(settingDataManager, grid.SheetName);
				if (data != null)
				{
					data.ParseGrid(grid);
				}
			}
			foreach (var grid in settingList)
			{
				IAdvSetting data = AdvSheetParser.FindSettingData(settingDataManager, grid.SheetName);
				if (data != null)
				{
					data.BootInit(settingDataManager);
				}
			}
		}


		public void AddScenario(Dictionary<string, AdvScenarioData> scenarioDataTbl)
		{
			Profiler.BeginSample("AddScenario");
			foreach (var book in DataList)
			{
				foreach (var sheet in book.ImportGridList)
				{
					if (scenarioDataTbl.ContainsKey(sheet.SheetName))
					{
						Debug.LogErrorFormat("{0} is already contains", sheet.SheetName);
						continue;
					}
					Profiler.BeginSample("new Scenario");
					sheet.InitLink();
					AdvScenarioData scenario = new AdvScenarioData(sheet);
					scenarioDataTbl.Add(sheet.SheetName, scenario);
					Profiler.EndSample();
				}
			}
			Profiler.EndSample();
		}


#if false
		void AddScenario(AdvChapterData chapter)
		{
			chapter.MakeScenarioData();
			foreach (var grid in chapter.ScenarioList)
			{
				string sheetName = grid.SheetName;
				if (!MacroManager.TryAddMacroData(sheetName, grid))
				{
					//既にある（エクスポートされたデータの可能性あり）
					if (scenarioDataTbl.ContainsKey(sheetName))
					{
						Debug.LogWarning(sheetName + " is already contains");
					}
					else
					{
						AdvScenarioData data = new AdvScenarioData(sheetName, grid);
						chapter.ScenarioDataList.Add(data);
						scenarioDataTbl.Add(sheetName, data);
					}
				}
			}
		}
#endif

#if false
		//****************************　TSVのロード用　****************************//
		/// <summary>
		/// TSVをロード
		/// </summary>
		/// <param name="url">ファイルパス</param>
		/// <param name="version">シナリオバージョン（-1以下で必ずサーバーからデータを読み直す）</param>
		/// <returns></returns>
		internal IEnumerator CoLoadFromTsv(string url, int version)
		{
			//起動ファイルの読み込み
			AssetFile bootFile = AssetFileManager.Load(url, version, this);
			Debug.Log("Load Chapter : " + ChapterName + " :Ver " + bootFile.Version);
			while (!bootFile.IsLoadEnd) yield return null;

			string rootDir = FilePathUtil.GetDirectoryPath(url);
			//設定ファイルの読み込み
			List<AssetFile> settingFileList = new List<AssetFile>();
			{
				StringGrid grid = bootFile.Csv;
				foreach (StringGridRow row in grid.Rows)
				{
					if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
					if (row.IsEmptyOrCommantOut) continue;					//データがない
					string path = AdvParser.ParseCell<string>(row, AdvColumnName.Param1);
					int ver = AdvParser.ParseCell<int>(row, AdvColumnName.Version);
					settingFileList.Add(AssetFileManager.Load(FilePathUtil.Combine(rootDir, path), ver, this));
				}
			}

			//設定ファイルの読み込み
			List<AssetFile> scenarioFileList = new List<AssetFile>();
			foreach (var item in settingFileList)
			{
				while (!item.IsLoadEnd) yield return null;
				if (!item.IsLoadError)
				{
					StringGrid grid = item.Csv;
					if (grid.SheetName != AdvSheetParser.SheetNameScenario)
					{
						this.RuntimeGridList.Add(grid);
					}
					else
					{
						foreach (StringGridRow row in grid.Rows)
						{
							if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
							if (row.IsEmptyOrCommantOut) continue;					//データがない
							string path = AdvParser.ParseCell<string>(row, AdvColumnName.FileName);
							int ver = AdvParser.ParseCellOptional<int>(row, AdvColumnName.Version, 0);

							//旧形式（ﾌｧｲﾙ分割なし）に対応
							if (!path.Contains("/"))
							{
								path = "Scenario/" + path;
							}
							path += ".tsv";
							scenarioFileList.Add(AssetFileManager.Load(FilePathUtil.Combine(rootDir, path), ver, this));
						}
					}
				}
				item.Unuse(this);
			}

			foreach (var item in scenarioFileList)
			{
				while (!item.IsLoadEnd) yield return null;
				if (!item.IsLoadError)
				{
					this.RuntimeGridList.Add(item.Csv);
				}
				item.Unuse(this);
			}

			bootFile.Unuse(this);
//			Debug.Log("Load End Chapter : " + url + " :Ver " + bootFile.Version);
			Debug.Log("Load End Chapter : " + ChapterName + " :Ver " + bootFile.Version);
		}
#endif

#if UNITY_EDITOR
		public void InitImportData(List<AdvImportBook> importDataList)
		{
			this.SettingList.Clear();
			this.DataList.Clear();
			this.DataList.AddRange(importDataList);
		}
#endif
	}
}