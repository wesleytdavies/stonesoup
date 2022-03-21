using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The teleporter teleports the player as long as it doesn't teleport them into a wall.
/// </summary>
public class Teleporter : Tile {
    //TODO: MAYBE let player charge up teleporter to go further
    //TODO: MAYBE add durability to teleporter so it can't be abused
    protected bool _isTeleporting = false;
    private const float _teleportationRadius = TILE_SIZE * 2f;

    public override void useAsItem(Tile tileUsingUs) {
        if (_isTeleporting) {
            return;
        }
        float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x);
        Vector3 edgeOfRadiusPosition = tileUsingUs.transform.position;
        edgeOfRadiusPosition.y += Mathf.Sin(aimAngle) * _teleportationRadius;
        edgeOfRadiusPosition.x += Mathf.Cos(aimAngle) * _teleportationRadius;
        if (!tileAtPoint(edgeOfRadiusPosition, TileTags.Wall)) {
            //prevent the player from teleporting outside the map
            Vector2 edgeOfRadiusGridPosition = toGridCoord(edgeOfRadiusPosition.x, edgeOfRadiusPosition.y);
            if (IsPointInGrid(edgeOfRadiusGridPosition)) {
                TeleportTile(tileUsingUs, edgeOfRadiusPosition);
                return;
            }
            else {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _tileHoldingUs.aimDirection);
                foreach (RaycastHit2D hit in hits) {
                    if (hit.collider.gameObject.GetComponent<BoxCollider2D>() && !hit.collider.gameObject.GetComponent<Tile>()) {
                        TeleportTile(tileUsingUs, hit.point);
                        return;
                    }
                }
            }
        }
        else {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _tileHoldingUs.aimDirection);
            foreach(RaycastHit2D hit in hits) {
                if (hit.collider.gameObject.GetComponent<Tile>().hasTag(TileTags.Wall)) {
                    TeleportTile(tileUsingUs, hit.point); //TODO: player collider will teleport into wall's collider and allow them to walk between walls
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
        base.dropped(tileDroppingUs);
    }

    protected void TeleportTile(Tile tileToTeleport, Vector3 destination) {
        _isTeleporting = true;
        tileToTeleport.transform.position = destination;
        _isTeleporting = false;
    }

    private bool IsPointInGrid(Vector2 point) {
        if (point.x < 0 || point.x > LevelGenerator.ROOM_WIDTH * 4 - 1) {
            return false;
        }
        if (point.y < 0 || point.y > LevelGenerator.ROOM_HEIGHT * 4 - 1) {
            return false;
        }
        return true;
    }
}
