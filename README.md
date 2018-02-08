# Game Prototyping

Version 0.2

New things:

- Rewrote the whole game with a "networked" thought.
- Player HP added (Also the lose condition, you can't move when playerHealth <= 0)
- Weapon damage added
- Solved the weapon-still-in-hand-after-throw bug (Damn it is really a hard bug because you have to synchronize the parentness of your weapon through network)

Some problem:

- PowerUp haven't been added
- Start UI haven't been added
- No super jump



PS: All network-related prefab should be put on the `Resources` folder.