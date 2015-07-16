cd WebSocketRails
nuget pack -Prop Configuration=Release -Build
nuget push WebSocketRailsNet.1.3.0.0.nupkg