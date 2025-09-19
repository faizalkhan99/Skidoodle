using UnityEngine;

public class CarPositionReset : MonoBehaviour
{
    [SerializeField] private Transform _startPos;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car"))
        {
            other.gameObject.SetActive(false);
            other.transform.position = _startPos.position;
            other.gameObject.SetActive(true);

        }
    }
}
