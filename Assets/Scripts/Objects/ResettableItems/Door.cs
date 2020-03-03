using Coffee;
using UnityEngine;

public class Door : ResettableItem, IInteractible
{
    private Animator anim;
    private Collider coll;
    
    private static readonly int OpenAnimProp = Animator.StringToHash("open");

    private enum States
    {
        Open, Closed
    }
    
    private States state;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        
        CloseImmediate();
    }

    public virtual bool Use(IActor actor)
    {
        if (state == States.Closed)
        {
            Open();
        }
        else
        {
            Close();
        }
        return true;
    }

    public override void ResetState()
    {
        CloseImmediate();
    }

    private void Open()
    {
        coll.isTrigger = true;
        state = States.Open;
        anim.SetBool(OpenAnimProp, true);
    }
    
    private void Close()
    {
        coll.isTrigger = false;
        state = States.Closed;
        anim.SetBool(OpenAnimProp, false);
    }

    private void CloseImmediate()
    {
        Close();
        anim.Play("Close", 0, 1f);
    }
}
