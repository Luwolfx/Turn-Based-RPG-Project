using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UISkillButton : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    BaseSkill buttonSkill;

    public void SetBaseSkill(BaseSkill baseSkill)
    {
        buttonSkill = baseSkill;
        text.text = baseSkill.GetSkillName();

        /*
        Debug.Log("To use "+ baseSkill.GetSkillName()+" it needs "+ baseSkill.GetManaPointsRequired() + " mana. Current Char has "+
                TurnSystem.Instance.GetSelectedCharacter().GetMana()+ " mana!");
        //*/

        if(TurnSystem.Instance.GetSelectedCharacter().GetMana() < baseSkill.GetManaPointsRequired())
        {
            GetComponent<Button>().interactable = false;
            return;
        }

        GetComponent<Button>().onClick.AddListener(() => {
            TurnSystem.Instance.SetSelectedSkill(baseSkill);
        });
    }

    public void ChangeOutlineColor(Color color)
    {
        GetComponent<Outline>().effectColor = color;
    }

    public void ResetOutlineColor()
    {
        GetComponent<Outline>().effectColor = new Color(0f, 0f, 0f, .5f);
    }

    public BaseSkill GetSkill()
    {
        return buttonSkill;
    }
}
