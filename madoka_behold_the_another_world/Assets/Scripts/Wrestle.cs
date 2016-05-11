using UnityEngine;
using System.Collections;


// CharacterControl_Baseの格闘共通クラス
// 特異なものはともかく、一般的なものはこっちに書いておく
public partial class CharacterControl_Base
{

    // 格闘時の移動速度
    protected float m_wrestlSpeed;        // N格闘1段目

    // 追加入力の有無を保持。trueであり
    protected bool m_addInput;


    // 格闘攻撃の種類
    public enum WrestleType
    {
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        CHARGE_WRESTLE,         // 格闘チャージ
        FRONT_WRESTLE_1,        // 前格闘1段目
        FRONT_WRESTLE_2,        // 前格闘2段目
        FRONT_WRESTLE_3,        // 前格闘3段目
        LEFT_WRESTLE_1,         // 左横格闘1段目
        LEFT_WRESTLE_2,         // 左横格闘2段目
        LEFT_WRESTLE_3,         // 左横格闘3段目
        RIGHT_WRESTLE_1,        // 右横格闘1段目
        RIGHT_WRESTLE_2,        // 右横格闘2段目
        RIGHT_WRESTLE_3,        // 右横格闘3段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_WRESTLE_1,           // 特殊格闘1段目
        EX_WRESTLE_2,           // 特殊格闘2段目
        EX_WRESTLE_3,           // 特殊格闘3段目
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        EX_FRONT_WRESTLE_2,     // 前特殊格闘2段目
        EX_FRONT_WRESTLE_3,     // 前特殊格闘3段目
        EX_LEFT_WRESTLE_1,      // 左横特殊格闘1段目
        EX_LEFT_WRESTLE_2,      // 左横特殊格闘2段目
        EX_LEFT_WRESTLE_3,      // 左横特殊格闘3段目
        EX_RIGHT_WRESTLE_1,     // 右横特殊格闘1段目
        EX_RIGHT_WRESTLE_2,     // 右横特殊格闘2段目
        EX_RIGHT_WRESTLE_3,     // 右横特殊格闘3段目
        BACK_EX_WRESTLE,        // 後特殊格闘
        // キャラごとの特殊な処理はこの後に追加、さやかのスクワルタトーレのような連続で切りつける技など

        WRESTLE_TOTAL
    };


    // N格1段目用判定の配置用フック(キャラごとに設定する。順番は上の列挙体と同じ）
    public GameObject []m_WrestleRoot = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

    // 格闘判定のオブジェクト
    public GameObject[] m_WrestleObject = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

