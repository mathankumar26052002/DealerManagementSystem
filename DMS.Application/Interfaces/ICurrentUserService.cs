using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int GetDealerId();

        int GetUserId();
    }
}
