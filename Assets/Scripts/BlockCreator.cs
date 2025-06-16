// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class BlockCreator : MonoBehaviour
{
    // [SerializeField] private Material[] blockMaterials; // Các material khác nhau cho từng loại khối
    public SaveDataJson saveDataJson;
    public GameObject blockPrefab; // Khối 3D đơn sẽ được sử dụng để tạo các khối Tetris
    public GameObject MapFrame;
    public GameObject BlockFrame;
    public GameObject ListBlock;
    public Canvas targetCanvas;
    public GameObject PlayerBlock;
    public GameObject PoolBlock;
    private SupportTools supportTools;
    private GameManager gameManager;
    private List<GameObject> ListPlayerBlock = new List<GameObject>();
    // public ObjectPoolManager objectPoolManager;

    public TextureResources textureResources;
    // Enum định nghĩa các loại khối Tetris
    public enum BlockType
    {
        //Khối 1
        o,

        //Khối 2
        oo,

        //khối 3
        i,
        l,
        j,

        // khối 4
        I, // Khối I (một hàng 4 ô)
        O, // Khối O (hình vuông 2x2)
        T, // Khối T
        L, // Khối L
        J, // Khối J
        S, // Khối S
        Z,  // Khối Z
    }

    private Vector3 FirstBlockPosition;
    private float blockSize;
    private float blockScale;
    public int totalStar = 0;
    public GameObject StarPrefab;
    public GameObject ListStar;

    GameObject[,] grid = new GameObject[16, 9];

    public GameObject testblock;
    // public Material test;
    int[] RandomRange;
    void Start()
    {
        supportTools = GetComponent<SupportTools>();
        gameManager = GetComponent<GameManager>();

        // test = testblock.GetComponent<Renderer>().material;
        // test.texture
        // test.SetTexture("_MainTex", textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "1"));
        // test.SetTexture("_OverlayTex1", textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "2"));
    }

    void Initialize(int map)
    {
        gameObject.GetComponent<SupportTools>().Initialize(FirstBlockPosition, blockSize);
        PlayerBlock.GetComponent<Movement>().Initialize(FirstBlockPosition, blockSize, MapFrame.transform.position.y);
    }


#region Spawn Player Block
    public GameObject CreateTetrisBlock(BlockType blockType)
    {
        GameObject tetrisBlock = PlayerBlock;
        tetrisBlock.transform.position = Vector3.zero; // Vị trí bắt đầu của khối Tetris
        tetrisBlock.name = $"{blockType}";
        // Tạo các khối con dựa trên loại khối
        List<Vector3> blockPositions = GetBlockPositions(blockType);

        // Tạo các khối con từ prefab và gắn vào khối chính
        // int aa = 2;

        foreach (Vector3 position in blockPositions)
        {
            int randomTexture = Random.Range(RandomRange[0], RandomRange[1] + 1);
            // if(tetrisBlock.transform.childCount == 0 || tetrisBlock.transform.childCount == 2) randomTexture =2;
            // else randomTexture = 1;
            // int randomTexture = aa;
            // aa++;
            Texture texture = textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"{randomTexture}");
            GameObject blockInstance = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);

            blockInstance.transform.parent = tetrisBlock.transform;
            blockInstance.transform.localPosition = position;
            blockInstance.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
            blockInstance.GetComponent<Renderer>().material.mainTexture = texture;
            blockInstance.name = texture.name;
        }


        // Thiết lập tâm của khối tetris
        SetPivotPoint(tetrisBlock, blockType);
        return tetrisBlock;
    }
    
    // Trả về danh sách vị trí các khối con dựa trên loại khối Tetris
    private List<Vector3> GetBlockPositions(BlockType blockType)
    {
        List<Vector3> positions = new List<Vector3>();
        float sizeX = blockSize / 2;
        switch (blockType)
        {
            case BlockType.o:
                positions.Add(new Vector3(0, 0, 0));
                break;

            case BlockType.oo:
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                break;

            case BlockType.i:
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;

            case BlockType.l:
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;

            case BlockType.j:
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                break;

            case BlockType.I:
                // Khối I (một hàng ngang 4 ô)
                positions.Add(new Vector3(-sizeX*3, -sizeX, 0));
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX*3, -sizeX, 0));
                break;
                
            case BlockType.O:
                // Khối O (hình vuông 2x2)
                positions.Add(new Vector3(-sizeX, sizeX, 0));
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, sizeX, 0));
                break;
                
            case BlockType.T:
                // Khối T
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;
                
            case BlockType.L:
                // Khối L
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(0, -sizeX*2, 0));
                positions.Add(new Vector3(sizeX*2, -sizeX*2, 0));
                break;
                
            case BlockType.J:
                // Khối J
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(0, -sizeX*2, 0));
                positions.Add(new Vector3(-sizeX*2, -sizeX*2, 0));
                break;
                
            case BlockType.S:
                // Khối S
                positions.Add(new Vector3(sizeX*2, sizeX*2, 0));
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                break;
                
            case BlockType.Z:
                // Khối Z
                positions.Add(new Vector3(-sizeX*2, sizeX*2, 0));
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;
        }
        
        return positions;
    }
    
    // Thiết lập điểm tâm của khối Tetris
    private void SetPivotPoint(GameObject tetrisBlock, BlockType blockType)
    {
        float XX = FirstBlockPosition.x + blockSize * 4;
        float YY = -FirstBlockPosition.y - GetBlockPositions(blockType)[0].y + MapFrame.transform.position.y*2;

        switch (blockType)
        {
            case BlockType.oo:
            case BlockType.I:
            case BlockType.O:
                tetrisBlock.transform.position = new Vector3(XX - blockSize / 2, YY - blockSize/2, FirstBlockPosition.z);
                break;
            default:
                tetrisBlock.transform.position = new Vector3(XX, YY, FirstBlockPosition.z);
                break;
        }
    }
    
    public void CreateRandomBlock()
    {
        // Chọn một loại khối ngẫu nhiên
        BlockType randomType = (BlockType)Random.Range(0, 12);
        // randomType = BlockType.I;

        // Debug.Log(randomType);
        // Tạo khối và đặt ở vị trí bắt đầu
        CreateTetrisBlock(randomType);
        
        PlayerBlock.GetComponent<Movement>().StartMoveDown();
    }

    public void ChangePlayerBlockToRainbow()
    {
        int PlayerBlockLength = PlayerBlock.transform.childCount;
        if(PlayerBlockLength > 1)
        {
            for(int i = 0; i < 1;)
            {
                Transform child = PlayerBlock.transform.GetChild(0);
                child.name = "Block";
                ObjectPoolManager.ReturnObjectToPool(child.gameObject);
                child.SetParent(PoolBlock.transform);
                PlayerBlockLength--;
                if(PlayerBlockLength == 1) break;
            }
        }
        
        float XX = FirstBlockPosition.x + blockSize * 4;
        float YY = -FirstBlockPosition.y + MapFrame.transform.position.y * 2;

        PlayerBlock.transform.position = new Vector3 (XX, YY, PlayerBlock.transform.position.z);
        Transform lastPlayerChild = PlayerBlock.transform.GetChild(0);
        lastPlayerChild.name = "Rainbow";
        lastPlayerChild.localPosition = Vector3.zero;
        lastPlayerChild.GetComponent<Renderer>().material.mainTexture = 
            textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "Rainbow");
    }
