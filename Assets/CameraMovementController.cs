using PersonalHelpers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovementController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IScrollHandler
{
    public void SetCtrlDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            Logger.Log("up");
            CrtlDown = false;
        }

        if (context.phase == InputActionPhase.Started)
        {
            Logger.Log("down");
            CrtlDown = true;
        }

    }

    [SerializeField] private Camera MainCamera = new();
    [SerializeField] private SpecLog Logger = new();
    [SerializeField] private float ScrollRate = 1;
    public static bool CrtlDown;


   Vector2 StartMousePos = Vector2.zero;
    Vector2 CurrentMousePos = Vector2.zero;

    Vector2 StartCameraPos = Vector2.zero;

    bool Panning = false;


    void StartPanning(PointerEventData eventData)
    {
        StartMousePos = eventData.position;
        StartCameraPos = MainCamera.transform.position;





    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Panning = true;

            StartPanning(eventData);
        }



        Logger.Log("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Panning = false;

        }
        Logger.Log("Up");
    }

    private void Update()
    {
        if (Panning)
        {
            Vector3 newCameraPos = StartCameraPos + (StartMousePos - CurrentMousePos) * (MainCamera.orthographicSize / Screen.height * 2f) ;

            newCameraPos.z = MainCamera.transform.position.z;



            MainCamera.transform.position = newCameraPos;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        CurrentMousePos = eventData.position;
    }

    public void OnScroll(PointerEventData eventData)
    {

        if (CrtlDown)
        {
            MainCamera.orthographicSize -= eventData.scrollDelta.y * ScrollRate * 4;
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 5, 1000);
        }
        else
        {
            MainCamera.orthographicSize -= eventData.scrollDelta.y * ScrollRate;
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 5, 1000);
        }


    }
}
