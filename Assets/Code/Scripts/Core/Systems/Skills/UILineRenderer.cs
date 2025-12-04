using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    [SerializeField] private Vector2[] points;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private bool useWorldSpace = false;

    [Header("Pulse Effect")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.7f;

    private float pulseTime = 0f;
    private Color originalColor;

    public Vector2[] Points
    {
        get => points;
        set
        {
            points = value;
            SetVerticesDirty();
            SetAllDirty();
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
        originalColor = color;

        if (enablePulse)
        {
            StartCoroutine(PulseRoutine());
        }
    }

    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(pulseTime) + 1f) / 2f);
            this.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            SetVerticesDirty();
            yield return null;
        }
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

            // Si estamos usando espacio mundial, convertir a local
            if (useWorldSpace)
            {
                start = transform.InverseTransformPoint(start);
                end = transform.InverseTransformPoint(end);
            }

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

        // Asegurar que las coordenadas están en espacio local correcto
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

    // Nuevo método para actualizar líneas en coordenadas mundiales
    public void UpdateLineWorldSpace(Vector3 worldFrom, Vector3 worldTo)
    {
        Vector2 localFrom = transform.InverseTransformPoint(worldFrom);
        Vector2 localTo = transform.InverseTransformPoint(worldTo);
        Points = new Vector2[] { localFrom, localTo };
    }
}