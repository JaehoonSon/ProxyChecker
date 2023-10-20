# ProxyChecker
A high-performance proxy checker built in C#. This proxy supports HTTP/s and SOCKS protocols. 

## Usage
Configure reading and writing path in `Directories.cs`
```
public static readonly string READING_LOCATION = "full path";
```

Build
```
dotnet build --configuration Release
```

Or Publish
```
dotnet publish --configuration Release
```
