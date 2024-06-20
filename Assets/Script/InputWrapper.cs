using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputWrapper : MonoBehaviour
{
    public Vector2 _move;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }
}
