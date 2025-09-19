using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsMove : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _moveSpeed;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        _rb.MovePosition(transform.position + transform.forward * _moveSpeed * Time.deltaTime);
    }
}
