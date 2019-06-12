using System;
using System.Collections.Generic;
using UnityEngine;

namespace  LuaFramework
{
	public class CoordinateUtil
{
    /// <summary>
    /// 检查target是否在actor前面
    /// </summary>
    /// <returns></returns>
    public static bool CheckInFront(Transform actor, Transform target)
    {
        Vector3 rhs = target.position - actor.position;

        return Vector3.Dot(actor.forward, rhs) > 0;
    }

    /// <summary>
    /// 检查一个target是否在center面前夹角内
    /// </summary>
    /// <param name="targetPos">目标点</param>
    /// <param name="centerPos">中心点</param>
    /// <param name="centerForward">中心点方向</param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static bool CheckInAngle(Vector3 targetPos, Vector3 centerPos,Vector3 centerForward ,float angle)
    {
        Vector3 targetDir = centerForward;
        Vector3 dir = Vector3.Normalize(targetPos - centerPos);
        float dot = Vector3.Dot(targetDir, dir);
        return dot > FixedMathf.Cos(angle);
    }

    /// <summary>
    /// 三角形区域
    /// </summary>
    /// <param name="v0x"></param>
    /// <param name="v0y"></param>
    /// <param name="v1x"></param>
    /// <param name="v1y"></param>
    /// <param name="v2x"></param>
    /// <param name="v2y"></param>
    /// <returns></returns>
    public static float triangleArea(float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
    {
        return UnityEngine.Mathf.Abs((v0x * v1y + v1x * v2y + v2x * v0y - v1x * v0y - v2x * v1y - v0x * v2y) / 2f);
    }

    /// <summary>
    /// 是否在三角形内
    /// </summary>
    /// <param name="point"></param>
    /// <param name="v0"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool isINTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float x = point.x;
        float y = point.z;
        float v0x = v0.x;
        float v0y = v0.z;
        float v1x = v1.x;
        float v1y = v1.z;
        float v2x = v2.x;
        float v2y = v2.z;
        float t = triangleArea(v0x, v0y, v1x, v1y, v2x, v2y);
        float a = triangleArea(v0x, v0y, v1x, v1y, x, y) + triangleArea(v0x, v0y, x, y, v2x, v2y) + triangleArea(x, y, v1x, v1y, v2x, v2y);

        if (FixedMathf.Abs(t - a) <= 0.01f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 检测目标前方一定距离的一点周围
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="distance"></param>
    /// <param name="radius"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static List<T> DetachRoundAreaAt<T>(Transform transform, float distance, float radius, int layerMask = 0, Predicate<T> condition=null)
    {
        Vector3 f0 = (transform.position + Vector3.forward * distance);

        Collider[] colliders = Physics.OverlapSphere(f0, radius, layerMask);
        List<T> t_result = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            T t_actor = colliders[i].GetComponent<T>();

            if(condition!=null&& condition(t_actor))
                t_result.Add(t_actor);
        }

        return t_result;
    }

    /// <summary>
    /// 检测目标正前方一定角度范围内的扇形区域
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="angle"></param>
    /// <param name="radius"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static List<T> DetachSectorAreaTowardDir<T>(Transform transform, int angle, float radius, int layerMask = 0, IComparer<Collider> sortMethod = null, Predicate<T> condition = null)
    {
        Quaternion r = transform.rotation;
        Vector3 f0 = (transform.position + (r * Vector3.forward) * radius);
        Debug.DrawLine(transform.position, f0, Color.red);
        Quaternion r0 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - angle * 0.5f, transform.rotation.eulerAngles.z);
        Quaternion r1 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + angle * 0.5f, transform.rotation.eulerAngles.z);
        Vector3 f1 = (transform.position + (r0 * Vector3.forward) * radius);
        Vector3 f2 = (transform.position + (r1 * Vector3.forward) * radius);
        Debug.DrawLine(transform.position, f1, Color.red);
        Debug.DrawLine(transform.position, f2, Color.red);
        Debug.DrawLine(f1, f2, Color.red);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (sortMethod != null)
            System.Array.Sort(colliders, sortMethod);

        List<T> t_result = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            T t_actor = colliders[i].GetComponent<T>();

            //帅选条件
            if (isINTriangle(colliders[i].transform.position, transform.position, f1, f2))
            {
                if (condition != null && condition(t_actor))
                    t_result.Add(t_actor);
            }
        }

        return t_result;
    }
}

}

