﻿using NBB.Application.DataContracts;
using System;

namespace NBB.Contracts.Application.Commands
{
    public class Msg7 : Command
    {
        public Guid ClientId { get; }

        public Msg7(Guid clientId, CommandMetadata metadata = null)
            : base(metadata)
        {
            ClientId = clientId;
        }

    }
}