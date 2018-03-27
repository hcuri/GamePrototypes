using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour {
    private static FloatingText popuptext;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("HUD");
        if (!popuptext)
            popuptext = Resources.Load<FloatingText>("PopupObj");
    }
	
    public static void CreateFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popuptext);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPos + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        instance.setText(text);
    }
}
