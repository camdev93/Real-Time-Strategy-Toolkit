using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
[RequireComponent(typeof(NavMeshSurface))]
public class LevelCreator : MonoBehaviour
{
    // Set player position
    public bool spawnPlayer;
    [HideInInspector]
    public GameObject NavmeshPlayerObject;
    [HideInInspector]
    public bool hasPlayerSpawned;

    // Updatable NavMesh
    [HideInInspector]
    public GameObject emptyParentFloor;
    [HideInInspector]
    public NavMeshSurface navMesh;

    // Level dimension parameters
    public int levelWidth, levelLength;
    public int roomWidthMin, roomLengthMin;
    public int maximumIterations;
    [HideInInspector]
    public int iterations;
    public int corridorWidth;

    public Material floorMaterial;

    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject verticalWallPrefab, horizontalWallPrefab;
    public GameObject[] coverPositionPrefab;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

    // START and END markers
    [HideInInspector]
    public bool objectOneInitialised, objectTwoInitialised;
    public GameObject pointOfInterest1Prefab, pointOfInterest2Prefab;

    // Firing cover parameters
    [Range(0.0f, 1.0f)]
    public float chanceOfCover;

    [HideInInspector]
    public MeshCollider meshCollider;
    [HideInInspector]
    public Mesh corridorMesh;
    [HideInInspector]
    public BoxCollider boxCollider;

