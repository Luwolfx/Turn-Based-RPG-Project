using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private GridObject gridObject;
    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;

        GridPosition gridPosition = gridObject.GetGridPosition();
        gameObject.name = $"Square ({gridPosition.x},{gridPosition.z})";
        
        if(!gridObject.HasGround())
            Destroy(gameObject);
    }

    public void Update()
    {
        text.text = gridObject.ToString();
    }
}
