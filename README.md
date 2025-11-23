# Battleship Game (Ναυμαχία)

## Overview
This project is a **Windows Forms game in C# (.NET Framework)** that simulates the classic **Battleship (Ναυμαχία)** board game.  
It features a simple, interactive user interface where the player competes against a computer-controlled opponent (AI).  

The application is built using **object-oriented design**, clearly separating **game logic** from **user interface (UI)** components.

---

## Authors
- **Stoikos Ioannis Panagiotis**  
- **Anargyrou Lamprou Aikaterini** 

 *Course:* Human–Computer Interaction / Software Development  
 *Semester:* Spring 2024–2025  

---

## Key Features
 Play against an AI opponent  
 Dynamic placement of ships on a 10×10 grid  
 Real-time visual feedback (hit, miss, sunk)  
 Automatic win/loss detection  
 New game initialization button  
 Clear separation between **UI** and **game logic**  

---

## Project Architecture
The application is divided into **four core classes**:

| Class | Role |
|--------|------|
| **MainForm** | Manages the Windows Forms UI and main game loop |
| **Board** | Handles ship placement, hits/misses, and victory conditions |
| **Ship** | Represents a single ship (name, size, positions, hit tracking) |
| **Program** | Entry point that launches the `MainForm` |

This modular approach follows **MVC-like design principles**, promoting maintainability and future expandability.

---

## Class Overview

### Ship.cs
Represents an individual ship with the following properties:

- `Name` – Ship name (e.g., Aircraft Carrier, Destroyer, Battleship, Submarine)  
- `Size` – Number of grid cells occupied (5, 4, 3, or 2)  
- `Cells` – List of `(row, column)` positions on the grid  
- `Hits` – Set of hit coordinates  

#### Methods
- **IsSunk()** – Returns `true` if the number of hits equals the ship’s size.  

> The `Ship` class is purely a **data container** with minimal logic — it does not handle placement or UI.

---

### Board.cs
Manages the logical game board for one player (player or AI).

#### Properties
- `Map` – 2D array (`char[,]`) representing grid cells  
  - `'.'` = empty  
  - `'S'` = ship  
  - `'X'` = hit  
  - `'-'` = miss  
- `Ships` – List of all `Ship` objects placed on the board  
- `rows`, `cols` – Dimensions (typically 10×10 grid)  
- `Random rnd` – For random ship placement and AI moves  

#### Key Methods
- **PlaceShip(Ship s)** – Randomly places a ship on the grid ensuring:
  - It fits within bounds.
  - It doesn’t overlap with other ships.
- **Shoot(int r, int c, out Ship hitShip)** – Processes a shot:
  - Marks hits and misses.
  - Updates the ship’s `Hits` list.
  - Returns `true` for hit, `false` for miss.
- **GetShipAt(r, c)** – Returns the ship located at a specific cell.
- **AllSunk()** – Checks whether all ships on the board have been sunk.

---

### MainForm.cs (User Interface)
The **MainForm** class extends `Form` and acts as the main control center for gameplay.

#### UI Components
- **Two DataGridViews:**
  - `grdPlayer` – Displays the player’s board with visible ships.
  - `grdEnemy` – Displays the opponent’s board (ships hidden).
- **Labels:**
  - `lblStatus` – Displays messages such as “Your turn!”, “Hit!”, or “You win!”.  
  - `lblEnemyList` / `lblPlayerList` – Show lists of sunk ships.  
- **Button:**
  - `btnNew` – Starts a new game.  

---

### Game Logic
#### StartGame()
- Creates two new `Board` instances (player & enemy).
- Randomly places all ships on both boards.
- Resets UI and internal counters.
- Displays ship codes (e.g., “ΑΕ” for Aircraft Carrier) on the player’s grid.

#### Player Turn (`GrdEnemy_CellClick`)
- Checks that the clicked cell is valid and not already targeted.
- Calls `enemyBoard.Shoot(row, col)`:
  - **Hit:** Marks cell red, shows “X”, and keeps turn if the player continues hitting.
  - **Miss:** Marks cell light gray/green and passes turn to the AI.
- When all enemy ships are sunk, displays a “Victory” message.

#### Computer Turn
- Chooses random untargeted cells using `aiTried`.
- Calls `playerBoard.Shoot()` and updates the player’s grid:
  - **Hit:** Marks red and continues turn.
  - **Miss:** Ends the AI’s turn.
- If all player ships are sunk → “You Lose” message.

#### Additional Methods
- **UpdateSunkLabels()** – Refreshes lists of sunk ships for both players.  
- **CenterLayout()** – Centers grids and labels dynamically when resizing.

---

### Program.cs
The entry point that starts the application:

```csharp
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MainForm());
}
