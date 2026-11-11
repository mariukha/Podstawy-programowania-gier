using UnityEngine;

public class Target : MonoBehaviour
{
    public float showTime = 1.5f;

    Renderer rend;
    Color baseColor;
    float timer;
    int hits;

    void Start()
    {
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }

    public void Hit()
    {
        hits++;
        timer = showTime;
        rend.material.color = Color.red;
        Debug.Log("Trafienie w tarczę: " + hits);
    }

    void Update()
    {
        if (timer <= 0f) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) rend.material.color = baseColor;
    }

    void OnGUI()
    {
        if (timer <= 0f) return;
        GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
        style.normal.textColor = Color.red;
        GUI.Label(new Rect(0, 60, Screen.width, 60), "TRAFIENIE! (" + hits + ")", style);
    }
}
