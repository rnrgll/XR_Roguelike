using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPile<T> : IReadPile<T>, IAddRemovePile<T>, IReturnPile<T>
{
    private readonly List<T> _cards = new();

    public int Count => _cards.Count;

    public bool IsEmpty => Count == 0;

    public IEnumerable<T> Cards => _cards;

    public void Add(T card) => _cards.Add(card);

    public bool Remove(T card) => _cards.Remove(card);

    public T GetCard(int idx)
    {
        return _cards[idx];
    }

    public List<T> GetCardList()
    {
        return _cards;
    }
}

public class HandPile<T> : IReadPile<T>, IAddRemovePile<T>, ISwappablePile<T>, IClearPile<T>, IReturnPile<T>
{
    private readonly List<T> _cards = new();

    public int Count => _cards.Count;

    public bool IsEmpty => Count == 0;

    public IEnumerable<T> Cards => _cards;

    public void Add(T card) => _cards.Add(card);

    public bool Remove(T card) => _cards.Remove(card);


    public void Swap(T outCard, T inCard)
    {
        int idx = _cards.IndexOf(outCard);
        if (idx < 0) throw new InvalidOperationException("교체할 카드가 손패에 없습니다.");
        _cards[idx] = inCard;
    }

    public void Clear() => _cards.Clear();

    public T GetCard(int idx)
    {
        return _cards[idx];
    }

    public List<T> GetCardList()
    {
        return _cards;
    }
}

public class GraveyardPile<T> : IReadPile<T>, IAddRemovePile<T>, IClearPile<T>, IReturnPile<T>
{
    private readonly List<T> _cards = new();

    public int Count => _cards.Count;

    public bool IsEmpty => Count == 0;

    public IEnumerable<T> Cards => _cards;

    public void Add(T card) => _cards.Add(card);
    public bool Remove(T card) => _cards.Remove(card);

    public void Clear() => _cards.Clear();

    public T GetCard(int idx)
    {
        return _cards[idx];
    }

    public List<T> GetCardList()
    {
        return _cards;
    }
}


public class BattlePile<T> : IReadPile<T>, IAddRemovePile<T>, IDrawablePile<T>, IShuffleablePile<T>, ISwappablePile<T>, IClearPile<T>, IReturnPile<T>
{
    private readonly List<T> _cards = new();

    public int Count => _cards.Count;

    public bool IsEmpty => Count == 0;

    public IEnumerable<T> Cards => _cards;

    public void Add(T card) => _cards.Add(card);
    public bool Remove(T card) => _cards.Remove(card);

    public T Draw()
    {
        T card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        int n = _cards.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = Manager.randomManager.RandInt(0, i + 1);
            T temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }
    }

    public void Swap(T outCard, T inCard)
    {
        int idx = _cards.IndexOf(outCard);
        if (idx < 0) throw new InvalidOperationException("교체할 카드가 덱에 없습니다.");
        _cards[idx] = inCard;
    }
    public void Clear() => _cards.Clear();

    public T GetCard(int idx)
    {
        return _cards[idx];
    }

    public List<T> GetCardList()
    {
        return _cards;
    }
}