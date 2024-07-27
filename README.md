# LeoEcs Lite Tooling

Leo Ecs Lite Toolset for Unity3D

- [LeoEcs Lite Tooling](#leoecs-lite-tooling)
  - [Intro](#intro)
    - [Configurations](#configurations)
    - [Demo Ecs Features](#demo-ecs-features)
- [Unity Package Installation](#unity-package-installation)


## Intro

### Configurations

![](https://github.com/UnioGame/UniGame.LeoEcsLite/blob/master/GitAssets/ecslite1.png)


### Demo Ecs Features

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

# Unity Package Installation

**Odin Inspector or Tri-Inspector recommended to usage with this Package (https://odininspector.com | https://github.com/codewriter-packages/Tri-Inspector)**

```json
 - coming soooooon
```



