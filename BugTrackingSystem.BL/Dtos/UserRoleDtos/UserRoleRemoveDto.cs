﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.BL;

public class UserRoleRemoveDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
