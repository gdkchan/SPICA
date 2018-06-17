# SPICA [![Build status](https://ci.appveyor.com/api/projects/status/ar1fyeo109v587xf/branch/master?svg=true)](https://ci.appveyor.com/project/gdkchan/spica/branch/master)
Experimental H3D tool for serializing/deserializing BCH.

Dependencies:
- OpenTK
- OpenTK.GLControl

Both can be found on NuGet.

Note: The version of OpenTK.GLControl on NuGet is broken, so it's recommended to build it yourself from source and manually add a reference to the compiled library.
OpenTK git can be found [here](https://github.com/opentk/opentk).

You will need .NET Framework 4.6 and a GPU capable of OpenGL 3.3 at least.

SPICA can be built on Linux/Mac using [Mono](https://www.mono-project.com/).
See `README.mono.md` for details.

**Windows build:**

To download the lastest automatic build for Windows, [Click Here](https://ci.appveyor.com/api/projects/gdkchan/spica/artifacts/spica_lastest.zip).
