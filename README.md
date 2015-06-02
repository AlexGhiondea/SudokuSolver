# Sudoku Solver lib

General Sudoku solver and generator library.

## What it can do
It can generate puzzles with any box size (2x3, 3x3, 2x2, 4x4, etc) and with any number of hints.

It can also solve any sudoku puzzle pretty fast.

Here is a how you can use it:

```csharp
var grid = SudokuPuzzle.Create(3, 3, 21);
Console.WriteLine(grid.PrettyPrint());

if (grid.SolveGrid())
    Console.WriteLine(grid.PrettyPrint());
```
This is the output:
```
|----------|----------|---------|
| .  .  .  | .  .  .  | .  .  . |
| .  .  3  | 6  .  .  | 7  4  5 |
| .  .  .  | .  .  .  | .  3  . |
|----------|----------|---------|
| 3  5  1  | .  7  .  | .  9  . |
| .  .  .  | .  6  1  | 3  .  . |
| .  .  .  | .  .  .  | 1  .  . |
|----------|----------|---------|
| 6  .  .  | 7  2  .  | .  .  . |
| .  .  .  | .  8  .  | 5  .  . |
| .  .  .  | .  5  .  | .  .  . |
|----------|----------|---------|

|----------|----------|---------|
| 8  4  7  | 5  9  3  | 2  6  1 |
| 2  9  3  | 6  1  8  | 7  4  5 |
| 5  1  6  | 2  4  7  | 8  3  9 |
|----------|----------|---------|
| 3  5  1  | 4  7  2  | 6  9  8 |
| 4  2  8  | 9  6  1  | 3  5  7 |
| 7  6  9  | 8  3  5  | 1  2  4 |
|----------|----------|---------|
| 6  8  5  | 7  2  4  | 9  1  3 |
| 9  3  4  | 1  8  6  | 5  7  2 |
| 1  7  2  | 3  5  9  | 4  8  6 |
|----------|----------|---------|
```

## How does it work

The solver uses constraints propagation to solve the puzzle. 

Before the solver starts solving the puzzle, it will create a model of the Sudoku puzzle. 
This model will keep, for each node in the puzzle, the available values for that node. As the puzzle is created from a string all the values corresponding to the initial puzzle nodes will be removed as potential values from the nodes's neighbour.

After this initial model is created, a Min-Heap is created based on the nodes in the puzzle that don't yet have a value. The Heap is using the number of possible values for a node as the key. This means that nodes with fewer choices are at the top of the Heap.

The algorithm will start taking nodes out of the Min-Heap and will try to set the value for the node. As each value is set for a node, that value is removed from its neighbours. This is the 'constraint propagation' part of the solver. By propagating these potential values to the neighbours, the set of potential values for the nodes is getting smaller and smaller. This leads to a very fast solver as the algorithm does not try values that are not even possible for a given node.
