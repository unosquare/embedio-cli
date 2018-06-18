[![Build status](https://ci.appveyor.com/api/projects/status/tp2ce47b9mpbo20f/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/embedio-cli/branch/master)
[![Build Status](https://travis-ci.org/unosquare/embedio-cli.svg?branch=master)](https://travis-ci.org/unosquare/embedio-cli)

# EmbedIO CLI

A `dotnet` global tool that enables start any web folder or EmbedIO-enabled DLL from command line.

## Installation

We are using the brand new implementation of the global tool in .NET Core Apps 2.1+. Now you can easily download the package by running the next command

```
dotnet tool install -g embedio-cli
```

## Custom installation
If you download the project and want to test installing your own version of the project you need to pack and then install the nuget

```
// In the root of your project run
$ dotnet pack

// Run the following command where you nupkg was created
$ dotnet tool install -g embedio-cli --add-source ./
```

## Usage

### Serve folder or web folder (static content only)

Run the command to serve a static folder and watch the files in the current directory. The command will serve a web page if it finds an `index.html` file or a `wwwroot` folder, otherwise it will serve the files to navigate them in the web browser.

```
$ embedio-cli
```

By default the files are been watching and if any change happens on them it would reload the page, you can disabled this functionality by passing `--no-watch`

### Run web folder

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
