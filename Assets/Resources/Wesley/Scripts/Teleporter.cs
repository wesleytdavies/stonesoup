using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the teleporter teleports the player as long as it doesn't teleport them into a wall
public class Teleporter : Tile {
    protected bool _isTeleporting = false;
    private const float _teleportationRadius = TILE_SIZE * 2f;

    public override void useAsItem(Tile tileUsingUs) {
        if (_isTeleporting) {
            return;
        }
        //TODO: prevent the player from teleporting outside the map
        float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x);
        Vector3 edgeOfRadiusPosition = tileUsingUs.transform.position;
        edgeOfRadiusPosition.y += Mathf.Sin(aimAngle) * _teleportationRadius;
        edgeOfRadiusPosition.x += Mathf.Cos(aimAngle) * _teleportationRadius;
        if (!tileAtPoint(edgeOfRadiusPosition, TileTags.Wall)) {
            _isTeleporting = true;
            tileUsingUs.transform.position = edgeOfRadiusPosition;
            _isTeleporting = false;
            return;
        }
        else {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _tileHoldingUs.aimDirection);
            foreach(RaycastHit2D hit in hits) {
                if (hit.collider.gameObject.GetComponent<Tile>().hasTag(TileTags.Wall)) {
                    _isTeleporting = true;
                    tileUsingUs.transform.position = hit.point; //TODO: player collider will teleport into wall's collider and allow them to walk between walls
                    _isTeleporting = false;
                    return;
                }
            }
        }
    }

    void Update() {
        if (_tileHoldingUs != null) {
            float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x);
            transform.localRotation = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg - 90f);
        }
    }

    public override void dropped(Tile tileDroppingUs) {
        if (_isTeleporting) {
            return;
        }
    }
}
