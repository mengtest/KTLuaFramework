using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kernel.core;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    var b1 = new BitSet(128);
	    b1[5] = true;

		System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
	    stopwatch.Start(); //  开始监视代码运行时间
	    b1 <<= 1;
		stopwatch.Stop(); //  停止监视
						  //b1 ^= b2;
	    TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
		UnityEngine.Debug.Log(">>>>>>>>>>"+timespan.TotalMilliseconds.ToString());

	    for (int i = 0; i < b1.Count; ++i)
	    {
		    UnityEngine.Debug.Log(b1[i]);
	    }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
