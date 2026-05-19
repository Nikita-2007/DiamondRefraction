using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public UIPanelAnimator diamondPanel;
    public UIPanelAnimator rayPanel;
    public RuntimeTransformGizmo gizmo;

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
                if (hit.collider.gameObject.layer ==
                    LayerMask.NameToLayer("Gizmo"))
                {
                    return;
                }
                if (hit.collider.GetComponent<DiamondGenerator>())
                {
                    diamondPanel.Show();
                    rayPanel.Hide();

                    gizmo.SetTarget(hit.transform);

                    return;
                }
                if (hit.collider.GetComponent<RayEmitterVisualizer>())
                {
                    rayPanel.Show();
                    diamondPanel.Hide();

                    gizmo.SetTarget(hit.transform);

                    return;
                }
            }
            diamondPanel.Hide();
            rayPanel.Hide();
            gizmo.Hide();
        }
    }
}