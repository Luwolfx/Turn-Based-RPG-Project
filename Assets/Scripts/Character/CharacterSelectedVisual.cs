using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectedVisual : MonoBehaviour
{
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material hasMovesMaterial;
    [SerializeField] private Material noneMaterial;
    [SerializeField] private Material enemyMaterial;
    private Character character;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Start()
    {
        character = transform.parent.GetComponent<Character>();
        TurnSystem.Instance.OnSelectedCharacterChange += TurnSystem_OnSelectedCharacterChange;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        UpdateVisual();
    }

    private void TurnSystem_OnSelectedCharacterChange(object sender, EventArgs args)
    {
        UpdateVisual();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs args)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(!character.OwnedByPlayer())
        {
            meshRenderer.enabled = true;
            meshRenderer.materials = new Material[] {enemyMaterial};
            return;
        }

        TurnSystem turnSystem = TurnSystem.Instance;
        if(turnSystem.GetSelectedCharacter() == character)
        {
            meshRenderer.enabled = true;
            meshRenderer.materials = new Material[] {selectedMaterial};
            return;
        }
        else if(turnSystem.CharacterHasMoveThisTurn(character))
        {
            meshRenderer.enabled = true;
            meshRenderer.materials = new Material[] {hasMovesMaterial};
            return;
        }
        else
        {
            meshRenderer.enabled = true;
            meshRenderer.materials = new Material[] {noneMaterial};
            return;
        }
    }

}
