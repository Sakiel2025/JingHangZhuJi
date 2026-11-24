using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToLoadScene : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Äãµã»÷ÆÁÄ»ÁË£¡");
            SceneManager.LoadScene("NPC1");
        }
    }
}