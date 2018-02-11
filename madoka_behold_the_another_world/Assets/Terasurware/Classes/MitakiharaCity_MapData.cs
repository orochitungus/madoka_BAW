using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MitakiharaCity_MapData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public string NAME;
		public string NAME_JP;
		public string NAME_EN;
		public float CameraPosX;
		public float CameraPosY;
		public float CameraPosZ;
		public float CameraRotX;
		public float CameraRotY;
		public float CameraRotZ;
		public int LowStory;
		public int HighStory;
		public int FieldOfView;
		public int FromCode;
		public string StageFileName;
		public int ForCode;
		public bool NowPosition;
	}
}

