using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public Text label;
    public Material standard;
    public Material hovered;
    public Material click;

    public event Action<string> Clicked;

    private bool hovering = false;
    private bool mouseDown = false;

    private void Update()
    {
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hits.Any(hit => hit.transform == transform))
        {
            // Trigger click event
            if (Input.GetKeyUp(KeyCode.Mouse0) && mouseDown)
            {
                if (Clicked != null) { Clicked(label.text); }
                mouseDown = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!mouseDown) { GetComponent<MeshRenderer>().material = click; }
                mouseDown = true;
            }
            else if (!mouseDown)
            {
                GetComponent<MeshRenderer>().material = hovered;
            }
            hovering = true;
        }
        else
        {
            if (hovering && !mouseDown) { GetComponent<MeshRenderer>().material = standard; }
            hovering = false;
        }

        // If the key is released (does not have to hover to happen)
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            mouseDown = false;
        }
    }
}
