# AdStripper
FFMPEG wrapper in .NET Core to automagically strip advertisements from MP4 files

### Usage
From folder containing application:
`dotnet AdStripper.dll -c Release`

#### Disclaimer
This is ultra-basic, hard-coded arguments for ffprobe and ffmpeg.  This application could and should be extended quite a bit so the flags can be set via code before executing the external processes.  Likely the FFMpegWrapper and FFProbeWrapper would be modified to only be just that, wrappers, and processing services implemented around those respective calls like maybe `IVideoSplitter`, `IVideoJoiner`, `IMetaDataParser` etc.  For now though this serves my purposes.
