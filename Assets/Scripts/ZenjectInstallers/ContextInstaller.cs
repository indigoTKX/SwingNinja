using UnityEngine;
using Zenject;

public class ContextInstaller : MonoInstaller
{
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private PlayerReference _player;
    [SerializeField] private PlayerAnimationController _playerAnimationController;
    
    public override void InstallBindings()
    {
        Container.Bind<GameStateManager>().FromInstance(_gameStateManager).AsSingle();
        Container.Bind<PlayerReference>().FromInstance(_player).AsSingle();
        Container.Bind<PlayerAnimationController>().FromInstance(_playerAnimationController).AsSingle();
    }
}