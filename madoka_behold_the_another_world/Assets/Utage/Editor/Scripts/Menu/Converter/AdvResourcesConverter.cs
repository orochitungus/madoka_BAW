// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	//「Utage」のリソースコンバーター
	public class AdvResourcesConverter : CustomEditorWindow
	{
		const string KeyScenario = "Scenario";
		const string KeyResouces = "Resouces";

		/// <summary>
		/// リソースのパス
		/// </summary>
		[SerializeField]
		Object resourcesDirectory;
		public Object ResourcesDirectory
		{
			get { return resourcesDirectory; }
			set { resourcesDirectory = value; }
		}

		/// シナリオファイルコンバート用のプロジェクトファイル
		[SerializeField]
		AdvScenarioDataProject projectSetting;
		public AdvScenarioDataProject ProjectSetting
		{
			get { return projectSetting; }
			set { projectSetting = value; }
		}

		//アセットバンドルのビルドをするか
		enum AssetBundleBuildMode
		{
			None,           //ビルドしない
			OnlyEditor,     //エディタ用のみビルドする
			AllPlatform,    //全プラットフォーム用のものをビルドする
		};
		[SerializeField]
		AssetBundleBuildMode buildMode = AssetBundleBuildMode.OnlyEditor;

		//アセットバンドルのリネーム法則
		public enum AssetBundleRenameType
		{
			None,           //名前を変えない
			Rename,         //リネームする
			OnlyNotNamed,   //まだ名前が設定されていないものだけリネームする
		};
		[SerializeField]
		AssetBundleRenameType renameType = AssetBundleRenameType.Rename;

		[SerializeField, EnumFlags]
		AssetBundleTargetFlags buildTargetFlags = AssetBundleTargetFlags.Windows;

		[SerializeField]
		BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;


		//****************出力設定****************//

		public enum OutputType
		{
			Default,
		};
		[SerializeField]
		OutputType outputType;

		[SerializeField]
		bool isOutputLog = true;

		/// <summary>
		/// サーバー用のリソースの出力先のパス
		/// </summary>
		[SerializeField, PathDialog(PathDialogAttribute.DialogType.Directory)]
		string outputPath;
		public string OutputPath
		{
			get { return outputPath; }
		}


		void OnEnable()
		{
			//スクロールを有効にする
			this.isEnableScroll = true;
		}

		//ウィンドウにプロパティを描画
		protected override bool DrawProperties()
		{
			bool ret = base.DrawProperties();
/*			SerializedObjectHelper.SerializedObject.Update();
			OnDrawCustom(this.SerializedObjectHelper);
			bool ret = SerializedObjectHelper.SerializedObject.ApplyModifiedProperties();*/

			if (!ret)
			{
				bool isEnable = (ResourcesDirectory != null || ProjectSetting != null) && !string.IsNullOrEmpty(OutputPath);
				EditorGUI.BeginDisabledGroup(!isEnable);
				bool isButton = GUILayout.Button("Convert", GUILayout.Width(180));
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(8f);

				if (isButton)
				{
					Convert();
				}
			}
			return ret;
		}
		
		//ファイルのコンバート
		void Convert()
		{
			try
			{
				AssetFileManager assetFileManager = FindObjectOfType<AssetFileManager>();
				if (assetFileManager == null)
				{
					Debug.LogError("FileManager is not found in current scene");
					return;
				}
				//ファイルの入出力に使う
				FileIOManager fileIOManager = assetFileManager.FileIOManager;
				switch (outputType)
				{
					case OutputType.Default:
					default:
						//アセットバンドルをビルド
						BuildAssetBundles(fileIOManager);
						break;
				}
				AssetDatabase.Refresh();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}


		//アセットバンドルのビルド
		void BuildAssetBundles(FileIOManager fileIOManager)
		{
			if (buildMode == AssetBundleBuildMode.None) return;

			//アセットバンドルをプラットフォーム別にビルド
			List<BuildTarget> buildTargets = new List<BuildTarget>();
			switch (buildMode)
			{
				case AssetBundleBuildMode.OnlyEditor://エディタ上のみ
					buildTargets.Add(AssetBundleHelper.BuildTargetFlagToBuildTarget(AssetBundleHelper.EditorAssetBundleTarget()));
					break;
				case AssetBundleBuildMode.AllPlatform://全プラットフォーム
					{
						buildTargets = AssetBundleHelper.BuildTargetFlagsToBuildTargetList(buildTargetFlags);
					}
					break;
				default:
					break;
			}

			MainAssetInfo inputDirAsset = new MainAssetInfo(this.ResourcesDirectory);
			List<MainAssetInfo> assets = GetAssetBudleList(inputDirAsset);
			RenameAssetBundles(inputDirAsset.AssetPath,assets);
			AssetBundleBuild[] builds = ToAssetBundleBuilds(assets);
			if (builds.Length <= 0) return;


			foreach (BuildTarget buildTarget in buildTargets)
			{
				string outputPath = FilePathUtil.Combine(OutputPath, AssetBundleHelper.BuildTargetToBuildTargetFlag(buildTarget).ToString());
				//出力先のディレクトリを作成
				if (!Directory.Exists(outputPath))
				{
					Directory.CreateDirectory(outputPath);
				}
				//アセットバンドルを作成
				AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, buildOptions, buildTarget);
				Debug.Log("BuildAssetBundles to " + buildTarget.ToString());
				if (isOutputLog)
				{
					WriteManifestLog(manifest, outputPath);
				}
			}
		}

		//アセットバンドルのリストを取得
		List<MainAssetInfo> GetAssetBudleList(MainAssetInfo inputDirAsset)
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			//シナリオ用のアセットを取得
			assets.AddRange(MakeScenarioAssetBudleList());
			//指定ディレクトリ以下のアセットを全て取得
			assets.AddRange(MakeAssetBudleList(inputDirAsset));
			return assets;
		}

		//シナリオのアセットバンドルのリストを取得
		List<MainAssetInfo> MakeScenarioAssetBudleList()
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			assets.Add(new MainAssetInfo(ProjectSetting.Scenarios));
			return assets;
		}

		//アセットバンドルのリストを取得
		List<MainAssetInfo> MakeAssetBudleList(MainAssetInfo inputDirAsset)
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			//指定ディレクトリ以下のアセットを全て取得
			foreach (MainAssetInfo asset in inputDirAsset.GetAllChildren())
			{
				if (asset.IsDirectory) continue;
				if (IsIgnoreAssetBundle(asset)) continue;
				assets.Add(asset);
			}
			return assets;
		}

		//アセットバンドル化しないアセットを取得
		bool IsIgnoreAssetBundle(MainAssetInfo asset)
		{
			string path = asset.AssetPath;
			if (path.EndsWith("keep.keep"))
			{
				return true;
			}

			return false;
		}


		//アセットバンドル名のリネーム
		void RenameAssetBundles(string rootPath, List<MainAssetInfo> assets)
		{
			if (renameType == AssetBundleRenameType.None) return;

			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}

				if (renameType == AssetBundleRenameType.OnlyNotNamed
					&& !string.IsNullOrEmpty(importer.assetBundleName))
				{
					//まだ名前がついていないときにのみネーミング
					continue;
				}

				string assetBundleName = ToAssetBundleName(rootPath,asset.AssetPath);
				//強制的にリネーム
				if (importer.assetBundleName != assetBundleName)
				{
					importer.assetBundleName = assetBundleName;
					importer.SaveAndReimport();
				}
			}
		}

		//アセットバンドル名を取得
		string ToAssetBundleName(string rootPath, string assetPath)
		{
			string name;
			if (assetPath.StartsWith(rootPath))
			{
				name = assetPath.Substring(rootPath.Length+1);
			}
			else
			{
				name = FilePathUtil.GetFileName(assetPath);
			}
			return FilePathUtil.ChangeExtension(name,".asset");
		}

		//アセットバンドルリストを取得
		AssetBundleBuild[] ToAssetBundleBuilds(List<MainAssetInfo> assets)
		{
			List<AssetBundleBuild> list = new List<AssetBundleBuild>();
			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}
				AssetBundleBuild build = new AssetBundleBuild();
				build.assetBundleName = importer.assetBundleName;
				build.assetBundleVariant = importer.assetBundleVariant;
				build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(importer.assetBundleName);
				list.Add(build);
			}
			return list.ToArray();
		}

		//ログファイルを書き込む
		void WriteManifestLog(AssetBundleManifest manifest, string outputPath)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach (string assetBundleName in manifest.GetAllAssetBundles())
			{
				builder.Append(assetBundleName);
				builder.AppendLine();

				Hash128 hash = manifest.GetAssetBundleHash(assetBundleName);
				builder.AppendFormat("  Hash128: {1}", assetBundleName, hash.ToString() );
				builder.AppendLine();


				builder.AppendLine();
			}
			string logFilePath = FilePathUtil.Combine(outputPath, Path.GetFileNameWithoutExtension(outputPath));
			logFilePath += ExtensionUtil.Log + ExtensionUtil.Txt;
			File.WriteAllText(logFilePath, builder.ToString());
		}


	}
