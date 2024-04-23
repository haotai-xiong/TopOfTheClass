using UnityEngine;

public class ExitSwitchSubject : MonoBehaviour
{

    public GameObject ExitIconButton;

    public void ExitSubjectSwitchScreen()
    {
        if (ExitIconButton != null)
        {
            ExitIconButton.SetActive(false);
        }
        else
        {
            Debug.LogWarning("This GameObject has no parent.", this);
        }
    }
}
