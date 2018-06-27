# Building SPICA with Mono

SPICA can be built and run using the [Mono](https://www.mono-project.com/) toolchain. This allows SPICA to be used on GNU/Linux, MacOS X, and possibly other platforms.

This build has been tested using Mono 5.12.0 on Ubuntu Linux for amd64.

Note: SPICA requires [OpenTK](https://opentk.github.io/). A compatible pre-compiled version is provided in the `Libraries` subdirectory. If you prefer to use your own build, replace the appropriate files therein.

## Platform-specific setup

### Debian/Ubuntu Linux

Note: As of this writing, these distributions ship Mono 4.6, which **cannot** compile SPICA. If a newer version is not available, then you will need to install from the [Mono Project repository](https://www.mono-project.com/download/stable/#download-lin).  Also, the `libopentk1.1-cil` package provided by the distribution is too old to be of use.

Install the `mono-devel` package if you don't already have it. Then, see the **Build procedure** below.

### CentOS/Fedora Linux

Note: As of this writing, these distributions ship Mono 4.8, which **cannot** compile SPICA. If a newer version is not available, then you will need to install from the [Mono Project repository](https://www.mono-project.com/download/stable/#download-lin).

Install the `mono-devel` and `msbuild` packages if you don't already have them. Then, see the **Build procedure** below.

### MacOS X

TODO (contributions welcome)

## Build procedure

To compile SPICA, open a shell in the top-level directory of the SPICA source tree, and run
```
$ make
```
If compilation succeeds, you can start the program with
```
$ make run
```
Finally, you can clear out the build with
```
$ make clean
```
Additional targets and options are available in the `Makefile`.
