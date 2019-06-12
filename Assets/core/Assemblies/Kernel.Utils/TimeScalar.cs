using UnityEngine;
using System.Collections;

namespace LuaFramework
{
	public class TimeScalar : MonoBehaviour 
{
	static private GameObject inst = null;
	static public TimeScalar Instance
	{
		get
		{
			if( inst == null )
			{
				inst = new GameObject("TimeScalar");
				inst.AddComponent<TimeScalar>();
			}
			return inst.GetComponent<TimeScalar>();
		}
	}
	
	
	int zeroScalarRef;
	float currScalar;

	public float currScalarValue
	{
		get
		{
			if( zeroScalarRef > 0 )
				return 0.0f;
			
			return currScalar;
		}
	}
	
	public void SetTimeScalar( float scale )
	{
		if( scale > UnityEngine.Mathf.Epsilon )
		{
			currScalar *= scale;
		}
		else
		{
			++zeroScalarRef;
		}

		Time.timeScale = currScalarValue;
	}
	
	public void RecoverTimeScalar( float scale )
	{
		if( scale > UnityEngine.Mathf.Epsilon )
		{
			currScalar /= scale;
		}
		else if( zeroScalarRef > 0 )
		{
			-- zeroScalarRef;
		}
	}
	
	
	void Awake()
	{
		zeroScalarRef = 0;
		currScalar = 1.0f;
	}
	
	void OnDestroy()
	{
		inst = null;
		Time.timeScale = 1.0f;
	}
	
	void Update()
	{
		Time.timeScale = currScalarValue;
	}
	
	

}

}

//Global time scalar used to handling Time.timeScale. Auto recover timer when scene loading.

