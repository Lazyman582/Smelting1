using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoad : MonoBehaviour
{
    public GameObject[] Prefab;
    public GameObject[] FatherObj;
    public void YuzhitiX(int a)
    {
        GameObject newobj = Instantiate(Prefab[a], FatherObj[a].transform);
    }
    public void YuzhitiDes(int b)
    {
        // 先把所有子物体放进列表，再遍历删除（最安全）
        List<Transform> childList = new List<Transform>();
        foreach (Transform child in FatherObj[b].transform)
        {
            childList.Add(child);
        }

        // 遍历列表删除，绝不会漏删、不乱跳
        foreach (Transform item in childList)
        {
            if (item.name.StartsWith(Prefab[b].name))
            {
                Destroy(item.gameObject);
            }
        }
    }
}
