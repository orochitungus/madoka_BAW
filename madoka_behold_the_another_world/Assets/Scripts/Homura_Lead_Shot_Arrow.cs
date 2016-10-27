using UnityEngine;
using System.Collections;

//public class Homura_Lead_Shot_Arrow : Bullet 
//{

//    // 分散までの時間（生成時点でカウントが始まるので、射出以降の時間になるように調整する）
//    public float m_Insp_LeapTime;
//    // 分散までの累積時間
//    private float m_ShotTime;

//    // 左側の拡散弾
//    public GameObject m_Insp_LeftArrow;
//    // 右側の拡散弾
//    public GameObject m_Insp_RightArrow;
//    // 左右の矢を作ったか
//    private bool m_cleateSubArrow;

//	// Use this for initialization
//	void Start () 
//    {
//        // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
//        this.LifeTime = 4.0f;
//        this.MoveDirection = Vector3.zero;
//        // 独立フラグを初期化
//        this.IsIndependence = false;
//        // 誘導カウンターを初期化
//        this.InductionCounter = 0;
//        // 誘導時間を初期化
//        this.InductionBias = 30;
//        // 親スクリプトを初期化
//        this.ParentScriptName = "Homura_Final_BattleControl";
//        InjectionObjectName = "homura_ribon_magica_battle_use";
//        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）
//        InjectionObject = transform.root.GetComponentInChildren<Homura_Final_BattleControl>().gameObject;
//        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
//        Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), InjectionObject.transform.GetComponent<Collider>());
//        // 撃ったキャラが誰であるか保持
//        InjectionCharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
//        // 累積時間を初期化
//        m_ShotTime = 0;
//        m_cleateSubArrow = false;
//	}
	
//	// Update is called once per frame
//	void Update () 
//    {
//        UpdateCore();
  
//        // 累積時間加算
//        m_ShotTime += Time.deltaTime;
//        // 累積時間一定以上で射出した矢のleftとrightにInstantiateで2本の矢を生成
//        if (!m_cleateSubArrow && m_ShotTime > m_Insp_LeapTime)
//        {
//            // 左側に矢を生成して子にする
//            // 子オブジェクトを取得
//            GameObject Left = gameObject.transform.FindChild("left").gameObject;
//            // 子オブジェクトの場所に左の矢を生成する
//            GameObject leftarrow = (GameObject)GameObject.Instantiate(m_Insp_LeftArrow, Left.transform.position, Left.transform.rotation);
//            leftarrow.transform.parent = Left.transform;
//            // 右側に矢を生成して子にする
//            // 子オブジェクトを取得
//            GameObject Right = gameObject.transform.FindChild("right").gameObject;
//            // 子オブジェクトに右の矢を生成する
//            GameObject rightarrow = (GameObject)GameObject.Instantiate(m_Insp_RightArrow, Right.transform.position, Right.transform.rotation);
//            rightarrow.transform.parent = Right.transform;

//            m_cleateSubArrow = true;
//        }
//	}
//}
