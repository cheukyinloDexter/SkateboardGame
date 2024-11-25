using UnityEngine;

public class DragAndRelease : MonoBehaviour
{
    private Vector3 _startDragPosition;
    private Vector3 _releasePosition;
    private bool _isDragging = false;

    private Rigidbody _rigidbody;

    [SerializeField]
    private float forceMultiplier = 10f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        // Store the position where the drag starts
        _startDragPosition = Input.mousePosition;
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        // Optional: You can add visual feedback for the drag
    }

    void OnMouseUp()
    {
        if (_isDragging)
        {
            // Store the position where the drag ends
            _releasePosition = Input.mousePosition;

            // Calculate the drag direction
            Vector3 dragDirection = _releasePosition - _startDragPosition;

            // Convert the drag direction to world space
            Vector3 worldDirection = new Vector3(dragDirection.x, 0, dragDirection.y);

            // Apply force in the opposite direction of the drag
            _rigidbody.AddForce(-worldDirection * forceMultiplier);

            _isDragging = false;
        }
    }
}
