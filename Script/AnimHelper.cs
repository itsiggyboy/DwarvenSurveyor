using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHelper : MonoBehaviour
{
    public Animator anim;


    public void SetAnimBool(bool b)
    {
        anim.SetBool("b", b);
    }

    public void Toggle(string booleanName)
    {
        anim.SetBool("booleanName", !anim.GetBool("booleanName"));
    }
}
