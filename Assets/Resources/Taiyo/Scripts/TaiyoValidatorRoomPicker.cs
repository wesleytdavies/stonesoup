using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaiyoValidatorRoomPicker :Room
{
    public Room[] RoomChoices;


    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<Room> roomsThatMeetConstraints = new List<Room>();

        foreach (Room room in RoomChoices)
        {

            TaiyoValidatorRoom validatedRoom = room.GetComponent<TaiyoValidatorRoom>();

            if (validatedRoom.MeetsConstraints(requiredExits))
            {
                Debug.Log("ADD ROOM!");
                roomsThatMeetConstraints.Add(room);
            }
        }

        Debug.Log(roomsThatMeetConstraints.Count);
        return GlobalFuncs.randElem(roomsThatMeetConstraints).createRoom(requiredExits);
    }
}
