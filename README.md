#Planner for 2D grid maps using A* with jump point search

##Building the library
Open the solution file in Visual Studio. Click on "Build Solution" in the "Build" menu. 
To build with command line, change the directory to the root of the repository, then use the command:
```bash
csc /target:library /out:aStar\bin\Debug\AStar.dll /r:aStar\lib\C5.dll aStar\*.cs
```
The library will be built as `AStar.dll` in `aStar\bin\Debug` or any other directory specified in the `\out` option.

##Usage
In Visual Studio, add a reference to `AStar.dll` your project through the Solution Explorer. 
If you're compiling with command line, use the `\r` option to reference `AStar.dll`. 

By default, the planner uses jump point search and Eucledian distance as the heuristic.
To use vanilla A* serach, set the `JPS` field to `false`. To use Manhattan distance as
the heuristic, set the `Fast` property to `true`.

Note that using Manhattan distance as heuristic does not guarantee the shortest path, but
can reduce the search area when using vanilla A*.

##Sample code
```csharp
/// Create a planner with a 10 by 10 grid with no obstacles
/// and using only the first quadrant.
/// Set the 2nd argument to true to use all four quandrants.
Point mapDimensions = new Point(20, 20);
AStar.Planner pathFinder = new Planner(mapDimensions, false);

/// Adding obstacles
for (int i = 1; i < 19; i++)
{
	pathFinder.AddObstacle(new Point(i, 5));
}

/// Find a path between point (0, 0) and (9, 9)
pathFinder.FindPath(new Point(0, 0), new Point(9, 9));

Console.WriteLine("Path length: " + pathFinder.PathLength);
Console.WriteLine("Cells in min path from start to goal: ");
foreach (Point p in pathFinder.Path) {  Console.WriteLine(p); }

Console.WriteLine("\n+ ASCII printout (JPS): ");
Console.WriteLine(pathFinder.Ascii());

/// Using vanilla A* instead of jump point serach
pathFinder.JPS = false;
pathFinder.FindPath(new Point(0, 0), new Point(9, 9));
Console.WriteLine("\n+ ASCII printout (non JPS): ");
Console.WriteLine(pathFinder.Ascii());
```

##Sample output
Output from the sample code above: 
```
Path length: 15.6568542494924
Cells in min path from start to goal: 
{X=0,Y=0}
{X=0,Y=1}
{X=0,Y=2}
{X=0,Y=3}
{X=0,Y=4}
{X=0,Y=5}
{X=1,Y=6}
{X=2,Y=7}
{X=3,Y=8}
{X=4,Y=9}
{X=5,Y=9}
{X=6,Y=9}
{X=7,Y=9}
{X=8,Y=9}
{X=9,Y=9}

+ ASCII printout (JPS): 
----------------------
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|    *****g          |
|   *                |
|  *                 |
| *                  |
|*XXXXXXXXXXXXXXXXXX |
|*   ·               |
|*                   |
|*                   |
|*·                  |
|s·                  |
----------------------

+ ASCII printout (non JPS): 
----------------------
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|                    |
|    ·····g          |
|   ···***           |
|  ·***··            |
|·**····             |
|*XXXXXXXXXXXXXXXXXX |
|*·········          |
|*········           |
|*·······            |
|*······             |
|s······             |
----------------------
```

##Running the GUI

Set `MinotaurPathfinder` as a "StartUp Project" under the "Project" menu 
in Visual Studio before clicking "Start". 
Click and drag on the grid to draw and erase obstacles. 
The start (red) and goal (green) can dragged around to.

##Known issues

For large grid maps bigger than ~4000000 cells, a `System.StackOverflowException` or 
`System.OutOfMemoryException` may be thrown.
The stack overflow is due to the recursive function used to find the jump points, and can
be avoided by either setting a larger stack size, or setting the `JPS` field to `false`.
There does not seem to be a way around the out of memory exception, which is due to
an inability to allocate enough space to for the grid map.
