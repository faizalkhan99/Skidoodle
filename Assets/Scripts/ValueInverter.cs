using UnityEngine;

public class ValueInverter : MonoBehaviour
{
    public void InvertVal(bool value)
    {
        gameObject.SetActive(!value);   
    }
}
