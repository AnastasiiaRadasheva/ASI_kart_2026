using UnityEditor;
using UnityEngine;

namespace Sain.TougeRacer.Editor
{
    [CustomEditor(typeof(CarController))]
    public class CarControllerEditor : UnityEditor.Editor
    {
        private static bool reference;
        private static bool suspension;
        private static bool carSettings;
        private static bool gearSettings;
        private static bool visuals;
        private static bool ghostmode;
        private static bool debug;

        private GUIStyle titleStyle;
        private GUIStyle foldoutStyle;
        private GUIStyle labelStyle;

        // Reference
        private SerializedProperty rayPoints;
        private SerializedProperty groundLayer;
        private SerializedProperty accelPoint;

        // Suspension Settings
        private SerializedProperty springStiffness;
        private SerializedProperty damperStiffness;
        private SerializedProperty restLength;
        private SerializedProperty springTravel;
        private SerializedProperty wheelRadius;
        private SerializedProperty frontWheelPosZ;
        private SerializedProperty frontWheelPosY;
        private SerializedProperty frontWheelPosX;
        private SerializedProperty rearWheelPosZ;
        private SerializedProperty rearWheelPosY;
        private SerializedProperty rearWheelPosX;

        // Car Dynamics Settings
        private SerializedProperty acceleration;
        private SerializedProperty maxSpeed;
        private SerializedProperty reverseSpeed;
        private SerializedProperty deceleration;
        private SerializedProperty steerStrength;
        private SerializedProperty turningCurve;
        private SerializedProperty dragCoefficient;
        private SerializedProperty brakingDeceleration;
        private SerializedProperty brakingDragCoefficient;

        // Gear System Settings
        private SerializedProperty gearNum;
        private SerializedProperty revRangeBoundary;
        private SerializedProperty engineBraking;

        // Visuals
        private SerializedProperty tires;
        private SerializedProperty frontTireParent;
        private SerializedProperty maxSteerAngle;
        private SerializedProperty steeringWheel;
        private SerializedProperty maxSteerWheelAngle;
        private SerializedProperty skidmarks;
        private SerializedProperty smokes;
        private SerializedProperty minSideSkidVel;

        // Ghost Mode Settings
        private SerializedProperty normalLayer;
        private SerializedProperty ghostLayer;

        private static Tab currentTab = Tab.Reference;

        private enum Tab
        {
            Reference,
            Suspension,
            CarSettings,
            Visuals,
        }

        private void OnEnable()
        {
            // FindProperty может вернуть null, если поле переименовали/удалили.
            // Поэтому просто находим всё, а в GUI будем рисовать только то, что найдено.
            rayPoints = serializedObject.FindProperty("rayPoints");
            groundLayer = serializedObject.FindProperty("groundLayer");
            accelPoint = serializedObject.FindProperty("accelPoint");

            springStiffness = serializedObject.FindProperty("springStiffness");
            damperStiffness = serializedObject.FindProperty("damperStiffness");
            restLength = serializedObject.FindProperty("restLength");
            springTravel = serializedObject.FindProperty("springTravel");
            wheelRadius = serializedObject.FindProperty("wheelRadius");

            frontWheelPosZ = serializedObject.FindProperty("frontWheelPosZ");
            frontWheelPosY = serializedObject.FindProperty("frontWheelPosY");
            frontWheelPosX = serializedObject.FindProperty("frontWheelPosX");

            rearWheelPosZ = serializedObject.FindProperty("rearWheelPosZ");
            rearWheelPosY = serializedObject.FindProperty("rearWheelPosY");
            rearWheelPosX = serializedObject.FindProperty("rearWheelPosX");

            acceleration = serializedObject.FindProperty("acceleration");
            maxSpeed = serializedObject.FindProperty("maxSpeed");
            reverseSpeed = serializedObject.FindProperty("reverseSpeed");
            deceleration = serializedObject.FindProperty("deceleration");
            steerStrength = serializedObject.FindProperty("steerStrength");
            turningCurve = serializedObject.FindProperty("turningCurve");
            dragCoefficient = serializedObject.FindProperty("dragCoefficient");
            brakingDeceleration = serializedObject.FindProperty("brakingDeceleration");
            brakingDragCoefficient = serializedObject.FindProperty("brakingDragCoefficient");

            gearNum = serializedObject.FindProperty("gearNum");
            revRangeBoundary = serializedObject.FindProperty("revRangeBoundary");
            engineBraking = serializedObject.FindProperty("engineBraking");

            tires = serializedObject.FindProperty("tires");
            frontTireParent = serializedObject.FindProperty("frontTireParent");
            maxSteerAngle = serializedObject.FindProperty("maxSteerAngle");
            steeringWheel = serializedObject.FindProperty("steeringWheel");
            maxSteerWheelAngle = serializedObject.FindProperty("maxSteerWheelAngle");
            skidmarks = serializedObject.FindProperty("skidmarks");
            smokes = serializedObject.FindProperty("smokes");
            minSideSkidVel = serializedObject.FindProperty("minSideSkidVel");

            normalLayer = serializedObject.FindProperty("normalLayer");
            ghostLayer = serializedObject.FindProperty("ghostLayer");
        }

