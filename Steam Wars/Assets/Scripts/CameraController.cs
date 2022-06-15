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

    bool moved;

    public Vector3 newPos;
    Quaternion newRot;
    Vector3 newZoom;

    Vector3 dragStartPos;
    Vector3 dragCurrentPos;
    Vector3 rotStartPos;
    Vector3 rotCurrentPos;

    private void Awake()
    {
        Instance = this;
        moved = false;
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

            if (followTransform.GetComponent<Unit>().hasShot)
            {
                followTransform = null;
                
            }

            transform.position = followTransform.position;
            followTransform.GetComponent<Unit>().isSelected = true;

            moved = true;
        }
        else
        {
            moved = false;
            MovementInput();
            MouseInput();
        }

        Zoom();

        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
            moved = false;
        }

        if (!moved)
        {
            dragStartPos = Vector3.zero;
            dragCurrentPos = Vector3.zero;
            newPos = Vector3.zero;
            moved = true;
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

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * movementTime);

        

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
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }

        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        if(newZoom.y > 50)
        {
            newZoom.y = 50;
        }

        if (newZoom.z < -50)
        {
            newZoom.z = -50;
        }

        if (newZoom.y < 3)
        {
            newZoom.y = 3;
        }

        if (newZoom.z > -3)
        {
            newZoom.z = -3;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
