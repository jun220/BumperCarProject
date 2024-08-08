using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCube : MonoBehaviour
{
    public float RotationSpeed = 10.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.fixedDeltaTime, Space.World);
    }
}
