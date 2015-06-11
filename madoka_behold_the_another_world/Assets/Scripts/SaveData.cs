using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// savingparameterはそのままセーブできないので、これに一旦入れてから保存する
// ロード時はこのクラスのインスタンスに読み込み、savingparameterに書き込む
[Serializable()]                // セーブ可能にするため、[Serializable()]属性を付加
public class SaveData
{
    // ストーリー進行度
    public int story;
    // キャラクターの配置位置
    public float nowposition_x;
    public float nowposition_y;
    public float nowposition_z;
    // キャラクターの配置角度
    public float nowrotation_x;
    public float nowrotation_y;
    public float nowrotation_z;
    // アイテムボックスの開閉フラグ
    public bool[] itemboxopen = new bool[MadokaDefine.NUMOFITEMBOX];
    // 現在の所持金
    public int nowmoney;
    // 現在のパーティー(護衛対象も一応パーティーに含める)
    public int[] nowparty = new int[4];
    // 各キャラのレベル
    public int[] nowlevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの現在の経験値
    public int[] nowExp = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラのHP    
    public int[] nowHP = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの最大HP
    public int[] nowMaxHP = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの覚醒ゲージ
    public float[] nowArousal = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの最大覚醒ゲージ
    public float[] nowMaxArousal = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの攻撃力レベル
    public int[] StrLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各キャラの防御力レベル
    public int[] DefLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 残弾数レベル
    public int[] BulLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // ブースト量レベル
    public int[] BoostLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 覚醒ゲージレベル
    public int[] ArousalLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // ソウルジェム汚染率
    public float[] GemContimination = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // スキルポイント
    public int[] SkillPoint = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 各アイテムの保持数
    public int[] m_numofItem = new int[Item.itemspec.Length];
    // 現在装備中のアイテム(itemspecのインデックスと同じ。装備していないときは-1になる）
    public int m_nowequipItem;
    // 現在の場所（Kanonと同じ形式、マップ.xlsxを参照）
    public int nowField;
    // 前にいた場所（Kanonと同じ形式、マップ.xlsxを参照）
    public int beforeField;
}
