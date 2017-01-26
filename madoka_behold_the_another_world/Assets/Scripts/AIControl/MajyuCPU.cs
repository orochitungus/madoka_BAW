using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace BehaviourTrees
{
	/// <summary>
	/// CPU操作基本
	/// XXCPUにBehaviourTreeで作った基本ルーチンを入れる
	/// ルーチンに応じてそのルーチンを実行するためのコマンドをCPUControllerに入れる
	/// CPUControllerのフラグが立ちキャラクターが動く
	/// </summary>
	public class MajyuCPU : MonoBehaviour
	{
		BehaviourTreeInstance node;

		/// <summary>
		/// コントローラー制御
		/// </summary>
		public CPUController Cpucontroller;

		/// <summary>
		/// 制御対象
		/// </summary>
		public CharacterControlBase ControlTarget;

		/// <summary>
		/// 制御対象のカメラ
		/// </summary>
		public Player_Camera_Controller Playercameracontroller;

		/// <summary>
		/// 敵がレンジ内にいるか否か
		/// </summary>
		public bool EnemyExist;

		
		public CPUNowMode Cpunowmode;
		
		// Use this for initialization
		void Start()
		{
			if (Cpunowmode == CPUNowMode.CPU)
			{
				// 哨戒時の索敵判定
				var lockonnode = new DecoratorNode(IsEnemyExist, new ActionNode(EnemyRock));
				// 哨戒時の移動判定
				var returnchecknode = new SequencerNode(new BehaviourTreeBase[]
				{
					new ActionNode(GotoReturnPoint),
					new ActionNode(GotoStartPoint)
				});

				// 最初の判定
				var rootnode = new SelectorNode(new BehaviourTreeBase[]
				{
					lockonnode,
					returnchecknode
				});

				// 最初の判定を定義する
				node = new BehaviourTreeInstance(rootnode);
				// 全部の判定を終えたらリセットする
				node.finishRP.Where(p => p != BehaviourTreeInstance.NodeState.READY).Subscribe(p => ResetCoroutineStart());
				node.Excute();
			}
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
			// 折り返し地点に近接していたらfalseを返す
			if (Vector3.Distance(transform.position, ControlTarget.EndingPoint.transform.position) < 1.0f)
			{
				Debug.Log("折り返し地点に到達");
				// スタート地点をロックする
				Playercameracontroller.RockOnTarget.Clear();
				Playercameracontroller.RockOnTarget.Add(ControlTarget.StartingPoint);
				ControlTarget.IsRockon = true;
				Playercameracontroller.Enemy = ControlTarget.StartingPoint;
				Playercameracontroller.IsRockOn = true;
				Cpucontroller.Top = true;
				return new ExecutionResult(false);
			}
			else
			{
				// ロックしていない場合はロックする
				if(Playercameracontroller.RockOnTarget.Count == 0)
				{
					Playercameracontroller.RockOnTarget.Add(ControlTarget.StartingPoint);
					ControlTarget.IsRockon = true;
					Playercameracontroller.Enemy = ControlTarget.StartingPoint;
					Playercameracontroller.IsRockOn = true;
				}
				Debug.Log("折り返し地点へ移動");
				Cpucontroller.Top = true;
				return new ExecutionResult(true);
			}
		}

		private ExecutionResult GotoStartPoint(BehaviourTreeInstance instance)
		{
			// スタート地点に近接していたらfalseを返す
			if (Vector3.Distance(transform.position, ControlTarget.StartingPoint.transform.position) < 1.0f)
			{
				Debug.Log("スタート地点に到達");
				// 折り返し地点をロックする
				Playercameracontroller.RockOnTarget.Clear();
				Playercameracontroller.RockOnTarget.Add(ControlTarget.EndingPoint);
				ControlTarget.IsRockon = true;
				Playercameracontroller.Enemy = ControlTarget.EndingPoint;
				Playercameracontroller.IsRockOn = true;
				Cpucontroller.Top = true;
				return new ExecutionResult(false);
			}
			else
			{
				// ロックしていない場合はロックする
				if(Playercameracontroller.RockOnTarget.Count == 0)
				{
					Playercameracontroller.RockOnTarget.Add(ControlTarget.EndingPoint);
					ControlTarget.IsRockon = true;
					Playercameracontroller.Enemy = ControlTarget.EndingPoint;
					Playercameracontroller.IsRockOn = true;
				}
				Debug.Log("スタート地点へ移動");
				Cpucontroller.Top = true;
				return new ExecutionResult(true);
			}
		}

		private void ResetCoroutineStart()
		{
			StartCoroutine(WaitCoroutine());
		}

		/// <summary>
		/// 次の思考に至るまでの時間
		/// </summary>
		/// <returns></returns>
		IEnumerator WaitCoroutine()
		{
			yield return new WaitForSeconds(1.0f);
			node.Reset();
		}

	}
}
