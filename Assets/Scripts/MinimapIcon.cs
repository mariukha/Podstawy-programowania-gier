using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MinimapIcon : MonoBehaviour
{
    public enum Shape { Circle, Square, Arrow }

    public Shape shape = Shape.Circle;
    public Color color = Color.yellow;
    public string legend = "Obiekt";
    public float size = 1f;
    public bool rotateWithObject;
    public int order;
    
    public static readonly List<MinimapIcon> All = new List<MinimapIcon>();

    public Texture2D Texture { get; private set; }

    Transform icon;
    Renderer rend;

    void Awake()
    {
        Texture = MakeTexture(shape);

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name + "_Ikona";
        Destroy(quad.GetComponent<Collider>());

        rend = quad.GetComponent<Renderer>();
        rend.shadowCastingMode = ShadowCastingMode.Off;
        rend.receiveShadows = false;
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = Texture;
        material.color = color;
        rend.material = material;
        rend.enabled = false;

        icon = quad.transform;
        icon.localScale = Vector3.one * size;
    }

    void OnEnable()
    {
        All.Add(this);
        if (icon != null) icon.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        All.Remove(this);
        if (icon != null) icon.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (icon != null) Destroy(icon.gameObject);
    }

    void LateUpdate()
    {
        Vector3 p = transform.position;
        icon.position = new Vector3(p.x, 20f + order * 0.1f, p.z);
        float yaw = rotateWithObject ? transform.eulerAngles.y : 0f;
        icon.rotation = Quaternion.Euler(90f, yaw, 0f);
    }

    public void SetVisible(bool visible)
    {
        if (rend != null) rend.enabled = visible;
    }

    static Texture2D MakeTexture(Shape shape)
    {
        const int n = 64;
        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            float u = (x + 0.5f) / n * 2f - 1f;
            float v = (y + 0.5f) / n * 2f - 1f;
            bool outer = Inside(shape, u, v, 1f);
            bool inner = Inside(shape, u, v, 0.75f);
            Color c = inner ? Color.white : outer ? Color.black : Color.clear;
            tex.SetPixel(x, y, c);
        }
        tex.Apply();
        return tex;
    }

    static bool Inside(Shape shape, float u, float v, float scale)
    {
        switch (shape)
        {
            case Shape.Circle:
                return u * u + v * v < 0.9f * 0.9f * scale * scale;
            case Shape.Square:
                return Mathf.Abs(u) < 0.8f * scale && Mathf.Abs(v) < 0.8f * scale;
            default:
                float c = -0.23f;
                v = (v - c) / scale + c;
                u /= scale;
                return v > -0.8f && v < 0.9f && Mathf.Abs(u) < 0.75f * (0.9f - v) / 1.7f;
        }
    }
}
