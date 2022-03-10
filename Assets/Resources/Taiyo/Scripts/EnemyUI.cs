using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyUI : MonoBehaviour
{

    // Start is called before the first frame update
    public Text friendlyValueText;
    public Text nonFriendlyValueText;

    void Start()
    {
        this.gameObject.name = "EnemyUI";
        this.transform.parent = GameObject.Find("Canvas").transform;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(37, -26);
        this.transform.localScale = Vector3.one;

        Invoke("SetValue", 0.1f);
    }

    private void SetValue()
    {
        friendlyValueText.text = "0";
        nonFriendlyValueText.text = GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
