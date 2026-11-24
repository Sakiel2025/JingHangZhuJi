using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToNPC3 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("NPC3");
        }
    }
}