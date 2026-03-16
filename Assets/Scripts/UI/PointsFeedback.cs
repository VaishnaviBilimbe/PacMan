using UnityEngine;
using TMPro;

public class PointsFeedback : MonoBehaviour
{
    public float floatingSpeed = 1.5f;
    public float lifetime = 1f;
    private TextMeshPro textMesh;

    void Start()
    {
        // Initial setup happens in Create() via AddComponent
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * floatingSpeed * Time.deltaTime, Space.World);
        
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a -= Time.deltaTime / lifetime;
            textMesh.color = c;
        }
    }

    public static void Create(Vector3 position, string text, Color color)
    {
        GameObject go = new GameObject("PointsFeedback");
        go.transform.position = position + Vector3.up * 0.5f;
        
        PointsFeedback pf = go.AddComponent<PointsFeedback>();
        pf.textMesh = go.AddComponent<TextMeshPro>();
        pf.textMesh.text = text;
        pf.textMesh.color = color;
        pf.textMesh.fontSize = 5;
        pf.textMesh.alignment = TextAlignmentOptions.Center;
    }
}
