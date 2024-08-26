// Decompiled with JetBrains decompiler
// Type: Maxi.Services.SouthSide.SouthsideFile.Types.FileWf
// Assembly: Maxi.Services.SouthSide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE90B488-E2C0-40E8-800A-A49C289E35ED
// Assembly location: C:\Users\erojas\Desktop\DLL SOUTHSIDE PRODUCCION\Maxi.Services.SouthSide.dll


namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    public class FileWf
    {
        public FileWf.Header FileHeader { get; set; }

        public CashLetterWf CashLetter { get; set; }

        public FileWf.Control FileControl { get; set; }

        public class Header : SED
        {
            public Header(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils)
            {
            }

            public override string TypeIdentifier
            {
                get
                {
                    return string.Format("{0}", GetType());
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
            public string RecordType
            {
                get
                {
                    return "01";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 2, true)]
            public string StandarLevel
            {
                get
                {
                    return "03";
                }
            }

            [SEDFieldAttributte(SEDFieldType.A, 1, 3)]
            public string TestFileIndicator
            {
                get
                {
                    return ConstantFields.TestFileIndicator;
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 9, 4)]
            public string ImmediateDestinationRoutingNumber { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 9, 5)]
            public string ImmediateOriginRoutingNumber { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 6)]
            public string FileCreationDate { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 4, 7)]
            public string FileCreationTime { get; set; }

            [SEDFieldAttributte(SEDFieldType.A, 1, 8, true)]
            public string ResendIndicator
            {
                get
                {
                    return "N";
                }
            }

            [SEDFieldAttributte(SEDFieldType.A, 18, 9)]
            public string ImmediateDestinationName
            {
                get
                {
                    return ConstantFields.DestinationName;
                }
            }

            [SEDFieldAttributte(SEDFieldType.AN, 18, 10)]
            public string ImmediateOrigiName
            {
                get
                {
                    return ConstantFields.OrigiName;
                }
            }

            [SEDFieldAttributte(SEDFieldType.AN, 1, 11, true)]
            public string FileIDModifier
            {
                get
                {
                    return "1";
                }
            }

            [SEDFieldAttributte(SEDFieldType.A, 2, 12, true)]
            public string CountryCode
            {
                get
                {
                    return "US";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 4, 13, true)]
            public string UserField
            {
                get
                {
                    return "    ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 1, 14, true)]
            public string Reserved
            {
                get
                {
                    return " ";
                }
            }
        }

        public class Control : SED
        {
            public Control(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils)
            {
            }

            public override string TypeIdentifier
            {
                get
                {
                    return string.Format("{0}", GetType());
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
            public string RecordType
            {
                get
                {
                    return "99";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 6, 2)]
            public string CashLetterCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 3)]
            public string TotalRecordCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
            public string TotalItemCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 16, 5)]
            public string FileTotalAmount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 14, 6)]
            public string ImmediateOriginContactName
            {
                get
                {
                    return ConstantFields.OriginatorContactName;
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 10, 7)]
            public string ImmediateOriginContactPhoneNumber
            {
                get
                {
                    return ConstantFields.OriginatorContactPhoneNumber;
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 16, 8, true)]
            public string Reserved
            {
                get
                {
                    return "                ";
                }
            }
        }
    }
}
