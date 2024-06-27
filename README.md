# FebucciAssignments
Apply to Febucci's Unity Tools Programmer position

## Assignment 01 - Path to wall

Permits to create wall mesh on random generated path.
In order to work, you need to a GameObject in scene with PathToWall component.

#### PathToWall.cs : MonoBehaviour

Class who manage a wall mesh creation generating random near positions.

| Parameter | Description |
| --- | --- |
| Path : Vector3[] | Represents path position on which create the wall mesh |
| Use Custom Height : bool | If not checked, wall mesh generation will use default height of 4 units |
| Custom Height : float | (visible only if useCustomHeight checked) Custom height that wall mesh generation will use |

| Method | Description |
| --- | --- |
| GenerateNewPath() : void | Generate a path of random points (min: 5, max: 15) every point is in at a max radius of 2 each other |
| GenerateWallMesh() : void | Generate a wall mesh with set height on path points (if generated) |
| GenerateWallMeshDoubleFace() : void | Generate a wall mesh with set height who can be seen on both faces on path points (if generated) |

All these methods have their own context menu

#### PathToWallEditor.cs : Editor

Custom inspector for PathToWall class. It adds button to PathToWall inspector for better usability
GenerateWallMesh and GenerateWallMeshDoubleFace buttons can be pressed only if path exists

## Assignment 02 - Cube Creator

Custom tool who manages cuboid creation inside the scene view using two handles. Gizmos edges will be shown for preview.
Opening CubeCreatorTool you'll have two handles:
- end point handle allows to set scale moving it
- start point handle allows to set scale as end point handle and rotation

In order to work, you need a GameObject (at top level hierarchy) in scene with CubeCreator component
Multiple CubeCreator selected work together creating their own cube shown in scene view.

| Shortcut | Description |
| --- | --- |
| P | (Available if Scene View selected) If no CubeCreator GameObject selected it will select the first it can find and will open CubeCreatorTool.<br>If (one or more) CubeCreator selected, it will directly open CubeCreatorTool |
| Y | Create a cube with position, scale and rotation as setted in scene |

## Assignments 03 - Node framework

Framework to manage a board with many node inside purely based on UI Toolkit (no GraphView or GraphTool Foundation).
Every board can be exported in JSON file

#### Board Editor

Window with the edited board title, an export button and the grid with nodes shown.

**Node link**
In order to link two nodes you have to click on a node port first and then on the node you want to link to.

| Shortcut | Description |
| --- | --- |
| Left Click (Grid) | Unselect node or link if any ||
| Left Click (Node or Link) | Select hovered node or link. If you started a link action and click on Grid, same node where link action started or another link, link action will be interrupted. | 
| Left Hold (Grid) | Move the background grid **No board movement implemented yet** |
| Left Hold (Node) | Move selected node around the board |
| Shift + Left Hold (Node) | Move selected node and every directed attached node around the board |
| Right Click (Grid) | Open contextual menu with available nodes to create. Click on a menu voice will create a node of that type at mouse position |
| Right Click (Node or Link) | Open contextual menu with "Delete" option available. Click on it will delete the hovered node or link |
| S | Create a SingleNode on mouse position |
| B | Create a BinaryNode on mouse position |
| Del (Node or Link) | Delete selected node (and every attached link) or link |

**Known issues**

- Undo-redo: Undo a node delete action will lose any link it had
- No grid scroll added also if grid drag implemented (nodes will not scroll on grid)

#### Board.cs : ScriptableObject

Board class containing a root node and a list of nodes (root contained in list as well). It can create, delete and set parent-child node relationship with **undo-redo** related commands.
Board asset will contains every related node inside.

#### Node.cs : ScriptableObject

Node class is an abstract class containing description and methods to handles children in a common way.
No children definition inside in order to let every child class declare children in its own way.

| Method | Description |
| --- | --- |
| GetChildren() : Node[] | Returns children in a Node array when they are null or only one as well |
| AddChild(int) : void | Add a child at index. If a child already exists at that index it will be overwritten |
| RemoveChild(Node) | Remove node from node children |

#### Current Available Nodes

| Node | Description |
| --- | --- |
| SingleNode | Node with only a child |
| BinaryNode | Node with maximum two children |

#### You can add new custom nodes in easy way

Code:
```
class MyNewNode : Node
{
    public Node[] GetChildren() { return Node[0]; }
    public void AddChild(int childIndex) { }
    public void RemoveChild(Node childToRemove) { }
}
```

You can add your own style in UIBuilder tool.

**N.B.**
Actually you have to manually add the command "AddToClassList"s class in NodeView constructor.
I'll manage auto-add class as soon as possible

## Assignments 04 - ScreenShot taker and editor

### ScreenShot taker

You can take screenshot of what camera is rendering. You can zoom in and out up 10x and change camera grid size.

Screenshots saved in "Image/Assignment/" path.

### ScreenShot editor

This tool let you make simple editing to your screenshot in "Image/Assignment/".

Actually you can:

- Flip on X and Y axis (only 1 axe or both)
- Invert screenshot colors
- Keep or not original screenshot
