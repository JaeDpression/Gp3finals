using UnityEngine;

public class RetailItem : MonoBehaviour
{
    public GameController controller;

    private void OnMouseDown()
    {
        controller.ItemClicked(this);
    }
}
