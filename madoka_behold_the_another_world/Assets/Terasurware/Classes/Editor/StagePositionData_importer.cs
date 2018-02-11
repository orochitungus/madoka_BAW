using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StagePositionData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/DataFile/StagePositionData.xlsx";
	private static readonly string exportPath = "Assets/DataFile/StagePositionData.asset";
	private static readonly string[] sheetNames = { "Test","BrokenMitakihara","ImagicaShockGroundZero","MitakiharaHospitalHomuraRoom","MitakiharaHospital56F","MitakiharaHospital106F","MitakiharaHospital150F","MitakiharaHospital1F","MitakiharaHospitalBicycleYard","MitakiharaCityMap","FootBridge","MitakiharaStationArea", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			StagePositionRotData data = (StagePositionRotData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StagePositionRotData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StagePositionRotData> ();
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

					StagePositionRotData.Sheet s = new StagePositionRotData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						StagePositionRotData.Param p = new StagePositionRotData.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Xpos = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Ypos = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.Zpos = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Xrot = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Yrot = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Zrot = (float)(cell == null ? 0 : cell.NumericCellValue);
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
