using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The LevelGenerator class will be a central location to call supporting methods that will be used to create rooms, corridors and space partitioning.
public class LevelGenerator
{
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    private int levelWidth;
    private int levelLength;

    public LevelGenerator(int levelWidth, int levelLength)
    {
        this.levelWidth = levelWidth;
        this.levelLength = levelLength;
    }

    // The CalculateLevel method will call the supporting generators
    // This method will return a list of nodes to the 'LevelCreator' in order to generate the mesh
    public List<Node> CalculateLevel(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset, int corridorWidth)
    {
        // BSP constructor will take in width and length to constrain the area of exploitation
        // BSP will split appropriatly divide the space and designated sections for rooms, these calculations are exclusively for coordinate calculations
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(levelWidth, levelLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);

        // children objects that represent rooms must be extracted here but will only nodes without their own child obejcts
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset);

        CorridorsGenerator corridorGenerator = new CorridorsGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allNodesCollection, corridorWidth);

        // Concat is used here to also support the visualisation of the corridors
        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}