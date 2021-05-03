using UnityEngine;
using System.Collections.Generic;

namespace LongviewEditor.Main.Cameras.ControlSystems
{
    public class DebugCameraControls : MonoBehaviour
    {
        private const string ORBIT_CONTAINER_NAME = "OrbitContainer";
        private const float MOVE_SPEED = 0.1f;
        private const float MOVE_SPEED_MULT = 3f;
        private const float CAM_LOOK_MULT = 0.25f;

        public Camera Cam;
        public float ScaleMultiplier;

        private bool isDragging = false;
        private bool isDraggingOrbit = false;
        private bool isOrbiting = false; 
        private float speedMult = 1f;
        private Vector3 oldMousePos;
        private Transform camTransform;
        private Transform orbitContainer;

		private float moveSpeed;
		private float moveSpeedMult;
		private bool isFarCam;

        public void Start()
        {
			camTransform = Cam.transform;

			moveSpeed = MOVE_SPEED * ScaleMultiplier;
			moveSpeedMult = MOVE_SPEED_MULT;
			isFarCam = ScaleMultiplier < 1f;
        }

        public void Update()
        {
            float dt = Time.deltaTime;
            if (Input.GetKey(KeyCode.W) && isDragging || isDraggingOrbit)
            {
                Vector3 fwdSpeed = new Vector3(0f, 0f, moveSpeed * speedMult);
                camTransform.Translate(fwdSpeed);
            }

            if (Input.GetKey(KeyCode.A) && isDragging)
            {
                Vector3 fwdSpeed = new Vector3(-moveSpeed * speedMult, 0f, 0f);
                camTransform.Translate(fwdSpeed);
                ClearCamTarget();
            }

            if (Input.GetKey(KeyCode.S) && isDragging || isDraggingOrbit)
            {
                Vector3 fwdSpeed = new Vector3(0f, 0f, -moveSpeed * speedMult);
                camTransform.Translate(fwdSpeed);
            }

            if (Input.GetKey(KeyCode.D) && isDragging)
            {
                Vector3 fwdSpeed = new Vector3(moveSpeed * speedMult, 0f, 0f);
                camTransform.Translate(fwdSpeed);
                ClearCamTarget();
            }

            if (Input.GetKey(KeyCode.Q) && isDragging)
            {
                Vector3 fwdSpeed = new Vector3(0f, -moveSpeed * speedMult, 0f);
                camTransform.Translate(fwdSpeed);
                ClearCamTarget();
            }

            if (Input.GetKey(KeyCode.E) && isDragging)
            {
                Vector3 fwdSpeed = new Vector3(0f, moveSpeed * speedMult, 0f);
                camTransform.Translate(fwdSpeed);
                ClearCamTarget();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedMult = moveSpeedMult;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speedMult = 1f;
            }

            if (isDragging && !isOrbiting)
            {
                ClearCamTarget();

                Vector3 diff = oldMousePos - Input.mousePosition;
                diff *= CAM_LOOK_MULT;
                oldMousePos = Input.mousePosition;
                Vector3 euler = camTransform.eulerAngles;
                if (Mathf.RoundToInt(euler.z) != 180)
                {
                    euler.y -= diff.x;
                }
                else
                {
                    euler.y += diff.x;
                }

                camTransform.eulerAngles = euler;
                camTransform.rotation *= Quaternion.Euler(new Vector3(diff.y, 0f, 0f));
            }

            if (isDraggingOrbit && isOrbiting)
            {
                Vector3 diff = oldMousePos - Input.mousePosition;
                diff *= CAM_LOOK_MULT;
                oldMousePos = Input.mousePosition;
                Vector3 euler = orbitContainer.transform.eulerAngles;
                if (Mathf.RoundToInt(euler.z) != 180)
                {
                    euler.y -= diff.x;
                }
                else
                {
                    euler.y += diff.x;
                }

                orbitContainer.transform.eulerAngles = euler;
                orbitContainer.transform.rotation *= Quaternion.Euler(new Vector3(-diff.y, 0f, 0f));
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDraggingOrbit = false;
                isDragging = true;
                oldMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                isDraggingOrbit = true;
                oldMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isDraggingOrbit = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt) && orbitContainer != null)
            {
                isOrbiting = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                isOrbiting = false;
            }
        }
        
        private void ClearCamTarget()
        {
            if (orbitContainer != null)
            {
                camTransform.SetParent(null);
                GameObject.Destroy(orbitContainer.gameObject);
                orbitContainer = null;
            }
        }
        
        public void SetCamTarget(Transform target)
        {
            GameObject tmpGo = new GameObject();
            tmpGo.name = ORBIT_CONTAINER_NAME;
            orbitContainer = tmpGo.transform;
            orbitContainer.position = target.position;
            orbitContainer.LookAt(camTransform.position);
            camTransform.LookAt(target.position);
            camTransform.SetParent(orbitContainer);
        }

        public void Unload()
        {
            ClearCamTarget();
            camTransform = null;
        }
    }
}
