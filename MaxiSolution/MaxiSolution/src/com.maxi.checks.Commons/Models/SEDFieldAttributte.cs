using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.SouthsideFile
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SEDFieldAttributte:Attribute
    {
        public SEDFieldType Type { get; set; }
        public Int32 Lenght { get; set; }
        public Int32 Order { get; set; }
        public Boolean IsConstant { get; set; }


        public SEDFieldAttributte(SEDFieldType type, Int32 lenght, Int32 order, Boolean isConstant)
        {
            Type = type;
            Lenght = lenght;
            Order = order;
            IsConstant = isConstant;
        }

        public SEDFieldAttributte(SEDFieldType type, Int32 lenght, Int32 order):this(type, lenght, order, false)
        {

        }
    }

    public enum SEDFieldType
    {
        A,
        AN,
        ANS,
        N,
        NB,
        NBS,
        NBSM,
        B,
        Bin,
    }

}
