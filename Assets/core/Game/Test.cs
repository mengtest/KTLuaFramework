using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kernel.core;
using UnityEngine;

public struct BBB
{
	public float aaa;
	public float bbb;
}

public unsafe class MyPtr
{
	private BBB* vArray;

	public MyPtr(int count)
	{
		vArray = Create(count);
	}

	private BBB* Create(int count)
	{
		BBB* vArray = stackalloc BBB[count];
		return vArray;
	}

	public BBB* Value { get { return vArray; } }
}


public class Test : MonoBehaviour
{ 
	// Start is called before the first frame update
	void Start()
	{
		unsafe
		{
			Dictionary<int, MyPtr> dic = new Dictionary<int, MyPtr>();

			for (int i = 0; i < 90; ++i)
			{
				dic.Add(i, new MyPtr(1));
			}

			Stopwatch stopwatch2 = new Stopwatch();
			stopwatch2.Start(); //  开始监视代码运行时间

			var v = dic[45].Value->aaa;

			stopwatch2.Stop(); //  停止监视
			TimeSpan timespan2 = stopwatch2.Elapsed; //  获取当前实例测量得出的总时间
			UnityEngine.Debug.Log(">>>>>>>>>>time:" + timespan2.TotalMilliseconds.ToString());
		}


		//   var b1 = new BitSet(128);
		//   b1[5] = true;

		//System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
		//   stopwatch.Start(); //  开始监视代码运行时间
		//   b1 <<= 1;
		//stopwatch.Stop(); //  停止监视
		//				  //b1 ^= b2;
		//   TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
		//UnityEngine.Debug.Log(">>>>>>>>>>"+timespan.TotalMilliseconds.ToString());

		//   for (int i = 0; i < b1.Count; ++i)
		//   {
		//    UnityEngine.Debug.Log(b1[i]);
		//   }

		//AttributeProvider attr = new AttributeProvider();
		//var attributes = new Dictionary<int, double>();
		//for (int i = 0; i < 10; ++i)
		//{
		//	attributes.Add(i, i);
		//}

		//attr.Reset(attributes);
		//for (int i = 0; i < 10; ++i)
		//{

		//	attr.SetChangedVariableForce(i, i * 100);
		//}

		//Stopwatch stopwatch = new Stopwatch();
		//stopwatch.Start(); //  开始监视代码运行时间


		//var value = attr.GetAttributeVariable(5);


		//stopwatch.Stop(); //  停止监视
		//UnityEngine.Debug.Log(">>>>>>>>>:" + value.Value.ToString());

		//TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
		//UnityEngine.Debug.Log(">>>>>>>>>>time:" + timespan.TotalMilliseconds.ToString());

	}
    // Update is called once per frame
    void Update()
    {
        
    }
}