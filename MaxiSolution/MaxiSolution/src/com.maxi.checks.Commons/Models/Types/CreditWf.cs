using Boz.Services.Contracts.Utils;
using System;
using System.Diagnostics;

namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    /*
        This class refers to record type 61 (CREDIT Type 61), 
        modified to be used as Type 25 at the request of the SouthSide Bank and JackHenry.
        Do not use for Type 61 printing.
     */
    public class CreditWf : SED
    {
        private IServiceUtils _serviceUtils;
        public CreditWf(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils)
        {
            _serviceUtils = serviceUtils;
        }

        public override string TypeIdentifier
        {
            get
            {
                return string.Format("{0}:{1}", GetType(), ItemSequenceNumber);
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 2, 1)]
        public String RecordType
        {
            get { return "25"; }
        }

        [SEDFieldAttributte(SEDFieldType.NBSM, 15, 2)]
        public String AuxiliaryOnUs { get; set; }

        [SEDFieldAttributte(SEDFieldType.ANS, 1, 3, true)]
        public String ExternalProcessingCode
        {
            get { return " "; }
        }

        [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
        public String PayorBankRoutingNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.N, 1, 5)]
        public String PayorBankRoutingNumberCheckDigit { get; set; }

        //[SEDFieldAttributte(BTEDFieldType.N, 8, 4)]
        //public String PayorBankRoutingNumber
        //{
        //    get { return ConstantFields.DestinationRoutingNumber.Substring(0, 8); }
        //}

        //[SEDFieldAttributte(BTEDFieldType.N, 1, 5)]
        //public String PayorBankRoutingNumberCheckDigit
        //{
        //    get { return ConstantFields.DestinationRoutingNumber.Substring(8, 1); }
        //}

        [SEDFieldAttributte(SEDFieldType.NBSM, 20, 6)]
        public String OnUs { get; set; }

        //[BTEDFieldAttributte(BTEDFieldType.NBSM, 20, 6)]
        //public String OnUs
        //{
        //    get { return ConstantFields.OriginRoutingNumber; }
        //}

        [SEDFieldAttributte(SEDFieldType.N, 10, 7)]
        public String ItemAmount { get; set; }

        [SEDFieldAttributte(SEDFieldType.NB, 15, 8)]
        public String ItemSequenceNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 9, true)]
        public String Documentationtype
        {
            get { return "G"; }
        }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 10, true)]
        public String ReturnAcceptanceIndicator
        {
            get { return " "; }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 11, true)]
        public String MICRValidIndicator
        {
            get { return " "; }
        }

        [SEDFieldAttributte(SEDFieldType.A, 1, 12, true)]
        public String BOFDIndicator
        {
            get { return "U"; }
        }

        [SEDFieldAttributte(SEDFieldType.N, 2, 13, true)]
        public String CheckDetailRecordAddendumCount
        {
            get { return "00"; }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 14, true)]
        public String CorrectionIndicator
        {
            get { return " "; }
        }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 15, true)]
        public String ArchiveTypeIndicator
        {
            get { return " "; }
        }

        //---------------------------
        public DetailAndData ImageFrontDetail { get; set; }

        public DetailAndData ImageRearDetail { get; set; }

        public override byte[] GetBinary()
        {
            var imageFrontTemp = _serviceUtils.AppendBytes(GetBinary(ImageFrontDetail.ImageDetail), GetBinary(ImageFrontDetail.ImageData));
            var imageRearTemp = _serviceUtils.AppendBytes(GetBinary(ImageRearDetail.ImageDetail), GetBinary(ImageRearDetail.ImageData));
            var output = _serviceUtils.AppendBytes(base.GetBinary(), imageFrontTemp, imageRearTemp);
            return output;
        }


        public class DetailAndData
        {
            private bool _tryGetFile = false;

            private int _imageSize = 0;
            private byte[] _image;
            private String _imagePath;
            private ImageSide _imageSide;

            public delegate byte[] GetImageAction();

            public delegate String GetImageSizeAction();

            public String ImagePath
            {
                get { return _imagePath; }
            }

            public ImageSide ImageSide
            {
                get { return _imageSide; }
            }

            private IServiceUtils _serviceUtils;
            public DetailAndData(String imagePath, ImageSide imageSide, IServiceUtils serviceUtils)
            {
                _imagePath = imagePath;
                _imageSide = imageSide;
                ImageDetail = new Detail(GetImage, GetImageSize);
                ImageData = new Data(GetImage, GetImageSize);
                _serviceUtils = serviceUtils;
                
            }

            private void LoadImage()
            {
                _tryGetFile = true;
                _image = _serviceUtils.GetCashLetterFile(ImagePath, ImageSide == ImageSide.Front, ImageDataAgentName,
                    ImageDataResume, ImageDataCheckDetail, ref _imageSize);
            }

            private byte[] GetImage()
            {
                if (!_tryGetFile)
                {
                    LoadImage();
                }
                return _image;
            }

            private String GetImageSize()
            {
                if (!_tryGetFile)
                {
                    LoadImage();
                }
                return _imageSize.ToString();
            }

            public String ImageDataAgentName { get; set; }
            public String ImageDataResume { get; set; }
            public String ImageDataCheckDetail { get; set; }

            public Detail ImageDetail { get; private set; }

            public Data ImageData { get; private set; }


            public class Detail
            {
                private GetImageAction _getImageAction;
                private GetImageSizeAction _getImageSizeAction;

                public Detail(GetImageAction getImageAction, GetImageSizeAction getImageSizeAction)
                {
                    _getImageAction = getImageAction;
                    _getImageSizeAction = getImageSizeAction;
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
                public String RecordType
                {
                    get { return "50"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 2, true)]
                public String ImageIndicator
                {
                    get { return "1"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 9, 3)]
                public String ImageCreatorRoutingNumber { get; set; }

                //[SEDFieldAttributte(SEDFieldType.N, 9, 3)]
                //public String ImageCreatorRoutingNumber
                //{
                //    get { return ConstantFields.DestinationRoutingNumber; }
                //}


                [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
                public String ImageCreatorDate { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 2, 5, true)]
                public String ImageViewFormat
                {
                    get { return "00"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 6, true)]
                public String ImageViewCompressionAlgorithm
                {
                    get { return "00"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 7)]
                public String ImageViewDataSize
                {
                    get { return _getImageSizeAction(); }
                }



                [SEDFieldAttributte(SEDFieldType.N, 1, 8, true)]
                public String ViewSide { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 2, 9, true)]
                public String ViewDescriptor
                {
                    get { return "00"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 10, true)]
                public String DigitalSignatureIndicator
                {
                    get { return "0"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 11, true)]
                public String DigitalSignatureMmethod
                {
                    get { return "  "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 5, 12, true)]
                public String SecurityKeySize
                {
                    get { return "     "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 13, true)]
                public String StartOfProtectedData
                {
                    get { return "       "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 14, true)]
                public String LengthOfProtectedData
                {
                    get { return "       "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 15, true)]
                public String ImageRecreateIndicator
                {
                    get { return " "; }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 8, 16, true)]
                public String UserField
                {
                    get { return "        "; }
                }

                [SEDFieldAttributte(SEDFieldType.B, 15, 17, true)]
                public String Reserved
                {
                    get { return "               "; }
                }

            }

            public class Data
            {

                private GetImageAction _getImageAction;
                private GetImageSizeAction _getImageSizeAction;

                public Data(GetImageAction getImageAction, GetImageSizeAction getImageSizeAction)
                {
                    _getImageAction = getImageAction;
                    _getImageSizeAction = getImageSizeAction;
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
                public String Data_RecordType
                {
                    get { return "52"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 9, 2)]
                public String Data_ECEInstitutionRoutingNumber { get; set; }



                //[SEDFieldAttributte(SEDFieldType.N, 9, 2)]
                //public String Data_ECEInstitutionRoutingNumber
                //{
                //    get { return ConstantFields.OriginRoutingNumber; }
                //}


                [SEDFieldAttributte(SEDFieldType.N, 8, 3)]
                public string Data_BundleBusinessDate { get; set; }

                [SEDFieldAttributte(SEDFieldType.AN, 2, 4, true)]
                public String Data_CycleNumber
                {
                    get { return "01"; }
                }

                [SEDFieldAttributte(SEDFieldType.NB, 15, 5)]
                public string Data_ItemSequenceNumber { get; set; }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 6, true)]
                public String Data_SecurityOriginatorName
                {
                    get { return "                "; }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 7, true)]
                public String Data_SecurityAuthenticatorName
                {
                    get { return "                "; }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 8, true)]
                public String Data_SecurityKeyName
                {
                    get { return "                "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 9, true)]
                public String Data_ClippingOrigin
                {
                    get { return "0"; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 10, true)]
                public String Data_ClippingCoordinateH1
                {
                    get { return "    "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 11, true)]
                public String Data_ClippingCoordinateH2
                {
                    get { return "    "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 12, true)]
                public String Data_ClippingCoordinateV1
                {
                    get { return "    "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 13, true)]
                public String Data_ClippingCoordinateV2
                {
                    get { return "    "; }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 14, true)]
                public String Data_LengthOfImageReferenceKey
                {
                    get { return "0   "; }
                }

                //[SEDFieldAttributte(SEDFieldType.NB, 0, EndDetailFields + 15)]
                //public String Data_ImageReferenceKey
                //{
                //    get { return String.Empty; }
                //}

                [SEDFieldAttributte(SEDFieldType.NB, 5, 16, true)]
                public String Data_LengthOfDigitalSignature
                {
                    get { return "00000"; }
                }

                //[SEDFieldAttributte(SEDFieldType.N, 0, EndDetailFields + 17)]
                //public String Data_DigitalSignature
                //{
                //    get { return String.Empty; }
                //}

                [SEDFieldAttributte(SEDFieldType.NB, 7, 18)]
                public String Data_LengthOfImageData
                {
                    get { return _getImageSizeAction(); }
                }

                [SEDFieldAttributte(SEDFieldType.Bin, 0, 19)]
                public byte[] Data_ImageData
                {
                    get { return _getImageAction(); }
                }

            }


        }

        public enum ImageSide
        {
            Front,
            Rear
        }

    }
}
