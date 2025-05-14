using System.Collections.Generic;
using Newtonsoft.Json;

public class Farming 
{
    public enum SpawnType
    {
        None = -1,
        
        AlwaysSpawn,
        DefaultSpawn,
        TutorialSpawn,
    }

    public enum SpawnFacility
    {
        None = -1,
        
        AlwaysSpawnArea,
        SecurityRoom,
        MainHall,
        MainLobby,
        Office,
        Main3FHall,
        Sub3FHall,
        Main2FHall,
        Sub2FHall,
        DiningRoom,
        SubLobby,
        Cafe,
    }
    
    public class DayData
    {
        public int Nday;
        public List<Item> Items;
    }
    
    public class Item
    {
        public SpawnType SpawnType;
        public SpawnFacility SpawnFacility;
        public SpawnItemData SpawnItemData;
    }
    
    public class SpawnItemData
    {
        public int SpawnItem;
        public float SpawnPer;
    }
}


