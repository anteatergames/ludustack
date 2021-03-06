﻿using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class TeamMember : Entity
    {
        public bool Leader { get; set; }

        public InvitationStatus InvitationStatus { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string Work { get; set; }

        public string Quote { get; set; }

        public Guid TeamId { get; set; }

        public virtual Team Team { get; set; }
    }
}