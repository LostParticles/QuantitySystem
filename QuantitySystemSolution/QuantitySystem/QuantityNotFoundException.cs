﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QuantitySystem
{
    [Serializable()]
    public class QuantityNotFoundException : Exception
    {
      public QuantityNotFoundException()
      {
         // Add any type-specific logic, and supply the default message.
      }

      public QuantityNotFoundException(string message): base(message) 
      {
         // Add any type-specific logic.
      }
      public QuantityNotFoundException(string message, Exception innerException): 
         base (message, innerException)
      {
         // Add any type-specific logic for inner exceptions.
      }
      protected QuantityNotFoundException(SerializationInfo info, 
         StreamingContext context) : base(info, context)
      {
         // Implement type-specific serialization constructor logic.
      }    
    }
}