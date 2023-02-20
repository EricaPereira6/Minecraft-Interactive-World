using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform highlight;
    [SerializeField]
    private GameObject parentSlot;

    private List<Transform> slots = new List<Transform>();
    private static int slotIndex = 0;

    public static Block.BlockType CurrentBlockTypeInSlot
    {
        get
        {
            if (slotIndex == 1) return Block.BlockType.STONE;
            if (slotIndex == 2) return Block.BlockType.PINKFLOWER;
            if (slotIndex == 3) return Block.BlockType.ROOT;
            if (slotIndex == 4) return Block.BlockType.WOOD;
            if (slotIndex == 5) return Block.BlockType.COAL;
            if (slotIndex == 6) return Block.BlockType.DIAMOND;
            if (slotIndex == 7) return Block.BlockType.GRASS;
            return Block.BlockType.DIRT;
        }
    }

    public void Start()
    {
        foreach (Transform slot in parentSlot.transform)
        {
            if (slot.parent == parentSlot.transform && slot.gameObject.activeSelf)
            {
                slots.Add(slot);
            }
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                slotIndex--;
            else
                slotIndex++;
            
            if (slotIndex > slots.Count - 1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = slots.Count - 1;

            highlight.position = slots[slotIndex].transform.position;
        }
    }
}