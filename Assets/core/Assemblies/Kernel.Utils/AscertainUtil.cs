using System;
using System.Collections.Generic;

public static class AscertainUtil {

    #region 王冲添加 容错解析Dictionary<string, object>类型
    public static int TryGetInt (this Dictionary<string, object> dict, string key, int defaultValue = default(int))
    {
        int value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!int.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
            }
        }
        return value;
    }
    public static Byte TryGetByte(this Dictionary<string, object> dict, string key, Byte defaultValue = default(Byte))
    {
        Byte value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!Byte.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
            }
        }
        return value;
    }
    public static float TryGetFloat(this Dictionary<string, object> dict, string key, float defaultValue = default(float))
    {
        float value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!float.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
            }
        }
        return value;
    }

    public static long TryGetLong(this Dictionary<string, object> dict, string key, long defaultValue = default(long))
    {
        long value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!long.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
            }
        }
        return value;
    }
    public static string TryGetString(this Dictionary<string, object> dict, string key, string defaultValue = "")
    {
        string value = defaultValue;
        object obj = null;
        if (dict != null && dict.TryGetValue(key,out obj))
        {
            value = obj.ToString();
        }
        return value;
    }
    #endregion
}