    void Start()
    {
        // method can be placed here to generate new levels on each scene play
        //CreateLevel();

        navMesh.BuildNavMesh();

        if (!spawnPlayer)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().enabled = false;
        }
        else if (spawnPlayer)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().enabled = true;
        }

    }

    int roomIterations;

    // This method will generate a new instance of a level based on parameter values
    public void CreateLevel()
    {
        DestroyAllChildren();

        navMesh = GetComponent<NavMeshSurface>();
        LevelGenerator generator = new LevelGenerator(levelWidth, levelLength);

        var listOfRooms = generator.CalculateLevel(maximumIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        CreateWalls(wallParent);

        // upon each iteration this will render a new navmesh and add a mesh collider to each surface
        navMesh.BuildNavMesh();

        roomIterations = listOfRooms.Count;
    }

    // This method will randomise parameters values and generate a new random level
    public void RandomiseParameters()
    {
        DestroyAllChildren();

        int _levelWidth = Random.Range((levelWidth / 2), levelWidth);
        int _levelLength = Random.Range((levelLength / 2), levelLength);
        int _maximumIterations = Random.Range((maximumIterations / 2), maximumIterations);
        int _roomWidthMin = Random.Range((roomWidthMin / 2), roomWidthMin);
        int _roomLengthMin = Random.Range((roomLengthMin / 2), roomLengthMin);
        float _roomBottomCornerModifier = Random.Range((roomBottomCornerModifier / 2f), roomBottomCornerModifier);
        float _roomTopCornerModifier = Random.Range((roomTopCornerModifier / 2f), roomTopCornerModifier);
        int _roomOffset = Random.Range((roomOffset / 2), roomOffset);
        int _corridorWidth = Random.Range(5, corridorWidth);

        navMesh = GetComponent<NavMeshSurface>();
        LevelGenerator generator = new LevelGenerator(_levelWidth, _levelLength);

        var listOfRooms = generator.CalculateLevel(_maximumIterations,
            _roomWidthMin,
            _roomLengthMin,
            _roomBottomCornerModifier,
            _roomTopCornerModifier,
            _roomOffset,
            _corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        CreateWalls(wallParent);

        // upon each iteration this will render a new navmesh and add a mesh collider to each surface
        navMesh.BuildNavMesh();

        roomIterations = listOfRooms.Count;
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, horizontalWallPrefab);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, verticalWallPrefab);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //______________________________________________________________________________________
        //MUST REMAIN IN THIS ORDER
        GameObject _levelFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        meshCollider = _levelFloor.AddComponent(typeof(MeshCollider)) as MeshCollider;
        _levelFloor.GetComponent<MeshFilter>().mesh = mesh;
        _levelFloor.GetComponent<MeshRenderer>().material = floorMaterial;

        GameObject _emptyParentFloor = Instantiate(emptyParentFloor, meshCollider.transform.position, meshCollider.transform.rotation);
       
        _emptyParentFloor.name = _emptyParentFloor.name;

        // Add BoxCollider to mesh floor
        boxCollider = _levelFloor.AddComponent(typeof(BoxCollider)) as BoxCollider;
        meshCollider = _emptyParentFloor.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshCollider = _levelFloor.GetComponent<MeshCollider>();
        meshCollider.enabled = false;
        //_________________________________________________________________________________________

        meshCollider = _emptyParentFloor.GetComponent<MeshCollider>();
        _levelFloor.transform.parent = _emptyParentFloor.transform;
        _emptyParentFloor.transform.parent = transform;
        meshCollider.enabled = false;

        //_______________________________________________________________________________________

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);

            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);

            // This statement is to ensure that no cover positions spawn inside a corridor
            if (mesh.bounds.size.z <= corridorWidth || mesh.bounds.size.x <= corridorWidth)
            {

            }
            else
            {
                // Only need a single iteration for spawning cover positions
                SpawnCoverPosition(meshCollider, mesh);
            }
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);

            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);

            // This statement is to ensure that no cover positions spawn inside a corridor
            if (mesh.bounds.size.z <= corridorWidth || mesh.bounds.size.x <= corridorWidth)
            {

            }
            else
            {
                // Only need a single iteration for spawning cover positions
                SpawnCoverPosition(meshCollider, mesh);
            }
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);

            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);

            // This statement is to ensure that no cover positions spawn inside a corridor
            if (mesh.bounds.size.z <= corridorWidth || mesh.bounds.size.x <= corridorWidth)
            {

            }
            else
            {
                // Only need a single iteration for spawning cover positions
                SpawnCoverPosition(meshCollider, mesh);
            }
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);

            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);

            // This statement is to ensure that no cover positions spawn inside a corridor
            if (mesh.bounds.size.z <= corridorWidth || mesh.bounds.size.x <= corridorWidth)
            {

            }
            else
            {
                // Only need a single iteration for spawning cover positions
                SpawnCoverPosition(meshCollider, mesh);
            }
        }
   
        // This section of the method will decide where (on the floor mesh) to place the start and end objective markers.

        iterations++;

        if (iterations == 1)
        {
            PointsOfInterestPositions(mesh, pointOfInterest1Prefab);
        }
        if (iterations >= roomIterations / 2)
        {
            PointsOfInterestPositions(mesh, pointOfInterest2Prefab);
        }

        if (hasPlayerSpawned == false && spawnPlayer == true)
        {
            SpawnPlayer(mesh,NavmeshPlayerObject);
        }
    }

    private void SpawnPlayer(Mesh mesh, GameObject player)
    {
        float minX = mesh.bounds.min.x;
        float maxX = mesh.bounds.max.x;
        float minZ = mesh.bounds.min.z;
        float maxZ = mesh.bounds.max.z;

        Vector3 randomSpawnPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        Vector3 feetOnFloor = new Vector3(0,(player.GetComponent<NavMeshAgent>().height/2f),0);

        Instantiate(player, randomSpawnPosition + feetOnFloor, Quaternion.identity, gameObject.transform);
        player.tag = "Player";

        hasPlayerSpawned = true;
        
    }

    public void SpawnCoverPosition(MeshCollider col, Mesh _mesh)
    {
        if (iterations<=((maximumIterations/2)+4))
        {
            int index = Random.Range(0, coverPositionPrefab.Length);
            float offset = coverPositionPrefab[index].GetComponent<MeshFilter>().sharedMesh.bounds.size.y / 2;
            float dice = Random.Range(0.0f, 1.0f);

            float minX = _mesh.bounds.min.x;
            float maxX = _mesh.bounds.max.x;
            float minZ = _mesh.bounds.min.z;
            float maxZ = _mesh.bounds.max.z;

            Vector3 randomSpawnPosition = new Vector3(Random.Range(minX, maxX), offset, Random.Range(minZ, maxZ));

            if (dice < chanceOfCover)
            {
                GameObject cover = Instantiate(coverPositionPrefab[index], randomSpawnPosition, Quaternion.identity, col.gameObject.transform);
                cover.transform.Rotate(0f, Random.Range(0f, 180f), 0f);
                cover.name = "FireCoverPosition";
            }
        }
    }

    private void PointsOfInterestPositions(Mesh mesh, GameObject position)
    {
        float minX = mesh.bounds.min.x;
        float maxX = mesh.bounds.max.x;
        float minZ = mesh.bounds.min.z;
        float maxZ = mesh.bounds.max.z;

        Vector3 randomSpawnPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        if (!objectOneInitialised && !objectTwoInitialised)
        {
            if (!position.TryGetComponent(out ObjectOne object_1))
            {
                object_1 = position.AddComponent(typeof(ObjectOne)) as ObjectOne;
            }

            Instantiate(position, randomSpawnPosition, Quaternion.identity, gameObject.transform);

            objectOneInitialised = true;
        }
        else if (!objectTwoInitialised && objectOneInitialised)
        {
            if (!position.TryGetComponent(out ObjectTwo object_2))
            {
                object_2 = position.AddComponent(typeof(ObjectTwo)) as ObjectTwo;
            }

            Instantiate(position, randomSpawnPosition, Quaternion.identity, gameObject.transform);

            objectTwoInitialised = true;
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    public void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}