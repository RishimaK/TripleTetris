using System;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // public BlockCreator.BlockType BlockType { get; set; }
    private Vector2 FirstBlockPosition;
    private Vector2 LimitHeightTouch;
    private float blockSize;
    private Camera mainCamera;

    public bool enabledTouch = false;

    public bool allowMoveDown = false;
    [SerializeField] private BlockCreator blockCreator;


    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Vector2 firstBlockPosition, float size, float MapFrameY)
    {
        FirstBlockPosition = firstBlockPosition;
        LimitHeightTouch = new Vector2(-FirstBlockPosition.y + MapFrameY * 2 + size / 2, firstBlockPosition.y - size / 2);
        // blockSize = GetMeshSize(transform.GetChild(0)).x;
        blockSize = size;
    }

    // Vector3 GetMeshSize(Transform block)
    // {
    //     MeshFilter meshFilter = block.GetComponent<MeshFilter>();
    //     if (meshFilter != null && meshFilter.mesh != null)
    //     {
    //         // Lấy kích thước nguyên bản của mesh
    //         Vector3 size = meshFilter.mesh.bounds.size;
    //         // Debug.Log("Kích thước mesh: " + size);

    //         // Kích thước thực tế với scale
    //         Vector3 scaledSize = Vector3.Scale(size, block.transform.localScale);
    //         // Debug.Log("Kích thước thực tế: " + scaledSize);
    //         return scaledSize;
    //     }
    //     return Vector3.zero;
    // }

    public void StartMoveDown()
    {
        speed = 5;
        allowMoveDown = true;
        enabledTouch = true;
    }

    public void StopAllAction()
    {
        allowMoveDown = false;
        enabledTouch = false;
    }

    // Xoay khối 90 độ theo trục Y
    public void RotateBlock()
    {
        float angle = -90f * Mathf.Deg2Rad;
        float cosAngle = Mathf.Cos(angle); // 0 cho 90 độ
        float sinAngle = Mathf.Sin(angle); // 1 cho 90 độ

        List<Vector3> listNewPosition = new List<Vector3>();
        foreach (Transform child in transform)
        {
            // Vị trí hiện tại của con so với cha (local position)
            Vector3 currentLocalPos = child.localPosition;

            // Tính vị trí mới sau khi xoay
            float newX = currentLocalPos.x * cosAngle - currentLocalPos.y * sinAngle;
            float newY = currentLocalPos.x * sinAngle + currentLocalPos.y * cosAngle;

            // Vị trí mới trong không gian cục bộ của cha
            listNewPosition.Add(new Vector3(newX, newY, currentLocalPos.z));
        }

        if (!blockCreator.CheckBlockTouchWhenRotate(listNewPosition, transform)) return;

        for (int i = 0; i < listNewPosition.Count; i++)
        {
            transform.GetChild(i).localPosition = listNewPosition[i];
        }
    }

    bool CheckWallKick(float nextPositionX)
    {
        float min = FirstBlockPosition.x;
        float max = -FirstBlockPosition.x;
        foreach (Transform child in transform)
        {
            float childPositionX = child.position.x + nextPositionX;
            if (Math.Round(childPositionX - min) < 0 ||
                Math.Round(max - childPositionX) < 0) return false;
        }
        return blockCreator.CheckBlockTouch(nextPositionX, transform, -speed * timer);
    }


    private float timer = 0.01f;
    private float speed = 5;
    private float waitTime = 0.1f;

    void CheckBlockMove()
    {
        if (!allowMoveDown) return;
        float distance = -speed * timer;
        float trueDistance = blockCreator.CheckNextPosition(distance);
        if (trueDistance == distance)
        {
            waitTime = 0.1f;
            transform.position += new Vector3(0, trueDistance, 0);
        }
        else
        {
            if (waitTime == 0.1f) transform.position += new Vector3(0, trueDistance, 0);
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                return;
            }
            else
            {
                allowMoveDown = false;
                StopTrailRenderer();
                blockCreator.LockPlayerBlock();
            }
        }
    }

    private Vector2 PastTouch = Vector2.zero;
    private Vector2 CurrentTouch = Vector2.zero;
    private bool TouchMove = false;
    private bool StationaryTouch = false;
    private float countTimetouch = 0;
    void Update()
    {
        CheckBlockMove();
        if (!enabledTouch) return;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;
            CurrentTouch = ConvertPixelToWorldUnit(touch.position);
            if (CurrentTouch.y > LimitHeightTouch.x || CurrentTouch.y < LimitHeightTouch.y) return;
            if (touchPhase == TouchPhase.Began)
            {
                PastTouch = ConvertPixelToWorldUnit(touch.position);
                countTimetouch = 0;
                StationaryTouch = false;
                TouchMove = false;
            }
            else if (touchPhase == TouchPhase.Moved)
            {
                countTimetouch += Time.deltaTime;

                if (Mathf.Abs(PastTouch.x - CurrentTouch.x) >= blockSize)
                {
                    TouchMove = true;
                    speed = 5;
                    if (PastTouch.x > CurrentTouch.x && CheckWallKick(-blockSize))
                    {
                        // move left
                        gameObject.transform.position += new Vector3(-blockSize, 0, 0);
                        PastTouch -= new Vector2(blockSize, 0);
                    }
                    else if (PastTouch.x < CurrentTouch.x && CheckWallKick(blockSize))
                    {
                        // move right
                        gameObject.transform.position += new Vector3(blockSize, 0, 0);
                        PastTouch += new Vector2(blockSize, 0);
                    }
                }
                else if (PastTouch.y > CurrentTouch.y)
                {
                    // move down
                    if(PastTouch.y - CurrentTouch.y >= blockSize / 2)
                    {
                        speed = 70;
                        TouchMove = true;
                    // PastTouch -= new Vector2(0, blockSize);
                    }
                }
            }
            else if (touchPhase == TouchPhase.Stationary)
            {
                if (speed == 70) StationaryTouch = true;
                if (TouchMove)
                {
                    if (Mathf.Abs(PastTouch.x - CurrentTouch.x) >= blockSize)
                    {
                        if (PastTouch.x > CurrentTouch.x && CheckWallKick(-blockSize))
                        {
                            // move left
                            gameObject.transform.position += new Vector3(-blockSize, 0, 0);
                            PastTouch -= new Vector2(blockSize, 0);
                        }
                        else if (PastTouch.x < CurrentTouch.x && CheckWallKick(blockSize))
                        {
                            // move right
                            gameObject.transform.position += new Vector3(blockSize, 0, 0);
                            PastTouch += new Vector2(blockSize, 0);
                        }
                    }
                }

            }
            else if (touchPhase == TouchPhase.Ended)
            {
                EndedTouch();
                // if(!StationaryTouch && countTimetouch <= 0.3f && speed == 70)
                // {
                //     speed = 300;
                //     // blockCreator.MoveBlockToGround();
                //     // allowMoveDown = false;
                //     enabledTouch = false;
                // } else speed = 5;

                // if(!TouchMove)
                // {
                //     RotateBlock();
                // }
            }
        }
        else if (TouchMove)
        {
            EndedTouch();
        }
    }

    void EndedTouch()
    {
        if (!StationaryTouch && countTimetouch <= 0.3f && speed == 70)
        {
            speed = 300;
            enabledTouch = false;
            PlayTrailRenderer();

        }
        else speed = 5;

        if (!TouchMove)
        {
            RotateBlock();
        }
    }

    public void DisableTouch()
    {
        enabledTouch = false;
    }

    public void EnabledTouch()
    {
        enabledTouch = true;
    }

    Vector3 ConvertPixelToWorldUnit(Vector2 pixelPosition)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(pixelPosition.x, pixelPosition.y, 10 + transform.position.z)
        );

        return worldPoint;
    }

    void PlayTrailRenderer()
    {
        foreach (Transform child in transform)
        {
            bool haveBlockUpper = CheckBlock(child);
            if (!haveBlockUpper)
            {
                TrailRenderer trailRenderer = child.GetComponent<TrailRenderer>();
                trailRenderer.enabled = true;
            }
        }
    }

    bool CheckBlock(Transform child)
    {
        foreach (Transform item in transform)
        {
            if (item.position.x == child.position.x && item.position.y > child.position.y)
            {
                return true;
            }
        }

        return false;
    }

    void StopTrailRenderer()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
