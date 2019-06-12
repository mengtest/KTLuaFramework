using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 类主要功能：重复和延迟执行方法
/// 注意事项：
/// 创建日期：2015.9.16
/// 最后修改日期：2015.9.19
/// 最后修改人（包括创建人）：lijunfeng
/// </summary>
public sealed class GlobalCoroutine : MonoBehaviour,IRelease
{
    private class Invoker
    {
        public Action func;
        public float delay;
        public float delta;
        public float currDelta;
    }

    static private GlobalCoroutine _instance = null;

    private Dictionary<int, Invoker> m_dic = new Dictionary<int, Invoker>();
    private Action m_delayTodo = null;


    static public GlobalCoroutine Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject o = new GameObject("GlobalCoroutine");
                _instance = o.AddComponent<GlobalCoroutine>();
                DontDestroyOnLoad(o);
            }

            return _instance;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func"></param>
    /// <param name="delta">间隔</param>
    /// <param name="delay"></param>
    public void InvokeRepeating(Action func,float delta, float delay = 0)
    {
        if (m_dic.ContainsKey(func.GetHashCode()))
            return;

        Invoker t_invoker = new Invoker();
        t_invoker.func = func;
        t_invoker.delay = delay;
        t_invoker.delta = delta;
        t_invoker.currDelta = delta;
        m_dic.Add(func.GetHashCode(), t_invoker);
    }

    public void CancelInvoke(Action func)
    {
        Invoker t_invoker;

        if (m_dic.TryGetValue(func.GetHashCode(),out t_invoker))
        {
            t_invoker.func = null;
            m_dic.Remove(func.GetHashCode());
        }
    }

    /// <summary>
    /// 延迟做某事
    /// </summary>
    /// <param name="delay"></param>
    public void DoSomethingDelay(float delay,Action delayTodo)
    {
        if (delay > 0)
        {
            m_delayTodo = delayTodo;
            StartCoroutine(CD(delay));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    private IEnumerator CD(float second)
    {
        yield return new WaitForSeconds(second);

        if (m_delayTodo != null)
            m_delayTodo();
    }

    private void FixedUpdate()
    {
        foreach(var item in m_dic)
        {
            if(item.Value.delay<=0)
            {
                if(item.Value.currDelta<=0)
                {
                    item.Value.currDelta += item.Value.delta;
                    item.Value.func();
                    continue;
                }

                item.Value.currDelta -= Time.fixedDeltaTime;
                continue;
            }

            item.Value.delay -= Time.fixedDeltaTime;
        }   
    }

    public void Release(bool destroy = false)
    {
        m_dic.Clear();
        m_delayTodo = null;

        if (destroy)
            _instance = null;
    }
}
