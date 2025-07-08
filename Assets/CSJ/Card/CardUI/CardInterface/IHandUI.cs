using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandUI
{
    void InitializeUI(PlayerController pc);
}

public abstract class UIRequire : MonoBehaviour, IHandUI
{
    protected CardController cardController;
    protected PlayerController playerController;
    public virtual void InitializeUI(PlayerController pc)
    {
        if (cardController != null)
            UnSubscribe();

        playerController = pc;
        cardController = pc.GetCardController();

        Subscribe();
    }

    protected void OnEnable()
    {
        if (cardController != null)
        {
            UnSubscribe();
            Subscribe();
        }
    }

    protected void OnDisable()
    {
        if (cardController != null)
            UnSubscribe();
    }

    protected abstract void Subscribe();

    protected abstract void UnSubscribe();

    protected virtual void OnDestroy()
    {
        UnSubscribe();
        UnSubscribe();
    }
}
