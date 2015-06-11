using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Scene_Source_maker_madoka
{
    class Program
    {
        // 最大表示行数
        public const int max_Line = 4;       

        // 1行当たりの文字数
        public const int max_Words = 30;

        // 元データが記載されているファイル
        public const string origin_file = "original.txt";

        // 出力結果を保存するファイル
        public const string result_file = "result.txt";
        
        // 名前データを格納したクラス
        static public Names names = null;   // 共有させるならstaticが必須

        static void Main(string[] args)
        {
            names = new Names();

            // ファイルを読み込む（こっちはshift-jisで構わない）
            string[] lines1 = File.ReadAllLines(origin_file, System.Text.Encoding.GetEncoding("Shift_JIS"));

            // 取得内容によって変える
            for (int a = 0; a < lines1.Length; a++)
            {
                string OR_string = lines1[a];
                // 長さを取得する
                int word_le = OR_string.Length;
                // 何もない行は読まない
                if (OR_string != "")
                {
                    // case を書き込む
                    File.AppendAllText(result_file, "case " + a + ":\r\n", System.Text.Encoding.GetEncoding("UTF-8"));

                    // :までを切り出す 
                    int iFind1 = OR_string.IndexOf('：');

                    // :以前を名前とする
                    string name = OR_string.Substring(0, iFind1);
                    // ここで：前の名前が誰であったかによって処理分岐
                    drawserif(a, name, OR_string, word_le, iFind1);
                          
                }
                // breakを書き込む
                File.AppendAllText(result_file, "break;\r\n", System.Text.Encoding.GetEncoding("Shift_JIS"));
            }
            Console.WriteLine("COMPLETE!!");
            Console.ReadLine();
        }

        // 名前をもとにセリフを書く
        // 共有させるならstaticが必須
        // 第1引数：ループ番号
        // 第2引数：名前
        // 第3引数：読み取った文字列
        // 第4引数：文字列の長さ
        // 第5引数：コロン前までの長さ
        static void drawserif(int a, string name, string OR_string, int word_le, int iFind1)
        {
            //for (int i = 0; i < names.CharacterNameList.GetLength(0); i++)
            {
                // 名前が登録されている誰であるかを取得する（いなければ0を返す）
                int i = CheckName(name);
                // ＊（全角アスタ）の場合xstory強制追加かつKeybreak=trueの空caseを入れる
                if (name == "＊")
                {
                    File.AppendAllText(result_file, "keybreak = true;\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    File.AppendAllText(result_file, "FullClear();\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    File.AppendAllText(result_file, "IncrementXstory();\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    return;
                }
                // ＊＊（全角アスタ2つ）の場合xstoryを強制追加しない空caseを入れる
                else if (name == "＊＊")
                {
                    File.AppendAllText(result_file, "keybreak = true;\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    File.AppendAllText(result_file, "FullClear();\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    return;
                }
                // ＊＊＊（全角アスタ3つ）の場合名前を出さないcaseを入れる
                else if (name == "＊＊＊")
                {
                    File.AppendAllText(result_file, "keybreak = false;\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    File.AppendAllText(result_file, "FullClear();\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    return;
                }
                // それ以外の場合表記された名前での台詞を出すcaseを入れる（その場合英文はなし）
                else if (name != "")
                {
                    File.AppendAllText(result_file, "keybreak = false;\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    // Namesに定義されている名前だった場合、本名と英文表記を追加して台詞を出すcaseを入れる                           
                    // 定義あり
                    if (i != 0)
                    {
                        // 顔
                        File.AppendAllText(result_file, "m_facetype = " +  names.CharacterNameList[i][3] + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        // 名前日本語
                        File.AppendAllText(result_file, "m_drawname_jp = " + "\"" + names.CharacterNameList[i][1] + "\"" + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        // 名前英語
                        File.AppendAllText(result_file, "m_drawname_en = " + "\"" + names.CharacterNameList[i][2] + "\"" + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    }
                    // 定義なし
                    else
                    {
                        // 顔
                        File.AppendAllText(result_file, "m_facetype = " + "0" + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        // 名前日本語
                        File.AppendAllText(result_file, "m_drawname_jp = \"" + name + "\"" + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        // 名前英語
                        File.AppendAllText(result_file, "m_drawname_en = \"" + "\"" + ";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                    }
                    // セリフ
                    string[] word = new string[max_Line];


                    if (word_le - iFind1 < 31)
                    {
                        word[0] = OR_string.Substring(iFind1 + 1);
                    }
                    else
                    {
                        word[0] = OR_string.Substring(iFind1 + 1, 30);
                    }
                    File.AppendAllText(result_file, "m_serif[0] = \"" + word[0] + "\";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));

                    // 2行以上にわたる場合
                    for (int j = 1; j < max_Line; j++)
                    {
                        if (word_le - iFind1 > 30 * j)
                        {
                            if (word_le - iFind1 < 31 + 31 * j)
                            {
                                word[j] = OR_string.Substring(iFind1 + 31 + 30 * (j - 1));
                            }
                            else
                            {
                                word[j] = OR_string.Substring(iFind1 + 31 + 30 * (j - 1), 30);
                            }
                            File.AppendAllText(result_file, "m_serif[" + j + "] = \"" + word[j] + "\";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        }
                        else
                        {
                            File.AppendAllText(result_file, "m_serif[" + j + "] = \""  + "\";\r\n", System.Text.Encoding.GetEncoding("UTF-8"));
                        }
                    }
                    return;
                }
            }              
        }


        // 該当する名前をチェックする
        // 第1引数：元データにおけるキャラクターの名前
        static int CheckName(string inputname)
        {
            int name_Result = 0;

            // 全員分ループを回し、見つかったインデックスを返す
            for (int i = 0; i < names.CharacterNameList.GetLength(0); i++)
            {
                if (names.CharacterNameList[i][0] == inputname)
                {
                    return i;
                }
            }

            return name_Result;
        }
        
    }
}
