using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public abstract class RequestFeatures
    {
        const int maxPagSize = 50;
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;
        public int PageSize 
        { 
            get
            {
                return _pageSize;
            } 
            set
            {
                _pageSize = (value > maxPagSize) ? maxPagSize : value;
            }
        }
    }
    public class EmployeeParameters : RequestFeatures
    { 
    }
}
