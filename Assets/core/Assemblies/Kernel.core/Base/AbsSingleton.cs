using UnityEngine;

/// <summary>
/// 抽象单例
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AbsSingleton<T> where T : AbsSingleton<T>, new()
{ 
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }
}
