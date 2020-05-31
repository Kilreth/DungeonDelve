using UnityEngine;

public class DungeonMap : MonoBehaviour
{
    private Camera playerCamera;
    private Camera dungeonMapCamera;
    [SerializeField]
    private Camera dungeonMapCameraPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = gameObject.transform.Find(
            "FirstPersonCharacter").gameObject.GetComponent<Camera>();
        dungeonMapCamera = Instantiate(
            dungeonMapCameraPrefab,
            new Vector3(GM.Instance.DungeonParameters.Cols / 2,
                        GM.Instance.DungeonParameters.Rows / 2,
                        10),
            dungeonMapCameraPrefab.transform.rotation);
        dungeonMapCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GM.Instance.MapActive)
                HideDungeonMap();
            else
                ShowDungeonMap();
        }
    }

    public void ShowDungeonMap()
    {
        GM.Instance.MapActive = true;
        GM.Instance.Canvas.enabled = false;
        playerCamera.enabled = false;
        dungeonMapCamera.enabled = true;
    }

    public void HideDungeonMap()
    {
        GM.Instance.MapActive = false;
        GM.Instance.Canvas.enabled = true;
        playerCamera.enabled = true;
        dungeonMapCamera.enabled = false;
    }
}
