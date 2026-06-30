# CatRunner (Unity)

<img width="1280" height="720" alt="catrunner_preview" src="https://github.com/user-attachments/assets/a9f15443-b136-4cb5-a354-bfb3ba9a9fed" />

> **(Work in progress...)**

A 2D cat-themed **endless runner** built in **Unity** as a learning project.

The game's signature hook: on the title screen, a ball of yarn hangs from a rope — you **swipe to cut it**, the camera does a quick fast-travel across the room, and the cat drops into the runner loop.

---

> **Disclaimer:**
> This project is an original learning/portfolio piece.
> All sprites, names, and assets are either self-made or sourced from free libraries.
> It is **not intended for monetization** and is shared for educational purposes only.

---

## 🎮 About the Game

You control a cat that runs automatically to the right. Time your jumps and crouches to dodge obstacles (cactus, cucumber, ladder) and rack up points. The longer you survive, the faster the world moves.

The project was built to practice:

- Unity 2D fundamentals (URP, sprites, animation, physics)
- State-driven game loops (menu → intro → play → game over → menu)
- Procedural spawning (obstacles + decorative wall props)
- Input handling with the new **Input System**
- Camera transitions and game feel
- Clean, namespace-based C# architecture
- **Pixelart practice** — hand-painting every in-game visual (see below)

---

## 🎨 Pixelart & Assets

A core part of this project was **learning pixelart from scratch**, alongside the code. The author hand-painted the game's visual identity: the cat (idle / run / jump / crouch), the ball of wool, the yarn, the cactus, the cucumber, the ladder, the door, the window, the room tile, and every parallax background layer (ground, stars, moon, clouds).

**Most of the assets in the project are original work** created as part of the learning process. Everything under `Assets/_Project/Assets/` and `Assets/_Project/Prefabs/` is self-made. The only third-party content comes from the standard Unity packages shipped with the engine (URP defaults, TextMesh Pro examples, etc.).

---

## ✨ Features

- **Title-screen interaction:** swipe to cut the rope holding the yarn (mouse / touch + `TrailRenderer` linecast)
- **Procedural obstacles:** random spawn distance, fixed or random Y offset
- **Procedural wall props:** weighted random with repeat-penalty to avoid streaks
- **Infinite scrolling room:** tiled wall chunks recycled when off-camera
- **Parallax background:** multiple layers (ground, clouds, stars, moon) moving at different speeds
- **Player feel:** jump with coyote time, jump buffer, variable height (jump cut on release)
- **Crouch mechanic:** swaps colliders + slows the world when grounded, fast-falls when airborne
- **Speed scaling:** world accelerates over time, capped at a max speed
- **Score-by-speed:** points accumulate as a function of current world speed
- **State machine-driven flow:** `Idle → CutYarn → FastTravel → ArrivalIntro → Playing → GameOver → ReturnToMenu`
- **Modular architecture:** each concern lives in its own `CatRunner.*` namespace

---

## 🧱 Tech Stack

- **Engine:** Unity **6000.3.2f1** (Unity 6)
- **Language:** C#
- **Render Pipeline:** Universal Render Pipeline (URP) `17.3.0`
- **Input System:** `com.unity.inputsystem` `1.17.0`
- **2D Tooling:** 2D Animation, 2D Sprite, 2D PSDImporter, 2D SpriteShape, 2D Tilemap
- **UI:** UGUI + TextMesh Pro
- **Platform target:** WebGL / Windows / Android (configurable per build)

---

## 🗂️ Project Structure

```
Assets/
└── _Project/
    ├── Animations/        # Player.controller + Idle / Run / Jump_Up / Jump_Fall / Crouch clips
    ├── Assets/            # Cat sprites, parallax layers, room tile, yarn, props
    ├── Audio/             # (reserved for SFX / music)
    ├── Prefabs/
    │   ├── Obstacles/     # Obstacle_00, Obstacle_01, Obstacle_02, Obstacle_Ladder
    │   ├── WallProps/     # Door, Window
    │   ├── Yarn.prefab
    │   └── room_background.prefab
    ├── Scenes/            # SampleScene.unity
    └── Scripts/
        ├── Core/          # GameManager, GameState, ScoreManager, CameraController
        ├── Player/        # PlayerController, PlayerInputController, PlayerMoveMode
        ├── Enviroment/    # Spawners, Movers, ParallaxLayerMover, WallChunkRecycler
        ├── Data/          # WallPropData
        ├── Gameplay/      # ScoreBySpeed
        ├── Input/         # InputSystem_Actions (auto-generated)
        ├── Interfaces/    # IDespawnHandler
        ├── Menu/          # RopeController, SwipeCutter, YarnLinker
        └── UI/            # ScoreUI
```

