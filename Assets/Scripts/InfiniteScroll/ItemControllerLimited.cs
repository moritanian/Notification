using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class ItemControllerLimited<Item, ItemData> : UIBehaviour, IInfiniteScrollSetup where Item : InfiniteScrollItemBase<ItemData> {


    List<ItemData> itemDataList;
    private int itemListSize {
        get { return itemDataList != null ? itemDataList.Count : 0; }     
    }

    InfiniteScroll _infiniteScroll;
    private InfiniteScroll infiniteScroll
    {
        get {
            if (_infiniteScroll == null)
                _infiniteScroll = GetComponent<InfiniteScroll>();
              
            return _infiniteScroll;
        }
    }

	public void OnPostSetupItems()
	{
		infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
		GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
	}

	public void OnUpdateItem(int itemCount, GameObject item)
	{
		if(itemCount < 0 || itemCount >= itemListSize) {
			item.gameObject.SetActive (false);
		}
		else {
		    item.gameObject.SetActive (true);
			item.GetComponent<Item>().UpdateItem(itemDataList[itemCount]);
		}
	}

    public void SetItems(List<ItemData> itemDataList)
    {
        this.itemDataList = itemDataList;
        UpdateRectSize();
        infiniteScroll.Scroll(0);
    }

    public void ClearItems()
    {
        infiniteScroll.Scroll(0);
        itemDataList = null;
        UpdateRectSize();
        infiniteScroll.UpdateAllItems();
    }

    public Item GetItem(int postion)
    {
        return infiniteScroll.GetItem(postion).GetComponent<Item>();
    }

    public Item InsertItem(int position, ItemData itemData, bool isScroll = false)
    {
        this.itemDataList.Insert(position, itemData);
        UpdateRectSize();
        if (isScroll)
            infiniteScroll.Scroll(position);
        else
            infiniteScroll.UpdateAllItems();

        return GetItem(position);
    }

    public void UpdateItems()
    {
        infiniteScroll.UpdateAllItems();
    }

    private void UpdateRectSize()
    {
        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        delta.y = infiniteScroll.itemScale * (itemListSize + 0.5f);
        rectTransform.sizeDelta = delta;
    }
}
