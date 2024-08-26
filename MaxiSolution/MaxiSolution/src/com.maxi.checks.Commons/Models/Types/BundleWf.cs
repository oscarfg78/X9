using System.Collections.Generic;

namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    public class BundleWf
    {
        public Header BundleHeader { get; set; }

        public CreditWf Credit { get; set; }

        public List<CheckWf> Checks { get; set; }

        public Control BundleControl { get; set; }

        public class Header : SED
        {
            public Header(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils) { }

            public override string TypeIdentifier
            {
                get
                {
                    return string.Format("{0}:{1}", GetType(), BundleSequenceNumber);
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
            public string RecordType
            {
                get
                {
                    return "20";
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
            public string BundleBusinessDate { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 8, 6)]
            public string BundleCreationDate { get; set; }

            [SEDFieldAttributte(SEDFieldType.AN, 10, 7)]
            public string BundleId { get; set; }

            [SEDFieldAttributte(SEDFieldType.NB, 4, 8)]
            public string BundleSequenceNumber { get; set; }

            [SEDFieldAttributte(SEDFieldType.AN, 2, 9, true)]
            public string CycleNumber
            {
                get
                {
                    return "01";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 9, 10, true)]
            public string ReturnLocationRoutingNumber
            {
                get
                {
                    return "         ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 5, 11, true)]
            public string UserField
            {
                get
                {
                    return "     ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 12, 12, true)]
            public string Reserved
            {
                get
                {
                    return "            ";
                }
            }
        }

        public class Control : SED
        {
            public Control(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils) { }

            public override string TypeIdentifier
            {
                get
                {
                    return string.Format("{0}:{1}", GetType(), BundleSequenceNumber);
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
            public string RecordType
            {
                get
                {
                    return "70";
                }
            }

            [SEDFieldAttributte(SEDFieldType.N, 4, 2)]
            public string ItemsWithinBundleCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 12, 3)]
            public string BundleTotalAmount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 12, 4)]
            public string MICRValidTotalAmount { get; set; }

            [SEDFieldAttributte(SEDFieldType.N, 5, 5)]
            public string ImagesWithinBundleCount { get; set; }

            [SEDFieldAttributte(SEDFieldType.B, 20, 6, true)]
            public string UserField
            {
                get
                {
                    return "                    ";
                }
            }

            [SEDFieldAttributte(SEDFieldType.B, 25, 7, true)]
            public string Reserved
            {
                get
                {
                    return "                         ";
                }
            }

            public string BundleSequenceNumber { get; set; }
        }
    }
}
