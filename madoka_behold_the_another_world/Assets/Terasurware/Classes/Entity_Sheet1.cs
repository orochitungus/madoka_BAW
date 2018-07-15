using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_Sheet1 : ScriptableObject
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
		public int Def_Growth;
		public int Boost_OR;
		public int Boost_Growth;
		public double Arousal_OR;
		public double Arousal_Growth;
		public double JumpWaitTime;
		public double LandingWaitTime;
		public double WalkSpeed;
		public double RunSpeed;
		public double AirDashSpeed;
		public double AirMoveSpeed;
		public double RiseSpeed;
		public double JumpUseBoost;
		public double DashCancelUseBoost;
		public double StepUseBoost;
		public double BoostLess;
		public double StepMoveLength;
		public double StepInitalVelocity;
		public double StepMove1F;
		public double ColliderHeight;
		public double RockonRange;
		public double RockonRangeLimit;
		public double EXP;
		public double DownDurationValue;
	}
}

