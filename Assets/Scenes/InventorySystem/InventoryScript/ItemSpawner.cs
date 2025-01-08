using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public List<ItemObject> itemObjects;
    public float minRadius = 2.0f;
    public float maxRadius = 10.0f;

    public Transform itemPickerTf;

    private void Awake()
    {
       if(Instance == null)
            Instance = this;
       else
            Destroy(gameObject);
    }

   public void SpawnItem(SO_Item item, int amount) //Drop from player
    {
        if(item.gamePrefab == null)
        {
            Debug.LogError("No prefab in " + item.name + "Please assign first!!!");
            return;
        }

        Vector2 randPos = Random.insideUnitCircle.normalized * minRadius;
        Vector3 offset = new Vector3(randPos.x, 0, randPos.y);
        GameObject spawnItem = Instantiate(item.gamePrefab, itemPickerTf.position + offset, Quaternion.identity);

        spawnItem.GetComponent<ItemObject>().SetAmount(amount);
    }


    public void SpawnItemByGUI(int SpawnAmount = 1) //Drop from player
    {
        for (int i = 0; i < SpawnAmount; i++)
        {
            int ind = Random.Range(0,itemObjects.Count);
            float distance = Random.Range(minRadius,maxRadius);
            Vector2 randPoint = Random.insideUnitCircle.normalized * distance;
            Vector3 offset = new Vector3(randPoint.x, 0, randPoint.y);
            ItemObject itemObjectSpawn = Instantiate(itemObjects[ind], itemPickerTf.position + offset, Quaternion.identity);
            itemObjectSpawn.RandomAmount();
        }
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Spawn 1 Random Item"))
        {
            SpawnItemByGUI();
        }
        if (GUILayout.Button("Spawn 10 Random Items"))
        {
            SpawnItemByGUI(10);
        }
        if (GUILayout.Button("Spawn 50 Random Items"))
        {
            SpawnItemByGUI(50);
        }
    }
}
