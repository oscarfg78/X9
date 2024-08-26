using System.Collections.Generic;

// 2 com.maxillc.checks.x9File.Types
namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    public class CashLetterWf
    {
        public Header CashLetterHeader { get; set; }

        public List<BundleWf> Bundles { get; set; }

        public Control CashLetterControl { get; set; }

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
                    return "10";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 2, true)]
            public string CollectionType
            {
                get
                {
                    return "01";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 9, 3)]
            public string DestinationRoutingNumber { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 9, 4)]
            public string ECEInstitutionRoutingNumber { get; set; }

            [SEDFieldAttributte(SEDFieldType.NB, 8, 5)]
            public string CashLetterBusinessDate { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 6)]
            public string CashLetterCreationDate { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 4, 7)]
            public string CashLetterCreationTime { get; set; }

            [SEDFieldAttributte(SEDFieldType.A, 1, 8, true)]
            public string CashLetterRecordType
            {
                get
                {
                    return "I";
                }
            }

            [SEDFieldAttributte(SEDFieldType.AN, 1, 9, true)]
            public string CashLetterDocumentationType
            {
                get
                {
                    return "G";
                }
            }

            [SEDFieldAttributte(SEDFieldType.AN, 8, 10)]
            public string CashLetterId { get; set; }

            [SEDFieldAttributte(SEDFieldType.ANS, 14, 11)]
            public string OriginatorContactName
            {
                get
                {
                    return ConstantFields.OriginatorContactName;
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 10, 12)]
            public string OriginatorContactPhoneNumber
            {
                get
                {
                    return ConstantFields.OriginatorContactPhoneNumber;
                }
            }

            [SEDFieldAttributte(SEDFieldType.AN, 1, 13, true)]
            public string FedWorkType
            {
                get
                {
                    return "C";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 2, 14, true)]
            public string UserField
            {
                get
                {
                    return "  ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 1, 15, true)]
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
                    return "90";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 6, 2)]
            public string BundleCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 3)]
            public string ItemsWithinCashLetterCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 14, 4)]
            public string CashLetterTotalAmount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 9, 5)]
            public string ImagesWithinCashLetterCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.A, 18, 6, true)]
            public string ECEInstitutionName
            {
                get
                {
                    return "                  ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 8, 7, true)]
            public string SettlementDate
            {
                get
                {
                    return "        ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 15, 8, true)]
            public string Reserved
            {
                get
                {
                    return "               ";
                }
            }
        }
    }
}
