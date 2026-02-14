using UnityEngine;

public class ClimbControl : MonoBehaviour
{
    public float speed = 2f;
    public bool canClimb = true;

    void Update()
    {
        if (canClimb)
        {
            float v = Input.GetAxis("Vertical");
            transform.Translate(0, v * speed * Time.deltaTime, 0);
        }
    }

    public void Stop()
    {
        canClimb = false;
    }

    public void GoOn()
    {
        canClimb = true;
    }
}