using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;

public class WildernessNpcsMod : MonoBehaviour
{
    private const int MaxSpawnGroups = 10;
    private const float SpawnMinDistance = 50.0f;
    private const float SpawnMaxDistance = 200.0f;
    private const float SpawnCheckDistance = 100.0f;
    private const float MinimumUpdateIntervalInSeconds = 10.0f;
    private const float MaximumUpdateIntervalInSeconds = 60.0f;

    private static readonly MobileTypes[] SpawnableMobileTypes =
    {
        MobileTypes.Rogue,
        MobileTypes.Thief,
        MobileTypes.GiantBat,
        MobileTypes.Orc,
        MobileTypes.OrcShaman,
        MobileTypes.Battlemage,
        MobileTypes.Centaur,
        MobileTypes.GrizzlyBear,
        MobileTypes.Spider,
        MobileTypes.Rat,
        MobileTypes.SkeletalWarrior,
        MobileTypes.Knight,
        MobileTypes.Archer,
        MobileTypes.Healer,
    };

    private static readonly Dictionary<MobileTypes, bool> MobileTypeAlliances = new Dictionary<MobileTypes, bool>
    {
        { MobileTypes.Rogue, false },
        { MobileTypes.Thief, false },
        { MobileTypes.GiantBat, false },
        { MobileTypes.Orc, false },
        { MobileTypes.OrcShaman, false },
        { MobileTypes.Battlemage, false },
        { MobileTypes.Centaur, false },
        { MobileTypes.GrizzlyBear, false },
        { MobileTypes.Spider, false },
        { MobileTypes.Rat, false },
        { MobileTypes.SkeletalWarrior, false },
        { MobileTypes.Knight, true },
        { MobileTypes.Archer, true },
        { MobileTypes.Healer, true },
    };

    private static readonly Dictionary<MobileTypes, int> MobileTypeMaxSpawnCounts = new Dictionary<MobileTypes, int>
    {
        { MobileTypes.Rogue, 10 },
        { MobileTypes.Thief, 5 },
        { MobileTypes.GiantBat, 10 },
        { MobileTypes.Orc, 10 },
        { MobileTypes.OrcShaman, 5 },
        { MobileTypes.Battlemage, 5 },
        { MobileTypes.Centaur, 10 },
        { MobileTypes.GrizzlyBear, 3 },
        { MobileTypes.Spider, 5 },
        { MobileTypes.Rat, 20 },
        { MobileTypes.SkeletalWarrior, 5 },
        { MobileTypes.Knight, 20 },
        { MobileTypes.Archer, 20 },
        { MobileTypes.Healer, 10 },
    };

    private static Mod _mod;
    private static GameObject _gameObject;
    private Vector3 _previousPosition;

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        _mod = initParams.Mod;
        _gameObject = new GameObject(_mod.Title);
        _gameObject.AddComponent<WildernessNpcsMod>();
    }

    private void Awake()
    {
        _mod.IsReady = true;
    }

    private void Start()
    {
        _previousPosition = GetPlayerPosition();
        StartCoroutine(UpdateLoop());
    }


    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            var currentPosition = GetPlayerPosition();
            var deltaDistance = (currentPosition - _previousPosition).magnitude;
            if (deltaDistance >= SpawnCheckDistance && IsPlayerInWilderness())
            {
                for (var spawnGroupIndex = 0; spawnGroupIndex < Random.Range(0, MaxSpawnGroups); ++spawnGroupIndex)
                {
                    var mobileTypeToSpawn = SpawnableMobileTypes[Random.Range(0, SpawnableMobileTypes.Length)];
                    var isMobileTypeAnAlly = MobileTypeAlliances[mobileTypeToSpawn];
                    var maxSpawnCountForMobileType = MobileTypeMaxSpawnCounts[mobileTypeToSpawn];

                    var spawnerGameObject = new GameObject("Spawner");
                    spawnerGameObject.transform.parent = _gameObject.transform;
                    spawnerGameObject.transform.position = GameManager.Instance.PlayerObject.transform.position;
                    var foeSpawner = spawnerGameObject.AddComponent<FoeSpawner>();
                    foeSpawner.FoeType = mobileTypeToSpawn;
                    foeSpawner.AlliedToPlayer = isMobileTypeAnAlly;
                    foeSpawner.MinDistance = SpawnMinDistance;
                    foeSpawner.MaxDistance = SpawnMaxDistance;
                    foeSpawner.SpawnCount = Random.Range(1, maxSpawnCountForMobileType);
                }
                _previousPosition = currentPosition;
            }
            yield return new WaitForSeconds(Random.Range(MinimumUpdateIntervalInSeconds, MaximumUpdateIntervalInSeconds));
        }
    }

    private Vector3 GetPlayerPosition()
    {
        return GameManager.Instance.PlayerObject.transform.position;
    }

    private bool IsPlayerInWilderness()
    {
        var playerGps = GameManager.Instance.PlayerGPS;

        if (!playerGps.IsPlayerInLocationRect)
        {
            return true;
        }

        switch (playerGps.CurrentLocationType)
        {
            case DFRegion.LocationTypes.DungeonKeep:
            case DFRegion.LocationTypes.DungeonLabyrinth:
            case DFRegion.LocationTypes.DungeonRuin:
            case DFRegion.LocationTypes.Coven:
            case DFRegion.LocationTypes.HomePoor:
            case DFRegion.LocationTypes.Graveyard:
            case DFRegion.LocationTypes.HomeFarms:
            case DFRegion.LocationTypes.HomeWealthy:
            case DFRegion.LocationTypes.Tavern:
            case DFRegion.LocationTypes.TownCity:
            case DFRegion.LocationTypes.TownHamlet:
            case DFRegion.LocationTypes.TownVillage:
            case DFRegion.LocationTypes.ReligionTemple:
            case DFRegion.LocationTypes.ReligionCult:
            case DFRegion.LocationTypes.HomeYourShips:
                return false;
            default:
                return true;
        }
    }
}
