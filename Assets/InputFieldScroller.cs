using PersonalHelpers;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class InputFieldScroller : MonoBehaviour, IScrollHandler
{
   [SerializeField] private bool Bounded;
    [SerializeField] private float Min;
    [SerializeField] private float Max;
    public bool Exponential = true;
    public bool Integer = false;
    public bool Round = false;
    public float BaseRoundStep = 1;
    public float CtrlMult = 10f;
    public TMP_InputField InputField;

    private void OnEnable()
    {
        TryGetComponent<TMP_InputField>(out InputField);
       
    }

    public void OnScroll(PointerEventData eventData)
    {
        float roundStep = BaseRoundStep;


        if (Integer)
        {

            //to indicate that making it an integer forces it to round

            Round = true;
        }


        


        if (InputField == null)
        {
            return;
        }

        if (float.TryParse(InputField.text, out float value))
        {
            Debug.Log(value);

            if (Exponential)
            {
                roundStep = Mathf.Log10(value / 2f);

              //  Debug.Log(roundStep);

                roundStep = Mathf.Max(Mathf.Floor(roundStep),1);

              //  Debug.Log(roundStep);

                roundStep = Mathf.Pow(10f,roundStep) * BaseRoundStep;

                if (CameraMovementController.CrtlDown)
                {
                    roundStep *= CtrlMult;
                }



            //    Debug.Log(roundStep);



            }

            if (float.IsNaN(value))
            {
                return;
            }

            float ScrollAmount = Mathf.Sign(eventData.scrollDelta.y) * roundStep;

            if (Round)
            {

                if (ScrollAmount > 0)
                {
                    ScrollAmount = Mathf.Max(ScrollAmount, roundStep);
                }
                else
                {
                    ScrollAmount = Mathf.Min(ScrollAmount, -roundStep);

                }

            }




            value += ScrollAmount;

            if (Integer)
            {
                if (Mathf.Sign(eventData.scrollDelta.y) == -1)
                {
                    value = Mathf.FloorToInt(value);
                }
                else
                {
                    value = Mathf.CeilToInt(value);


                }


            }
            else if (Round && Mathf.Abs(roundStep) > 0)
            {
                if (Mathf.Sign(eventData.scrollDelta.y) == -1)
                {
                    value = Mathf.Floor(value / roundStep) * roundStep;
                }
                else
                {
                    value = Mathf.Ceil(value / roundStep) * roundStep;

                }
            }
        }

        if (Bounded)
        {
            value = Mathf.Clamp(value,Min,Max);
        }

        InputField.text = value.ToString();

        InputField.onEndEdit.Invoke("");

    }

}
