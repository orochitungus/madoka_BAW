using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class CharacterBasicSpec_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/DataFile/CharacterBasicSpec.xlsx";
	private static readonly string exportPath = "Assets/DataFile/CharacterBasicSpec.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			CharacterBasicSpec data = (CharacterBasicSpec)AssetDatabase.LoadAssetAtPath (exportPath, typeof(CharacterBasicSpec));
			if (data == null) {
				data = ScriptableObject.CreateInstance<CharacterBasicSpec> ();
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

					CharacterBasicSpec.Sheet s = new CharacterBasicSpec.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						CharacterBasicSpec.Param p = new CharacterBasicSpec.Param ();
						
					cell = row.GetCell(0); p.NAME_JP = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.NAME_EN = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.HP_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.HP_Growth = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Def_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Def_Growth = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Boost_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.Boost_Growth = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.Arousal_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.Arousal_Growth = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.JumpWaitTime = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.LandingWaitTime = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.WalkSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.RunSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.AirDashSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.AirMoveSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.RiseSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.JumpUseBoost = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.DashCancelUseBoost = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(19); p.StepUseBoost = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.BoostLess = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(21); p.StepMoveLength = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(22); p.StepInitalVelocity = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(23); p.StepMove1F = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(24); p.ColliderHeight = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(25); p.RockonRange = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(26); p.RockonRangeLimit = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(27); p.EXP = (int)(cell == null ? 0 : cell.NumericCellValue);
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
