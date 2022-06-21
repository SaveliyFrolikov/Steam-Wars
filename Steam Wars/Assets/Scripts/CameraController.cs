using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public Vector3 minPos;
    public Vector3 maxPos;
    public Vector3 minZoom;
    public Vector3 maxZoom;

    [Space]

    public Transform cameraTransform;
    public Transform followTransform;

    [Space]

    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPos;
    Quaternion newRot;
    [HideInInspector]public Vector3 newZoom;

    Vector3 dragStartPos;
    Vector3 dragCurrentPos;
    Vector3 rotStartPos;
    Vector3 rotCurrentPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        newPos = transform.position;
        newRot = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void Update()
    {
        

        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * movementTime);
        }

        Zoom();

        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    private void MovementInput()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.newPos += transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.newPos += transform.forward * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.newPos += transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.newPos += transform.right * -movementSpeed;
        }

        if(Input.GetKey(KeyCode.Q))
        {
            newRot *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (newPos.x > 26)
        {
            newPos.x = 26;
        }

        if (newPos.x < -26)
        {
            newPos.x = -26;
        }

        if (newPos.z > 26)
        {
            newPos.z = 26;
        }

        if (newPos.z < -26)
        {
            newPos.z = -26;
        }

        

        

        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * movementTime);
    }

    private void MouseInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry))
            {
                dragStartPos = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPos = ray.GetPoint(entry);

                newPos = transform.position + dragStartPos - dragCurrentPos;
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            rotStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            rotCurrentPos = Input.mousePosition;

            Vector3 difference = rotStartPos - rotCurrentPos;

            rotStartPos = rotCurrentPos;

            newRot *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
        }
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom.y += zoomAmount.y * 1.5f * Input.mouseScrollDelta.y;
            newZoom.z += zoomAmount.z * Input.mouseScrollDelta.y;
        }

        if (Input.GetKey(KeyCode.R))
        {
            //newZoom += zoomAmount;
            newZoom.y += zoomAmount.y * 1.5f;
            newZoom.z += zoomAmount.z;
        }

        if (Input.GetKey(KeyCode.F))
        {
            newZoom.y -= zoomAmount.y * 1.5f;
            newZoom.z -= zoomAmount.z;
        }

        if(newZoom.y > 43.5)
        {
            newZoom.y = 43.5f;
        }

        if (newZoom.z < -29)
        {
            newZoom.z = -29;
        }

        if (newZoom.y < 3)
        {
            newZoom.y = 3;
        }

        if (newZoom.z > -2)
        {
            newZoom.z = -2;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