        public override void OnInspectorGUI()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };

            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13
            };

            EditorGUILayout.LabelField(new GUIContent("Car Controller"), titleStyle);
            EditorGUILayout.Separator();

            serializedObject.Update();

            DrawTabSettings();
            DrawDebug();

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            var controller = (CarController)target;
            if (controller == null) return;

            // Если точки не назначены — просто ничего не делаем (иначе Unity будет падать)
            if (controller.RayPoints == null || controller.RayPoints.Length < 4) return;
            if (controller.Tires == null || controller.Tires.Length < 4) return;

            for (int i = 0; i < 4; i++)
            {
                if (controller.RayPoints[i] == null) return;
                if (controller.Tires[i] == null) return;
            }

            // Если properties не найдены (поле переименовали) — тоже выходим
            if (frontWheelPosX == null || frontWheelPosY == null || frontWheelPosZ == null ||
                rearWheelPosX == null || rearWheelPosY == null || rearWheelPosZ == null ||
                wheelRadius == null)
                return;

            Undo.RecordObject(controller, "CarController Scene Edit");

            controller.RayPoints[0].localPosition = new Vector3(frontWheelPosX.floatValue,  frontWheelPosY.floatValue,  frontWheelPosZ.floatValue);
            controller.RayPoints[1].localPosition = new Vector3(-frontWheelPosX.floatValue, frontWheelPosY.floatValue,  frontWheelPosZ.floatValue);
            controller.RayPoints[2].localPosition = new Vector3(rearWheelPosX.floatValue,   rearWheelPosY.floatValue, -rearWheelPosZ.floatValue);
            controller.RayPoints[3].localPosition = new Vector3(-rearWheelPosX.floatValue,  rearWheelPosY.floatValue, -rearWheelPosZ.floatValue);

            for (int i = 0; i < 4; i++)
            {
                var tire = controller.Tires[i];
                var rayPoint = controller.RayPoints[i];

                if (tire == null || rayPoint == null) continue;

                var tireParent = tire.transform.parent;
                if (tireParent == null) continue;

                float tireY = rayPoint.localPosition.y - (0.75f - wheelRadius.floatValue);
                tireParent.localPosition = new Vector3(rayPoint.localPosition.x, tireY, rayPoint.localPosition.z);

                Handles.color = Color.green;
                Handles.DrawWireDisc(tire.transform.position, tire.transform.right, wheelRadius.floatValue);

                Handles.color = Color.red;
                Handles.DrawLine(rayPoint.position, tire.transform.position);
            }
        }

        private void DrawTabSettings()
        {
            currentTab = (Tab)GUILayout.Toolbar((int)currentTab,
                new string[] { "Reference", "Suspension", "Car", "Visuals" });

            switch (currentTab)
            {
                case Tab.Reference:
                    DrawReference();
                    break;
                case Tab.Suspension:
                    DrawSuspensionSettings();
                    break;
                case Tab.CarSettings:
                    DrawCarSettings();
                    DrawGearSettings();
                    break;
                case Tab.Visuals:
                    DrawVisualSettings();
                    DrawGhostmodeSettings();
                    break;
            }
        }

        private void DrawDebug()
        {
            var controller = (CarController)target;

            EditorGUILayout.Separator();
            debug = EditorGUILayout.Foldout(debug, new GUIContent("Debug"), true, foldoutStyle);

            if (!debug || controller == null) return;

            EditorGUILayout.LabelField("Current Speed (kph): ", (controller.CurrentSpeed * 3.6f).ToString("F2"));
            EditorGUILayout.LabelField("Current Gear: ", (controller.CurrentGear + 1).ToString());

            string revs = string.Empty;
            for (int i = 0; i < Mathf.RoundToInt(controller.Revs * 30); i++)
                revs += "|";

            EditorGUILayout.LabelField("Revs: ", revs);
        }

        private void DrawReference()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Reference"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(rayPoints);
            DrawProp(groundLayer);
            DrawProp(accelPoint);
        }

        private void DrawSuspensionSettings()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Suspension Settings"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(springStiffness);
            DrawProp(damperStiffness);
            DrawProp(restLength);
            DrawProp(springTravel);
            DrawProp(wheelRadius);

            DrawProp(frontWheelPosX, new GUIContent("Front Suspension X"));
            DrawProp(frontWheelPosY, new GUIContent("Front Suspension Y"));
            DrawProp(frontWheelPosZ, new GUIContent("Front Suspension Z"));

            DrawProp(rearWheelPosX, new GUIContent("Rear Suspension X"));
            DrawProp(rearWheelPosY, new GUIContent("Rear Suspension Y"));
            DrawProp(rearWheelPosZ, new GUIContent("Rear Suspension Z"));
        }

        private void DrawCarSettings()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Car Settings"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(acceleration);
            DrawProp(maxSpeed);
            DrawProp(reverseSpeed);
            DrawProp(deceleration);
            DrawProp(steerStrength);
            DrawProp(turningCurve);
            DrawProp(dragCoefficient);
            DrawProp(brakingDeceleration);
            DrawProp(brakingDragCoefficient);
        }

        private void DrawGearSettings()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Gear Settings"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(gearNum, new GUIContent("Num. of Gears"));
            DrawProp(revRangeBoundary);
            DrawProp(engineBraking);
        }

        private void DrawVisualSettings()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Visuals Settings"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(tires);
            DrawProp(frontTireParent);
            DrawProp(maxSteerAngle);
            DrawProp(steeringWheel);
            DrawProp(maxSteerWheelAngle);
            DrawProp(skidmarks);
            DrawProp(smokes);
            DrawProp(minSideSkidVel);
        }

        private void DrawGhostmodeSettings()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("Ghostmode"), labelStyle);
            EditorGUILayout.Separator();

            DrawProp(normalLayer);
            DrawProp(ghostLayer);
        }

        private static void DrawProp(SerializedProperty prop, GUIContent label = null)
        {
            if (prop == null)
            {
                EditorGUILayout.HelpBox("Property not found (field renamed/removed).", MessageType.Warning);
                return;
            }

            if (label == null) EditorGUILayout.PropertyField(prop);
            else EditorGUILayout.PropertyField(prop, label);
        }
    }
}
