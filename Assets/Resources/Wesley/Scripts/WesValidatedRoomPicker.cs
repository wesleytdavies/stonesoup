using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WesValidatedRoomPicker : Room
{
    public Room[] roomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<Room> roomsThatMeetConstraints = new List<Room>();
        foreach (Room room in roomChoices)
        {
            WesValidatedRoom validatedRoom = room.GetComponent<WesValidatedRoom>();
            if (validatedRoom.MeetsConstrants(requiredExits))
            {
                roomsThatMeetConstraints.Add(validatedRoom);
            }
        }
        Room randomRoom = GlobalFuncs.randElem(roomsThatMeetConstraints);
        return randomRoom.createRoom(requiredExits);
    }
}
