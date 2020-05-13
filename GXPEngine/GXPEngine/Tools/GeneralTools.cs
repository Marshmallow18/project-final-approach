﻿namespace GXPEngine
{
    public class GeneralTools
    {
        /// <summary>
        /// Cycle through indexes from 0 to Array Length
        /// </summary>
        /// <param name="i"></param>
        /// <param name="arrayLength"></param>
        /// <returns></returns>
        public static int GetCircularArrayIndex(int i, int arrayLength)
        {
            int mod = i % arrayLength;
            return (mod >= 0) ? mod : mod + arrayLength;
        }
    }
}