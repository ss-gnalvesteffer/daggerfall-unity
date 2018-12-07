using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;
using UnityEngine;

namespace DaggerfallWorkshop
{
    public class DaggerfallBed : MonoBehaviour
    {
        void Start()
        {
        }

        internal void Rest()
        {
            //PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            //PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;

            //int buildingKey = playerEnterExit.BuildingDiscoveryData.buildingKey;

            //int mapId = playerGPS.CurrentLocation.MapTableData.MapId;
            //RoomRental_v1 room = GameManager.Instance.PlayerEntity.GetRentedRoom(mapId, buildingKey);
            //if (room != null)
            //{
            //    PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
            //    room.allocatedBed = playerMotor.transform.position;
            //    Debug.Log("Changing allocated bed");
            //}
            //else
            //Debug.Log("Not in a room");
            GameManager.Instance.PlayerEnterExit.IsPlayerInSelectedBed = true;
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenRestWindow);
        }
    }
}
