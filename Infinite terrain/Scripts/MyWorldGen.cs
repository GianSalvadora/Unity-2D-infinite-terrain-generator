using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWorldGen : MonoBehaviour
{
    public GameObject baseTile;
    public Transform player;
    public static MyWorldGen instance;
    public float waterLevel;
    public float sandSize;

    public NoiseField waterLevelNoise;

    public NoiseField precipitationNoise;
    public NoiseField temperatureNoise;
    public NoiseField warp;

    public enum LoadType
    {
        waterLevelNoise,
        precipitationNoise,
        temperatureNoise,
        LandValue
    }

    public LoadType loadType = LoadType.LandValue;

    public enum BiomeType
    {
        DryCold,
        MoistCold,
        WetCold,

        LukeDry,
        LukeMoist,
        LukeWet,

        DryHot,
        MoistHot,
        WetHot,
    }

    public BiomeType[,] BiomeTable = new BiomeType[3, 3] {   
    //COLDEST        Normal          Hot                  
    { BiomeType.DryCold, BiomeType.LukeDry, BiomeType.DryHot},              //Dry
    {BiomeType.MoistCold, BiomeType.LukeMoist, BiomeType.MoistHot},              //Normal
    { BiomeType.WetCold, BiomeType.LukeWet, BiomeType.WetHot},             //Wet
};
    public int seed;


    void Awake()
    {
        instance = this;
        MyTile.instance = this;
        MyTile.waterLevelNoise = waterLevelNoise.GetNoise();
        MyTile.precipitationNoise = precipitationNoise.GetNoise();
        MyTile.warp = warp.GetNoise();
        MyTile.temperatureNoise = temperatureNoise.GetNoise();
        MyTile.waterLevel = waterLevel;
        MyTile.sandSize = sandSize;
    }

    public BiomeType GetBiomeType(int precipitation, int temperature)
    {
        return BiomeTable[precipitation, temperature];
    }
    public MyTile GetTile()
    {
        return Instantiate(baseTile).GetComponent<MyTile>();
    }
}

[System.Serializable]
public class NoiseField
{
    public FastNoiseLite.NoiseType noiseType;
    public FastNoiseLite.FractalType fractalType;

    public float mainFrequency;
    public int octaves;
    public float lacunarity, gain;


    public FastNoiseLite.CellularDistanceFunction cellularDistanceFunction;
    public FastNoiseLite.CellularReturnType cellularReturnType;
    public float cellularJitter;

    public FastNoiseLite.DomainWarpType domainWarp;

    public float domainWarpAmplitude;
    public FastNoiseLite noise;

    public FastNoiseLite GetNoise()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(this.noiseType);
        noise.SetFractalType(this.fractalType);
        noise.SetFrequency(this.mainFrequency);
        if (this.fractalType != FastNoiseLite.FractalType.None)
        {
            noise.SetFractalLacunarity(this.lacunarity);
            noise.SetFractalGain(this.gain);
            noise.SetFractalOctaves(this.octaves);
            noise.SetSeed(MyWorldGen.instance.seed);
        }

        if (this.noiseType == FastNoiseLite.NoiseType.Cellular)
        {
            noise.SetCellularDistanceFunction(this.cellularDistanceFunction);
            noise.SetCellularReturnType(this.cellularReturnType);
            noise.SetCellularJitter(this.cellularJitter);
        }

        noise.SetDomainWarpType(domainWarp);
        noise.SetDomainWarpAmp(domainWarpAmplitude);

        return noise;
    }

}