using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Material green;
    public bool success;

    private void Update()
    {
        if (success)
        {
            GetComponent<MeshRenderer>().material = green;
        }
    }
}
