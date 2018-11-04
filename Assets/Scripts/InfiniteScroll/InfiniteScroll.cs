using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class InfiniteScroll : UIBehaviour
{
	public RectTransform itemPrototype;

	[SerializeField, Range(0, 30)]
	int instantateItemCount = 9;

	[SerializeField]
	private Direction direction;

	public OnItemPositionChange onUpdateItem = new OnItemPositionChange();

	[System.NonSerialized]
	public LinkedList<RectTransform> itemList = new LinkedList<RectTransform>();

	protected float diffPreFramePosition = 0;

	protected int currentItemNo = 0;

	public enum Direction
	{
		Vertical,
		Horizontal,
	}

	// cache component
    private ScrollRect scrollRect;

    private RectTransform _rectTransform;
	protected RectTransform rectTransform {
		get {
			if(_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	private float anchoredPosition
	{
		get {
			return direction == Direction.Vertical ? -rectTransform.anchoredPosition.y : rectTransform.anchoredPosition.x;
		}
        set
        {
            if (direction == Direction.Vertical)
                rectTransform.anchoredPosition = new Vector2(0, -value);
            else
                rectTransform.anchoredPosition = new Vector2(value, 0);
        }
	}

    private float scrollVeclocity
    {
        get
        {
            return direction == Direction.Vertical ? -scrollRect.velocity.y : scrollRect.velocity.x;
        }
        set
        {
            if (direction == Direction.Vertical)
                scrollRect.velocity = new Vector2(0, -value);
            else
                scrollRect.velocity = new Vector2(value, 0);
        }
    }

    private float _itemScale = -1;
	public float itemScale {
		get {
			if(itemPrototype != null && _itemScale == -1) {
				_itemScale = direction == Direction.Vertical ? itemPrototype.sizeDelta.y : itemPrototype.sizeDelta.x;
			}
			return _itemScale;
		}
	}

    protected override void Awake()
    {
        base.Awake();
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    protected override void Start ()
	{
		var controllers = GetComponents<MonoBehaviour>()
				.Where(item => item is IInfiniteScrollSetup)
				.Select(item => item as IInfiniteScrollSetup)
				.ToList();

		// create items

		scrollRect.horizontal = direction == Direction.Horizontal;
		scrollRect.vertical = direction == Direction.Vertical;
		scrollRect.content = rectTransform;

		itemPrototype.gameObject.SetActive(false);
		
		for(int i = 0; i < instantateItemCount; i++) {
			var item = GameObject.Instantiate(itemPrototype) as RectTransform;
			item.SetParent(transform, false);
			item.name = i.ToString();
			item.anchoredPosition = direction == Direction.Vertical ? new Vector2(0, -itemScale * i) : new Vector2(itemScale * i, 0);
			itemList.AddLast(item);

			item.gameObject.SetActive(true);

			foreach(var controller in controllers) {
				controller.OnUpdateItem(i, item.gameObject);
			}
		}

		foreach(var controller in controllers){
			controller.OnPostSetupItems();
		}
	}

    void Update()
    {
        UpdateAllItemsPosition();
    }

    void UpdateAllItemsPosition() {
		if (itemList.First == null) {
			return;
		}

        if(itemScale == 0)
        {
            Debug.LogError("itemScale is 0");
            return;
        }
        
		while(anchoredPosition - diffPreFramePosition  < -itemScale * 2) {
			diffPreFramePosition -= itemScale;

			var item = itemList.First.Value;
			itemList.RemoveFirst();
			itemList.AddLast(item);

			var pos = itemScale * instantateItemCount + itemScale * currentItemNo;
			item.anchoredPosition = (direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

			onUpdateItem.Invoke(currentItemNo + instantateItemCount, item.gameObject);

			currentItemNo++;
		}

		while(anchoredPosition - diffPreFramePosition > -0.5 * itemScale) {
			diffPreFramePosition += itemScale;

			var item = itemList.Last.Value;
			itemList.RemoveLast();
			itemList.AddFirst(item);

			currentItemNo--;

			var pos = itemScale * currentItemNo;
			item.anchoredPosition = (direction == Direction.Vertical) ? new Vector2(0, -pos): new Vector2(pos, 0);
			onUpdateItem.Invoke(currentItemNo, item.gameObject);
		}
	}
    
    public void UpdateAllItems()
    {

        UpdateAllItemsPosition();


        int i = 0;
        foreach (var item in itemList)
        {
            onUpdateItem.Invoke(i + currentItemNo, item.gameObject);
            i++;
        }
    }

    public GameObject GetItem(int position)
    {
        return itemList.ElementAt(position - currentItemNo).gameObject;
    }

    public void Scroll(int position)
    {
        
        anchoredPosition = position * itemScale;

        scrollVeclocity = 0;

        UpdateAllItems();

    }

	[System.Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> {}
}
