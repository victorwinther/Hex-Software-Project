using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_controller : MonoBehaviour

{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;

    [SerializeField]
    private SpriteRenderer mapRenderer;
    
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    private Vector3 dragOrigin;

    private void Awake()
    {
       mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
       mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

       mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
       mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            zoomStep = 0.1f;
            ZoomIn();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            zoomStep = 0.1f;
            ZoomOut();
        }
        PanCamera();
        CalculateMaxZoom();
    }
    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + difference);

     
        }
    }
    public void ZoomInButton()
    {
        zoomStep = 1;
        ZoomIn();
    }
    public void ZoomOutButton()
    {
        zoomStep = 1;
        ZoomOut();
    }
    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);

    }
    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);

    }
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
    private void CalculateMaxZoom()
    {
        //calculate current screen ratio
        float w = Screen.width / mapRenderer.bounds.size.x;
        float h = Screen.height / mapRenderer.bounds.size.y;
        float ratio = w / h;
        float ratio2 = h / w;
        if (ratio2 > ratio)
        {
            maxCamSize = (mapRenderer.bounds.size.y / 2);
        }
        else
        {
            maxCamSize = (mapRenderer.bounds.size.y / 2);
            maxCamSize /= ratio;
        }
    }




}
