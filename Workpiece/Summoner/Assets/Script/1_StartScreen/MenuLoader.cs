using UnityEngine;

// Loads the shared HUD menu for scenes that need menu and option UI.
public class MenuLoader : MonoBehaviour
{
    private void Awake()
    {
        StartGameFlow.LoadStartScreenHud();
    }
}
