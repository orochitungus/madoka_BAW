using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// ディープコピーを行うためのクラス（ジェネリック版）
/// </summary>
static class DeepCopyUtils
{
    public static object DeepCopy(this object target)
    {

        object result;
        BinaryFormatter b = new BinaryFormatter();

        MemoryStream mem = new MemoryStream();

        try
        {
            b.Serialize(mem, target);
            mem.Position = 0;
            result = b.Deserialize(mem);
        }
        finally
        {
            mem.Close();
        }

        return result;

    }
}

/// <summary>
/// ディープコピーを行うためのクラス（拡張メソッド版）
/// </summary>
public static class DeepCopyHelper
{
    public static T DeepCopy<T>(T target)
    {

        T result;
        BinaryFormatter b = new BinaryFormatter();

        MemoryStream mem = new MemoryStream();

        try
        {
            b.Serialize(mem, target);
            mem.Position = 0;
            result = (T)b.Deserialize(mem);
        }
        finally
        {
            mem.Close();
        }

        return result;

    }
}