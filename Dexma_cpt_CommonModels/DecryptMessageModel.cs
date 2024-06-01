

using System.ComponentModel;

namespace Dexma_cpt_CommonModels
{
    public class DecryptMessageModel : INotifyPropertyChanged
    {
        private int? decryptMessageModelId;
        public int? DecryptMessageModelId
        {
            get { return decryptMessageModelId; }
            set
            {
                decryptMessageModelId = value;
                OnPropertyChanged(nameof(DecryptMessageModelId));
            }
        }

        private string messageData;
        public string MessageData
        {
            get { return messageData; }
            set
            {
                messageData = value;
                OnPropertyChanged(nameof(MessageData));
            }
        }

        private DateTime sendingDateTime;
        public DateTime SendingDateTime
        {
            get { return sendingDateTime; }
            set
            {
                sendingDateTime = value;
                OnPropertyChanged(nameof(SendingDateTime));
            }
        }

        private string messageFrom;
        public string MessageFrom
        {
            get { return messageFrom; }
            set
            {
                messageFrom = value;
                OnPropertyChanged(nameof(MessageFrom));
            }
        }

        private string? isEdited;
        public string? IsEdited
        {
            get { return isEdited; }
            set
            {
                isEdited = value;
                OnPropertyChanged(nameof(IsEdited));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
