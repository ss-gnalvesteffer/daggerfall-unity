using System.Collections;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;

public class NpcHealthIndicatorMod : MonoBehaviour
{
    public static Mod Mod;

    private GameObject _npcHealthIndicatorPrefab;

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        Mod = initParams.Mod;
        var gameObject = new GameObject(Mod.Title);
        gameObject.AddComponent<NpcHealthIndicatorMod>();
    }

    void Awake()
    {
        _npcHealthIndicatorPrefab = Mod.GetAsset<GameObject>("Assets/NPCHealthIndicator.prefab");
        StartCoroutine(EntityBehavioursUpdateLoop());
        Mod.IsReady = true;
    }

    private IEnumerator EntityBehavioursUpdateLoop()
    {
        while (true)
        {
            var entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            foreach (var entityBehaviour in entityBehaviours)
            {
                if (entityBehaviour.transform.CompareTag("Player"))
                {
                    continue;
                }
                var hasHealthIndicator = entityBehaviour.transform.Find("NPCHealthIndicator(Clone)") != null;
                if (!hasHealthIndicator)
                {
                    var npcHealthIndicator = Instantiate(_npcHealthIndicatorPrefab, entityBehaviour.transform);
                    npcHealthIndicator.AddComponent<NpcHealthIndicator>();
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
