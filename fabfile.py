import configparser
import os
import shutil
from fabric2 import *
from time import sleep
from patchwork.files import exists
from datetime import datetime, date

config = configparser.ConfigParser()
config.read('config.ini')

targetIp = config['Machines']['Target']
sourceIp = config['Machines']['Source']
print(f'Target ip is: {targetIp}')
print(f'Source ip is: {sourceIp}')

targetConn = Connection(targetIp,user='mateusz')
sourceConn = Connection(sourceIp,user='mateusz')

grpcServerFilename = 'grpc_server'
restServerFilename = 'rest_server'
meterFilename = 'meter'
grpcTraceFile = f'{grpcServerFilename}.trace.zip'
restTraceFile = f'{restServerFilename}.trace.zip'
collectedTracesDir = 'collected_traces'

def runbg(self, cmd, sockname="dtach"):
    return self.run('dtach -n `mktemp -u /tmp/%s.XXXX` %s' % (sockname, cmd))
Connection.runbg = runbg

@task
def copyGrpcServer(ctx):
    targetConn.run('rm -rf /home/mateusz/grpc_server*')
    print('Copying grpc server..')
    targetConn.put(f'publish/{grpcServerFilename}.zip', '/home/mateusz/')
    targetConn.run(f'unzip {grpcServerFilename}.zip -d {grpcServerFilename}')


@task
def copyRestServer(ctx):
    targetConn.run('rm -rf /home/mateusz/rest_server*')
    print('Copying rest server..')
    targetConn.put(f'publish/{restServerFilename}.zip', '/home/mateusz/')
    targetConn.run(f'unzip {restServerFilename}.zip -d {restServerFilename}')

@task
def copyMeter(ctx):
    sourceConn.run('rm -rf /home/mateusz/meter*')
    print('Copying meter..')
    sourceConn.put(f'publish/{meterFilename}.zip', '/home/mateusz/')
    sourceConn.run(f'unzip {meterFilename}.zip -d {meterFilename}')

@task 
def copyAll(ctx):
    copyGrpcServer(ctx)
    copyRestServer(ctx)
    copyMeter(ctx)

def startTracing(traceFileName):
    targetConn.run(f'rm -f {traceFileName}.trace.zip')
    targetConn.runbg(f'sudo ./perfcollect collect {traceFileName};sleep 3')

noReuests = 200000
noConcurrentRequests = 50

@task
def grpcExperiment(ctx):
    startTracing(grpcServerFilename)
    targetLocalIp = targetConn.run('ip address | grep 10 | grep -oE "\\b([0-9]{1,3}\\.){3}[0-9]{1,3}\\b" | head -n 1').stdout.strip()

    with(targetConn.cd(grpcServerFilename)):
        with(targetConn.prefix('export COMPlus_PerfMapEnabled=1;export COMPlus_EnableEventLog=1')):
            targetConn.runbg('dotnet DummyGrpc.dll')

    with(sourceConn.cd(meterFilename)):
        sourceConn.run(f'dotnet Meter.dll {noReuests} {noConcurrentRequests} {targetLocalIp} grpc')        

@task
def restExperiment(ctx):
    startTracing(restServerFilename)
    targetLocalIp = targetConn.run('ip address | grep 10 | grep -oE "\\b([0-9]{1,3}\\.){3}[0-9]{1,3}\\b" | head -n 1').stdout.strip()

    with(targetConn.cd(restServerFilename)):
        with(targetConn.prefix('export COMPlus_PerfMapEnabled=1;export COMPlus_EnableEventLog=1')):
            targetConn.runbg('dotnet DummyRest.dll')

    with(sourceConn.cd(meterFilename)):
        sourceConn.run(f'dotnet Meter.dll {noReuests} {noConcurrentRequests} {targetLocalIp} rest')        

    collect(ctx )

@task
def removeTraces(ctx):
    targetConn.runbg('sudo rm -rf /root/lttng-traces')

@task
def collect(ctx):
    perfcollectPid = targetConn.run('ps -a | grep perf_ | awk \'{print $1}\'')
    if perfcollectPid.stdout:
        for pid in perfcollectPid.stdout.splitlines():
            print(f'Killing perf collect PID {pid}')
            targetConn.run(f'sudo kill -INT {pid}')

    dotnetPid = targetConn.run('ps -e | grep dotnet | awk \'{print $1}\'')
    if(dotnetPid.stdout):
        for pid in dotnetPid.stdout.splitlines():
            print(f'Killing dotnet process PID {pid}')
            targetConn.run(f'sudo kill {pid}')
    
    sleep(1)
    grpcCollectPath = f'{collectedTracesDir}\\grpc\\{noReuests}_{noConcurrentRequests}'
    restCollectPath = f'{collectedTracesDir}\\rest\\{noReuests}_{noConcurrentRequests}'

    if(not os.path.exists(grpcCollectPath)):
        os.makedirs(grpcCollectPath)
    if(not os.path.exists(restCollectPath)):
        os.makedirs(restCollectPath)
    
    timestamp = f'{datetime.now():%Y_%m_%d_%H_%M_%S}'
    if exists(targetConn, grpcTraceFile):
        targetConn.get(f'/home/mateusz/{grpcTraceFile}', f'{grpcCollectPath}\\{grpcServerFilename}_{timestamp}.trace.zip')
        sourceConn.get(f'/home/mateusz/{meterFilename}/grpc.csv', f'{grpcCollectPath}\\{grpcServerFilename}_{timestamp}.csv')
        targetConn.run(f'rm -f {meterFilename}/grpc.csv')
        targetConn.run(f'rm -f {grpcTraceFile}')
    if exists(targetConn, restTraceFile):
        targetConn.get(f'/home/mateusz/{restTraceFile}', f'{restCollectPath}\\{restServerFilename}_{timestamp}.trace.zip')
        sourceConn.get(f'/home/mateusz/{meterFilename}/rest.csv', f'{restCollectPath}\\{restServerFilename}_{timestamp}.csv')
        targetConn.run(f'rm -f {restServerFilename}.trace.zip')
        targetConn.run(f'rm -f {meterFilename}/rest.csv')
        targetConn.run(f'rm -f {restTraceFile}')