﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    interface IJsonSerializable
    {
        public string ToJson();
    }
}
