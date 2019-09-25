using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteWheelSelection<T>
{
    private Dictionary<T, float> _actions = new Dictionary<T, float>();
    private float _totalWeight;
    
    public void Add(T key, float weight)
    {
        _actions.Add(key, weight);
        _totalWeight += weight;
    }

    public void Remove(T keyName)
    {
        if (_actions.ContainsKey(keyName))
        {
            float weight = _actions[keyName];
            _totalWeight -= weight;
            _actions.Remove(keyName);
        }
    }

    public void ClearAll()
    {
        _actions.Clear();
    }
    /*
    public T Roulette()
    {
        float rValue = Random.Range(0f, _totalWeight); 
        T result = default;
        foreach (KeyValuePair<T, float> action in _actions)
        {
            rValue -= action.Value; // Restamos su peso a nuestro número random
            if (rValue < 0) // Si el resultado nos da menor a 0, quiere decir que esta es nuestra acción
            {
                result = action.Key;
                break; 
            }
        }
        return result;
    }*/
}
