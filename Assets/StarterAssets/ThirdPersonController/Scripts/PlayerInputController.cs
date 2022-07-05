using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public bool IsJumping;
    public bool IsSprinting;
    public bool IsMoving => Move != Vector2.zero;

    public Vector2 Move;

    private void Update()
    {
        HandleInput();
        Debug.Log(Move.magnitude);
    }

    private void HandleInput()
    {
        var xMov = Input.GetAxis("Horizontal");
        var yMov = Input.GetAxis("Vertical");
        Move = new Vector2(xMov, yMov);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            IsJumping = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IsSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsSprinting = false;
        }
    }
}