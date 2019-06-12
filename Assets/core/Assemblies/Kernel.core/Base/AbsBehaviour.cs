using UnityEngine;

public abstract class AbsBehaviour<T>:MonoBehaviour where T :AbsBehaviour<T>
{ 
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this as T;
    }
}
