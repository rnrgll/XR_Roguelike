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
    public virtual void InitializeUI(PlayerController pc)
    {
        if (cardController != null)
            UnSubscribe();

        cardController = pc.GetCardController();

        Subscribe();
    }

    protected void OnEnable()
    {
        if (cardController != null)
        {
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

}
