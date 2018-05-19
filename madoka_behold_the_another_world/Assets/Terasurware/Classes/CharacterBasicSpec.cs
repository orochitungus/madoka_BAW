using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBasicSpec : ScriptableObject
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
		
		public string NAME_JP;
		public string NAME_EN;
		public int HP_OR;
		public int HP_Growth;
		public int Def_OR;
		public float Def_Growth;
		public int Boost_OR;
		public float Boost_Growth;
		public int Arousal_OR;
		public float Arousal_Growth;
		public float JumpWaitTime;
		public float LandingWaitTime;
		public float WalkSpeed;
		public float RunSpeed;
		public float AirDashSpeed;
		public float AirMoveSpeed;
		public float RiseSpeed;
		public float JumpUseBoost;
		public float DashCancelUseBoost;
		public float StepUseBoost;
		public float BoostLess;
		public float StepMoveLength;
		public float StepInitalVelocity;
		public float StepMove1F;
		public float ColliderHeight;
		public float RockonRange;
		public float RockonRangeLimit;
		public int EXP;
		public float DownDurationValue;
	}
}

