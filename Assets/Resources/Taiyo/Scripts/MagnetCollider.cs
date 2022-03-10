using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetCollider : MonoBehaviour
{

    private bool _hittingWall = false;
    private Collider2D[] _castResults;
    public float detectSpan = 0.03f;
    private float _timer = 0;

    private void Start()
    {
        _castResults = new Collider2D[10];
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(detectSpan > _timer)
        {
            return;
        }
        else
        {
            _timer = 0;
        }

        int numResults = Physics2D.OverlapCircleNonAlloc(transform.position, 1, _castResults);

        _hittingWall = false;
        
        for (int i = 0; i < numResults && i < _castResults.Length; i++)
        {

            Collider2D result = _castResults[i];
            Tile otherTile = result.GetComponent<Tile>();
            if (otherTile != null && otherTile.hasTag(TileTags.Wall))
            {
                _hittingWall = true;
                break;
            }
        }
    }

    public bool IsHittingWall()
    {
        return _hittingWall;
    }

}
