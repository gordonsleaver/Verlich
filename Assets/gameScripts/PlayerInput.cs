using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 movementInput { get; private set; }
    public bool jumpInput { get; private set; }
    public Vector2 mouseInput { get; private set; }
    public bool sTreeInput { get; private set; }

    // Update is called once per frame, checks what keys player is pressing and sets variables accordingly
    private void Update()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        jumpInput = Input.GetKey("space");
        sTreeInput = Input.GetKey(KeyCode.E);
        mouseInput = Input.mousePosition;
    }
}
