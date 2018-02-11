using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StagePositionRotData : ScriptableObject
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
		
		public int ID;
		public float Xpos;
		public float Ypos;
		public float Zpos;
		public float Xrot;
		public float Yrot;
		public float Zrot;
	}
}

