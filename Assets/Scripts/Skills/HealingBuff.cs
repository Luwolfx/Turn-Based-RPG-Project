using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBuff : BaseSkill
{
    [SerializeField] [Range(1,10)] private int turnsDuration = 3;
    [SerializeField] private Character caster;

    private Vector3 targetPosition;

    private void Start() 
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        character.HealDamage(caster.GetStats().GetInteligence());

        turnsDuration--;

        if(turnsDuration <= 0)
            Destroy(this);
    }

    public override bool IsPassiveSkill() => true;

    public void SetCaster(Character character)
    {
        caster = character;
    }

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
