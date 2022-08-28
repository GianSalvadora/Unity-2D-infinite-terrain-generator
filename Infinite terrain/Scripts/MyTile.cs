using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour
{
    SpriteRenderer sr;
    public static FastNoiseLite waterLevelNoise;
    public static FastNoiseLite precipitationNoise;
    public static FastNoiseLite temperatureNoise;
    public static FastNoiseLite warp;
    public static MyWorldGen instance;
    public static float waterLevel;

    public static float sandSize;

    public float precipitation;

    public float temperature;

    public int wetness;
    public int hotness;
    public MyWorldGen.BiomeType biomeType;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Load()
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;

        float waterLevelValue = Mathf.Abs(waterLevelNoise.GetNoise(xPos, yPos));
        warp.DomainWarp(ref xPos, ref yPos);
        temperature = Mathf.Abs(temperatureNoise.GetNoise(xPos, yPos));
        precipitation = Mathf.Abs(precipitationNoise.GetNoise(xPos, yPos));


        wetness = Mathf.RoundToInt(precipitation * (instance.BiomeTable.GetLength(0) - 1));
        hotness = Mathf.RoundToInt(temperature * (instance.BiomeTable.GetLength(1) - 1));
        biomeType = instance.GetBiomeType(wetness, hotness);


        if (instance.loadType == MyWorldGen.LoadType.LandValue)
        {

            if (waterLevelValue <= waterLevel)
            {
                sr.color = Color.blue;
            }
            else
            {
                if (waterLevelValue <= waterLevel + sandSize)
                {

                    sr.color = Color.yellow;

                }
                else
                {

                    switch (biomeType)
                    {
                        case (MyWorldGen.BiomeType.DryCold):
                            sr.color = Color.green;
                            break;
                        case (MyWorldGen.BiomeType.MoistCold):
                            sr.color = Color.cyan;
                            break;
                        case (MyWorldGen.BiomeType.WetCold):
                            sr.color = Color.cyan;
                            break;
                        case (MyWorldGen.BiomeType.LukeDry):
                            sr.color = Color.green;
                            break;
                        case (MyWorldGen.BiomeType.LukeMoist):
                            sr.color = Color.green;
                            break;
                        case (MyWorldGen.BiomeType.LukeWet):
                            sr.color = Color.green;
                            break;
                        case (MyWorldGen.BiomeType.DryHot):
                            sr.color = Color.yellow;
                            break;
                        case (MyWorldGen.BiomeType.MoistHot):
                            sr.color = Color.yellow;
                            break;
                        case (MyWorldGen.BiomeType.WetHot):
                            sr.color = Color.green;
                            break;


                    }

                }
            }
        }
        else if (instance.loadType == MyWorldGen.LoadType.waterLevelNoise)
        {
            sr.color = Color.Lerp(Color.black, Color.white, waterLevelValue);
        }
        else if (instance.loadType == MyWorldGen.LoadType.precipitationNoise)
        {
            sr.color = Color.Lerp(Color.Lerp(Color.red, Color.yellow, 0.5f), Color.blue, precipitation);
        }
        else
        {
            sr.color = Color.Lerp(Color.blue, Color.red, temperature);
        }
    }
}
