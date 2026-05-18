using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public UIPanelAnimator diamondPanel;
    public UIPanelAnimator rayPanel;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray =
                Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<DiamondGenerator>())
                {
                    diamondPanel.Show();
                    rayPanel.Hide();
                    return;
                }

                if (hit.collider.GetComponent<RayEmitter>())
                {
                    rayPanel.Show();
                    diamondPanel.Hide();
                    return;
                }
            }

            diamondPanel.Hide();
            rayPanel.Hide();
        }
    }
}