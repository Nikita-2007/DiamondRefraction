using UnityEngine;

public class RayEmitter : MonoBehaviour
{
    public int rayCount = 1;

    public float sizeY = 2f;
    public float sizeZ = 2f;

    public Vector3 Direction
    {
        get
        {
            return transform.right;
        }
    }

    public Vector3 GetRayStart(int y, int z)
    {
        float fy =
            rayCount == 1
            ? 0.5f
            : (float)y / (rayCount - 1);

        float fz =
            rayCount == 1
            ? 0.5f
            : (float)z / (rayCount - 1);

        Vector3 local =
            new Vector3(
                0f,
                Mathf.Lerp(-sizeY / 2f, sizeY / 2f, fy),
                Mathf.Lerp(-sizeZ / 2f, sizeZ / 2f, fz)
            );

        return transform.TransformPoint(local);
    }
}