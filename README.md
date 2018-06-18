[![Build status](https://ci.appveyor.com/api/projects/status/tp2ce47b9mpbo20f/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/embedio-cli/branch/master)
[![Build Status](https://travis-ci.org/unosquare/embedio-cli.svg?branch=master)](https://travis-ci.org/unosquare/embedio-cli)

## EmbedIO CLI

Start any web folder or EmbedIO-enabled DLL from command line.

### Run web folder (static content only)

```
$ Unosquare.Labs.EmbedIO.Command -p c:\wwwroot
```

### Run web folder with WebAPI or WebSocket Assembly

```
$ Unosquare.Labs.EmbedIO.Command -p c:\wwwroot --api mywebapi.dll
```