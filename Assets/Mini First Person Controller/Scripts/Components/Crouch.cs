using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;

    [Header("Slow Movement")]
    [Tooltip("Movement to slow down when crouched.")]
    public FirstPersonMovement movement;
    [Tooltip("Movement speed when crouched.")]
    public float movementSpeed = 2;

    [Header("Low Head")]
    [Tooltip("Head to lower when crouched.")]
    public Transform headToLower;
    [HideInInspector]
    public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;
    
    [Tooltip("Speed of head movement when crouching.")]
    public float crouchTransitionSpeed = 6f; // Smooth transition speed

    [Tooltip("Collider to lower when crouched.")]
    public CapsuleCollider colliderToLower;
    [HideInInspector]
    public float? defaultColliderHeight;

    private float targetHeadYPosition;
    private float targetColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    void Reset()
    {
        // Try to get components.
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    void Start()
    {
        if (headToLower)
        {
            defaultHeadYLocalPosition = headToLower.localPosition.y;
            targetHeadYPosition = defaultHeadYLocalPosition.Value;
        }

        if (colliderToLower)
        {
            defaultColliderHeight = colliderToLower.height;
            targetColliderHeight = defaultColliderHeight.Value;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKey(key))
        {
            if (!defaultHeadYLocalPosition.HasValue)
                defaultHeadYLocalPosition = headToLower.localPosition.y;

            if (!defaultColliderHeight.HasValue)
                defaultColliderHeight = colliderToLower.height;

            // Set target positions for smooth transition
            targetHeadYPosition = crouchYHeadPosition;
            targetColliderHeight = defaultColliderHeight.Value * 0.5f;

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            // Reset to default positions
            targetHeadYPosition = defaultHeadYLocalPosition.Value;
            targetColliderHeight = defaultColliderHeight.Value;

            if (IsCrouched)
            {
                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }

        // Smoothly interpolate the camera position
        if (headToLower)
        {
            float newY = Mathf.Lerp(headToLower.localPosition.y, targetHeadYPosition, Time.deltaTime * crouchTransitionSpeed);
            headToLower.localPosition = new Vector3(headToLower.localPosition.x, newY, headToLower.localPosition.z);
        }

        // Smoothly interpolate the collider size
        if (colliderToLower)
        {
            float newHeight = Mathf.Lerp(colliderToLower.height, targetColliderHeight, Time.deltaTime * crouchTransitionSpeed);
            colliderToLower.height = newHeight;
            colliderToLower.center = Vector3.up * newHeight * 0.5f;
        }
    }

    #region Speed override.
    void SetSpeedOverrideActive(bool state)
    {
        if (!movement) return;

        if (state)
        {
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}
