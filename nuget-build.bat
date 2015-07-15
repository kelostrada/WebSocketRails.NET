cd WebSocketRails
nuget pack -Prop Configuration=Release -Build
nuget push WebSocketRailsNet.1.2.1.0.nupkg