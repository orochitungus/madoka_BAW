using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class MitakiharaCityMapData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/DataFile/MitakiharaCityMapData.xlsx";
	private static readonly string exportPath = "Assets/DataFile/MitakiharaCityMapData.asset";
	private static readonly string[] sheetNames = { "MapData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			MitakiharaCity_MapData data = (MitakiharaCity_MapData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(MitakiharaCity_MapData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<MitakiharaCity_MapData> ();
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

					MitakiharaCity_MapData.Sheet s = new MitakiharaCity_MapData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						MitakiharaCity_MapData.Param p = new MitakiharaCity_MapData.Param ();
						
					cell = row.GetCell(0); p.NAME = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.NAME_JP = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.NAME_EN = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.CameraPosX = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.CameraPosY = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.CameraPosZ = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.CameraRotX = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.CameraRotY = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.CameraRotZ = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.LowStory = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.HighStory = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.FieldOfView = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.FromCode = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.StageFileName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(14); p.ForCode = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.NowPosition = (cell == null ? false : cell.BooleanCellValue);
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
