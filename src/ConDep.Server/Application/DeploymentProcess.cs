﻿using System.Diagnostics;
using ConDep.Server.Domain.Deployment;
using ConDep.Server.Domain.Deployment.Commands;
using ConDep.Server.Domain.Deployment.Events;
using ConDep.Server.Domain.Infrastructure;
using ConDep.Server.Domain.Queue;
using ConDep.Server.Domain.Queue.Commands;
using ConDep.Server.Domain.Queue.Events;

namespace ConDep.Server.Application
{
    public class DeploymentProcess :
        IHandleEvent<DeploymentQueued>,
        IHandleEvent<EnvironmentQueueReadyForDeployment>,
        IHandleEvent<DeploymentCreated>,
        IHandleEvent<DeploymentFinished>,
        IHandleEvent<DeploymentDequeued>
    {
        private readonly ICommandBus _commandBus;

        public DeploymentProcess(ICommandBus commandBus, DeploymentScheduler scheduler)
        {
            _commandBus = commandBus;
            
            //Todo: Configurable expiration on deployment queue
            //scheduler.ScheduleQueueCleanup(60 * 5);
            scheduler.ScheduleQueueProcessing(30);
            Trace.WriteLine("DeploymentProcess object created!");
        }

        public void Handle(DeploymentQueued @event)
        {
            Trace.WriteLine("DeploymentProcess: " + GetHashCode());
            var cmd = new ProcessEnvironmentQueue(@event.Environment);
            _commandBus.Send(cmd);

        }

        public void Handle(EnvironmentQueueReadyForDeployment @event)
        {
            Trace.WriteLine("DeploymentProcess: " + GetHashCode());
            var createDeploymentCmd = new CreateDeployment(@event.SourceId, @event.Environment, @event.Module, @event.Artifact);
            _commandBus.Send(createDeploymentCmd);
        }

        public void Handle(DeploymentCreated @event)
        {
            Trace.WriteLine("DeploymentProcess: " + GetHashCode());
            var deploymentQueueItemInProgressCmd = new SetDeploymentQueueItemInProgress(@event.SourceId, @event.Environment);
            _commandBus.Send(deploymentQueueItemInProgressCmd);

            var deployCmd = new Deploy(@event.SourceId);
            _commandBus.Send(deployCmd);

            //var finishDeploymentCmd = new FinishDeployment(@event.SourceId);
            //_commandBus.Send(finishDeploymentCmd);
        }

        public void Handle(DeploymentFinished @event)
        {
            Trace.WriteLine("DeploymentProcess: " + GetHashCode());
            var dequeueDeployment = new DeQueueDeployment(@event.SourceId, @event.Environment);
            _commandBus.Send(dequeueDeployment);
        }

        public void Handle(DeploymentDequeued @event)
        {
            Trace.WriteLine("DeploymentProcess: " + GetHashCode());
            var cmd = new ProcessEnvironmentQueue(@event.Environment);
            _commandBus.Send(cmd);
        }
    }
}