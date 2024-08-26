using Boz.Services.Contracts.Utils;
using Maxi.Services.SouthSide.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Maxi.Services.SouthSide.SouthsideFile
{
    public abstract class SED
    {
        private ILogger _logger;
        private IServiceUtils _serviceUtils;
        public SED(ILogger logger, IServiceUtils serviceUtils) { 
            _logger = logger; 
            _serviceUtils = serviceUtils;
        }

        public abstract string TypeIdentifier { get; }

        public virtual byte[] GetBinary()
        {
            return GetBinary(this);
        }

        public byte[] GetBinary(object values)
        {
            return GetX96(GetFields(values.GetType(), values), 2);
        }

        private List<SEDField> GetFields(Type type, object values)
        {
            try
            {
                List<SEDField> list = new List<SEDField>();
                foreach (PropertyInfo item in from prop in type.GetProperties()
                                              where Attribute.IsDefined(prop, typeof(SEDFieldAttributte))
                                              select prop)
                {
                    object obj = item.GetCustomAttributes(typeof(SEDFieldAttributte), inherit: false).FirstOrDefault();
                    if (obj != null)
                    {
                        SEDFieldAttributte sEDFieldAttributte = (SEDFieldAttributte)obj;
                        list.Add(new SEDField
                        {
                            Lenght = sEDFieldAttributte.Lenght,
                            Order = sEDFieldAttributte.Order,
                            Type = sEDFieldAttributte.Type,
                            Name = item.Name,
                            IsConstant = sEDFieldAttributte.IsConstant,
                            ValueString = ((item.GetValue(values, null) == null || sEDFieldAttributte.Type == SEDFieldType.Bin) ? null : item.GetValue(values, null).ToString()),
                            ValueBytes = ((item.GetValue(values, null) == null || sEDFieldAttributte.Type != SEDFieldType.Bin) ? null : ((byte[])item.GetValue(values, null)))
                        });
                    }
                }
                return list.OrderBy((SEDField i) => i.Order).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ErrorUtils.FormatError($"SED.GetFields({TypeIdentifier})", ex, "SOUTSIDESEND"));
                throw;
            }
        }

        private byte[] GetX96(List<SEDField> fields, int binaryStringChange)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                byte[][] array = new byte[binaryStringChange][];
                foreach (SEDField field in fields)
                {
                    if (field.Type == SEDFieldType.Bin)
                    {
                        if (stringBuilder.ToString() != string.Empty)
                        {
                            array[0] = _serviceUtils.GetEncoding().GetBytes(stringBuilder.ToString());
                            stringBuilder.Clear();
                            array[1] = field.ValueBytes;
                        }
                    }
                    else
                    {
                        stringBuilder.Append(FormatFiel(field));
                    }
                }
                if (stringBuilder.ToString() != string.Empty)
                {
                    array[0] = _serviceUtils.GetEncoding().GetBytes(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                if (array[1] != null)
                {
                    return _serviceUtils.AppendBytes(_serviceUtils.RecordBegining(array[0].Length + array[1].Length), array[0], array[1]);
                }
                return _serviceUtils.AppendBytes(_serviceUtils.RecordBegining(array[0].Length), array[0]);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ErrorUtils.FormatError($"SED.GetX96({TypeIdentifier})", ex, "SOUTSIDESEND"));
                throw;
            }
        }

        private string FormatFiel(SEDField field)
        {
            if (field.ValueString != null)
            {
                if (field.IsConstant)
                    return field.ValueString;
                if (field.ValueString.Length > field.Lenght)
                    field.ValueString = field.ValueString.Substring(0, field.Lenght);
                switch (field.Type)
                {
                    case SEDFieldType.A:
                    case SEDFieldType.AN:
                    case SEDFieldType.ANS:
                    case SEDFieldType.NB:
                    case SEDFieldType.B:
                        return field.ValueString.PadRight(field.Lenght, ' ');
                    case SEDFieldType.N:
                        return field.ValueString.PadLeft(field.Lenght, '0');
                    case SEDFieldType.NBS:
                    case SEDFieldType.NBSM:
                        return field.ValueString.PadLeft(field.Lenght, ' ');
                }
            }
            return string.Empty;
        }
    }
}
