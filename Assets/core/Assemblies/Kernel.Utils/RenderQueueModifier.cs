using UnityEngine;

/// <summary>
/// 用于ngui
/// </summary>
public class RenderQueueModifier : MonoBehaviour
{
    public enum RenderType
    {
        FRONT,
        BACK
    }

    //public UIWidget m_target = null;
    public RenderType m_type = RenderType.FRONT;
    public bool isAccumule = false;//是否按顺序累加渲染层级


    Renderer[] _renderers;
    int _lastQueue = 0;

    void Start ()
    {
        GetRender();
    }

    void FixedUpdate ()
    {
      //  Debug.LogError(m_target);
        //if(m_target == null || m_target.drawCall == null)
        //    return;
        //int queue = m_target.drawCall.renderQueue;
        //queue += m_type == RenderType.FRONT ? 1 : -1;
        //if(_lastQueue != queue)
        //{
        //    _lastQueue = queue;

        //    foreach(Renderer r in _renderers)
        //    {
        //        r.material.renderQueue = _lastQueue;
        //        if (isAccumule)
        //        {
        //            _lastQueue++;
        //        }
                
        //    }
        //}
    }

    public void GetRender()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

}