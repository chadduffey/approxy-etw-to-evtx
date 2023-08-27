# AppProxy-Logs-ETW-to-EVTX-Service

Windows Service Project (needs to be installed as a windows service to run). 

## Installation

Everything needed to install the service is in the binaries (a service installer is packaged - it tells the service to be automatic, to run as system etc).    
You still must register the service with `installutil MyNewService.exe` on the server to host it, but no parameters are required. 

## Dependencies

You'll need the MSFT ETW package: https://www.nuget.org/packages/Microsoft.Diagnostics.Tracing.TraceEvent/ 

## ETW

![ETW](/png/etw-service.png)