---

## 🧠 Architecture Notes

The codebase is organized by domain using `CatRunner.*` namespaces, keeping responsibilities isolated:

| Namespace | Responsibility |
|---|---|
| `CatRunner.Core` | Global state, game loop, score, camera transitions |
| `CatRunner.Player` | Cat movement, jump, crouch, input binding |
| `CatRunner.Environment` | World spawning, recycling, parallax, despawning |
| `CatRunner.Gameplay` | Score-by-speed rules |
| `CatRunner.Menu` | Title screen: rope physics + swipe-to-cut interaction |
| `CatRunner.UI` | HUD bindings |
| `CatRunner.Interfaces` | `IDespawnHandler` contract for anything that can be recycled |

**Despawn contract** — anything that should be cleaned up when leaving the camera implements `IDespawnHandler`. A `DespawnTrigger` collider at the left edge of the screen invokes it, and the object self-destroys. This keeps spawners decoupled from object lifetime.

**State machine** — `GameManager` owns a single `GameState` and broadcasts `OnGameStateChanged`. Every system (player, camera, spawners, parallax, recycler) subscribes and reacts only to the states it cares about. Adding a new state is a matter of extending the enum + handling the case.

---

## 🚀 How to Run

### Option A — Open in Unity
1. Clone this repository:
   ```bash
   git clone <your-repo-url>
   ```
2. Open **Unity Hub**
3. Click **Add project** and select the cloned folder
4. Open the project with **Unity 6000.3.2f1** (or any Unity 6.x build)
5. Open `Assets/_Project/Scenes/SampleScene.unity`
6. Press **Play**

> Make sure the `InputSystem_Actions.inputactions` asset is the one referenced by `PlayerInputController` (default: project-wide, **Player** action map).

### Option B — Build

Use **File → Build Settings** and pick a target. WebGL and Windows are the most common targets; the project uses the URP so the URP asset shipped with Unity 6 works out of the box.

---

## 🎮 Controls

| Action | Input |
|---|---|
| Cut the rope (menu) | Mouse drag / touch swipe over the rope |
| Jump | `Space` / `W` / `↑` / Gamepad south button |
| Crouch (hold) | `Left Ctrl` / `S` / `↓` / Gamepad east button |

> Crouching on the ground **slows the world**; crouching mid-air **fast-falls**.

---

## 📚 Learning Notes

This project was made iteratively as part of a personal Unity roadmap. Main focus areas:

- Clean separation of concerns (no god-scripts)
- Event-driven communication between systems (`OnGameStateChanged`, `OnScoreChanged`, `IDespawnHandler`)
- Data-driven spawning (`WallPropData`, `ObstacleSpawnConfig`) so designers can tune content without touching code
- Game feel: coyote time, jump buffering, jump cut, crouch-as-world-slowdown
- Reusable menu→game transition pattern (rope physics + swipe + camera fast-travel)

---

## 🛠️ Extending the Project

A few natural next steps if you want to keep going:

- Add sound effects + music in `Assets/_Project/Audio/`
- Add a High Score system backed by `PlayerPrefs` (the `ScoreManager` event hook is already there)
- Add more obstacle prefabs and tune weights in `ObstacleSpawner`
- Add mobile touch support (the `SwipeCutter` already works with pointer events, but the swipe input could be promoted to a proper `InputAction`)

---

## 📄 License

This repository is shared for **learning and portfolio purposes only**.
You are free to explore the code, but please do not redistribute it as a commercial product or monetize derivative works.

---

## 👤 Author

Made by **P4TTT0**  
- GitHub: https://github.com/P4TTT0
- itch.io: https://p4ttt0.itch.io/

