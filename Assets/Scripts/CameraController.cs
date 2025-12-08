using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    public GameObject middlePoint;
    public Camera camera;

    public float edgeBuffer = 0.05f;

    private GameObject player1;
    private GameObject player2;

    public float wallWidth = 0.5f;
    private GameObject leftWall;
    private GameObject rightWall;

    void Start()
    {
        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");

        leftWall = new GameObject("LeftCameraWall");
        rightWall = new GameObject("RightCameraWall");

        BoxCollider leftCol = leftWall.AddComponent<BoxCollider>();
        BoxCollider rightCol = rightWall.AddComponent<BoxCollider>();

        leftCol.isTrigger = false;
        rightCol.isTrigger = false;

        leftWall.layer = 0;
        rightWall.layer = 0;
    }

    void Update()
    {
        Vector3 p1 = player1.transform.position;
        Vector3 p2 = player2.transform.position;

        float middleX = (p1.x + p2.x) / 2f;
        middlePoint.transform.position = new Vector3(middleX, 1f, 0f);

        float distance = Mathf.Abs(p1.x - p2.x);

        Vector3 targetPosition = new Vector3(middleX, 3f, -11f);
        camera.transform.position = targetPosition;

        UpdateWalls();
    }

    void UpdateWalls()
    {
        float camHeight = camera.orthographicSize * 2f;
        float camWidth = camHeight * camera.aspect;

        Vector3 leftEdge = camera.ViewportToWorldPoint(new Vector3(-50f, 0.5f, camera.nearClipPlane));
        Vector3 rightEdge = camera.ViewportToWorldPoint(new Vector3(50f, 0.5f, camera.nearClipPlane));

        float wallHeight = camHeight;

        leftWall.transform.position = new Vector3(leftEdge.x - wallWidth, 1f, 0f);
        rightWall.transform.position = new Vector3(rightEdge.x + wallWidth, 1f, 0f);

        leftWall.transform.localScale = new Vector3(wallWidth, wallHeight, 1f);
        rightWall.transform.localScale = new Vector3(wallWidth, wallHeight, 1f);
    }
}
