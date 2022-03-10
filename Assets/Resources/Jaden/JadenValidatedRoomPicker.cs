using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JadenValidatedRoomPicker : Room
{
    public Room[] RoomChoices;

    public override Room createRoom(ExitConstraint requiredExits) {
        
        List<Room> roomsMeetingConstraints = new List<Room>();

        foreach(Room r in RoomChoices) {
                                            //would want to make this more robust
            JadenValidatedRoom validatedRoom = r.GetComponent<JadenValidatedRoom>();
            if(validatedRoom.MeetsConstraints(requiredExits)) {
                roomsMeetingConstraints.Add(validatedRoom);
            }
        }
        
        return GlobalFuncs.randElem(roomsMeetingConstraints);
    }
}
