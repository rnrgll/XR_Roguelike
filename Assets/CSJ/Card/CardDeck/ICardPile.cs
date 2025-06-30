using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadPile<T>
{
    int Count { get; }
    bool IsEmpty { get; }
    IEnumerable<T> Cards { get; }
}

public interface IAddRemovePile<T>
{
    void Add(T card);
    bool Remove(T card);
}

public interface IDrawablePile<T>
{
    T Draw();
}

public interface IShuffleablePile<T>
{
    void Shuffle();
}

public interface ISwappablePile<T>
{
    void Swap(T outCard, T inCard);
}

public interface IClearPile<T>
{
    void Clear();
}

public interface IReturnPile<T>
{
    T GetCard(int idx);
    List<T> GetCardList();
}