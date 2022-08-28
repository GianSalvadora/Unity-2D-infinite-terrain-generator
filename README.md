# Unity-infinite-terrain-generator
Unity infinite terrain generator

Features
*Object pooling - faster generation
*Different noise included - using Fast noise lite
*Some sprites included - using Mini world sprites
*Biome generation - generates biomes, for now deserts and ice biomes exists
*Seeded - input numerical values to display different generation, similar seeds shows similar results
*Safe spawn - ensures the player spawns on land and not on water
*Render distance - different render distances 
*Different views - land(Normal), precipitation, water level, temperature view
*chunks will always spawn around the player

Down side
*World gen object should be stuck at 0, 0
*large seeds might break - untested
*no chunk saving, YET
*dont change the render distance while running

basic explaination

it uses 4 noise
- water level noise
- precipitation noise
- temperature noise
- idk if this counts as noise but it warps precipitation and temperature noise


MyChunkGen.cs - Start()

1.chunk pool is set depending on render distance
2.ensures the player is safe by checking the water level value, if not move the player upwards then check again
3.Loads the first few chunks around the player
4.sets the mainchunk(the chunk the player is in) as the center chunk

the main chunk will check if the player leaves it. If so the chunk will tell the chunk generator to set a new main chunk and load/unload chunks

1.if the player leaves main chunk, main chunk call OnMainChunkChange() on MyChunkGen.cs

MyChunkGen.cs - OnMainChunkChange()

1 find the new main chunk
2 unloads the chunks outside of the render distance, chunks inside the render distance remains
3 unloaded chunks gets their values changed then used again to load the new chunks

Example 1 - 10 render distance, 150 camera size

![image](https://user-images.githubusercontent.com/79357222/187061851-18cc6022-6086-45dd-a613-f858b3740ea9.png)

Example 2 - 10 render distance, 150 camera size

![image](https://user-images.githubusercontent.com/79357222/187061877-4f57f3d5-eb99-48b9-985d-411f4da376f1.png)

Example 3- 1 render distance, 10 camera size (INTENDED USE)

![image](https://user-images.githubusercontent.com/79357222/187061938-fae57e02-81fc-4742-ba92-77c42c836a89.png)
