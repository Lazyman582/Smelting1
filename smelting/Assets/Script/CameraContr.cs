using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CameraContr : MonoBehaviour
{
    public bool Istouch = false;
    public List<GameObject> targetObjects = new List<GameObject>();
    public Camera cam;
    public GameObject prefab;
    public Vector3 spawnPosition;
    private PlayableDirector playDirector;
    // Start is called before the first frame update
    void Start()
    {
        playDirector = FindObjectOfType<PlayableDirector>();
        playDirector.stopped += OnTimelineStoped;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Touch touch = Input.GetTouch(0);
            HandleTouchClick(touch);



        }
    }
    void OnTimelineStoped(PlayableDirector director)
    {
        Debug.Log("4444");
        
    }
    void HandleTouchClick(Touch touch)
    {

        Ray ray = cam.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 检测是否是目标物体
            if (targetObjects.Contains(hit.collider.gameObject))
            {
                Debug.Log("有效点击：" + hit.collider.name);
                OnHitTarget(hit.collider.gameObject);
            }
        }
    }

   

    void OnHitTarget(GameObject obj)
    {
        switch (obj.name)
        {
            case "ironroe":
              
                obj.SetActive(false);
                GameObject instance = Instantiate(prefab,new Vector3(8.22999954f, -0.939999998f, -7.97100019f), Quaternion.identity);
                instance.transform.localScale = new Vector3(30,30,30);
                instance.gameObject.name = "stone";
                targetObjects.Add(instance);
                break;

            case "stone":
                Destroy(obj);
                playDirector.Play();
                
                break;

            default:
                
                break;
        }
    }

}
