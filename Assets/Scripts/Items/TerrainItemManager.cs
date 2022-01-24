using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainItemManager : MonoBehaviour
{
    private const int xMaxPos = 12;
    private const int yMaxPos = 10;

    public static TerrainItemManager instance;

    [SerializeField] private bool instantiateItems = true;     
    [SerializeField] private List<Transform> _ironBlocks;
    [SerializeField] private List<TerrainItemMetadata> _terrainItems;
    public List<TerrainItemMetadata> TerrainItems => _terrainItems;

    private void Awake()
    {
        if (instance == null)
            instance = this; 
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if(instantiateItems)
            ChooseItemsLocation();
    }

    private void InstantiateItem(Vector2Int brickPos)
    {
        if(HasItemInTile(brickPos))
        {
            var item = GetBrickItem(brickPos);
            Instantiate(item, ((Vector3Int)brickPos), Quaternion.identity);
        }
    }

    private void ChooseItemsLocation()
    {
        Vector2 itemPosition = Vector2.zero;

        foreach (var itemMetadada in TerrainItems)
        {
            for (int i = 0; i < itemMetadada.Amount; i++)
            {
                if (!itemMetadada.FixedPosition)
                {
                    Vector2Int newPos = GenerateNewTilePosition();
                    while (!HasBrick(newPos) || (HasBrick(newPos) && HasItemInTile(newPos)))
                    {
                        newPos = GenerateNewTilePosition();
                    }
                    itemPosition = newPos;

                    var newtItem = Instantiate(itemMetadada.Item, itemPosition, Quaternion.identity);
                    itemMetadada.Instances.Add(new TerrainItemMetadataInstance(Vector2Int.RoundToInt(itemPosition), newtItem));
                }
                else if(itemMetadada.Instances[0] != null) // Fixed Position só terá 1 elemento
                {
                    itemPosition = itemMetadada.Instances[0].itemPosition;
                    var newtItem = Instantiate(itemMetadada.Item, itemPosition, Quaternion.identity);
                    itemMetadada.Instances[0].itemInstance = newtItem;
                }
            }
        }
    }

    private bool HasBrick(Vector2Int positionToCheck)
    {
        bool result = true;
        if (Brick.instance != null)
            result = Brick.instance.HasBrickTile(positionToCheck); // Brick detection

        return result;
    }

    private Vector2Int GenerateNewTilePosition() => new Vector2Int(Random.Range(0, xMaxPos), Random.Range(0, -yMaxPos));

    private bool HasItemInTile(Vector2Int tilePosition)
    {
        bool result = false;
        foreach (var metadata in TerrainItems)
        {
            foreach (var itemInstance in metadata.Instances)
            {
                result |= itemInstance.itemPosition == tilePosition;
                if (result) return true;
            }
        }
        return result;
    }

    private GameObject GetBrickItem(Vector2Int brickPos)
    {
        foreach (var metadata in TerrainItems)
        {
            var lookedItem = metadata.Instances.SingleOrDefault(itemData => itemData.itemPosition == brickPos);
            if (lookedItem != null)
            {
                print(lookedItem.itemInstance);
                return lookedItem.itemInstance;
            }
        }

        return null;
    }
}
