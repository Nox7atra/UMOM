using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float _Speed = 10;
    
    private void Update()
    {
        transform.LookAt(new Vector3());
        transform.RotateAround( new Vector3(),Vector3.up, _Speed * Time.deltaTime );
    }
}
