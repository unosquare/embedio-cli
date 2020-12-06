[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/embedio/)](https://github.com/igrigorik/ga-beacon)
[![Build status](https://ci.appveyor.com/api/projects/status/tp2ce47b9mpbo20f/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/embedio-cli/branch/master)
[![Build Status](https://travis-ci.org/unosquare/embedio-cli.svg?branch=master)](https://travis-ci.org/unosquare/embedio-cli)

![EmbedIO](https://raw.githubusercontent.com/unosquare/embedio/master/images/embedio.png)

# EmbedIO CLI

**IMPORTANT:** This project is archived, we are no longer supporting this .NET Core tool.

*:star: Please star this project if you find it useful!*

A `dotnet` global tool that enables start any web folder or [EmbedIO](https://github.com/unosquare/embedio) assembly (WebAPI or WebSocket) from command line.

## Installation

We are using the brand new implementation of the [Global Tool in .NET Core Apps 2.1+](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools). Now you can easily download the package by running the next command

```
dotnet tool install -g embedio-cli
```

### Custom installation
If you download the project and want to test installing your own version of the project you need to pack and then install the nuget

```
// In the root of your project run
$ dotnet pack

// Run the following command where you nupkg was created
$ dotnet tool install -g embedio-cli --add-source ./
```

## Usage

### Default serve

Run the command to serve a static folder and watch the files in the current directory. The command will serve a web page if it finds an `index.html` file or a `wwwroot` folder, otherwise it will serve the files to navigate them in the web browser.

```
$ embedio-cli
```

By default, all the files inside the folder are been watched and if any change happens on them it will reload the page, you can disable this functionality by passing `--no-watch`

### Specify folder

```
$ embedio-cli -p c:\wwwroot
```

### Run WebAPI or WebSocket Assembly

```
$ embedio-cli --api mywebapi.dll
```

### Run web folder with WebAPI or WebSocket Assembly

```
$ embedio-cli -p c:\wwwroot --api mywebapi.dll
```

## Arguments

| Short Argument | Long Argument | Description | Default | Required |
|:-----: | :-----------: | :----------: | :-----------:| :-----------:|
| p | path | The path where files will be serve | ./ | :x: |
| o | port | The port where the app will be listening | 9696 | :x: |
| a | api | The path to the dll | ./ | :x: |
|  | no-watch | Disables the watch mode | false | :x: |  
