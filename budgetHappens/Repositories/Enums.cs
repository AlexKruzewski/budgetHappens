using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.Repositories
{
    public enum PeriodLength
    {
        Weekly = 0,
        Monthly = 1,
        Yearly = 2
    }

    public enum DataType
    {
        Decimal = 0,
        String = 1,
        Int = 2,
        Date = 3
    }
}
