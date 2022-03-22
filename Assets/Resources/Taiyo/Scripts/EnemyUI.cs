using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyUI : MonoBehaviour
{

    // Start is called before the first frame update
    public Text friendlyValueText;
    public Text nonFriendlyValueText;

    public Text scoreText;

    private List<Tile> _enemyTiles;

    public float updateSpan = 1.0f;

    private float _timer = 0;

    private int _enemyCount = 0;

    private int _prevFriendCount = 0;

    static int _score = 0;

    public bool resetScore = false;

    void Start()
    {
        if (resetScore)
        {
            _score = 0;
            Destroy(this.gameObject);
            return;
        }
        this.gameObject.name = "EnemyUI";
        this.transform.parent = GameObject.Find("Canvas").transform;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(37, -26);
        this.transform.localScale = Vector3.one;
        _enemyTiles = new List<Tile>();
        Invoke("SetValue", 0.1f);


    }

    

    private void SetValue()
    {
        scoreText.text = _score.ToString() + "p";
        friendlyValueText.text = "0";
        var o = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject obj in o)
        {
            _enemyTiles.Add(obj.GetComponent<Tile>());
        }

        _enemyCount = 0;
        _prevFriendCount = 0;
        nonFriendlyValueText.text = o.Length.ToString();
    }

    public int GetEnemyCount()
    {
        return _enemyCount;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > updateSpan)
        {
            _timer = 0;
            int friendCount = 0;
            int nonFriendCount = 0;
            foreach(Tile t in _enemyTiles)
            {
                if(t != null)
                {
                    if (t.hasTag(TileTags.Friendly))
                        friendCount++;
                    else
                        nonFriendCount++;
                }
            }

            if(friendCount > _prevFriendCount)
            {
                _score += (friendCount - _prevFriendCount) * 100;
            }

            if(friendCount < _prevFriendCount)
            {
                _score -= (_prevFriendCount - friendCount) * 300;
            }

            scoreText.text = _score.ToString()+"p";

            _prevFriendCount = friendCount;


            _enemyCount = friendCount;
            friendlyValueText.text = friendCount.ToString();
            nonFriendlyValueText.text = nonFriendCount.ToString();
        }   
    }
}
