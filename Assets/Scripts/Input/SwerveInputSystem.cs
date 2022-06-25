using UnityEngine;

public class SwerveInputSystem : MonoBehaviour
{
    private float lastFingerPositionX;
    private float moveFactorX;
    public float MoveFactorX => moveFactorX;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastFingerPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            moveFactorX = Input.mousePosition.x - lastFingerPositionX;
            lastFingerPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveFactorX = 0f;
        }
    }
}
