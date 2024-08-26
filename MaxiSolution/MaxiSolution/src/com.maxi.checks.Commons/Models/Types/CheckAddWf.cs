
namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    public class CheckAddWf : SED
    {
        public CheckAddWf(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils)
        {
        }

        public override string TypeIdentifier
        {
            get
            {
                return string.Format("{0}:{1}", GetType(), BOFDItemSequenceNumber);
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
        public string RecordType
        {
            get
            {
                return "26";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 2)]
        public string CheckDetailAddendumARecordNumber
        {
            get
            {
                return "1";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 9, 3)]
        public string BOFDRoutingNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
        public string BOFDBussinessDate 
        { 
            get
            {
                return "        ";
            } 
        }

        [SEDFieldAttributte(SEDFieldType.NB, 15, 5)]
        public string BOFDItemSequenceNumber { get; set;}

        [SEDFieldAttributte(SEDFieldType.ANS, 18, 6)]
        public string BOFDDepositAccountNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.ANS, 5, 7)]
        public string BOFDDepositBranch
        {
            get
            {
                return "     ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.ANS, 15, 8)]
        public string PayeeName
        {
            get
            {
                return "SouthSide Bank";
            }
        }

        [SEDFieldAttributte(SEDFieldType.A, 1, 9, true)]
        public string TruncationIndicator
        {
            get
            {
                return "Y";
            }
        }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 10, true)]
        public string ConversionIndicator
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 11, true)]
        public string BOFDCorrectionIndicator
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.ANS, 1, 12, true)]
        public string UserField
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.B, 3, 13, true)]
        public string Reserved
        {
            get
            {
                return "   "; 
            }
        }                
    }
}
