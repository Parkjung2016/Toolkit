using UnityEngine;
using PJH.Toolkit.CustomDebug;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PJHDebug.LogColorPart("Test", color: Color.red, tag: "Test");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
