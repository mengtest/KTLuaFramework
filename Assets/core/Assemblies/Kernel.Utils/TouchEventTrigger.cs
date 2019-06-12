/*********************
 * 姓名：王冲
 * 功能：集合所有手指事件。 按下、松开、长按、双击、拖动……
 * 日期：2017/8/7
**********************/

using UnityEngine;
using System.Collections;
using System;

public class TouchEventTrigger : MonoBehaviour {

    public Action onPressDownEvent;
    public Action onPressUpEvent;
    public Action onLongPressEvent;

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void OnPress(bool isPress)
    {
        if (isPress)
        {
            if (onPressDownEvent != null)
                onPressDownEvent();
            if(gameObject.activeSelf && onLongPressEvent != null)
                StartCoroutine("LongPress");
        }
        else
        {
            StopCoroutine("LongPress");
            if (onPressUpEvent != null)
                onPressUpEvent();
        }
    }

    IEnumerator LongPress()
    {
        yield return new WaitForSeconds(1);
        if (onLongPressEvent != null)
            onLongPressEvent();
    }


}
