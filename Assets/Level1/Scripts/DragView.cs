using UnityEngine;

public class DragView : MonoBehaviour
{
    float dragSpeed = 0.5f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float h = Input.GetAxis("Mouse X");
            transform.Translate(-h * dragSpeed, 0, 0);
        }
    }
}