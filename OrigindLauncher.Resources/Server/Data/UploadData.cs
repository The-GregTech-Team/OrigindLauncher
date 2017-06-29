namespace OrigindLauncher.Resources.Server
{
    public class UploadData
    {
        public string Text;

        public UploadData(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator string(UploadData data)
        {
            return data.Text;
        }

        public static implicit operator UploadData(string data)
        {
            return new UploadData(data);
        }
    }
}