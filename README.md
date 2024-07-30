# Taski
Pluggable application for automating tasks

This project contains two core module and one processor console.
For code modules, I've used .NET Standard 2.0 to be able to load plug-ins in older and newer version of .NET

At this moment, we cant use .NET Framework and the interface needs Logger and Configuration through dependency injection. However it is possible to use Class libraries implemented in .NET Framework as well as .NET Core.
