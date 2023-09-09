using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator anim;

    private void Start() 
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void UpdateMovementSpeed(float speed)
    {
        anim.SetFloat("MovingSpeed", speed);
    }

    public void TriggerAnimation(string skillName)
    {
        anim.SetTrigger(skillName);
    }
}
