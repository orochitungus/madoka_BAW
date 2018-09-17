using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSkillData : ScriptableObject
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
		
		public string SkillType;
		public string HitType;
		public string SkillName;
		public int OriginalStr;
		public int AntiBoostStr;
		public int GrowthCoefficientStr;
		public int OriginalBulletNum;
		public int GrowthCoefficientBul;
		public float DownPoint;
		public int LearningLevel;
		public float WaitTime;
		public float MoveSpeed;
		public float AnimSpeed;
		public float Arousal;
		public string ReloadType;
		public float ReloadTime;
	}
}

