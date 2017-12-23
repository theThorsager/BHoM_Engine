﻿using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SnRatio SnRatio(double value, int receiverID, int speakerID, Frequency frequency)
        {
            return new SnRatio()
            {
                Value = value,
                ReceiverID = receiverID,
                SpeakerID = speakerID,
                Frequency = frequency
            };
        }

        /***************************************************/
    }
}
