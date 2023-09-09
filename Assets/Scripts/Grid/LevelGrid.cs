using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Vector2Int gridSquares;
    [SerializeField] private float squareSize = .5f;
    private GridSystem gridSystem;

    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.LogError("Two TurnSystem was instanciated. Destroying one! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem(gridSquares.x, gridSquares.y, squareSize);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab); //? DEBUG PURPOSES
    }

    public void AddCharacterAtGridPosition(GridPosition gridPosition, Character character)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddCharacter(character);
    }

    public List<Character> GetCharacterListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetCharacterList();
    }

    public Character GetCharacterAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetCharacterList()[0];
    }

    public void RemoveCharacterAtGridPosition(GridPosition gridPosition, Character character)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveCharacter(character);
    }

    public void RemoveCharacterOfGrid(Character character)
    {
        RemoveCharacterAtGridPosition(character.GetGridPosition(), character);
    }

    public void CharacterMovedGridPosition(Character character, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveCharacterAtGridPosition(fromGridPosition, character);
        AddCharacterAtGridPosition(toGridPosition, character);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public GridObject GetGridObject(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition);

    public int GetGridWidth() => gridSystem.GetWidth();

    public int GetGridHeight() => gridSystem.GetHeight();

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public bool HasCharacterOnGrid(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyCharacters();
    }

    public bool HasPlayerCharacterOnGrid(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasPlayerCharacters();
    }

    public bool HasEnemyCharacterOnGrid(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasEnemyCharacters();
    }

    public bool HasGroundOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasGround();
    }

    public bool HasOpositeCharacterOnGrid(Character character, GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        if(character.OwnedByPlayer())
        {
            return gridObject.HasEnemyCharacters();
        }
        else
        {
            return gridObject.HasPlayerCharacters();
        }
    }

    public List<GridPosition> GetValidTargetGridPositionList(Character character ,GridPosition gridPosition, int distance)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int z = -distance; z <= distance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > distance)
                    continue;

                if (!HasCharacterOnGrid(testGridPosition))
                    continue;

                Character targetcharacter = GetCharacterAtGridPosition(testGridPosition);

                if (!targetcharacter.OwnedByPlayer() == character.OwnedByPlayer())
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }


    public int GetTargetNearCharactersCount(Character character, GridPosition gridPosition, int maxDistance)
    {
        return GetValidTargetGridPositionList(character, gridPosition, maxDistance).Count;
    }

}
