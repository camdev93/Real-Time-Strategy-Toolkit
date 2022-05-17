using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;
    public RoomNode RootNode { get => rootNode; }
    public BinarySpacePartitioner(int levelWidth, int levelLength)
    {
        // Arguement 1 in the 'RoomNode' constructor will take in the root node which will be at a zeroed position
        // Arguement 2 will take in the maximum dimensions o the entire level 
        // Arguement 3 will be the parent node which will begin as null because it is the root node is the parent.
        // Arguement 4 will initialise the root index which will start at zero
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(levelWidth, levelLength), null, 0);
    }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        // If there is a room's size that needs to be checked it can be passed through the to the Queue and check it in the below while loop until the method reaches its maximum iterations

        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iterations = 0;

        // While the current amount of iterations are less than the maximum iterations set in the parameter the method for splitting the given space will keep being called

        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();
            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }

        // Method will return the list that contains every node to be generated.
        return listToReturn;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomLengthMin, int roomWidthMin, Queue<RoomNode> graph)
    {
        // Line class will be used as a partitioner to divide the space within the threshold
        Line line = GetLineDividingSpace(
            currentNode.BottomLeftAreaCorner,
            currentNode.TopRightAreaCorner,
            roomWidthMin,
            roomLengthMin);

        // Further dividing already parted spaces into 2 smaller spaces
        RoomNode node1, node2;

        // Using line to split the node horizontally
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner,
                new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner,
                new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }

        // Nodes that have just been generated will be added to an appropriate graph
        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);
    }

    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        // 'listToReturn' will be returned in the above method
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    // This method will set an orientation depending on the size of the total area set.
    // this will return a new line that will store values it needs to split a space.
    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {

        Orientation orientation;

        // This analyse the status of the total area to check which direction the space is able to be divided.
        // if there is enough width space the method will create a line in the vertical direction and the same respective calculation for available length space in the horizontal direction
        // if there is space for both orientations then a random selection will be made
        bool lengthStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= 2 * roomLengthMin;
        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= 2 * roomWidthMin;

        if (lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        return new Line(orientation, GetCoordinatesFororientation(
            orientation,
            bottomLeftAreaCorner,
            topRightAreaCorner,
            roomWidthMin,
            roomLengthMin));
    }

    // This method read coordinates for our line by checking the size of our room and randomly choosing a value between the minimum length and width of a given room
    private Vector2Int GetCoordinatesFororientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            // Create and set new coordinates
            // In this condition a point location is being generated between the minimum and maximum values set in the Random.Range function.
            // Minumum is the distance from the bottom left area corner added to the rooms minimum length which the user will specify in the editor
            // Maximum is the top right area corner minus the minimum room length to make sure the spaces can not be divided into a size smaller than the length size
            coordinates = new Vector2Int(
                0,
                Random.Range(
                (bottomLeftAreaCorner.y + roomLengthMin),
                (topRightAreaCorner.y - roomLengthMin)));
        }
        else
        {
            // Create and set new coordinates
            // In this condition a point location is being generated between the minimum and maximum values set in the Random.Range function.
            // Minumum is the distance from the bottom left area corner added to the rooms minimum width which the user will specify in the editor
            // Maximum is the top right area corner minus the minimum room width to make sure the spaces can not be divided into a size smaller than the width size
            coordinates = new Vector2Int(
                Random.Range(
                (bottomLeftAreaCorner.x + roomWidthMin),
                (topRightAreaCorner.x - roomWidthMin))
                , 0);
        }

        return coordinates;
    }
}