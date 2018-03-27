using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageSkyData : ScriptableObject
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
		
		public int MinStory;
		public int MaxStory;
		public float RotateX;
		public float RotateY;
		public float RotateZ;
		public string Comment;
	}
}

