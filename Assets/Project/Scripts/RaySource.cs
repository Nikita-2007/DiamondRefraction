using UnityEngine;

public class RaySource : MonoBehaviour
{
    [Header("Ray Parameters")]
    public int rayCount = 10;          // не float, не будет автоматически
    public float sourceHeight = 2f;
    public float sourceWidth = 2f;
    public float beamLength = 10f;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(sourceWidth, sourceHeight, 0.05f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(sourceWidth, sourceHeight, 0.05f));
    }
}   