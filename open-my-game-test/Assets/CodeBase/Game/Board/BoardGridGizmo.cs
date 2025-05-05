#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BoardGridGizmo : MonoBehaviour
{
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;
    [SerializeField] private float cell = 1f;
    [SerializeField] private Vector2 origin = new(-3, -3);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int x = 0; x <= width; x++)
        {
            Vector3 a = transform.TransformPoint(new Vector3(origin.x + x * cell, origin.y, 0));
            Vector3 b = transform.TransformPoint(new Vector3(origin.x + x * cell, origin.y + height * cell, 0));
            Gizmos.DrawLine(a, b);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 a = transform.TransformPoint(new Vector3(origin.x, origin.y + y * cell, 0));
            Vector3 b = transform.TransformPoint(new Vector3(origin.x + width * cell, origin.y + y * cell, 0));
            Gizmos.DrawLine(a, b);
        }
    }
#endif
}