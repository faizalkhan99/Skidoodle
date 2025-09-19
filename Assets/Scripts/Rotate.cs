using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotDir;
    [SerializeField] private float _rotationSpeed;
    void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime * _rotDir);        
    }
}
