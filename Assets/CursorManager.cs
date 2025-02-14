using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexutre;

    private Vector2 cursorHotspot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexutre.width / 2, cursorTexutre.height / 2);
        Cursor.SetCursor(cursorTexutre,cursorHotspot,CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
