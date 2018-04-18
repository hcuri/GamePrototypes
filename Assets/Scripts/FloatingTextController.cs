using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour {
    private static FloatingText popuptext;
    private static FloatingText popuptext2;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("HUD");
        if (!popuptext)
            popuptext = Resources.Load<FloatingText>("PopupObj");
        if (!popuptext2)
            popuptext2 = Resources.Load<FloatingText>("PopupObj2");
    }
	
    public static void CreateFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popuptext);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPos + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        instance.setText(text);
    }

    public static void CreateFixedFloatingText(int PU_type_name)
    {
        string text = "";
        switch(PU_type_name)
        {
            case 0: text = "10 HP Recovered"; break;
            case 1: text = "Weapon Power ++"; break;
            case 2: text = "Weapon Speed ++"; break;
            case 3: text = "Maximum Power Mode, ON!"; break;
            case 4: text = "Invisible Mode, ON!"; break;
            case 5: text = "Invulnerable Mode ON!"; break;
        }

        FloatingText instance = Instantiate(popuptext2);
        Vector2 screenPos = new Vector2(0,-250);
        instance.transform.position = screenPos;
        instance.transform.SetParent(canvas.transform, false);    
        instance.setText(text);
    }
}
