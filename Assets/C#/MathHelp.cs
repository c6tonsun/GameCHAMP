using UnityEngine;

public static class MathHelp {
    
    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }

    public static Vector3 MultiplyVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static float AbsBiggest(Vector3 v3, bool ignoreY)
    {
        float biggest = 0f;

        if (ignoreY) v3.y = 0f;

        if (v3.x < 0) v3.x *= -1;
        if (v3.y < 0) v3.y *= -1;
        if (v3.z < 0) v3.z *= -1;

        if (v3.x > biggest) biggest = v3.x;
        if (v3.y > biggest) biggest = v3.y;
        if (v3.z > biggest) biggest = v3.z;

        return biggest;
    }
    
    public static Vector3[] CapsuleEndPoints(CapsuleCollider capCol, out float radius)
    {
        radius = AbsBiggest(capCol.transform.localScale, true) * 0.5f;
        float scaleY = capCol.transform.localScale.y;
        Vector3 center = capCol.transform.position + capCol.center;
        Vector3 offset = capCol.transform.up * capCol.height * 0.5f * scaleY;
        Vector3 offsetFix = capCol.transform.up * radius;

        if (scaleY < 0)
        {
            offset += offsetFix;
            scaleY *= -1;
        }
        else
        {
            offset -= offsetFix;
        }

        if (radius * 2 > scaleY * capCol.height)
            return new Vector3[] { center };
        else
            return new Vector3[] { center + offset, center - offset };
    }
}
