// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimurausing System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Utage
{
	public class AdvParamStruct
	{
		public Dictionary<string, AdvParamData> Tbl { get { return tbl; } }
		Dictionary<string, AdvParamData> tbl = new Dictionary<string, AdvParamData>();

		public AdvParamStruct() { }

		//通常のパラメーターを追加
		public void AddData(StringGrid grid)
		{
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;
				if (row.IsEmptyOrCommantOut) continue;
				AdvParamData data = new AdvParamData();
				if (!data.TryParse(row))
				{
					Debug.LogError(row.ToErrorString(" Parse Error"));
					continue;
				}
				else
				{
					if (Tbl.ContainsKey(data.Key))
					{
						Debug.LogError(row.ToErrorString(data.Key + " is already contaisn"));
					}
					else
					{
						Tbl.Add(data.Key, data);
					}
				}
			}
		}

		//構造体のヘッダ情報として初期化
		public AdvParamStruct(StringGridRow names, StringGridRow types, StringGridRow fileTypes)
		{
			if (names.Length != types.Length)
			{
				Debug.LogError( names.Grid.Name + " Contains Cell in Name or Type" );
				return;
			}
			for( int i = 1; i < names.Length; ++i )
			{
				AdvParamData data = new AdvParamData();
				string fileType = (i < fileTypes.Length) ? fileTypes.Strings[i] : "";
				if (!data.TryParse(names.Strings[i], types.Strings[i], fileType))
				{
					Debug.LogError(string.Format( "{0} Header [<b>{1}</b>]: ", names.Grid.Name, i));
					continue;
				}
				Tbl.Add(data.Key, data );
			}
		}

		//ヘッダ情報と値一覧から初期化
		public AdvParamStruct(AdvParamStruct header, StringGridRow values)
		{
			int index = 1;
			foreach (var keyValue in header.Tbl)
			{
				if (!keyValue.Value.Key.StartsWith("//"))
				{
					string value = index < values.Length ? values.Strings[index] : "";
					AdvParamData data = new AdvParamData();
					if (!data.TryParse(keyValue.Value, value))
					{
						Debug.LogError(values.ToErrorString(" Parse Error <b>" + value + "</b>"));
						continue;
					}
					Tbl.Add(data.Key, data);
				}
				++index;
			}
		}

		//中身を全てコピー
		internal AdvParamStruct Clone()
		{
			AdvParamStruct clone = new AdvParamStruct();
			foreach (var item in Tbl)
			{
				AdvParamData param = new AdvParamData();
				param.Copy(item.Value);
				clone.Tbl.Add(item.Key, param);
			}
			return clone;
		}

		/// <summary>
		/// システムデータ以外の値をデフォルト値で初期化
		/// </summary>
		internal void InitDefaultNormal(AdvParamStruct src)
		{
			foreach (var keyValue in src.Tbl)
			{
				if (keyValue.Value.SaveFileType == AdvParamData.FileType.System) continue;

				AdvParamData data;
				if (Tbl.TryGetValue(keyValue.Key, out data))
				{
					data.Copy(keyValue.Value);
				}
				else
				{
					Debug.LogError("Param: " + keyValue.Key + "  is not found in default param");
				}
			}
		}


		//システムファイルのセーブするデータ数をカウント
		public int CountFileType( AdvParamData.FileType fileType )
		{
			int count = 0;
			foreach (var keyValue in Tbl)
			{
				if (keyValue.Value.SaveFileType == fileType)
				{
					++count;
				}
			}
			return count;
		}

		public List<AdvParamData> GetFileTypeList( AdvParamData.FileType fileType )
		{
			List<AdvParamData> list = new List<AdvParamData>();
			foreach (var keyValue in Tbl)
			{
				if (keyValue.Value.SaveFileType == fileType)
				{
					list.Add(keyValue.Value);
				}
			}
			return list;
		}

		const int Version = 0;
		//セーブデータ用のバイナリ書き込み
		public void Write(BinaryWriter writer, AdvParamData.FileType fileType)
		{
//			Profiler.BeginSample("Write Param");
			int count = CountFileType(fileType);
			writer.Write(Version);
			writer.Write(count);

			foreach (var keyValue in Tbl)
			{
				if (keyValue.Value.SaveFileType == fileType)
				{
					writer.Write(keyValue.Value.Key);
					writer.Write(keyValue.Value.ParameterString);
				}
			}
//			Profiler.EndSample();
		}

		//セーブデータ用のバイナリ読み込み
		public void Read(BinaryReader reader, AdvParamData.FileType fileType)
		{
			int version = reader.ReadInt32();
			if (version < 0 || version > Version)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
				return;
			}

			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				string key = reader.ReadString();
				string paramString = reader.ReadString();
				AdvParamData data;
				if (Tbl.TryGetValue(key, out data))
				{
					if (data.SaveFileType == fileType)
					{
						data.Read(paramString);
					}
				}
				else
				{
					//セーブされていたが、パラメーター設定から消えているので読み込まない
				}
			}
		}
	};
}
