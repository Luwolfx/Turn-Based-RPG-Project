using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField] private Transform skillButtonPrefab;
    [SerializeField] private Transform skillListTransform;

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private GameObject skipTurnButton;

    private void Start() 
    {
        TurnSystem.Instance.OnSelectedCharacterChange += TurnSystem_OnSelectedCharacterChange;
        TurnSystem.Instance.OnSelectedSkillChange += TurnSystem_OnSelectedSkillChange;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        CreateSkillButtons();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        turnText.text = $"Turn {TurnSystem.Instance.GetTurnCount()}";

        skipTurnButton.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }

    private void TurnSystem_OnSelectedCharacterChange(object sender, EventArgs e)
    {
        CreateSkillButtons();
    }
    private void TurnSystem_OnSelectedSkillChange(object sender, EventArgs e)
    {
        UpdateSelectedButton();
    }

    private void CreateSkillButtons()
    {
        foreach(Transform buttons in skillListTransform)
            Destroy(buttons.gameObject);

        Character selectedCharacter = TurnSystem.Instance.GetSelectedCharacter();
        if(selectedCharacter == null)
            return;

        foreach(BaseSkill baseSkill in selectedCharacter.GetSkills())
        {
            if(baseSkill.IsPassiveSkill())
                continue;

            Transform instantiatedSkillButton = Instantiate(skillButtonPrefab, skillListTransform);
            UISkillButton skillButton = instantiatedSkillButton.GetComponent<UISkillButton>();

            skillButton.SetBaseSkill(baseSkill);
        }

        UpdateSelectedButton();
    }

    void UpdateSelectedButton()
    {
        BaseSkill newSelectedSkill = TurnSystem.Instance.GetSelectedSkill();

        if(newSelectedSkill == null)
            return;

        UISkillButton newSelectedButton = null;

        foreach (Transform button in skillListTransform)
        {
            UISkillButton skillButton = button.GetComponent<UISkillButton>();

            skillButton.ResetOutlineColor();

            if(skillButton.GetSkill() == newSelectedSkill)
                newSelectedButton = skillButton;

        }
        
        newSelectedButton.ChangeOutlineColor(Color.yellow);
    }
}
