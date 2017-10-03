using UnityEngine;

public class ResizeScriptToScreen : MonoBehaviour
{
    protected void Start()
    {
        var sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            return;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        sr.size = new Vector2((float)worldScreenWidth, (float)worldScreenHeight);
    }
}
