using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    private static MouseRaycast instance;
    [SerializeField] private LayerMask targetLayers;

    GridPosition mouseOverGridPosition;

    private void Awake() 
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() 
    {
        if(mouseOverGridPosition != LevelGrid.Instance.GetGridPosition(GetPosition()))
        {
            mouseOverGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
            GridSystemVisual.Instance.MouseOver(mouseOverGridPosition);
        }
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.targetLayers);

        return raycastHit.point;
    }

}