    // 格闘開始（一応派生は認めておく。専用のはそっちで利用）
    // WrestleType      [in]:格闘攻撃の種類
    // skilltype            [in]：スキルのインデックス(キャラごとに異なる)
    protected virtual void WrestleDone(AnimationState WrestleType, int skilltype)
    {
        // 追加入力フラグをカット
        this.m_addInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 上記の座標は足元を向いているので、自分の高さに補正する
            targetpos.y = transform.position.y;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを向けた方向に合わせる            
            m_MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
        }
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = m_MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR = Quaternion.Euler(rotateOR_E);
            this.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
        }
        // それ以外は本体の角度を移動方向にする
        else
        {
            this.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
        }
        // アニメーション速度
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        // m_AnimStateを変更する
        this.m_AnimState[0] = WrestleType;

        // ステートを変更
        m_AnimState[0] = WrestleType;        
        // アニメーションを再生する
        this.GetComponent<Animation>().Play(m_AnimationNames[(int)WrestleType]);
        // アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)WrestleType]].speed = speed;
        // 移動速度を調整する
        this.m_wrestlSpeed = movespeed;       
    }

    // 回り込み近接・左(相手の斜め前へ移動して回り込むタイプ）
    // WrestleType      [in]:格闘攻撃の種類
    // skilltype            [in]：スキルのインデックス(キャラごとに異なる)
    protected virtual void WrestleDone_GoAround_Left(AnimationState WrestleType, int skilltype)
    {
        // 追加入力フラグをカット
        this.m_addInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の左側を移動方向とする（通常時のロックオン時左移動をさせつつ前進させる）
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 自機をロックオン対象の左側に向ける(上記の角度から45度ずらす）
            Vector3 addrot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 10, transform.rotation.eulerAngles.z);
            // クォータニオンに変換
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            // 方向ベクトルを向けた方向に合わせる            
            m_MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
        }
        // 本体角度が0の場合カメラの方向に45度足した値をを移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = m_MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y = rotateOR.eulerAngles.y - 10;
            rotateOR = Quaternion.Euler(rotateOR_E);
            this.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
        }
        // それ以外は本体の角度+45度を移動方向にする
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y - 10;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            this.m_MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
        }
        // アニメーション速度
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        // m_AnimStateを変更する
        this.m_AnimState[0] = WrestleType;

        // ステートを変更
        m_AnimState[0] = WrestleType;
        // アニメーションを再生する
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)WrestleType]);
        // アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)WrestleType]].speed = speed;
        // 移動速度を調整する
        this.m_wrestlSpeed = movespeed; 
    }

    // 回り込み近接・右(相手の斜め前へ移動して回り込むタイプ）
    // WrestleType      [in]:格闘攻撃の種類
    // skilltype            [in]：スキルのインデックス/キャラごとに異なる
    protected virtual void WrestleDone_GoAround_Right(AnimationState WrestleType, int skilltype)
    {
        // 追加入力フラグをカット
        this.m_addInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の左側を移動方向とする（通常時のロックオン時左移動をさせつつ前進させる）
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 自機をロックオン対象の左側に向ける(上記の角度から45度ずらす）
            Vector3 addrot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 10, transform.rotation.eulerAngles.z);
            // クォータニオンに変換
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            // 方向ベクトルを向けた方向に合わせる            
            m_MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
        }
        // 本体角度が0の場合カメラの方向に45度足した値をを移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = m_MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y = rotateOR.eulerAngles.y + 10;
            rotateOR = Quaternion.Euler(rotateOR_E);
            this.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
        }
        // それ以外は本体の角度+45度を移動方向にする
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 10;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            this.m_MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
        }
        // アニメーション速度
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        // m_AnimStateを変更する
        this.m_AnimState[0] = WrestleType;

        // ステートを変更
        m_AnimState[0] = WrestleType;
        // アニメーションを再生する
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)WrestleType]);
        // アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)WrestleType]].speed = speed;
        // 移動速度を調整する
        this.m_wrestlSpeed = movespeed; 
    }

    // 後格闘（防御）
    // skilltype            [in]：スキルのインデックス/キャラごとに異なる
    protected virtual void GuardDone(int skilltype)
    {
        //1．追加入力フラグをカット
        this.m_addInput = false;
        int skillIndex = skilltype;
        //2．ロックオンしている場合→自機をロックオン対象に向ける
        // ロックオン且つ本体角度が0でない時
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを向けた方向に合わせる            
            m_MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
        }
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = m_MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y += 180;
            rotateOR = Quaternion.Euler(rotateOR_E);
            this.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
        }
        //   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 180;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            this.m_MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
        }
        //3．アニメーション速度を設定する
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        //4．AnimStateを変更する
        this.m_AnimState[0] = AnimationState.Back_Wrestle;
        //5．アニメーションを再生する
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Back_Wrestle]);
        //6．アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Back_Wrestle]].speed = speed;
        //7．移動速度を0にする
        this.m_wrestlSpeed = 0;
    }

    // 前特殊格闘
    // skilltype            [in]：スキルのインデックス/キャラごとに異なる
    protected virtual void WrestleDone_UpperEx(int skilltype)
    {
        // 追加入力フラグをカット
        this.m_addInput = false;
        // ステートを変更		
        int skillIndex = skilltype;	
	    // 移動速度取得
        float movespeed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Movespeed;
        // ロックオン中なら移動方向をロックオン対象のほうへ固定する
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを上向きにする            
            m_MoveDirection = new Vector3(0, 1, 0);
        }
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // 方向ベクトルを上向きにする            
            m_MoveDirection = new Vector3(0, 1, 0);
        }
        //   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 180;
            // 方向ベクトルを上向きにする            
            m_MoveDirection = new Vector3(0, 1, 0);
        }
        // アニメーション速度を設定する
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        // AnimStateを変更する
        this.m_AnimState[0] = AnimationState.EX_Front_Wrestle_1;
        // アニメーションを再生する
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.EX_Front_Wrestle_1]);
        // アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.EX_Front_Wrestle_1]].speed = speed;
        // 移動速度を上にする
        this.m_wrestlSpeed = movespeed;
    }

    // 後特殊格闘
    // skilltype            [in]：スキルのインデックス/キャラごとに異なる
    protected virtual void WrestleDone_DownEx(int skilltype)
    {
        // 追加入力フラグをカット
        this.m_addInput = false;
        // ステートを変更		
        int skillIndex = skilltype;
        // 移動速度取得
        float movespeed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Movespeed;
        // ロックオン中なら移動方向をロックオン対象のほうへ固定する
        if (m_IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを下向きにする            
            m_MoveDirection = new Vector3(0, -1, 0);
        }
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // 方向ベクトルを下向きにする            
            m_MoveDirection = new Vector3(0, -1, 0);
        }
        //   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 180;
            this.transform.rotation = Quaternion.Euler(new Vector3(addrot.x, addrot.y, addrot.z));
            // 方向ベクトルを下向きにする            
            m_MoveDirection = new Vector3(0, -1, 0);
        }
        // アニメーション速度を設定する
        float speed = Character_Spec.cs[(int)m_character_name][skillIndex].m_Animspeed;
        // AnimStateを変更する
        this.m_AnimState[0] = AnimationState.BACK_EX_Wrestle;
        // アニメーションを再生する
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.BACK_EX_Wrestle]);
        // アニメーションの速度を調整する
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.BACK_EX_Wrestle]].speed = speed;
        // 移動速度を設定する
        this.m_wrestlSpeed = movespeed;
    }

    // 格闘終了時に実行
    // Wrestletype      [in]:派生先の攻撃のアニメーション（持たせないときはIdleを指定すること）
    // NextWrestle      [in]:派生先の格闘攻撃の種類(引数2個はUnityが認識しない。Animationに関数を乗せる機能自体がSendMessageと同じ系列らしい）
    protected virtual void WrestleFinish(AnimationState wrestletype)
    {
        // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
        DestroyWrestle();
        // 格闘の累積時間を初期化
        m_wrestletime = 0;
        // 派生ありで入力を持っていたら、次のモーションを再生する
        if (m_addInput && wrestletype != AnimationState.Idle)
        {
            // スキルのインデックス
            CharacterSkill.SkillType skill = CharacterSkill.SkillType.NONE;
            switch (wrestletype)
            {
                // 射撃系の場合
                // 通常射撃
                case AnimationState.Shot:
                case AnimationState.Shot_AirDash:
                case AnimationState.Shot_run:
                case AnimationState.Shot_toponly:
                    if (Nowmode == ModeState.NORMAL)
                    {
                        skill = CharacterSkill.SkillType.SHOT;
                    }
                    else
                    {
                        skill = CharacterSkill.SkillType.SHOT_M2;
                    }
                    break;
                // サブ射撃
                case AnimationState.Sub_Shot:
                    if (Nowmode == ModeState.NORMAL)
                    {
                        skill = CharacterSkill.SkillType.SUB_SHOT;
                    }
                    else
                    {
                        skill = CharacterSkill.SkillType.SUB_SHOT_M2;
                    }
                    break;
                // 特殊射撃
                case AnimationState.EX_Shot:
                    if (Nowmode == ModeState.NORMAL)
                    {
                        skill = CharacterSkill.SkillType.EX_SHOT;
                    }
                    else
                    {
                        skill = CharacterSkill.SkillType.EX_SHOT_M2;
                    }
                    break;
                // 格闘系の場合(それ以外)
                // Wrestle1を0として、それからどれだけ差があるかで計算する
                default:
                    // とりあえずAnimationStateをintにする
                    int IndexOR = (int)wrestletype;
                    // それがWrestle1とどれだけ差がある？
                    IndexOR = IndexOR - (int)AnimationState.Wrestle_1;
                    int wrestle1;
                    // ノーマル
                    if (Nowmode == ModeState.NORMAL)
                    {                        
                        wrestle1 = (int)CharacterSkill.SkillType.WRESTLE_1;                        
                    }
                    // 別モード
                    else
                    {
                        wrestle1 = (int)CharacterSkill.SkillType.WRESTLE_1_M2;
                    }
                    skill = (CharacterSkill.SkillType)(wrestle1 + IndexOR);
                    break;
            }
            WrestleDone(wrestletype, (int)skill);
        }
        // 持っていなかったら、地上→Idleに戻る、空中→Fallに戻る（IdleとFallで追加入力フラグは強制カット）
        else
        {
            if (this.m_isGrounded)
            {
                m_AnimState[0] = AnimationState.Idle;
                this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
            }
            else
            {
                m_AnimState[0] = AnimationState.Fall;
                m_fallStartTime = Time.time;
                this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
            }
        }
    }


    // 格闘判定出現時に実行（一応派生は認めておく。専用のはそっちで利用）
    // Wrestletype      [in]:格闘攻撃の種類
    protected virtual void WrestleStart(WrestleType wrestletype)
    {
        // 判定を生成し・フックと一致させる  
        Vector3 pos = m_WrestleRoot[(int)wrestletype].transform.position;
        Quaternion rot = m_WrestleRoot[(int)wrestletype].transform.rotation;
        var obj = (GameObject)Instantiate(m_WrestleObject[(int)wrestletype], pos, rot);
        // 親子関係を再設定する(=判定をフックの子にする）
        if (obj.transform.parent == null)
        {
            obj.transform.parent = m_WrestleRoot[(int)wrestletype].transform;
            // 親子関係を付けておく
            obj.transform.GetComponent<Rigidbody>().isKinematic = true;
        }      

        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = 0;/*(int)CharacterSkill.SkillType.WRESTLE_1 + (int)wrestletype;*/
 
        // キャラごとに構成が異なるので、ここで処理分岐(入力が格闘でありながら、動作が格闘でない技を持つキャラが多くいる）
        switch (m_character_name)
        {
            case Character_Spec.CHARACTER_NAME.MEMBER_MADOKA:
                if (wrestletype == WrestleType.WRESTLE_1) skillIndex = 4;
                else if (wrestletype == WrestleType.WRESTLE_2) skillIndex = 5;
                else if (wrestletype == WrestleType.WRESTLE_3) skillIndex = 6;
                else if (wrestletype == WrestleType.FRONT_WRESTLE_1) skillIndex = 8;
                else if (wrestletype == WrestleType.LEFT_WRESTLE_1) skillIndex = 9;
                else if (wrestletype == WrestleType.RIGHT_WRESTLE_1) skillIndex = 10;
                else if (wrestletype == WrestleType.BACK_WRESTLE) skillIndex = 11;
                else if (wrestletype == WrestleType.AIRDASH_WRESTLE) skillIndex = 12;
                else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1) skillIndex = 14;
                else if (wrestletype == WrestleType.BACK_EX_WRESTLE) skillIndex = 15;
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_MAMI:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B:
                if (wrestletype == WrestleType.WRESTLE_1) skillIndex = 4;
                else if (wrestletype == WrestleType.WRESTLE_2) skillIndex = 5;
                else if (wrestletype == WrestleType.WRESTLE_3) skillIndex = 6;
                else if (wrestletype == WrestleType.FRONT_WRESTLE_1) skillIndex = 7;
                else if (wrestletype == WrestleType.LEFT_WRESTLE_1) skillIndex = 8;
                else if (wrestletype == WrestleType.RIGHT_WRESTLE_1) skillIndex = 9;
                else if (wrestletype == WrestleType.BACK_WRESTLE) skillIndex = 10;
                else if (wrestletype == WrestleType.AIRDASH_WRESTLE) skillIndex = 11;
                else if (wrestletype == WrestleType.EX_WRESTLE_1) skillIndex = 12;
                else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1) skillIndex = 13;
                else if (wrestletype == WrestleType.BACK_EX_WRESTLE) skillIndex = 14;
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_KYOKO:

                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_YUMA:
                if (wrestletype == WrestleType.WRESTLE_1) skillIndex = 3;
                else if (wrestletype == WrestleType.WRESTLE_2) skillIndex = 4;
                else if (wrestletype == WrestleType.WRESTLE_3) skillIndex = 5;
                else if (wrestletype == WrestleType.FRONT_WRESTLE_1) skillIndex = 6;
                else if (wrestletype == WrestleType.LEFT_WRESTLE_1) skillIndex = 7;
                else if (wrestletype == WrestleType.RIGHT_WRESTLE_1) skillIndex = 8;
                else if (wrestletype == WrestleType.BACK_WRESTLE) skillIndex = 9;
                else if (wrestletype == WrestleType.AIRDASH_WRESTLE) skillIndex = 10;
                else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1) skillIndex = 12;
                else if (wrestletype == WrestleType.BACK_EX_WRESTLE) skillIndex = 13;
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_ORIKO:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA:
                break;
            case Character_Spec.CHARACTER_NAME.MEMBER_SCHONO:
                if (wrestletype == WrestleType.WRESTLE_1) skillIndex = 3;
                else if (wrestletype == WrestleType.WRESTLE_2) skillIndex = 4;
                else if (wrestletype == WrestleType.WRESTLE_3) skillIndex = 5;
                else if (wrestletype == WrestleType.FRONT_WRESTLE_1) skillIndex = 6;
                else if (wrestletype == WrestleType.LEFT_WRESTLE_1) skillIndex = 7;
                else if (wrestletype == WrestleType.RIGHT_WRESTLE_1) skillIndex = 8;
                else if (wrestletype == WrestleType.BACK_WRESTLE) skillIndex = 9;
                else if (wrestletype == WrestleType.AIRDASH_WRESTLE) skillIndex = 10;
                else if (wrestletype == WrestleType.EX_WRESTLE_1) skillIndex = 11;
                else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1) skillIndex = 12;
                else if (wrestletype == WrestleType.BACK_EX_WRESTLE) skillIndex = 13;
                break;
            case Character_Spec.CHARACTER_NAME.ENEMY_MAJYU:
                 if(wrestletype == WrestleType.WRESTLE_1) skillIndex = 1; 
                 else if(wrestletype == WrestleType.WRESTLE_2) skillIndex = 2;
                 else if(wrestletype == WrestleType.WRESTLE_3) skillIndex = 3;
                 else if(wrestletype == WrestleType.FRONT_WRESTLE_1) skillIndex = 4;
                 else if(wrestletype == WrestleType.BACK_WRESTLE) skillIndex = 5;   
                 else if(wrestletype == WrestleType.AIRDASH_WRESTLE) skillIndex = 6;   
                 else if(wrestletype == WrestleType.EX_FRONT_WRESTLE_1) skillIndex = 7;
                 else if(wrestletype == WrestleType.BACK_EX_WRESTLE) skillIndex = 8;        
                break;
        }


       
        // 格闘判定を拾う
        var wrestleCollision = GetComponentInChildren<Wrestle_Core>();

        // 各ステートを計算する
        // 攻撃力
        int offensive = Character_Spec.cs[(int)m_character_name][skillIndex].m_OriginalStr + Character_Spec.cs[(int)m_character_name][skillIndex].m_GrowthCoefficientStr * (this.m_StrLevel - 1);
        // ダウン値
        float downR = Character_Spec.cs[(int)m_character_name][skillIndex].m_DownPoint;
        // 覚醒ゲージ増加量
        float arousal = Character_Spec.cs[(int)m_character_name][skillIndex].m_arousal + Character_Spec.cs[(int)m_character_name][skillIndex].m_GrowthCoefficientStr * (this.m_StrLevel - 1);
        // ヒットタイプ
        CharacterSkill.HitType hittype = Character_Spec.cs[(int)m_character_name][skillIndex].m_Hittype;
        // 打ち上げ量（とりあえず固定）

        // 格闘時に加算する力（固定）

        // 判定のセッティングを行う
        wrestleCollision.SetStatus(offensive, downR, arousal, hittype);     
  
        
    }

    // 後格闘（防御）判定出現時に実行
    protected virtual void GuardStart()
    {
        // 判定を生成し・フックと一致させる  
        Vector3 pos = m_WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform.position;
        Quaternion rot = m_WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform.rotation;
        var obj = (GameObject)Instantiate(m_WrestleObject[(int)WrestleType.BACK_WRESTLE], pos, rot);
        // 親子関係を再設定する(=判定をフックの子にする）
        if (obj.transform.parent == null)
        {
            obj.transform.parent = m_WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform;
            // 親子関係を付けておく
            obj.transform.GetComponent<Rigidbody>().isKinematic = true;
        }
        
    }

    // くっついている格闘オブジェクトをすべて消す
    protected void DestroyWrestle()
    {
        for (int i = 0; i < m_WrestleRoot.Length; i++)
        {            
            // あらかじめ子があるかチェックしないとGetChildを使うときはエラーになる
            if (this.m_WrestleRoot[i] != null && this.m_WrestleRoot[i].GetComponentInChildren<Wrestle_Core>() != null)
            {
                var wrestle = this.m_WrestleRoot[i].GetComponentInChildren<Wrestle_Core>();
               
                if (wrestle != null)
                {
                    Destroy(wrestle.gameObject);
                }
            }
        }
    }
       
}
