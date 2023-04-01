# 8080-emulator-net

Simple intel 8080 emulator, written in C# with .NET Core. It's a learning project, just for fun.

OpCode reference table can be found [here](http://www.emulator101.com/reference/8080-by-opcode.html).

At the moment I have implemented only the bare minimun OPs to make Space Invaders "run".

**No roms are included in the repo.**

Rendering is handled with [Monogame](http://www.monogame.net/).

![Space Invaders](https://raw.githubusercontent.com/mizrael/8080-emulator-net/master/screenshots/space_invaders1.png)

## Building and running
Simply run `dotnet build` from the root folder. Then run:
1. `cd emu8080.Game`
1. `dotnet run`

### OSX
If you're trying to build on OSX, you might need to install some dependencies with `brew install mono-libgdiplus`

## TODO
- proper interrupt handling
- audio
- input