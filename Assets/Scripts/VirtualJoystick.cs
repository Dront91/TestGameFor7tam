using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static VirtualJoystick Instance;
    [SerializeField] private Image _joyBack;
    [SerializeField] private Image _joyStick;
    public Vector3 Value { get; private set; }
    public Vector3 FireDirection { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_joyBack.rectTransform, eventData.position, eventData.pressEventCamera, out position);
        position.x = (position.x / _joyBack.rectTransform.sizeDelta.x);
        position.y = (position.y / _joyBack.rectTransform.sizeDelta.y);
        position.x = position.x * 2 - 1;
        position.y = position.y * 2 - 1;
        Value = new Vector3(position.x, position.y, 0);
        if (Value.magnitude > 1)
            Value = Value.normalized;
        FireDirection = Value;
        float offsetX = _joyBack.rectTransform.sizeDelta.x / 2 - _joyStick.rectTransform.sizeDelta.x / 2;
        float offsetY = _joyBack.rectTransform.sizeDelta.y / 2 - _joyStick.rectTransform.sizeDelta.y / 2;
        _joyStick.rectTransform.anchoredPosition = new Vector2(Value.x * offsetX, Value.y * offsetY);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector3.zero;
        _joyStick.rectTransform.anchoredPosition = Vector3.zero;
    }
}
