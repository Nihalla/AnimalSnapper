using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hashing : MonoBehaviour
{
    public int IdleBool;
    public int WalkingBool;
    public int EatingBool;

    private void Awake()
    {
        IdleBool = Animator.StringToHash("Idle");
        WalkingBool = Animator.StringToHash("Walking");
        EatingBool = Animator.StringToHash("Eating");
    }
}
