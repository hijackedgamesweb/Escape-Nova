using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class OrbitCircleUI : Graphic
{
    [SerializeField] private float thickness = 2f;
    [SerializeField] private int segments = 128;
    [SerializeField] private float radius = 100f;

    public void SetRadius(float r) => radius = r;
    public void SetThickness(float t) => thickness = t;
    

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float angleStep = 360f / segments;
        float outerRadius = radius;
        float innerRadius = radius - thickness;

        Vector2 prevOuter = Vector2.zero;
        Vector2 prevInner = Vector2.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector2 outer = new Vector2(cos * outerRadius, sin * outerRadius);
            Vector2 inner = new Vector2(cos * innerRadius, sin * innerRadius);

            if (i > 0)
            {
                int idx = vh.currentVertCount;

                vh.AddVert(prevInner, color, prevInner);
                vh.AddVert(inner, color, inner);
                vh.AddVert(outer, color, outer);
                vh.AddVert(prevOuter, color, prevOuter);

                vh.AddTriangle(idx, idx + 1, idx + 2);
                vh.AddTriangle(idx, idx + 2, idx + 3);
            }

            prevInner = inner;
            prevOuter = outer;
        }
    }
}