#endregion

#region Create Map
    public void CreateLever (int map)
    {
        RandomRange = saveDataJson.TakeMapData().map[map].RandomRange;
        ListPlayerBlock.Clear();
        Vector2 landmarkPocation = GetLandmarkPocation();
        GameObject firstBlock = ObjectPoolManager.SpawnObject(blockPrefab, blockPrefab.transform.position, Quaternion.identity);
        Transform ListBlockTransform = ListBlock.transform;

        if (landmarkPocation.x / landmarkPocation.y > 9/16f) 
        {
            blockScale = -landmarkPocation.y * 2 / 16 / firstBlock.GetComponent<MeshFilter>().mesh.bounds.size.x;
        }
        else blockScale = (-landmarkPocation.x * 2 - 5)/ 9 / firstBlock.GetComponent<MeshFilter>().mesh.bounds.size.x;
        landmarkPocation.x =  -blockScale * 8;
        // 5: kich thuoc khung
        // 9: so luong block hang ngang
        firstBlock.transform.localScale = new Vector3(blockScale, blockScale, blockScale);

        float blockFrameScale = -landmarkPocation.x * 2 / GetMeshSize(BlockFrame.transform).x;
        BlockFrame.transform.localScale = BlockFrame.transform.localScale * blockFrameScale;
        BlockFrame.transform.position = new Vector3(0, 0, 90 + 4);

        blockSize = GetMeshSize(firstBlock.transform).x;
        FirstBlockPosition = new Vector3(landmarkPocation.x, landmarkPocation.y + blockSize / 2 + MapFrame.transform.position.y, 90 + blockScale);

        int[][] blockListData = saveDataJson.TakeMapData().map[map].BlockList;
        if (blockListData == null)
        {
            firstBlock.name = "Block";
            ObjectPoolManager.ReturnObjectToPool(firstBlock);
            firstBlock.transform.SetParent(PoolBlock.transform);
        }
        else
        {
            int blockListDataHigh = blockListData.Length;
            GameObject newBlock;
            for (int i = 0; i < blockListDataHigh; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // int randomTexture = Random.Range(1, 8);
                    int x = blockListDataHigh - i - 1;
                    if (blockListData[x][j] == 0) continue;
                    int randomTexture = blockListData[x][j];

                    if(randomTexture == -1) randomTexture = Random.Range(RandomRange[0], RandomRange[1] + 1);

                    Texture texture = textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"{randomTexture}");
                    if (ListBlockTransform.transform.childCount == 0) newBlock = firstBlock;
                    else newBlock = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);

                    newBlock.transform.localScale = firstBlock.transform.localScale;
                    newBlock.transform.position = FirstBlockPosition + new Vector3(j * blockSize, i * blockSize, 0);
                    newBlock.transform.SetParent(ListBlockTransform);
                    newBlock.GetComponent<Renderer>().material.mainTexture = texture;
                    newBlock.name = texture.name;
                    grid[i, j] = newBlock;
                }
            }
            totalStar = 0;
        }

        Initialize(map);
        CreateRandomBlock();
    }
