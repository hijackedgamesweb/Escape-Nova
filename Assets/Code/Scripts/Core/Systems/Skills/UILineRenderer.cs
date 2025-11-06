using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    [SerializeField] private Vector2[] points;
    [SerializeField] private float lineWidth = 2f;

    public Vector2[] Points
    {
        get => points;
        set
        {
            points = value;
            SetVerticesDirty();
        }
    }

    public float LineWidth
    {
        get => lineWidth;
        set
        {
            lineWidth = value;
            SetVerticesDirty();
        }
    }

    protected override void Start()
    {
        base.Start();
        canvasRenderer.SetMaterial(materialForRendering, null);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Length < 2)
            return;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];
            AddLineSegment(vh, start, end);
        }
    }

    private void AddLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * lineWidth / 2f;

        int vertexIndex = vh.currentVertCount;

        UIVertex vertex = new UIVertex();
        vertex.color = color;

        vertex.position = start - perpendicular;
        vh.AddVert(vertex);

        vertex.position = start + perpendicular;
        vh.AddVert(vertex);

        vertex.position = end + perpendicular;
        vh.AddVert(vertex);

        vertex.position = end - perpendicular;
        vh.AddVert(vertex);

        vh.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
        vh.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex);
    }

    public void UpdateLine(Vector2 from, Vector2 to)
    {
        Points = new Vector2[] { from, to };
    }
}