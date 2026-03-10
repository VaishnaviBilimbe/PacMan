# Pac-Man 3D Project

A modern 3D take on the classic Pac-Man gameplay implemented in Unity.

## 🛠 Project Information
- **Unity Version**: 6000.0.64f1
- **Platform**: PC / WebGL

## 🎮 Controls
| Key | Action |
| --- | --- |
| **W / Up Arrow** | Move Forward / Up |
| **S / Down Arrow** | Move Backward / Down |
| **A / Left Arrow** | Move Left |
| **D / Right Arrow** | Move Right |

## 🕹 Gameplay Mechanics
- **Objective**: Navigate the maze, eat all the dots and energizers to progress.
- **Level Progression**: Successfully eating all dots unlocks the **Next Level**. Difficulty scales automatically.
- **Restart Logic**: If all lives are lost, you can **Play Again** to restart the current level. The game resets in-place without scene reloads.
- **Ghosts**: Four unique ghosts (Blinky, Pinky, Inky, Clyde) with different AI targeting behaviors.
- **Energizers**: Collecting an energizer turns ghosts blue (Frightened Mode), allowingคุณ to eat them for bonus points.

### Special Power-Ups
- **🔴 Booster Bottle (Red)**: Grants a temporary speed boost and allows Pac-Man to pass through walls (Ghost Mode).
- **🟢 Health Orb (Green)**: Grants temporary invincibility, preventing death when caught by ghosts.

## 📊 Features
- **Centralized Difficulty**: Scaling is managed via the `GameDifficultyData` ScriptableObject, controlling speeds and durations.
- **Dynamic HUD**: Real-time display of Player Name, **Current Level Score**, Lives, and Level number.
- **Score System**: 
    - **Current Score**: Shown during gameplay for the specific level.
    - **Overall Score**: Total score across all completed levels, submitted to the leaderboard upon game completion or failure.
- **Leaderboard**: Local high-score tracking with player names and ranks.
- **Maze Generation**: Runtime maze generation with automatic dot counting for progress tracking.

## ⚠️ Known Limitations
- **Ghost AI**: Movement is predominantly target-based; ghosts do not currently implement complex pathfinding (like A*) beyond their simple grid decisions.
- **Grid Alignment**: Movement requires strict alignment with the maze grid; stopping midway or clipping depends on the `MazeGenerator` setup.
- **Session Persistence**: While high scores persist via `PlayerPrefs`, the active game session (mid-level progress) is not saved between application restarts.
