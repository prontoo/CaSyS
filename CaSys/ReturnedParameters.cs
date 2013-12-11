// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ReturnedParameters.cs" company="Nextsense">
//   © 2010 Nextsense
// </copyright>
// <summary>
//   Defines the ReturnedParameters type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace CaSys
{
    public class ReturnedParameters
    {
        public string ParameterName
        {
            get;
            set;
        }

        public string ParameterValue
        {
            get;
            set;
        }
    }

    public class ReturnParametersList : List<ReturnedParameters>
    {
    }
}
