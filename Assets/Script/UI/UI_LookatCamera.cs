using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LookatCamera : MonoBehaviour
{
    Transform myTransform;
    Vector3 eulerAngles;

    private void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.position = myTransform.parent.position + Vector3.up * 0.25F;
        myTransform.LookAt(Camera.main.transform);
        eulerAngles = myTransform.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.y += 180F;
        myTransform.eulerAngles = eulerAngles;
    }
}
