# SkyscraperGame

An offline version of the puzzle game Skyscrapers (see [here](https://www.puzzle-skyscrapers.com/) or [here](https://www.brainbashers.com/skyscrapers.asp) build with C# using WPF for a graphical user interface.

![NewGame](images/InProgress.JPG)


## Rules
![NewGame](images/FinishedPuzzle.JPG)

Like in Sudoku, the goal is to fill a square grid with numbers, so that every column and row contains each number exactly once.
Unlike Sudoku, there are no boxes, instead, the numbers on the outside of the grid indicate how many numbers, or "skyscrapers" are visible from that position.
A skyscraper is visible from a position if it is taller (i.e. the number is bigger) than all skyscrapers in front of it.

The puzzle ends if you have filled in all cells and no conditions are left to be checked or if proved that the puzzle is infeasible (more on that below).

## New Game
![NewGame](images/NewGame.JPG)

The game starts with a randomly generated puzzle.
By clicking new Game you can generate a new puzzle, but will be asked to confirm beforehand to avoid accidentally deleting your progress.

There are different settings you can play around with to adapt the difficulty.
You can change the grid size, the number of already set numbers and the number of skyscraper conditions.
By default all puzzles will have a solution. If you want to try out a puzzle that is infeasible, you can check the "Allow Infeasible" box.

Additionally it is possible to generate the same puzzle again, if you set a random seed. This can be useful if you want to share a puzzle with someone else.

## Controls

### Setting a Number / Skyscraper
![NewGame](images/CellDialog.JPG)

Left click on a cell to open a dialog to enter a number with your keyboard.
Press the cancel button or any other key to close the dialog without changing anything.

The game automatically tracks valid numbers and does not allow you to enter duplicate numbers in a row or column,
additionally numbers that have already been proved to be invalid will not be allowed either.
However the Skyscraper condition will only be checked when you request so explicitely.

### Checking a Skyscraper condition
Click on the surrounding cells to check if that particular Skyscraper condition can still be satisfied, if not,
you have to undo your moves until you reach a feasible state again.
Once you insert a number, the surrounding cells (which start out green) will be updated to reflect the new information
For convenience, there is the "Check All" button to check all outstanding conditions at the end.

### Undo Moves
You can undo your moves one by one, either by clicking the undo button, or by pressing Escape or Backspace.

## Installation

The program is available as a standalone executable (on Windows x64-86), which can be downloaded from the releases.
You can also use VisualStudio or dotnet to build the program yourself, by cloning the repository and building the solution contained in dotnet_solution with Visual Studio or running dotnet build in the terminal.

## Work in Progress

### Loading and Saving

In the future it will be possible to save and load games. And share puzzles with others.

## Automatic solving

A automatic solver is in development. Also take a look [here](https://github.com/TonyCongqianWang/42HeilbronnCPiscine/tree/main/SkyscraperSolver) for a solver written C with a command line interface.