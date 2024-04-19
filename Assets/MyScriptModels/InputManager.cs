using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private readonly string horizontalInput = "Horizontal";

    private readonly string jumpInput = "Jump";

    private readonly string dashInput = "Dash";


    public float HorizontalRaw()
    {
        return Input.GetAxisRaw(horizontalInput);
    }

    public bool Jump()
    {
        return Input.GetButtonDown(jumpInput);
    }

    public bool Dash()
    {
        return Input.GetButtonDown(dashInput);
    }
}