#endregion

#region Functions Caculate
    public Vector2 GetCanvasSizeInUnits()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Canvas not assigned!");
            return Vector2.zero;
        }

        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
        Vector2 size = Vector2.zero;

        switch (targetCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            case RenderMode.ScreenSpaceCamera:
                // Trong chế độ Screen Space, đơn vị là pixels
                size = canvasRect.rect.size;
                Debug.Log("Canvas size in pixels: " + size);
                
                // Chuyển đổi từ pixels sang đơn vị thế giới nếu có camera
                if (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera && targetCanvas.worldCamera != null)
                {
                    // Lấy khoảng cách từ canvas đến camera
                    float distance = Mathf.Abs(targetCanvas.planeDistance);
                    
                    // Tính kích thước trong đơn vị thế giới
                    Camera cam = targetCanvas.worldCamera;
                    float height = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float width = height * cam.aspect;
                    
                    // Tỷ lệ giữa kích thước pixel và kích thước thế giới
                    float pixelToUnitRatio = height / Screen.height;
                    
                    size.x *= pixelToUnitRatio;
                    size.y *= pixelToUnitRatio;
                    
                    Debug.Log("Canvas size in world units: " + size);
                }
                break;
                
            case RenderMode.WorldSpace:
                // Trong World Space, đơn vị đã là đơn vị thế giới (unit)
                size = new Vector2(
                    canvasRect.rect.width * canvasRect.localScale.x,
                    canvasRect.rect.height * canvasRect.localScale.y
                );
                Debug.Log("Canvas size in world units: " + size);
                break;
        }

        return size;
    }

    Vector3 GetMeshSize(Transform block)
    {
        MeshFilter meshFilter = block.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            // Lấy kích thước nguyên bản của mesh
            Vector3 size = meshFilter.mesh.bounds.size;
            // Debug.Log("Kích thước mesh: " + size);

            // Kích thước thực tế với scale
            Vector3 scaledSize = Vector3.Scale(size, block.transform.localScale);
            // Debug.Log("Kích thước thực tế: " + scaledSize);
            return scaledSize;
        }
        return Vector3.zero;
    }

    Vector2 GetLandmarkPocation()
    {
        RectTransform rectBody = MapFrame.GetComponent<RectTransform>();

        float uiWidthInUnits = rectBody.rect.x * targetCanvas.transform.localScale.x;
        float uiHeightInUnits = rectBody.rect.y * targetCanvas.transform.localScale.y;
        // float uiHeightInUnits = rectBody.rect.y * targetCanvas.transform.localScale.y + rectBody.position.y;

        return new Vector2(uiWidthInUnits, uiHeightInUnits);
    }

    public bool IsValidPosition(Transform block)
    {
        foreach (Transform child in block)
        {
            Vector3 childPosition = child.position;
            int x = Mathf.FloorToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            
            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                // if(grid[x - 1, y - 1]) {return false;};
                if(grid[x, y] != null)
                {
                    return false;
                }
                // else grid[x, y] = true;
            }
            else if(x <= 0)
            {
                return false;
            };
        }
        return true;
    }

    public void LockPlayerBlock(string txt = "")
    {
        List<GameObject> listChildBlock = new List<GameObject>();
        ListPlayerBlock.Clear();
        isPlayerBlockMoveDown = false;

        int total = PlayerBlock.transform.childCount;
        for(int i = 0; i < 1;)
        {
            total--;
            if(total == 0) i++;
            Transform child = PlayerBlock.transform.GetChild(0);
            Vector3 childPosition = child.position;
            int x = Mathf.RoundToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            if (grid[x, y] == null && x < 16) grid[x, y] = child.gameObject;
            else
            {
                LoseGame();
                return;
            }

            child.position = new Vector3(FirstBlockPosition.x + y * blockSize, FirstBlockPosition.y + x * blockSize, child.position.z);
            ListPlayerBlock.Add(child.gameObject);
            child.SetParent(ListBlock.transform);
            listChildBlock.Add(child.gameObject);
        }

        CheckListBlock(listChildBlock);
    }

    void CheckListBlock(List<GameObject> listChildBlock)
    {
        if(listChildBlock[0].name == "Rainbow")
        {
            CheckRainbowBlock(listChildBlock[0]);
            return;
        }

        int time = 0;
        foreach(GameObject childBlock in listChildBlock)
        {
            Vector3 childPosition = childBlock.transform.position;
            int x = Mathf.RoundToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            List<GameObject> nearbyBlocks = new List<GameObject>();
            CheckSameNearbyBlock(nearbyBlocks, childBlock, x, y);
            int largestTime = 0;
            if(nearbyBlocks.Count > 2) largestTime = DeleteListBlock(nearbyBlocks);
            if(largestTime > time) time = largestTime;
        }
        
        if (time == 0) WaitForMove();
        else StartCoroutine(CheckGrid(time * 0.1f + 0.3f));
    }

    int DeleteListBlock (List<GameObject> list)
    {
        int time = 0;
        
        int firstX = Mathf.RoundToInt((list[0].transform.position.y - FirstBlockPosition.y) / blockSize);
        int firstY = Mathf.RoundToInt((list[0].transform.position.x - FirstBlockPosition.x) / blockSize);
        gameManager.CheckScore(list[0].name, list.Count);

        int countItem = 0;
        foreach(GameObject child in list)
        {
            if(DOTween.IsTweening(child)) break;
            int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
            int childTimer = Mathf.Abs(firstX - x) + Mathf.Abs(firstY - y);
            if(childTimer > time) time = childTimer;

            child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * childTimer).OnComplete(() => {
                child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
                    child.name = "Block";
                    ObjectPoolManager.ReturnObjectToPool(child);
                    child.transform.SetParent(PoolBlock.transform);
                });
            });
            grid[x, y] = null;

            countItem++;
            if(countItem > 3)
            {
                PlayStarAnimation(child, 0.1f * childTimer + 0.1f);
            }

        }
        return time;
    }

    bool isPlayerBlockMoveDown = false;

    IEnumerator CheckGrid(float time, string txt = null)
    {
        int timer = 0;

        yield return new WaitForSeconds(time);
        // Debug.Log(grid[1, 4]);
        for(int i = 0; i < 16; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                GameObject block = grid[i, j];
                if(block != null)
                {
                    if(i - 1 >= 0 && grid[i - 1, j] == null)
                    {
                        List<GameObject> nearbyBlocks = new List<GameObject>();
                        CheckNearbyBlock(nearbyBlocks, block.transform, i, j);
                        GameObject checkBlock = nearbyBlocks.Find(block => Mathf.RoundToInt((block.transform.position.y - FirstBlockPosition.y) / blockSize) == 0);

                        if(checkBlock == null) 
                        {
                            int endTime = MoveBlockToBottom(nearbyBlocks);
                            if(endTime > timer && endTime != 16) timer = endTime;
                        }
                    }
                }
            }
        }

        if (isPlayerBlockMoveDown) Invoke("CheckPlayerBlockAgain", 0.1f * (float)timer);
        else if (txt == null) Invoke("WaitForMove", 0.1f * (float)timer);
        else if (txt != null) Invoke("DisableTouchInSupportTools", 0.1f * (float)timer);
    }

    void CheckPlayerBlockAgain()
    {
        List<GameObject> listChildBlock = ListPlayerBlock;
        isPlayerBlockMoveDown = false;
        CheckListBlock(listChildBlock);
    }

    void DisableTouchInSupportTools()
    {
        supportTools.DisableTouch();
    }

    int MoveBlockToBottom(List<GameObject> nearbyBlocks)
    {
        int shortest = 16;
        foreach(GameObject child in nearbyBlocks)
        {
            if(DOTween.IsTweening(child.transform)) break;
            int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
            int count = 0;
            for(int i = x - 1; i >= 0; i--)
            {
                if(grid[i,y] == null) count++;
                else if(nearbyBlocks.Contains(grid[i,y]))
                {
                    count = 16;
                    break;
                }
                else break;
            }

            if(shortest > count) shortest = count;
        }

        if(shortest < 16)
        {
            foreach(GameObject child in nearbyBlocks)
            {
                int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
                int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
                grid[x, y] = null;
                if(!isPlayerBlockMoveDown && ListPlayerBlock.Contains(child)) isPlayerBlockMoveDown = true;

                child.transform.DOMove(new Vector3(child.transform.position.x, FirstBlockPosition.y + (x - shortest) * blockSize, child.transform.position.z), 0.1f * shortest);
            }

            foreach(GameObject child in nearbyBlocks)
            {
                int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
                int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
                // grid[x, y] = null;
                grid[x - shortest, y] = child;
            }
        }

        return shortest;
    }

    void WaitForMove()
    {
        CreateRandomBlock();
        gameManager.CheckWinGame();
    }

    public void CheckNearbyBlock(List<GameObject> nearbyBlocks,  Transform block, int x, int y)
    {
        if(nearbyBlocks.Contains(block.gameObject)) return;
        nearbyBlocks.Add(block.gameObject);

        if(x + 1 < grid.GetLength(0) && grid[x + 1, y] != null) 
        {
            CheckNearbyBlock(nearbyBlocks, grid[x + 1, y].transform, x + 1, y);
        }
        if(x - 1 >= 0 && grid[x - 1, y] != null) 
        {
            CheckNearbyBlock(nearbyBlocks, grid[x - 1, y].transform, x - 1, y);
        }
        if(y + 1 < grid.GetLength(1) && grid[x, y + 1] != null) 
        {
            CheckNearbyBlock(nearbyBlocks, grid[x, y + 1].transform, x, y + 1);
        }
        if(y - 1 >= 0 && grid[x, y - 1] != null) 
        {
            CheckNearbyBlock(nearbyBlocks, grid[x, y - 1].transform, x, y - 1);
        }
    }

    public void CheckSameNearbyBlock(List<GameObject> nearbyBlocks,  GameObject block, int x, int y)
    {
        if(nearbyBlocks.Contains(block)) return;
        nearbyBlocks.Add(block);

        if(x + 1 < grid.GetLength(0) && grid[x + 1, y] != null && block.name == grid[x + 1, y].name) 
        {
            CheckSameNearbyBlock(nearbyBlocks, grid[x + 1, y], x + 1, y);
        }
        if(x - 1 >= 0 && grid[x - 1, y] != null && block.name == grid[x - 1, y].name) 
        {
            CheckSameNearbyBlock(nearbyBlocks, grid[x - 1, y], x - 1, y);
        }
        if(y + 1 < grid.GetLength(1) && grid[x, y + 1] != null && block.name == grid[x, y + 1].name) 
        {
            CheckSameNearbyBlock(nearbyBlocks, grid[x, y + 1], x, y + 1);
        }
        if(y - 1 >= 0 && grid[x, y - 1] != null && block.name == grid[x, y - 1].name) 
        {
            CheckSameNearbyBlock(nearbyBlocks, grid[x, y - 1], x, y - 1);
        }
    }

    public bool CheckBlockTouch(float nextPositionX, Transform block, float nextBlockPosition)
    {
        foreach (Transform child in block)
        {
            Vector3 childPosition = child.position;
            int x = Mathf.FloorToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            x = x == -1 ? 0 : x;
            int xNext = Mathf.FloorToInt((childPosition.y + nextBlockPosition - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            if(x > xNext)
            {
                if(nextPositionX > 0)
                {
                    if(grid[x, y + 1] != null) return false;
                }
                else
                {
                    if(grid[x, y - 1] != null) return false;
                }
            }
            else
            {
                if(nextPositionX > 0)
                {
                    if(grid[x, y + 1] != null) return false;
                    if(x + 1 < grid.GetLength(0) && grid[x + 1, y + 1] != null) return false;
                }
                else
                {
                    if(grid[x, y - 1] != null) return false;
                    if(x + 1 < grid.GetLength(0) && grid[x + 1, y - 1] != null) return false;
                }
            }
        }
        return true;
    }

    public bool CheckBlockTouchWhenRotate(List<Vector3> listNewPosition, Transform block)
    {
        bool touchLeft = false;
        bool touchRight = false;
        bool touchTop = false;
        bool touchBottom = false;

        for(int i = 0; i < listNewPosition.Count; i++)
        {
            Vector3 childlocalPosition = listNewPosition[i];
            Vector3 childPosition = childlocalPosition + block.position;
            int x = Mathf.FloorToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            if(x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                if(grid[x, y] != null)
                {
                    if(Mathf.Round(childlocalPosition.x) > 0) touchRight = true;
                    else if(Mathf.Round(childlocalPosition.x) < 0) touchLeft = true;
                    if(Mathf.Round(childlocalPosition.y) > 0) touchTop = true;
                    else if(Mathf.Round(childlocalPosition.y) < 0) touchBottom = true;
                    // return false;
                }
            }
            else 
            {
                if(x < 0) touchBottom = true;
                if(x >= grid.GetLength(0)) touchTop = true;
                if(y < 0) touchLeft = true;
                if(y >= grid.GetLength(1)) touchRight = true;
                // return false;
            }
        }

        if((touchLeft && touchRight) || (touchBottom && touchTop)) return false;
        string status = "";
        if(touchLeft)
        {
            if(touchTop) status = "topLeft";
            else if (touchBottom) status = "bottomLeft";
            else status = "left";
        }
        else if(touchRight)
        {
            if(touchTop) status = "topRight";
            else if (touchBottom) status = "bottomRight";
            else status = "right";
        }
        else if (touchTop) status = "top";
        else if (touchBottom) status = "bottom";
        else return true;

        return IsValidToRotate(listNewPosition, block, status);
    }

    bool IsValidToRotate (List<Vector3> listNewPosition, Transform block, string txt)
    {
        float xx = 0;
        float yy = 0;

        switch (txt)
        {
            case "left":
                xx = blockSize;
                break;
            case "right":
                xx = -blockSize;
                break;
            case "top":
                yy = -blockSize;
                break;
            case "bottom":
                yy = blockSize;
                break;
            case "topLeft":
                xx = blockSize;
                yy = -blockSize;
                break;
            case "topRight":
                xx = -blockSize;
                yy = -blockSize;
                break;
            case "bottomLeft":
                xx = blockSize;
                yy = blockSize;
                break;
            case "bottomRight":
                xx = -blockSize;
                yy = blockSize;
                break;
        }

        if(block.name == "I")
        {
            float child0X = block.GetChild(0).localPosition.x;
            if((txt == "left" && child0X > 0) || (txt == "right" && child0X < 0))
            {
                xx *= 2;
                yy *= 2;
            }
        }

        if(xx != 0 && yy != 0)
        {
            if(!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, 0, 0)))
            {
                if(!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(0, yy, 0)))
                {
                    if(!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, yy, 0)))
                    {
                        return false;
                    }
                    else block.position += new Vector3 (xx, yy, 0);
                }
                else block.position += new Vector3 (0, yy, 0);
            }
            else block.position += new Vector3 (xx, 0, 0);
        }
        else
        {
            if(CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, yy, 0)))
                block.position += new Vector3 (xx, yy, 0);
            else return false;
        }

        return true;
    }

    bool CheckTouchAfterMoveBlock (List<Vector3> listNewPosition, Transform block, Vector3 pos)
    {
        for(int i = 0; i < listNewPosition.Count; i++)
        {
            Vector3 childPosition = listNewPosition[i] + block.position + pos;
            int x = Mathf.FloorToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            if(x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                if(grid[x, y] != null) return false;
            }
            else return false;
        }
        return true;
    }

    public float CheckNextPosition(float distance)
    {
        // int xLength = grid.GetLength(0);
        // int yLength = grid.GetLength(1);
        foreach (Transform child in PlayerBlock.transform)
        {
            Vector3 childPosition = child.position;
            int x = Mathf.FloorToInt((childPosition.y - FirstBlockPosition.y + distance) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            
            if (x >= 0 && x < 16 && y >= 0 && y < 9)
            {
                if(grid[x, y] != null)
                {
                    x++;
                    return FirstBlockPosition.y  +  (blockSize * x) - childPosition.y;
                }
            }
            else if(x <= 0)
            {
                return FirstBlockPosition.y - childPosition.y;
            };
        }
        return distance;
    }
    #endregion

    #region Tool
    List<GameObject> ListBlockInfluence = new List<GameObject>();
    // GameObject firstBlock;
    public void CheckListBlockToDestroy(string ToolType, Vector2 CurrentTouch)
    {
        int x = Mathf.RoundToInt((CurrentTouch.y - FirstBlockPosition.y) / blockSize);
        int y = Mathf.RoundToInt((CurrentTouch.x - FirstBlockPosition.x) / blockSize);
        if (x < 0 || x >= 16 || y < 0 || y >= 9)
        {
            RedoListBlockInfluence();
            ListBlockInfluence.Clear();
            return;
        }

        if (ListBlockInfluence.Count > 0)
        {
            if (grid[x, y] == ListBlockInfluence[0]) return;
            RedoListBlockInfluence();
        }
        
        ListBlockInfluence.Clear();

        if (grid[x, y] != null) ListBlockInfluence.Add(grid[x, y]);

        GameObject child;
        switch (ToolType)
        {
            case "Boom":
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (i < 0 || i >= 16 || j < 0 || j >= 9) continue;
                        child = grid[i, j];
                        if (child == null) continue;
                        ListBlockInfluence.Add(child);
                    }
                }
                break;
            case "TNT":
                for (int i = x; i < 16; i++)
                {
                    child = grid[i, y];
                    if (child == null) continue;
                    ListBlockInfluence.Add(child);
                }

                for (int i = x - 1; i >= 0; i--)
                {
                    child = grid[i, y];
                    if (child == null) continue;
                    ListBlockInfluence.Add(child);
                }
                break;
            case "Hammer":
                for (int j = y; j < 9; j++)
                {
                    child = grid[x, j];
                    if (child == null) continue;
                    ListBlockInfluence.Add(child);
                }

                for (int j = y - 1; j >= 0; j--)
                {
                    child = grid[x, j];
                    if (child == null) continue;
                    ListBlockInfluence.Add(child);
                }
                break;
        }

        // StartCoroutine(PlayAnimListBlockInfluence(ListBlockInfluence));
        ShowListBlockInfluence();
    }

    public void RedoListBlockInfluence()
    {
        foreach(GameObject child in ListBlockInfluence)
        {
            child.GetComponent<Renderer>().material.color = Color.white;
        }
    }
    
    void ShowListBlockInfluence()
    {
        foreach (GameObject child in ListBlockInfluence)
        {
            child.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    // IEnumerator PlayAnimListBlockInfluence(List<GameObject> list)
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(5f);
    //         int firstX = Mathf.RoundToInt((list[0].transform.position.y - FirstBlockPosition.y) / blockSize);
    //         int firstY = Mathf.RoundToInt((list[0].transform.position.x - FirstBlockPosition.x) / blockSize);
    //         gameManager.CheckScore(list[0].name, list.Count);

    //         foreach(GameObject child in list)
    //         {
    //             if(DOTween.IsTweening(child)) break;
    //             int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
    //             int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
    //             int childTimer = Mathf.Abs(firstX - x) + Mathf.Abs(firstY - y);

    //             yield return new WaitForSeconds(5f);
    //             // child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * childTimer).OnComplete(() => {
    //             //     child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
    //             //     });
    //             // });
    //         }

    //     }

    // }

    public bool ActiveBoom(int x, int y)
    {
        if (ListBlockInfluence.Count == 0) return false;
        RedoListBlockInfluence();
        int timer = DeleteListBlock(ListBlockInfluence);
        ListBlockInfluence.Clear();
        // for (int i = x - 1; i <= x + 1; i++)
        // {
        //     for (int j = y - 1; j <= y + 1; j++)
        //     {
        //         if (i < 0 || i >= 16 || j < 0 || j >= 9) continue;
        //         GameObject child = grid[i, j];
        //         if (child == null) continue;
        //         child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
        //         {
        //             child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        //             {
        //                 gameManager.CheckScore(child.name, 1);
        //                 child.name = "Block";
        //                 ObjectPoolManager.ReturnObjectToPool(child);
        //                 child.transform.SetParent(PoolBlock.transform);
        //             });
        //         });
        //         grid[i, j] = null;
        //     }
        // }

        StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));
        return true;
    }

    public bool DeleteHorizontalRow(int x, int y)
    {
        // xoá hàng ngang
        if (ListBlockInfluence.Count == 0) return false;

        RedoListBlockInfluence();
        int timer = DeleteListBlock(ListBlockInfluence);
        ListBlockInfluence.Clear();
        // int count = 0;
        // for (int j = y; j < 9; j++)
        // {
        //     GameObject child = grid[x,j];
        //     if(child == null) continue;
        //     child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * count).OnComplete(() => {
        //         child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
        //             child.name = "Block";
        //             ObjectPoolManager.ReturnObjectToPool(child);
        //             child.transform.SetParent(PoolBlock.transform);
        //         });
        //     });
        //     count++;
        //     grid[x,j] = null;
        // }

        // int count1 = 0;
        // for (int j = y - 1; j >= 0; j--)
        // {
        //     GameObject child = grid[x,j];
        //     if(child == null) continue;
        //     count1++;
        //     child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * count1).OnComplete(() => {
        //         child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
        //             gameManager.CheckScore(child.name, 1);
        //             child.name = "Block";
        //             ObjectPoolManager.ReturnObjectToPool(child);
        //             child.transform.SetParent(PoolBlock.transform);
        //         });
        //     });
        //     grid[x,j] = null;
        // }

        // int timer = count > count1 ? count : count1;
        StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));
        return true;
    }

    public bool DeleteVerticalRow(int x, int y)
    {
        // xoá hàng dọc
        if (ListBlockInfluence.Count == 0) return false;

        RedoListBlockInfluence();
        int timer = DeleteListBlock(ListBlockInfluence);
        ListBlockInfluence.Clear();
        // int count = 0;
        // for (int i = x; i < 16; i++)
        // {
        //     GameObject child = grid[i,y];
        //     if(child == null) continue;
        //     child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * count).OnComplete(() => {
        //         child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
        //             gameManager.CheckScore(child.name, 1);
        //             child.name = "Block";
        //             ObjectPoolManager.ReturnObjectToPool(child);
        //             child.transform.SetParent(PoolBlock.transform);
        //         });
        //     });
        //     count++;
        //     grid[i,y] = null;
        // }

        // int count1 = 0;
        // for (int i = x - 1; i >= 0; i--)
        // {
        //     GameObject child = grid[i,y];
        //     if(child == null) continue;
        //     count1++;
        //     child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * count1).OnComplete(() => {
        //         child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
        //             gameManager.CheckScore(child.name, 1);
        //             child.name = "Block";
        //             ObjectPoolManager.ReturnObjectToPool(child);
        //             child.transform.SetParent(PoolBlock.transform);
        //         });
        //     });
        //     grid[i,y] = null;
        // }

        // int timer = count > count1 ? count : count1;
        StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));
        return true;
    }

    void CheckRainbowBlock(GameObject block)
    {
        Vector3 blockPosition = block.transform.position;
        int x = Mathf.RoundToInt((blockPosition.y - FirstBlockPosition.y) / blockSize);
        int y = Mathf.RoundToInt((blockPosition.x - FirstBlockPosition.x) / blockSize);
        List<GameObject> ListnearbyBlock1 = new List<GameObject>();
        List<GameObject> ListnearbyBlock2 = new List<GameObject>();
        List<GameObject> ListnearbyBlock3 = new List<GameObject>();
        List<GameObject> ListnearbyBlock4 = new List<GameObject>();
        List<GameObject> ListnearbyBlock = new List<GameObject>();
        int timer = 0;
        int count = 0;

        if(x + 1 < grid.GetLength(0) && grid[x + 1, y] != null) 
        {
            // ListnearbyBlock1.Add(block);
            CheckSameNearbyBlock(ListnearbyBlock1, grid[x + 1, y], x + 1, y);
            if(ListnearbyBlock1.Count > 1) count = DeleteListBlock(ListnearbyBlock1);
            else ListnearbyBlock.Add(grid[x + 1, y]);
            timer = count > timer ? count : timer;
        }
        if(x - 1 >= 0 && grid[x - 1, y] != null) 
        {
            // ListnearbyBlock2.Add(block);
            CheckSameNearbyBlock(ListnearbyBlock2, grid[x - 1, y], x - 1, y);
            if(ListnearbyBlock2.Count > 1) count = DeleteListBlock(ListnearbyBlock2);
            else ListnearbyBlock.Add(grid[x - 1, y]);
            timer = count > timer ? count : timer;
        }
        if(y + 1 < grid.GetLength(1) && grid[x, y + 1] != null) 
        {
            // ListnearbyBlock3.Add(block);
            CheckSameNearbyBlock(ListnearbyBlock3, grid[x, y + 1], x, y + 1);
            if(ListnearbyBlock3.Count > 1) count = DeleteListBlock(ListnearbyBlock3);
            else ListnearbyBlock.Add(grid[x, y + 1]);
            timer = count > timer ? count : timer;
        }
        if(y - 1 >= 0 && grid[x, y - 1] != null) 
        {
            // ListnearbyBlock4.Add(block);
            CheckSameNearbyBlock(ListnearbyBlock4, grid[x, y - 1], x, y - 1);
            if(ListnearbyBlock4.Count > 1) count = DeleteListBlock(ListnearbyBlock4);
            else ListnearbyBlock.Add(grid[x, y - 1]);
            timer = count > timer ? count : timer;
        }

        if(ListnearbyBlock.Count > 1)
        {
            count = CountUsingLINQ(ListnearbyBlock);
            timer = count > timer ? count : timer;
        }

        supportTools.DisableTouch();

        if (timer > 0)
        {
            timer++;
            block.name = "Block";
            block.transform.DOScale(block.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                block.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    ObjectPoolManager.ReturnObjectToPool(block);
                    block.transform.SetParent(PoolBlock.transform);
                });
            });
            grid[x, y] = null;
            StartCoroutine(CheckGrid(timer * 0.1f + 0.3f));
        }
        else
        {
            if (x > 0)
            {
                SetRaibowBlock(block, grid[x - 1, y]);
            }
            else
            {
                if (y < 8 && grid[x, y + 1] != null) SetRaibowBlock(block, grid[x, y + 1]);
                else if (y > 0 && grid[x, y - 1] != null) SetRaibowBlock(block, grid[x, y - 1]);
                else if (x < 15 && grid[x + 1, y] != null) SetRaibowBlock(block, grid[x, y - 1]);
                else
                {
                    int randomTexture = Random.Range(1, 8);
                    Texture texture = textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"{randomTexture}");
                    block.name = texture.name;
                    block.GetComponent<Renderer>().material.mainTexture = texture;
                }
            }
            CreateRandomBlock();
        }
    }

    int CountUsingLINQ(List<GameObject> list)
    {
        var groupedObjects = list.GroupBy(obj => obj.name);
        int timer = 0;
        foreach (var group in groupedObjects)
        {
            if(group.Count() > 1)
            {
                int count = 0;
                if(list.Count == group.Count()) count = DeleteListBlock(list);
                else
                {
                    List<GameObject> listBlock = list.FindAll(item => item.name == group.Key);
                    count = DeleteListBlock(listBlock);
                }
                timer = count > timer ? count : timer;
            }
        }
        return timer;
    }

    // void CountUsingDictionary(List<GameObject> list)
    // {
    //     Dictionary<string, int> nameCounts = new Dictionary<string, int>();

    //     foreach (GameObject obj in list)
    //     {
    //         string objName = obj.name;
            
    //         if (nameCounts.ContainsKey(objName))
    //         {
    //             nameCounts[objName]++;
    //         }
    //         else
    //         {
    //             nameCounts[objName] = 1;
    //         }
    //     }

    //     Debug.Log($"Tổng số loại GameObject: {nameCounts.Count}");
    //     foreach (var pair in nameCounts)
    //     {
    //         Debug.Log($"Tên: {pair.Key}, Số lượng: {pair.Value}");
    //     }
    // }
    

    // void CountElementsWithDictionary<T>(List<T> list)
    // {
    //     Dictionary<T, int> countDictionary = new Dictionary<T, int>();

    //     foreach (T item in list)
    //     {
    //         if (countDictionary.ContainsKey(item))
    //         {
    //             countDictionary[item]++;
    //         }
    //         else
    //         {
    //             countDictionary[item] = 1;
    //         }
    //     }

    //     Debug.Log($"Tổng số loại phần tử: {countDictionary.Count}");
    //     foreach (var pair in countDictionary)
    //     {
    //         Debug.Log($"Phần tử: {pair.Key}, Số lượng: {pair.Value}");
    //     }
    // }

    void SetRaibowBlock (GameObject block, GameObject blockToCopy)
    {
        block.name = blockToCopy.name;
        block.GetComponent<Renderer>().material.mainTexture = 
            blockToCopy.GetComponent<Renderer>().material.mainTexture;
    }
