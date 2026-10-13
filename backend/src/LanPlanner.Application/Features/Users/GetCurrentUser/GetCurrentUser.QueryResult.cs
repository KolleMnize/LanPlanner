using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanPlanner.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQueryResult(UserDto User);