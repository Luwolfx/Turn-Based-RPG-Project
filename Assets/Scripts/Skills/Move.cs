using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : BaseSkill
{
    [Header("Stats")]
    [SerializeField] [Range(1, 10)] private int maxMoveDistance = 4;

    [Header("Movement Config")]
    [SerializeField] [Range(.1f, 3f)] private float walkSpeed = .5f;
    [SerializeField] [Range(.1f, 3f)] private float runSpeed = 1f;
    [SerializeField] [Range(5f, 15f)] private float rotateSpeed = 10f;
    [SerializeField] [Range(.1f, 1f)] private float runDistance = .3f;

    private Vector3 targetPosition;

    protected override void Awake() 
    {
        base.Awake();
        targetPosition = transform.position;
    }

    public override void Cast(GridPosition gridPosition, Action onCastComplete)
    {
        base.StartCast(onCastComplete);
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    }

    public override string GetSkillName() => "Move";

    private void Update()
    {
        if(!isCasting)
            return;

        float stopDistance = .05f;
        if(Vector3.Distance(transform.position, targetPosition) > runDistance)
        {
            MoveCharacter(runSpeed);
        }
        else if(Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            MoveCharacter(walkSpeed);
        }
        else
        {
            characterAnimation.UpdateMovementSpeed(0);
            base.EndCast();
        }
    }

    private void MoveCharacter(float speed)
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed * speed);
        characterAnimation.UpdateMovementSpeed(speed);
    }

    public void FaceTarget(Vector3 target)
    {
        transform.forward = target;
    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition characterGridPosition = character.GetGridPosition();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = characterGridPosition + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if(characterGridPosition == testGridPosition)
                    continue;

                if(LevelGrid.Instance.HasCharacterOnGrid(testGridPosition))
                    continue;

                if(!LevelGrid.Instance.HasGroundOnGridPosition(testGridPosition))
                    continue;
                
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public Character GetNearestOpositeCharacter()
    {
        Character nearest = null;
        float nearestDistance = 9999f;

        foreach(Character character in TurnSystem.Instance.GetOpositeCharacters(character))
        {
            float distance = Vector3.Distance(transform.position, character.transform.position);
            if(distance < nearestDistance)
            {
                nearest = character;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public List<GridPosition> GetValidAiPosition()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition characterGridPosition = GetNearestOpositeCharacter().GetGridPosition();

        for(int x = -1; x <= 1; x++)
        {
            for(int z = -1; z <= 1; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = characterGridPosition + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if(characterGridPosition == testGridPosition)
                    continue;

                if(LevelGrid.Instance.HasCharacterOnGrid(testGridPosition))
                    continue;

                if(!LevelGrid.Instance.HasGroundOnGridPosition(testGridPosition))
                    continue;

                if(!GetValidGridPositionList().Contains(testGridPosition))
                    continue;
                
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidAiPosition();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        } else
        {
            // No possible Enemy AI Actions
            return null;
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = LevelGrid.Instance.GetTargetNearCharactersCount(character, gridPosition, maxMoveDistance);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }

}
