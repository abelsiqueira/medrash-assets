using UnityEngine;
using System.Collections;

public class MainCharacterCamera : MonoBehaviour
{
    public Transform cameraTransform;
    private Transform target;

    public float distance = 3.0f;

    public float height = 1.0f;

    public float angularSmoothLag = 0.3f;
    public float angularMaxSpeed = 175.0f;

    public float heightSmoothLag = 0.3f;

    public float snapSmoothLag = 0.2f;
    public float snapMaxSpeed = 720.0f;

    public float clampHeadPositionScreenSpace = 0.75f;

    public float lockCameraTimeout = 0.2f;

    private Vector3 headOffset = Vector3.zero;
    private Vector3 centerOffset = Vector3.zero;

    private float heightVelocity = 0.0f;
    private float angleVelocity = 0.0f;
    private bool snap = false;
    private MainCharacterController controller;
    private float targetHeight = 100000.0f;

    void SetDistance(float newDist)
    {
        distance = newDist;
    }

    float GetDistance()
    {
        return distance;
    }

    void Awake()
    {
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
		
        if (!cameraTransform)
        {
            Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
            enabled = false;
		}
        target = transform;
        if (target)
        {
            controller = target.GetComponent<MainCharacterController>();
        }

        if (controller)
        {
            CharacterController characterController = (CharacterController) target.GetComponent("CharacterController");
            centerOffset = characterController.bounds.center - target.position;
            headOffset = centerOffset;
            headOffset.y = characterController.bounds.max.y - target.position.y;
        }
        else
		{
            Debug.Log("Please assign a target to the camera that has a ThirdPersonController script attached.");	
		}
        Cut(target, centerOffset);
    }

    void DebugDrawStuff()
    {
        Debug.DrawLine(target.position, target.position + headOffset);
    }

    float AngleDistance(float a, float b)
    {
        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);

        return Mathf.Abs(b - a);
    }

    void Apply(Transform dummyTarget, Vector3 dummyCenter)
    {
        if (!controller) return;

        Vector3 targetCenter = target.position + centerOffset;
        Vector3 targetHead = target.position + headOffset;

        float originalTargetAngle = target.eulerAngles.y;
        float currentAngle = cameraTransform.eulerAngles.y;

        float targetAngle = originalTargetAngle;

        if (Input.GetButton("Fire2")) snap = true;

        if (snap)
        {
            if (AngleDistance(currentAngle, originalTargetAngle) < 3.0f) snap = false;

            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
        }
        else
        {
            if (controller.GetLockCameraTimer() < lockCameraTimeout)
            {
                targetAngle = currentAngle;
            }

       	   if (AngleDistance(currentAngle, targetAngle) > 160 && controller.IsMovingBackwards()) targetAngle += 180;

            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, angularSmoothLag, angularMaxSpeed);
        }

        if (controller.IsJumping())
        {
            float newTargetHeight = targetCenter.y + height;
            if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5) targetHeight = targetCenter.y + height;
        }
        else
        {
            targetHeight = targetCenter.y + height;
        }

        currentHeight = cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);

        currentRotation = Quaternion.Euler(0, currentAngle, 0);

		cameraTransform.position = targetCenter;
        cameraTransform.position += currentRotation * Vector3.back * distance;

        camHeight.x = cameraTransform.position.x;
        camHeight.y = currentHeight; 
        camHeight.z = cameraTransform.position.z;
        cameraTransform.position = camHeight;
            
        SetUpRotation(targetCenter, targetHead);
    }

    Vector3 camHeight = new Vector3();

    float currentHeight; Quaternion currentRotation;

    void LateUpdate()
    {
        Apply(transform, Vector3.zero);
    }

    void Cut(Transform dummyTarget, Vector3 dummyCenter)
    {
        float oldHeightSmooth = heightSmoothLag;
        float oldSnapMaxSpeed = snapMaxSpeed;
        float oldSnapSmooth = snapSmoothLag;

        snapMaxSpeed = 10000;
        snapSmoothLag = 0.001f;
        heightSmoothLag = 0.001f;

        snap = true;
        Apply(transform, Vector3.zero);

        heightSmoothLag = oldHeightSmooth;
        snapMaxSpeed = oldSnapMaxSpeed;
        snapSmoothLag = oldSnapSmooth;
    }

    void SetUpRotation(Vector3 centerPos, Vector3 headPos)
    {
		Vector3 cameraPos = cameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;

        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

        Ray centerRay = cameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        Ray topRay = cameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1));

        Vector3 centerRayPos = centerRay.GetPoint(distance);
        Vector3 topRayPos = topRay.GetPoint(distance);

        float centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);

        float heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

        float extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
        if (extraLookAngle < centerToTopAngle)
        {
            extraLookAngle = 0;
        }
        else
        {
            extraLookAngle = extraLookAngle - centerToTopAngle;
            cameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
        }
    }

    Vector3 GetCenterOffset()
    {
        return centerOffset;
    }
}