#if false
	//「Utage」のリソースコンバーター
	public class AdvResourcesConverter : CustomEditorWindow
	{
		const string KeyScenario = "Scenario";
		const string KeyResouces = "Resouces";
		//****************リソース****************//

		//リソースのコンバート
		[SerializeField]
		bool isConvertResources = true;
		public bool IsConvertResources
		{
			get { return isConvertResources; }
			set { isConvertResources = value; }
		}

		/// <summary>
		/// リソースのパス
		/// </summary>
		[SerializeField]
		Object resourcesDirectory;
		public Object ResourcesDirectory
		{
			get { return resourcesDirectory; }
			set { resourcesDirectory = value; }
		}

		/// <summary>
		/// 新しいファイルのみコピーするか
		/// </summary>
		[SerializeField]
		bool isOnlyNew = true;
		public bool IsOnlyNew
		{
			get { return isOnlyNew; }
			set { isOnlyNew = value; }
		}

		//****************シナリオ****************//

		/// <summary>
		/// シナリオファイルもコンバートするか
		/// </summary>
		[SerializeField]
		bool isConvertScenario = true;
		public bool IsConvertScenario
		{
			get { return isConvertScenario; }
			set { isConvertScenario = value; }
		}

		/// <summary>
		/// シナリオファイルコンバート用のプロジェクトファイル
		/// </summary>
		[SerializeField]
		AdvScenarioDataProject projectSetting;
		public AdvScenarioDataProject ProjectSetting
		{
			get { return projectSetting; }
			set { projectSetting = value; }
		}

		/// <summary>
		/// シナリオファイルもコンバートするか
		/// </summary>
		bool isConvertScenarioToTsv = false;
		public bool IsConvertScenarioToTsv
		{
			get { return isConvertScenarioToTsv; }
			set { isConvertScenarioToTsv = value; }
		}

		//****************アセットバンドル****************//

		//アセットバンドルのビルドをするか
		enum AssetBundleBuildMode
		{
			None,			//ビルドしない
			OnlyEditor,		//エディタ用のみビルドする
			AllPlatform,	//全プラットフォーム用のものをビルドする
		};
		[SerializeField]
		AssetBundleBuildMode assetBundleBuildMode = AssetBundleBuildMode.OnlyEditor;

		[SerializeField]
		Object assetBundleDirectory;
		public Object AssetBundleDirectory
		{
			get { return assetBundleDirectory; }
			set { assetBundleDirectory = value; }
		}

		//アセットバンドルのリネーム法則
		public enum AssetBundleRenameType
		{
			None,			//名前を変えない
			Rename,			//リネームする
			OnlyNotNamed,	//まだ名前が設定されていないものだけリネームする
		};
		[SerializeField]
		AssetBundleRenameType assetBundleRenameType = AssetBundleRenameType.Rename;

		[SerializeField, EnumFlags]
		AssetBundleTargetFlags buildTargetFlags = AssetBundleTargetFlags.WebPlayer;


		//****************出力設定****************//

		public enum OutputType
		{
			Default,
			Advance,
		};
		[SerializeField]
		OutputType outputType;

		[SerializeField]
		bool isOutputLocal;

		[SerializeField]
		bool isOutputLocalLog = true;

		/// <summary>
		/// ローカル用のリソースの出力先のパス
		/// </summary>
		[SerializeField]
		Object localDirectory;
		public Object LocalDirectory
		{
			get { return localDirectory; }
			set { localDirectory = value; }
		}

		[SerializeField]
		bool isOutputServer;

		[SerializeField]
		bool isOutputServerLog = true;

		/// <summary>
		/// サーバー用のリソースの出力先のパス
		/// </summary>
		[SerializeField, PathDialog(PathDialogAttribute.DialogType.Directory)]
		string outputServerResourcePath;
		public string OutputServerResourcePath
		{
			get { return outputServerResourcePath; }
			set { outputServerResourcePath = value; }
		}

		/// <summary>
		/// サーバー用のリソースの出力先のパス
		/// </summary>
		[SerializeField, PathDialog(PathDialogAttribute.DialogType.Directory)]
		string advancedOutputPath;
		public string AdvancedOutputPath
		{
			get { return advancedOutputPath; }
			set { advancedOutputPath = value; }
		}

		[SerializeField]
		bool isOutputAdvancedLog = true;


		void OnEnable()
		{
			//スクロールを有効にする
			this.isEnableScroll = true;
		}

		protected override bool DrawProperties()
		{
			SerializedObjectHelper.SerializedObject.Update();
			OnDrawCustom(this.SerializedObjectHelper);
			bool ret = SerializedObjectHelper.SerializedObject.ApplyModifiedProperties();

			if (!ret)
			{
				bool isEnableOutputResources =
					ResourcesDirectory != null
					&& !(IsConvertScenario && ProjectSetting == null)
					;

				EditorGUI.BeginDisabledGroup(!isEnableOutputResources);
				bool isButton = GUILayout.Button("Convert", GUILayout.Width(180));
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(8f);

				if (isButton)
				{
					Convert();
				}
			}
			return ret;
		}

		void OnDrawCustom(SerializedObjectHelper helper)
		{
			helper.IsDrawScript = true;
			helper.DrawHeader();
			helper.BeginGroup("Resources");
			{
				helper.DrawProperty("isConvertResources", "Convert");
				if (this.IsConvertResources)
				{
					helper.DrawProperty("resourcesDirectory");
					helper.DrawProperty("isOnlyNew");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("Scenario");
			{
				helper.DrawProperty("isConvertScenario", "Convert");
				if (this.IsConvertScenario)
				{
					helper.DrawProperty("projectSetting", "Project Setting");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("AssetBundle");
			{
				helper.DrawProperty("assetBundleBuildMode", "BuildMode");
				if (this.assetBundleBuildMode != AssetBundleBuildMode.None)
				{
					helper.DrawProperty("assetBundleDirectory", "Directory");
					helper.DrawProperty("assetBundleRenameType", "RenameType");
					helper.DrawProperty("buildTargetFlags", "buildTarget");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("Output Setting");
			{
				helper.DrawProperty("outputType");
				if (outputType == OutputType.Default)
				{
					helper.BeginGroup("Local");
					{
						helper.DrawProperty("isOutputLocal", "Output");
						if (isOutputLocal)
						{
							helper.DrawProperty("isOutputLocalLog", "Output Log");
							helper.DrawProperty("localDirectory", "Directory");
						}
					}
					helper.EndGroup();

					helper.BeginGroup("Server");
					{
						helper.DrawProperty("isOutputServer", "Output");
						if (isOutputServer)
						{
							helper.DrawProperty("isOutputServerLog", "Output Log");
							helper.DrawProperty("outputServerResourcePath", "Output Path");
						}
					}
					helper.EndGroup();
				}
				else
				{
					helper.DrawProperty("isOutputAdvancedLog", "Output Log");
					helper.DrawProperty("advancedOutputPath");
				}
			}
			helper.EndGroup();
		}

		//ファイルのコンバート
		void Convert()
		{
			try
			{
				AssetFileManager assetFileManager = FindObjectOfType<AssetFileManager>();
				if (assetFileManager == null)
				{
					Debug.LogError("FileManager is not found in current scene");
					return;
				}
				//ファイルの入出力に使う
				FileIOManager fileIOManager = assetFileManager.FileIOManager;
				AssetFileManagerSettings settings = assetFileManager.Settings;

				if (outputType == OutputType.Default)
				{
					AssetFileManagerSettings.LoadType loadType = settings.LoadTypeSetting;
					if (isOutputLocal && LocalDirectory != null)
					{
						//ローカルの場合は、LoadTypeをLocalCompressedに
						settings.LoadTypeSetting = AssetFileManagerSettings.LoadType.LocalCompressed;
						string output = new MainAssetInfo(LocalDirectory).FullPath;
						//リソースをバージョンアップ
						AdvFileListConverter converter = new AdvFileListConverter(output, fileIOManager, settings);
						converter.VersionUp(VersionUpLocalFiles);
						if (isOutputLocalLog) converter.WriteLog(false);
					}
					if (isOutputServer && !string.IsNullOrEmpty(OutputServerResourcePath))
					{
						//サーバーの場合は、LoadTypeをServerに
						settings.LoadTypeSetting = AssetFileManagerSettings.LoadType.Server;
						//シナリオやリソースをバージョンアップ
						AdvFileListConverter converter = new AdvFileListConverter(OutputServerResourcePath, fileIOManager, settings);
						converter.VersionUp(VersionUpServerFiles);
						if (isOutputServerLog) converter.WriteLog(false);
						//アセットバンドルをビルド
						BuildAssetBundles(OutputServerResourcePath, fileIOManager, settings, isOutputServerLog);
					}
					settings.LoadTypeSetting = loadType;
				}
				else
				{
					//シナリオやリソースをバージョンアップ
					AdvFileListConverter converter = new AdvFileListConverter(AdvancedOutputPath, fileIOManager, settings);
					converter.VersionUp(VersionUpAdvanceFiles);
					if (isOutputAdvancedLog) converter.WriteLog(false);
					//アセットバンドルをビルド
					BuildAssetBundles(AdvancedOutputPath, fileIOManager, settings, isOutputAdvancedLog);
				}

				AssetDatabase.Refresh();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		//ローカルのリソースをバージョンアップ
		void VersionUpLocalFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
		}

		//サーバーのリソースやシナリオをバージョンアップ
		void VersionUpServerFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
			if (IsConvertScenario && IsConvertScenarioToTsv) VersionUpScenario(converter);
		}

		//特殊設定のリソースやシナリオをバージョンアップ
		void VersionUpAdvanceFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
			if (IsConvertScenario && IsConvertScenarioToTsv) VersionUpScenario(converter);
		}

		//リソースのバージョンアップ
		void VersionUpResources(AdvFileListConverter converter)
		{
			converter.VersionUpResouces(KeyResouces, ResourcesDirectory, IsOnlyNew);
		}

		//シナリオのバージョンアップ
		void VersionUpScenario(AdvFileListConverter converter)
		{
			converter.ConvertFileList.EditorVersionUp(KeyScenario, VersionUpScenarioSub(converter));
		}

		List<ConvertFileList.CusomFileVersionUpInfo> VersionUpScenarioSub(AdvFileListConverter converter)
		{
			List<ConvertFileList.CusomFileVersionUpInfo> list = new List<ConvertFileList.CusomFileVersionUpInfo>();

			AdvExcelCsvConverter excelConverter = new AdvExcelCsvConverter();
			List<AdvExcelCsvConverter.CsvInfo> csvInfoList = new List<AdvExcelCsvConverter.CsvInfo>();
			foreach (AdvScenarioDataProject.ChapterData item in ProjectSetting.ChapterDataList)
			{
				if (!excelConverter.TryConvertToCsvList(item.ExcelPathList, item.chapterName, 0, ref csvInfoList))
				{
					throw new System.Exception("Convert is failed");
				}
			}

			int count = 0;
			foreach (AdvExcelCsvConverter.CsvInfo csvInfo in csvInfoList)
			{
				ConvertFileList.CusomFileVersionUpInfo versionUpInfo;
				if (converter.TryVersionUpFileFromMem(csvInfo.Path + ExtensionUtil.TSV, System.Text.Encoding.UTF8.GetBytes(csvInfo.Grid.ToText()), out versionUpInfo))
				{
					++count;
				}
				list.Add(versionUpInfo);
			}
			Debug.Log(string.Format("Scenario {0}/{1} files updated", count, csvInfoList.Count));
			return list;
		}

		//アセットバンドルのビルド
		void BuildAssetBundles(string outPutDirectoryPath, FileIOManager fileIOManager, AssetFileManagerSettings settings, bool isOutputLog)
		{
			if (assetBundleBuildMode == AssetBundleBuildMode.None) return;

			//アセットバンドルをプラットフォーム別にビルド
			List<BuildTarget> buildTargets = new List<BuildTarget>();
			switch (assetBundleBuildMode)
			{
				case AssetBundleBuildMode.OnlyEditor://エディタ上のみ
					buildTargets.Add(EditorUserBuildSettings.activeBuildTarget);
					break;
				case AssetBundleBuildMode.AllPlatform://全プラットフォーム
					{
						buildTargets = AssetBundleHelper.BuildTargetFlagsToBuildTargetList(buildTargetFlags);
					}
					break;
				default:
					break;
			}

			List<MainAssetInfo> assets = GetAssetBudleList(settings);
			RenameAssetBundles(assets);
			AssetBundleBuild[] builds = ToAssetBundleBuilds(assets);
			if (builds.Length <= 0) return;


			foreach (BuildTarget buildTarget in buildTargets)
			{
				string outputPath = FilePathUtil.Combine(outPutDirectoryPath, AssetBundleHelper.BuildTargetToBuildTargetFlag(buildTarget).ToString());
				//出力先のディレクトリを作成
				if (!Directory.Exists(outputPath))
				{
					Directory.CreateDirectory(outputPath);
				}
				//アセットバンドルを作成
				AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.None, buildTarget);

				//アセットバンドルの情報をバージョンアップ
				AdvFileListConverter converter = new AdvFileListConverter(outputPath, fileIOManager, settings);
				converter.VersionUp(
					(x) =>
					{
						int count = x.ConvertFileList.EditorVersionUpAssetBundle(manifest, buildTarget);
						Debug.Log("" + count + " AssetBundle version up to target " + buildTarget.ToString());
					});
				if (isOutputLog) converter.WriteLog(true);
			}
		}

		//アセットバンドルのリストを取得
		List<MainAssetInfo> GetAssetBudleList(AssetFileManagerSettings settings)
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			//シナリオ用のアセットを取得
			if (IsConvertScenario && !IsConvertScenarioToTsv)
			{
				assets.AddRange(MakeScenarioAssetBudleList());
			}
			//指定ディレクトリ以下のアセットを全て取得
			assets.AddRange( MakeAssetBudleList(AssetBundleDirectory,settings) );
			return assets;
		}

		//シナリオのアセットバンドルのリストを取得
		List<MainAssetInfo> MakeScenarioAssetBudleList()
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			assets.Add(new MainAssetInfo(ProjectSetting.Scenarios));
			return assets;
		}

		//アセットバンドルのリストを取得
		List<MainAssetInfo> MakeAssetBudleList(Object rootDirectory, AssetFileManagerSettings settings)
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			//指定ディレクトリ以下のアセットを全て取得
			MainAssetInfo inputDirAsset = new MainAssetInfo(AssetBundleDirectory);
			foreach (MainAssetInfo asset in inputDirAsset.GetAllChildren())
			{
				if (asset.IsDirectory) continue;
				if (IsIgnoreAssetBundle(asset)) continue;
				AssetFileSetting assetFileSetting = settings.FindSettingFromPath(asset.AssetPath);
				AssetFileEncodeType encodeType = assetFileSetting.EncodeType;
				switch (encodeType)
				{
					case AssetFileEncodeType.AssetBundle:
						assets.Add(asset);
						break;
					default:
						break;
				}
			}
			return assets;
		}

		//アセットバンドル化しないアセットを取得
		bool IsIgnoreAssetBundle(MainAssetInfo asset)
		{
			string path = asset.AssetPath;
			if (path.EndsWith("keep.keep"))
			{
				return true;
			}

			return false;
		}


		//アセットバンドル名のリネーム
		void RenameAssetBundles(List<MainAssetInfo> assets)
		{
			if (assetBundleRenameType == AssetBundleRenameType.None) return;

			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}

				if (assetBundleRenameType == AssetBundleRenameType.OnlyNotNamed
					&& !string.IsNullOrEmpty(importer.assetBundleName))
				{
					//まだ名前がついていないときにのみネーミング
					continue;
				}

				string assetBundleName = Path.GetFileNameWithoutExtension(asset.AssetPath);
				//強制的にリネーム
				if (importer.assetBundleName != assetBundleName)
				{
					importer.assetBundleName = assetBundleName;
					importer.SaveAndReimport();
				}
			}
		}

		//アセットバンドルリストを取得
		AssetBundleBuild[] ToAssetBundleBuilds(List<MainAssetInfo> assets)
		{
			List<AssetBundleBuild> list = new List<AssetBundleBuild>();
			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}
				AssetBundleBuild build = new AssetBundleBuild();
				build.assetBundleName = importer.assetBundleName;
				build.assetBundleVariant = importer.assetBundleVariant;
				build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(importer.assetBundleName);
				list.Add(build);
			}
			return list.ToArray();
		}

	}
#endif
}