using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class MobileControl : MonoBehaviour
{
    [Header("摇杆 UI")]
    public RectTransform joystickBg;
    public RectTransform joystickHandle;

    [Header("移动")]
    public float moveSpeed = 5f;
    public float moveSmooth = 12f;

    [Header("转向")]
    public float lookSensitivity = 2.2f;
    public float lookSmooth = 10f;
    public float minPitch = -60f;
    public float maxPitch = 60f;

    [Header("摇杆限制")]
    public float joystickRadius = 60f;

    private CharacterController cc;
    private Transform mainCam;

    private Vector2 moveInput;
    private Vector2 smoothMove;

    private Vector2 lookInput;
    private Vector2 smoothLook;
    private float pitch;

    private int moveFingerId = -1;
    private Vector2 fingerStartPos;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        HandleJoystickInput();
        HandleLookInput();
        Move();
        Look();
    }

    void HandleJoystickInput()
    {
        moveInput = Vector2.zero;

        foreach (var touch in Input.touches)
        {
         
            bool inJoystick = RectTransformUtility.RectangleContainsScreenPoint(
                joystickBg, touch.position, null);

            if (touch.phase == TouchPhase.Began)
            {
                if (inJoystick && moveFingerId == -1)
                {
                    moveFingerId = touch.fingerId;
                    fingerStartPos = touch.position;
                }
            }

            if (touch.fingerId == moveFingerId)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Vector2 dir = touch.position - fingerStartPos;
                    dir = Vector2.ClampMagnitude(dir, joystickRadius);
                    moveInput = dir / joystickRadius;

                    // 摇杆跟随
                    joystickHandle.anchoredPosition = dir;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    moveFingerId = -1;
                    moveInput = Vector2.zero;
                    joystickHandle.anchoredPosition = Vector2.zero;
                }
            }
        }
    }

    // 右屏转向
    void HandleLookInput()
    {
        lookInput = Vector2.zero;

        foreach (var touch in Input.touches)
        {
            bool inJoystick = RectTransformUtility.RectangleContainsScreenPoint(
                joystickBg, touch.position, null);

            if (!inJoystick)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    lookInput = touch.deltaPosition;
                }
            }
        }
    }

    // 平滑移动
    void Move()
    {
        smoothMove = Vector2.Lerp(smoothMove, moveInput, Time.deltaTime * moveSmooth);

        Vector3 f = mainCam.forward;
        Vector3 r = mainCam.right;
        f.y = 0; r.y = 0;
        f.Normalize(); r.Normalize();

        Vector3 dir = f * smoothMove.y + r * smoothMove.x;
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }

    // 平滑转向
    void Look()
    {
        smoothLook = Vector2.Lerp(smoothLook, lookInput, Time.deltaTime * lookSmooth);

        // 左右转角色
        transform.Rotate(0, smoothLook.x * lookSensitivity * Time.deltaTime, 0);

        // 上下转相机
        pitch -= smoothLook.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        mainCam.localEulerAngles = new Vector3(pitch, 0, 0);
    }
}