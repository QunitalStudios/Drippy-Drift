using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftCarWheels : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.Rotate(Time.deltaTime + 4, 0f, 0f);
        transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).transform.Rotate(Time.deltaTime + 4, 0f, 0f);
        transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).transform.Rotate(Time.deltaTime + 2, 0f, 0f);
        transform.GetChild(2).transform.GetChild(0).transform.GetChild(3).transform.Rotate(Time.deltaTime + 2, 0f, 0f);
    }
}
