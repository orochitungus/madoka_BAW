using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StageCodeData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/DataFile/StageCodeData.xlsx";
	private static readonly string exportPath = "Assets/DataFile/StageCodeData.asset";
	private static readonly string[] sheetNames = { "Test","BrokenMitakihara","ImagicashockGroundZero","MitakiharaHospitalHomuraRoom","MitakiharaHospital56F","MitakiharaHospital106F","MitakiharaHospital150F","MitakiharaHospital1F","MitakiharaHospitalBycicleyard","MitakiharaCity","FootBridge","MitakiharaStationArea", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			StageCodeData data = (StageCodeData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StageCodeData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StageCodeData> ();
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

					StageCodeData.Sheet s = new StageCodeData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						StageCodeData.Param p = new StageCodeData.Param ();
						
					cell = row.GetCell(0); p.StagefromIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.BGM = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.ChangeBGM = (cell == null ? false : cell.BooleanCellValue);
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
