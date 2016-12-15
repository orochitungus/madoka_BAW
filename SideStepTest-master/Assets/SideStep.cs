using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 実験用
/// </summary>
namespace SaitamaGames.Sandbox
{
    /// <summary>
    /// サイドステップのテスト
    /// </summary>
    public class SideStep : MonoBehaviour 
    {
        /// <summary>敵</summary>
        [SerializeField]
        private GameObject enemy;

        /// <summary>ステップ速度</summary>
        [SerializeField, Range(0, 1)]
        private float stepSpeed = 0.16f;

        /// <summary>ステップ先</summary>
        private Vector3 target;

        /// <summary>ステップ中にフォーカスする座標</summary>
        private Vector3 focus;

        /// <summary>回転用の角度を保存</summary>
        private float degeree;


        /// <summary>Mono Method</summary>
        void Start () 
        {
        
        }
        
        /// <summary>Mono Method</summary>
        void Update () 
        {
            var now = transform.position - focus;
            var rotateAmount = degeree * stepSpeed;
            degeree -= rotateAmount;
            if (Mathf.Abs(degeree) < 0.5f)
            {
                rotateAmount = degeree;
                degeree = 0;
            }
            var rot = Quaternion.AngleAxis(rotateAmount, Vector3.up) * now;

            // 座標と回転の設定
            transform.position = focus + rot; 
            transform.rotation = Quaternion.LookRotation(-now, Vector3.up);
        }

        /// <summary>
        /// サイドステップを行う
        /// </summary>
        /// <param name="deg">角度 in degree</param>
        public void Step(float deg)
        {
            // 敵の位置をA、プレイヤーの位置をB、移動先のプレイヤーの座標をB'とすると
            focus = enemy.transform.position;
            var now = transform.position - focus; // A→B
            target = Quaternion.AngleAxis(deg, Vector3.up) * now; // A→B'
            degeree = deg;
        }
    }
}
