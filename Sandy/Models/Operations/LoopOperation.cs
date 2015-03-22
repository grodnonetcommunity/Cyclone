﻿using System.Collections.Generic;

namespace AV.Cyclone.Sandy.Models.Operations
{
    public class LoopOperation : Operation
    {
        public LoopOperation()
        {
            this.Operations = new Dictionary<int, IList<Operation>>();
        }

        /// <summary>
        /// Key is a number of iteration
        /// Value is a list of operations for particular operation
        /// </summary>
        public Dictionary<int, IList<Operation>> Operations { get; set; }
    }
}