using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDebuff : BaseSkill
{
    [SerializeField] [Range(1,10)] private int turnsDuration = 3;

    private Vector3 targetPosition;

    private void Start() 
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        float damage = 2f;
        character.TakeDamage(damage);

        turnsDuration--;

        if(turnsDuration <= 0)
            Destroy(this);
    }

    public override bool IsPassiveSkill() => true;

    public override string GetSkillName() => "HealingBuff";

    public override List<GridPosition> GetValidGridPositionList()
    {
        // EMPTY
        // SKILL DOES NOT NEED GRID POSITIONS
        throw new NotImplementedException();
    }

    public override void Cast(GridPosition gridPosition, Action onCastComplete)
    {
        // EMPTY
        // SKILL DOES NOT CAST, JUST DO SOMETHING EVERY TURN!
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // EMPTY
        // ENEMY CAN'T DO THIS ACTION
        throw new NotImplementedException();
    }
}
