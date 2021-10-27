using System;
using UnityEngine;

public class ForceField : Vulnerable
{
    private Resizable resizable;
    private Vector3 startLocalScale;
    private Action callback;

    protected override void Start()
    {
        base.Start();

        startLocalScale = transform.localScale;
        resizable = gameObject.GetComponent<Resizable>();

        if (resizable != null)
        {
            transform.localScale = Vector3.zero;
            resizable.ScaleOverTime(startLocalScale, 1.5f);
        }
    }

    public void Deactivate()
    {
        Die();
    }

    public void setDeathCallback(Action callback)
    {
        this.callback = callback;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Die()
    {
        callback?.Invoke();

        if (resizable == null)
        {
            base.Die();
        }
        else
        {
            resizable.ScaleOverTime(Vector3.zero, 1.5f, base.Die);
        }
    }
}
