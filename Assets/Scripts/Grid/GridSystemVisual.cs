using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridQuadVisualPrefab;
    [SerializeField] private Material gridQuadVisualMaterial;
    [SerializeField] private Material gridQuadVisualActiveMaterial;
    [SerializeField] private Material gridQuadVisualMouseOverMaterial;

    private GridVisualQuad[,] gridVisualQuads;

    private List<GridVisualQuad> activeQuads;
    GridVisualQuad lastMouseVisualQuad = null;

    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.LogError("Two TurnSystem instanciated. Destroying one! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        activeQuads = new List<GridVisualQuad>();
    }

    private void Start() 
    {
        LevelGrid grid = LevelGrid.Instance;

        gridVisualQuads = new GridVisualQuad[
            LevelGrid.Instance.GetGridWidth(),
            LevelGrid.Instance.GetGridHeight()
        ];

        for (int x = 0; x < grid.GetGridWidth(); x++ )
        {
            for (int z = 0; z < grid.GetGridHeight(); z++ )
            {
                GridPosition gridPosition = new GridPosition(x,z);
                GridObject gridObject = grid.GetGridObject(gridPosition);

                if(!gridObject.HasGround())
                    continue;
                
                Transform quad =Instantiate(gridQuadVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                quad.parent = transform;

                gridVisualQuads[x, z] = quad.GetComponent<GridVisualQuad>();
            }
        }

        TurnSystem.Instance.OnSelectedSkillChange += TurnSystem_OnSelectedSkillChange;
    }

    public void DeactivateAllGridPosition()
    {
        if(activeQuads.Count > 0)
        {
            List<GridVisualQuad> quadsToRemove = new List<GridVisualQuad>();
            foreach(GridVisualQuad quad in activeQuads)
            {
                quadsToRemove.Add(quad);
                
                if(lastMouseVisualQuad == quad)
                    continue;

                ChangeQuadMaterial(quad, gridQuadVisualMaterial);
            }

            foreach(GridVisualQuad quad in quadsToRemove)
                activeQuads.Remove(quad);
        }
    }

    public void ActivateGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach(GridPosition gridPosition in gridPositionList)
        {
            GridVisualQuad quad = gridVisualQuads[gridPosition.x, gridPosition.z];
            activeQuads.Add(quad);

            if(lastMouseVisualQuad == quad)
                    continue;

            ChangeQuadMaterial(quad, gridQuadVisualActiveMaterial);
        }
    }

    public void MouseOver(GridPosition gridPosition)
    {
        if(lastMouseVisualQuad != null)
        {
            if(activeQuads.Contains(lastMouseVisualQuad))
            {
                ChangeQuadMaterial(lastMouseVisualQuad, gridQuadVisualActiveMaterial);
            }
            else
            {
                ChangeQuadMaterial(lastMouseVisualQuad, gridQuadVisualMaterial);
            }
        }

        GridVisualQuad newMouseVisualQuad = gridVisualQuads[gridPosition.x, gridPosition.z];
        ChangeQuadMaterial(newMouseVisualQuad, gridQuadVisualMouseOverMaterial);
        lastMouseVisualQuad = newMouseVisualQuad;
    }

    private void ChangeQuadMaterial(GridPosition gridPosition, Material material) 
    {
        GridVisualQuad quad = gridVisualQuads[gridPosition.x, gridPosition.z];

        if(quad != null)
            quad.ChangeMaterial(material);
    }

    private void ChangeQuadMaterial(GridVisualQuad quad, Material material) 
    {
        if(quad != null)
            quad.ChangeMaterial(material);
    }


    private void UpdateGridVisual()
    {
        DeactivateAllGridPosition();
        
        BaseSkill selectedSkill = TurnSystem.Instance.GetSelectedSkill();

        if(selectedSkill == null)
            return;

        ActivateGridPositionList(selectedSkill.GetValidGridPositionList());
    }

    private void TurnSystem_OnSelectedSkillChange(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
}
