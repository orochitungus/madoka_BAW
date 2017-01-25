using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace BehaviourTrees
{
	public class MajyuCPU : MonoBehaviour
	{
		BehaviourTreeInstance node;

		/// <summary>
		/// 敵がレンジ内にいるか否か
		/// </summary>
		public bool EnemyExist;
		
		// Use this for initialization
		void Start()
		{
				
		}

		// Update is called once per frame
		void Update()
		{

		}

		/// <summary>
		/// （SelectorNode）敵がレンジ内にいるか否か判定する
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private ExecutionResult IsEnemyExist(BehaviourTreeInstance instance)
		{
			if(EnemyExist)
			{
				Debug.Log("敵がいる。ロックして攻撃");
				return new ExecutionResult(true);
			}
			Debug.Log("敵はいない。哨戒を続ける");
			return new ExecutionResult(false);
		}

		/// <summary>
		/// 敵をロックする
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private ExecutionResult EnemyRock(BehaviourTreeInstance instance)
		{
			Debug.Log("敵をロックする");
			return new ExecutionResult(true);
		}

		/// <summary>
		/// 折り返し地点へ移動する
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private ExecutionResult GotoReturnPoint(BehaviourTreeInstance instance)
		{
			Debug.Log("折り返し地点へ移動");
			return new ExecutionResult(true);
		}

		private ExecutionResult GotoStartPoint(BehaviourTreeInstance instance)
		{
			Debug.Log("スタート地点へ移動");
			return new ExecutionResult(true);
		}
	}
}
