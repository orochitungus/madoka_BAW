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
				
			Entity_Sheet1 data = (Entity_Sheet1)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_Sheet1));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_Sheet1> ();
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

					Entity_Sheet1.Sheet s = new Entity_Sheet1.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_Sheet1.Param p = new Entity_Sheet1.Param ();
						
					cell = row.GetCell(0); p.NAME_JP = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.NAME_EN = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.HP_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.HP_Growth = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Def_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Def_Growth = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Boost_OR = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.Boost_Growth = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.Arousal_OR = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.Arousal_Growth = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.JumpWaitTime = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.LandingWaitTime = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.WalkSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.RunSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.AirDashSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.AirMoveSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.RiseSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.JumpUseBoost = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.DashCancelUseBoost = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(19); p.StepUseBoost = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.BoostLess = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(21); p.StepMoveLength = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(22); p.StepInitalVelocity = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(23); p.StepMove1F = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(24); p.ColliderHeight = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(25); p.RockonRange = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(26); p.RockonRangeLimit = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(27); p.EXP = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(28); p.DownDurationValue = (cell == null ? 0.0 : cell.NumericCellValue);
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
