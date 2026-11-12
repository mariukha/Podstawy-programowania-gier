using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class Minimap : MonoBehaviour
{
    public Transform player;
    public float height = 30f;
    public float zoom = 10f;
    public float mapScale = 0.3f;
    public bool rotateWithPlayer;

    Camera cam;
    RenderTexture texture;
    Texture2D panelTex;

    void Start()
    {
        texture = new RenderTexture(512, 512, 16);
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = zoom;
        cam.targetTexture = texture;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.12f, 0.15f);
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = height + 20f;
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    void OnBeginCamera(ScriptableRenderContext context, Camera current)
    {
        bool show = current == cam;
        foreach (MinimapIcon icon in MinimapIcon.All) icon.SetVisible(show);
    }

    void LateUpdate()
    {
        Vector3 p = player.position;
        transform.position = new Vector3(p.x, p.y + height, p.z);
        float yaw = rotateWithPlayer ? player.eulerAngles.y : 0f;
        transform.rotation = Quaternion.Euler(90f, yaw, 0f);
    }

    static Texture2D Rounded(Color color)
    {
        const int n = 32;
        const float r = 10f;
        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            float dx = Mathf.Max(0f, Mathf.Abs(x + 0.5f - n / 2f) - (n / 2f - r));
            float dy = Mathf.Max(0f, Mathf.Abs(y + 0.5f - n / 2f) - (n / 2f - r));
            float a = Mathf.Clamp01(r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
            tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * a));
        }
        tex.Apply();
        return tex;
    }

    void OnGUI()
    {
        if (texture == null) return;
        if (panelTex == null) panelTex = Rounded(new Color(0f, 0f, 0f, 0.5f));

        float size = Screen.height * mapScale;
        float m = Screen.height * 0.02f;
        float x = Screen.width - size - m;
        float y = m;
        float b = Mathf.Max(2f, Screen.height * 0.004f);

        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(x - b, y - b, size + 2f * b, size + 2f * b), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(x, y, size, size), texture);

        int font = Mathf.RoundToInt(Screen.height * 0.02f);
        float line = font * 1.5f;
        GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = font, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(0, 0, 0, 0) };
        style.normal.textColor = Color.white;

        List<MinimapIcon> legend = new List<MinimapIcon>();
        HashSet<string> shown = new HashSet<string>();
        foreach (MinimapIcon icon in MinimapIcon.All)
            if (shown.Add(icon.legend)) legend.Add(icon);

        float ly = y + size + b + m * 0.5f;
        GUIStyle panel = new GUIStyle { border = new RectOffset(10, 10, 10, 10) };
        panel.normal.background = panelTex;
        GUI.Box(new Rect(x - b, ly, size + 2f * b, legend.Count * line + font * 0.6f), GUIContent.none, panel);

        ly += font * 0.3f;
        foreach (MinimapIcon icon in legend)
        {
            GUI.color = icon.color;
            GUI.DrawTexture(new Rect(x + font * 0.4f, ly + (line - font) / 2f, font, font), icon.Texture);
            GUI.color = Color.white;
            GUI.Label(new Rect(x + font * 1.8f, ly, size, line), icon.legend, style);
            ly += line;
        }
    }
}
