using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class CharacterSkillData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/DataFile/CharacterSkillData.xlsx";
	private static readonly string exportPath = "Assets/DataFile/CharacterSkillData.asset";
	private static readonly string[] sheetNames = { "なし","madoka","sayaka","homura_g","mami","kyoko","yuma","kirika","oriko","sconosciuto","homura_b","u_madoka","majyu","d_homura","nagisa","sayaka_g","michel", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			CharacterSkillData data = (CharacterSkillData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(CharacterSkillData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<CharacterSkillData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					CharacterSkillData.Sheet s = new CharacterSkillData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						CharacterSkillData.Param p = new CharacterSkillData.Param ();
						
					cell = row.GetCell(0); p.SkillType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.HitType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.SkillName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.OriginalStr = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.GrowthCoefficientStr = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.OriginalBulletNum = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.GrowthCoefficientBul = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.DownPoint = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.LearningLevel = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.WaitTime = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.MoveSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.AnimSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.Arousal = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.ReloadType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(14); p.ReloadTime = (float)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
