# SPICA [![Build status](https://ci.appveyor.com/api/projects/status/ar1fyeo109v587xf/branch/master?svg=true)](https://ci.appveyor.com/project/gdkchan/spica/branch/master)
Experimental H3D tool for serializing/deserializing BCH.

### Dependencies:
- .NET Framework 4.6
- OpenGL 3.3 at least
- OpenTK
- OpenTK.GLControl
  - Both can be found on NuGet.
  - _Note: The version of OpenTK.GLControl on NuGet is broken, so it's recommended to build it yourself from source and manually add a reference to the compiled library._
  - OpenTK git can be found [here](https://github.com/opentk/opentk).


### Builds
SPICA can be built on Linux/Mac using [Mono](https://www.mono-project.com/).
See `README.mono.md` for details.

**Windows build:**

To download the lastest automatic build for Windows, [Click Here](https://ci.appveyor.com/api/projects/gdkchan/spica/artifacts/spica_lastest.zip).


# Modder Notes _(pokemon & unity3d)_
_specifically pokemon fans, although possibly not limited to just that realm. DAE files are kind of a pain, but they are open source and easily manipulated. I needed a simple way to combine N animations into a single dae file. Spica already supported single animation exports, yet i hate blender, so prefer to create the game ready models and animations programatically._

### what you'll need
1. get a decrypted .3ds file _(possible called .cia)_
2. visual studio 2017 community
3. DotNet.3DS.Toolkit.v1.2.0
4. puyotools
5. pk3ds _(build 354 - others may work)_
6. spica
7. unity3d

### Step 1. extract from rom
- use DotNet.3DS.Toolkit.v1.2.0 to extract the already decrpyed cia/.3ds file  
  - _(make sure outputs are placed in a diff directory)_

### Step 2. unpack a GARC
- Open PK3DS and choose tools>Misc Tools> Un(pack)+BCLIM  
- Drag the garc from a/0/9/4 in the first field while holding ctrl.  
- When it's done, you'll have 0000.bin - 10422.bin  

### Step 3. decrypt all the .bins
- `git clone git@github.com:nickworonekin/puyotools.git`  
- build in visual studio 2017  
- run and decompress the entire directory output by the pk3ds tool  
  - `...\puyotools\PuyoTools\bin\Debug\PuyoTools.exe`  
- _you now have 10k files with all the pkmn data in it an uncompressed for spica_

### Step 4. how to use spica
- open spica
- select merge _(folder with plus on it)_
- choose the 9 files corresponding to the pkmn you want _(shift key)_
- [here is a model index][usum model index]
- export textures to `.../pokemon/tex`
  - most _(not all)_ textures need to be mirrored in order to work in unity*
- hit save _(floppy disk)_ with selected animation(s) and model to store as .dae

**this version of the codebase will save all the animations as a single animation on export**  

### Step 5. unity3d
- import textures first into unity, 
- then import .dae model
- check out the animation tab 


### bounty
[![poli.gif](https://s15.postimg.cc/697oxd7t7/poli.gif)](https://postimg.cc/image/mwz6zv2kn/)
[![poli.png](https://s15.postimg.cc/v2h8xw8sb/poli.png)](https://postimg.cc/image/b7v7brtkn/)



[usum model index]: https://gbatemp.net/threads/pokemon-sun-moon-pokemon-animations-textures-and-models.473906/
[alternative model index]: https://gbatemp.net/threads/sun-moon-pokemon-model-file-mapping-cheat-list-for-a-0-9-4-archives.478882/

## Future
- make this a cli operation
  - no need for the UI here _(at least for pkmn)_