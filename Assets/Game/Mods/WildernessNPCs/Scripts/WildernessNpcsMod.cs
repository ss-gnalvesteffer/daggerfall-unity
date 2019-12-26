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
    private const int MaxSpawnGroups = 5;
    private const float SpawnMinDistance = 20.0f;
    private const float SpawnMaxDistance = 150.0f;
    private const float SpawnCheckDistance = 50.0f;
    private const float MinimumUpdateIntervalInSeconds = 30.0f;
    private const float MaximumUpdateIntervalInSeconds = 60.0f * 5;

    private static readonly MobileTypes[] SpawnableMobileTypes =
    {
        MobileTypes.Thief,
        MobileTypes.GiantBat,
        MobileTypes.Orc,
        MobileTypes.GrizzlyBear,
        MobileTypes.Spider,
        MobileTypes.Rat,
        MobileTypes.SkeletalWarrior,
        MobileTypes.Knight,
        MobileTypes.Healer,
    };

    private static readonly Dictionary<MobileTypes, bool> MobileTypeAlliances = new Dictionary<MobileTypes, bool>
    {
        { MobileTypes.Thief, false },
        { MobileTypes.GiantBat, false },
        { MobileTypes.Orc, false },
        { MobileTypes.GrizzlyBear, false },
        { MobileTypes.Spider, false },
        { MobileTypes.Rat, false },
        { MobileTypes.SkeletalWarrior, false },
        { MobileTypes.Knight, true },
        { MobileTypes.Healer, true },
    };

    private static readonly Dictionary<MobileTypes, int> MobileTypeMaxSpawnCounts = new Dictionary<MobileTypes, int>
    {
        { MobileTypes.Thief, 5 },
        { MobileTypes.GiantBat, 5 },
        { MobileTypes.Orc, 5 },
        { MobileTypes.GrizzlyBear, 3 },
        { MobileTypes.Spider, 3 },
        { MobileTypes.Rat, 10 },
        { MobileTypes.SkeletalWarrior, 5 },
        { MobileTypes.Knight, 10 },
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
