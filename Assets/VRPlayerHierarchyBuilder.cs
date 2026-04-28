using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem.XR;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public static class VRPlayerHierarchyBuilder
{
    [MenuItem("Tools/Create VR Player Hierarchy")]
    public static void CreateVRPlayerHierarchy()
    {
        //==================================================
        // Root
        //==================================================
        GameObject root = new GameObject("PlayerRoot");
        Undo.RegisterCreatedObjectUndo(root, "Create PlayerRoot");

        root.transform.position = Vector3.zero;

        root.AddComponent<CapsuleCollider>();

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        root.AddComponent<Test>();

        //==================================================
        // XR Origin
        //==================================================
        GameObject xrOriginObj = new GameObject("XR Origin");
        Undo.RegisterCreatedObjectUndo(xrOriginObj, "Create XR Origin");

        xrOriginObj.transform.SetParent(root.transform, false);

        XROrigin xrOrigin = xrOriginObj.AddComponent<XROrigin>();

        //==================================================
        // Camera Offset
        //==================================================
        GameObject cameraOffset = new GameObject("Camera Offset");
        Undo.RegisterCreatedObjectUndo(cameraOffset, "Create Camera Offset");

        cameraOffset.transform.SetParent(xrOriginObj.transform, false);

        xrOrigin.CameraFloorOffsetObject = cameraOffset;

        //==================================================
        // Main Camera
        //==================================================
        GameObject mainCamera = new GameObject("Main Camera");
        Undo.RegisterCreatedObjectUndo(mainCamera, "Create Main Camera");

        mainCamera.transform.SetParent(cameraOffset.transform, false);
        mainCamera.tag = "MainCamera";

        Camera cam = mainCamera.AddComponent<Camera>();
        mainCamera.AddComponent<AudioListener>();

        TrackedPoseDriver cameraTPD = mainCamera.AddComponent<TrackedPoseDriver>();
        cameraTPD.positionInput = new InputActionProperty();
        cameraTPD.rotationInput = new InputActionProperty();

        xrOrigin.Camera = cam;

        //==================================================
        // Left Controller Tracker
        //==================================================
        GameObject leftTracker = new GameObject("LeftControllerTracker");
        Undo.RegisterCreatedObjectUndo(leftTracker, "Create Left Controller");

        leftTracker.transform.SetParent(cameraOffset.transform, false);

        TrackedPoseDriver leftTPD = leftTracker.AddComponent<TrackedPoseDriver>();
        leftTPD.positionInput = new InputActionProperty();
        leftTPD.rotationInput = new InputActionProperty();

        GameObject leftIK = new GameObject("LeftHandIKTarget");
        Undo.RegisterCreatedObjectUndo(leftIK, "Create Left IK");

        leftIK.transform.SetParent(leftTracker.transform, false);
        leftIK.transform.localPosition = new Vector3(0f, -0.05f, -0.1f);
        leftIK.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        //==================================================
        // Right Controller Tracker
        //==================================================
        GameObject rightTracker = new GameObject("RightControllerTracker");
        Undo.RegisterCreatedObjectUndo(rightTracker, "Create Right Controller");

        rightTracker.transform.SetParent(cameraOffset.transform, false);

        TrackedPoseDriver rightTPD = rightTracker.AddComponent<TrackedPoseDriver>();
        rightTPD.positionInput = new InputActionProperty();
        rightTPD.rotationInput = new InputActionProperty();

        GameObject rightIK = new GameObject("RightHandIKTarget");
        Undo.RegisterCreatedObjectUndo(rightIK, "Create Right IK");

        rightIK.transform.SetParent(rightTracker.transform, false);
        rightIK.transform.localPosition = new Vector3(0f, -0.05f, -0.1f);
        rightIK.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        //==================================================
        // Character Model
        //==================================================
        GameObject characterModel = new GameObject("CharacterModel");
        Undo.RegisterCreatedObjectUndo(characterModel, "Create Character Model");

        characterModel.transform.SetParent(root.transform, false);

        characterModel.AddComponent<Animator>();
        characterModel.AddComponent<RigBuilder>();

        //==================================================
        // Rig Root
        //==================================================
        GameObject rigRoot = new GameObject("Rig");
        Undo.RegisterCreatedObjectUndo(rigRoot, "Create Rig");

        rigRoot.transform.SetParent(characterModel.transform, false);
        rigRoot.AddComponent<Rig>();

        //==================================================
        // Constraint placeholders
        //==================================================
        CreateChild("HeadConstraint", rigRoot.transform);
        CreateChild("LeftArmIK", rigRoot.transform);
        CreateChild("RightArmIK", rigRoot.transform);

        //==================================================
        // Select
        //==================================================
        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);
    }

    static GameObject CreateChild(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
        obj.transform.SetParent(parent, false);
        return obj;
    }
}