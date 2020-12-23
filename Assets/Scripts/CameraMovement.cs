using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Camera whiteCam;
    [SerializeField] private Camera blackCam;
    [SerializeField] private Camera upCam;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToTarget = 10;

    private Vector3 previousPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            cam.transform.position = target.position;
            var angles=cam.transform.rotation.eulerAngles;
            if(angles.x+rotationAroundXAxis>=10 && angles.x+rotationAroundXAxis<=90)
                cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);
            
            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;
        }

        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            cam.transform.position=upCam.transform.position;
            cam.transform.rotation=upCam.transform.rotation;
        }
        else if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            cam.transform.position=whiteCam.transform.position;
            cam.transform.rotation=whiteCam.transform.rotation;
        }
        else if(Input.GetKeyUp(KeyCode.Alpha3))
        {
            cam.transform.position=blackCam.transform.position;
            cam.transform.rotation=blackCam.transform.rotation;
        }
    }
}