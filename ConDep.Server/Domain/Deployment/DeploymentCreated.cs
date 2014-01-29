﻿using System;
using ConDep.Server.Infrastructure;

namespace ConDep.Server.DomainEvents
{
    public class DeploymentCreated : IEvent
    {
        public DeploymentCreated(Guid id, string environment, string module, string artifact)
        {
            SourceId = id;
            Environment = environment;
            Module = module;
            Artifact = artifact;
        }

        public Guid SourceId { get; private set; }
        public string Environment { get; private set; }
        public string Module { get; private set; }
        public string Artifact { get; private set; }
        public bool Dispatched { get; set; }
    }
}