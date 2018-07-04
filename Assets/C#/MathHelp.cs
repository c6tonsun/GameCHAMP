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

    public static float FartherFromZero(float a, float b)
    {
        bool aNegative = a < 0;
        bool bNegative = b < 0;

        if (aNegative) a *= -1;
        if (bNegative) b *= -1;

        if (a > b)
        {
            if (aNegative) return -a;
            else return a;
        }
        else
        {
            if (bNegative) return -b;
            else return b;
        }
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

    public static Vector3 GetCurvePosition(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    public static Quaternion GetCurveRotation(Quaternion q0, Quaternion q1, Quaternion q2, float t)
    {
        return Quaternion.Lerp(Quaternion.Lerp(q0, q1, t), Quaternion.Lerp(q1, q2, t), t);
    }

    /// <summary>
    /// Takes two pairs of vector3 and calculates pairs direction.
    /// </summary>
    /// <param name="v0">First pairs start.</param>
    /// <param name="v1">First pairs end.</param>
    /// <param name="w0">Second pairs start.</param>
    /// <param name="w1">Second pairs end.</param>
    /// <returns>Angle between directions in degrees.</returns>
    public static float AngleBetweenVector3(Vector3 v0, Vector3 v1, Vector3 w0, Vector3 w1)
    {
        Vector3 v = v0 - v1;
        Vector3 w = w0 - w1;

        return Vector3.Angle(v, w);
    }

    /// <summary>
    /// Takes three vector3 and calculates direction.
    /// </summary>
    /// <param name="v0">First direction start.</param>
    /// <param name="v1">First direction end and second direction start.</param>
    /// <param name="v2">Second direction end.</param>
    /// <returns>Angle between directions in degrees.</returns>
    public static float AngleBetweenVector3(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 v = v0 - v1;
        Vector3 w = v1 - v2;

        return Vector3.Angle(v, w);
    }
}
