# LeoEcs Lite Tooling

Leo Ecs Lite Toolset for Unity3D

- [LeoEcs Lite Tooling](#leoecs-lite-tooling)
  - [Intro](#intro)
    - [Configurations](#configurations)
- [Ecs Tools](#ecs-tools)
  - [Ecs Features](#ecs-features)
  - [ECSDI Attribute](#ecsdi-attribute)
    - [Inject World, Pools \&\& Global Data](#inject-world-pools--global-data)
    - [Ecs Filters Injection](#ecs-filters-injection)
    - [Aspects Injection](#aspects-injection)
  - [Entity Prefab Converter](#entity-prefab-converter)
  - [Ecs Entity Browser](#ecs-entity-browser)
- [Unity Package Installation](#unity-package-installation)


## Intro

### Configurations

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite1.png)


# Ecs Tools

## Ecs Features

- Scriptable Object Feature

```cs
/// <summary>
/// scriptable object demo feature
/// </summary>
[CreateAssetMenu(menuName = "Game/Feature/Gameplay/IdleLevelsFeature")]
public class IdleLevelsFeature : BaseLeoEcsFeature
{
    public LevelsDataAsset LevelsDataAsset;

    public override async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
    {
        // level up
        ecsSystems.Add(new LevelUpAfterWinSystem());
        
        // update level configs
        ecsSystems.Add(new UpdateLevelConfigurationSystem());
        ecsSystems.Add(new SetSpawnDataSystem());
        ecsSystems.Add(new SetScoreSystem());
        
        // start next level 
        ecsSystems.Add(new NextLevelSystem());
        ecsSystems.Add(new ResetScoreAfterWinSystem());
        ecsSystems.Add(new RemoveIgnoreAfterWinStateSystem());
        
        // creep progression
        ecsSystems.Add(new SetCreepGrowCoefByLevelSystem());
        ecsSystems.Add(new SetCreepHealthByLevelSystem());
        
        ecsSystems.DelHere<UpdateConfigurationByLevelRequest>();
        ecsSystems.DelHere<NextLevelRequest>();
    }
}
```

- Regular class Feture

```cs
/// <summary>
/// regular class demo ecs feature
/// </summary>
[Serializable]
public class IdleLevelsFeature : LeoEcsFeature
{
    protected override async UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
    {
        // level up
        ecsSystems.Add(new LevelUpAfterWinSystem());
        
        // update level configs
        ecsSystems.Add(new UpdateLevelConfigurationSystem());
        ecsSystems.Add(new SetSpawnDataSystem());
        ecsSystems.Add(new SetScoreSystem());
        ecsSystems.Add(new SetRewardsSystem());
        ecsSystems.Add(new ConfigurationsSetEventSystem());
        
        // update rewards (after energy update)
        ecsSystems.Add(new WaitScoreAndSetRewardsSystem());
        
        // start next level 
        ecsSystems.Add(new NextLevelSystem());
        ecsSystems.Add(new ResetScoreAfterWinSystem());
        ecsSystems.Add(new RemoveIgnoreAfterWinStateSystem());
        
        // creep progression
        ecsSystems.Add(new SetCreepGrowCoefByLevelSystem());
        ecsSystems.Add(new SetCreepHealthByLevelSystem());
        
        ecsSystems.DelHere<UpdateConfigurationByLevelRequest>();
        ecsSystems.DelHere<NextLevelRequest>();
    }
}
```

## ECSDI Attribute

Allow to inject data right to the system and aspects. Less code more focus on the feature

### Inject World, Pools && Global Data

**world globale data example**

```cs
    var world = ecsSystems.GetWorld();
    world.SetGlobal(LevelsDataAsset.Data);
    var levelData = world.GetGlobal<LevelsData>();
```

```cs

    [ECSDI]
    public class LevelDataInitSystem : IEcsRunSystem
    {
        private EcsWorld _world;// defaul world injected into system
        private LevelsData _map; // level data injected from global by type
        private EcsPool<LevelComponent> _levelPool;// leve pool auto injected

        public void Run(IEcsSystems systems){...}
    }

```

### Ecs Filters Injection

```cs
    [ECSDI]
    public class LevelUpAfterWinSystem : IEcsRunSystem
    {
        private EcsWorld _world;//defaul world injected into system
        private EcsPool<LevelComponent> _levelPool;// leve pool auto injected
        
        private EcsFilterInject<Inc<LevelComponent>> _levelFilter;//filter auto injection
        
        private EcsFilterInject<Inc<PlayerComponent,LevelComponent>,
            Exc<LevelCompletedComponent>> _playerFilter;


        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in _levelFilter.Value)
            {
                foreach (var playerEntity in _playerFilter.Value)
                {
                    ...
                }
            }
        }
    }  
```

### Aspects Injection

```cs

    [ECSDI]
    public class LevelAspect : EcsAspect
        where TComponent : struct
    {
        //components
        
        /// <summary>
        /// current state
        /// </summary>
        public EcsPool<LevelComponent> Level;
        public EcsPool<LifeTimeComponent> LifeTime;
        public EcsPool<LevelStatusComponent> LevelStatus;
        
        //filters
        public EcsFilterInject<Inc<LevelComponent>> LevelFilter;//filter auto injection
        
        public EcsFilterInject<Inc<PlayerComponent,LevelComponent>,
            Exc<LevelCompletedComponent>> PlayerFilter;
    }

    [ECSDI]
    public class LevelDataInitSystem : IEcsRunSystem
    {
        private EcsWorld _world;// defaul world injected into system
        private LevelsData _map; // level data injected from global by type
        private LevelAspect _levelAspect;// leve pool auto injected

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in _levelAspect.LevelFilter.Value)
            {
                ref var levelComponent = ref _levelAspect.Level.Get(levelEntity);
                foreach (var playerEntity in _levelAspect.PlayerFilter.Value)
                {
                    ...
                }
            }
        }
    }

```

## Entity Prefab Converter

Convert Unity prefab to ecs entity.

Supports:
- MonoBehaviour Converters
- Scriptble Objects shared configs
- Serializable converters

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite5.png)

## Ecs Entity Browser

- Menu:

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite2.png)

- Entity Browser

1. Entity Filtering By: entity name, component type, entity id
2. Runtime Entity editing and component data validation

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite3.png)

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite4.png)

# Unity Package Installation

**Odin Inspector or Tri-Inspector recommended to usage with this Package (https://odininspector.com | https://github.com/codewriter-packages/Tri-Inspector)**

```json
 - coming soooooon
```



