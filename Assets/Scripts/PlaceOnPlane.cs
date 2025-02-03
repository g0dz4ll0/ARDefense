using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

/// <summary>
/// How to use this script:
/// - Add the ARPlaneManager component to the XROrigin GameObject.
/// - Add the ARRaycastManager component to the XOrigin GameObject.
/// - Attach this script to the XOrigin GameObject.
/// - Add the prefab that will be spawned to the <see cref="placedPrefab"/>
/// - Create a new input system called TouchControls that has the <Pointer>/press as the binding.
/// 
/// Touch to place the <see cref="placedPrefab"/> object on the touch position.
/// Will only place the object if the touch position is on detected trackables.
/// Move the existing spawned object on the touch position.
/// Using the Unity new input system.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
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

    /// <summary>
    /// The input touch control.
    /// </summary>
    TouchControls controls;

    /// <summary>
    /// If there is any touch input.
    /// </summary>
    bool isPressed;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();
        // If there is existing touch input, change the isPressed to true.
        // If the touch input is stopped, change the isPressed to false.
        controls.control.touch.performed += _ => isPressed = true;
        controls.control.touch.canceled += _ => isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is any pointer device connected to the system.
        // Or if there is existing touch input.
        if (Pointer.current == null || !isPressed)
            return;

        // Store the current touch position
        var touchPosition = Pointer.current.position.ReadValue();

        // Check if the raycast hits any trackables.
        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means it's the closest.
            var hitPos = hits[0].pose;

            // Check if there is a spawned object already. If there is none, instantiate the prefab.
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPos.position, hitPos.rotation);
            }
            else
            {
                // Change the spawned object position and rotation to the touch position.
                spawnedObject.transform.position = hitPos.position;
                spawnedObject.transform.rotation = hitPos.rotation;
            }

            // Make the spawned object always look at the camera.
            if (lookAtCamera)
            {
                Vector3 lookDir = Camera.main.transform.position - spawnedObject.transform.position;
                lookDir.y = 0;
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    void OnEnable()
    {
        controls.control.Enable();
    }

    void OnDisable()
    {
        controls.control.Disable();
    }
}
