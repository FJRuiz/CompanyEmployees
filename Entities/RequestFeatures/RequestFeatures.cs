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
        public string OrderBy { get; set; }
    }
    public class EmployeeParameters : RequestFeatures
    {
        public EmployeeParameters()
        {
            OrderBy = "name";
        }
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; }

        public bool ValidAgeRange => MaxAge > MinAge;

        public string SearchTerm { get; set; }

    }

    public class CompanyParameters : RequestFeatures
    {
        public CompanyParameters()
        {
            OrderBy = "name";
        }

        public string SearchTerm { get; set; }

    }
}
