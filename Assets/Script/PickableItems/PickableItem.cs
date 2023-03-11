using UnityEngine;


public abstract class PickableItem : MonoBehaviour
{
    [Header("Pickable Item Parameters")]
    [SerializeField] protected Rigidbody rBody;
    [SerializeField] protected Collider itemCollider;
    [SerializeField] private GameObject canvas;
    [SerializeField] protected PickableItemType itemType;


    public void DisaplayDetails(bool enable) => canvas.SetActive(enable);

    public abstract PickableItemType Pick();
    public abstract void Drop(Vector3 positionToDrop, Quaternion rotationWhileDropping);
}


public enum PickableItemType
{
    Gun,
    Bullet,
}