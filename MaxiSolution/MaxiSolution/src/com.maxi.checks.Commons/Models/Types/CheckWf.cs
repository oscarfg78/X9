using Boz.Services.Contracts.Utils;
using System;
using System.Diagnostics;

namespace Maxi.Services.SouthSide.SouthsideFile.Types
{
    public class CheckWf : SED
    {
        private IServiceUtils _serviceUtils;
        public CheckWf(ILogger logger, IServiceUtils serviceUtils) : base(logger, serviceUtils)
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
        public string RecordType
        {
            get
            {
                return "25";
            }
        }

        [SEDFieldAttributte(SEDFieldType.NBSM, 15, 2)]
        public string AuxiliaryOnUs { get; set; }

        [SEDFieldAttributte(SEDFieldType.ANS, 1, 3)]
        public string ExternalProcessingCode { get; set; }

        [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
        public string PayorBankRoutingNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.N, 1, 5)]
        public string PayorBankRoutingNumberCheckDigit { get; set; }

        [SEDFieldAttributte(SEDFieldType.NBSM, 20, 6)]
        public string OnUs { get; set; }

        [SEDFieldAttributte(SEDFieldType.N, 10, 7)]
        public string ItemAmount { get; set; }

        [SEDFieldAttributte(SEDFieldType.NB, 15, 8)]
        public string ItemSequenceNumber { get; set; }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 9, true)]
        public string Documentationtype
        {
            get
            {
                return "G";
            }
        }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 10, true)]
        public string ReturnAcceptanceIndicator
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 11, true)]
        public string MICRValidIndicator
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.A, 1, 12, true)]
        public string BOFDIndicator
        {
            get
            {
                return "U";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 2, 13, true)]
        public string CheckDetailRecordAddendumCount
        {
            get
            {
                return "00";
            }
        }

        [SEDFieldAttributte(SEDFieldType.N, 1, 14, true)]
        public string CorrectionIndicator
        {
            get
            {
                return " ";
            }
        }

        [SEDFieldAttributte(SEDFieldType.AN, 1, 15, true)]
        public string ArchiveTypeIndicator
        {
            get
            {
                return " ";
            }
        }

        public CheckAddWf ChecksAdd { get; set; }

        public DetailAndData ImageFrontDetail { get; set; }

        public DetailAndData ImageRearDetail { get; set; }

        public override byte[] GetBinary()
        {
            var tmpBase = base.GetBinary();
            var tmpChecksAdd = ChecksAdd.GetBinary();
            var detailImageFrontBinary = GetBinary(ImageFrontDetail.ImageDetail);
            var dataImageFrontBinary = GetBinary(ImageFrontDetail.ImageData);
            var tmpImageFrontDetail = _serviceUtils.AppendBytes(detailImageFrontBinary, dataImageFrontBinary);
            var detailImageRearBinary = GetBinary(ImageRearDetail.ImageDetail);
            var dataImageRearBinary = GetBinary(ImageRearDetail.ImageData);
            var tmpImageRearDetail = _serviceUtils.AppendBytes(detailImageRearBinary, dataImageRearBinary);
            var output = _serviceUtils.AppendBytes(tmpBase,
                _serviceUtils.AppendBytes(tmpChecksAdd,
                    tmpImageFrontDetail,
                    tmpImageRearDetail
                )
            );
            return output;
        }

        public class DetailAndData
        {
            private bool _tryGetFile = false;
            private int _imageSize = 0;
            private byte[] _image;
            private string _imagePath;
            private ImageSide _imageSide;
            private string _tellerSpray;
            private bool _isReclear;
            private IServiceUtils _serviceUtils;

            public DetailAndData(string imagePath, ImageSide imageSide, bool isReclear, string tellerSpray, IServiceUtils serviceUtils)
            {
                _isReclear = isReclear;
                _imagePath = imagePath;
                _imageSide = imageSide;
                _tellerSpray = tellerSpray;
                ImageDetail = new Detail(new GetImageAction(GetImage), new GetImageSizeAction(GetImageSize));
                ImageData = new Data(new GetImageAction(GetImage), new GetImageSizeAction(GetImageSize));
                _serviceUtils = serviceUtils;   
            }

            public string ImagePath
            {
                get
                {
                    return _imagePath;
                }
            }

            public ImageSide ImageSide
            {
                get
                {
                    return _imageSide;
                }
            }

            //public String TellerSpray
            //{
            //    get { return _tellerSpray; }
            //}

            public bool IsReclear
            {
                get
                {
                    return _isReclear;
                }
            }

            private void LoadImage()
            {
                _tryGetFile = true;
                _image = _serviceUtils.GetCheckFile(ImagePath, ImageSide.Equals(ImageSide.Front) && IsReclear, ref _imageSize, _tellerSpray, ImageSide == ImageSide.Rear);
            }

            private byte[] GetImage()
            {
                if (!_tryGetFile)
                    LoadImage();
                return _image;
            }

            private string GetImageSize()
            {
                if (!_tryGetFile)
                    LoadImage();
                return _imageSize.ToString();
            }

            public Detail ImageDetail { get; private set; }

            public Data ImageData { get; private set; }

            public delegate byte[] GetImageAction();

            public delegate string GetImageSizeAction();

            public class Detail
            {
                private GetImageAction _getImageAction;
                private GetImageSizeAction _getImageSizeAction;

                public Detail(
                  GetImageAction getImageAction,
                  GetImageSizeAction getImageSizeAction)
                {
                    _getImageAction = getImageAction;
                    _getImageSizeAction = getImageSizeAction;
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
                public string RecordType
                {
                    get
                    {
                        return "50";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 2, true)]
                public string ImageIndicator
                {
                    get
                    {
                        return "1";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 9, 3)]
                public string ImageCreatorRoutingNumber { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 8, 4)]
                public string ImageCreatorDate { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 2, 5, true)]
                public string ImageViewFormat
                {
                    get
                    {
                        return "00";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 6, true)]
                public string ImageViewCompressionAlgorithm
                {
                    get
                    {
                        return "00";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 7)]
                public string ImageViewDataSize
                {
                    get
                    {
                        return _getImageSizeAction();
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 8, true)]
                public string ViewSide { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 2, 9, true)]
                public string ViewDescriptor
                {
                    get
                    {
                        return "00";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 10, true)]
                public string DigitalSignatureIndicator
                {
                    get
                    {
                        return "0";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 11, true)]
                public string DigitalSignatureMmethod
                {
                    get
                    {
                        return "  ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 5, 12, true)]
                public string SecurityKeySize
                {
                    get
                    {
                        return "     ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 13, true)]
                public string StartOfProtectedData
                {
                    get
                    {
                        return "       ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 7, 14, true)]
                public string LengthOfProtectedData
                {
                    get
                    {
                        return "       ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 15, true)]
                public string ImageRecreateIndicator
                {
                    get
                    {
                        return " ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 8, 16, true)]
                public string UserField
                {
                    get
                    {
                        return "        ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.B, 15, 17, true)]
                public string Reserved
                {
                    get
                    {
                        return "               ";
                    }
                }
            }

            public class Data
            {
                private GetImageAction _getImageAction;
                private GetImageSizeAction _getImageSizeAction;

                public Data(
                  GetImageAction getImageAction,
                  GetImageSizeAction getImageSizeAction)
                {
                    _getImageAction = getImageAction;
                    _getImageSizeAction = getImageSizeAction;
                }

                [SEDFieldAttributte(SEDFieldType.N, 2, 1, true)]
                public string Data_RecordType
                {
                    get
                    {
                        return "52";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 9, 2)]
                public string Data_ECEInstitutionRoutingNumber { get; set; }

                [SEDFieldAttributte(SEDFieldType.N, 8, 3)]
                public string Data_BundleBusinessDate { get; set; }

                [SEDFieldAttributte(SEDFieldType.AN, 2, 4, true)]
                public string Data_CycleNumber
                {
                    get
                    {
                        return "01";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.NB, 15, 5)]
                public string Data_ItemSequenceNumber { get; set; }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 6, true)]
                public string Data_SecurityOriginatorName
                {
                    get
                    {
                        return "                ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 7, true)]
                public string Data_SecurityAuthenticatorName
                {
                    get
                    {
                        return "                ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.ANS, 16, 8, true)]
                public string Data_SecurityKeyName
                {
                    get
                    {
                        return "                ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 1, 9, true)]
                public string Data_ClippingOrigin
                {
                    get
                    {
                        return "0";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 10, true)]
                public string Data_ClippingCoordinateH1
                {
                    get
                    {
                        return "    ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 11, true)]
                public string Data_ClippingCoordinateH2
                {
                    get
                    {
                        return "    ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 12, true)]
                public string Data_ClippingCoordinateV1
                {
                    get
                    {
                        return "    ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 13, true)]
                public string Data_ClippingCoordinateV2
                {
                    get
                    {
                        return "    ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.N, 4, 14, true)]
                public string Data_LengthOfImageReferenceKey
                {
                    get
                    {
                        return "0   ";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.NB, 5, 16, true)]
                public string Data_LengthOfDigitalSignature
                {
                    get
                    {
                        return "00000";
                    }
                }

                [SEDFieldAttributte(SEDFieldType.NB, 7, 18)]
                public string Data_LengthOfImageData
                {
                    get
                    {
                        return _getImageSizeAction();
                    }
                }

                [SEDFieldAttributte(SEDFieldType.Bin, 0, 19)]
                public byte[] Data_ImageData
                {
                    get
                    {
                        return _getImageAction();
                    }
                }
            }
        }

        public enum ImageSide
        {
            Front,
            Rear,
        }
    }
}
