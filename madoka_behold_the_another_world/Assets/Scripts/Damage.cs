using UnityEngine;
using System.Collections;


// ダメージ関連の処理はこちらへ
public partial class CharacterControl_Base : MonoBehaviour 
{   
   
    private bool m_Explode;                 // 死亡エフェクトの存在
    private static int m_DamageLess = 2;    // この値×防御力分ダメージが減衰する


    // 被弾時HPを減少させる。SendMessageで弾丸などから呼ばれる
    // 第1引数：攻撃したキャラクター
    // 第2引数：与えるダメージ量
    public void DamageHP(int []arr)
    {
        int attackedcharacter = arr[0];
        int damage = arr[1];
        DamageHP(attackedcharacter, damage);
        
    }
    // 上記の引数2個指定バージョン
    public void DamageHP(int attackedcharacter, int damage)
    {
        // 防御力分ダメージを減衰する
        damage -= m_DefLevel * m_DamageLess;
        if (damage <= 0)
        {
            damage = 1;
        }
        this.NowHitpoint -= damage;
        // 死んだ場合、止めを刺したキャラに経験値が加算されるようにする（味方殺しでは経験値は増えない。また、まどかとアルティメットまどか、弓ほむらと銃ほむらは経験値とLVを共有する）
        if (IsPlayer == CHARACTERCODE.ENEMY && NowHitpoint < 1)
        {
            // 与える経験値の総量
            int addexp = Character_Spec.Exp[(int)m_character_name];
            if (savingparameter.GetNowHP(attackedcharacter) > 0)
            {
                savingparameter.AddExp(attackedcharacter, addexp);
                // 銃ほむらの場合、弓ほむらにも加算する
                if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
                {
                    savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B, addexp);
                }
                // 弓ほむらの場合、銃ほむらにも加算する
                else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
                {
                    savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA, addexp);
                }
                // まどかの場合、アルティメットまどかにも加算する
                else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
                {
                    savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA, addexp);
                }
                // アルティメットまどかの場合、まどかにも加算する
                else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
                {
                    savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA, addexp);
                }
            }
        }

        // PCにヒットさせた場合、savingparameterの値も変える
        if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
        {
            int charactername = (int)this.m_character_name;
            savingparameter.SetNowHP(charactername, NowHitpoint);
        }
    }

    // 被弾側の覚醒ゲージを増加させる。SendMessageで弾丸などから呼ばれる
    public void DamageArousal(float arousal)
    {
        m_Arousal += arousal;
        // PCにヒットさせた場合、savingparameterの値も変える
        if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
        {
            int charactername = (int)this.m_character_name;
            savingparameter.SetNowArousal(charactername, m_Arousal);
        }
    }


    // 被弾時ダウン値を加算させる
    public void DownRateInc(float downratio)
    {
        this.m_nowDownRatio += downratio;
    }
    // 被弾時ステートを変える 
    public virtual void DamageInit(AnimationState animationstate)
    {
        // くっついているエフェクトを消す
        BrokenEffect();    
        // くっついている格闘判定を消す
        DestroyWrestle();
        // 継承先で本体にくっついていたオブジェクトをカット
        // m_DownRebirthTimeのカウントを開始
        // 入力をポーズ以外すべて禁止
	    // ダメージアニメーションを再生
	    // 動作及び慣性をカット
	    // 飛び越えフラグをカット
	    // のけぞりならDamageInit→DamageDoneを呼ぶ
        // 吹き飛びならDamageInit→BlowDoneを呼ぶ

        // m_DownRebirthTimeのカウントを開始
        this.m_DownRebirthTime = Time.time;

        // （継承先で本体にくっついていたオブジェクトをカット）
	    // （UpdateCoreで入力をポーズ以外すべて禁止）
	    // ダメージアニメーションを再生
        //Debug.Log(m_AnimationNames[(int)AnimationState.Damage]);
        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Damage]);
        
        
	    // 動作及び慣性をカット
        this.m_MoveDirection = Vector3.zero;
	    // 飛び越えフラグをカット	
        this.m_Rotatehold = false;

        // 固定状態をカット
        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        

	    
	    // 死亡時は強制吹き飛び属性
        if (this.NowHitpoint < 1)
        {
            animationstate = AnimationState.BlowInit;
            // 属性がEnemyなら爆発エフェクトを貼り付ける
            if (this.IsPlayer == CHARACTERCODE.ENEMY)
            {
                // エフェクトをロードする
                Object Explosion = Resources.Load("Explosion_death");
                // 現在の自分の位置にエフェクトを置く
                var obj = (GameObject)Instantiate(Explosion, transform.position, transform.rotation);                      
                // 親子関係を再設定する
                obj.transform.parent = this.transform;                
                // 死亡爆発が起こったというフラグを立てる
                this.m_Explode = true;
            }
            animationstate = AnimationState.BlowInit;
            m_nowDownRatio = 5;
        }


        // 吹き飛び
        if(animationstate == AnimationState.BlowInit)
        {
            BlowDone();
        }
        // のけぞりならDamageInit→DamageDoneを呼ぶ
        else 
        {
            DamageDone();
        }

    }
    // のけぞりダメージ時、ダメージの処理を行う
    public virtual void DamageDone()
    {
        // 重力をカット
	    // ダメージ硬直の計算開始
	    // ステートをDamageに切り替える

        // 重力をカット
        this.GetComponent<Rigidbody>().useGravity = false;
        // ダメージ硬直の計算開始
        this.m_DamagedTime = Time.time;
        // ステートをDamageに切り替える
        this.m_AnimState[0] = AnimationState.Damage;
    }

    // 吹き飛びダメージ時、ダメージの処理を行う
    public virtual void BlowDone()
    {
        // Rotateの固定を解除        
	    // 重力を復活
	    // ステートをBlowへ切り替える
	    // ダウン値がMAXならステートをDownに切り替える
        // ダウンアニメを再生する
        // m_launchOffsetだけ浮かし、攻撃と同じベクトルを与える。ここの値はm_BlowDirectionに保存したものを使う
        
        
        // 重力を復活
        this.GetComponent<Rigidbody>().useGravity = true;
        // 固定していた場合、固定解除
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // 錐揉みダウン（ダウン値MAX）なら錐揉みダウンアニメを再生し、ステートをSpinDownへ切り替える
        if (this.m_nowDownRatio >= this.m_DownRatio)
        {
            // 錐揉みダウンアニメを再生する
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.SpinDown]);
            this.m_AnimState[0] = AnimationState.SpinDown;
        }
        // そうでないならステートをBlowに切り替える
        else
        {
            // Rotateの固定を解除        
            this.GetComponent<Rigidbody>().freezeRotation = false;
            // ダウンアニメを再生する
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Down]);
            this.m_AnimState[0] = AnimationState.Blow;
        }    
        // 攻撃と同じベクトルを与える。ここの値はm_BlowDirectionに保存したものを使う
        this.GetComponent<Rigidbody>().AddForce(this.m_BlowDirection.x*10, 10, this.m_BlowDirection.z*10);       
    }

    // ダメージ(のけぞり）
    public virtual void Damage()
    {
        // ダメージ硬直終了
        if (Time.time > this.m_DamagedTime + this.m_DamagedWaitTime)
        {      
            // 空中にいた→ダウンアニメを再生する→Blowへ移行（飛ばされない）
            if (!this.m_isGrounded)
            {
                // Rotateの固定を解除        
                this.GetComponent<Rigidbody>().freezeRotation = false;
                // ダウンアニメを再生する
                this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Down]);
                // 重力を復活
                this.GetComponent<Rigidbody>().useGravity = true;                
                // Blowへ移行
                this.m_AnimState[0] = AnimationState.Blow;
            }
            // 地上にいた→Idleへ移行し、Rotateをすべて0にして固定する
            else
            {
                this.GetComponent<Rigidbody>().useGravity = true;
                this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
                this.m_AnimState[0] = AnimationState.Idle;
            }
        }
    }

    // 吹き飛び
    protected virtual void Blow()
    {
        // 接地までなにもせず、接地したらDownへ移行し、m_DownTimeを計算する
	    // ブースト入力があった場合、ダウン値がMAX未満でブーストゲージが一定量あれば、Reversalへ変更	
        // ブースト量を減らす
        // rotationを0にして復帰アニメを再生する
        // 再固定する
        // ステートを復帰にする

        // 接地までなにもせず、接地したらDownへ移行し、m_DownTimeを計算する
        if (this.m_isGrounded)
        {
            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Down]);
            // downへ移行
            this.m_AnimState[0] = AnimationState.Down;
            this.m_DownTime = Time.time;
            // 速度を0まで落とす（吹き飛び時のベクトルを消す）
            this.m_MoveDirection = Vector3.zero;
            // 回転を戻す
            // rotationの固定を復活させ、0,0,0にする
            this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
        }
        // ブースト入力があった場合、ダウン値がMAX未満でブーストゲージが一定量あれば、Reversalへ変更	
        // rotationを0にして復帰アニメを再生する
        else if (this.m_nowDownRatio <= this.m_DownRatio &&  this.m_hasJumpInput && this.m_Boost >= this.m_ReversalUseBoost)
        {
            // ブースト量を減らす
            this.m_Boost -= this.m_ReversalUseBoost;
            // 復帰処理を行う
            ReversalInit();
        }
    }

    // 錐揉みダウン
    protected virtual void SpinDown()
    {      
        // 落下に入ったら落下速度を調整する
        m_MoveDirection.y = MadokaDefine.FALLSPEED;
        
        // 基本Blowと同じだが、着地と同時にアニメをダウンに切り替える
        if (this.m_isGrounded)
        {
            // ダウンアニメを再生
            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Down]);
            // downへ移行
            this.m_AnimState[0] = AnimationState.Down;
            this.m_DownTime = Time.time;
            // 速度を0まで落とす（吹き飛び時のベクトルを消す）
            this.m_MoveDirection = Vector3.zero;
            // 回転を戻す
            // rotationの固定を復活させ、0,0,0にする
            this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
        }
    }



    // ダウン
    protected virtual void Down()
    {
        // rotationの固定を復活させ、0,0,0にする
        this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);        
        this.GetComponent<Rigidbody>().freezeRotation = true;

	    // m_DownTimeが規定値を超えると、復帰アニメを再生する
        if (Time.time > this.m_DownTime + this.m_DownWaitTime)
        {
            // ただし自機側ではHP0だと復活させない
            if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
            {
                if (NowHitpoint < 1)
                {
                    return;
                }
            }
            ReversalInit();
        }
    }

    // ダウン復帰
    protected virtual void Reversal()
    {
        
        
    }

    // ダウン復帰後処理（ダウン復帰アニメの最終フレームに実装）
    protected virtual void ReversalComplete()
    {
        // 復帰アニメが終わると、Idleにする
        // ダウン値を0に戻す
        this.m_nowDownRatio = 0.0f;
        // m_DownRebirthTimeを0にする
        this.m_DownRebirthTime = 0;
        // ステートをIdleに戻す
        this.m_AnimState[0] = AnimationState.Idle;
        // Idleのアニメを再生する
        this.GetComponent<Animation>().Play(this.m_AnimationNames[(int)AnimationState.Idle]);
        // m_DownTimeを0にする
        this.m_DownTime = 0;
    }

    // 復帰処理
    protected void ReversalInit()
    {
        // rotationの固定を復活させ、0,0,0にする
        this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
        // 再固定する
        this.GetComponent<Rigidbody>().freezeRotation = true;
        // ステートを復帰にする
        this.m_AnimState[0] = AnimationState.Reversal;
        // 復帰アニメを再生する
        this.GetComponent<Animation>().Play(this.m_AnimationNames[(int)AnimationState.Reversal]);
    }
}