﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuantitySystem.Quantities;
using QuantitySystem.Attributes;

namespace QuantitySystem.Units.Metric.SI
{
    [MetricUnit("J", typeof(Energy<>))]
    public sealed class Joule : MetricUnit
    {
    }
}
