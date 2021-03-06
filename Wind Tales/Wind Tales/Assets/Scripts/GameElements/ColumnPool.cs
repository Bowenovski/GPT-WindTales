﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Boo.Lang;

public class ColumnPool : MonoBehaviour 
{
	public GameObject columnPrefab;									//The column game object.
	public float columnPoolSize =2 ;						    //How many columns to keep on standby.
	public float spawnRate = 3f;									//How quickly columns spawn.
	public float columnMin = -1f;									//Minimum y value of the column position.
	public float columnMax = 3.5f;									//Maximum y value of the column position.
    public float BlockingColumnSpawn = 3f;                          //How quickly Blocking columns spawn.
    public float BlowingMinimum = 0;                               //You set the minimum blowing requiremnets for this game. 
    private GameControl games;

    private List<GameObject> columns;									//Collection of pooled columns.
	private int currentColumn;									//Index of the current column in the collection.

	private Vector2 objectPoolPosition = new Vector2 (-15,-25);		//A holding position for our unused columns offscreen.
	private float spawnXPosition = 10f;
   
    private float timeSinceLastSpawned;
    private float timeSinceLastSpawnedBlockage;                      


    void Start()
    {
        columns = new List<GameObject>();
	    for (int i = 0; i < columnPoolSize; i++)
	    {
	        columns.Add((GameObject)Instantiate(columnPrefab, objectPoolPosition, Quaternion.identity));
	    }
        games = GetComponent<GameControl>();
		timeSinceLastSpawned = 0f;
        BlockingColumnSpawn = 0f;
    }


    private void ColumnsFix()
    {
        columns = new List<GameObject>();
        for (int i = 0; i < columnPoolSize; i++)
        {
            columns.Add((GameObject)Instantiate(columnPrefab, objectPoolPosition, Quaternion.identity));
        }
        currentColumn = 0;
    }

    void Update()
    {
        timeSinceLastSpawned += Time.deltaTime;
        timeSinceLastSpawnedBlockage += Time.deltaTime;
        if (Input.GetAxis("Player_SimulateBreathing") >= 0.3 || Input.GetKey("q"))
        {
            var blockage = columns[games.score % 7].GetComponent<Column>().Blockage;
            blockage.SetActive(false);
        }

        if (GameControl.instance.GameOver == false && timeSinceLastSpawned >= spawnRate)
        {
            timeSinceLastSpawned = 0f;
            float spawnYPosition = Random.Range(columnMin, columnMax);
            if (columns[currentColumn] == null)
            {
                ColumnsFix();
            }
            columns[currentColumn].transform.position = new Vector2(spawnXPosition, spawnYPosition);

            if (timeSinceLastSpawnedBlockage >= BlockingColumnSpawn)
            {
                timeSinceLastSpawnedBlockage = 0f;
                columns[currentColumn].GetComponent<Column>().Blockage.SetActive(true);
            }

            currentColumn++;
            if (currentColumn >= columnPoolSize)
            {
                currentColumn = 0;
            }
        }
    }

}