#endregion

#region Particle
    void PlayStarAnimation (GameObject block, float val)
    {
        Transform star = ObjectPoolManager.SpawnObject(StarPrefab, block.transform.position , Quaternion.identity).transform;
        Image starImage = star.GetComponent<Image>();
        star.SetParent(ListStar.transform);
        star.position = new Vector3(star.position.x, star.position.y, 90);
        star.localScale = Vector3.zero;
        Color newColor = starImage.color;
        newColor.a = 1;
        starImage.color = newColor;
        totalStar += 1;
        star.DOScale(1, 0.2f).SetEase(Ease.OutBounce).SetDelay(val);
        // star.DORotate(new Vector3(0,0, 360 *5), 1.2f);
        starImage.DOFade(0, 0.5f).SetDelay(0.4f + val);
    }
#endregion
    void ResetListBlock()
    {
        for(int i = 0; i < 16; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                GameObject block = grid[i,j];
                if(block == null) continue;
                block.name = "Block";
                ObjectPoolManager.ReturnObjectToPool(block);
                block.transform.SetParent(PoolBlock.transform);
                grid[i, j] = null;
            }
        }

        int playerBlockChildCount = PlayerBlock.transform.childCount;
        for(int i = 0; i < 1;)
        {
            playerBlockChildCount--;
            if(playerBlockChildCount < 0) break;
            Transform block = PlayerBlock.transform.GetChild(0);
            block.name = "Block";
            ObjectPoolManager.ReturnObjectToPool(block.gameObject);
            block.transform.SetParent(PoolBlock.transform);
        }
    }

    public void GameOver()
    {
        PlayerBlock.GetComponent<Movement>().StopAllAction();
        Invoke("ResetListBlock", 0.1f);
    }

    void LoseGame ()
    {
        gameManager.LoseGame();
    }
}