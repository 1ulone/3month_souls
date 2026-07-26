using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using wine.util.ui;
using wine.core;

namespace wine.cutscene 
{
    public enum actionType
    {
        // moveRigidbodies,
        moveObjects,
        dialog,
        cameraMove,
        cameraRotate,
        resetCamera,
        fadeIn,
        fadeOut,
        customAction,
    }

    [System.Serializable]
    public class BaseAction 
    {
        public float startDelay = 0.15f;
        public float endDelay = 0.15f;

        public bool actionCompleted { get; set; }
        public IEnumerator coroutine { get { return _coroutine(); } } 

        public actionType type;

        /* NOTE: objects variable */
        public string targetObject;
        public float targetMoveSpeed;
        public Vector3 relativeDestination;

        /* NOTE: dialog variable */
        public DialogData dialog;

        /* NOTE: camera variable */
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        public float cameraSpeed; 

        /* NOTE: custom action variable */
        public int customID; 
        public float customEstimatedTime;
        public Action customAction;

        private IEnumerator _coroutine() {
            yield return new WaitForSecondsRealtime(startDelay);

            switch(type)
            {
                // case actionType.moveRigidbodies: {
                //     GameObject targetObj = GameObject.Find(targetObject);
                //     Vector3 destination = targetObj.transform.position + relativeDestination;
                //     if (targetObj.TryGetComponent<CharacterController>(out CharacterController cc))
                //         cc.Move(destination);
                //
                //     if (targetObj.TryGetComponent<NavMeshAgent>(out NavMeshAgent nma))
                //         nma.Move(destination);
                //
                //     if (targetObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                //     {
                //
                //         rb.linearVelocity = 
                //     }
                // } break;
                case actionType.moveObjects: {
                    GameObject targetObj = GameObject.Find(targetObject);
                    Vector3 destination = targetObj.transform.position + relativeDestination;
                    LeanTween.move(targetObj, destination, targetMoveSpeed).setOnComplete(()=> { actionCompleted = true; }).setIgnoreTimeScale(true);
                } break;
                case actionType.dialog: {
                    // yield return DialogController.instances.EnterDialogScene(dialog);
                    actionCompleted = true;
                } break;
                case actionType.cameraMove: {
                    // CameraController.instances.isOnCutscene = true;
                    GameObject cam = CameraController.instances.gameObject;
                    LeanTween.move(cam.gameObject, cam.transform.position + cameraPosition, cameraSpeed).setOnComplete(()=> { actionCompleted = true; } ).setIgnoreTimeScale(true);
                } break; 
                case actionType.cameraRotate: {
                    // CameraController.instances.isOnCutscene = true;
                    GameObject cam = CameraController.instances.gameObject;
                    LeanTween.rotate(cam.gameObject, cam.transform.rotation.eulerAngles + cameraRotation, cameraSpeed).setOnComplete(()=> { actionCompleted = true; } ).setIgnoreTimeScale(true);
                } break; 
                case actionType.resetCamera: {
                    CameraController camcon = CameraController.instances;
                    GameObject cam = camcon.gameObject;
                    bool isResettingRotation = false;

                    // if (camcon.RotationIsNotDefault()) 
                    // {
                    //     isResettingRotation = true;
                    //
                    //     LeanTween.rotate(cam.gameObject, camcon.defaultRotation.eulerAngles, 1).setOnComplete(()=> 
                    //     { isResettingRotation = false; }).setIgnoreTimeScale(true);
                    //
                    //     yield return new WaitUntil(()=> isResettingRotation == false);
                    // } 

                    // bool isResettingMovement = false;
                    // if (camcon.PositionIsNotDefault())
                    // {
                    //     isResettingMovement = true;
                    //
                    //     LeanTween.move(cam.gameObject, camcon.targetPosition, 1).setOnComplete(()=> 
                    //     { isResettingMovement = false; }).setIgnoreTimeScale(true);
                    //
                    //     yield return new WaitUntil(()=> isResettingMovement == false);
                    // }
                    //
                    // camcon.isOnCutscene = false;
                    // actionCompleted = true;
                } break;
                case actionType.fadeIn: {
                    yield return FadeTransitionUI.instances.FadeInOut(true);
                    actionCompleted = true;
                } break;
                case actionType.fadeOut: {
                    yield return FadeTransitionUI.instances.FadeInOut(false);
                    actionCompleted = true;
                } break;
                case actionType.customAction: {
                    customAction.Invoke();
                    yield return new WaitForSecondsRealtime(customEstimatedTime);
                    actionCompleted = true;
                } break;
            }
            yield return new WaitUntil(() => actionCompleted == true);

            yield return new WaitForSecondsRealtime(endDelay);
        }
    }

    [CustomPropertyDrawer(typeof (BaseAction)), CanEditMultipleObjects]
    public class BaseActionDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty start_delay = property.FindPropertyRelative("startDelay");
            SerializedProperty end_delay = property.FindPropertyRelative("endDelay");

            SerializedProperty type_prop = property.FindPropertyRelative("type");

            SerializedProperty target_object = property.FindPropertyRelative("targetObject");
            SerializedProperty target_move_speed = property.FindPropertyRelative("targetMoveSpeed");
            SerializedProperty target_destination = property.FindPropertyRelative("relativeDestination");

            SerializedProperty dialog = property.FindPropertyRelative("dialog");

            SerializedProperty camera_position = property.FindPropertyRelative("cameraPosition");
            SerializedProperty camera_rotation = property.FindPropertyRelative("cameraRotation");
            SerializedProperty camera_speed = property.FindPropertyRelative("cameraSpeed");

            SerializedProperty custom_id = property.FindPropertyRelative("customID");
            SerializedProperty custom_estimated_time = property.FindPropertyRelative("customEstimatedTime");

            float y = position.y;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), start_delay, new GUIContent("Start Delay"));
            y += lineHeight;

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), type_prop);
            y += lineHeight;

            actionType type = (actionType)type_prop.enumValueIndex;
            switch(type)
            {
                case actionType.moveObjects:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), target_object, new GUIContent("Target Object"));
                        y += lineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), target_move_speed, new GUIContent("Target Move Speed"));
                        y += lineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), target_destination, new GUIContent("Relative Destination"));
                        y += lineHeight;
                    } break;
                case actionType.dialog:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), dialog, new GUIContent("Dialog"));
                        y += lineHeight;
                    } break;
                case actionType.cameraMove:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), camera_position, new GUIContent("Camera New Position"));
                        y += lineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), camera_speed, new GUIContent("Camera Speed"));
                        y += lineHeight;
                    } break;
                case actionType.cameraRotate:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), camera_rotation, new GUIContent("Camera New Rotation"));
                        y += lineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), camera_speed, new GUIContent("Camera Speed"));
                        y += lineHeight;
                    } break;
                case actionType.customAction:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), custom_id, new GUIContent("Custom ID"));
                        y += lineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), custom_estimated_time, new GUIContent("Custom Est Time"));
                        y += lineHeight;
                    } break;
            }

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), end_delay, new GUIContent("End Delay"));
            y += lineHeight;


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProp = property.FindPropertyRelative("type");
            actionType type = (actionType)typeProp.enumValueIndex;

            int lines = 3; // type line
            switch(type)
            {
                case actionType.moveObjects: lines += 3; break;
                case actionType.dialog: lines += 1; break;
                case actionType.cameraMove: lines += 2; break;
                case actionType.cameraRotate: lines += 2; break;
                case actionType.customAction: lines += 2; break;
            }

            return lines * (EditorGUIUtility.singleLineHeight + 2);
        }
    }
}
