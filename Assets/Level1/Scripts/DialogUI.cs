using UnityEngine;

public class DialogUI : MonoBehaviour
{
    public ClimbControl player;

    public void CloseDialog()
    {
        gameObject.SetActive(false);
        player.GoOn();
    }
}