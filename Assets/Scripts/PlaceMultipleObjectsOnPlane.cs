using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceMutlipleObjectsOnPlane : MonoBehaviour
{
    /// <summary>
    /// The prefab that will be instantiated on touch.
    /// </summary>
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    /// <summary>
    /// Option to make the object look at the camera.
    /// </summary>
    [SerializeField]
    [Tooltip("If true, the instantiated objects will always face towards the current camera position.")]
    bool lookAtCamera;

    /// <summary>
    /// The instantiated object.
    /// </summary>
    GameObject spawnedObject;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    TouchControls controls;

    void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();
        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                OnPress(device.position.ReadValue());
            }
        };
    }

    void OnEnable()
    {
        controls.control.Enable();
    }

    void OnDisable()
    {
        controls.control.Disable();
    }

    void OnPress(Vector3 position)
    {
        if (aRRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = hits[0].pose;

            spawnedObject = Instantiate(placedPrefab, hitPos.position, hitPos.rotation);

            // Make the spawned object always look at the camera.
            if (lookAtCamera)
            {
                Vector3 lookDir = Camera.main.transform.position - spawnedObject.transform.position;
                lookDir.y = 0;
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
}
