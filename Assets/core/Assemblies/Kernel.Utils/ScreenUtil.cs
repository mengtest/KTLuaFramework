using UnityEngine;


public class ScreenUtil
{
    private static float m_width;
    private static float m_height;
    private static float m_scaleWidth;
    private static float m_scaleHeight;

    public static void InitScreenResolution(float width, float height)
    {
        m_width = width;
        m_height = height;

        m_scaleWidth = Screen.width/ m_width;
        m_scaleHeight = Screen.height/ m_height;
    }

    public static Vector2 Scale
    {
        get
        {
            return new Vector2(m_scaleWidth, m_scaleHeight);
        }
    }

    public static float ScaleWidth
    {
        get
        {
            return m_scaleWidth;
        }
    }

    public static float ScaleHeight
    {
        get
        {
            return m_scaleHeight;
        }
    }

    public static Vector3 MousePosition
    {
        get
        {
            return new Vector3(Input.mousePosition.x / m_scaleWidth, Input.mousePosition.y / m_scaleHeight, 0);
        }
    }
}
