using UnityEngine;
using TMPro;

public class LifeLossFeedback : MonoBehaviour
{
    public float floatingSpeed = 2f;
    public float lifetime = 1.5f;
    private TextMeshPro textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
            textMesh.text = "-Life";
            textMesh.color = Color.red;
            textMesh.fontSize = 6;
            textMesh.alignment = TextAlignmentOptions.Center;
        }
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * floatingSpeed * Time.deltaTime, Space.World);
        
        // Optional: Fade out
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a -= Time.deltaTime / lifetime;
            textMesh.color = c;
        }
    }

    public static void Create(Vector3 position)
    {
        GameObject go = new GameObject("LifeLossFeedback");
        go.transform.position = position + Vector3.up * 1f;
        go.AddComponent<LifeLossFeedback>();
    }
}
